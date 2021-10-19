using System;
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

            // Only listen out for our devices
            serialDeviceManager.Filter = x => ((SerialDevice)x.Value).VendorName == "Wilderness Labs";

            // This way whenever out list changes the Combobox will automagically get updated.
            cbxDevices.UsesDataSource = true;
            cbxDevices.DataSource = new DevicesDataSource (serialDeviceManager.DeviceList);

            btnOpen.Activated += (o, e) => {
                if (cbxDevices.Count > 0 && cbxDevices.SelectedIndex > -1) {
                    var obj = cbxDevices.StringValue;
                    var device = serialDeviceManager.DeviceList[obj] as SerialDevice;
                    device.Open ();
                    cbxDevices.Enabled = !device.SerialPort.IsOpen;

                    if (device.SerialPort.IsOpen)
                        lblDeviceCommands.StringValue = $"{device.Key} Opened. Company: {device.VendorName}";
                    else
                        lblDeviceCommands.StringValue = string.Empty;

                    btnOpen.Title = device.SerialPort.IsOpen ? "Close" : "Open";
                }
            };

            serialDeviceManager.OnDeviceAdded += (o, e) => {
                MainThread.BeginInvokeOnMainThread (() => {
                    lblStatus.StringValue = "Added " + Environment.NewLine + e.Device.ToString ();

                    btnOpen.Enabled = serialDeviceManager.DeviceList.Count > 0;
                });
            };

            serialDeviceManager.OnDeviceRemoved += (o, e) => {
                MainThread.BeginInvokeOnMainThread (() => {
                    lblStatus.StringValue = "Removed " + Environment.NewLine + e.Device.ToString ();

                    btnOpen.Enabled = serialDeviceManager.DeviceList.Count > 0;
                });
            };

            var serial = Task.Run (() => {
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