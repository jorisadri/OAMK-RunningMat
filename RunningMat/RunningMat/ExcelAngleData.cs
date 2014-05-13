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
using System.Windows.Forms;
using System.IO;


namespace RunningMat
{
   public class ExcelAngleData:BaseClass
    {
       //link to the site were i found the chart
       //http://stackoverflow.com/questions/2377862/embedding-winforms-graph-in-wpf-window


       public DispatcherTimer PhoneAngletimer;
       int counter = 0;

       string FileName;

       public Workbook PhoneData = new Workbook();
       Worksheet worksheet = new Worksheet("AngleData");
       public bool PhoneDataExcist = false;

        public string[,] xlInfo;  // var for Excel data

        //int time = 0;
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
            PhoneAngletimer = new DispatcherTimer();
            PhoneAngletimer.Interval = TimeSpan.FromMilliseconds(200);
            PhoneAngletimer.Tick += PhoneAngletimer_Tick;

            
        }

        void PhoneAngletimer_Tick(object sender, EventArgs e)
        {
            
            if (App.PhoneConnection.started)
            {
                counter++;
                worksheet.Cells[counter, 0] = new Cell(App.PhoneConnection.PhonePitch);
                worksheet.Cells[counter, 1] = new Cell(App.PhoneConnection.PhoneRoll);

            }

            
            
        }
        public void NewExcel()
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select a folder";
                if (dlg.ShowDialog() == DialogResult.OK)
                {


                    FileName = Path.Combine(dlg.SelectedPath, "PhoneAngles" + DateTime.Now.ToString("dd;MM;yyyy;HH,mm,ss") + ".xls");

                }
            }

            
            PhoneData.Worksheets.Add(worksheet);
            PhoneDataExcist = true;
        }



        public void Safe()
        {
            App.PhoneConnection.started = false;
            PhoneData.Save(FileName);
            PhoneDataExcist = false;
        
        }
        
        public void GetExcel()
        {
            

            try
            {
                
                //https://code.google.com/p/excellibrary/

                Workbook book = Workbook.Load(PathFile("Excel(.xls)|*.xls|All files(*.*)|*.*"));
                Worksheet sheet = book.Worksheets[0];
                TotalSamples = sheet.Cells.LastRowIndex;
                xlInfo = new string[2, TotalSamples];
                PlotAngles = new double[TotalSamples+1];
                PlotAngles[0] = 0;

                for (int i = 0; i < sheet.Cells.LastRowIndex; i++)
                {
                    
                    xlInfo[0, i] = Convert.ToString(sheet.Cells[i+1, 0]);
                    if (Convert.ToDouble(xlInfo[0, i]) > 10)
                    {
                        xlInfo[0, i] = "10";
                    }

                    else if (Convert.ToDouble(xlInfo[0, i]) < -11)
                    {
                         xlInfo[0, i] = "-11";
                    }

                    
                    xlInfo[1, i] = Convert.ToString(sheet.Cells[i, 1]);
                    if (Convert.ToDouble(xlInfo[1, i]) > 6)
                    {
                        xlInfo[1, i] = "6";
                    }

                    else if (Convert.ToDouble(xlInfo[1, i]) < -6)
                    {
                        xlInfo[1, i] = "-6";
                    }
                   
                    PlotAngles[i+1] = Math.Tan(Convert.ToDouble(xlInfo[0, i])) + PlotAngles[i];
                    

                }

                // http://stackoverflow.com/questions/16079956/does-npoi-have-support-to-xlsx-format For opening xlsx files. 
            }

            catch (Exception)
            {

                System.Windows.MessageBox.Show("Wrong File. Choose the correct file (Excel .xls)");
                GetExcel();
            }
            
        }

        public void Samplerate()
        {
           SampleFrequentie = (1 / ((App.VLCVideo.TotalMovieTime / 1000) / TotalSamples)).ToString();
        }

    }
}
