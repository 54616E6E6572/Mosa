﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using Mosa.ClassLib;

namespace Mosa.Devices
{

    public abstract class HardwareDevice : Device
    {

        public HardwareDevice() { base.deviceStatus = DeviceStatus.Initializing; }

        public abstract bool Setup();
        public abstract bool Probe();
        public abstract bool Start();
        public abstract LinkedList<IDevice> CreateSubDevices();
        public abstract bool OnInterrupt();

        public bool Activate(DeviceManager deviceManager)
        {
            base.deviceStatus = DeviceStatus.Initializing;

            if (!Setup()) {
                base.deviceStatus = DeviceStatus.Error;
                return false;
            }

            if (!Probe()) {
                base.deviceStatus = DeviceStatus.NotFound;
                return false;
            }

            if (!Start()) {
                base.deviceStatus = DeviceStatus.Error;
                return false;
            }

            base.deviceStatus = DeviceStatus.Online;

            LinkedList<IDevice> devices = CreateSubDevices();

            if (devices != null)
                foreach (IDevice device in devices)
                    deviceManager.Add(device);

            return true;
        }

    }
}