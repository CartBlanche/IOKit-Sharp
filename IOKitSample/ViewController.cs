using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using Xamarin.Essentials;

namespace IOKit.IOKitSample
{
    public partial class ViewController : NSViewController
    {
        Dictionary<string, MeadowDevice> deviceList = new Dictionary<string, MeadowDevice>();
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // Do any additional setup after loading the view.

            IOKit.OnDeviceAdded += (o, e) => {
                lblStatus.StringValue = "Added " + Environment.NewLine + e.Device.ToString ();

                // Add the device in. If it already exists it should just be replaced.
                deviceList[e.Device.TTYDevice] = e.Device;

                // List our attached devices
                lblDevices.StringValue = string.Join (
                        Environment.NewLine,
                        deviceList.Select (pair => pair.Key.ToString ()).ToArray ()
                    );
            };

            IOKit.OnDeviceRemoved += (o, e) => {
                lblStatus.StringValue = "Removed " + Environment.NewLine + e.Device.ToString ();

                // Remove the device from the list
                deviceList.Remove (e.Device.TTYDevice);

                // List any attached devices
                lblDevices.StringValue = string.Join (
                        Environment.NewLine,
                        deviceList.Select (pair => pair.Key.ToString ()).ToArray ()
                    );
            };

            var t = Task.Run (() => {
                IOKit.InitialiseSerialUSB ();
            });
            //t.Wait ();
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
