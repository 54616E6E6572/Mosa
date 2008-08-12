﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 *  Phil Garcia (<mailto:phil@thinkedge.com>)
*/

using System;

using Mosa.Runtime.CompilerFramework;
using IL = Mosa.Runtime.CompilerFramework.IL;

namespace Mosa.Platforms.x86
{
    /// <summary>
    /// Provides stub methods for selected x86 native assembly instructions.
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// Wraps the x86 ldit instruction to load the interrupt descriptor table.
        /// </summary>
        /// <param name="idt">A pointer to the interrupt descriptor table.</param>
        [Intrinsic(typeof(Architecture), typeof(LditInstruction))]
        public static void Ldit(IntPtr idt) { ThrowPlatformNotSupported(); }

        /// <summary>
        /// Wraps the x86 cli instruction to disable interrupts
        /// </summary>
        /// <param name="idt">A pointer to the interrupt descriptor table.</param>
        [Intrinsic(typeof(Architecture), typeof(CliInstruction))]
        public static void Cli() { ThrowPlatformNotSupported(); }

        /// <summary>
        /// Wraps the x86 sti instruction to enable interrupts
        /// </summary>
        [Intrinsic(typeof(Architecture), typeof(StiInstruction))]
        public static void Sti() { ThrowPlatformNotSupported(); }

        /// <summary>
        /// Throws a <see cref="System.PlatformNotSupportedException"/>.
        /// </summary>
        /// <remarks>
        /// This function is used by the MSIL implementations of the x86 assembly language 
        /// routines above in order to throw the <see cref="System.PlatformNotSupportedException"/> 
        /// in non-x86 compilation scenarios.
        /// </remarks>
        [Intrinsic(typeof(Architecture), typeof(IL.NopInstruction))]
        private static void ThrowPlatformNotSupported()
        {
            throw new PlatformNotSupportedException(@"This operation requires compilation for the x86 architecture.");
        }
    }
}