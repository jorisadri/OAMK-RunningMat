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
        MccDaq.DigitalLogicState triggerX = new MccDaq.DigitalLogicState();
        MccDaq.DigitalLogicState triggerY = new MccDaq.DigitalLogicState();
       
        int XChannelin = 0;
        int YChannelin = 1;
       
        

        short OutDataValueY;
        float OutDataValueEngUnitsY;

        short OutDataValueX;
        float OutDataValueEngUnitsX;

        double[] LastRecorded; 
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
            LastRecorded= new double[10];
            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            
        }

        
       public void GetDataX()
        {
           

                USB1.AIn(XChannelin, MccDaq.Range.Bip5Volts, out OutDataValueX);
                USB1.ToEngUnits(MccDaq.Range.Bip5Volts, OutDataValueX, out OutDataValueEngUnitsX);
                InputPotentiometerX = (OutDataValueEngUnitsX * 100) + 95;


                if (InputPotentiometerX >= 190)
                {
                    InputPotentiometerX = Map(InputPotentiometerX, 190, 360, 0, 10.2);
                }

                else if (InputPotentiometerX < 190)
                {
                    InputPotentiometerX = Map(InputPotentiometerX, 190, 0, 0, -12.4);
                }

            
        }


        public void SafeTrigger()
        {

            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            
            USB1.DBitIn(MccDaq.DigitalPortType.FifthPortA,0 ,out triggerX);


            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
           
            USB1.DBitIn(MccDaq.DigitalPortType.FifthPortA, 1, out triggerY);

            if (triggerX <= 0 || triggerY <= 0)
            {
               
            }


        }

       public void GetDataY()
       {
            
           USB1.AIn(YChannelin, MccDaq.Range.Bip5Volts, out OutDataValueY);
           USB1.ToEngUnits(MccDaq.Range.Bip5Volts, OutDataValueY, out OutDataValueEngUnitsY);
           InputPotentiometerY = OutDataValueEngUnitsY * 100;

           if (InputPotentiometerY >= 73)
           {
               InputPotentiometerY = Map(InputPotentiometerY, 73, 83.5, 0, -6);
           }

           else if (InputPotentiometerY < 73)
           {
               InputPotentiometerY = Map(InputPotentiometerY, 62.5, 73, 6, 0);
           }
       
       
       }
   
   
        
    }
}
