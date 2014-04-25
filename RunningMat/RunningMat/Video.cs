using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxAXVLC;
using AXVLC;
using System.Threading;



namespace RunningMat
{
   public class Video:BaseClass
    {
       public AxVLCPlugin2 vlc=new AxVLCPlugin2();
  

      public TimeSpan LenghtMovie; 
       public double SampleFrequenty;
            

        private double _framerate=1;
        public double FrameRate  
        {
            get { return _framerate; }
            set { _framerate = value; RaisePropChanged("FrameRate"); }
        }


        public Video()
        {
            
    
        }


            public void LoadVideo()
            {
                //http://zahidakbar.wordpress.com/2011/06/27/using-the-vlc-activex-control-in-wpf/ 
                vlc.playlist.stop();

                string local = Path("Movie(.mp4)|*.mp4|All files(*.*)|*.*");
                if (local != "")
                {

                    vlc.playlist.add("file:///" + local);

                   // vlc.playlist.add("file:///" + local, null, AXVLC.VLCPlaylistMode.VLCPlayListReplaceAndGo, 0);

                    
                }
            }

            public void Play()
            {
                
                // bad code but input.lenght is to slow to give lenght off movie. 
                vlc.playlist.play();
                Thread.Sleep(150);
                vlc.playlist.togglePause();
                LenghtMovie = TimeSpan.FromMilliseconds(vlc.input.Length);
                vlc.playlist.stop();
                vlc.playlist.play();
            }

           

    }
}
