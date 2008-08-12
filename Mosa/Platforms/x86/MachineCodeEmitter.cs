﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 *  Simon Wollwage (<mailto:simon_wollwage@yahoo.co.jp>)
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

using Mosa.Runtime.CompilerFramework;
using Mosa.Runtime.Metadata;
using Mosa.Runtime.Vm;
using Mosa.Runtime.Metadata.Signatures;

namespace Mosa.Platforms.x86
{
    /// <summary>
    /// An x86 machine code emitter.
    /// </summary>
    public sealed class MachineCodeEmitter : ICodeEmitter, IDisposable
    {
        #region Types

        struct Patch
        {
            public Patch(int label, long position)
            {
                this.label = label;
                this.position = position;
            }

            public int label;
            public long position;
        }

        struct CodeDef
        {
            public CodeDef(Type dest, Type src, byte[] code, byte? regField)
            {
                this.dest = dest;
                this.src = src;
                this.code = code;
                this.regField = regField;
            }

            public Type dest;
            public Type src;
            public byte[] code;
            public byte? regField;
        }

        #endregion // Types

        #region Data members

        /// <summary>
        /// The stream used to write machine code bytes to.
        /// </summary>
        private Stream _codeStream;

        /// <summary>
        /// Patches we need to perform.
        /// </summary>
        private List<Patch> _patches = new List<Patch>();

        /// <summary>
        /// List of labels that were emitted.
        /// </summary>
        private Dictionary<int, long> _labels = new Dictionary<int, long>();

        /// <summary>
        /// List of literal patches we need to perform.
        /// </summary>
        private List<Patch> _literals = new List<Patch>();

        #endregion // Data members

        #region Construction

        /// <summary>
        /// Initializes a new instance of <see cref="MachineCodeEmitter"/>.
        /// </summary>
        /// <param name="codeStream">The stream the machine code is written to.</param>
        public MachineCodeEmitter(Stream codeStream)
        {
            Debug.Assert(null != codeStream, @"MachineCodeEmitter needs a code stream.");
            if (null == codeStream)
                throw new ArgumentNullException(@"codeStream");

            _codeStream = codeStream;
        }

        #endregion // Construction

        #region IDisposable Members

        /// <summary>
        /// Completes emitting the code of a method.
        /// </summary>
        public void Dispose()
        {
            // Flush the stream - we're not responsible for disposing it, as it belongs
            // to another component that gave it to the code generator.
            _codeStream.Flush();
        }

        #endregion // IDisposable Members

        #region ICodeEmitter Members

        void ICodeEmitter.Comment(string comment)
        {
            /*
             * The machine code emitter does not support comments. They're simply ignored.
             * 
             */
        }

        void ICodeEmitter.Label(int label)
        {
            /*
             * FIXME: Labels are used to resolve branches inside a procedure. Branches outside
             * of procedures are handled differently, t.b.d. 
             * 
             * So we store the current instruction offset with the label info to be able to 
             * resolve later backward jumps to this location.
             *
             * Additionally there may have been forward branches to this location, which have not
             * been resolved yet, so we need to scan these and resolve them to the current
             * instruction offset.
             * 
             */

            // Save the current position
            long currentPosition = _codeStream.Position;
            // Relative branch offset
            int relOffset;

            // Check if this label has forward references on it...
            _patches.RemoveAll(delegate(Patch p)
            {
                if (p.label == label)
                {
                    _codeStream.Position = p.position;
                    relOffset = (int)currentPosition - ((int)p.position + 4);
                    byte[] bytes = BitConverter.GetBytes(relOffset);
                    _codeStream.Write(bytes, 0, bytes.Length);
                    return true;
                }

                return false;
            });

            // Add this label to the label list, so we can resolve the jump later on
            _labels.Add(label, currentPosition);

            // Reset the position
            _codeStream.Position = currentPosition;

            // HACK: The machine code emitter needs to replace FP constants
            // with an EIP relative address, but these are not possible on x86,
            // so we store the EIP via a call in the right place on the stack
            //if (0 == label)
            //{
            //    // FIXME: This code doesn't need to be emitted if there are no
            //    // large constants used.
            //    _codeStream.WriteByte(0xE8);
            //    WriteImmediate(0);

            //    SigType i4 = new SigType(CilElementType.I4);

            //    RegisterOperand eax = new RegisterOperand(i4, GeneralPurposeRegister.EAX);
            //    Pop(eax);

            //    MemoryOperand mo = new MemoryOperand(i4, GeneralPurposeRegister.EBP, new IntPtr(-8));
            //    Mov(mo, eax);
            //}
        }

