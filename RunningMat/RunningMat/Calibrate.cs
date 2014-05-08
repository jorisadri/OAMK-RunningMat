using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningMat
{
    public class Calibrate:BaseClass
    {
       public double Right;
       public double Left;
       public double Front = 317;
       public double Back;

        public Calibrate()
        { 
        
        }

        public void StartCalibrating(int side)
        {      //Front
            if (side==1)
            {
                Front = App.DataPotentiometer.InputPotentiometerX + 95;

            }
                //Back
            else if (side==2)
            {
                Back = App.DataPotentiometer.InputPotentiometerX + 95;
            }
                //Right 
            else if (side==3)
            {
                Right = App.DataPotentiometer.InputPotentiometerY;
            }
                //Left
            else if (side==4)
            {
                Left = App.DataPotentiometer.InputPotentiometerY;
            }

            App.DataPotentiometer.InputPotentiometerX = Map(App.DataPotentiometer.InputPotentiometerX,Front, 0, 18.5,((360-Front)*(37/600)));
            if (App.DataPotentiometer.InputPotentiometerX >Front)
            {
                App.DataPotentiometer.InputPotentiometerX = Map(App.DataPotentiometer.InputPotentiometerX, Front, 360, ((Front-360)*(37/600)), ((360 - Front) * (37 / 600)));
            }

        }

    }
}
