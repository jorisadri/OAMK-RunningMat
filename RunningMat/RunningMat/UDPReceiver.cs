using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace RunningMat
{
   public  class UDPReceiver:BaseClass
    {

        //http://acrocontext.wordpress.com/2013/08/15/c-simple-udp-listener-in-asynchronous-way/
        private Socket udpSock;
        private byte[] buffer;
        public int port = 12345;
        string Received;
        double roll;
        double pitch;
        public bool started = false;
        int index;

        string[] seperator = new string[] { ", " };
        
    

        private double _phonepitch = 0.0;
        public double PhonePitch
        {
            get { return _phonepitch; }
            set { _phonepitch = value; RaisePropChanged("PhonePitch");
            if (App.UIController.choise == App.Choise.MakeAngle||App.controltreadmill.FCheck)
            {
                App.DataPotentiometer.InputPotentiometerX = PhonePitch;
            }
            
            }
        }

        public UDPReceiver()
        {
            Starter();
        }

        private double _phoneroll = 0.0;
        public double PhoneRoll
        {
            get { return _phoneroll; }
            set { _phoneroll = value; RaisePropChanged("PhoneRoll");
            if (App.UIController.choise == App.Choise.MakeAngle||App.controltreadmill.FCheck)
            {
                App.DataPotentiometer.InputPotentiometerY = PhoneRoll;
            }
            }
        }
        public void Starter()
        {

            //Setup the socket and message buffer
            udpSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSock.Bind(new IPEndPoint(IPAddress.Any, port));
            buffer = new byte[1024];

            //Start listening for a new message.
            EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
            udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, udpSock);
            
            
        }

        private void DoReceiveFrom(IAsyncResult iar)
        {
            try
            {
                //Get the received message.
                Socket recvSock = (Socket)iar.AsyncState;
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                int msgLen = recvSock.EndReceiveFrom(iar, ref clientEP);
                byte[] localMsg = new byte[msgLen];
                Array.Copy(buffer, localMsg, msgLen);

                //Start listening for a new message.
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, udpSock);

                //Handle the received message
                Received=Encoding.ASCII.GetString(localMsg,0, localMsg.Length);


                string[] ArrayReveive = Received.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
               

                if (App.controltreadmill.FCheck)
                {

                    if (ArrayReveive[0] == "O")
                    {
                        // if error here use this
                        //PhonePitch = Convert.ToDouble( ArrayReveive[index+4].Replace(',', '.'));
                        //PhoneRoll = Convert.ToDouble(ArrayReveive[index+5].Replace(',', '.'));

                        if (Convert.ToDouble(ArrayReveive[4]) > 10)
                        {
                            PhonePitch = 10;
                        }

                        else if (Convert.ToDouble(ArrayReveive[4]) < -10)
                        {
                            PhonePitch = -10;
                        }

                        else
                        {
                            PhonePitch = Convert.ToDouble(ArrayReveive[4]);
                            PhoneRoll = Convert.ToDouble(ArrayReveive[5]);
                        }

                        if ( Convert.ToDouble(ArrayReveive[5])>6)
                        {
                            PhoneRoll = 6;
                        }

                        else if (Convert.ToDouble(ArrayReveive[5])<-6)
                        {
                            PhoneRoll = -6;
                        }

                        else
                        PhoneRoll = Convert.ToDouble(ArrayReveive[5]);
                        
                    }
                }
                else
                {
                    if (ArrayReveive[0] == "O")
                    {
                        // if error here use this
                        //PhonePitch = Convert.ToDouble( ArrayReveive[index+4].Replace(',', '.'))/2,5;
                        //PhoneRoll = Convert.ToDouble(ArrayReveive[index+5].Replace(',', '.'))2,5;
                        if (Convert.ToDouble(ArrayReveive[4]) / 2.5>10)
                        {
                            PhonePitch = 10;
                        }

                        else if (Convert.ToDouble(ArrayReveive[4]) / 2.5 < -10)
                        {
                            PhonePitch = -10;
                        }

                        else
                        {
                            PhonePitch = Convert.ToDouble(ArrayReveive[4]) / 2.5;
                            
                        }

                        if (Convert.ToDouble(ArrayReveive[5]) > 6)
                        {
                            PhoneRoll = 6;
                        }

                        else if (Convert.ToDouble(ArrayReveive[5]) < -6)
                        {
                            PhoneRoll = -6;
                        }

                        else
                        {
                            PhoneRoll = Convert.ToDouble(ArrayReveive[5]) / 2.5;
                        }
                    }
                }
                started = true;
                
            }
            catch (ObjectDisposedException)
            {
                started = false;
                //expected termination exception on a closed socket.
                // ...I'm open to suggestions on a better way of doing this.
            }
        }


        //http://stackoverflow.com/questions/6803073/get-local-ip-address-c-sharp
        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

    }
}
