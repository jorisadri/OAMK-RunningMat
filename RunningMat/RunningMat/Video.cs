﻿using System;
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
   public class Video:BaseClass
    {
       

       private MediaElement _movie;
       public double TotalMovieTime;

       public TimeSpan LenghtMovie; 
       public double SampleFrequenty;
       public double Time;
       
            

        private double _framerate=1;
        public double FrameRate  
        {
            get { return _framerate; }
            set { _framerate = value; _movie.SpeedRatio = FrameRate; RaisePropChanged("FrameRate"); }
        }

        private Uri VideoFile;
       

        public MediaElement Movie
        {
            get { return _movie; }
        }


        public Video()
        {
            _movie= new MediaElement();
            _movie.LoadedBehavior = MediaState.Manual;
            _movie.UnloadedBehavior = MediaState.Manual;
            _movie.MediaOpened += _movie_MediaOpened;
            _movie.MediaEnded += _movie_MediaEnded;
        }

        void _movie_MediaEnded(object sender, RoutedEventArgs e)
        {
            App.Excel.PhoneAngletimer.Stop();
            App.Excel.Safe();
            MessageBox.Show("File is Safed");
        }

        void _movie_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {

            TotalMovieTime = _movie.NaturalDuration.TimeSpan.TotalMilliseconds;
            App.Excel.Samplerate();
           
        }
        


            public void LoadVideo()
            {
               
                //http://weblogs.asp.net/jdanforth/archive/2012/12/14/binding-mediaelement-to-a-viewmodel-in-a-windows-8-store-app.aspx
               

                string local = PathFile("Movie(.mp4, .mov)|*.mp4;*.mov|All files(*.*)|*.*");
                if (local != "")
                {

                  
                    VideoFile = new Uri(local);
                    _movie.Source = VideoFile;
                    
                    

                    
                }
            }

          

           

    }
}
