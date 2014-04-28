using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AxAXVLC;
using Microsoft.Win32;
using ExcelLibrary;
using ExcelLibrary.SpreadSheet;
using System.Threading;
using System.IO;

namespace RunningMat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       /// AxVLCPlugin vlc;
       // Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();

        // USB ports for Potentiometer values
        private MccDaq.MccBoard USB1 = new MccDaq.MccBoard(0);
        

       // https://sparrowtoolkit.codeplex.com

        //Channel
        //int XChannelin = 0;
        //MccDaq.Range TheRange;
        //double InputX;

       // private  MccDaq.DigitalPortDirection DigitalIn;
       // private MccDaq.DigitalPortType PortType;

       // private MccDaq.DigitalPortDirection DigitalOut;
       // private MccDaq.DigitalPortType PortOne;
       // private MccDaq.DigitalPortType PortTwo;
       // private MccDaq.DigitalPortType PortTree;
       // private MccDaq.DigitalPortType PortFour;

       //// private int ADResolution, NumAIChans, HighChan;
       

       

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(VLCPlayerLoaded);
            XValue.DataContext = App.DataPotentiometer;
            YValue.DataContext = App.DataPotentiometer;
            SampleRate.DataContext = App.Excel;
            XAngle.DataContext = App.Excel;
            YAngle.DataContext = App.Excel;
            //speed.DataContext = App.controltreadmill;
            SpeedVideo.DataContext = App.VLCVideo;
            testslider.DataContext = App.controltreadmill;
            Film.DataContext = App.VLCVideo;
            framerate.DataContext = App.VLCVideo;
           
            
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
           // App.VLCVideo.LoadVideo();
           // App.Excel.GetExcel();

            //binding with MediaElement almost imposible. 
            //Movie.Source = new Uri(App.VLCVideo.Path("Movie(.mp4)|*.mp4|All files(*.*)|*.*"));
           // App.SpeedTreadmill.SerialPortArduino.Open();

            
            App.VLCVideo.LoadVideo();
            App.Excel.GetExcel();

                
        }

       
        void VLCPlayerLoaded(object sender, RoutedEventArgs e)
        {
            
            //WindowVLC.Child = App.VLCVideo.vlc;
        }

        
          


       

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //App.VLCVideo.Play();
            //App.Excel.Samplerate();
            //App.controltreadmill.MotorControllerX.RunWorkerAsync();

            if (App.VLCVideo.Movie.IsLoaded)
            {
                App.VLCVideo.Movie.Play();
            }
            if (!App.controltreadmill.MotorControllerX.IsBusy)
            {
               App.controltreadmill.MotorControllerX.RunWorkerAsync(); 
            }
             
           

            
       
           
           
           
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {

            //App.VLCVideo.vlc.playlist.togglePause();
            //Movie.Pause();
            App.VLCVideo.Movie.Pause();

        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            //App.VLCVideo.vlc.playlist.stop();
           App.VLCVideo.Movie.Stop();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.controltreadmill.MotorControllerX.RunWorkerAsync();
            //App.SpeedTreadmill.GetSpeedTimer.Start();

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

   
    }
     
}


   
