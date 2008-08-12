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
using Mosa.Runtime.Metadata;
using Mosa.Runtime.Metadata.Signatures;

namespace Mosa.Runtime.CompilerFramework.IL
{
    /// <summary>
    /// Implements various binary comparison instructions.
    /// </summary>
    public class BinaryComparisonInstruction : BinaryInstruction
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the BinaryComparisonInstruction.
        /// </summary>
        /// <param name="code">The opcode of the instruction.</param>
        public BinaryComparisonInstruction(OpCode code)
            : base(code, 1)
        {
            // FIXME: Check opcodes
        }

        #endregion // Construction

        #region Methods

        public override void Decode(IInstructionDecoder decoder)
        {
            base.Decode(decoder);

            // Set the result
            SetResult(0, CreateResultOperand(decoder.Architecture, new SigType(CilElementType.I4)));
        }

        public override string ToString()
        {
            string result, op;
            bool un = false;
            switch (_code)
            {
                case OpCode.Ceq:
                    op = @"==";
                    break;

                case OpCode.Cgt:
                    op = @">";
                    break;

                case OpCode.Cgt_un:
                    op = @">";
                    un = true;
                    break;

                case OpCode.Clt:
                    op = @"<";
                    break;

                case OpCode.Clt_un:
                    op = @"<";
                    un = true;
                    break;

                default:
                    throw new InvalidOperationException(@"Invalid opcode.");
            }

            Operand[] ops = this.Operands;
            if (true == un)
                result = String.Format(@"{4} ; {0} = unchecked({1} {2} {3})", this.Results[0], ops[0], op, ops[1], base.ToString());
            else
                result = String.Format(@"{4} ; {0} = ({1} {2} {3})", this.Results[0], ops[0], op, ops[1], base.ToString());

            return result;
        }

        public sealed override void Visit(IILVisitor visitor)
        {
            visitor.BinaryComparison(this);
        }

        #endregion // Methods
    }
}