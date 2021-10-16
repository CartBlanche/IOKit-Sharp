using System;

namespace IOKit.Sharp
{
    public class BaseDevice
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

        public string Name {
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
                VendorID, ProductID, Name);
        }
    }
}