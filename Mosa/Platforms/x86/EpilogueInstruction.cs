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

using Mosa.Runtime.CompilerFramework;
using IL = Mosa.Runtime.CompilerFramework.IL;
using IR = Mosa.Runtime.CompilerFramework.IR;

using Mosa.Runtime.Metadata;
using Mosa.Runtime.Metadata.Signatures;

namespace Mosa.Platforms.x86
{
    /// <summary>
    /// x86 specific specialization of the <see cref="IR.EpilogueInstruction"/>.
    /// </summary>
    public sealed class EpilogueInstruction : IR.EpilogueInstruction
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of <see cref="EpilogueInstruction"/>.
        /// </summary>
        /// <param name="stackSize">The stacksize requirements of the method.</param>
        public EpilogueInstruction(int stackSize) :
            base(stackSize)
        {
        }

        #endregion // Construction

        #region EpilogueInstruction Overrides

        public override object Expand(MethodCompilerBase methodCompiler)
        {
            IArchitecture architecture = methodCompiler.Architecture;

            SigType I = new SigType(CilElementType.I);
            RegisterOperand ebp = new RegisterOperand(I, GeneralPurposeRegister.EBP);
            RegisterOperand esp = new RegisterOperand(I, GeneralPurposeRegister.ESP);

            return new Instruction[] {
                // add esp, -localsSize
                architecture.CreateInstruction(typeof(IL.AddInstruction), IL.OpCode.Add, esp, new ConstantOperand(I, -this.StackSize)),
                // pop ebp
                architecture.CreateInstruction(typeof(IR.PopInstruction), ebp),
                // ret
                architecture.CreateInstruction(typeof(IR.ReturnInstruction))
            };
        }

        #endregion // EpilogueInstruction Overrides
    }
}
