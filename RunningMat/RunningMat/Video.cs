using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxAXVLC;
using AXVLC;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;




namespace RunningMat
{
    public class Video : BaseClass
    {
        private MediaElement _movie;
        public double TotalMovieTime;
        public TimeSpan LenghtMovie;
        public double SampleFrequenty;
        public double Time;
        private Uri VideoFile;

        private double _framerate = 1;
        public double FrameRate
        {
            get { return _framerate; }
            set { _framerate = value; _movie.SpeedRatio = FrameRate; RaisePropChanged("FrameRate"); }
        }

        public MediaElement Movie
        {
            get { return _movie; }
        }

        public Video()
        {
            
            _movie = new MediaElement();
            _movie.LoadedBehavior = MediaState.Manual;
            _movie.UnloadedBehavior = MediaState.Manual;
            //events for Video
            _movie.MediaOpened += _movie_MediaOpened;
            _movie.MediaEnded += _movie_MediaEnded;
        }
        //when movie ends safe the excel data when makeing new data
        void _movie_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (App.UIController.choise == App.Choise.MakeAngle)
            {
                App.Excel.PhoneAngletimer.Stop();
                App.Excel.Safe();
                App.Excel.counter = 0;
            }
            else
            {
                App.controltreadmill.Stop = true;
                App.controltreadmill.MotorStopPitch();
                App.controltreadmill.MotorStopRoll();
            }
        }
        //when film opens calculate the samplerate
        void _movie_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            TotalMovieTime = _movie.NaturalDuration.TimeSpan.TotalMilliseconds;
            App.Excel.Samplerate();
        }

        public void LoadVideo()
        {
            //http://weblogs.asp.net/jdanforth/archive/2012/12/14/binding-mediaelement-to-a-viewmodel-in-a-windows-8-store-app.aspx
            //get Path of the videoFile
            string local = PathFile("Movie(.mp4, .mov)|*.mp4;*.mov|All files(*.*)|*.*");
            if (local != "")
            {
                VideoFile = new Uri(local);
                _movie.Source = VideoFile;
            }
        }
    }
}
