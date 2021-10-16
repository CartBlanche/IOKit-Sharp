using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using IOKit.Sharp;
using Xamarin.Essentials;

namespace IOKit.IOKitSample
{
    public partial class ViewController : NSViewController
    {
        SerialDeviceManager serialDeviceManager = new SerialDeviceManager ();
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // Do any additional setup after loading the view.
            serialDeviceManager.OnDeviceAdded += (o, e) => {
                lblStatus.StringValue = "Added " + Environment.NewLine + e.Device.ToString ();

                // List our attached devices
                lblDevices.StringValue = string.Join (
                        Environment.NewLine,
                        serialDeviceManager.DeviceList.Select (pair => pair.Key.ToString ()).ToArray ()
                    );
            };

            serialDeviceManager.OnDeviceRemoved += (o, e) => {
                lblStatus.StringValue = "Removed " + Environment.NewLine + e.Device.ToString ();

                // List any attached devices
                lblDevices.StringValue = string.Join (
                        Environment.NewLine,
                        serialDeviceManager.DeviceList.Select (pair => pair.Key.ToString ()).ToArray ()
                    );
            };

            var t = Task.Run (() => {
                serialDeviceManager.Start ();
            });
        }

        public override NSObject RepresentedObject {
            get {
                return base.RepresentedObject;
            }
            set {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
