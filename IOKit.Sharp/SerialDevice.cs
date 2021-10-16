using System;

namespace IOKit.Sharp
{
    public class SerialDevice : BaseDevice
    {
        #region Serial Properties
        public string Port { //DialInDevice
            get;
            set;
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
    }
}