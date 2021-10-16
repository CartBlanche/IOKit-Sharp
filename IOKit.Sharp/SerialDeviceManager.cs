using System;
using System.Diagnostics;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
    public class SerialDeviceManager : BaseDeviceManager
    {
        #region Let's Start Listening for Devices
        public override void Start()
        {
			try {
				int kr;

				var ioServiceMatching = IOKit.IOServiceMatching ("IOSerialBSDClient"); // "IOHIDSystem" // "IOSerialBSDServiceValue" //"IOUSBDeviceClassName"
				NSMutableDictionary addingDeviceDictionary = ObjCRuntime.Runtime.GetNSObject<NSMutableDictionary> (ioServiceMatching);
				/* TODO doesn't look like it's needed or doing any auto filtering on this hence the `invalidDeviceList`
				 * addingDeviceDictionary["USBVendorID"] = new NSNumber (0x2E6A);
				 * addingDeviceDictionary["USBProductID"] = new NSNumber (0x0001); */
				NSMutableDictionary removingDeviceDictionary = new NSMutableDictionary (addingDeviceDictionary);

				//To set up asynchronous notifications, create a notification port and
				//add its run loop event source to the program’s run loop
				IntPtr gNotifyPort = IOKit.IONotificationPortCreate (0);
				var dq = new DispatchQueue ("IOKit.Serial.Detector");
				IOKit.IONotificationPortSetDispatchQueue (gNotifyPort, dq.Handle);

				IntPtr notificationRunLoopSource = IOKit.IONotificationPortGetRunLoopSource (gNotifyPort);

				var runLoopSource = new CFRunLoopSource (notificationRunLoopSource);
				var runloop = NSRunLoop.Current;
				var cfRunLoop = runloop.GetCFRunLoop ();
				cfRunLoop.AddSource (runLoopSource, NSRunLoop.NSDefaultRunLoopMode);

				//Now set up two notifications:
				//one to be called when a device is first matched by the I/O Kit and another to be called when the
				//device is terminated
				//Notification of first match:
				uint addedIterator = 0;
				kr = IOKit.IOServiceAddMatchingNotification (
					gNotifyPort,
					IOKit.kIOFirstMatchNotification,
					addingDeviceDictionary.Handle,
					DoSerialDeviceAdded,
					IntPtr.Zero,
					ref addedIterator);

				//Iterate over set of matching devices to access already-present devices
				//and to arm the notification
				if (kr == IOKit.kIOReturnSuccess) {
					DoSerialDeviceAdded (IntPtr.Zero, addedIterator);
				}
				else {
					Debug.WriteLine ("IOServiceAddMatchingNotification result for added devices: {0}", kr);
				}

				//Notification of termination:
				uint removedIterator = 0;
				kr = IOKit.IOServiceAddMatchingNotification (
					gNotifyPort,
					IOKit.kIOTerminatedNotification,
					removingDeviceDictionary.Handle,
					DoSerialDeviceRemoved,
					IntPtr.Zero,
					ref removedIterator);

				//Iterate over set of matching devices to release each one and to
				//arm the notification
				if (kr == IOKit.kIOReturnSuccess) {
					DoSerialDeviceRemoved (IntPtr.Zero, removedIterator);
				}
				else {
					Debug.WriteLine ("IOServiceAddMatchingNotification result for removed devices:  {0}", kr);
				}

				//Start the run loop so notifications will be received
				cfRunLoop.Run ();
			}
			catch (Exception ex) {
				Debug.WriteLine (ex.Message);
			}
		}
        #endregion

        #region Device Callbacks
        void DoSerialDeviceAdded (IntPtr p, uint addedIterator)
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

				var dialinDevice = IOKit.GetPropertyValue (usbDevice, "IODialinDevice");

				// TODO if ((Array.IndexOf (invalidDeviceList, dialinDevice) == -1) && (Array.IndexOf (invalidDeviceList, product) == -1)) {
					Debug.WriteLine ("IODialinDevice: {0}", args: dialinDevice);
					Debug.WriteLine ("IOSerialBSDClientType: {0}", args: IOKit.GetPropertyValue (usbDevice, "IOSerialBSDClientType"));
					Debug.WriteLine ("IOTTYBaseName: {0}", args: IOKit.GetPropertyValue (usbDevice, "IOTTYBaseName"));
					Debug.WriteLine ("IOTTYDevice: {0}", args: IOKit.GetPropertyValue (usbDevice, "IOTTYDevice"));
					Debug.WriteLine ("IOTTYSuffix: {0}", args: IOKit.GetPropertyValue (usbDevice, "IOTTYSuffix"));

					Debug.WriteLine ("");

					EventHandler<DeviceArgs> addedEvent = OnDeviceAdded;
					// Fire off the Add event with the information we've gathered.
					if (addedEvent != null) {
						var device = new SerialDevice {
							Path = dialinDevice,
							SerialBSDClientType = IOKit.GetPropertyValue (usbDevice, "IOSerialBSDClientType"),
							TTYBaseName = IOKit.GetPropertyValue (usbDevice, "IOTTYBaseName"),
							TTYDevice = IOKit.GetPropertyValue (usbDevice, "IOTTYDevice"),
							TTYSuffix = IOKit.GetPropertyValue (usbDevice, "IOTTYSuffix"),
							VendorName = vendor,
							Name = product,
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

		void DoSerialDeviceRemoved (IntPtr p, uint removedIterator)
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

				var dialinDevice = IOKit.GetPropertyValue (usbDevice, "IODialinDevice");

				// TODO if ((Array.IndexOf (invalidDeviceList, dialinDevice) == -1) && (Array.IndexOf (invalidDeviceList, product) == -1)) {

					EventHandler<DeviceArgs> removedEvent = OnDeviceRemoved;
					// Fire off the Remove event with the information we've gathered.
					if (removedEvent != null) {
						var device = new SerialDevice {
							Path = dialinDevice,
							SerialBSDClientType = IOKit.GetPropertyValue (usbDevice, "IOSerialBSDClientType"),
							TTYBaseName = IOKit.GetPropertyValue (usbDevice, "IOTTYBaseName"),
							TTYDevice = IOKit.GetPropertyValue (usbDevice, "IOTTYDevice"),
							TTYSuffix = IOKit.GetPropertyValue (usbDevice, "IOTTYSuffix"),
							VendorName = vendor,
							Name = product,
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
	}
}