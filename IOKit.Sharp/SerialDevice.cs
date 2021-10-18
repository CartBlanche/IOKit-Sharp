using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace IOKit.Sharp
{
    public class SerialDevice : BaseDevice
    {
        SerialPort serialPort = null;
        public SerialPort SerialPort => serialPort;

        #region Serial Properties
        string port;
        public string Port { //DialInDevice
            get => port;
            set {
                if (port != value) {
                    port = value;

                    // Lazy load it
                    if (serialPort == null)
                        serialPort = new SerialPort {
                            BaudRate = 115200, // This value is ignored when using ACM
                            Parity = Parity.None,
                            DataBits = 8,
                            StopBits = StopBits.One,
                            Handshake = Handshake.None,

                            // Set the read/write timeouts
                            ReadTimeout = 5000,
                            WriteTimeout = 5000
                        }; ;

                    if (serialPort.IsOpen)
                        serialPort.Close ();

                    serialPort.PortName = port;
                }
            }
        }

        public string SerialBSDClientType {
            get;
            set;
        }

        public string TTYBaseName {
            get;
            set;
        }

        public string TTYDevice {
            get;
            set;
        }

        public string TTYSuffix {
            get;
            set;
        }
        #endregion

        public string VendorName {
            get;
            set;
        }

        public string SerialNo {
            get;
            set;
        }

        public override string ToString ()
        {
            return base.ToString () + string.Format (
                "VendorName:\t\t\t{0}" + Environment.NewLine +
                "SerialNo:\t\t\t\t{1}" + Environment.NewLine +
                "Port:\t\t\t{2}" + Environment.NewLine + // DialinDevice
                "SerialBSDClientType:\t{3}" + Environment.NewLine +
                "TTYBaseName:\t\t{4}" + Environment.NewLine +
                "TTYDevice:\t\t\t{5}" + Environment.NewLine +
                "TTYSuffix:\t\t\t{6}" + Environment.NewLine,
                VendorName, SerialNo, Port, SerialBSDClientType, TTYBaseName, TTYDevice, TTYSuffix);
        }

        public override void Close ()
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close ();
        }

        public override void Open ()
        {
            if (serialPort != null && !serialPort.IsOpen) {
                serialPort.Open ();
                serialPort.BaseStream.ReadTimeout = 0;
            }
        }

        public override async Task WriteAsync (byte[] encodedBytes,
                                        int encodedToSend,
                                        CancellationToken cancellationToken = default)
        {
            if (serialPort == null || serialPort.IsOpen == false) {
                throw new Exception ("Device Disconnected");
            }

            await serialPort.BaseStream.WriteAsync (encodedBytes, 0, encodedToSend, cancellationToken).ConfigureAwait (false);
        }
    }
}