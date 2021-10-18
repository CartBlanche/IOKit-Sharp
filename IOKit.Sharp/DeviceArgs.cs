using System;

namespace IOKit.Sharp
{
    public class DeviceArgs : EventArgs
    {
        BaseDevice device;

        public BaseDevice Device => device;

        public DeviceArgs (BaseDevice device)
        {
            this.device = device;
        }
    }
}