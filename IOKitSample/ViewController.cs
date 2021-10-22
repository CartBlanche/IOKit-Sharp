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
        USBDeviceManager usbDeviceManager = new USBDeviceManager ();
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // Do any additional setup after loading the view.

            // Only listen out for our devices
            serialDeviceManager.Filter = x => ((SerialDevice)x.Value).VendorName == "Wilderness Labs";

            btnOpen.Activated += (o, e) => {
                if (cbxSerialDevices.Count > 0 && cbxSerialDevices.SelectedIndex > -1) {
                    var obj = cbxSerialDevices.StringValue;
                    var device = serialDeviceManager.DeviceList[obj] as SerialDevice;
                    device.Open ();
                    cbxSerialDevices.Enabled = !device.SerialPort.IsOpen;

                    if (device.SerialPort.IsOpen)
                        lblDeviceCommands.StringValue = $"{device.Key} Opened. Company: {device.VendorName}";
                    else
                        lblDeviceCommands.StringValue = string.Empty;

                    btnOpen.Title = device.SerialPort.IsOpen ? "Close" : "Open";
                }
            };

            serialDeviceManager.OnDeviceAdded += (o, e) => {
                MainThread.BeginInvokeOnMainThread (() => {
                    lblStatus.StringValue = $"Added Serial\n{e.Device}";

                    btnOpen.Enabled = serialDeviceManager.DeviceList.Count > 0;

                    UpdateSerialComboBox ();
                });
            };

            serialDeviceManager.OnDeviceRemoved += (o, e) => {
                MainThread.BeginInvokeOnMainThread (() => {
                    lblStatus.StringValue = $"Removed Serial\n{e.Device}";

                    btnOpen.Enabled = serialDeviceManager.DeviceList.Count > 0;

                    UpdateSerialComboBox ();
                });
            };

            var serial = Task.Run (() => {
                serialDeviceManager.Start ();
            });

            usbDeviceManager.OnDeviceAdded += (o, e) => {
                MainThread.BeginInvokeOnMainThread (() => {
                    lblStatus.StringValue = $"Added USB\n {e.Device}";

                    UpdateUSBComboBox ();
                });
            };

            usbDeviceManager.OnDeviceRemoved += (o, e) => {
                MainThread.BeginInvokeOnMainThread (() => {
                    lblStatus.StringValue = $"Removed USB\n{e.Device}";

                    UpdateUSBComboBox ();
                });
            };

            var usb = Task.Run (() => {
                usbDeviceManager.Start ();
            });
        }

        private void UpdateSerialComboBox ()
        {
            // As the number of attached device will be a handful (who plugs in 20 devices at once?),
            // clearing the comboboxlist each time shouldn't be too expensive.
            cbxSerialDevices.RemoveAll ();
            foreach (var item in serialDeviceManager.DeviceList) {
                cbxSerialDevices.Add (FromObject (((SerialDevice)item.Value).Key));
            }
        }

        private void UpdateUSBComboBox ()
        {
            // As the number of attached device will be a handful (who plugs in 20 devices at once?),
            // clearing the comboboxlist each time shouldn't be too expensive.
            cbxUSBDevices.RemoveAll ();
            foreach (var item in usbDeviceManager.DeviceList) {
                cbxUSBDevices.Add (FromObject (((USBDevice)item.Value).ProductName));
            }
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