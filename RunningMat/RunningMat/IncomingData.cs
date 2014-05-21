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
    public class IncomingData : BaseClass
    {   // USB connection for safety triggers
        private MccDaq.MccBoard USB1 = new MccDaq.MccBoard(0);
        MccDaq.DigitalLogicState triggerX = new MccDaq.DigitalLogicState();
        MccDaq.DigitalLogicState triggerY = new MccDaq.DigitalLogicState();

        //channels Potentiometers
        int XChannelin = 0;
        int YChannelin = 1;
        //variables for data potentiometers
        short OutDataValueY;
        float OutDataValueEngUnitsY;

        short OutDataValueX;
        float OutDataValueEngUnitsX;

        private double _inputpotentiometerX = 0.0;
        public double InputPotentiometerX
        {
            get { return _inputpotentiometerX; }
            set { _inputpotentiometerX = value; RaisePropChanged("InputPotentiometerX"); }
        }

        private double _inputpotentiometerY = 0.0;
        public double InputPotentiometerY
        {
            get { return _inputpotentiometerY; }
            set { _inputpotentiometerY = value; RaisePropChanged("InputPotentiometerY"); }
        }

        public IncomingData()
        {   //USB Ports for trigger
            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
        }


        //get angle pitch
        public void GetDataX()
        {
            USB1.AIn(XChannelin, MccDaq.Range.Bip5Volts, out OutDataValueX);
            USB1.ToEngUnits(MccDaq.Range.Bip5Volts, OutDataValueX, out OutDataValueEngUnitsX);
            //potentiometer go`s from -95 to 265 so make it 0-360
            InputPotentiometerX = (OutDataValueEngUnitsX * 100) + 95;

           // map pitch value to degrees. 190 is 0 degrees, 360 = 10.2 degrees  0= -12,4 degrees (here is poibility to calibrate the potentiometers)
            if (InputPotentiometerX >= 190)
                InputPotentiometerX = Map(InputPotentiometerX, 190, 360, 0, 10.2);
            else if (InputPotentiometerX < 190)
                InputPotentiometerX = Map(InputPotentiometerX, 190, 0, 0, -12.4);
        }

        //get angle roll
        public void GetDataY()
        {
            USB1.AIn(YChannelin, MccDaq.Range.Bip5Volts, out OutDataValueY);
            USB1.ToEngUnits(MccDaq.Range.Bip5Volts, OutDataValueY, out OutDataValueEngUnitsY);
            InputPotentiometerY = OutDataValueEngUnitsY * 100;

            //map roll value to degrees. 73 is 0 degrees, 83,5 = -6 degrees  62.5= 6 degrees (here is poibility to calibrate the potentiometers)
            if (InputPotentiometerY >= 73)
                InputPotentiometerY = Map(InputPotentiometerY, 73, 83.5, 0, -6);
            else if (InputPotentiometerY < 73)
                InputPotentiometerY = Map(InputPotentiometerY, 62.5, 73, 6, 0);
        }

        //read the safetytriggers (not working yet)
        public void SafeTrigger()
        {
            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            USB1.DBitIn(MccDaq.DigitalPortType.FifthPortA, 0, out triggerX);
            USB1.DConfigPort(MccDaq.DigitalPortType.FifthPortA, MccDaq.DigitalPortDirection.DigitalIn);
            USB1.DBitIn(MccDaq.DigitalPortType.FifthPortA, 1, out triggerY);
            //if (triggerX <= 0 || triggerY <= 0)
            //{
            //}
        }



    }
}
