using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace RunningMat
{
    public class UDPReceiver : BaseClass
    {

        //http://acrocontext.wordpress.com/2013/08/15/c-simple-udp-listener-in-asynchronous-way/
        //the socket itself
        private Socket udpSock;
        // buffer is used to hold the incoming data
        private byte[] buffer;
        //port for connection. Max port number is 65535
        public int port = 12345;
        string Received;
        //bool to check if there is phoneconnection
        public bool started = false;
        //used to split the message apart
        string[] seperator = new string[] { ", " };
        // The number for max angles
        int clipPitch = 10;
        int clipRoll = 6;

        //
        private double _phonepitch = 0.0;
        public double PhonePitch
        {
            get { return _phonepitch; }
            set
            {
                if (App.controltreadmill.FCheck && App.UIController.choise != App.Choise.MakeAngle)
                {
                    _phonepitch = value;
                    App.DataPotentiometer.InputPotentiometerX = PhonePitch;
                }
                else
                {
                    _phonepitch = value / 2.5;
                    if (_phonepitch < clipPitch && _phonepitch > -clipPitch)
                    { }

                    else
                    {
                        if (_phonepitch > clipPitch)
                            _phonepitch = clipPitch;

                        else if (_phonepitch < -clipPitch)
                            _phonepitch = -clipPitch;
                    }
                }
                RaisePropChanged("PhonePitch");
            }
        }

        private double _phoneroll = 0.0;
        public double PhoneRoll
        {
            get { return _phoneroll; }
            set
            {
                if (App.controltreadmill.FCheck && App.UIController.choise != App.Choise.MakeAngle)
                {
                    _phoneroll = value;
                    App.DataPotentiometer.InputPotentiometerY = PhoneRoll;
                }
                else
                {
                    _phoneroll = value / 2.5;
                    if (_phoneroll < clipRoll && _phoneroll > -clipRoll)
                    { }
                    else
                    {
                        if (_phoneroll> clipRoll)
                            _phoneroll = clipRoll;

                        else if (_phoneroll < -clipRoll)
                            _phoneroll = -clipRoll;
                    }
                }
                RaisePropChanged("PhoneRoll");
            }
        }

        public UDPReceiver()
        {
            // start listening for new data by starting the application
            Starter();
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
                Received = Encoding.ASCII.GetString(localMsg, 0, localMsg.Length);
                string[] ArrayReveive = Received.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                if (ArrayReveive[0] == "O")
                {
                    // if error here use this
                    PhonePitch = Convert.ToDouble(ArrayReveive[4].Replace(',', '.'));
                    PhoneRoll = Convert.ToDouble(ArrayReveive[5].Replace(',', '.'));
                    //PhonePitch = Convert.ToDouble(ArrayReveive[4]);
                    //PhoneRoll = Convert.ToDouble(ArrayReveive[5]);
                }
                started = true;
            }
            catch (ObjectDisposedException)
            {
                started = false;
            }
        }
        //http://stackoverflow.com/questions/6803073/get-local-ip-address-c-sharp
        //get the Local ip for show in messagebox
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
