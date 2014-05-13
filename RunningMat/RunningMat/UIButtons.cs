using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RunningMat
{
   public class UIButtons
    {

        
        public App.PlayState status = new App.PlayState();
       
        public App.Choise choise = new App.Choise();




        public void DoYourThing()
        {

            switch (choise)
            {
                case App.Choise.Run:
                    {
                        switch (status)
                        {
                            case App.PlayState.Load:
                                {
                                    App.VLCVideo.Movie.Stop();
                                    App.controltreadmill.Stop = true;
                                    App.VLCVideo.LoadVideo();
                                    App.Excel.GetExcel();
                                    
                                    break;
                                }
                            case App.PlayState.Start:
                                {
                                    if (App.VLCVideo.Movie.IsLoaded && !App.controltreadmill.MotorControllerX.IsBusy && App.SpeedTreadmill.Arduino)
                                    {
                                        App.VLCVideo.Movie.Play();
                                        App.controltreadmill.MotorControllerX.RunWorkerAsync();
                                        App.controltreadmill.Stop = false;
                                        App.SpeedTreadmill.GetSpeedTimer.Start();
                                    }

                                    else
                                    {
                                        App.VLCVideo.Movie.Play();
                                        App.controltreadmill.MotorControllerX.RunWorkerAsync();
                                        App.controltreadmill.Stop = false;

                                    }
                                    break;
                                }
                            case App.PlayState.Pauze:
                                {
                                    App.VLCVideo.Movie.Pause();
                                    App.controltreadmill.Stop = true;
                                    App.controltreadmill.MotorStopPitch();
                                    App.controltreadmill.MotorStopRoll();
                                   
                                    break;
                                }
                            case App.PlayState.Stop:
                                {
                                    App.VLCVideo.Movie.Stop();
                                    App.controltreadmill.Stop = true;
                                    App.controltreadmill.MotorStopPitch();
                                    App.controltreadmill.MotorStopRoll();
                                    break;
                                }
                        }
                        break;
                    }
                case App.Choise.MakeAngle:
                    {
                        
                        
                        switch (status)
                        {

                            case App.PlayState.Load:
                                {
                                    break;
                                }
                            case App.PlayState.Start:
                                {
                                    if (!App.VLCVideo.Movie.HasVideo )
                                    {
                                        MessageBox.Show("Open SensorUDP on android. Use this IP:" + App.PhoneConnection.LocalIPAddress() + " The port number is:" + App.PhoneConnection.port + "\n Select Orientation and select Network");
                                        App.VLCVideo.LoadVideo();
                                       
                                    }

                                    if (!App.Excel.PhoneDataExcist )
                                    {
                                        App.Excel.NewExcel();
                                    }



                                    if (App.VLCVideo.Movie.IsLoaded && App.PhoneConnection.started)
                                    {

                                        App.VLCVideo.Movie.Play();
                                        App.Excel.PhoneAngletimer.Start();
                                    }

                                    else
                                    {
                                        MessageBox.Show("No connection from phone \nOpen SensorUDP on android. Use this IP:" + App.PhoneConnection.LocalIPAddress() + " The port number is:" + App.PhoneConnection.port + "\nSelect Orientation and select Network");
                                    }


                                    break;
                                }
                            case App.PlayState.Pauze:
                                {
                                    App.Excel.PhoneAngletimer.Stop();
                                    App.VLCVideo.Movie.Pause();
                                    break;
                                }
                            case App.PlayState.Stop:
                                {
                                    App.Excel.PhoneAngletimer.Stop();
                                    App.VLCVideo.Movie.Stop();
                                    App.Excel.Safe();
                                    break;
                                }
                        }
                        break;
                    }

                case App.Choise.Test:
                    {
                        switch (status)
                        {
                            case App.PlayState.Load:
                                {
                                    break;
                                }
                            case App.PlayState.Start:
                                {
                                    App.controltreadmill.MotorControllerX.RunWorkerAsync();
                                    break;
                                }
                            case App.PlayState.Pauze:
                                {
                                    App.controltreadmill.Stop = true;
                                    App.controltreadmill.MotorStopPitch();
                                    App.controltreadmill.MotorStopRoll();
                                    break;
                                }
                            case App.PlayState.Stop:
                                {
                                    App.controltreadmill.Stop = true;
                                    App.controltreadmill.MotorStopPitch();
                                    App.controltreadmill.MotorStopRoll();
                                    break;
                                }
                        }
                        break;
                    }
            
            }

        
        }

       


    


     

        
        

    }
}
