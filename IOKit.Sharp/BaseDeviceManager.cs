using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreFoundation;
using Foundation;

namespace IOKit.Sharp
{
	public abstract class BaseDeviceManager
	{
		#region Event Handlers
		public EventHandler<DeviceArgs> OnDeviceAdded;

		public EventHandler<DeviceArgs> OnDeviceRemoved;
		#endregion

		public Predicate<KeyValuePair<string, BaseDevice>> Filter = null;

		protected Dictionary<string, BaseDevice> deviceList = new Dictionary<string, BaseDevice> ();
		public Dictionary<string, BaseDevice> DeviceList {
            get {
				if (deviceList.Count > 0 && Filter != null)
					return deviceList.Filter (Filter);
				else
					return deviceList;
			}
        }

		#region Device Callbacks, that MUST be implemented in derived classes
		protected abstract void DoDeviceAdded (IntPtr p, uint addedIterator);

		protected abstract void DoDeviceRemoved (IntPtr p, uint removedIterator);
		#endregion

		public abstract void Start ();

		protected void Start (string serviceMatchingName)
		{
			try {
				int kr;

				var ioServiceMatching = IOKit.IOServiceMatching (serviceMatchingName);
				NSMutableDictionary addingDeviceDictionary = ObjCRuntime.Runtime.GetNSObject<NSMutableDictionary> (ioServiceMatching);
				/* TODO doesn't look like it's needed or doing any auto filtering on this hence the `invalidDeviceList`
				 * addingDeviceDictionary["USBVendorID"] = new NSNumber (0x2E6A);
				 * addingDeviceDictionary["USBProductID"] = new NSNumber (0x0001); */
				NSMutableDictionary removingDeviceDictionary = new NSMutableDictionary (addingDeviceDictionary);

				//To set up asynchronous notifications, create a notification port and
				//add its run loop event source to the program’s run loop
				IntPtr gNotifyPort = IOKit.IONotificationPortCreate (0);
				var dq = new DispatchQueue (serviceMatchingName + ".Detector");
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
					DoDeviceAdded,
					IntPtr.Zero,
					ref addedIterator);

				//Iterate over set of matching devices to access already-present devices
				//and to arm the notification
				if (kr == IOKit.kIOReturnSuccess) {
					DoDeviceAdded (IntPtr.Zero, addedIterator);
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
					DoDeviceRemoved,
					IntPtr.Zero,
					ref removedIterator);

				//Iterate over set of matching devices to release each one and to
				//arm the notification
				if (kr == IOKit.kIOReturnSuccess) {
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
	}

	public static class HelperExtensions
	{
		/// <summary>
		/// Filter a dictionary, using the given predicate.
		/// </summary>
		public static Dictionary<Key, Value> Filter<Key, Value> (this Dictionary<Key, Value> dictionary,
				Predicate<KeyValuePair<Key, Value>> predicate)
		{
			var filteredEnumerable = dictionary.Where (x => predicate (x));

			// Note a new dictionary is created (some overhead)
			return filteredEnumerable.ToDictionary (d => d.Key, d => d.Value);
		}
	}
}