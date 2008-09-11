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
using System.Diagnostics;

namespace Mosa.Runtime.Metadata.Signatures
{
    /// <summary>
    /// Represents a type signature
    /// </summary>
    public class SigType
    {
        /// <summary>
        /// Holds the CIL element type of the signature type.
        /// </summary>
        private CilElementType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="SigType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public SigType(CilElementType type)
        {
            _type = type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public CilElementType Type { get { return _type; } }

        /// <summary>
        /// Gibt einen <see cref="T:System.String"/> zur�ck, der den aktuellen <see cref="T:System.Object"/> darstellt.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.String"/>, der den aktuellen <see cref="T:System.Object"/> darstellt.
        /// </returns>
        public override string ToString()
        {
            return _type.ToString();
        }

        /// <summary>
        /// Parses the type signature.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static SigType ParseTypeSignature(byte[] buffer, ref int index)
        {
            SigType result;
            CilElementType type = (CilElementType)buffer[index++];
            switch (type)
            {
                case CilElementType.Void: result = new SigType(type); break;
                case CilElementType.Boolean: goto case CilElementType.Void;
                case CilElementType.Char: goto case CilElementType.Void;
                case CilElementType.I1: goto case CilElementType.Void;
                case CilElementType.U1: goto case CilElementType.Void;
                case CilElementType.I2: goto case CilElementType.Void;
                case CilElementType.U2: goto case CilElementType.Void;
                case CilElementType.I4: goto case CilElementType.Void;
                case CilElementType.U4: goto case CilElementType.Void;
                case CilElementType.I8: goto case CilElementType.Void;
                case CilElementType.U8: goto case CilElementType.Void;
                case CilElementType.R4: goto case CilElementType.Void;
                case CilElementType.R8: goto case CilElementType.Void;
                case CilElementType.String: goto case CilElementType.Void;
                case CilElementType.Object: goto case CilElementType.Void;
                case CilElementType.I: goto case CilElementType.Void;
                case CilElementType.U: goto case CilElementType.Void;
                case CilElementType.TypedByRef: goto case CilElementType.Void;

                case CilElementType.Array:
                    result = ParseArraySignature(buffer, ref index);
                    break;

                case CilElementType.Class:
                    result = ParseClassSignature(buffer, ref index);
                    break;

                case CilElementType.FunctionPtr:
                    result = ParseFunctionPointer(buffer, ref index);
                    break;

                case CilElementType.GenericInst:
                    result = ParseGenericInstance(buffer, ref index);
                    break;

                case CilElementType.MVar:
                    result = ParseMVar(buffer, ref index);
                    break;

                case CilElementType.Ptr:
                    result = ParsePointer(buffer, ref index);
                    break;

                case CilElementType.SZArray:
                    result = ParseSZArraySignature(buffer, ref index);
                    break;

                case CilElementType.ValueType:
                    result = ParseValueType(buffer, ref index);
                    break;

                case CilElementType.Var:
                    result = ParseVar(buffer, ref index);
                    break;

                case CilElementType.ByRef:
                    result = ParseReference(buffer, ref index);
                    break;

                default:
                    throw new NotSupportedException(@"Unsupported CIL element type: " + type);
            }

            return result;
        }

        /// <summary>
        /// Parses the var.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseVar(byte[] buffer, ref int index)
        {
            int varIdx = Utilities.ReadCompressedInt32(buffer, ref index);
            return new VarSigType(varIdx);
        }

        /// <summary>
        /// Parses the type of the value.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseValueType(byte[] buffer, ref int index)
        {
            TokenTypes token = ReadTypeDefOrRefEncoded(buffer, ref index);
            return new ValueTypeSigType(token);
        }

        /// <summary>
        /// Parses the pointer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParsePointer(byte[] buffer, ref int index)
        {
            CustomMod[] mods = CustomMod.ParseCustomMods(buffer, ref index);
            SigType type = ParseTypeSignature(buffer, ref index);
            return new PtrSigType(mods, type);
        }

        /// <summary>
        /// Parses the reference.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseReference(byte[] buffer, ref int index)
        {
            SigType type = ParseTypeSignature(buffer, ref index);
            return new RefSigType(type);
        }

        /// <summary>
        /// Parses the M var.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseMVar(byte[] buffer, ref int index)
        {
            int varIdx = Utilities.ReadCompressedInt32(buffer, ref index);
            return new MVarSigType(varIdx);
        }

        /// <summary>
        /// Parses the generic instance.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseGenericInstance(byte[] buffer, ref int index)
        {
            SigType originalType;
            CilElementType type = (CilElementType)buffer[index++];
            switch (type)
            {
                case CilElementType.Class:
                    originalType = ParseClassSignature(buffer, ref index);
                    break;

                case CilElementType.ValueType:
                    originalType = ParseValueType(buffer, ref index);
                    break;

                default:
                    throw new InvalidOperationException(@"Invalid signature type.");
            }

            int genArgCount = Utilities.ReadCompressedInt32(buffer, ref index);
            SigType[] genArgs = new SigType[genArgCount];
            for (int i = 0; i < genArgCount; i++)
            {
                genArgs[i] = ParseTypeSignature(buffer, ref index);
            }

            return new GenericInstSigType(originalType, genArgs);
        }

        /// <summary>
        /// Parses the function pointer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseFunctionPointer(byte[] buffer, ref int index)
        {
            TokenTypes token = (TokenTypes)Utilities.ReadCompressedInt32(buffer, ref index);
            return new FnptrSigType(token);
        }

        /// <summary>
        /// Parses the class signature.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseClassSignature(byte[] buffer, ref int index)
        {
            TokenTypes token = ReadTypeDefOrRefEncoded(buffer, ref index);
            return new ClassSigType(token);
        }

        /// <summary>
        /// Parses the array signature.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseArraySignature(byte[] buffer, ref int index)
        {
            SigType elementType = ParseTypeSignature(buffer, ref index);
            int rank, count;
            int[] sizes, lowerBounds;

            rank = Utilities.ReadCompressedInt32(buffer, ref index);
            count = Utilities.ReadCompressedInt32(buffer, ref index);
            sizes = new int[count];
            for (int i = 0; i < count; i++)
                sizes[i] = Utilities.ReadCompressedInt32(buffer, ref index);

            count = Utilities.ReadCompressedInt32(buffer, ref index);
            lowerBounds = new int[count];
            for (int i = 0; i < count; i++)
                lowerBounds[i] = Utilities.ReadCompressedInt32(buffer, ref index);

            return new ArraySigType(elementType, rank, sizes, lowerBounds);
        }

        /// <summary>
        /// Parses the SZ array signature.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static SigType ParseSZArraySignature(byte[] buffer, ref int index)
        {
            CustomMod[] customMods = CustomMod.ParseCustomMods(buffer, ref index);
            SigType elementType = ParseTypeSignature(buffer, ref index);
            return new SZArraySigType(customMods, elementType);
        }

        /// <summary>
        /// 
        /// </summary>
        private static readonly TokenTypes[] _typeDefOrRefEncodedTables = new TokenTypes[] { TokenTypes.TypeDef, TokenTypes.TypeRef, TokenTypes.TypeSpec };

        /// <summary>
        /// Reads the type def or ref encoded.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public static TokenTypes ReadTypeDefOrRefEncoded(byte[] buffer, ref int index)
        {
            int value = Utilities.ReadCompressedInt32(buffer, ref index);
            Debug.Assert(0 != (value & 0xFFFFFFFC), @"Invalid TypeDefOrRefEncoded index value.");
            TokenTypes token = (TokenTypes)((value >> 2) | (int)_typeDefOrRefEncodedTables[value & 0x03]);
            return token;
        }

        /// <summary>
        /// Matcheses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Matches(SigType other)
        {
            if (object.ReferenceEquals(this, other))
                return true;
            // TODO: Check to make sure a SigType matches
            if (other.Type != this.Type)
                return false;
            switch (this.Type)
            {
                case CilElementType.Void:
                case CilElementType.Boolean:
                case CilElementType.Char:
                case CilElementType.I1:
                case CilElementType.U1:
                case CilElementType.I2:
                case CilElementType.U2:
                case CilElementType.I4:
                case CilElementType.U4:
                case CilElementType.I8:
                case CilElementType.U8:
                case CilElementType.R4:
                case CilElementType.R8:
                case CilElementType.String:
                case CilElementType.I:
                case CilElementType.U:
                case CilElementType.Object:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
