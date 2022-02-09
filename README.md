# IOKit-Sharp
Build an IOKit Binding for MacOS .NET Applications

## Build Status
[![Build](https://github.com/CartBlanche/IOKit-Sharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/CartBlanche/IOKit-Sharp/actions)

## Project Status:
✅ **Active**.

IOKit-Sharp is a simple to use IOKit binding to (Surprise, surprise) [IOKit](https://developer.apple.com/documentation/iokit?language=objc).
As the Xamarin.Mac developers think that IOKit is too niche, the primary aim is to, over time, provide an easy way for MacOS .NET developers to integrate IOKit support into their apps.

## Nuget Status 
[![Version](https://img.shields.io/nuget/v/IOKit.Sharp.svg)](https://www.nuget.org/packages/IOKit.Sharp/)
[![Downloads](https://img.shields.io/nuget/dt/IOKit.Sharp.svg)](https://www.nuget.org/packages/IOKit.Sharp/)

## Nuget Download
📦 [NuGet](https://nuget.org/packages/IOKit.Sharp): `dotnet add package IOKit.Sharp`

## Features

- Simple Event System for each device type
- Device filtering per device type (see SerialDevice example below, but should work for other devices too)
- No external .NET dependencies

## Screenshots

<img width="585" alt="Screenshot 2021-11-22 at 18 40 56" src="https://user-images.githubusercontent.com/271363/142917485-9f14d48d-1488-4896-8c8d-48f69647b40c.png">

<img width="585" alt="Screenshot 2021-11-22 at 18 40 37" src="https://user-images.githubusercontent.com/271363/142917532-ba596288-d9e0-499e-8be5-333e236d7fed.png">

## Usage

### Quick start

It is easy to get IOKit-Sharp to notify you when a device is plugged in or removed from within your MacOS application.

```csharp

        // Some initial set-up maybe inside you ViewController
        SerialDeviceManager serialDeviceManager = new SerialDeviceManager ();
        USBDeviceManager usbDeviceManager = new USBDeviceManager ();
.
.
.
        serialDeviceManager.OnDeviceAdded += (o, e) => {
            MainThread.BeginInvokeOnMainThread (() => {
                Console.WriteLine ($"Added Serial\n{e.Device}");
            });
        };

        serialDeviceManager.OnDeviceRemoved += (o, e) => {
            MainThread.BeginInvokeOnMainThread (() => {
                Console.WriteLine ($"Removed Serial\n{e.Device}");
            });
        };

        usbDeviceManager.OnDeviceAdded += (o, e) => {
            MainThread.BeginInvokeOnMainThread (() => {
                Console.WriteLine ($"Added USB\n{e.Device}");
            });
        };

        usbDeviceManager.OnDeviceRemoved += (o, e) => {
            MainThread.BeginInvokeOnMainThread (() => {
                Console.WriteLine ($"Added USB\n{e.Device}");
            });
        };

```

Then when you are ready to start listening to the events....
```csharp
        var serial = Task.Run (() => {
            serialDeviceManager.Start ();
        });

        var usb = Task.Run (() => {
            usbDeviceManager.Start ();
        });
```

The library also supports device filtering, so you only get notified about devices you might be interested in.
Just provide the appropriate Predicate via the `Filter` property.
```csharp
        serialDeviceManager.Filter = x => ((SerialDevice)x.Value).VendorName == "Wilderness Labs";
```

## Sponsoship
[![Sponsor Me](https://img.shields.io/badge/sponsor-$$$-purple.svg)](https://github.com/sponsors/CartBlanche)


## Want to help make this better?
Have a look at our [Issues](https://github.com/CartBlanche/IOKit-Sharp/issues) page and let us know what you would like to work on and contribute.
