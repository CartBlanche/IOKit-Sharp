using System;
using System.Collections.Generic;

namespace IOKit.Sharp
{
    public abstract class BaseDeviceManager
    {
        #region Event Handlers
        public EventHandler<DeviceArgs> OnDeviceAdded;

        public EventHandler<DeviceArgs> OnDeviceRemoved;
        #endregion

        protected Dictionary<string, BaseDevice> deviceList = new Dictionary<string, BaseDevice> ();
        public Dictionary<string, BaseDevice> DeviceList => deviceList;

        public abstract void Start ();
    }
}