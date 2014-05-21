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
        bool FullScreen = false;

        public MainWindow()
        {
            InitializeComponent();

            //all binding values
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
            Pitch.DataContext = App.PhoneConnection;
            Roll.DataContext = App.PhoneConnection;
            ControllsystemPot.DataContext = App.controltreadmill;
            ControllsystemPhone.DataContext = App.controltreadmill;

            //to stop motors if they still running
            App.controltreadmill.MotorStopPitch();
            App.controltreadmill.MotorStopRoll();

            Start.IsEnabled = false;
            Stop.IsEnabled = false;
            Pause.IsEnabled = false;

            //event for closing program
            Closing += MainWindow_Closing;
            //event for fullscreen
            App.VLCVideo.Movie.MouseDown += Movie_MouseDown;
            

        }

        void Movie_MouseDown(object sender, MouseButtonEventArgs e)
        {
            object visualWindowContent = new object();
            System.Windows.Controls.Grid content = new System.Windows.Controls.Grid();
            if (FullScreen == false)
            {
                // put film in fullscreen
                visualWindowContent = this.Film;
                this.Content = App.VLCVideo.Movie;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }

            else if (FullScreen == true)
            {
                //try to put it back to normal mode (this is not working)
                //this.Content = visualWindowContent;
                //this.WindowStyle = WindowStyle.SingleBorderWindow;
                //this.WindowState = WindowState.Normal;
            }
            FullScreen = !FullScreen;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {       //We need to cansel the quit to stop the motors 
            e.Cancel = true;
            App.controltreadmill.Stop = true;
            App.controltreadmill.MotorStopPitch();
            App.controltreadmill.MotorStopRoll();
            e.Cancel = false;
        }

        // this events controlls the interface buttons
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
            if (App.UIController.choise == App.Choise.MakeAngle && !App.PhoneConnection.started)
            {
                App.UIController.status = App.PlayState.Start;
                App.UIController.DoYourThing();
            }
            else
            {
                App.UIController.status = App.PlayState.Start;
                App.UIController.DoYourThing();
                Stop.IsEnabled = true;
                Start.IsEnabled = false;
                Pause.IsEnabled = true;
            }
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

        private void MakeAngleData_Click(object sender, RoutedEventArgs e)
        {
            App.UIController.choise = App.Choise.MakeAngle;
            App.UIController.status=App.PlayState.Load;
            App.UIController.DoYourThing();    
            Start.IsEnabled = true;
            Pause.IsEnabled = false;
            Stop.IsEnabled = false;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            App.UIController.choise = App.Choise.Test;
            App.UIController.status = App.PlayState.Load;
            Start.IsEnabled = true;
            Pause.IsEnabled = false;
            Stop.IsEnabled = false;
        }


        //Programma quits when escape is pressed and the movie is on fullscreen
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                App.Current.Shutdown();
            
        }

       
       










    }

}



