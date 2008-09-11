﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using Mosa.DeviceDrivers;

namespace Mosa.DeviceDrivers
{
    /// <summary>
    /// 
    /// </summary>
	public class BusResources : IBusResources
	{
        /// <summary>
        /// 
        /// </summary>
		protected IResourceManager resourceManager;

        /// <summary>
        /// 
        /// </summary>
		protected IIOPortRegion[] ioPortRegions;
        /// <summary>
        /// 
        /// </summary>
		protected IMemoryRegion[] memoryRegions;
        /// <summary>
        /// 
        /// </summary>
		protected IInterruptHandler interruptHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusResources"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <param name="ioPortRegions">The io port regions.</param>
        /// <param name="memoryRegions">The memory regions.</param>
        /// <param name="interruptHandler">The interrupt handler.</param>
		public BusResources(IResourceManager resourceManager, IIOPortRegion[] ioPortRegions, IMemoryRegion[] memoryRegions, IInterruptHandler interruptHandler)
		{
			this.resourceManager = resourceManager;
			this.ioPortRegions = ioPortRegions;
			this.memoryRegions = memoryRegions;
			this.interruptHandler = interruptHandler;
		}

        /// <summary>
        /// Gets the IO port region.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
		public IIOPortRegion GetIOPortRegion(byte index)
		{
			return ioPortRegions[index];
		}

        /// <summary>
        /// Gets the memory region.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
		public IMemoryRegion GetMemoryRegion(byte index)
		{
			return memoryRegions[index];
		}

        /// <summary>
        /// Gets the IO port.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
		public IReadWriteIOPort GetIOPort(byte region, ushort index)
		{
			return resourceManager.IOPortResources.GetIOPort(ioPortRegions[region].BaseIOPort, index);
		}

        /// <summary>
        /// Gets the memory.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns></returns>
		public IMemory GetMemory(byte region)
		{
			return resourceManager.MemoryResources.GetMemory(memoryRegions[region].BaseAddress, memoryRegions[region].Size);
		}

        /// <summary>
        /// Gets the IRQ.
        /// </summary>
        /// <value>The IRQ.</value>
		public byte IRQ
		{
			get
			{
				if (interruptHandler == null)
					return 0xFF;	// 0xFF means unused

				return interruptHandler.IRQ;
			}
		}

        /// <summary>
        /// Enables the IRQ.
        /// </summary>
		public void EnableIRQ()
		{
			interruptHandler.Enable();
		}

        /// <summary>
        /// Disables the IRQ.
        /// </summary>
		public void DisableIRQ()
		{
			interruptHandler.Enable();
		}

	}

}
