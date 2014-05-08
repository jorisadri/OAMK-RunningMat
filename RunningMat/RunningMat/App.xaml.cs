using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
        public static DispatcherTimer Updater = new DispatcherTimer();
       

        public static IncomingData DataPotentiometer = new IncomingData();
        public static Video VLCVideo = new Video();
        public static ExcelAngleData Excel = new ExcelAngleData();
        public static Speed SpeedTreadmill = new Speed();
        public static ControlTreadmill Steering = new ControlTreadmill();
        public static ControlTreadmill controltreadmill = new ControlTreadmill();
        public static Calibrate calibrating = new Calibrate();

        public static bool Shutdown=false;


        public App()
        {
            
            Updater.Interval = TimeSpan.FromMilliseconds(1000);
            Updater.Tick += Updater_Tick;
            


          // MotorUpdate.Interval = TimeSpan.FromMilliseconds(30); //i took this time because of the sample rate
          
           
        }
      

        void Updater_Tick(object sender, EventArgs e)
        {

            Shutdown = true;

        }

      


    }
}
