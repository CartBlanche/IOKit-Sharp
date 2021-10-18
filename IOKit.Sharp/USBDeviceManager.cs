using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
    public class USBDeviceManager : BaseDeviceManager
    {
        #region Device Callbacks
        protected override void DoDeviceAdded (IntPtr p, uint addedIterator)
        {
            uint usbDevice = IOKit.IOIteratorNext (addedIterator);

            while (usbDevice != 0) {
                // TODO YOUR PR Here :)

                EventHandler<DeviceArgs> addedEvent = OnDeviceAdded;
                // Fire off the Add event with the information we've gathered.
                if (addedEvent != null) {
                    // TODO Populate our USB Device Correctly Here
                    var device = new USBDevice {
                        ProductName = "/*TODO */",
                    };

                    // Add the device in. If it already exists it should just be replaced.
                    deviceList[device.ProductName] = device;
                    addedEvent (null, new DeviceArgs (device));
                }

                if (IOKit.IOObjectRelease (usbDevice) != IOKit.kIOReturnSuccess) {
                    Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
                }

                usbDevice = IOKit.IOIteratorNext (addedIterator);
            }
        }

        protected override void DoDeviceRemoved (IntPtr p, uint removedIterator)
        {
            uint usbDevice = IOKit.IOIteratorNext (removedIterator);

            while (usbDevice != 0) {
                // TODO YOUR PR Here :)

                EventHandler<DeviceArgs> removedEvent = OnDeviceRemoved;
                // Fire off the Add event with the information we've gathered.
                if (removedEvent != null) {
                    // TODO Populate our USB Device Correctly Here
                    var device = new USBDevice {
                        ProductName = "/* TODO */",
                    };

                    // Add the device in. If it already exists it should just be replaced.
                    deviceList[device.ProductName] = device;
                    removedEvent (null, new DeviceArgs (device));
                }

                if (IOKit.IOObjectRelease (usbDevice) != IOKit.kIOReturnSuccess) {
                    Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
                }
                usbDevice = IOKit.IOIteratorNext (removedIterator);
            }
        }
        #endregion


        #region Let's Start Listening for USB Devices
        public override void Start ()
        {
            Start (IOKit.kIOUSBDeviceClassName);
        }
        #endregion
    }
}