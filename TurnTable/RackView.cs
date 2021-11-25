//==============================================================================
//		MainWindow.xaml.cs
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
using System.Windows.Input;
using System.Windows.Markup;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Windows.Threading;
using Path = System.IO.Path;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using HPT_U8 = System.Byte;
using HPT_U16 = System.UInt16;
using HPT_U32 = System.UInt32;
using HPT_U64 = System.UInt64;
using DEVICEID = System.UInt32;


//==============================================================================
//==============================================================================
namespace SSD_Machine
{
	//==========================================================================
	//		U P P E R
	//==========================================================================
	public class UpperListBox : INotifyPropertyChanged
	{
		private string	_doorSwitch;
		private string	_linkState;
		private string	_senseNVMe;
		private string	_senseSata;
		private string	_driveType;
		private string	_powerState;
		private string	_port12V;
		private string	_portVolt;
		private string	_temperature;
		private string	_gnLedOnOff;
		private string	_gnLedBlink;
		private string	_redLedOnOff;
		private string	_redLedBlink;   

		private	Uri		_imageLinkButton;
		private	Uri		_imagePowerSwitchButton;
		private	Uri		_imageGreenLedButton;
		private	Uri		_imageGreenLedBlinkButton;
		private	Uri		_imageRedLedButton;
		private	Uri		_imageRedLedBlinkButton;

		private string	_modelName;
		private string	_serialNumber;
		private string	_capacity;

		private int 	_deviceCmdCode;
		private int		_resultOfDevCmd;
		private bool	_waitReqDevCmd;
		private int		_waitCountDevCmd;
		private int		_delayedDevCmdCode;
		private bool	_pairReScanFlag;
		private bool	_pairUnplugFlag;

		private string _logicalDrive;
		private string _btnCheckSpeed;
		private int     _progress;
		private string _diskReadPoint;
		private string _diskWritePoint;
		private string _btnBackground;
		private string _result;
		private string _resultBackground;

		public string	RackPos			{ get; set; }

		public string	DoorSwitch
		{
			get { return _doorSwitch;}
			set { _doorSwitch = value;  OnPropertyChanged(); }
		}

		public string	DoorForeColor	{ get; set; }
		public string	DoorBackColor	{ get; set; }

		public string LinkState
		{
			get { return _linkState; } 
			set { _linkState = value; OnPropertyChanged(); }
		}

		public string	LinkStateForeColor	{ get; set; }
		public string	LinkStateBackColor	{ get; set; }

		public string SenseNVMe
		{
			get { return _senseNVMe; } 
			set { _senseNVMe = value; OnPropertyChanged(); }
		}

		public string	SenseNVMeForeColor	{ get; set; }
		public string	SenseNVMeBackColor	{ get; set; }

		public string SenseSata
		{
			get { return _senseSata; } 
			set { _senseSata = value; OnPropertyChanged(); }
		}
		public string	SenseSataForeColor	{ get; set; }
		public string	SenseSataBackColor	{ get; set; }

		public string DriveType
		{
			get { return _driveType; } 
			set { _driveType = value; OnPropertyChanged(); }
		}
		public string	DriveTypeForeColor	{ get; set; }
		public string	DriveTypeBackColor	{ get; set; }

		public string PowerState
		{
			get { return _powerState; } 
			set { _powerState = value; OnPropertyChanged(); }
		}
		public string	PowerStateForeColor	{ get; set; }
		public string	PowerStateBackColor	{ get; set; }


		public string Port12V
		{
			get { return _port12V; } 
			set { _port12V = value; OnPropertyChanged(); }
		}
		public string	Port12VForeColor	{ get; set; }
		public string	Port12VBackColor	{ get; set; }


		public string PortVolt
		{
			get { return _portVolt; } 
			set { _portVolt = value; OnPropertyChanged(); }
		}

		public string Temperature
		{
			get { return _temperature; } 
			set { _temperature = value; OnPropertyChanged(); }
		}


		public string GnLedOnOff
		{
			get { return _gnLedOnOff; } 
			set { _gnLedOnOff = value; OnPropertyChanged(); }
		}
		public string	GnLedOnOffForeColor	{ get; set; }
		public string	GnLedOnOffBackColor	{ get; set; }


