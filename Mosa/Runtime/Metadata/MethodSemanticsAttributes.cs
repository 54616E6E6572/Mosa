/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the GNU GPL v3, with Classpath Linking Exception
 * Licensed under the terms of the New BSD License for exclusive use by the Ensemble OS Project
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    public enum MethodSemanticsAttributes
    {
        /// <summary>
        /// 
        /// </summary>
        Setter = 0x0001,
        /// <summary>
        /// 
        /// </summary>
        Getter = 0x0002,
        /// <summary>
        /// 
        /// </summary>
        Other = 0x0004,
        /// <summary>
        /// 
        /// </summary>
        AddOn = 0x0008,
        /// <summary>
        /// 
        /// </summary>
        RemoveOn = 0x0010,
        /// <summary>
        /// 
        /// </summary>
        Fire = 0x0020
    }
}
