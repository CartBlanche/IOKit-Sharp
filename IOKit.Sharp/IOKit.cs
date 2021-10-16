using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;

namespace IOKit
{

    public static class IOKit
    {
		#region DllImports
		// This is based off of Chris Hamons' gist https://gist.github.com/chamons/82ab06f5e83d2cb10193

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern uint IOServiceGetMatchingService (uint masterPort, IntPtr matching);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern uint IOServiceGetMatchingServices (uint masterPort, IntPtr matching, ref uint iterator);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern IntPtr IOServiceMatching (string s);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern IntPtr IORegistryEntryCreateCFProperty (uint entry, IntPtr key, IntPtr allocator, uint options);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern int IOObjectRelease (uint o);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern IntPtr IONotificationPortCreate (uint o);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern IntPtr IONotificationPortGetRunLoopSource (IntPtr notify);

		[Obsolete ("IOMasterPort has been Deprecated by Apple and is most cases is no longer needed.")]
		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern int IOMasterPort (uint masterPort, ref uint matching);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern int IOServiceAddMatchingNotification (
			IntPtr notifyPort,
			string notificationType,
			IntPtr matching,
			Action<IntPtr, uint> deviceNotification,
			IntPtr refCon,
			ref uint notification);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern uint IOIteratorNext (uint iterator);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern int IORegistryEntryGetName (uint entry, IntPtr name);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern int IORegistryEntryGetParentEntry (uint current_device, string name, ref uint parent);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern IntPtr IORegistryEntrySearchCFProperty (uint entry, string plane, IntPtr key, IntPtr allocator, uint options);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern void IONotificationPortSetDispatchQueue (IntPtr gNotifyPort, IntPtr dispatchQueue);

		[DllImport ("/System/Library/Frameworks/IOKit.framework/IOKit")]
		public static extern int IORegistryEntryCreateCFProperties (uint usbDevice, IntPtr properties, IntPtr allocator, uint options);
        #endregion

		#region IOKit Contants to match the Objective C/Swift constants
		public const int kIOReturnSuccess = 0;

		public const string kIOServicePlane = "IOService";
		public const string kIOFirstMatchNotification = "IOServiceFirstMatch";
		public const string kIOTerminatedNotification = "IOServiceTerminate";

		public const string kUSBVendorString = "kUSBVendorString";
		public const string kUSBSerialNumberString = "kUSBSerialNumberString";
		public const string kUSBProductString = "kUSBProductString";
		#endregion

		// List of Devices that we aren't interested in being notified about
		static string[] invalidDeviceList = {
			"/dev/tty.Bluetooth-Incoming-Port",
			"Meadow Bootloader",
		};

        #region Accessor Methods
        public static string GetSerialNumber ()
		{
			string serial = string.Empty;
			uint platformExpert = IOServiceGetMatchingService (0, IOServiceMatching ("IOPlatformExpertDevice"));
			if (platformExpert != 0) {
				serial = GetPropertyValue (platformExpert, "IOPlatformSerialNumber");
				IOObjectRelease (platformExpert);
			}

			return serial;
		}

		public static string GetPropertyValue (uint device, string propertyName)
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
	}
}