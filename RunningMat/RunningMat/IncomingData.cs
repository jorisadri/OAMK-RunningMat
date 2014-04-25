using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
namespace RunningMat
{
   public class IncomingData:BaseClass
    {
        private MccDaq.MccBoard USB1 = new MccDaq.MccBoard(0);
        public BackgroundWorker GetDataX = new BackgroundWorker();
        int XChannelin = 0;
        int YChannelin = 1;

        short OutDataValueY;
        float OutDataValueEngUnitsY;

        short OutDataValueX;
        float OutDataValueEngUnitsX;
      //  MccDaq.Range TheRange;

        private double _inputpotentiometerX = 0.0;
        public double InputPotentiometerX 
        {
            get { return _inputpotentiometerX;}
            set { _inputpotentiometerX = value; RaisePropChanged("InputPotentiometerX"); }
        }

        private double _inputpotentiometerY = 0.0;
        public double InputPotentiometerY 
        {
            get { return _inputpotentiometerY; }
            set { _inputpotentiometerY = value; RaisePropChanged("InputPotentiometerY"); }
        }

        public IncomingData()
        {   
            GetDataX.DoWork+=GetDataX_DoWork;      
        }

        void GetDataX_DoWork(object sender, DoWorkEventArgs e)
        {   
 	       USB1.AIn(XChannelin, MccDaq.Range.Bip5Volts, out OutDataValueX);
           USB1.ToEngUnits(MccDaq.Range.Bip5Volts, OutDataValueX, out OutDataValueEngUnitsX);
           InputPotentiometerX = OutDataValueEngUnitsX * 100;
         
           if (InputPotentiometerX>=93)
           {
               InputPotentiometerX = Map(InputPotentiometerX, 93, 260, 0, 10);
           }

           else if (InputPotentiometerX< 93)
           {
                 InputPotentiometerX = Map(InputPotentiometerX, -90, 92, -11.5,-1 );
           }
       
        }


        public void SafeTrigger()
        {
            MccDaq.DigitalLogicState triggerX = new MccDaq.DigitalLogicState() ;
            MccDaq.DigitalLogicState triggerY = new MccDaq.DigitalLogicState();
            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            USB1.DBitIn(MccDaq.DigitalPortType.FifthPortA,0 ,out triggerX);

            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            USB1.DBitIn(MccDaq.DigitalPortType.FifthPortA, 1, out triggerY);

            if (triggerX==0||triggerY==0)
            {
                App.controltreadmill.MotorStop();
            }


        }

     //  public void GetDataX()
       

          
           
       

       public void GetDataY()
       {
           USB1.AIn(YChannelin, MccDaq.Range.Bip5Volts, out OutDataValueY);
           USB1.ToEngUnits(MccDaq.Range.Bip5Volts, OutDataValueY, out OutDataValueEngUnitsY);
           InputPotentiometerY = OutDataValueEngUnitsY * 100;
       }
        
    }
}
