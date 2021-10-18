using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
    public class HIDDeviceManager : BaseDeviceManager
    {
        #region Device Callbacks
        public override void DoDeviceAdded (IntPtr p, uint addedIterator)
        {
            // TODO YOUR PR Here :)
            throw new NotImplementedException ();
        }

        public override void DoDeviceRemoved (IntPtr p, uint removedIterator)
        {
            // TODO YOUR PR Here :)
            throw new NotImplementedException ();
        }
        #endregion

        #region Let's Start Listening for HID Devices
        public override void Start ()
        {
            Start (/* TODO YOUR PR Here  :)*/ string.Empty);
        }
        #endregion
    }
}