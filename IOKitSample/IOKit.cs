using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;

namespace IOKit
{
	public class MeadowDeviceArgs : EventArgs
	{
		MeadowDevice device;

		public MeadowDevice Device => device;

		public MeadowDeviceArgs (MeadowDevice device)
		{
			this.device = device;
		}
	}

	public class MeadowDevice
	{
		#region IO* Properties
		public string DialinDevice {
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

		#region USB Properties
		public string VendorID {
			get;
			set;
		}

		public string ProductID {
			get;
			set;
		}

		public string SerialNo {
			get;
			set;
		}
		#endregion

		public override string ToString ()
		{
			return string.Format (
				"VendorID:\t\t\t{0}" + Environment.NewLine +
				"ProductID:\t\t\t{1}" + Environment.NewLine +
				"SerialNo:\t\t\t\t{2}" + Environment.NewLine +
				"DialinDevice:\t\t\t{3}" + Environment.NewLine +
				"SerialBSDClientType:\t{4}" + Environment.NewLine +
				"TTYBaseName:\t\t{5}" + Environment.NewLine +
				"TTYDevice:\t\t\t{6}" + Environment.NewLine +
				"TTYSuffix:\t\t\t{7}" + Environment.NewLine,
				VendorID, ProductID, SerialNo, DialinDevice, SerialBSDClientType, TTYBaseName, TTYDevice, TTYSuffix);
		}
	}

	public static class IOKit
    {
		#region DllImports
		// This is based off of Chris Hamons' gist https://gist.github.com/chamons/82ab06f5e83d2cb10193

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern uint IOServiceGetMatchingService (uint masterPort, IntPtr matching);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern uint IOServiceGetMatchingServices (uint masterPort, IntPtr matching, ref uint iterator);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern IntPtr IOServiceMatching (string s);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern IntPtr IORegistryEntryCreateCFProperty (uint entry, IntPtr key, IntPtr allocator, uint options);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern int IOObjectRelease (uint o);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern IntPtr IONotificationPortCreate (uint o);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern IntPtr IONotificationPortGetRunLoopSource (IntPtr notify);

		[Obsolete ("IOMasterPort has been Deprecated by Apple and is most cases is no longer needed.")]
		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern int IOMasterPort (uint masterPort, ref uint matching);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern int IOServiceAddMatchingNotification (
			IntPtr notifyPort,
			string notificationType,
			IntPtr matching,
			Action<IntPtr, uint> deviceNotification,
			IntPtr refCon,
			ref uint notification);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern uint IOIteratorNext (uint iterator);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern int IORegistryEntryGetName (uint entry, IntPtr name);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern int IORegistryEntryGetParentEntry (uint current_device, string name, ref uint parent);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern IntPtr IORegistryEntrySearchCFProperty (uint entry, string plane, IntPtr key, IntPtr allocator, uint options);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern void IONotificationPortSetDispatchQueue (IntPtr gNotifyPort, IntPtr dispatchQueue);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		static extern int IORegistryEntryCreateCFProperties (uint usbDevice, IntPtr properties, IntPtr allocator, uint options);
        #endregion

        #region Event Handlers
        public static EventHandler<MeadowDeviceArgs> OnDeviceAdded;

		public static EventHandler<MeadowDeviceArgs> OnDeviceRemoved;
		#endregion

		#region IOKit Contants to match the Objective C/Swift constants
		const int kIOReturnSuccess = 0;

		const string kIOServicePlane = "IOService";
		const string kIOFirstMatchNotification = "IOServiceFirstMatch";
		const string kIOTerminatedNotification = "IOServiceTerminate";

		const string kUSBVendorString = "kUSBVendorString";
		const string kUSBSerialNumberString = "kUSBSerialNumberString";
		const string kUSBProductString = "kUSBProductString";
		#endregion

		// List of Devices that we aren't interested in being notified about
		static string[] invalidDeviceList = {
			"/dev/tty.Bluetooth-Incoming-Port",
			"Meadow Bootloader",
		};

        #region Accessor Methods
        static string GetSerialNumber ()
		{
			string serial = string.Empty;
			uint platformExpert = IOServiceGetMatchingService (0, IOServiceMatching ("IOPlatformExpertDevice"));
			if (platformExpert != 0) {
				serial = GetPropertyValue (platformExpert, "IOPlatformSerialNumber");
				IOObjectRelease (platformExpert);
			}

			return serial;
		}

