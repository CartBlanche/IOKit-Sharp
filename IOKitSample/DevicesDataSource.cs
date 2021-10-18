using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using Foundation;
using IOKit.Sharp;

namespace IOKit.IOKitSample
{
    public class DevicesDataSource : NSComboBoxDataSource
    {
        readonly Dictionary<string, BaseDevice> source;

        public DevicesDataSource (Dictionary<string, BaseDevice> source)
        {
            this.source = source;
        }

        public override string CompletedString (NSComboBox comboBox, string uncompletedString)
        {
            return source.Where (n => n.Key.StartsWith (uncompletedString, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault ().Key;
        }

        public override nint IndexOfItem (NSComboBox comboBox, string value)
        {
            return Array.FindIndex (source.ToArray (), x => x.Key == value);
        }

        public override nint ItemCount (NSComboBox comboBox)
        {
            return source.Count;
        }

        public override NSObject ObjectValueForItem (NSComboBox comboBox, nint index)
        {
            return NSObject.FromObject (source.Keys.ElementAt ((int)index));
        }
    }
}
