/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

namespace Mosa.Devices
{
	public enum DeviceStatus : byte
	{
		Initializing,
		Online,
		Offline,
		NotFound,
		Error
	}
}