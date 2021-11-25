//#define IMG_DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.UserInterface;
using OpenCvSharp.Blob;
using OpenCvSharp.CPlusPlus;
namespace CytoDx
{
    public partial class MainWindow
    {
        public Mat src_img = new Mat();
        public Mat dst_img = new Mat();
        public static bool fTargetCond = false;
        public static Rect boundingRect;
        public static RotatedRect rotatedRect;
        public Point[][] contour;
        public HierarchyIndex[] hierarchy;
        public double BaseUpperLinePos = 0;
        public double PBMCLowerLinePos = 0;

        Cal_param c = new Cal_param();
        /* Calibration이 필요한 parameter */
        public class Cal_param
        {
            /* ROI 영역 설정, 카메라로 획득되는 이미지에 맞게 조정 필요 */
            public struct roi_param
            {
                public int x;
                public int y;
                public int width;
                public int height;
            }

            /* chamber 영역 기준이 되는 사각 위치 계산을 위한 RGB 하한 경계값 */
            public struct lowerbound_base
            {
                public int B;
                public int G;
                public int R;
            }

            /* chamber 영역 기준이 되는 사각 위치 계산을 위한 RGB 상한 경계값 */
            public struct upperbound_base
            {
                public int B;
                public int G;
                public int R;
            }

            /* PBMC 영역 사각 위치 계산을 위한 RGB 하한 경계값 설정 */
            public struct lowerbound_pbmc
            {
                public int B;
                public int G;
                public int R;
            }

            /* PBMC 영역 사각 위치 계산을 위한 RGB 상한 경계값 설정 */
            public struct upperbound_pbmc
            {
                public int B;
                public int G;
                public int R;
            }

            /* pixel단위로 획득된 이미지값을 mm로 환산하기 위해 calibration 필요함 */
            public double ypixel = 0.0848;
            /* chamber의 기준위치 이미지 처리 오류를 방지하기 위해 default값을 정하여 적용 (pixel) */
            public int RectHeightDefault = 82;
        }

        public Mat SetROI(Mat src, int x, int y, int width, int height)  // ROI 영역 설정, 카메라로 획득되는 이미지에 맞게 조정 필요
        {
            Mat result = new Mat();

            /* 필요시 이미지 크기를 조정하여 분석 */
            /*Size size = new Size(src.Width * 2, src.Height * 2);
            Cv2.Resize(src, src, size);
            Cv2.ImShow("resize", src);*/

            /* 보고 싶은 영역(ROI) 설정 */
            Mat roi = new Mat();
            Rect rect = new Rect(x, y, width, height);
            roi = src.SubMat(rect);
#if IMG_DEBUG
            Cv2.ImShow("ROI rect", roi);
#endif
            result = roi.Clone();

            return result;
        }

        public Mat FindRectBound(
            int L_B, int L_G, int L_R,  // Lower bound value, 획득되는 이미지에 맞게 민감하게 조정이 필요함
            int U_B, int U_G, int U_R,  // Upper bound value, 획득되는 이미지에 맞게 민감하게 조정이 필요함
            double ypixel, //픽셀당 거리값을 측정해서 캘리브레이션을 위해 사용
            Mat src, Mat dst, bool fDrawRect, bool fDrawContour, bool fDrawApprox)
        {
            Mat hsv = new Mat();
            Mat gray = new Mat();
            Mat canny = new Mat();
            Mat blur = new Mat();
            dst = src.Clone();

            /* color을 HSV모드로 변경 */
            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);
#if IMG_DEBUG
            Cv2.ImShow("hsv", hsv);
#endif

            /* HSV 이미지를 흑백으로 변경, BGR(RGB) 값은 획득되는 이미지에 맞게 민감하게 조정이 필요함 */
            Cv2.InRange(hsv, new Scalar(L_B, L_G, L_R), new Scalar(U_B, U_G, U_R), gray);
#if IMG_DEBUG
            Cv2.ImShow("gray", gray);
#endif
            dst = gray.Clone();

            /* Gaussian 필터를 사용해서 이미지를 blur하게 변경 */
            Cv2.GaussianBlur(dst, blur, new Size(3, 3), 1, 0, BorderTypes.Default);
            Cv2.GaussianBlur(dst, blur, new Size(3, 3), 3, 0, BorderTypes.Constant);
#if IMG_DEBUG
            Cv2.ImShow("blur", blur);
#endif
            dst = blur.Clone();

