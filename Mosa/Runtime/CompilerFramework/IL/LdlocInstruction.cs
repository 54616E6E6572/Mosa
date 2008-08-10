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

namespace Mosa.Runtime.CompilerFramework.IL
{
    /// <summary>
    /// Represents a load of a local variable.
    /// </summary>
    public class LdlocInstruction : LoadInstruction
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of <see cref="LdlocInstruction"/>.
        /// </summary>
        /// <param name="code">The opcode of the load.</param>
        public LdlocInstruction(OpCode code)
            : base(code)
        {
            // Local loads are stack operations, which are not required
            // in a register based vm.
            _ignore = true;
        }

        #endregion // Construction

        #region Properties

        #endregion // Properties

        #region Methods

        public override void Decode(IInstructionDecoder decoder)
        {
            // Opcode specific handling
            ushort locIdx;
            switch (_code)
            {
                case OpCode.Ldloc:
                    locIdx = decoder.DecodeUInt16();
                    break;

                case OpCode.Ldloc_s:
                    locIdx = decoder.DecodeByte();
                    break;

                case OpCode.Ldloc_0:
                    locIdx = 0;
                    break;

                case OpCode.Ldloc_1:
                    locIdx = 1;
                    break;

                case OpCode.Ldloc_2:
                    locIdx = 2;
                    break;

                case OpCode.Ldloc_3:
                    locIdx = 3;
                    break;

                default:
                    throw new NotImplementedException();
            }

            // Push the loaded value onto the evaluation stack
            SetResult(0, decoder.GetLocalOperand(locIdx));
        }

        public sealed override void Visit(IILVisitor visitor)
        {
            visitor.Ldloc(this);
        }

        public override string ToString()
        {
            return String.Format("{0} ; {1}", base.ToString(), this.Results[0]);
        }

        #endregion // Methods
    }
}
