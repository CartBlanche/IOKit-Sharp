using System;
using System.Diagnostics;

namespace IOKit.Sharp
{
    public class USBDeviceManager : BaseDeviceManager
    {
        #region Device Callbacks
        public override void DoDeviceAdded (IntPtr p, uint addedIterator)
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
                GetParentProperties(usbDevice, ref parent, ref parents, ref vendor, ref product, ref serialNumber, ref vendorID, ref productID);

                EventHandler<DeviceArgs> addedEvent = OnDeviceAdded;
                // Fire off the Add event with the information we've gathered.
                if (addedEvent != null) {
                    USBDevice device = new USBDevice {
                        ProductName = IOKit.GetDeviceName(usbDevice),
                        VendorID = IOKit.GetPropertyUIntValue(usbDevice, IOKit.kUSBVendorID),
                        ProductID = IOKit.GetPropertyUIntValue(usbDevice, IOKit.kUSBProductID),
                        VendorName = IOKit.GetPropertyStringValue(usbDevice, IOKit.kUSBVendorString),
                        SerialNo = IOKit.GetPropertyStringValue(usbDevice, IOKit.kUSBSerialNumberString)
                    };

                    // Add the device in. If it already exists it should just be replaced.
                    deviceList[device.ProductName] = device;
                    addedEvent(null, new DeviceArgs(device));
                }

                if (IOKit.IOObjectRelease (usbDevice) != IOKit.kIOReturnSuccess) {
                    Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
                }

                usbDevice = IOKit.IOIteratorNext (addedIterator);
            }
        }

        public override void DoDeviceRemoved (IntPtr p, uint removedIterator)
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
        private static void GetParentProperties(uint usbDevice, ref uint parent, ref uint parents, ref string vendor, ref string product, ref string serialNumber, ref uint vendorID, ref uint productID)
        {
            while (IOKit.IORegistryEntryGetParentEntry(parents, IOKit.kIOUSBPlane, ref parent) == IOKit.kIOReturnSuccess)
            {
                vendor = IOKit.GetPropertyStringValue(parent, IOKit.kUSBVendorString);
                if (!string.IsNullOrEmpty(vendor))
                    Debug.WriteLine("Vendor ID: {0}", args: vendor);

                product = IOKit.GetPropertyStringValue(parent, IOKit.kUSBProductString);
                if (!string.IsNullOrEmpty(product))
                    Debug.WriteLine("Product ID: {0}", args: product);

                serialNumber = IOKit.GetPropertyStringValue(parent, IOKit.kUSBSerialNumberString);
                if (!string.IsNullOrEmpty(serialNumber))
                    Debug.WriteLine("Serial No: {0}", args: serialNumber);

                if (parents != usbDevice)
                {
                    if (IOKit.IOObjectRelease(parents) != 0)
                    {
                        Debug.WriteLine("Unable to release device: {0} ", parents);
                    }
                }

                vendorID = IOKit.GetPropertyUIntValue(parent, IOKit.kUSBVendorID);
                if (vendorID == 0)
                    Debug.WriteLine("Vendor ID NOT found");

                productID = IOKit.GetPropertyUIntValue(parent, IOKit.kUSBProductID);
                if (productID == 0)
                    Debug.WriteLine("Product ID NOT found");

                if (parents != usbDevice)
                {
                    if (IOKit.IOObjectRelease(parents) != 0)
                    {
                        Debug.WriteLine("Unable to release device: {0} ", parents);
                    }
                }

                if (!string.IsNullOrEmpty(vendor)
                    && !string.IsNullOrEmpty(product)
                    && !string.IsNullOrEmpty(serialNumber))
                    break;

                parents = parent;
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