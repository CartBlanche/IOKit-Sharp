using System;
using System.Diagnostics;

namespace IOKit.Sharp
{
    public class USBDeviceManager : BaseDeviceManager
    {
        #region Device Callbacks
        protected override void DoDeviceAdded (IntPtr p, uint addedIterator)
        {
            uint usbDevice = IOKit.IOIteratorNext (addedIterator);

            while (usbDevice != 0) {
                EventHandler<DeviceArgs> addedEvent = OnDeviceAdded;

                // Fire off the Add event with the information we've gathered.
                if (addedEvent != null) {
                    USBDevice device = CreateUSBDeviceFromProperties (usbDevice);

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
                EventHandler<DeviceArgs> removedEvent = OnDeviceRemoved;

                // Fire off the Add event with the information we've gathered.
                if (removedEvent != null) {
                    USBDevice device = CreateUSBDeviceFromProperties (usbDevice);

                    // Remove the device from the list
                    deviceList.Remove (device.ProductName);
                    removedEvent (null, new DeviceArgs (device));
                }

                if (IOKit.IOObjectRelease (usbDevice) != IOKit.kIOReturnSuccess) {
                    Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
                }
                usbDevice = IOKit.IOIteratorNext (removedIterator);
            }
        }

        private static USBDevice CreateUSBDeviceFromProperties (uint usbDevice)
        {
            // Populate our USB Device Correctly Here
            return new USBDevice {
                ProductName = IOKit.GetDeviceName (usbDevice),
                VendorID = IOKit.GetPropertyUIntValue (usbDevice, IOKit.kUSBVendorID),
                ProductID = IOKit.GetPropertyUIntValue (usbDevice, IOKit.kUSBProductID),
                VendorName = IOKit.GetPropertyStringValue (usbDevice, IOKit.kUSBVendorString),
                SerialNo = IOKit.GetPropertyStringValue (usbDevice, IOKit.kUSBSerialNumberString)
            };
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