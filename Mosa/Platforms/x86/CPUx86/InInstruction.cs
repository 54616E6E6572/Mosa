﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Simon Wollwage (rootnode) <kintaro@think-in-co.de>
 */

using System;
using System.Collections.Generic;
using System.Text;

using Mosa.Runtime.CompilerFramework;
using CIL = Mosa.Runtime.CompilerFramework.CIL;
using IR = Mosa.Runtime.CompilerFramework.IR;
using System.Diagnostics;

namespace Mosa.Platforms.x86.CPUx86
{
	/// <summary>
	/// Representations the x86 in instruction.
	/// </summary>
	public sealed class InInstruction : TwoOperandInstruction, IIntrinsicInstruction
	{
		#region Data Members

		private static readonly OpCode R_C_8 = new OpCode(new byte[] { 0xE4 });
		private static readonly OpCode R_R_8 = new OpCode(new byte[] { 0xEC });
		private static readonly OpCode R_C_32 = new OpCode(new byte[] { 0xE5 });
		private static readonly OpCode R_R_32 = new OpCode(new byte[] { 0xED });

		#endregion // Data Members

		#region Methods

		/// <summary>
		/// 		/// </summary>
		/// <param name="destination"></param>		/// <param name="source"></param>		/// <param name="empty"></param>		/// <returns></returns>
        protected override OpCode ComputeOpCode(Operand destination, Operand source, Operand empty)
        {
            if (IsByte(source))
            {
                if ((destination is RegisterOperand) && (source is ConstantOperand)) return R_C_8;
                if ((destination is RegisterOperand) && (source is RegisterOperand)) return R_R_8;
            }
            else
            {
                if ((destination is RegisterOperand) && (source is ConstantOperand)) return R_C_32;
                if ((destination is RegisterOperand) && (source is RegisterOperand)) return R_R_32;
            }
            throw new ArgumentException(@"No opcode for operand type.");        }
		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="visitor">The visitor object.</param>
		/// <param name="context">The context.</param>
		public override void Visit(IX86Visitor visitor, Context context)
		{
			visitor.In(context);
		}

		/// <summary>
		/// Replaces the instrinsic call site
		/// </summary>
		/// <param name="context">The context.</param>
		public void ReplaceIntrinsicCall(Context context)
		{
			context.SetInstruction(CPUx86.Instruction.InInstruction, context.Result, context.Operand1);
		}

		#endregion // Methods
	}
}
