﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

// References
// http://www.t13.org/Documents/UploadedDocuments/docs2004/d1572r3-EDD3.pdf
// http://www.osdever.net/tutorials/lba.php
// http://www.nondot.org/sabre/os/files/Disk/IDE-tech.html

using Mosa.Devices;
using Mosa.ClassLib;
using Mosa.Devices.ISA;
using Mosa.Devices.Kernel;    // for IIPort

namespace Mosa.Devices.ISA.DiskControllers
{
    [ISADeviceSignature(AutoLoad = true, BasePort = 0x1F0, PortRange = 8)]
    [ISADeviceSignature(AutoLoad = false, BasePort = 0x170, PortRange = 8, ForceOption = "ide2")]
    public class IDEDiskDriver : ISAHardwareDevice, IDevice, IHardwareDevice, IDiskControllerDevice
    {

        #region Definitions

        internal struct IDECommand
        {
            internal const byte ReadSectorsWithRetry = 0x20;
            internal const byte WriteSectorsWithRetry = 0x30;
            internal const byte IdentifyDrive = 0xEC;
        };


        internal struct IdentifyDrive
        {
            internal const uint GeneralConfig = 0x00;
            internal const uint LogicalCylinders = 0x02;
            internal const uint LogicalHeads = 0x08;
            internal const uint LogicalSectors = 0x06 * 2;
            internal const uint SerialNbr = 0x14;
            internal const uint ControllerType = 0x28;
            internal const uint BufferSize = 0x15 * 2;
            internal const uint FirmwareRevision = 0x17 * 2;
            internal const uint ModelNumber = 0x1B * 2;
            internal const uint SupportDoubleWord = 0x30 * 2;

            internal const uint CommandSetSupported83 = 83 * 2; // 1 word
            internal const uint MaxLBA28 = 60 * 2; // 2 words
            internal const uint MaxLBA48 = 100 * 2;	// 3 words
        }

        #endregion

        protected SpinLock spinLock;

        public const uint DrivesPerConroller = 2; // the maximum supported

        protected IReadWriteIOPort DataPort;
        protected IReadWriteIOPort FeaturePort;
        protected IReadOnlyIOPort ErrorPort;
        protected IReadWriteIOPort SectorCountPort;
        protected IReadWriteIOPort LBALowPort;
        protected IReadWriteIOPort LBAMidPort;
        protected IReadWriteIOPort LBAHighPort;
        protected IReadWriteIOPort DeviceHeadPort;
        protected IReadOnlyIOPort StatusPort;
        protected IWriteOnlyIOPort CommandPort;

        //protected IRQHandler IdeIRQ;

        public enum LBAType
        {
            LBA28,
            LBA48
        }

        protected struct DriveInfo
        {
            public bool Present;
            public uint MaxLBA;

            public LBAType LBAType;

            // legacy info
            public uint Cylinders;
            public uint Heads;
            public uint SectorsPerTrack;
        }

        protected DriveInfo[] driveInfo;

        public IDEDiskDriver()
        {
            driveInfo = new DriveInfo[DrivesPerConroller];
        }

        public void Dispose() { }

        public override bool Setup()
        {
            base.name = "IDE_0x" + base.isaBusResources.GetIOPortRegion(0).GetPort(0).Address.ToString("X");

            DataPort = base.isaBusResources.GetIOPortRegion(0).GetPort(0);
            ErrorPort = base.isaBusResources.GetIOPortRegion(0).GetPort(1);
            FeaturePort = base.isaBusResources.GetIOPortRegion(0).GetPort(1);
            SectorCountPort = base.isaBusResources.GetIOPortRegion(0).GetPort(2);
            LBALowPort = base.isaBusResources.GetIOPortRegion(0).GetPort(3);
            LBAMidPort = base.isaBusResources.GetIOPortRegion(0).GetPort(4);
            LBAHighPort = base.isaBusResources.GetIOPortRegion(0).GetPort(5);
            DeviceHeadPort = base.isaBusResources.GetIOPortRegion(0).GetPort(6);
            CommandPort = base.isaBusResources.GetIOPortRegion(0).GetPort(7);
            StatusPort = base.isaBusResources.GetIOPortRegion(0).GetPort(7);

            for (int drive = 0; drive < DrivesPerConroller; drive++) {
                driveInfo[drive].Present = false;
                driveInfo[drive].MaxLBA = 0;
            }

            return true;
        }

        public override bool Probe()
        {
            LBALowPort.Write8Bits(0x88);

            if (LBALowPort.Read8Bits() != 0x88)
                return false;

            return true;
        }

        public override bool Start()
        {
            DeviceHeadPort.Write8Bits(0xA0);

            // TODO
            //Timer.Delay(1000 / 250); // wait 1/250th of a second

            if ((StatusPort.Read8Bits() & 0x40) == 0x40)
                driveInfo[0].Present = true;

            DeviceHeadPort.Write8Bits(0xB0);

            // TODO
            //Timer.Delay(1000 / 250); // wait 1/250th of a second

            if ((StatusPort.Read8Bits() & 0x40) == 0x40)
                driveInfo[1].Present = true;

            return true;
        }

        public override LinkedList<IDevice> CreateSubDevices()
        {
            LinkedList<IDevice> devices = new LinkedList<IDevice>();

            for (uint drive = 0; drive < DrivesPerConroller; drive++) {
                if (driveInfo[drive].Present) {
                    Open(drive);

                    //TextMode.Write(base.name);
                    //TextMode.Write(": Disk #");
                    //TextMode.Write((int)drive);
                    //TextMode.Write(" - ", (int)(driveInfo[drive].MaxLBA / 1024 / 2));
                    //TextMode.Write("MB, LBA=", (int)driveInfo[drive].MaxLBA);
                    //TextMode.WriteLine("");

                    devices.Add(new DiskDevice(this, drive, false));
                }
            }
            return devices;
        }

