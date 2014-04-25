using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RunningMat
{
    public class BaseClass :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropChanged(string toraise)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(toraise));
            }
        }

        
             public  double Map ( double value, double fromSource, double toSource, double fromTarget, double toTarget)
              {
                  return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
              }

      
        
       

        

       public string Path(string filter)
        {
            string mrl = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = filter;
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                mrl = ofd.FileName;
            }
            else
            { mrl = ""; }

            return mrl;
        }
    }
}
