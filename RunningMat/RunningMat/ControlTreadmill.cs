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
        
        int XChannel = 0;
        int YChannel = 1;
        ushort XMotorControlerFreq = 65500;
        ushort YMotorControlerFreq = 30000;
        // bool Ready = false;
        public double[,] posi = new double[2,10000];
        public bool Stop = false;
        double TimeStampX;
        int Position;
        
        

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
            set {
                _testsliderX = value; RaisePropChanged("TestSliderX");

                if (App.UIController.choise==App.Choise.Test)
                {
                    App.Excel.XAngle = TestSliderX.ToString();
                }
                }
        }

        private double _testsliderY = 0.0;
        public double TestSliderY
        {
            get { return _testsliderY; }
            set { _testsliderY = value; RaisePropChanged("TestSliderY");
            if (App.UIController.choise == App.Choise.Test)
            {
                App.Excel.YAngle = TestSliderY.ToString();
            }
            }
        }


        private bool _check = true;
        public bool Check
        {
            get { return _check; }
            set {
                if (value==false&&App.PhoneConnection.started )
                {
                     _check = value; RaisePropChanged("Check");
                     FCheck = true;
                }
                else 
                {
                    MessageBox.Show("No connection from phone \nOpen SensorUDP on android. Use this IP:" + App.PhoneConnection.LocalIPAddress() + " The port number is:" + App.PhoneConnection.port + "\nSelect Orientation and select Network");
                    _check = true; RaisePropChanged("Check");
                    FCheck = false;
                }
               
                }
        }

        private bool _fcheck;
        public bool FCheck
        {
            get { return _fcheck; }
            set
            {
                _fcheck = value; RaisePropChanged("FCheck");

            }
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

            if (Check)
            {
                App.DataPotentiometer.GetDataX();
                App.DataPotentiometer.GetDataY();
            }
           

             if (App.UIController.choise==App.Choise.Test)
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


                    if (TestSliderY < App.DataPotentiometer.InputPotentiometerY - 1)
                    {
                        RighY();
                    }

                    else if (TestSliderY > App.DataPotentiometer.InputPotentiometerY + 1)
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
