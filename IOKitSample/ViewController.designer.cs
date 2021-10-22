// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace IOKit.IOKitSample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton btnOpen { get; set; }

		[Outlet]
		AppKit.NSComboBox cbxSerialDevices { get; set; }

		[Outlet]
		AppKit.NSComboBox cbxUSBDevices { get; set; }

		[Outlet]
		AppKit.NSTextField lblDeviceCommands { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }
		
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

			if (cbxSerialDevices != null) {
				cbxSerialDevices.Dispose ();
				cbxSerialDevices = null;
			}

			if (btnOpen != null) {
				btnOpen.Dispose ();
				btnOpen = null;
			}

			if (cbxUSBDevices != null) {
				cbxUSBDevices.Dispose ();
				cbxUSBDevices = null;
			}
		}
	}
}
