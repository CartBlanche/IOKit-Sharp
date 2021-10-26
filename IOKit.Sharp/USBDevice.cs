using System;
using System.Threading;
using System.Threading.Tasks;

namespace IOKit.Sharp
{
    public class USBDevice : BaseDevice
    {
        public string VendorName {
            get;
            set;
        }

        public string SerialNo {
            get;
            set;
        }

        public override void Close ()
        {
            throw new NotImplementedException ();
        }

        public override void Open ()
        {
            throw new NotImplementedException ();
        }

        public override Task WriteAsync (byte[] encodedBytes, int encodedToSend, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException ();
        }

        public override string ToString ()
        {
            return base.ToString () +
                $"VendorName:\t\t\t{VendorName}{Environment.NewLine}" +
                $"SerialNo:\t\t\t{SerialNo}{Environment.NewLine}";
        }
    }
}