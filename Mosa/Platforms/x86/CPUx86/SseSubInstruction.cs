/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Simon Wollwage (rootnode) <kintaro@think-in-co.de>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Mosa.Runtime.CompilerFramework;
using Mosa.Runtime.CompilerFramework.Operands;
using IR = Mosa.Runtime.CompilerFramework.IR;

namespace Mosa.Platforms.x86.CPUx86
{
    /// <summary>
    /// Intermediate representation of an SSE based subtraction instruction.
    /// </summary>
    public sealed class SseSubInstruction : TwoOperandInstruction
    {
        #region Data Members

        private static readonly OpCode F = new OpCode (new byte[] {
            0xf3,
            0xf,
            0x5c
        });
        private static readonly OpCode I = new OpCode (new byte[] {
            0xf2,
            0xf,
            0x5c
        });

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instruction latency.
        /// </summary>
        /// <value>The latency.</value>
        public override int Latency
        {
            get { return 3; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Computes the opcode.
        /// </summary>
        /// <param name="destination">The destination operand.</param>
        /// <param name="source">The source operand.</param>
        /// <param name="third">The third operand.</param>
        /// <returns></returns>
        protected override OpCode ComputeOpCode (Operand destination, Operand source, Operand third)
        {
            if (source.Type.Type == Mosa.Runtime.Metadata.CilElementType.R4)
                return F;
            return I;
        }
        /// <summary>
        /// Allows visitor based dispatch for this instruction object.
        /// </summary>
        /// <param name="visitor">The visitor object.</param>
        /// <param name="context">The context.</param>
        public override void Visit (IX86Visitor visitor, Context context)
        {
            visitor.SseSub (context);
        }

        #endregion
    }
}
