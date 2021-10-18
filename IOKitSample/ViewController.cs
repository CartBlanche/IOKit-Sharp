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


            // TODO serialDeviceManager.Filter = (x => x.VendorID == 1155 && x.ProductID == 22336);

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
                        lblDeviceCommands.StringValue = string.Format ("{0} Opened. Company: {1}, Product: {2}", device.Port, device.VendorName, device.ProductName);
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