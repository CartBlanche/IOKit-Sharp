using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
	public class SerialDeviceManager : BaseDeviceManager
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

				while (IOKit.IORegistryEntryGetParentEntry (parents, IOKit.kIOServicePlane, ref parent) == IOKit.kIOReturnSuccess) {
					vendor = IOKit.GetPropertyValue (parent, IOKit.kUSBVendorString);
					if (!string.IsNullOrEmpty (vendor))
						Debug.WriteLine ("Vendor ID: {0}", args: vendor);

					product = IOKit.GetPropertyValue (parent, IOKit.kUSBProductString);
					if (!string.IsNullOrEmpty (product))
						Debug.WriteLine ("Product ID: {0}", args: product);

					serialNumber = IOKit.GetPropertyValue (parent, IOKit.kUSBSerialNumberString);
					if (!string.IsNullOrEmpty (serialNumber))
						Debug.WriteLine ("Serial No: {0}", args: serialNumber);

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

				var dialinDevice = IOKit.GetPropertyValue (usbDevice, IOKit.kIODialinDeviceKey);

				// TODO if ((Array.IndexOf (invalidDeviceList, dialinDevice) == -1) && (Array.IndexOf (invalidDeviceList, product) == -1)) {
				EventHandler<DeviceArgs> addedEvent = OnDeviceAdded;
				// Fire off the Add event with the information we've gathered.
				if (addedEvent != null) {
					var device = new SerialDevice {
						Port = dialinDevice,
						SerialBSDClientType = IOKit.GetPropertyValue (usbDevice, IOKit.kIOSerialBSDTypeKey),
						TTYBaseName = IOKit.GetPropertyValue (usbDevice, IOKit.kIOTTYBaseNameKey),
						TTYDevice = IOKit.GetPropertyValue (usbDevice, IOKit.kIOTTYDeviceKey),
						TTYSuffix = IOKit.GetPropertyValue (usbDevice, IOKit.kIOTTYSuffixKey),
						VendorName = vendor,
						ProductName = product,
						SerialNo = serialNumber,
					};

					// Add the device in. If it already exists it should just be replaced.
					deviceList[device.TTYDevice] = device;
					addedEvent (null, new DeviceArgs (device));
				}
				// }

				if (IOKit.IOObjectRelease (usbDevice) != 0) {
					Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
				}
				usbDevice = IOKit.IOIteratorNext (addedIterator);
			};
		}

		public override void DoDeviceRemoved (IntPtr p, uint removedIterator)
		{
			uint usbDevice = IOKit.IOIteratorNext (removedIterator);

			while (usbDevice != 0) {
				uint parent = 0;
				uint parents = usbDevice;
				var vendor = string.Empty;
				var product = string.Empty;
				var serialNumber = string.Empty;

				while (IOKit.IORegistryEntryGetParentEntry (parents, IOKit.kIOServicePlane, ref parent) == IOKit.kIOReturnSuccess) {
					vendor = IOKit.GetPropertyValue (parent, IOKit.kUSBVendorString);
					if (!string.IsNullOrEmpty (vendor))
						Debug.WriteLine ("Vendor ID: {0}", args: vendor);

					product = IOKit.GetPropertyValue (parent, IOKit.kUSBProductString);
					if (!string.IsNullOrEmpty (product))
						Debug.WriteLine ("Product ID: {0}", args: product);

					serialNumber = IOKit.GetPropertyValue (parent, IOKit.kUSBSerialNumberString);
					if (!string.IsNullOrEmpty (serialNumber))
						Debug.WriteLine ("Serial No: {0}", args: serialNumber);

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

				var dialinDevice = IOKit.GetPropertyValue (usbDevice, IOKit.kIODialinDeviceKey);

				// TODO if ((Array.IndexOf (invalidDeviceList, dialinDevice) == -1) && (Array.IndexOf (invalidDeviceList, product) == -1)) {

				EventHandler<DeviceArgs> removedEvent = OnDeviceRemoved;
				// Fire off the Remove event with the information we've gathered.
				if (removedEvent != null) {
					var device = new SerialDevice {
						Port = dialinDevice,
						SerialBSDClientType = IOKit.GetPropertyValue (usbDevice, IOKit.kIOSerialBSDTypeKey),
						TTYBaseName = IOKit.GetPropertyValue (usbDevice, IOKit.kIOTTYBaseNameKey),
						TTYDevice = IOKit.GetPropertyValue (usbDevice, IOKit.kIOTTYDeviceKey),
						TTYSuffix = IOKit.GetPropertyValue (usbDevice, IOKit.kIOTTYSuffixKey),
						VendorName = vendor,
						ProductName = product,
						SerialNo = serialNumber,
					};
					// Remove the device from the list
					deviceList.Remove (device.TTYDevice);
					removedEvent (null, new DeviceArgs (device));
				}
				// }

				if (IOKit.IOObjectRelease (usbDevice) != 0) {
					Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
				}
				usbDevice = IOKit.IOIteratorNext (removedIterator);
			};
		}
		#endregion

		#region Let's Start Listening for Devices
		public override void Start ()
		{
			Start (IOKit.kIOSerialBSDServiceValue);
		}
		#endregion
	}
}