		public string GnLedBlink
		{
			get { return _gnLedBlink; } 
			set { _gnLedBlink = value; OnPropertyChanged(); }
		}
		public string	GnLedBlinkForeColor	{ get; set; }
		public string	GnLedBlinkBackColor	{ get; set; }


		public string RedLedOnOff
		{
			get { return _redLedOnOff; } 
			set { _redLedOnOff = value; OnPropertyChanged(); }
		}
		public string	RedLedOnOffForeColor	{ get; set; }
		public string	RedLedOnOffBackColor	{ get; set; }

		public string RedLedBlink
		{
			get { return _redLedBlink; } 
			set { _redLedBlink = value; OnPropertyChanged(); }
		}
		public string	RedLedBlinkForeColor	{ get; set; }
		public string	RedLedBlinkBackColor	{ get; set; }


		public Uri ImageLinkButton
		{
			get { return _imageLinkButton; }
			set	{ _imageLinkButton = value; OnPropertyChanged(); }
		}

		public Uri ImagePowerSwitchButton
		{
			get { return _imagePowerSwitchButton; }
			set	{ _imagePowerSwitchButton = value; OnPropertyChanged(); }
		}

		public Uri ImageGreenLedButton
		{
			get { return _imageGreenLedButton; }
			set	{ _imageGreenLedButton = value; OnPropertyChanged(); }
		}

		public Uri ImageGreenLedBlinkButton
		{
			get { return _imageGreenLedBlinkButton; }
			set	{ _imageGreenLedBlinkButton = value; OnPropertyChanged(); }
		}

		public Uri ImageRedLedButton
		{
			get { return _imageRedLedButton; }
			set	{ _imageRedLedButton = value; OnPropertyChanged(); }
		}

		public Uri ImageRedLedBlinkButton
		{
			get { return _imageRedLedBlinkButton; }
			set	{ _imageRedLedBlinkButton = value; OnPropertyChanged(); }
		}


		public string ModelName
		{
			get { return _modelName; }
			set { _modelName = value; OnPropertyChanged(); }
		}
		public string SerialNumber
		{
			get { return _serialNumber; }
			set { _serialNumber = value; OnPropertyChanged(); }
		}
		public string Capacity
		{
			get { return _capacity; }
			set { _capacity = value; OnPropertyChanged(); }
		}

		public int DeviceCmdCode
		{
			get { return _deviceCmdCode; }
			set { _deviceCmdCode = value; OnPropertyChanged(); }
		}

		public int ResultOfDevCmd
		{
			get { return _resultOfDevCmd; }
			set { _resultOfDevCmd = value; OnPropertyChanged(); }
		}

		public bool WaitReqDevCmdFlag
		{
			get { return _waitReqDevCmd; }
			set { _waitReqDevCmd = value; OnPropertyChanged(); }
		}

		public int	WaiDevCmdCount
		{
			get { return _waitCountDevCmd; }
			set { _waitCountDevCmd = value; OnPropertyChanged(); }
		}

		public int	DelayedDevCmdCode
		{
			get { return _delayedDevCmdCode; }
			set { _delayedDevCmdCode = value; OnPropertyChanged(); }
		}

		public bool PairReScanFlag
		{
			get { return _pairReScanFlag; }
			set { _pairReScanFlag = value; OnPropertyChanged(); }
		}

		public bool PairUnplugFlag
		{
			get { return _pairUnplugFlag; }
			set { _pairUnplugFlag = value; OnPropertyChanged(); }
		}

		public string LogicalDrive
		{
			get { return _logicalDrive; }
			set { _logicalDrive = value; OnPropertyChanged(); }
		}

		public string BtnCheckSpeed
		{
			get { return _btnCheckSpeed; }
			set { _btnCheckSpeed = value; OnPropertyChanged(); }
		}

		public int Progress
		{
			get { return _progress; }
			set { _progress = value; OnPropertyChanged(); }
		}

		public string DiskReadPoint
		{
			get { return _diskReadPoint; }
			set { _diskReadPoint = value; OnPropertyChanged(); }
		}

		public string DiskWritePoint
		{
			get { return _diskWritePoint; }
			set { _diskWritePoint = value; OnPropertyChanged(); }
		}

