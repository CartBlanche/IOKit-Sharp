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
            return string.Format (
                "VendorID:\t\t\t{0}" + Environment.NewLine +
                "ProductID:\t\t\t{1}" + Environment.NewLine +
                "Name:\t\t\t\t{2}" + Environment.NewLine,
                VendorID, ProductID, ProductName);
        }

        public abstract void Close ();

        public abstract void Open ();

        public abstract Task WriteAsync (byte[] encodedBytes,
                                        int encodedToSend,
                                        CancellationToken cancellationToken = default);
    }
}