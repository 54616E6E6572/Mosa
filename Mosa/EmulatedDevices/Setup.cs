/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

namespace Mosa.EmulatedDevices
{
    /// <summary>
    /// 
    /// </summary>
	public static class Setup
	{
        /// <summary>
        /// 
        /// </summary>
		public static void Initialize()
		{
			new CMOS(CMOS.StandardIOBase);
			new VGATextDriver(VGATextDriver.StandardAddressBase);

			string[] files = new string[1];
			files[0] = @"..\..\..\hd.img";

			new IDEDiskDevice(IDEDiskDevice.PrimaryIOBase, files);
		}
	}
}
