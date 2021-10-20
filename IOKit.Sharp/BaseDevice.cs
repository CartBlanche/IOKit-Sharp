using System;
using System.Threading;
using System.Threading.Tasks;

namespace IOKit.Sharp
{
    public abstract class BaseDevice
    {
        #region Common Properties
        public uint VendorID {
            get;
            set;
        }

        public uint ProductID {
            get;
            set;
        }

        public string ProductName {
            get;
            set;
        }
        #endregion

        public override string ToString ()
        {
            return $"VendorID:\t\t\t{VendorID} \n" +
                $"ProductID:\t\t\t{ProductID} \n" +
                $"ProductName:\t\t{ProductName} \n";
        }

        public abstract void Close ();

        public abstract void Open ();

        public abstract Task WriteAsync (byte[] encodedBytes,
                                        int encodedToSend,
                                        CancellationToken cancellationToken = default);
    }
}