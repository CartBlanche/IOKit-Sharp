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
		AppKit.NSTextField lblDevices { get; set; }


		void ReleaseDesignerOutlets ()
		{
			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (lblDevices != null) {
				lblDevices.Dispose ();
				lblDevices = null;
			}
		}
	}
}