        public override bool OnInterrupt() { return true; }

        protected bool WaitForReqisterReady()
        {
            while (true) {
                uint status = StatusPort.Read8Bits();

                if ((status & 0x08) == 0x08)
                    return true;

                //TODO: add timeout check
            }

            //return false;
        }

        protected enum SectorOperation { Read, Write }

        protected bool PerformLBA28(SectorOperation operation, uint driveNbr, uint lba, byte[] data, uint offset)
        {
            FeaturePort.Write8Bits(0);
            SectorCountPort.Write8Bits(1);

            LBALowPort.Write8Bits((byte)(lba & 0xFF));
            LBAMidPort.Write8Bits((byte)((lba >> 8) & 0xFF));
            LBAHighPort.Write8Bits((byte)((lba >> 16) & 0xFF));

            DeviceHeadPort.Write8Bits((byte)(0xE0 | (driveNbr << 4) | ((lba >> 24) & 0x0F)));

            if (operation == SectorOperation.Write)
                CommandPort.Write8Bits(IDECommand.WriteSectorsWithRetry);
            else
                CommandPort.Write8Bits(IDECommand.ReadSectorsWithRetry);

            if (!WaitForReqisterReady())
                return false;

            BinaryFormat sector = new BinaryFormat(data);

            //TODO: Don't use PIO
            if (operation == SectorOperation.Read) {
                for (uint index = 0; index < 256; index++)
                    sector.SetUShort(offset + (index * 2), DataPort.Read16Bits());
            }
            else {
                for (uint index = 0; index < 256; index++)
                    DataPort.Write16Bits(sector.GetUShort(offset + (index * 2)));
            }

            return true;
        }

        //protected bool ReadLBA48 (SectorOperation operation, uint drive, uint lba, MemoryBlock memory)
        //{
        //    FeaturePort.Write8Bits (0);
        //    FeaturePort.Write8Bits (0);

        //    SectorCountPort.Write8Bits (0);
        //    SectorCountPort.Write8Bits (1);

        //    LBALowPort.Write8Bits ((byte)((lba >> 24) & 0xFF));
        //    LBALowPort.Write8Bits ((byte)(lba & 0xFF));

        //    LBAMidPort.Write8Bits ((byte)((lba >> 32) & 0xFF));
        //    LBAMidPort.Write8Bits ((byte)((lba >> 8) & 0xFF));

        //    LBAHighPort.Write8Bits ((byte)((lba >> 40) & 0xFF));
        //    LBAHighPort.Write8Bits ((byte)((lba >> 16) & 0xFF));

        //    DeviceHeadPort.Write8Bits ((byte)(0x40 | (drive << 4)));

        //    if (operation == SectorOperation.Write)
        //        CommandPort.Write8Bits (0x34);
        //    else
        //        CommandPort.Write8Bits (0x24);

        //    if (!WaitForReqisterReady ())
        //        return false;

        //    //TODO: Don't use PIO
        //    if (operation == SectorOperation.Read) {
        //        for (uint index = 0; index < 256; index++)
        //            memory.SetUShort (index * 2, DataPort.Read16Bits ());
        //    }
        //    else {
        //        for (uint index = 0; index < 256; index++)
        //            DataPort.WriteUShort (memory.GetUShort (index * 2));
        //    }

        //    return true;
        //}

        public bool Open(uint driveNbr)
        {
            if (driveNbr == 0)
                DeviceHeadPort.Write8Bits(0xA0);
            else
                if (driveNbr == 1)
                    DeviceHeadPort.Write8Bits(0xB0);
                else
                    return false;

            CommandPort.Write8Bits(IDECommand.IdentifyDrive);

            if (!WaitForReqisterReady())
                return false;

            BinaryFormat info = new BinaryFormat(new byte[512]);

            for (uint index = 0; index < 256; index++)
                info.SetUShort(index * 2, DataPort.Read16Bits());

            driveInfo[driveNbr].MaxLBA = info.GetUInt(IdentifyDrive.MaxLBA28);

            // legacy info
            driveInfo[driveNbr].Heads = info.GetUShort(IdentifyDrive.LogicalHeads);
            driveInfo[driveNbr].Cylinders = info.GetUShort(IdentifyDrive.LogicalCylinders);
            driveInfo[driveNbr].SectorsPerTrack = info.GetUShort(IdentifyDrive.LogicalSectors);

            return true;
        }

        public bool Release(uint driveNbr)
        {
            return true;
        }

        public uint GetSectorSize(uint driveNbr)
        {
            return 512;
        }

        public uint GetTotalSectors(uint driveNbr)
        {
            return driveInfo[driveNbr].MaxLBA;
        }

        public bool CanWrite(uint drive)
        {
            return true;
        }

        public bool ReadBlock(uint driveNbr, uint block, uint count, byte[] data)
        {
            if (data.Length < count * 512)
                return false;

            try {
                spinLock.Enter();
                for (uint index = 0; index < count; index++) {
                    if (!PerformLBA28(SectorOperation.Read, driveNbr, block, data, index * 512))
                        return false;
                }
                return true;
            }
            finally {
                spinLock.Exit();
            }
        }

        public bool WriteBlock(uint driveNbr, uint block, uint count, byte[] data)
        {
            if (data.Length < count * 512)
                return false;

            try {
                spinLock.Enter();
                for (uint index = 0; index < count; index++) {
                    if (!PerformLBA28(SectorOperation.Write, driveNbr, block, data, index * 512))
                        return false;
                }
                return true;
            }
            finally {
                spinLock.Exit();
            }
        }
    }
}