using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
    public class USBDeviceManager : BaseDeviceManager
    {
        public override void Start ()
        {
            // TODO : Your PR here
        }

        void DoUSBDeviceAdded (IntPtr p, uint addedIterator)
        {

        }

        void DoUSBDeviceRemoved (IntPtr p, uint removedIterator)
        {
        }
    }
}