        void ICodeEmitter.Literal(int label, SigType type, object data)
        {
            // Save the current position
            long currentPosition = _codeStream.Position;
            // Relative branch offset
            int relOffset;
            // Flag, if we should really emit the literal (only if the literal is used!)
            bool emit = false;
            // Byte representation of the literal
            byte[] bytes;

            // Check if this label has forward references on it...
            emit = (0 != _literals.RemoveAll(delegate(Patch p)
            {
                if (p.label == label)
                {
                    _codeStream.Position = p.position;
                    // HACK: We can't do PIC right now
                    //relOffset = (int)currentPosition - ((int)p.position + 4);
                    bytes = BitConverter.GetBytes((int)currentPosition);
                    _codeStream.Write(bytes, 0, bytes.Length);
                    return true;
                }

                return false;
            }));

            if (true == emit)
            {
                _codeStream.Position = currentPosition;
                switch (type.Type)
                {
                    case CilElementType.I8:
                        bytes = BitConverter.GetBytes((long)data);
                        break;

                    case CilElementType.U8:
                        bytes = BitConverter.GetBytes((ulong)data);
                        break;

                    case CilElementType.R4:
                        bytes = BitConverter.GetBytes((double)data);
                        break;

                    case CilElementType.R8:
                        bytes = BitConverter.GetBytes((double)data);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                _codeStream.Write(bytes, 0, bytes.Length);
            }
        }

        void ICodeEmitter.And(Operand dest, Operand src)
        {
            Emit(dest, src, cd_and);
        }

        void ICodeEmitter.Add(Operand dest, Operand src)
        {
            Emit(dest, src, cd_add);
        }

        void ICodeEmitter.Call(RuntimeMethod method)
        {
            throw new NotImplementedException();
/*
            _codeStream.WriteByte(0xE8);

            // Calculate the relative call destination
            int relOffset = method.Address.ToInt32() - ((int)_codeStream.Position + 4);
            WriteImmediate(relOffset);
 */
        }

        void ICodeEmitter.Call(int label)
        {
            _codeStream.WriteByte(0xE8);
            EmitRelativeBranchTarget(label);
        }

        void ICodeEmitter.Cli()
        {
            _codeStream.WriteByte(0xFA);
        }

        void ICodeEmitter.Cmp(Operand op1, Operand op2)
        {
            if (op1.StackType == StackTypeCode.F || op2.StackType == StackTypeCode.F)
            {
                RegisterOperand rop;

                if (op1 is MemoryOperand)
                {
                    rop = new RegisterOperand(new SigType(CilElementType.R8), SSE2Register.XMM0);
                    ((ICodeEmitter)this).Mov(rop, op1);
                    Emit(rop, op2, cd_cmpsd);
                }
                else
                {
                    Emit(op1, op2, cd_cmpsd);
                }

                _codeStream.WriteByte(0x00);    // EQ
            }
            else
            {
                Emit(op1, op2, cd_cmp);
            }
        }

        void ICodeEmitter.Int3()
        {
            _codeStream.WriteByte(0xCC);
        }

        void ICodeEmitter.Ja(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x87 }, dest);
        }

