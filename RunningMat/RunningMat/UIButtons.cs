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
        // controlls UIfunctions
        public App.PlayState status = new App.PlayState();
        public App.Choise choise = new App.Choise();

        public UIButtons()
        {

        }

        //function that checks the states of the buttons and do the needed staps
        public void DoYourThing()
        {
            switch (choise)
            {  //runs when run button is pressed
                case App.Choise.Run:
                    {
                        switch (status)
                        {
                            case App.PlayState.Load:
                                {
                                    App.VLCVideo.Movie.Stop();
                                    App.controltreadmill.Stop = true;
                                    App.controltreadmill.MotorStopPitch();
                                    App.controltreadmill.MotorStopRoll();
                                    App.VLCVideo.LoadVideo();
                                    App.Excel.GetExcel();
                                    App.Excel.Samplerate();
                                    break;
                                }
                            case App.PlayState.Start:
                                {
                                    if (App.VLCVideo.Movie.IsLoaded && !App.controltreadmill.MotorControllerX.IsBusy && App.SpeedTreadmill.Arduino)
                                    {
                                        App.VLCVideo.Movie.Play();
                                        App.controltreadmill.Stop = false;
                                        App.controltreadmill.MotorControllerX.RunWorkerAsync();
                                        App.SpeedTreadmill.GetSpeedTimer.Start();
                                    }
                                    else if (App.VLCVideo.Movie.IsLoaded && !App.controltreadmill.MotorControllerX.IsBusy)
                                    {
                                        App.VLCVideo.Movie.Play();
                                        App.controltreadmill.Stop = false;
                                        App.controltreadmill.MotorControllerX.RunWorkerAsync();
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
                //runs when makeangle button is pressed
                case App.Choise.MakeAngle:
                    {
                        switch (status)
                        {

                            case App.PlayState.Load:
                                {
                                    App.VLCVideo.Movie.Stop();
                                    App.controltreadmill.Stop = true;
                                    App.controltreadmill.MotorStopPitch();
                                    App.controltreadmill.MotorStopRoll();
                                    MessageBox.Show("Open SensorUDP on android. Use this IP:" + App.PhoneConnection.LocalIPAddress() + " The port number is:" + App.PhoneConnection.port + "\n Select Orientation and select Network");
                                    App.VLCVideo.LoadVideo();
                                    App.Excel.NewExcel();
                                    break;
                                }
                            case App.PlayState.Start:
                                {
                                    if (!App.VLCVideo.Movie.HasVideo)
                                    {
                                        MessageBox.Show("No video choose video");
                                        App.VLCVideo.LoadVideo();
                                    }

                                    if (!App.Excel.PhoneDataExcist)
                                    {
                                        MessageBox.Show("No excel choose location");
                                        App.Excel.NewExcel();
                                    }

                                    if (App.VLCVideo.Movie.IsLoaded && App.PhoneConnection.started && App.Excel.PhoneDataExcist)
                                    {
                                        App.VLCVideo.Movie.Play();
                                        App.Excel.PhoneAngletimer.Start();
                                    }
                                    else
                                        MessageBox.Show("No connection from phone Or video not selected \nOpen SensorUDP on android. Use this IP:" + App.PhoneConnection.LocalIPAddress() + " The port number is:" + App.PhoneConnection.port + "\nSelect Orientation and select Network");
                                        
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
                //runs when test button is pressed
                case App.Choise.Test:
                    {
                        App.VLCVideo.Movie.Stop();
                        switch (status)
                        {
                            case App.PlayState.Load:
                                {
                                    App.controltreadmill.Stop = true;
                                    App.controltreadmill.MotorStopPitch();
                                    App.controltreadmill.MotorStopRoll();
                                    break;
                                }
                            case App.PlayState.Start:
                                {
                                    if (!App.controltreadmill.MotorControllerX.IsBusy)
                                    {
                                        App.controltreadmill.Stop = false;
                                        App.controltreadmill.MotorControllerX.RunWorkerAsync();
                                    }
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
