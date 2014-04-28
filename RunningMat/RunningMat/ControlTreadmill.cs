using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RunningMat
{
   public class ControlTreadmill:BaseClass
    {
        int TimeNowX;
       // double LasTimeX;
       // double TimechangeX;
        int XChannel = 0;
        ushort XMotorControlerFreq=65000;
        // bool Ready = false;
        public double[,] posi = new double[2,10000];
 
        double TimeStampX;
        int Position;
        //MccDaq.Range TheRange = new MccDaq.Range();
 
        MccDaq.DigitalPortDirection PortDirection = MccDaq.DigitalPortDirection.DigitalOut;
      
        MccDaq.DigitalLogicState HighLogicState = MccDaq.DigitalLogicState.High;
        MccDaq.DigitalLogicState LowLogicState = MccDaq.DigitalLogicState.Low;
       
        MccDaq.DigitalPortType PortAux1 = MccDaq.DigitalPortType.AuxPort;
        MccDaq.DigitalPortType PortAux2 = MccDaq.DigitalPortType.AuxPort;

        MccDaq.DigitalPortType PortAux3 = MccDaq.DigitalPortType.AuxPort;
        MccDaq.DigitalPortType PortAux4 = MccDaq.DigitalPortType.AuxPort;
        
        public BackgroundWorker MotorControllerX = new BackgroundWorker();
        private MccDaq.MccBoard USBControler = new MccDaq.MccBoard(1);

        private double _testslider = 93.0;
        public double TestSlider
        {
            get { return _testslider; }
            set { _testslider = value; RaisePropChanged("TestSlider"); }
        }


        public ControlTreadmill()
        {
          ////  PortDirection = new MccDaq.DigitalPortDirection();
          //  PortDirection= MccDaq.DigitalPortDirection.DigitalOut;
          //  HighLogicState = MccDaq.DigitalLogicState.High;
          //  LowLogicState = MccDaq.DigitalLogicState.Low;

            MotorControllerX.DoWork += MotorController_DoWork;
            MotorControllerX.RunWorkerCompleted += MotorController_RunWorkerCompleted;
        }




        void MotorController_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MotorControllerX.RunWorkerAsync();
        }

        void MotorController_DoWork(object sender, DoWorkEventArgs e)
        {

            TimeNowX++;
            // Because the movie is running on another thread with this code you run the function on the UI thread (current thread) 
            //http://stackoverflow.com/questions/11625208/accessing-ui-main-thread-safely-in-wpf
            Application.Current.Dispatcher.Invoke(new Action(() => { TimeStampX = App.VLCVideo.Movie.Position.TotalMilliseconds; }));
            
           
            Position=Convert.ToInt32(TimeStampX/(1000/Convert.ToDouble(App.Excel.SampleFrequentie)));
           
            App.DataPotentiometer.GetDataX();
            App.DataPotentiometer.GetDataY();
         
           

            //posi[0,TimeNowX] = Position;
            //posi[1, TimeNowX] = TimeStampX;
          
            
            App.Excel.XAngle = App.Excel.xlInfo[0, Position];

           
            App.DataPotentiometer.SafeTrigger();


            if (Convert.ToDouble(App.Excel.XAngle) < App.DataPotentiometer.InputPotentiometerX - 1)
            {
            
                
                ForwardX();
            }

            else if (Convert.ToDouble(App.Excel.XAngle) > App.DataPotentiometer.InputPotentiometerX + 1)
            {

                BackwardX();
            }
            else
            {
                MotorStop();
               // USBControler.AOut(XChannel, MccDaq.Range.Bip10Volts, XMotorControlerFreq);
                
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

        public void MotorStop()
        {
            USBControler.AOut(XChannel, MccDaq.Range.Bip10Volts, 0);
            USBControler.DConfigBit(PortAux1, 0, PortDirection);
            USBControler.DBitOut(PortAux1, 0, LowLogicState);

            USBControler.DConfigBit(PortAux2, 1, PortDirection);
            USBControler.DBitOut(PortAux2, 1, LowLogicState);

            USBControler.DConfigBit(PortAux3, 2, PortDirection);
            USBControler.DBitOut(PortAux3, 2, LowLogicState);

            USBControler.DConfigBit(PortAux4, 3, PortDirection);
            USBControler.DBitOut(PortAux4, 3, LowLogicState);

            
        }


    }
}
