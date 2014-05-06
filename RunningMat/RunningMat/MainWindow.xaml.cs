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
using System.Windows.Forms.DataVisualization.Charting;



namespace RunningMat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
       // https://sparrowtoolkit.codeplex.com


        public MainWindow()
        {
            InitializeComponent();
          
           
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
            
            App.VLCVideo.LoadVideo();
            App.Excel.GetExcel();
            Start.IsEnabled = true;

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
                if (App.VLCVideo.Movie.IsLoaded && !App.controltreadmill.MotorControllerX.IsBusy)
                {
                    App.VLCVideo.Movie.Play();
                    App.controltreadmill.MotorControllerX.RunWorkerAsync();
                    Stop.IsEnabled = true;
                    Start.IsEnabled = false;
                }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {

            App.VLCVideo.Movie.Pause();
            
            Pause.IsEnabled = false;
            Start.IsEnabled = true;
           
            

           

        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            
           App.VLCVideo.Movie.Stop();
           Start.IsEnabled = true;
          
          

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


   