		static string GetPropertyValue (uint device, string propertyName)
		{
			string returnStr = string.Empty;

			NSString key = (NSString)propertyName;
			IntPtr propertyPointer = IORegistryEntryCreateCFProperty (device, key.Handle, IntPtr.Zero, 0);
			if (propertyPointer != IntPtr.Zero) {
				returnStr = NSString.FromHandle (propertyPointer);
			}

			return returnStr;
		}
        #endregion

        #region Initialisation
        public static void InitialiseSerialUSB ()
        {
			try {
				int kr;

				var ioServiceMatching = IOServiceMatching ("IOSerialBSDClient"); // "IOHIDSystem" // "IOSerialBSDServiceValue" //"IOUSBDeviceClassName"
				NSMutableDictionary addingDeviceDictionary = ObjCRuntime.Runtime.GetNSObject<NSMutableDictionary> (ioServiceMatching);
				/* TODO doesn't look like it's needed or doing any auto filtering on this hence the `invalidDeviceList`
				 * addingDeviceDictionary["USBVendorID"] = new NSNumber (0x2E6A);
				 * addingDeviceDictionary["USBProductID"] = new NSNumber (0x0001); */
				NSMutableDictionary removingDeviceDictionary = new NSMutableDictionary (addingDeviceDictionary);

				//To set up asynchronous notifications, create a notification port and
				//add its run loop event source to the program’s run loop
				IntPtr gNotifyPort = IONotificationPortCreate (0);
				var dq = new DispatchQueue ("IODetector");
				IONotificationPortSetDispatchQueue (gNotifyPort, dq.Handle);

				IntPtr notificationRunLoopSource = IONotificationPortGetRunLoopSource (gNotifyPort);

				var runLoopSource = new CFRunLoopSource (notificationRunLoopSource);
				var runloop = NSRunLoop.Current;
				var cfRunLoop = runloop.GetCFRunLoop ();
				cfRunLoop.AddSource (runLoopSource, NSRunLoop.NSDefaultRunLoopMode);

				//Now set up two notifications:
                //one to be called when a device is first matched by the I/O Kit and another to be called when the
				//device is terminated
				//Notification of first match:
				uint addedIterator = 0;
				kr = IOServiceAddMatchingNotification (
					gNotifyPort,
					kIOFirstMatchNotification,
					addingDeviceDictionary.Handle,
					DoDeviceAdded,
					IntPtr.Zero,
					ref addedIterator);

				//Iterate over set of matching devices to access already-present devices
				//and to arm the notification
				if (kr == kIOReturnSuccess) {
					DoDeviceAdded (IntPtr.Zero, addedIterator);
				}
                else {
					Debug.WriteLine ("IOServiceAddMatchingNotification result for added devices: {0}", kr);
				}

				//Notification of termination:
				uint removedIterator = 0;
				kr = IOServiceAddMatchingNotification (
					gNotifyPort,
					kIOTerminatedNotification,
					removingDeviceDictionary.Handle,
					DoDeviceRemoved,
					IntPtr.Zero,
					ref removedIterator);

				//Iterate over set of matching devices to release each one and to
				//arm the notification
				if (kr == kIOReturnSuccess) {
					DoDeviceRemoved (IntPtr.Zero, removedIterator);
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
		static void DoDeviceAdded (IntPtr p, uint addedIterator)
		{
			uint usbDevice = IOIteratorNext (addedIterator);

			while (usbDevice != 0) {
				uint parent = 0;
				uint parents = usbDevice;
				var vendor = string.Empty;
				var product = string.Empty;
				var serialNumber = string.Empty;

				while (IORegistryEntryGetParentEntry (parents, kIOServicePlane, ref parent) == kIOReturnSuccess) {
                    vendor = GetPropertyValue (parent, kUSBVendorString);
					if (!string.IsNullOrEmpty (vendor))
						Debug.WriteLine ("Vendor ID: {0}", args: vendor);

					product = GetPropertyValue (parent, kUSBProductString);
					if (!string.IsNullOrEmpty (product))
						Debug.WriteLine ("Product ID: {0}", args: product);

					serialNumber = GetPropertyValue (parent, kUSBSerialNumberString);
					if (!string.IsNullOrEmpty (serialNumber))
						Debug.WriteLine ("Serial No: {0}", args: serialNumber);

					if (parents != usbDevice) {
						if (IOObjectRelease (parents) != 0) {
							Debug.WriteLine ("Unable to release device: {0} ", parents);
						}
					}

					if (!string.IsNullOrEmpty (vendor)
						&& !string.IsNullOrEmpty (product)
						&& !string.IsNullOrEmpty (serialNumber))
						break;

					parents = parent;
				}

				var dialinDevice = GetPropertyValue (usbDevice, "IODialinDevice");

				if ((Array.IndexOf (invalidDeviceList, dialinDevice) == -1) && (Array.IndexOf (invalidDeviceList, product) == -1)) {
					Debug.WriteLine ("IODialinDevice: {0}", args: dialinDevice);
					Debug.WriteLine ("IOSerialBSDClientType: {0}", args: GetPropertyValue (usbDevice, "IOSerialBSDClientType"));
					Debug.WriteLine ("IOTTYBaseName: {0}", args: GetPropertyValue (usbDevice, "IOTTYBaseName"));
					Debug.WriteLine ("IOTTYDevice: {0}", args: GetPropertyValue (usbDevice, "IOTTYDevice"));
					Debug.WriteLine ("IOTTYSuffix: {0}", args: GetPropertyValue (usbDevice, "IOTTYSuffix"));

					Debug.WriteLine ("");

					EventHandler<MeadowDeviceArgs> addedEvent = OnDeviceAdded;
					// Fire off the Add event with the information we've gathered.
					if (addedEvent != null) {
						addedEvent (null, new MeadowDeviceArgs (
							new MeadowDevice {
								DialinDevice = dialinDevice,
								SerialBSDClientType = GetPropertyValue (usbDevice, "IOSerialBSDClientType"),
								TTYBaseName = GetPropertyValue (usbDevice, "IOTTYBaseName"),
								TTYDevice = GetPropertyValue (usbDevice, "IOTTYDevice"),
								TTYSuffix = GetPropertyValue (usbDevice, "IOTTYSuffix"),
								VendorID = vendor,
								ProductID = product,
								SerialNo = serialNumber,
							}
						));
					}
				}

				if (IOObjectRelease (usbDevice) != 0) {
					Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
				}
				usbDevice = IOIteratorNext (addedIterator);
			};
		}

        static void DoDeviceRemoved (IntPtr p, uint removedIterator)
		{
			uint usbDevice = IOIteratorNext (removedIterator);

			while (usbDevice != 0) {
				uint parent = 0;
				uint parents = usbDevice;
				var vendor = string.Empty;
				var product = string.Empty;
				var serialNumber = string.Empty;

				while (IORegistryEntryGetParentEntry (parents, kIOServicePlane, ref parent) == kIOReturnSuccess) {
					vendor = GetPropertyValue (parent, kUSBVendorString);
					if (!string.IsNullOrEmpty (vendor))
						Debug.WriteLine ("Vendor ID: {0}", args: vendor);

					product = GetPropertyValue (parent, kUSBProductString);
					if (!string.IsNullOrEmpty (product))
						Debug.WriteLine ("Product ID: {0}", args: product);

					serialNumber = GetPropertyValue (parent, kUSBSerialNumberString);
					if (!string.IsNullOrEmpty (serialNumber))
						Debug.WriteLine ("Serial No: {0}", args: serialNumber);

					if (parents != usbDevice) {
						if (IOObjectRelease (parents) != 0) {
							Debug.WriteLine ("Unable to release device: {0} ", parents);
						}
					}

					if (!string.IsNullOrEmpty (vendor)
						&& !string.IsNullOrEmpty (product)
						&& !string.IsNullOrEmpty (serialNumber))
						break;

					parents = parent;
				}

				var dialinDevice = GetPropertyValue (usbDevice, "IODialinDevice");

				if ((Array.IndexOf (invalidDeviceList, dialinDevice) == -1) && (Array.IndexOf (invalidDeviceList, product) == -1)) {

					EventHandler<MeadowDeviceArgs> removedEvent = OnDeviceRemoved;
					if (removedEvent != null) {
						removedEvent (null, new MeadowDeviceArgs (
							new MeadowDevice {
								DialinDevice = dialinDevice,
								SerialBSDClientType = GetPropertyValue (usbDevice, "IOSerialBSDClientType"),
								TTYBaseName = GetPropertyValue (usbDevice, "IOTTYBaseName"),
								TTYDevice = GetPropertyValue (usbDevice, "IOTTYDevice"),
								TTYSuffix = GetPropertyValue (usbDevice, "IOTTYSuffix"),
								VendorID = vendor,
								ProductID = product,
								SerialNo = serialNumber,
							}
						));
					}
				}

				if (IOObjectRelease (usbDevice) != 0) {
					Debug.WriteLine ("Unable to release device: {0} ", usbDevice);
				}
				usbDevice = IOIteratorNext (removedIterator);
			};

			
		}
		#endregion
	}
}