        void ICodeEmitter.Jae(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x83 }, dest);
        }

        void ICodeEmitter.Jb(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x82 }, dest);
        }

        void ICodeEmitter.Jbe(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x86 }, dest);
        }

        void ICodeEmitter.Je(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x84 }, dest);
        }

        void ICodeEmitter.Jg(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x8F }, dest);
        }

        void ICodeEmitter.Jge(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x8D }, dest);
        }

        void ICodeEmitter.Jl(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x8C }, dest);
        }

        void ICodeEmitter.Jle(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x8E }, dest);
        }

        void ICodeEmitter.Jne(int dest)
        {
            EmitBranch(new byte[] { 0x0F, 0x85 }, dest);
        }

        void ICodeEmitter.Jmp(int dest)
        {
            EmitBranch(new byte[] { 0xE9 }, dest);
        }

        void ICodeEmitter.Nop()
        {
            _codeStream.WriteByte(0x90);
        }

        void ICodeEmitter.Mul(Operand dest, Operand src)
        {
            // Write the opcode byte
            Debug.Assert(dest is RegisterOperand && ((RegisterOperand)dest).Register is GeneralPurposeRegister && ((GeneralPurposeRegister)((RegisterOperand)dest).Register).RegisterCode == GeneralPurposeRegister.EAX.RegisterCode);
            Emit(dest, src, cd_mul);
        }

        void ICodeEmitter.SseAdd(Operand dest, Operand src)
        {
            // Write the opcode byte
            Debug.Assert(dest is RegisterOperand && ((RegisterOperand)dest).Register is GeneralPurposeRegister && ((GeneralPurposeRegister)((RegisterOperand)dest).Register).RegisterCode == GeneralPurposeRegister.EAX.RegisterCode);
            // FIXME: Insert correct opcode here
            Emit(dest, src, cd_addsd);
        }

        void ICodeEmitter.SseSub(Operand dest, Operand src)
        {
            // Write the opcode byte
            Debug.Assert(dest is RegisterOperand && ((RegisterOperand)dest).Register is GeneralPurposeRegister && ((GeneralPurposeRegister)((RegisterOperand)dest).Register).RegisterCode == GeneralPurposeRegister.EAX.RegisterCode);
            // FIXME: Insert correct opcode here
            Emit(dest, src, cd_subsd);
        }

        void ICodeEmitter.SseMul(Operand dest, Operand src)
        {
            // Write the opcode byte
            //Debug.Assert(dest is RegisterOperand && ((RegisterOperand)dest).Register is SSE2Register && ((SSE2Register)((RegisterOperand)dest).Register).RegisterCode == SSE2Register.XMM0.RegisterCode);
            // HACK: Until we get a real register allocator (EAX == XMM0)
            Debug.Assert(dest is RegisterOperand && ((RegisterOperand)dest).Register is GeneralPurposeRegister && ((GeneralPurposeRegister)((RegisterOperand)dest).Register).RegisterCode == GeneralPurposeRegister.EAX.RegisterCode);
            // FIXME: Insert correct opcode here
            Emit(dest, src, cd_mulsd);
        }

        void ICodeEmitter.SseDiv(Operand dest, Operand src)
        {
            // Write the opcode byte
            Debug.Assert(dest is RegisterOperand && ((RegisterOperand)dest).Register is GeneralPurposeRegister && ((GeneralPurposeRegister)((RegisterOperand)dest).Register).RegisterCode == GeneralPurposeRegister.EAX.RegisterCode);
            // FIXME: Insert correct opcode here
            Emit(dest, src, cd_divsd);
        }

        void ICodeEmitter.Shl(Operand dest, Operand src)
        {
            // Write the opcode byte
            Debug.Assert(dest is RegisterOperand && (src is ConstantOperand /*|| src is MemoryOperand*/));
            // FIXME: Make sure the constant is emitted as a single-byte opcode
            Emit(dest, src, cd_shl);
        }

        void ICodeEmitter.Shr(Operand dest, Operand src)
        {
            // Write the opcode byte
            Debug.Assert(dest is RegisterOperand && (src is ConstantOperand));
            Emit(dest, src, cd_shr);
        }

        void ICodeEmitter.Div(Operand dest, Operand src)
        {
            // Write the opcode byte
            Debug.Assert(dest is RegisterOperand && ((RegisterOperand)dest).Register is GeneralPurposeRegister && ((GeneralPurposeRegister)((RegisterOperand)dest).Register).RegisterCode == GeneralPurposeRegister.EAX.RegisterCode);
            Emit(dest, src, cd_div);
        }

        void ICodeEmitter.Mov(Operand dest, Operand src)
        {
            if (dest.StackType != StackTypeCode.F && src.StackType != StackTypeCode.F)
            {
                Emit(dest, src, cd_mov);
            }
            else
            {
                Emit(dest, src, cd_movsd);
            }
        }

        void ICodeEmitter.Pop(Operand operand)
        {
            if (operand is RegisterOperand)
            {
                RegisterOperand ro = (RegisterOperand)operand;
                _codeStream.WriteByte((byte)(0x58 + ro.Register.RegisterCode));
            }
            else
            {
                Emit(new byte[] { 0x8F }, 0, operand, null);
            }
        }

        void ICodeEmitter.Push(Operand operand)
        {
            if (operand is ConstantOperand)
            {
                _codeStream.WriteByte(0x68);
                EmitImmediate(operand);
            }
            else if (operand is RegisterOperand)
            {
                RegisterOperand ro = (RegisterOperand)operand;
                _codeStream.WriteByte((byte)(0x50 + ro.Register.RegisterCode));
            }
            else
            {
                Emit(new byte[] { 0xFF }, 6, operand, null);
            }
        }

        void ICodeEmitter.Ret()
        {
            _codeStream.WriteByte(0xC3);
        }

        void ICodeEmitter.Sti()
        {
            _codeStream.WriteByte(0xFB);
        }

        void ICodeEmitter.Sub(Operand dest, Operand src)
        {
            Emit(dest, src, cd_sub);
        }

        #endregion // ICodeEmitter Members

        #region Code Definition Tables

        private static readonly CodeDef[] cd_add = new CodeDef[] {
            new CodeDef(typeof(Operand),            typeof(ConstantOperand),    new byte[] { 0x81 }, 0),
            new CodeDef(typeof(RegisterOperand),    typeof(Operand),            new byte[] { 0x03 }, null)
        };

        private static readonly CodeDef[] cd_and = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand), typeof(ConstantOperand),   new byte[] { 0x81 }, 4),
            new CodeDef(typeof(RegisterOperand), typeof(MemoryOperand),     new byte[] { 0x23 }, null),
            new CodeDef(typeof(RegisterOperand), typeof(RegisterOperand),   new byte[] { 0x23 }, null),
            new CodeDef(typeof(MemoryOperand),   typeof(RegisterOperand),   new byte[] { 0x21 }, null),
        };

        private static readonly CodeDef[] cd_cmpsd = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(Operand),            new byte[] { 0xF2, 0x0F, 0xC2 }, null),
        };

        private static readonly CodeDef[] cd_cmp = new CodeDef[] {
            new CodeDef(typeof(MemoryOperand),      typeof(RegisterOperand),    new byte[] { 0x39 }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),      new byte[] { 0x3B }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),      new byte[] { 0x3B }, null),
            new CodeDef(typeof(MemoryOperand),      typeof(ConstantOperand),    new byte[] { 0x81 }, 7),
            new CodeDef(typeof(RegisterOperand),    typeof(ConstantOperand),    new byte[] { 0x81 }, 7),
        };

        private static readonly CodeDef[] cd_mul = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(Operand),            new byte[] { 0xF7 }, 4),
        };

        private static readonly CodeDef[] cd_addsd = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),    new byte[] { 0xF2, 0x0F, 0x58 }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),      new byte[] { 0xF2, 0x0F, 0x58 }, null),
        };

        private static readonly CodeDef[] cd_subsd = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),    new byte[] { 0xF2, 0x0F, 0x5C }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),      new byte[] { 0xF2, 0x0F, 0x5C }, null)
        };

        private static readonly CodeDef[] cd_mulsd = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),    new byte[] { 0xF2, 0x0F, 0x59 }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),      new byte[] { 0xF2, 0x0F, 0x59 }, null),
        };

        private static readonly CodeDef[] cd_divsd = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),    new byte[] { 0xF2, 0x0F, 0x5E }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),      new byte[] { 0xF2, 0x0F, 0x5E }, null),
        };

        private static readonly CodeDef[] cd_shl = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(ConstantOperand),    new byte[] { 0xC1 }, 4),
        };

        private static readonly CodeDef[] cd_shr = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(ConstantOperand),    new byte[] { 0xC1 }, 5),
        };

        private static readonly CodeDef[] cd_div = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),    new byte[] { 0xF7 }, 7),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),      new byte[] { 0xF7 }, 7),
        };

        private static readonly CodeDef[] cd_mov = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(ConstantOperand),    new byte[] { 0xC7 }, 0),
            new CodeDef(typeof(MemoryOperand),      typeof(ConstantOperand),    new byte[] { 0xC7 }, 0),
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),    new byte[] { 0x8B }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),      new byte[] { 0x8B }, null),
            new CodeDef(typeof(MemoryOperand),      typeof(RegisterOperand),    new byte[] { 0x89 }, null),
        };

        private static readonly CodeDef[] cd_movsd = new CodeDef[] {
            new CodeDef(typeof(RegisterOperand),    typeof(LabelOperand),       new byte[] { 0xF2, 0x0F, 0x10 }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(MemoryOperand),       new byte[] { 0xF2, 0x0F, 0x10 }, null),
            new CodeDef(typeof(RegisterOperand),    typeof(RegisterOperand),       new byte[] { 0xF2, 0x0F, 0x10 }, null),
            new CodeDef(typeof(MemoryOperand),      typeof(RegisterOperand),       new byte[] { 0xF2, 0x0F, 0x11 }, null),
        };

        private static readonly CodeDef[] cd_sub = new CodeDef[] {
            new CodeDef(typeof(Operand),            typeof(ConstantOperand),    new byte[] { 0x81 }, 5),
            new CodeDef(typeof(RegisterOperand),    typeof(Operand),            new byte[] { 0x2B }, 0),
            new CodeDef(typeof(MemoryOperand),      typeof(RegisterOperand),    new byte[] { 0x29 }, 0),
        };

        #endregion // Code Definition Tables

        #region Code Generation

        /// <summary>
        /// Walks the code definition array for a matching combination and emits the corresponding code.
        /// </summary>
        /// <param name="dest">The destination operand.</param>
        /// <param name="src">The source operand.</param>
        /// <param name="codeDef">The code definition array.</param>
        private void Emit(Operand dest, Operand src, CodeDef[] codeDef)
        {
            foreach (CodeDef cd in codeDef)
            {
                if (true == cd.dest.IsInstanceOfType(dest) && true == cd.src.IsInstanceOfType(src))
                {
                    Emit(cd.code, cd.regField, dest, src);
                    return;
                }
            }

            // If this is reached, the operand combination could not be emitted as it is
            // not specified in the code definition table
            throw new NotSupportedException();
        }

        /// <summary>
        /// Emits relative branch code.
        /// </summary>
        /// <param name="code">The branch instruction code.</param>
        /// <param name="dest">The destination label.</param>
        private void EmitBranch(byte[] code, int dest)
        {
            _codeStream.Write(code, 0, code.Length);
            EmitRelativeBranchTarget(dest);
        }

        /// <summary>
        /// Emits the given code.
        /// </summary>
        /// <param name="code">The opcode bytes.</param>
        /// <param name="regField">The modR/M regfield.</param>
        /// <param name="dest">The destination operand.</param>
        /// <param name="src">The source operand.</param>
        private void Emit(byte[] code, byte? regField, Operand dest, Operand src)
        {
            byte? sib = null, modRM = null;
            IntPtr? displacement = null;

            // Write the opcode
            _codeStream.Write(code, 0, code.Length);

            // Write the mod R/M byte
            modRM = CalculateModRM(regField, dest, src, out sib, out displacement);
            if (null != modRM)
            {
                _codeStream.WriteByte(modRM.Value);
                if (true == sib.HasValue)
                {
                    _codeStream.WriteByte(sib.Value);
                }
            }

            // Add displacement to the code
            if (null != displacement)
            {
                LabelOperand label = src as LabelOperand;
                if (null != label)
                {
                    // HACK: PIC and FP won't work for now, have to really fix this for moveable 
                    // jitted code though
                    displacement = IntPtr.Zero;
                    _literals.Add(new Patch(label.Label, _codeStream.Position));
                }

                byte[] disp = BitConverter.GetBytes(displacement.Value.ToInt32());
                _codeStream.Write(disp, 0, disp.Length);
            }

            // Add immediate bytes
            if (dest is ConstantOperand)
                EmitImmediate(dest);
            if (src is ConstantOperand)
                EmitImmediate(src);
        }

        private void EmitRelativeBranchTarget(int label)
        {
            // The relative offset of the label
            int relOffset = 0;
            // The position in the code stream of the label
            long position;

            // Did we see the label?
            if (true == _labels.TryGetValue(label, out position))
            {
                // Yes, calculate the relative offset
                relOffset = (int)position - ((int)_codeStream.Position + 4);
            }
            else
            {
                // Forward jump, we can't resolve yet - store a patch
                _patches.Add(new Patch(label, _codeStream.Position));
            }

            // Emit the relative jump offset (zero if we don't know it yet!)
            byte[] bytes = BitConverter.GetBytes(relOffset);
            _codeStream.Write(bytes, 0, bytes.Length);
        }


        /// <summary>
        /// Emits an immediate operand.
        /// </summary>
        /// <param name="op">The immediate operand to emit.</param>
        private void EmitImmediate(Operand op)
        {
            byte[] imm = null;
            if (op is LocalVariableOperand)
            {
                // Add the displacement
                StackOperand so = (StackOperand)op;
                imm = BitConverter.GetBytes(so.Offset.ToInt32());
            }
            else if (op is LabelOperand)
            {
                _literals.Add(new Patch((op as LabelOperand).Label, _codeStream.Position));
                imm = new byte[4];
            }
            else if (op is MemoryOperand)
            {
                // Add the displacement
                MemoryOperand mo = (MemoryOperand)op;
                imm = BitConverter.GetBytes(mo.Offset.ToInt32());
            }
            else if (op is ConstantOperand)
            {
                // Add the immediate
                ConstantOperand co = (ConstantOperand)op;
                switch (op.Type.Type)
                {
                    case CilElementType.I: 
                        imm = BitConverter.GetBytes(Convert.ToInt32(co.Value));
                        break;

                    case CilElementType.I1: goto case CilElementType.I;
                    case CilElementType.I2: goto case CilElementType.I;
                    case CilElementType.I4: goto case CilElementType.I;

                    case CilElementType.U1: goto case CilElementType.I;
                    case CilElementType.U2: goto case CilElementType.I;
                    case CilElementType.U4: goto case CilElementType.I;

                    case CilElementType.I8: goto case CilElementType.R8;
                    case CilElementType.U8: goto case CilElementType.R8;
                    case CilElementType.R4: goto case CilElementType.R8;
                    case CilElementType.R8: goto default;
                    default:
                        throw new NotSupportedException();
                        break;
                }
            }
            else if (op is RegisterOperand)
            {
                // Nothing to do...
            }
            else
            {
                throw new NotImplementedException();
            }

            // Emit the immediate constant to the code
            if (null != imm)
                _codeStream.Write(imm, 0, imm.Length);
        }

        /// <summary>
        /// Calculates the value of the modR/M byte and SIB bytes.
        /// </summary>
        /// <param name="regField">The modR/M regfield value.</param>
        /// <param name="dest">The destination operand.</param>
        /// <param name="src">The source operand.</param>
        /// <param name="sib">A potential SIB byte to emit.</param>
        /// <param name="displacement">An immediate displacement to emit.</param>
        /// <returns>The value of the modR/M byte.</returns>
        private byte? CalculateModRM(byte? regField, Operand dest, Operand src, out byte? sib, out IntPtr? displacement)
        {
            MemoryOperand memAddrOp = null;

            displacement = null;
            sib = null;

            if (null != dest)
            {
                byte modRM = 0;
                if (dest is RegisterOperand && src is RegisterOperand)
                {
                    modRM |= 3 << 6;
                    modRM |= (byte)(((RegisterOperand)dest).Register.RegisterCode << 3);
                    modRM |= (byte)((RegisterOperand)src).Register.RegisterCode;
                }
                else
                {
                    if (null != regField)
                    {
                        modRM |= (byte)(regField.Value << 3);
                    }

                    RegisterOperand registerOp = null;
                    if (dest is MemoryOperand)
                    {
                        memAddrOp = (MemoryOperand)dest;
                        registerOp = src as RegisterOperand;

                        displacement = memAddrOp.Offset;
                        if (null != memAddrOp.Base && displacement != IntPtr.Zero)
                        {
                            modRM |= (byte)((2 << 6) | memAddrOp.Base.RegisterCode);
                        }
                        else
                        {
                            modRM |= 5;
                        }

/* FIXME: Fix SIB byte support
                        if (memAddrOp.Base != null && (memAddrOp.IndexRegister != null && (byte)memAddrOp.Scale > 0))
                        {
                            SIBByte = BuildSIBByte(memAddrOp);
                            modRM |= 3;
                        }
                        else 
 */
                        if (null != registerOp)
                            modRM |= (byte)(registerOp.Register.RegisterCode << 3);
                    }
                    else if (dest is RegisterOperand)
                    {
                        registerOp = (RegisterOperand)dest;
                        memAddrOp = src as MemoryOperand;

                        if (null == memAddrOp)
                        {
                            modRM |= 0xC0;
                        }
                        else
                        {
                            displacement = memAddrOp.Offset;
                            if (null != memAddrOp.Base && displacement != IntPtr.Zero)
                            {
                                modRM |= (byte)((2 << 6) | memAddrOp.Base.RegisterCode);
                            }
                            else
                            {
                                modRM |= 5;
                            }

/* FIXME: Fix SIB support
                            if (memAddrOp.Base != null && (memAddrOp.IndexRegister != null && (byte)memAddrOp.Scale > 0))
                            {
                                SIBByte = BuildSIBByte(memAddrOp);
                                modRM |= 3;
                            }
*/
                        }

                        if (null != registerOp)
                            modRM |= (byte)registerOp.Register.RegisterCode;
                    }

                }

                return modRM;
            }

            return null;
        }

        #endregion // Code Generation
    }
}