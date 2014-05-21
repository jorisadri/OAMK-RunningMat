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
    public class ExcelAngleData : BaseClass
    {
        //Timer for put phoneangles in excel
        public DispatcherTimer PhoneAngletimer;
        public int counter = 0;

        string FileName;


        public Workbook PhoneData = new Workbook();
        Worksheet worksheet;
        public bool PhoneDataExcist = false;

        public string[,] xlInfo;  // var for Excel data


        public int TotalSamples;
        double[] PlotAngles;

        private string _samplefrequentie = "1";
        public string SampleFrequentie
        {
            get { return _samplefrequentie; }
            set { _samplefrequentie = value; RaisePropChanged("SampleFrequentie"); }
        }

        private string _Xangle = "0";
        public string XAngle
        {
            get { return _Xangle; }
            set { _Xangle = value; RaisePropChanged("XAngle"); }
        }

        private string _Yangle = "0";
        public string YAngle
        {
            get { return _Yangle; }
            set { _Yangle = value; RaisePropChanged("YAngle"); }
        }

        public ExcelAngleData()
        {
            worksheet = new Worksheet("angles");
            PhoneAngletimer = new DispatcherTimer();
            //samplerate for saving phone angles can changed here.
            PhoneAngletimer.Interval = TimeSpan.FromMilliseconds(200);
            PhoneAngletimer.Tick += PhoneAngletimer_Tick;


        }

        void PhoneAngletimer_Tick(object sender, EventArgs e)
        {
            // if phone sends data the data will be safed in excel. Samplerate is 5 samples/sec
            if (App.PhoneConnection.started)
            {
                worksheet.Cells[counter, 0] = new Cell(App.PhoneConnection.PhonePitch);
                worksheet.Cells[counter, 1] = new Cell(App.PhoneConnection.PhoneRoll);
                counter++;
            }
        }
        // opens window to choose location for 
        public void NewExcel()
        {
            //
            counter = 0;
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select a folder";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FileName = Path.Combine(dlg.SelectedPath, "PhoneAngles" + DateTime.Now.ToString("dd;MM;yyyy;HH,mm,ss") + ".xls");
                    PhoneData.Worksheets.Add(worksheet);
                    PhoneDataExcist = true;
                }
                
            }
        }

        //function to safe excel 
        public void Safe()
        {
            App.PhoneConnection.started = false;
            PhoneDataExcist = false;
            System.Windows.MessageBox.Show("File is Safed");
            App.Excel.counter = 0;

        }
        // Opens Excel file
        public void GetExcel()
        {
            try
            {
                //https://code.google.com/p/excellibrary/
                //"Excel(.xls)|*.xls|All files(*.*)|*.*" to filter data in folderwindow
                Workbook book = Workbook.Load(PathFile("Excel(.xls)|*.xls|All files(*.*)|*.*"));
                Worksheet sheet = book.Worksheets[0];
                TotalSamples = sheet.Cells.LastRowIndex;
                xlInfo = new string[2, TotalSamples];
                PlotAngles = new double[TotalSamples + 1];
                PlotAngles[0] = 0;

                for (int i = 0; i < sheet.Cells.LastRowIndex; i++)
                {   // values 
                    xlInfo[0, i] = Convert.ToString(sheet.Cells[i + 1, 0]);
                    if (Convert.ToDouble(xlInfo[0, i]) > 10)
                        xlInfo[0, i] = "10";
                    else if (Convert.ToDouble(xlInfo[0, i]) < -10)
                        xlInfo[0, i] = "-10";
                    xlInfo[1, i] = Convert.ToString(sheet.Cells[i, 1]);
                    if (Convert.ToDouble(xlInfo[1, i]) > 6)
                        xlInfo[1, i] = "6";
                    else if (Convert.ToDouble(xlInfo[1, i]) < -6)
                        xlInfo[1, i] = "-6";
                    
                    //angles put in a slope. If this is plotted you should see the different heights of the path
                    PlotAngles[i + 1] = Math.Tan(Convert.ToDouble(xlInfo[0, i])) + PlotAngles[i];
                }

                // http://stackoverflow.com/questions/16079956/does-npoi-have-support-to-xlsx-format For opening xlsx files. 
            }

            catch (Exception)
            {
                System.Windows.MessageBox.Show("Wrong File. Choose the correct file (Excel .xls)");
                GetExcel();
            }

        }
        // Calculates the samplerate per sec only posible if movie already been loaded
        public void Samplerate()
        {
            SampleFrequentie = (1 / ((App.VLCVideo.TotalMovieTime / 1000) / TotalSamples)).ToString();
        }

    }
}