            /* Canny 변환을 통해 에지를 검출 */
            Cv2.Canny(dst, canny, 200, 350, 5, true);
#if IMG_DEBUG
            Cv2.ImShow("canny", canny);
#endif
            dst = canny.Clone();

            /* Canny 변환의 결과를 이용해서 경계선 검출 */
            /* RetrievalModes.External: 외곽 윤곽만 검출, 계층정보구성 X, ApproximationMode.TC89KCOS: 프리먼 체인코드에서 윤곽선 사용 */
            Cv2.FindContours(dst, out contour, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89L1);

            CalRectBound(ypixel, src, dst, fDrawRect, fDrawContour, fDrawApprox);

            return src;
        }

        public bool CalRectBound(double ypixel, //픽셀당 거리값을 측정해서 캘리브레이션을 위해 사용
                                Mat src, Mat dst,
                                bool fDrawRect, bool fDrawContour, bool fDrawApprox)
        {
            double cx = 0, cy = 0, ix = 0, iy = 0;
            int low = 0;
            double leng = 0;
            double area = 0;
            double selected_leng = 0;
            double selected_area = 0;

            for (int i = 0; i < contour.Length; i++)
            {
                leng = Cv2.ArcLength(contour[i], true);
                area = Math.Abs(Cv2.ContourArea(contour[i], true));
#if IMG_DEBUG
                iPrintf(string.Format("[{0}] leng: {1:0.00}, area: {2:0.00}", i, leng, area));
#endif

                /* 경계선의 길이가 일정 조건 이상만 연산에 사용 */
                //if (leng > 100)
                if (leng > 80 && (area > 1 && area < 200))
                {
                    fTargetCond = true;

                    Moments mmt = Cv2.Moments(contour[i]);
                    ix = mmt.M10 / mmt.M00;
                    iy = mmt.M01 / mmt.M00;

                    double temp = Math.Sqrt(Math.Pow(Math.Abs((dst.Width / 2) - ix), 2) + Math.Pow(Math.Abs((dst.Height / 2) - iy), 2));

                        low = i;
                        cx = ix;
                        cy = iy;
                    selected_leng = leng;
                    selected_area = area;
#if IMG_DEBUG
                    iPrintf(string.Format("[selected] leng: {0:0.00}, area: {1:0.00}", selected_leng, selected_area));
#endif
                }
            }

            if (contour.Length != 0)
            {
                /* 추출된 윤곽선을 이미지에 표현함 */
                if (fDrawContour == true)
                {
                    Cv2.DrawContours(src, contour, low, Scalar.Red, 1, LineTypes.AntiAlias);
                }

                if (fTargetCond == true)
                {
                    Point[][] approx = new Point[contour.Length][];

                    /* ApproxPolyDP을 이용해서 윤곽선을 단순화함 */
                    for (int i = 0; i < contour.Length; i++)
                    {
                        double epsilon = 0.001 * Cv2.ArcLength(contour[i], true);
                        approx[i] = Cv2.ApproxPolyDP(contour[i], epsilon, true);
                    }

                    /* 추출된 윤곽선을 단순화하여 이미지에 표현함 */
                    for (int i = 0; i < contour.Length; i++)
                    {
                        if (fDrawApprox == true)
                        {
                            Cv2.DrawContours(src, approx, low, Scalar.DarkCyan, 1, LineTypes.AntiAlias);
                        }
                    }

                    /* 단순화된 윤곽선의 최외곽 사각형을 생성함 */
                    boundingRect = Cv2.BoundingRect(approx[low]);
                    rotatedRect = Cv2.MinAreaRect(approx[low]);

                    if (fDrawRect == true)
                    {
                        Cv2.Rectangle(src, boundingRect, Scalar.Blue, 1);
                        Cv2.Rectangle(dst, boundingRect, Scalar.Blue, 1);
                        //Cv2.Ellipse(src, rotatedRect, Scalar.Red, 1);
                    }
                    iPrintf(string.Format("\nRect Height(pixel): {0}", boundingRect.Height));
                }

                /* 윤곽선의 무게중심에서 이미지의 중심까지 화살표로 표기함 (optional) */
                /*Cv2.ArrowedLine(src, new OpenCvSharp.Point(cx, cy), new OpenCvSharp.Point(src.Width / 2, cy),
                    Scalar.Blue, 1, LineTypes.AntiAlias);
                Cv2.ArrowedLine(src, new OpenCvSharp.Point(src.Width / 2, cy), new OpenCvSharp.Point(src.Width / 2, src.Height / 2),
                    Scalar.Blue, 1, LineTypes.AntiAlias);*/

                /* 사각형의 높이값을 이미지에 문자로 표기함 */
                if (fDrawRect == true)
                {
                    string HeightResult = string.Format("Height: {0:0.00}mm", boundingRect.Height * ypixel);
                    iPrintf(string.Format("Rect Height(mm): {0:0.00}", boundingRect.Height * ypixel));
                    Cv2.PutText(src, HeightResult,
                            new OpenCvSharp.Point(src.Width / 2 + 10, cy - boundingRect.Height + 5),
                            HersheyFonts.HersheySimplex, 0.4, Scalar.Blue, 1);
                }
            }

            iPrintf(string.Format("Contour Idx: {0}, leng: {1:0.00}, area: {2:0.00}", low, selected_leng, selected_area));
            iPrintf(string.Format("Rect X: {0}mm, Y: {1}mm", boundingRect.X * ypixel, boundingRect.Y * ypixel));

            return true;
        }

