/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mosa.Runtime.CompilerFramework.CIL
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AddInstruction : ArithmeticInstruction
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AddInstruction"/> class.
        /// </summary>
        public AddInstruction (OpCode opCode) : base(opCode)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows visitor based dispatch for this instruction object.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <param name="context">The context.</param>
        public override void Visit (ICILVisitor visitor, Context context)
        {
            visitor.Add (context);
        }

        #endregion Methods

    }
}
