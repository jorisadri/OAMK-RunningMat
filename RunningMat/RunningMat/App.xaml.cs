using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RunningMat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public DispatcherTimer Updater = new DispatcherTimer();
        public static DispatcherTimer MotorUpdate;

        public static IncomingData DataPotentiometer = new IncomingData();
        public static Video VLCVideo = new Video();
        public static ExcelAngleData Excel = new ExcelAngleData();
        public static Speed SpeedTreadmill = new Speed();
        public static ControlTreadmill Steering = new ControlTreadmill();
        public static ControlTreadmill controltreadmill = new ControlTreadmill();
       

      


        public App()
        {
             MotorUpdate = new DispatcherTimer();
            Updater.Interval = TimeSpan.FromMilliseconds(70);
            Updater.Tick += Updater_Tick;
            Updater.Start();

          // MotorUpdate.Interval = TimeSpan.FromMilliseconds(30); //i took this time because of the sample rate
           MotorUpdate.Tick += MotorUpdate_Tick;
           
        }

        void MotorUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                controltreadmill.MotorControllerX.RunWorkerAsync();
            }
            catch (Exception)
            {
                
                
            }
           
        }   

        void Updater_Tick(object sender, EventArgs e)
        {
           // VLCVideo.vlc.input.rate = VLCVideo.FrameRate;
           //DataPotentiometer.GetDataY();
            

        }


    }
}