        private void voidMain(string ImgFileName)
        {
            if (ImgFileName == "" || ImgFileName == null)
                return;
            Cal_param c = new Cal_param();
            Cal_param.roi_param roi = new Cal_param.roi_param();
            Cal_param.lowerbound_base Lbound_base = new Cal_param.lowerbound_base();
            Cal_param.upperbound_base Ubound_base = new Cal_param.upperbound_base();
            Cal_param.lowerbound_pbmc Lbound_pbmc = new Cal_param.lowerbound_pbmc();
            Cal_param.upperbound_pbmc Ubound_pbmc = new Cal_param.upperbound_pbmc();

            double static_chamber_vol = 2826.29;    //mm^3    // chamber 중간 고정 체적 (보수적인 산정)
            double chamber_total_height = 7.92; //mm
            double radius_chamber = 15.45;      //mm
            double measured_chamber_vol = 0;    //mm^3
            double calculated_DGM_vol = 0;      //ml

            /* 분석이 필요한 파일명 설정 */
            //ImgFileName = "chamber3.png";    //chamber1.png, chamber2.png, chamber3.png
            src_img = new Mat(ImgFileName);
            dst_img = src_img.Clone();

            /* roi(Region of Interest) 영역 설정 */
            roi.x = 40; roi.y = 250;
            roi.width = 290; roi.height = 110;

            /* 보고 싶은 영역(ROI) 설정 */
            src_img = SetROI(src_img, roi.x, roi.y, roi.width, roi.height);

            /* chamber 영역 사각계산 흑백 이미지 생성을 위한 하한 경계값 설정 */
            /* HSV 모드에서 이미지 편집툴에서 RGB값을 확인하여 선정함 */
            Lbound_base.B = 0; Lbound_base.G = 105; Lbound_base.R = 115;

            /* chamber 영역 사각계산 흑백 이미지 생성을 위한 상한 경계값 설정 */
            /* HSV 모드에서 이미지 편집툴에서 RGB값을 확인하여 선정함 */
            Ubound_base.B = 35; Ubound_base.G = 145; Ubound_base.R = 155;

            /* chamber Edge 및 chamber 기준 위치값 도출 */
            iPrintf(string.Format("\nStart Chamber Base Line detection\n"));
            src_img = FindRectBound(Lbound_base.B, Lbound_base.G, Lbound_base.R,
                                        Ubound_base.B, Ubound_base.G, Ubound_base.R,
                                        c.ypixel,
                                        src_img, dst_img, false, false, false);

            /* chamber의 하단 기준점의 위치(사각의 상단)를 mm로 환산 (BaseUpperLinePos) */
            /* chamber의 기준위치를 이미지에서 도출하는 것에 대한 오류를 방지하기 위해 
             * default값을 정하여 적용함 향후 이미지 처리를 적용할지 여부는 여러가지 샘플을 실험하여 확정할 필요가 있음 */
            if (boundingRect.Y < 85 && boundingRect.Y > 80)
            {
                iPrintf(string.Format("\nBase Line : {0} pixel", boundingRect.Y));
            }
            else
            {
                iPrintf(string.Format("\nBase Line Detection Fail! Set to Default Value!({0} -> {1})", boundingRect.Y, c.RectHeightDefault));
                boundingRect.Y = c.RectHeightDefault;
            }
            BaseUpperLinePos = boundingRect.Y * c.ypixel;

            /* PBMC 영역 사각계산 흑백 이미지 생성을 위한 하한 경계값 설정 */
            Lbound_pbmc.B = 15; Lbound_pbmc.G = 80; Lbound_pbmc.R = 175;   //23, 101, 192   //23, 106, 190  //17, 96, 164

            /* PBMC 영역 사각계산 흑백 이미지 생성을 위한 상한 경계값 설정 */
            Ubound_pbmc.B = 55; Ubound_pbmc.G = 120; Ubound_pbmc.R = 210;

            /* PBMC Edge 및 최하단 위치값 도출 */
            iPrintf(string.Format("\nStart PBMC Lower Line detection\n"));
            src_img = FindRectBound(Lbound_pbmc.B, Lbound_pbmc.G, Lbound_pbmc.R,
                                        Ubound_pbmc.B, Ubound_pbmc.G, Ubound_pbmc.R,
                                        c.ypixel,
                                        src_img, dst_img, true, true, true);

            /* chamber 기준위치와 PBMC 하단까지 거리값을 mm로 환산 (PBMCLowerLinePos) */
            PBMCLowerLinePos = BaseUpperLinePos - ((boundingRect.Y + boundingRect.Height) * c.ypixel);
            if(PBMCLowerLinePos <= 0 || PBMCLowerLinePos >= chamber_total_height)
            {
                iPrintf(string.Format("\nPBMC position detection fail!!! \n(criteria: 0 ~ {0:0.00}, detected: {1:0.00})\n", 
                                chamber_total_height, PBMCLowerLinePos));
                Cv2.WaitKey(0);
                return;
            }
            else
            {
                iPrintf(string.Format("\nPBMC Lower Line detection sucess!!!\n"));
            }

            /* PBMC 하단까지 거리값을 이미지에 출력하고 도출된 거리값의 위치를 화살표로 표기함 */
            iPrintf(string.Format("\n Base Upper Pos: {0}mm\n PBMC Lower Dist: {1}mm", BaseUpperLinePos, PBMCLowerLinePos));
            string DistResult = string.Format("Distance: {0:0.00}mm", PBMCLowerLinePos);

            /* 이미지 처리를 통해 도출된 PBMC 하단 선까지의 거리를 통해 투입해야할 DGM의 체적을 계산함 */
            measured_chamber_vol = Math.Pow(radius_chamber, 2) * Math.PI * (chamber_total_height - PBMCLowerLinePos);
            calculated_DGM_vol = (static_chamber_vol + measured_chamber_vol) / 1000;

            /* 최종 계산된 chamber 기준 위치에서 PBMC 하단 라인까지의 거리값을 이미지에 출력함 */
            Cv2.PutText(src_img, DistResult,
                new OpenCvSharp.Point(roi.width / 6, roi.height / 2 + 15), HersheyFonts.HersheySimplex, 0.4, Scalar.Red, 1);
            Cv2.ArrowedLine(src_img,
                        new OpenCvSharp.Point(roi.width / 2, BaseUpperLinePos / c.ypixel),
                        new OpenCvSharp.Point(roi.width / 2, (boundingRect.Y + boundingRect.Height)),
                        Scalar.Red, 1, LineTypes.AntiAlias);

            /* 최종 계산된 최종 투입할 DGM 체적값을 이미지에 출력함 */
            string VolResult = string.Format("DGM Volume: {0:0.00}ml", calculated_DGM_vol);
            Cv2.PutText(src_img, VolResult,
                new OpenCvSharp.Point(roi.width / 6, roi.height / 2), HersheyFonts.HersheySimplex, 0.4, Scalar.Cyan, 1);
            Cv2.ArrowedLine(src_img,
                        new OpenCvSharp.Point(roi.width / 3, (boundingRect.Y + boundingRect.Height)),
                        new OpenCvSharp.Point(roi.width / 3, 0),
                        Scalar.Cyan, 1, LineTypes.AntiAlias);

            //Console.BackgroundColor = ConsoleColor.DarkBlue;
            //Console.ForegroundColor = ConsoleColor.Cyan;
            iPrintf(string.Format("\n Static Volume: {0:0.00}ml \n Measured Volume: {1:0.00}ml \n Required DGM Volume: {2:0.00}ml",
                            static_chamber_vol / 1000, measured_chamber_vol / 1000, calculated_DGM_vol));
            pictureBox2.Image = src_img.ToBitmap();
            /* 최종 이미지를 출력함 */
            //Cv2.ImShow("src", src_img);

            /* 사용자 키입력을 대기함 */
            //Cv2.WaitKey(0);
        }
    }
}
