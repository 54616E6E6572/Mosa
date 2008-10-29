﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Alex Lyman (<mailto:mail.alex.lyman@gmail.com>)
 */

using System;
using System.Collections.Generic;
using System.IO;

using Mosa.Runtime.Linker;

namespace Mosa.ObjectFiles.Elf32.Format.Sections
{
    class Elf32RelocationSection : Elf32ProgbitsSection
    {
        public Elf32RelocationSection(Elf32File file, string name)
            : base(file, name, Elf32SectionType.SHT_RELA, Elf32SectionFlags.SHF_NONE)
        {
        }

        protected override int Link { get { return File.Sections.IndexOf(SymbolTable); } }
        protected override int Info { get { return File.Sections.IndexOf(TargetSection); } }

        public override int EntitySize { get { return 12; } }

        public Elf32SymbolTableSection SymbolTable { get; set; }
        public Elf32Section TargetSection { get; set; }

        struct RelocA
        {
            public int Offset;
            public LinkType LinkType;
            public Elf32Symbol Target;
            public int Addend;
        }

        List<RelocA> relocs = new List<RelocA>();

        public void Add(LinkType linkType, int offset, int relative, Elf32Symbol targetSym)
        {
            relocs.Add(
                new RelocA
                {
                    LinkType = linkType,
                    Offset = offset,
                    Addend = relative,
                    Target = targetSym
                }
            );
        }

        protected override void WriteDataImpl(BinaryWriter writer)
        {
            byte relative32Type, absolute32Type;
            switch (File.MachineKind)
            {
                case Elf32MachineKind.I386:
                    absolute32Type = 1;
                    relative32Type = 2;
                    break;
                default:
                    throw new NotSupportedException();
            }
            foreach (RelocA r in relocs)
            {
                writer.Write(r.Offset);
                int symIndex = SymbolTable.Symbols.IndexOf(r.Target);
                int type;
                switch (r.LinkType)
                {
                    case LinkType.I4 | LinkType.RelativeOffset:
                        type = relative32Type;
                        break;
                    case LinkType.I4 | LinkType.AbsoluteAddress:
                        type = absolute32Type;
                        break;
                    default: throw new NotSupportedException();
                }
                writer.Write((int)((symIndex << 8) | type));
                writer.Write(r.Addend);
            }
        }
    }
}
