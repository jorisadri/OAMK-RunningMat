using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace RunningMat
{
    public class BaseClass : INotifyPropertyChanged
    {
        //This event is used for updating the UI
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropChanged(string toraise)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(toraise));
        }


        //This function is used for map values between given numbers
        //http://stackoverflow.com/questions/14353485/how-do-i-map-numbers-in-c-sharp-like-with-map-in-arduino
        public double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;

        }

        // function gives the location from file
        public string PathFile(string filter)
        {
            string mrl = "";
            OpenFileDialog ofd = new OpenFileDialog();
            if (filter!="")
               ofd.Filter = filter;
            
            ofd.ShowDialog();
            if (ofd.FileName != "")
                mrl = ofd.FileName;
            else
             mrl = ""; 
            return mrl;
        }
    }
}

