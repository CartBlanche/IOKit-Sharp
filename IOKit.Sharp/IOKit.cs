using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;

namespace IOKit
{

	public static class IOKit
	{
		#region DllImports
		// This is based off of Chris Hamons' gist https://gist.github.com/chamons/82ab06f5e83d2cb10193 but with a bit more functionality

		// Please add DllImport in alphabetical order
		[DllImport (iOKitPath, CharSet = CharSet.Ansi)]
		public static extern bool CFNumberGetValue (IntPtr number, CFNumberType theType, out IntPtr valuePtr);
		[DllImport (iOKitPath)]
		public static extern IntPtr CFUUIDGetConstantUUIDWithBytes (IntPtr alloc, byte byte0, byte byte1, byte byte2, byte byte3, byte byte4, byte byte5, byte byte6, byte byte7, byte byte8, byte byte9, byte byte10, byte byte11, byte byte12, byte byte13, byte byte14, byte byte15);

		[DllImport (iOKitPath)]
		public static extern int IOCreatePlugInInterfaceForService (uint device, IntPtr pluginType, IntPtr interfaceType, IntPtr theInterface, ref int theScore);

		[DllImport (iOKitPath)]
		public static extern uint IOIteratorNext (uint iterator);

		[Obsolete ("IOMasterPort has been Deprecated by Apple and is most cases is no longer needed.")]
		[DllImport (iOKitPath)]
		public static extern int IOMasterPort (uint masterPort, ref uint matching);

		[DllImport (iOKitPath)]
		public static extern IntPtr IONotificationPortCreate (uint port);

		[DllImport (iOKitPath)]
		public static extern IntPtr IONotificationPortGetRunLoopSource (IntPtr notify);

		[DllImport (iOKitPath)]
		public static extern void IONotificationPortSetDispatchQueue (IntPtr gNotifyPort, IntPtr dispatchQueue);

		[DllImport (iOKitPath)]
		public static extern int IOObjectRelease (uint device);

		[DllImport (iOKitPath)]
		public static extern int IORegistryEntryCreateCFProperties (uint device, IntPtr properties, IntPtr allocator, uint options);

		[DllImport (iOKitPath)]
		public static extern IntPtr IORegistryEntryCreateCFProperty (uint device, IntPtr key, IntPtr allocator, uint options);

		[DllImport (iOKitPath)]
		public static extern int IORegistryEntryGetName (uint device, IntPtr name);

		[DllImport (iOKitPath)]
		public static extern int IORegistryEntryGetParentEntry (uint device, string name, ref uint parent);

		[DllImport (iOKitPath)]
		public static extern int IORegistryEntryGetRegistryEntryID (uint device, ref uint deviceID);

		[DllImport (iOKitPath)]
		public static extern IntPtr IORegistryEntrySearchCFProperty (uint device, string plane, IntPtr key, IntPtr allocator, uint options);

		[DllImport (iOKitPath)]
		public static extern uint IOServiceGetMatchingService (uint masterPort, IntPtr matching);

		[DllImport (iOKitPath)]
		public static extern uint IOServiceGetMatchingServices (uint masterPort, IntPtr matching, ref uint iterator);

		[DllImport (iOKitPath)]
		public static extern int IOServiceAddMatchingNotification (
			IntPtr notifyPort,
			string notificationType,
			IntPtr matching,
			Action<IntPtr, uint> deviceNotification,
			IntPtr refCon,
			ref uint notification);

		[DllImport (iOKitPath)]
		public static extern IntPtr IOServiceMatching (string name);
		#endregion

		#region IOKit Contants to match the Objective C/Swift constants
		public const string iOKitPath = "/System/Library/Frameworks/IOKit.framework/IOKit";

		public const int kIOReturnSuccess = 0;


		/// <summary>
		/// CF number types
		/// https://github.com/xamarin/mirepoix/blob/master/src/Xamarin.NativeHelpers/CoreFoundation.cs#L124
		/// </summary>
		public enum CFNumberType
		{
			SInt8 = 1,
			SInt16 = 2,
			SInt32 = 3,
			SInt64 = 4,
			Float32 = 5,
			Float64 = 6,
			Char = 7,
			Short = 8,
			Int = 9,
			Long = 10,
			LongLong = 11,
			Float = 12,
			Double = 13,
			CFIndex = 14,
			NSInteger = 15,
			CGFloat = 16
		}
		#region From IOKitKeys.h
		// IOKit Keys pulled from https://opensource.apple.com/source/xnu/xnu-792/iokit/IOKit/IOKitKeys.h.auto.html

		// registry plane names
		public const string kIOServicePlane = "IOService";
		public const string kIOPowerPlane = "IOPower";
		public const string kIODeviceTreePlane = "IODeviceTree";
		public const string kIOAudioPlane = "IOAudio";
		public const string kIOFireWirePlane = "IOFireWire";
		public const string kIOUSBPlane = "IOUSB";

		// IOService class name
		public const string kIOResourcesClass = "IOResources";

		// IOResources class name
		public const string kIOServiceClass = "IOService";

		// IOService notification types
		public const string kIOPublishNotification = "IOServicePublish";
		public const string kIOFirstPublishNotification = "IOServiceFirstPublish";
		public const string kIOMatchedNotification = "IOServiceMatched";
		public const string kIOFirstMatchNotification = "IOServiceFirstMatch";
		public const string kIOTerminatedNotification = "IOServiceTerminate";

