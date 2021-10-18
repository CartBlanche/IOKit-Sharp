using System;
using System.Threading;
using System.Threading.Tasks;

namespace IOKit.Sharp
{
    public class USBDevice : BaseDevice
    {
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
    }
}