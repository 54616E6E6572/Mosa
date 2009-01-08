﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Simon "Kintaro" Wollwage (<mailto:kintaro@mosa-project.org>)
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.Linker.Elf32.Sections
{
    /// <summary>
    /// 
    /// </summary>
    public class Elf32Section : Mosa.Runtime.Linker.LinkerSection
    {
        /// <summary>
        /// 
        /// </summary>
        protected Elf32SectionHeader header = new Elf32SectionHeader();
        /// <summary>
        /// 
        /// </summary>
        protected System.IO.MemoryStream sectionStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Elf32Section"/> class.
        /// </summary>
        /// <param name="kind">The kind of the section.</param>
        /// <param name="name">The name.</param>
        /// <param name="virtualAddress">The virtualAddress.</param>
        public Elf32Section(Mosa.Runtime.Linker.SectionKind kind, string name, IntPtr virtualAddress)
            : base(kind, name, virtualAddress)
        {
            header = new Elf32SectionHeader();
            header.Name = Elf32StringTableSection.AddString(name);
            sectionStream = new System.IO.MemoryStream();
        }

        /// <summary>
        /// Gets the length of the section in bytes.
        /// </summary>
        /// <value>The length of the section in bytes.</value>
        public override long Length
        {
            get 
            {
                return this.sectionStream.Length;
            }
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public Elf32SectionHeader Header
        {
            get
            {
                return header;
            }
        }

        /// <summary>
        /// Allocates the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="alignment">The alignment.</param>
        /// <returns></returns>
        public System.IO.Stream Allocate(int size, int alignment)
        {
            // Do we need to ensure a specific alignment?
            if (alignment > 1)
                InsertPadding(alignment);

            return this.sectionStream;
        }

        /// <summary>
        /// Writes the specified fs.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void Write(System.IO.BinaryWriter writer)
        {
            Header.Offset = (uint)writer.BaseStream.Position;
            this.sectionStream.WriteTo(writer.BaseStream);
        }

        /// <summary>
        /// Writes the header.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void WriteHeader(System.IO.BinaryWriter writer)
        {
            Header.Size = (uint)Length;
            Header.Write(writer);
        }

        /// <summary>
        /// Patches the section at the given offset with the specified value.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="linkType">Type of the link.</param>
        /// <param name="value">The value.</param>
        public void ApplyPatch(long offset, LinkType linkType, long value)
        {
            long pos = this.sectionStream.Position;
            this.sectionStream.Position = offset;

            // Apply the patch
            switch (linkType & LinkType.SizeMask)
            {
                case LinkType.I1:
                    this.sectionStream.WriteByte((byte)value);
                    break;

                case LinkType.I2:
                    this.sectionStream.Write(BitConverter.GetBytes((ushort)value), 0, 2);
                    break;

                case LinkType.I4:
                    this.sectionStream.Write(BitConverter.GetBytes((uint)value), 0, 4);
                    break;

                case LinkType.I8:
                    this.sectionStream.Write(BitConverter.GetBytes(value), 0, 8);
                    break;
            }

            this.sectionStream.Position = pos;
        }

        #region Internals

        /// <summary>
        /// Pads the stream with zeros until the specific alignment is reached.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        private void InsertPadding(int alignment)
        {
            long address = this.VirtualAddress.ToInt64() + this.sectionStream.Length;
            int pad = (int)(alignment - (address % alignment));
            this.sectionStream.Write(new byte[pad], 0, pad);
        }

        #endregion // Internals
    }
}
