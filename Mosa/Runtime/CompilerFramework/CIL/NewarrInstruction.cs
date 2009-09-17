﻿/*
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

using Mosa.Runtime.Metadata;

namespace Mosa.Runtime.CompilerFramework.CIL
{
	/// <summary>
	/// 
	/// </summary>
	public class NewarrInstruction : UnaryInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="NewarrInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public NewarrInstruction(OpCode opcode)
			: base(opcode, 1)
		{
		}

		#endregion // Construction

		#region CILInstruction Overrides

		/// <summary>
		/// Decodes the specified instruction.
		/// </summary>
		/// <param name="instruction">The instruction.</param>
		/// <param name="decoder">The instruction decoder, which holds the code stream.</param>
		public override void Decode(ref InstructionData instruction, IInstructionDecoder decoder)
		{
			// Decode base classes first
			base.Decode(ref instruction, decoder);

			// Read the type specification
			TokenTypes arrayEType;
			decoder.Decode(out arrayEType);
			throw new NotImplementedException();
			/*
				TypeReference eType = MetadataTypeReference.FromToken(decoder.Metadata, arrayEType);

				// FIXME: If _operands[0] is an integral constant, we can infer the maximum size of the array
				// and instantiate an ArrayTypeSpecification with max. sizes. This way we could eliminate bounds
				// checks in an optimization stage later on, if we find that a value never exceeds the array 
				// bounds.

				// Build a type specification
				ArrayTypeSpecification typeRef = new ArrayTypeSpecification(eType);
				_results[0] = CreateResultOperand(typeRef);
			 */
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="instruction">The instruction.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString(ref InstructionData instruction)
		{
			throw new NotImplementedException();
			//            TypeSpecification typeSpec = (TypeSpecification)_results[0].Type;
			//            return String.Format(@"{0} = new {1}[{2}]", _results[0], typeSpec.ElementType, _operands[0]);
		}

		#endregion // CILInstruction Overrides

	}
}
