// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace IOKit.IOKitSample
{
    [Register ("ViewController")]
    partial class ViewController
    {
		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSTextField lblDeviceCommands { get; set; }

		[Outlet]
		AppKit.NSComboBox cbxDevices { get; set; }

		[Outlet]
		AppKit.NSButton btnOpen { get; set; }


		void ReleaseDesignerOutlets ()
		{
			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (lblDeviceCommands != null) {
				lblDeviceCommands.Dispose ();
				lblDeviceCommands = null;
			}

			if (cbxDevices != null) {
				cbxDevices.Dispose ();
				cbxDevices = null;
			}

			if (btnOpen != null) {
				btnOpen.Dispose ();
				btnOpen = null;
			}
		}
	}
}
