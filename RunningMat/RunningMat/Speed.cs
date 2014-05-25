using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Windows.Threading;
using System.Management;
using System.Windows;
namespace RunningMat
{
    public class Speed : BaseClass
    {
        //serialport and timer for get the values from arduino
        public DispatcherTimer GetSpeedTimer;
        public System.IO.Ports.SerialPort SerialPortArduino;
        string ReadString;
        public bool Arduino = false;
        private double _speedKMH = 3;
        public double SpeedKMH
        {
            get { return _speedKMH; }
            set { _speedKMH = value; RaisePropChanged("SpeedKMH"); }
        }

        public Speed()
        {
            OpenArduino();
        }

        public void OpenArduino()
        {
            try
            {
                SerialPortArduino = new System.IO.Ports.SerialPort();
                GetSpeedTimer = new DispatcherTimer();
                GetSpeedTimer.Interval = TimeSpan.FromMilliseconds(20);
                GetSpeedTimer.Tick += GetSpeedTimer_Tick;
                //checks if the correct arduino is connected
                SerialPortArduino.PortName = AutodetectArduinoPort();
                SerialPortArduino.Open();
                Arduino = true;

            }
            catch (Exception)
            {
                MessageBox.Show("No Arduino found with PNPDeviceID:64935343733351707252 \nThe speed will be set to 3 Km/h");
                Arduino = false;
            }
            
        }
        // to detect if the correct arduino is connected
        private string AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    //safe COM port of arduino if arduino with PNPDeviceI: 64935343733351707252 is connected 
                    string desc = item["PNPDeviceID"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("64935343733351707252"))
                    {
                        return deviceId;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        // read buffer serialport on COM from arduino when timer hits
        void GetSpeedTimer_Tick(object sender, EventArgs e)
        {
            
            int IndexOfNonNummeric = -1;
            if (SerialPortArduino.BytesToRead > 0)
            {
                ReadString = SerialPortArduino.ReadExisting();
                //checks if string is valid
                for (int i = 0; i < ReadString.Length - 1; i++)
                {
                    if (!Char.IsDigit(ReadString.ElementAtOrDefault(i)))
                        IndexOfNonNummeric = i;
                }
                if (IndexOfNonNummeric > -1)
                    ReadString = ReadString.Substring(0, IndexOfNonNummeric);

                if (ReadString.Length > 0)
                {
                    try
                    {
                        if (App.UIController.choise != App.Choise.MakeAngle)
                        {
                            SpeedKMH = Convert.ToInt32(ReadString);
                            SpeedKMH = ((SpeedKMH / 100) * 3.052);
                            SpeedKMH = Math.Round(SpeedKMH, 1);
                            //if threadmill has speed of 3 the movie plays on normal speed
                            App.VLCVideo.FrameRate = SpeedKMH / 3;
                        }
                        else
                            App.VLCVideo.FrameRate = 1.0;
                    }
                    catch (Exception)
                    {
                    }

                }
            }
        }
    }
}
