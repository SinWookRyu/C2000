//==============================================================================
//		HptDev.cs
//==============================================================================


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Media;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Markup;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Windows.Threading;
using Path = System.IO.Path;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MaterialSkin.Controls;

using HPT_U8 = System.Byte;
using HPT_U16 = System.UInt16;
using HPT_U32 = System.UInt32;
using HPT_U64 = System.UInt64;
using DEVICEID = System.UInt32;
using System.Windows.Forms;
using System.Reflection;


//==============================================================================
//==============================================================================
namespace CytoDx
{
    partial class MainWindow
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("kernel32.dll")]
        static extern bool GetExitCodeProcess(IntPtr hProcess, ref int lpExitCode);
        [DllImport("kernel32.dll")]
        static extern bool WaitForSingleObject(IntPtr hProcess, uint waitforsecond);
        [DllImport("kernel32.dll")]
        static extern bool WaitForInputIdle(IntPtr hProcess, uint waitforsecond);
        [DllImport("kernel32.dll")]
        public static extern void CloseHandle(IntPtr hObject);

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public unsafe string ByteToString(byte[] strByte)
		{
			//return Encoding.Default.GetString(strByte);
			return Encoding.UTF8.GetString(strByte);
		}

		//----------------------------------------------------------------------
		//----------------------------------------------------------------------
		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		//----------------------------------------------------------------------
		//----------------------------------------------------------------------
		static void FromShort(ushort number, out byte byte1, out byte byte2)
		{
			byte2 = (byte)(number >> 8);
			byte1 = (byte)(number & 255);
		}

		//----------------------------------------------------------------------
		//----------------------------------------------------------------------
		public unsafe static string UnsafeByteToString(ushort* pBytes, int size, bool swap = false)
		{
			unsafe
			{
				ushort* pca = pBytes;
				byte[] bytes = new byte[size * 2];
				byte bLo, bHi;
				int index = 0;
				string str;
				for (ushort* counter = pca; *counter != 0; counter++)
				{
					FromShort(*counter, out bLo, out bHi);
					if (swap)
					{
                        if (bHi != 0)
                            bytes[index++] = bHi;
                        if (bLo != 0)
                            bytes[index++] = bLo;
					}
					else
					{
                        if (bLo != 0)
                            bytes[index++] = bLo;
                        if (bHi != 0)
                            bytes[index++] = bHi;					
					}

				}
				str = System.Text.Encoding.ASCII.GetString(bytes, 0, index);
				return str.Trim();
			}
		}


		//----------------------------------------------------------------------
		//----------------------------------------------------------------------
		public unsafe static string UnsafeByteToString(byte* pBytes, int size)
		{
			unsafe
			{
				Byte* pca = pBytes;
				byte[] bytes = new byte[size];
				int index = 0;
				for (byte* counter = pBytes; *counter != 0; counter++)
				{
					bytes[index++] = *counter;
				}
				return System.Text.Encoding.ASCII.GetString(bytes, 0, index);
			}
		}

		//----------------------------------------------------------------------
		//----------------------------------------------------------------------
		static string GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		//----------------------------------------------------------------------
		//----------------------------------------------------------------------
		static byte[] StringToByteArray(string str, int length)
		{
			return Encoding.ASCII.GetBytes(str.PadRight(length, ' '));
		}

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        public void iPrintf(string str, bool bLog = true, bool bNewLine=true, bool bException=false, bool bSilent=false)
		{
			try
			{
				if (bShudown == true || str == "" || bSilent == true)
					return;

				this.Invoke(new MethodInvoker(delegate ()
                {
                    if (config.bDisplayException || !bException)
                    {
                        if (bNewLine)
                        {
                            SerialMessageBox.AppendText(str);
                            SerialMessageBox.AppendText("\r\n");
                        }
                        else
                            SerialMessageBox.AppendText($"{str}");
                        SerialMessageBox.ScrollToCaret();
                    }
                    if(bLog)
                        logger.Debug($"{str}");
                }));
			}		
			catch(Exception ex)
			{
                logger.Fatal(ex.Message);
			}
		}

        //----------------------------------------------------------------------
        //----------------------------------------------------------------------
        //public void Printf(string str, bool bException=false)
        //{
        //    if (config.bDisplayException || !bException)
        //    {
        //        SerialMessageBox.AppendText($"{str}\r\n");
        //        SerialMessageBox.ScrollToCaret();
        //    }
        //    logger.Info($"{str}");
        //}


		//----------------------------------------------------------------------
		//----------------------------------------------------------------------
		//private  void msec_delay(int msec)
		//{
  //          //this.Invoke( (ThreadStart)(() => { }), DispatcherPriority.ApplicationIdle);
		//	Thread.Sleep(msec);
		//}
				
		//----------------------------------------------------------------------
        //private async void SsdCommandTask()
        //{ 
            
        //    var taskDiskSpd = Task.Run(() => SsdCommandThread());
        //    await taskDiskSpd;            
        //}
		//	Thread
		//----------------------------------------------------------------------
		//public void SsdCommandThread()
		//{
  //          bRunningThread = true;
  //          try
		//	{
  //              while (bRunningThread)
		//		{
		//			try
		//			{
						
		//			}
		//			catch (Exception ex)
		//			{
		//				iPrintf(ex.ToString(), true, true, true);
		//			}
		//		}   //	while()
		//	}
		//	catch(Exception ex)
		//	{
		//		iPrintf(ex.ToString(), true, true, true);              
		//	}
		//	iPrintf("Thread Closed");
		//	//==================================================================
		//}
    }
}




