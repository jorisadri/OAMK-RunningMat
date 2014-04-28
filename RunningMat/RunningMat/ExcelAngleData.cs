using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelLibrary;
using ExcelLibrary.SpreadSheet;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Xml.Linq;

namespace RunningMat
{
   public class ExcelAngleData:BaseClass
    {
        public string[,] xlInfo;  // var for Excel data

        int time = 0;
        public int TotalSamples;
        double[] PlotAngles;

        private string _samplefrequentie="1";
        public string SampleFrequentie {
            get { return _samplefrequentie; }
            set { _samplefrequentie = value; RaisePropChanged("SampleFrequentie"); }
        }

        private string _Xangle="0";
        public string XAngle 
        {
            get { return _Xangle; }
            set { _Xangle = value; RaisePropChanged("XAngle"); }
        }

        private string _Yangle="0";
        public string YAngle 
        {
            get { return _Yangle; }
            set { _Yangle = value; RaisePropChanged("YAngle"); }
        }

        public ExcelAngleData() 
        {
           
        }

        public void GetExcel()
        {
            try
            {
                //NextAngleTimer.Interval = TimeSpan.FromMilliseconds(1000 / Convert.ToDouble(SampleFrequentie));
                //https://code.google.com/p/excellibrary/

                Workbook book = Workbook.Load(Path("Excel(.xls)|*.xls|All files(*.*)|*.*"));
                Worksheet sheet = book.Worksheets[0];
                TotalSamples = sheet.Cells.LastRowIndex;
                xlInfo = new string[2, TotalSamples];
                PlotAngles = new double[TotalSamples+1];
                PlotAngles[0] = 0;

                for (int i = 0; i < sheet.Cells.LastRowIndex; i++)
                {
                    xlInfo[0, i] = Convert.ToString(sheet.Cells[i+1, 0]);
                    xlInfo[1, i] = Convert.ToString(sheet.Cells[i, 1]);
                    PlotAngles[i+1] = Math.Tan(Convert.ToDouble(xlInfo[0, i])) + PlotAngles[i];

                }

                // http://stackoverflow.com/questions/16079956/does-npoi-have-support-to-xlsx-format For opening xlsx files. 
            }

            catch (Exception)
            {

                MessageBox.Show("Wrong File. Choose the correct file (Excel .xls)");
                GetExcel();
            }
            
        }

        public void Samplerate()
        {
           SampleFrequentie = (1 / ((App.VLCVideo.TotalMovieTime / 1000) / TotalSamples)).ToString();
        }


        //void NextAngleTimer_Tick(object sender, EventArgs e)
        //{
        //    if (time < (xlInfo.Length/2)-1)
        //        time++;
            
        //    else
        //        time = 0;
            
        //    XAngle = xlInfo[0, time];
        //    YAngle = xlInfo[1, time];

           
        //    NextAngleTimer.Interval = TimeSpan.FromMilliseconds(1000/Convert.ToDouble(SampleFrequentie));
           
        //}

    }
}
