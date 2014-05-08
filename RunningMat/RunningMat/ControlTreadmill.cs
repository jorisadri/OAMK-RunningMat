using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RunningMat
{
   public class ControlTreadmill:BaseClass
    {
        int TimeNowX;
       // double LasTimeX;
       // double TimechangeX;
        int XChannel = 0;
        int YChannel = 1;
        ushort XMotorControlerFreq = 65500;
        ushort YMotorControlerFreq = 30000;
        // bool Ready = false;
        public double[,] posi = new double[2,10000];
        public bool Stop = false;
        double TimeStampX;
        int Position;
        public  bool Test=false;

        MccDaq.DigitalPortDirection PortDirection = MccDaq.DigitalPortDirection.DigitalOut;
      

        MccDaq.DigitalLogicState HighLogicState = MccDaq.DigitalLogicState.High;
        MccDaq.DigitalLogicState LowLogicState = MccDaq.DigitalLogicState.Low;
       
        MccDaq.DigitalPortType PortAux1 = MccDaq.DigitalPortType.AuxPort;
        MccDaq.DigitalPortType PortAux2 = MccDaq.DigitalPortType.AuxPort;

        MccDaq.DigitalPortType PortAux3 = MccDaq.DigitalPortType.AuxPort;
        MccDaq.DigitalPortType PortAux4 = MccDaq.DigitalPortType.AuxPort;
        
        public BackgroundWorker MotorControllerX = new BackgroundWorker();
        private MccDaq.MccBoard USBControler = new MccDaq.MccBoard(1);

        private double _testsliderX = 0.0;
        public double TestSliderX
        {
            get { return _testsliderX; }
            set { _testsliderX = value; RaisePropChanged("TestSliderX"); }
        }

        private double _testsliderY = 0.0;
        public double TestSliderY
        {
            get { return _testsliderY; }
            set { _testsliderY = value; RaisePropChanged("TestSliderY"); }
        }

        public ControlTreadmill()
        {

            MotorControllerX.DoWork += MotorController_DoWork;
            MotorControllerX.RunWorkerCompleted += MotorController_RunWorkerCompleted;
        }





        void MotorController_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!Stop)
            {
                MotorControllerX.RunWorkerAsync();
            }

            
          
               
            
            
        }

        void MotorController_DoWork(object sender, DoWorkEventArgs e)
        {

            App.DataPotentiometer.GetDataX();
            App.DataPotentiometer.GetDataY();

            if (Test)
            {
                if (TestSliderX > App.DataPotentiometer.InputPotentiometerX - 2)
                {
                    ForwardX();
                }

                else if (TestSliderX < App.DataPotentiometer.InputPotentiometerX + 2)
                {
                    BackwardX();

                }

                else
                    MotorStopPitch();


                if (TestSliderY < App.DataPotentiometer.InputPotentiometerY -1)
                {
                    RighY();
                }

                else if (TestSliderY > App.DataPotentiometer.InputPotentiometerY +1)
                {
                    LeftY();

                }

                else
                    MotorStopRoll(); 

            }

            else
            {
                // Because the movie is running on another thread with this code you run the function on the UI thread (current thread) 
                //http://stackoverflow.com/questions/11625208/accessing-ui-main-thread-safely-in-wpf
                Application.Current.Dispatcher.Invoke(new Action(() => { TimeStampX = App.VLCVideo.Movie.Position.TotalMilliseconds; }));

                Position = Convert.ToInt32(TimeStampX / (1000 / Convert.ToDouble(App.Excel.SampleFrequentie)));

                App.Excel.XAngle = App.Excel.xlInfo[0, Position];
                App.Excel.YAngle = App.Excel.xlInfo[1, Position];


                if (Convert.ToDouble(App.Excel.XAngle) < App.DataPotentiometer.InputPotentiometerX - 2)
                    BackwardX();

                else if (Convert.ToDouble(App.Excel.XAngle) > App.DataPotentiometer.InputPotentiometerX + 2)
                    ForwardX();

                else
                    MotorStopPitch();


                if (Convert.ToDouble(App.Excel.YAngle) < App.DataPotentiometer.InputPotentiometerY - 1)
                    RighY();


                else if (Convert.ToDouble(App.Excel.YAngle) > App.DataPotentiometer.InputPotentiometerY + 1)
                    LeftY();

                else
                    MotorStopRoll();


            }
        }

       
        
        
        public void BackwardX()
        {
            USBControler.DConfigBit(PortAux1, 0, PortDirection);
            USBControler.DBitOut(PortAux1, 0, HighLogicState);
            USBControler.DConfigBit(PortAux2, 1, PortDirection);
            USBControler.DBitOut(PortAux2, 1, LowLogicState);
            USBControler.AOut(XChannel, MccDaq.Range.Bip10Volts, XMotorControlerFreq);
        }

        public void ForwardX()
        {
            USBControler.DConfigBit(PortAux1, 0, PortDirection);
            USBControler.DBitOut(PortAux1, 0, LowLogicState);
            USBControler.DConfigBit(PortAux2, 1, PortDirection);
            USBControler.DBitOut(PortAux2, 1, HighLogicState);
            USBControler.AOut(XChannel, MccDaq.Range.Bip10Volts, XMotorControlerFreq);

        }

       
        public void MotorStopPitch()
        {
            USBControler.AOut(XChannel, MccDaq.Range.Bip10Volts, 0);
            USBControler.DConfigBit(PortAux1, 0, PortDirection);
            USBControler.DBitOut(PortAux1, 0, LowLogicState);

            USBControler.DConfigBit(PortAux2, 1, PortDirection);
            USBControler.DBitOut(PortAux2, 1, LowLogicState);
        }

        public void MotorStopRoll()
        {
            USBControler.AOut(YChannel, MccDaq.Range.Bip10Volts, 0);
            USBControler.DConfigBit(PortAux3, 2, PortDirection);
            USBControler.DBitOut(PortAux3, 2, LowLogicState);

            USBControler.DConfigBit(PortAux4, 3, PortDirection);
            USBControler.DBitOut(PortAux4, 3, LowLogicState);  
        }

        public void RighY()
        {
            USBControler.DConfigBit(PortAux3, 2, PortDirection);
            USBControler.DBitOut(PortAux3, 2, HighLogicState);
            USBControler.DConfigBit(PortAux4, 3, PortDirection);
            USBControler.DBitOut(PortAux4, 3, LowLogicState);
            USBControler.AOut(YChannel, MccDaq.Range.Bip10Volts, YMotorControlerFreq);
        }

        public void LeftY()
        {
            USBControler.DConfigBit(PortAux3, 2, PortDirection);
            USBControler.DBitOut(PortAux3, 2, LowLogicState);
            USBControler.DConfigBit(PortAux4, 3, PortDirection);
            USBControler.DBitOut(PortAux4, 3, HighLogicState);
            USBControler.AOut(YChannel, MccDaq.Range.Bip10Volts, YMotorControlerFreq);
        }

      


    }
}
