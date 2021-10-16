using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
    public class HIDDeviceManager : BaseDeviceManager
    {
        public override void Start ()
        {
            // TODO : Your PR here
        }

        void DoHIDDeviceAdded (IntPtr p, uint addedIterator)
        {

        }

        void DoHIDeviceRemoved (IntPtr p, uint removedIterator)
        {
        }
    }
}