		public const string kUSBVendorID = "idVendor";
		public const string kUSBVendorString = "kUSBVendorString";
		public const string kUSBSerialNumberString = "kUSBSerialNumberString";
		public const string kUSBProductID = "idProduct";
		public const string kUSBProductString = "kUSBProductString";

		// property of root that describes the machine's serial number as a string
		public const string kIOPlatformSerialNumberKey = "IOPlatformSerialNumber";
		#endregion

		#region From IOKitLib.h
		public const int kIORegistryIterateRecursively = 0x00000001;
		public const int kIORegistryIterateParents = 0x00000002;
		#endregion

		#region From IOSerialKeys.h
		// Serial constants pulled in from https://opensource.apple.com/source/IOSerialFamily/IOSerialFamily-6/IOSerialFamily.kmodproj/IOSerialKeys.h.auto.html
		public const string kIOSerialBSDServiceValue = "IOSerialBSDClient";

		// Keys
		public const string kIOCalloutDeviceKey = "IOCalloutDevice";
		public const string kIODialinDeviceKey = "IODialinDevice";
		public const string kIOSerialBSDTypeKey = "IOSerialBSDClientType";
		public const string kIOTTYDeviceKey = "IOTTYDevice";
		public const string kIOTTYBaseNameKey = "IOTTYBaseName";
		public const string kIOTTYSuffixKey = "IOTTYSuffix";
		#endregion

		#region From IOUSBLib.h
		public const string kIOUSBDeviceClassName = "IOUSBDevice";
		public const string kIOUSBInterfaceClassName = "IOUSBInterface";

		public static IntPtr kIOUSBDeviceUserClientTypeID = CFUUIDGetConstantUUIDWithBytes (IntPtr.Zero,
														  0x9d, 0xc7, 0xb7, 0x80, 0x9e, 0xc0, 0x11, 0xD4,
														  0xa5, 0x4f, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61);

		public static IntPtr kIOUSBDeviceInterfaceID = CFUUIDGetConstantUUIDWithBytes (IntPtr.Zero,
															 0x5c, 0x81, 0x87, 0xd0, 0x9e, 0xf3, 0x11, 0xD4,
															 0x8b, 0x45, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61);
		public static IntPtr kIOCFPlugInInterfaceID = CFUUIDGetConstantUUIDWithBytes (IntPtr.Zero,
															0xC2, 0x44, 0xE8, 0x58, 0x10, 0x9C, 0x11, 0xD4,
															0x91, 0xD4, 0x00, 0x50, 0xE4, 0xC6, 0x42, 0x6F);
		#endregion

		#endregion

		#region Helper Methods
		public static string GetSerialNumber ()
		{
			string serial = string.Empty;
			uint platformExpert = IOServiceGetMatchingService (0, IOServiceMatching ("IOPlatformExpertDevice"));
			if (platformExpert != 0) {
				serial = GetPropertyStringValue (platformExpert, kIOPlatformSerialNumberKey);
				IOObjectRelease (platformExpert);
			}

			return serial;
		}

		public static string FindParentPropertyValue (uint device, string propertyName)
		{
			string returnStr = string.Empty;

			NSString key = (NSString)propertyName;
			IntPtr propertyPointer = IORegistryEntrySearchCFProperty (device, kIOServicePlane, key.Handle, IntPtr.Zero, kIORegistryIterateParents | kIORegistryIterateRecursively);
			if (propertyPointer != IntPtr.Zero) {
				returnStr = CFString.FromHandle (propertyPointer);
			}

			return returnStr;
		}

		public static string GetPropertyStringValue (uint device, string propertyName)
		{
			string returnStr = string.Empty;

			NSString key = (NSString)propertyName;
			IntPtr propertyPointer = IORegistryEntryCreateCFProperty (device, key.Handle, IntPtr.Zero, 0);
			if (propertyPointer != IntPtr.Zero) {
				returnStr = CFString.FromHandle (propertyPointer);
			}

			return returnStr;
		}

		public static uint GetPropertyUIntValue (uint device, string propertyName)
		{
			uint returnValue = 0;

			NSString key = (NSString)propertyName;
			IntPtr propertyPointer = IORegistryEntryCreateCFProperty (device, key.Handle, IntPtr.Zero, 0);
			if (propertyPointer != IntPtr.Zero) {
				var bsdValue = IntPtr.Zero;
				var result = CFNumberGetValue (propertyPointer, CFNumberType.Int, out bsdValue);
				if (result)
					returnValue = (uint)bsdValue.ToInt32 ();
			}

			return returnValue;
		}

		unsafe public static string GetDeviceName (uint device)
		{
			char[] deviceNameBuffer = new char[256];
			string deviceName = string.Empty;

			fixed (char* pChar = deviceNameBuffer) {
				IntPtr intPtr = new IntPtr ((void*)pChar);
				var result = IOKit.IORegistryEntryGetName (device, intPtr);
				if (result == 0) {
					deviceName = Marshal.PtrToStringAuto (intPtr);
				}
			}
			return deviceName;
		}
		#endregion
	}
}