		public string BtnBackground
		{
			get { return _btnBackground; }
			set { _btnBackground = value; OnPropertyChanged(); }
		}

		public string Result
		{
			get { return _result; }
			set { _result = value; OnPropertyChanged(); }
		}
		
		public string ResultBackground
		{
			get { return _resultBackground; }
			set { _resultBackground = value; OnPropertyChanged(); }
		}

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}
	}



	//==========================================================================
	//		L O W E R
	//==========================================================================
	public class LowerListBox : INotifyPropertyChanged
	{
		private string	_doorSwitch;
		private string	_linkState;
		private string	_senseNVMe;
		private string	_senseSata;
		private string	_driveType;
		private string	_powerState;
		private string	_port12V;
		private string	_portVolt;
		private string	_temperature;
		private string	_gnLedOnOff;
		private string	_gnLedBlink;
		private string	_redLedOnOff;
		private string	_redLedBlink;

		private	Uri		_imageLinkButton;
		private	Uri		_imagePowerSwitchButton;
		private	Uri		_imageGreenLedButton;
		private	Uri		_imageGreenLedBlinkButton;
		private	Uri		_imageRedLedButton;
		private	Uri		_imageRedLedBlinkButton;

		private string	_modelName;
		private string	_serialNumber;
		private string	_capacity;

		private int 	_deviceCmdCode;
		private int		_resultOfDevCmd;
		private bool	_waitReqDevCmd;
		private int		_waitCountDevCmd;
		private int		_delayedDevCmdCode;
		private bool	_pairGetInfoFlag;

		private string _logicalDrive;
		private string  _btnCheckSpeed;
		private int     _progress;
		private string _diskReadPoint;
		private string _diskWritePoint;
		private string _btnBackground;
		private string _result;
		private string _resultBackground;

		public string	RackPos			{ get; set; }

		public string	DoorSwitch
		{
			get { return _doorSwitch;}
			set { _doorSwitch = value;  OnPropertyChanged(); }
		}

		public string	DoorForeColor	{ get; set; }
		public string	DoorBackColor	{ get; set; }

		public string LinkState
		{
			get { return _linkState; } 
			set { _linkState = value; OnPropertyChanged(); }
		}

		public string	LinkStateForeColor	{ get; set; }
		public string	LinkStateBackColor	{ get; set; }

		public string SenseNVMe
		{
			get { return _senseNVMe; } 
			set { _senseNVMe = value; OnPropertyChanged(); }
		}

		public string	SenseNVMeForeColor	{ get; set; }
		public string	SenseNVMeBackColor	{ get; set; }

		public string SenseSata
		{
			get { return _senseSata; } 
			set { _senseSata = value; OnPropertyChanged(); }
		}
		public string	SenseSataForeColor	{ get; set; }
		public string	SenseSataBackColor	{ get; set; }

		public string DriveType
		{
			get { return _driveType; } 
			set { _driveType = value; OnPropertyChanged(); }
		}
		public string	DriveTypeForeColor	{ get; set; }
		public string	DriveTypeBackColor	{ get; set; }

		public string PowerState
		{
			get { return _powerState; } 
			set { _powerState = value; OnPropertyChanged(); }
		}
		public string	PowerStateForeColor	{ get; set; }
		public string	PowerStateBackColor	{ get; set; }


		public string Port12V
		{
			get { return _port12V; } 
			set { _port12V = value; OnPropertyChanged(); }
		}
		public string	Port12VForeColor	{ get; set; }
		public string	Port12VBackColor	{ get; set; }


		public string PortVolt
		{
			get { return _portVolt; } 
			set { _portVolt = value; OnPropertyChanged(); }
		}

		public string Temperature
		{
			get { return _temperature; } 
			set { _temperature = value; OnPropertyChanged(); }
		}


		public string GnLedOnOff
		{
			get { return _gnLedOnOff; } 
			set { _gnLedOnOff = value; OnPropertyChanged(); }
		}
		public string	GnLedOnOffForeColor	{ get; set; }
		public string	GnLedOnOffBackColor	{ get; set; }


		public string GnLedBlink
		{
			get { return _gnLedBlink; } 
			set { _gnLedBlink = value; OnPropertyChanged(); }
		}
		public string	GnLedBlinkForeColor	{ get; set; }
		public string	GnLedBlinkBackColor	{ get; set; }


		public string RedLedOnOff
		{
			get { return _redLedOnOff; } 
			set { _redLedOnOff = value; OnPropertyChanged(); }
		}
		public string	RedLedOnOffForeColor	{ get; set; }
		public string	RedLedOnOffBackColor	{ get; set; }

		public string RedLedBlink
		{
			get { return _redLedBlink; } 
			set { _redLedBlink = value; OnPropertyChanged(); }
		}
		public string	RedLedBlinkForeColor	{ get; set; }
		public string	RedLedBlinkBackColor	{ get; set; }


		public Uri ImageLinkButton
		{
			get { return _imageLinkButton; }
			set	{ _imageLinkButton = value; OnPropertyChanged(); }
		}

		public Uri ImagePowerSwitchButton
		{
			get { return _imagePowerSwitchButton; }
			set	{ _imagePowerSwitchButton = value; OnPropertyChanged(); }
		}

		public Uri ImageGreenLedButton
		{
			get { return _imageGreenLedButton; }
			set	{ _imageGreenLedButton = value; OnPropertyChanged(); }
		}

		public Uri ImageGreenLedBlinkButton
		{
			get { return _imageGreenLedBlinkButton; }
			set	{ _imageGreenLedBlinkButton = value; OnPropertyChanged(); }
		}

		public Uri ImageRedLedButton
		{
			get { return _imageRedLedButton; }
			set	{ _imageRedLedButton = value; OnPropertyChanged(); }
		}

		public Uri ImageRedLedBlinkButton
		{
			get { return _imageRedLedBlinkButton; }
			set	{ _imageRedLedBlinkButton = value; OnPropertyChanged(); }
		}

		public string ModelName
		{
			get { return _modelName; }
			set { _modelName = value; OnPropertyChanged(); }
		}
		public string SerialNumber
		{
			get { return _serialNumber; }
			set { _serialNumber = value; OnPropertyChanged(); }
		}
		public string Capacity
		{
			get { return _capacity; }
			set { _capacity = value; OnPropertyChanged(); }
		}

		public int DeviceCmdCode
		{
			get { return _deviceCmdCode; }
			set { _deviceCmdCode = value; OnPropertyChanged(); }
		}

		public int ResultOfDevCmd
		{
			get { return _resultOfDevCmd; }
			set { _resultOfDevCmd = value; OnPropertyChanged(); }
		}

		public bool WaitReqDevCmdFlag
		{
			get { return _waitReqDevCmd; }
			set { _waitReqDevCmd = value; OnPropertyChanged(); }
		}

		public int	WaiDevCmdCount
		{
			get { return _waitCountDevCmd; }
			set { _waitCountDevCmd = value; OnPropertyChanged(); }
		}

		public int	DelayedDevCmdCode
		{
			get { return _delayedDevCmdCode; }
			set { _delayedDevCmdCode = value; OnPropertyChanged(); }
		}

		public bool PairGetInfoFlag
		{
			get { return _pairGetInfoFlag; }
			set { _pairGetInfoFlag = value; OnPropertyChanged(); }
		}

		public string LogicalDrive
		{
			get { return _logicalDrive; }
			set { _logicalDrive = value; OnPropertyChanged(); }
		}

		public string BtnCheckSpeed
		{
			get { return _btnCheckSpeed; }
			set { _btnCheckSpeed = value; OnPropertyChanged(); }
		}

		public int Progress
		{
			get { return _progress; }
			set { _progress = value; OnPropertyChanged(); }
		}

		public string DiskReadPoint
		{
			get { return _diskReadPoint; }
			set { _diskReadPoint = value; OnPropertyChanged(); }
		}

		public string DiskWritePoint
		{
			get { return _diskWritePoint; }
			set { _diskWritePoint = value; OnPropertyChanged(); }
		}

		public string BtnBackground
		{
			get { return _btnBackground; }
			set { _btnBackground = value; OnPropertyChanged(); }
		}

		public string Result
		{
			get { return _result; }
			set { _result = value; OnPropertyChanged(); }
		}

		public string ResultBackground
		{
			get { return _resultBackground; }
			set { _resultBackground = value; OnPropertyChanged(); }
		}

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}
	}
}

