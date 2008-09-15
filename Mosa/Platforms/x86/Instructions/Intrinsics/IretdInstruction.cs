﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (<mailto:phil@thinkedge.com>)
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using IR = Mosa.Runtime.CompilerFramework.IR;

using Mosa.Runtime.CompilerFramework;


namespace Mosa.Platforms.x86.Instructions.Intrinsics
{
    /// <summary>
    /// Intrinsic instruction implementation for the x86 cli instruction.
    /// </summary>
    public sealed class IretdInstruction : IR.IRInstruction
    {
        #region Construction

        /// <summary>
        /// 
        /// </summary>
        public IretdInstruction()
        {
        }

        #endregion // Construction

        #region CliInstruction Overrides

        /// <summary>
        /// Allows visitor based dispatch for this instruction object.
        /// </summary>
        /// <param name="visitor">The visitor object.</param>
        /// <param name="arg">A visitor specific context argument.</param>
        /// <typeparam name="ArgType">An additional visitor context argument.</typeparam>
        protected override void Visit<ArgType>(IR.IIRVisitor<ArgType> visitor, ArgType arg)
        {
            IX86InstructionVisitor<ArgType> x86visitor = visitor as IX86InstructionVisitor<ArgType>;
            Debug.Assert(null != x86visitor);
            if (null != x86visitor)
                x86visitor.Iretd(this, arg);
        }

        /// <summary>
        /// Returns a string representation of the instruction.
        /// </summary>
        /// <returns>
        /// A string representation of the instruction in intermediate form.
        /// </returns>
        public override string ToString()
        {
            return String.Format(@"x86 iretd");
        }

        #endregion // CliInstruction Overrides
    }
}
