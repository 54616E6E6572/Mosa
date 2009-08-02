/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using Mosa.EmulatedKernel;
using Mosa.EmulatedDevices.Emulated;

namespace Mosa.EmulatedDevices
{
	/// <summary>
	/// 
	/// </summary>
	public static class Setup
	{
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public static void Initialize()
		{
			PCIBus pciBus = new PCIBus();

			pciBus.Add(new PCIController(PCIController.StandardIOBase, pciBus));
			IOPortDispatch.RegisterDevice(pciBus.Get(0) as IIOPortDevice);

			IOPortDispatch.RegisterDevice(new CMOS(CMOS.StandardIOBase));
			IOPortDispatch.RegisterDevice(new VGAText());

			string[] files = new string[1];
			files[0] = @"..\..\Data\HardDriveImage\hd.img";

			// Fix for Linux
			files[0] = files[0].Replace('\\', System.IO.Path.DirectorySeparatorChar);

			IOPortDispatch.RegisterDevice(new IDEController(IDEController.PrimaryIOBase, files));
		}
	}
}
