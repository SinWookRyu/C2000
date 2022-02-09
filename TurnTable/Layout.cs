using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace CytoDx
{
    public partial class MainWindow
    {
        private const int CNT_TIP_PNT = 84;
        private const int CNT_TUBE_PNT = 10;
        private const int CNT_COOL_PNT = 12;

        public double[,] dbTip_PntPos = new double[CNT_TIP_PNT, 2]
        { 
           //L 1mL Tip (11EA: IP1 ~ IP11)
          { 0.0,  0.0 }, { 12.0, 0.0  }, { 24.0,  0.0 },  { 36.0,  0.0 },
          { 0.0, 12.0 }, { 12.0, 12.0 }, { 24.0, 12.0 }, { 36.0, 12.0 },
          { 0.0, 24.0 }, { 12.0, 24.0 }, { 24.0, 24.0 },

          //10uL Tip (6EA: IP12 ~ IP17)
          { 55.0,  0.0 }, { 67.0,  0.0 },
          { 55.0, 12.0 }, { 67.0, 12.0 },
          { 55.0, 24.0 }, { 67.0, 24.0 },

          //300uL Tip (12EA: IP18 ~ IP29)
          { 86.0,  0.0 }, { 98.0,  0.0  }, { 110.0,  0.0 },
          { 86.0, 12.0 }, { 98.0, 12.0  }, { 110.0, 12.0 },
          { 86.0, 24.0 }, { 98.0, 24.0  }, { 110.0, 24.0 },
          { 86.0, 36.0 }, { 98.0, 36.0  }, { 110.0, 36.0 },

          // 1mL Tip (20EA: IP30 ~ IP49)
          { 0.0, 52.0 }, { 12.0, 52.0 }, { 24.0, 52.0 }, { 36.0, 52.0 }, { 48.0, 52.0 }, { 60.0, 52.0 }, { 72.0, 52.0 }, { 84.0, 52.0 }, { 96.0, 52.0 }, { 108.0, 52.0 },
          { 0.0, 64.0 }, { 12.0, 64.0 }, { 24.0, 64.0 }, { 36.0, 64.0 }, { 48.0, 64.0 }, { 60.0, 64.0 }, { 72.0, 64.0 }, { 84.0, 64.0 }, { 96.0, 64.0 }, { 108.0, 64.0 },

          // 5mL Tip (35EA: IP50 ~ IP84)
          { 1.0,  84.0 }, { 19.0,  84.0 }, { 37.0,  84.0 }, { 55.0,   84.0 }, { 73.0,   84.0 }, { 91.0,   84.0 }, { 109.0, 84.0 },
          { 1.0, 102.0 }, { 19.0, 102.0 }, { 37.0, 102.0 }, { 55.0,  102.0 }, { 73.0,  102.0 }, { 91.0,  102.0 }, { 109.0,102.0 },
          { 1.0, 120.0 }, { 19.0, 120.0 }, { 37.0, 120.0 }, { 55.0,  120.0 }, { 73.0,  120.0 }, { 91.0,  120.0 }, { 109.0,120.0 },
          { 1.0, 138.0 }, { 19.0, 138.0 }, { 37.0, 138.0 }, { 55.0,  138.0 }, { 73.0,  138.0 }, { 91.0,  138.0 }, { 109.0,138.0 },
          { 1.0, 156.0 }, { 19.0, 156.0 }, { 37.0, 156.0 }, { 55.0,  156.0 }, { 73.0,  156.0 }, { 91.0,  156.0 }, { 109.0,156.0 },
        };
        public double[,] dbTube_PntPos = new double[CNT_TUBE_PNT, 2]
        {
          //50mL Tip (6EA: RP1 ~ RP6)
          {  0.0,  0.0 }, {   0.0, 38.0  }, {   0.0,  76.0 },
          { 38.0,  0.0 }, { 38.0, 38.0  }, { 38.0,  76.0 },
          //15mL Tip (4EA: RP7 ~ RP10)
          { 72.0,  -5.0 }, { 72.0, 25.0  }, { 72.0, 55.0 },  { 72.0, 85.0 },
        };
        public double[,] dbCooling_PntPos = new double[CNT_COOL_PNT, 2]
        {
          //1.5mL Tip (4EA: CP1 ~ CP4)
          { 0.0,  0.0 }, { 0.0, 34.0  }, { 0.0, 68.0 },  { 0.0, 102.0 },
          //50mL Tip (4EA: CP5 ~ CP8)
          { 22.0,  2.0 }, { 22.0, 36.0  }, { 22.0,  70.0 },  { 22.0,  104.0 },
          //15mL Tip (4EA: CP9 ~ CP12)
          { 58.5,  -14.0 }, { 12.0, 20.0  }, { 24.0,  54.0 },  { 36.0,  88.0 },
        };

        private void RemoveRowContainIndexChar(string idxChar)
        {
            for (int i = 0; i < DV_World_T_Pnt.Rows.Count; i++)
            {
                String Value = DV_World_T_Pnt.Rows[i].Cells[0].Value as string;

                if ((Value != null) && Value.Contains(idxChar) == true)
                {
                    DV_World_T_Pnt.Rows.Remove(DV_World_T_Pnt.Rows[i]);
                    i--;
                }
            }
            
            DV_World_T_Pnt.Refresh();

            //if (idxChar == "IP")
            //{
            //    DV_World_T_Pnt.Rows.Add(CNT_TIP_PNT);
            //}
            //else if (idxChar == "RP")
            //{
            //    DV_World_T_Pnt.Rows.Add(CNT_TUBE_PNT);
            //}
            //else if (idxChar == "CP")
            //{
            //    DV_World_T_Pnt.Rows.Add(CNT_COOL_PNT);
            //}
        }

        private void btnSetTipBasePos_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to apply current XY position to Tip Base Point??",
                                                    "Data Applied!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                ReadMotorPosition(true);
                Thread.Sleep(50);

                label_Tip_BasePosX.Text = CurrentPos.Step0AxisX.ToString("F2");
                label_Tip_BasePosY.Text = CurrentPos.Step1AxisY.ToString("F2");
            }
            else
            {
                return;
            }
        }

        private void btnSetTipOffset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to confirm Tip Origin Point??",
                                                    "Data Applied!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                label_Tip_OrgPosX.Text = string.Format("{0:0.00}", double.Parse(label_Tip_BasePosX.Text) + 
                                                                        double.Parse(edit_Tip_OffsetX.Text));
                label_Tip_OrgPosY.Text = string.Format("{0:0.00}", double.Parse(label_Tip_BasePosY.Text) + 
                                                                        double.Parse(edit_Tip_OffsetY.Text));
            }
            else
            {
                return;
            }
        }

        private void btnGenerateTipPoint_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to generate Tip Teaching Point (IP1~IP84)??",
                                                    "Data Generated!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                ;
            }
            else
            {
                return;
            }

            try
            {
                int idx = 0;

                string[] tmpZPosArray = new string[CNT_TIP_PNT];
                string[] tmpGripPosArray = new string[CNT_TIP_PNT];
                string[] tmpPipettPosArray = new string[CNT_TIP_PNT];

                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    if (DV_World_T_Pnt.Rows[i].Cells[1].Value.ToString().Contains("Tip") == true)
                    {
                        tmpZPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[4].Value.ToString();
                        tmpGripPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[5].Value.ToString();
                        tmpPipettPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[6].Value.ToString();
                        idx = idx + 1;
                    }
                }

                RemoveRowContainIndexChar("IP");

                int start_row = DV_World_T_Pnt.Rows.Count - 1;
                DV_World_T_Pnt.Rows.Add(CNT_TIP_PNT);

                for (int i = 0; i < CNT_TIP_PNT; i++)
                {
                    //DV_World_T_Pnt.Rows.Add();
                    DV_World_T_Pnt.Rows[start_row + i].Cells[0].Value = "IP" + (i + 1).ToString();
                    DV_World_T_Pnt.Rows[start_row + i].Cells[1].Value = "Tip(Auto)";
                    DV_World_T_Pnt.Rows[start_row + i].Cells[2].Value = (double.Parse(label_Tip_OrgPosX.Text) - dbTip_PntPos[i, 0]).ToString("F2");
                    DV_World_T_Pnt.Rows[start_row + i].Cells[3].Value = (double.Parse(label_Tip_OrgPosY.Text) - dbTip_PntPos[i, 1]).ToString("F2");

                    if (tmpZPosArray[i] == null)
                         DV_World_T_Pnt.Rows[start_row + i].Cells[4].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[4].Value = tmpZPosArray[i];

                    if (tmpGripPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[5].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[5].Value = tmpGripPosArray[i];

                    if (tmpPipettPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[6].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[6].Value = tmpPipettPosArray[i];
                }

                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    HighlightOrgPointRow(i);
                }

                DV_World_T_Pnt.Refresh();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }

            MessageBox.Show("Data Generated Done!!", "Completed", MessageBoxButtons.OK);
        } 

        private void btnSetTubeBasePos_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to apply current XY position to Tube Base Point??",
                                                    "Data Applied!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                ReadMotorPosition(true);
                Thread.Sleep(50);

                label_Tube_BasePosX.Text = CurrentPos.Step0AxisX.ToString("F2");
                label_Tube_BasePosY.Text = CurrentPos.Step1AxisY.ToString("F2");
            }
            else
            {
                return;
            }
        }

        private void btnSetTubeOffset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to confirm Tube Origin Point??",
                                                    "Data Applied!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                label_Tube_OrgPosX.Text = string.Format("{0:0.00}", double.Parse(label_Tube_BasePosX.Text) + 
                                                                            double.Parse(edit_Tube_OffsetX.Text));
                label_Tube_OrgPosY.Text = string.Format("{0:0.00}", double.Parse(label_Tube_BasePosY.Text) + 
                                                                            double.Parse(edit_Tube_OffsetY.Text));
            }
            else
            {
                return;
            }
        }

        private void btnGenerateTubePoint_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to generate Tube Teaching Point (RP1~RP10)??",
                                                    "Data Generated!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                ;
            }
            else
            {
                return;
            }

            try
            {
                int idx = 0;
                string[] tmpZPosArray = new string[CNT_TUBE_PNT];
                string[] tmpGripPosArray = new string[CNT_TUBE_PNT];
                string[] tmpPipettPosArray = new string[CNT_TUBE_PNT];

                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    if (DV_World_T_Pnt.Rows[i].Cells[1].Value.ToString().Contains("Tube") == true)
                    {
                        tmpZPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[4].Value.ToString();
                        tmpGripPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[5].Value.ToString();
                        tmpPipettPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[6].Value.ToString();
                        idx = idx + 1;
                    }
                }

                RemoveRowContainIndexChar("RP");
                
                int start_row = DV_World_T_Pnt.Rows.Count - 1;
                DV_World_T_Pnt.Rows.Add(CNT_TUBE_PNT);

                for (int i = 0; i < CNT_TUBE_PNT; i++)
                {
                    //DV_World_T_Pnt.Rows.Add();
                    DV_World_T_Pnt.Rows[start_row + i].Cells[0].Value = "RP" + (i + 1).ToString();
                    DV_World_T_Pnt.Rows[start_row + i].Cells[1].Value = "Tube(Auto)";
                    DV_World_T_Pnt.Rows[start_row + i].Cells[2].Value = double.Parse(label_Tube_OrgPosX.Text) - dbTube_PntPos[i, 0];
                    DV_World_T_Pnt.Rows[start_row + i].Cells[3].Value = double.Parse(label_Tube_OrgPosY.Text) - dbTube_PntPos[i, 1];

                    if (tmpZPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[4].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[4].Value = tmpZPosArray[i];

                    if (tmpGripPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[5].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[5].Value = tmpGripPosArray[i];

                    if (tmpPipettPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[6].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[6].Value = tmpPipettPosArray[i];
                }

                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    HighlightOrgPointRow(i);
                }

                DV_World_T_Pnt.Refresh();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }

            MessageBox.Show("Data Generated Done!!", "Completed", MessageBoxButtons.OK);
        }

        private void btnSetCoolingBasePos_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to apply current XY position to Tip Base Point??",
                                                    "Data Applied!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                ReadMotorPosition(true);
                Thread.Sleep(50);

                label_Cooling_BasePosX.Text = CurrentPos.Step0AxisX.ToString("F2");
                label_Cooling_BasePosY.Text = CurrentPos.Step1AxisY.ToString("F2");
            }
            else
            {
                return;
            }
        }

        private void btnSetCoolingOffset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to confirm Cooler Origin Point??",
                                                    "Data Applied!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                label_Cooling_OrgPosX.Text = string.Format("{0:0.00}", double.Parse(label_Cooling_BasePosX.Text) + 
                                                                              double.Parse(edit_Cooling_OffsetX.Text));
                label_Cooling_OrgPosY.Text = string.Format("{0:0.00}", double.Parse(label_Cooling_BasePosY.Text) + 
                                                                              double.Parse(edit_Cooling_OffsetY.Text));
            }
            else
            {
                return;
            }
        }

        private void btnGenerateCoolingPoint_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to generate Cooler Teaching Point (CP1~CP12)??",
                                                    "Data Generated!!", MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                ;
            }
            else
            {
                return;
            }

            try
            {
                int idx = 0;
                string[] tmpZPosArray = new string[CNT_COOL_PNT];
                string[] tmpGripPosArray = new string[CNT_COOL_PNT];
                string[] tmpPipettPosArray = new string[CNT_COOL_PNT];

                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    if (DV_World_T_Pnt.Rows[i].Cells[1].Value.ToString().Contains("Cooler") == true)
                    {
                        tmpZPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[4].Value.ToString();
                        tmpGripPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[5].Value.ToString();
                        tmpPipettPosArray[idx] = DV_World_T_Pnt.Rows[i].Cells[6].Value.ToString();
                        idx = idx + 1;
                    }
                }

                RemoveRowContainIndexChar("CP");

                int start_row = DV_World_T_Pnt.Rows.Count - 1;
                DV_World_T_Pnt.Rows.Add(CNT_COOL_PNT);

                for (int i = 0; i < CNT_COOL_PNT; i++)
                {
                    //DV_World_T_Pnt.Rows.Add();
                    DV_World_T_Pnt.Rows[start_row + i].Cells[0].Value = "CP" + (i + 1).ToString();
                    DV_World_T_Pnt.Rows[start_row + i].Cells[1].Value = "Cooler(Auto)";
                    DV_World_T_Pnt.Rows[start_row + i].Cells[2].Value = double.Parse(label_Cooling_OrgPosX.Text) - dbCooling_PntPos[i, 0];
                    DV_World_T_Pnt.Rows[start_row + i].Cells[3].Value = double.Parse(label_Cooling_OrgPosY.Text) - dbCooling_PntPos[i, 1];

                    if (tmpZPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[4].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[4].Value = tmpZPosArray[i];

                    if (tmpGripPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[5].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[5].Value = tmpGripPosArray[i];

                    if (tmpPipettPosArray[i] == null)
                        DV_World_T_Pnt.Rows[start_row + i].Cells[6].Value = "0";
                    else
                        DV_World_T_Pnt.Rows[start_row + i].Cells[6].Value = tmpPipettPosArray[i];
                }

                for (int i = 0; i < DV_World_T_Pnt.Rows.Count - 1; i++)
                {
                    HighlightOrgPointRow(i);
                }

                DV_World_T_Pnt.Refresh();
            }
            catch (Exception ex)
            {
                iPrintf(ex.Message.ToString());
            }

            MessageBox.Show("Data Generated Done!!", "Completed", MessageBoxButtons.OK);
        }
    }
}
