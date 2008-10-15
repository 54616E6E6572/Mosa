﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.CompilerFramework
{
    /// <summary>
    /// An enumeration identifying common linker sections.
    /// </summary>
    public enum LinkerSection
    {
        /// <summary>
        /// Identifies the program text section.
        /// </summary>
        Text = 0,

        /// <summary>
        /// Identifies the read/write data section.
        /// </summary>
        Data = 1,

        /// <summary>
        /// Identifies the read-only data section.
        /// </summary>
        ROData = 2,

        /// <summary>
        /// Identifies the bss section.
        /// </summary>
        /// <remarks>
        /// The .bss section is a chunk of memory initialized to zero by the loader.
        /// </remarks>
        BSS = 3,

        /// <summary>
        /// Holds the highest section index.
        /// </summary>
        Max = 4,
    }
}
