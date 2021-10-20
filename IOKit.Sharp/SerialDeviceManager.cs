using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
    public class SerialDeviceManager : BaseDeviceManager
    {
        #region Device Callbacks
        protected override void DoDeviceAdded (IntPtr p, uint addedIterator)
        {
            uint usbDevice = IOKit.IOIteratorNext (addedIterator);

            while (usbDevice != 0) {
                uint parent = 0;
                uint parents = usbDevice;
                var vendor = string.Empty;
                var product = string.Empty;
                var serialNumber = string.Empty;
                uint vendorID = 0;
                uint productID = 0;

                GetParentProperties (usbDevice, ref parent, ref parents, ref vendor, ref product, ref serialNumber, ref vendorID, ref productID);

                var dialinDevice = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIODialinDeviceKey);

                EventHandler<DeviceArgs> addedEvent = OnDeviceAdded;
                // Fire off the Add event with the information we've gathered.
                if (addedEvent != null) {
                    var device = new SerialDevice {
                        Port = dialinDevice,
                        SerialBSDClientType = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOSerialBSDTypeKey),
                        TTYBaseName = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOTTYBaseNameKey),
                        TTYDevice = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOTTYDeviceKey),
                        TTYSuffix = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOTTYSuffixKey),
                        VendorName = vendor,
                        ProductName = product,
                        SerialNo = serialNumber,
                        VendorID = vendorID,
                        ProductID = productID,
                    };

                    // Add the device in. If it already exists it should just be replaced.
                    deviceList[device.Port] = device;
                    addedEvent (null, new DeviceArgs (device));
                }

                if (IOKit.IOObjectRelease (usbDevice) != 0) {
                    Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
                }
                usbDevice = IOKit.IOIteratorNext (addedIterator);
            };
        }

        protected override void DoDeviceRemoved (IntPtr p, uint removedIterator)
        {
            uint usbDevice = IOKit.IOIteratorNext (removedIterator);

            while (usbDevice != 0) {
                uint parent = 0;
                uint parents = usbDevice;
                var vendor = string.Empty;
                var product = string.Empty;
                var serialNumber = string.Empty;
                uint vendorID = 0;
                uint productID = 0;

                GetParentProperties (usbDevice, ref parent, ref parents, ref vendor, ref product, ref serialNumber, ref vendorID, ref productID);

                var dialinDevice = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIODialinDeviceKey);

                EventHandler<DeviceArgs> removedEvent = OnDeviceRemoved;
                // Fire off the Remove event with the information we've gathered.
                if (removedEvent != null) {
                    var device = new SerialDevice {
                        Port = dialinDevice,
                        SerialBSDClientType = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOSerialBSDTypeKey),
                        TTYBaseName = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOTTYBaseNameKey),
                        TTYDevice = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOTTYDeviceKey),
                        TTYSuffix = IOKit.GetPropertyStringValue (usbDevice, IOKit.kIOTTYSuffixKey),
                        VendorName = vendor,
                        ProductName = product,
                        SerialNo = serialNumber,
                        VendorID = vendorID,
                        ProductID = productID,
                    };

                    // Remove the device from the list
                    deviceList.Remove (device.Port);
                    removedEvent (null, new DeviceArgs (device));
                }

                if (IOKit.IOObjectRelease (usbDevice) != 0) {
                    Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
                }
                usbDevice = IOKit.IOIteratorNext (removedIterator);
            };
        }

        private static void GetParentProperties (uint usbDevice, ref uint parent, ref uint parents, ref string vendor, ref string product, ref string serialNumber, ref uint vendorID, ref uint productID)
        {
            while (IOKit.IORegistryEntryGetParentEntry (parents, IOKit.kIOServicePlane, ref parent) == IOKit.kIOReturnSuccess) {
                vendor = IOKit.GetPropertyStringValue (parent, IOKit.kUSBVendorString);
                if (!string.IsNullOrEmpty (vendor))
                    Debug.WriteLine ("Vendor ID: {0}", args: vendor);

                product = IOKit.GetPropertyStringValue (parent, IOKit.kUSBProductString);
                if (!string.IsNullOrEmpty (product))
                    Debug.WriteLine ("Product ID: {0}", args: product);

                serialNumber = IOKit.GetPropertyStringValue (parent, IOKit.kUSBSerialNumberString);
                if (!string.IsNullOrEmpty (serialNumber))
                    Debug.WriteLine ("Serial No: {0}", args: serialNumber);

                if (parents != usbDevice) {
                    if (IOKit.IOObjectRelease (parents) != 0) {
                        Debug.WriteLine ("Unable to release device: {0} ", parents);
                    }
                }

                vendorID = IOKit.GetPropertyUIntValue (parent, IOKit.kUSBVendorID);
                if (vendorID == 0)
                    Debug.WriteLine ("Vendor ID NOT found");

                productID = IOKit.GetPropertyUIntValue (parent, IOKit.kUSBProductID);
                if (productID == 0)
                    Debug.WriteLine ("Product ID NOT found");

                if (parents != usbDevice) {
                    if (IOKit.IOObjectRelease (parents) != 0) {
                        Debug.WriteLine ("Unable to release device: {0} ", parents);
                    }
                }

                if (!string.IsNullOrEmpty (vendor)
                    && !string.IsNullOrEmpty (product)
                    && !string.IsNullOrEmpty (serialNumber))
                    break;

                parents = parent;
            }
        }
        #endregion

        #region Let's Start Listening for Serial Devices
        public override void Start ()
        {
            Start (IOKit.kIOSerialBSDServiceValue);
        }
        #endregion
    }
}