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
using Xceed.Wpf.AvalonDock.Layout;



namespace RunningMat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
       // https://sparrowtoolkit.codeplex.com
        bool FullScreen = false;
        

        public MainWindow()
        {
            InitializeComponent();
          
           
            XValue.DataContext = App.DataPotentiometer;
            YValue.DataContext = App.DataPotentiometer;
            SampleRate.DataContext = App.Excel;
            XAngle.DataContext = App.Excel;
            YAngle.DataContext = App.Excel;
            speed.DataContext = App.SpeedTreadmill;
            SpeedVideo.DataContext = App.VLCVideo;
            testsliderX.DataContext = App.controltreadmill;
            testsliderY.DataContext = App.controltreadmill;
            Film.DataContext = App.VLCVideo;
            Multiplier.DataContext = App.VLCVideo;

            Start.IsEnabled = false;
            Stop.IsEnabled = false;
            Pause.IsEnabled = false;
            Pitch.DataContext = App.PhoneConnection;
            Roll.DataContext = App.PhoneConnection;
            ControllsystemPot.DataContext = App.controltreadmill;
            ControllsystemPhone.DataContext = App.controltreadmill;

            App.controltreadmill.MotorStopPitch();
            App.controltreadmill.MotorStopRoll();
           
            

            Closing += MainWindow_Closing;

            App.VLCVideo.Movie.MouseDown += Movie_MouseDown;
            
        }

        void Movie_MouseDown(object sender, MouseButtonEventArgs e)
        {
            object visualWindowContent = new object();

            System.Windows.Controls.Grid content = new System.Windows.Controls.Grid();
            if (FullScreen == false)
            {

                visualWindowContent = this.Film;
                
                this.Content = App.VLCVideo.Movie;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }

            else if (FullScreen == true)
            {
                //App.VLCVideo.Movie.D
                this.Content = visualWindowContent;

                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
            FullScreen = !FullScreen;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                e.Cancel = true;
                App.controltreadmill.Stop = true;
                App.controltreadmill.MotorStopPitch();
                App.controltreadmill.MotorStopRoll();
                e.Cancel = false ;
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            
            App.UIController.status = App.PlayState.Load;
            App.UIController.choise = App.Choise.Run;
            App.UIController.DoYourThing();
            Start.IsEnabled = true;
            Stop.IsEnabled = false;
            Pause.IsEnabled = false;
            
            

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            
            App.UIController.status = App.PlayState.Start;
            App.UIController.DoYourThing();

                Stop.IsEnabled = true;
                Start.IsEnabled = false;
                Pause.IsEnabled = true;
            
            
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {

            App.UIController.status = App.PlayState.Pauze;
            App.UIController.DoYourThing();
            Pause.IsEnabled = false;
            Start.IsEnabled = true;
           
           
            

           

        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {

           App.UIController.status = App.PlayState.Stop;
           App.UIController.DoYourThing();
           Start.IsEnabled = true;
           Pause.IsEnabled = false;
           Stop.IsEnabled = false;
          
          

        }
       

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                App.Current.Shutdown();
            }
        }

        private void MakeAngleData_Click(object sender, RoutedEventArgs e)
        {
            App.UIController.choise = App.Choise.MakeAngle;
            App.UIController.DoYourThing();
            App.VLCVideo.Movie.Stop();
            Start.IsEnabled = true;
            Pause.IsEnabled = false;
            Stop.IsEnabled = false;
            
          
            
            

        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            App.UIController.choise = App.Choise.Test;
            App.UIController.status = App.PlayState.Stop;

            Start.IsEnabled = true;
            Pause.IsEnabled = false;
            Stop.IsEnabled = false;
           

            
        }

       
      



      
      

   
    }
     
}


   
