﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using Mosa.ClassLib;

namespace Mosa.DeviceDrivers
{
	public interface IResourceManager
	{
		IOPortResources IOPortResources { get; }
		MemoryResources MemoryResources { get; }
		InterruptHandler InterruptHandler { get; }
	}
}
