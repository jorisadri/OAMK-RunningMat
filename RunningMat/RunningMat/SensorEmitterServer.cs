using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace SensorEmitterServer
{
    public enum Context
    {
        WhileStartingListener,
        WhileReceivingData
    }

    public class ValuesReceivedEventArgs<T> : EventArgs
    {
        public T SensorReading { get; set; }
    }

    public class ExceptionOccuredEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public Context Context { get; set; }
    }

    /// <summary>
    /// Interface for a sensor reading (classes that hold values of sensor measurements).
    /// </summary>
    public interface ISensorReading
    {
        /// <summary>
        /// Has to return a constant number of values this ISensorReading class expects.
        /// </summary>
        int NumSensorValues { get; }

        void SetSensorValues(float[] values);
    }

    /// <summary>
    /// A TCP Server, that accepts incoming connections from the Windows Phone
    /// 'Sensor Emitter' App, and packs all received data into SensorReading
    /// objects.
    ///
    /// Usage: Create a SensorServer&lt;SensorEmitterReading&gt; object, connect to both 
    /// ValuesReceived and ExceptionOccured events and start the server with 
    /// the Start method.
    /// </summary>
    /// <typeparam name="T">A class, which has to inherit from ISensorReading 
    /// and supply a parameterless constructor.</typeparam>
    public class SensorServer<T> : IDisposable
        where T : ISensorReading, new()
    {
        private const int MagicNumber = 0x42fea723;

        public const int DefaultTcpPort = 3547;

        private volatile bool alive = true;
        private Thread listenThread;

        public event EventHandler<ValuesReceivedEventArgs<T>> ValuesReceived;
        public event EventHandler<ExceptionOccuredEventArgs> ExceptionOccured;

        private int numSensorValues;

        public SensorServer()
        {
            numSensorValues = new T().NumSensorValues;
        }

        /// <summary>
        /// Starts listening on the TCP port 3547 and awaits phones that connect to it
        /// via the Windows Phone 'Sensor Emitter' App.
        ///
        /// If any clients connects and sends data, the ValuesReceived event is raised
        /// for each packet that arrives successfully. If any error occurs in the process,
        /// or while starting the TCP server, the ExceptionOccured event is fired.
        /// Make sure to connect to these two events.
        /// </summary>
        public void Start() { Start(SensorServer<T>.DefaultTcpPort); }

        /// <summary>
        /// Starts listening on the given TCP port and awaits phones that connect to it
        /// via the Windows Phone 'Sensor Emitter' App.
        ///
        /// If any clients connects and sends data, the ValuesReceived event is raised
        /// for each packet that arrives successfully. If any error occurs in the process,
        /// or while starting the TCP server, the ExceptionOccured event is fired.
        /// Make sure to connect to these two events.
        /// </summary>
        /// <param name="tcpPort">The port to listen to.</param>
        public void Start(int tcpPort)
        {
            listenThread = new Thread(new ParameterizedThreadStart(ListenForClients));
            listenThread.IsBackground = true;
            listenThread.Start(tcpPort);
        }

        /// <summary>
        /// Thread, that waits for incoming connections. For each connection, a
        /// seperate thread is started then.
        /// </summary>
        private void ListenForClients(object port)
        {
            var tcpListener = new TcpListener(IPAddress.Any, (int)port);

            // Try to start the listener
            try { tcpListener.Start(); }
            catch (SocketException sockEx)
            {
                OnException(sockEx, Context.WhileStartingListener);
            }
            
            while (alive)
            {
                // Blocks until a client has connected to the server
                TcpClient client = tcpListener.AcceptTcpClient();

                // Create a thread to handle communication with connected client
                var clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }

            try { tcpListener.Stop(); }
            catch (SocketException) { }
        }

        /// <summary>
        /// Thread, that handles the actual communication after a client connected
        /// </summary>
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            // Write magic number as a greeting and to signal we are a compatible 'sensor emitter' server
            clientStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(MagicNumber)), 0, 4);

            // Read from the incoming TCP stream
            using (BinaryReader br = new BinaryReader(clientStream))
            {
                bool canRead = true;
                while (canRead && alive)
                {
                    try
                    {
                        int length = IPAddress.NetworkToHostOrder(br.ReadInt32());

                        if (length < 0 || length != numSensorValues)
                        {
                            if (length < 0)
                                OnException(new ArgumentOutOfRangeException("The format of " +
                                    "the TCP stream is incorrect. It said 'a negative number " +
                                    "of items is going to follow this', which obviously does " +
                                    "not make sense, as exactly " + numSensorValues +
                                    " values are expected."), Context.WhileReceivingData);
                            else
                                OnException(new ArgumentOutOfRangeException("The format of " +
                                    "the TCP stream is incorrect. It said '" + length +
                                    " items are going to be in this single measurement " +
                                    "package', which does not match the expected value. " +
                                    "Valid packages from the SensorEmitter App have " +
                                    "exactly " + numSensorValues + " values."),
                                    Context.WhileReceivingData);
                            canRead = false;
                        }
                        else
                        {
                            // Read all measurement values
                            float[] values = new float[length];
                            for (int i = 0; i < length; i++)
                                values[i] = ReadNetworkFloat(br);

                            // Fire event, that a new pack of values was read
                            OnValuesReceived(values);
                        }
                    }
                    catch (IOException) { canRead = false; }
                }
            }

            tcpClient.Close();
        }


        /// <summary>
        /// Reads a float in network byte order (big endian) with the given binary reader.
        /// This is needed because the IPAddress class doesn't provide an overload
        /// of NetworkToHostOrder for any floating point types. This is equivalent to:
        ///
        ///     int ntohl = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(reader.ReadBytes(4), 0));
        ///     return BitConverter.ToSingle(BitConverter.GetBytes(ntohl), 0);
        ///
        /// but with better performance.
        /// </summary>
        public static float ReadNetworkFloat(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Fires the ExceptionOccured event back to the main thread.
        /// </summary>
        private void OnException(Exception ex, Context context)
        {
            if (ExceptionOccured != null && Application.Current != null)
                Application.Current.Dispatcher.Invoke(ExceptionOccured, this,
                        new ExceptionOccuredEventArgs() { Exception = ex, Context = context });
        }

        /// <summary>
        /// Fires the ValuesReceived event back to the main thread.
        /// </summary>
        private void OnValuesReceived(float[] values)
        {
            if (ValuesReceived != null && Application.Current != null)
            {
                T sensorReading = new T();
                sensorReading.SetSensorValues(values);

                Application.Current.Dispatcher.Invoke(ValuesReceived, this,
                        new ValuesReceivedEventArgs<T>() { SensorReading = sensorReading });
            }
        }

        /// <summary>
        /// Stops all associated threads, the TCP Server and clears up ressources of this object.
        /// </summary>
        public void Dispose()
        {
            alive = false;
            listenThread.Abort();
        }
    }

    /// <summary>
    /// Holds values that were received from the Windows Phone 'Sensor Emitter' App.
    /// </summary>
    public class SensorEmitterReading : ISensorReading
    {
        public int NumSensorValues { get { return 23; } }

        public double QuaternionX { get; set; }
        public double QuaternionY { get; set; }
        public double QuaternionZ { get; set; }
        public double QuaternionW { get; set; }

        public double RotationPitch { get; set; }
        public double RotationRoll { get; set; }
        public double RotationYaw { get; set; }

        public double RotationRateX { get; set; }
        public double RotationRateY { get; set; }
        public double RotationRateZ { get; set; }

        public double RawAccelerationX { get; set; }
        public double RawAccelerationY { get; set; }
        public double RawAccelerationZ { get; set; }

        public double LinearAccelerationX { get; set; }
        public double LinearAccelerationY { get; set; }
        public double LinearAccelerationZ { get; set; }

        public double GravityX { get; set; }
        public double GravityY { get; set; }
        public double GravityZ { get; set; }

        public double MagneticHeading { get; set; }
        public double TrueHeading { get; set; }
        public double HeadingAccuracy { get; set; }

        public double MagnetometerX { get; set; }
        public double MagnetometerY { get; set; }
        public double MagnetometerZ { get; set; }
        public bool MagnetometerDataValid { get; set; }

        public void SetSensorValues(float[] values)
        {
            if (values == null)
                throw new ArgumentNullException("No array of values given.");

            if (values.Length != NumSensorValues)
                throw new ArgumentException("Unexpected length of array. Exactly " +
                                            NumSensorValues + " items were expected.");

            double x = values[0];
            double y = values[1];
            double z = values[2];
            double w = values[3];

            RotationPitch = Math.Atan2(2 * (y * z + w * x), w * w - x * x - y * y + z * z);
            RotationRoll = Math.Atan2(2 * (x * y + w * z), w * w + x * x - y * y - z * z);
            RotationYaw = Math.Asin(-2 * (x * z - w * y));

            QuaternionX = values[0];
            QuaternionY = values[1];
            QuaternionZ = values[2];
            QuaternionW = values[3];

            RotationRateX = values[7];
            RotationRateY = values[8];
            RotationRateZ = values[9];

            RawAccelerationX = values[13];
            RawAccelerationY = values[14];
            RawAccelerationZ = values[15];

            LinearAccelerationX = values[4];
            LinearAccelerationY = values[5];
            LinearAccelerationZ = values[6];

            GravityX = values[10];
            GravityY = values[11];
            GravityZ = values[12];

            MagneticHeading = values[16];
            TrueHeading = values[17];
            HeadingAccuracy = values[18];

            MagnetometerX = values[19];
            MagnetometerY = values[20];
            MagnetometerZ = values[21];
            MagnetometerDataValid = values[22] == 1d;
        }
    }
}