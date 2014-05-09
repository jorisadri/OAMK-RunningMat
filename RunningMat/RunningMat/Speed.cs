using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Windows.Threading;
namespace RunningMat
{
   public class Speed:BaseClass
    {
        public DispatcherTimer GetSpeedTimer;
        public System.IO.Ports.SerialPort SerialPortArduino;
        string ReadString;
        private double _speedKMH = 0;
        public double SpeedKMH {
            get { return _speedKMH; }
            set { _speedKMH = value; RaisePropChanged("SpeedKMH"); }
        }

        public Speed()
        {
            try
            {
                SerialPortArduino = new System.IO.Ports.SerialPort();
                GetSpeedTimer = new DispatcherTimer();
                GetSpeedTimer.Interval = TimeSpan.FromMilliseconds(20);
                GetSpeedTimer.Tick += GetSpeedTimer_Tick;
                SerialPortArduino.PortName = "COM3";
                SerialPortArduino.Open();

            }
            catch (Exception)
            {
                
                
            }
           
            

        }

        void GetSpeedTimer_Tick(object sender, EventArgs e)
        {
            int IndexOfNonNummeric = -1;
            if (SerialPortArduino.BytesToRead > 0)
            {
                ReadString = SerialPortArduino.ReadExisting();

                for (int i = 0; i < ReadString.Length-1; i++)
                {
                    if (!Char.IsDigit(ReadString.ElementAtOrDefault(i)))
                    {
                        IndexOfNonNummeric = i;
                    }
                    
                }

                if (IndexOfNonNummeric > -1)
                {
                    ReadString = ReadString.Substring(0, IndexOfNonNummeric);
                }
                if (ReadString.Length > 0)
                {
                    try
                    {
                        SpeedKMH = Convert.ToInt32(ReadString);
                        SpeedKMH=((SpeedKMH/100)*3.052);


                        SpeedKMH = Math.Round(SpeedKMH, 1);

                        App.VLCVideo.FrameRate = SpeedKMH / 3;
                       
                    }
                    catch (Exception)
                    {
                        
                        
                    }
                   
                }
            }
        }
    }
}
