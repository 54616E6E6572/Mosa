/*
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
    /// Represents a machine specific abstract register.
    /// </summary>
    public abstract class Register
    {
        /// <summary>
        /// Holds the machine specific index or code of the register.
        /// </summary>
        public abstract int RegisterCode { get; }

        /// <summary>
        /// Determines if this is a floating point register.
        /// </summary>
        public abstract bool IsFloatingPoint { get; }

        /// <summary>
        /// Returns the width of the register in bits.
        /// </summary>
        public abstract int Width { get; }
    }
}
