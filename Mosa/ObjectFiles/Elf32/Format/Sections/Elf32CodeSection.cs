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
using System.Text;
using Mosa.Runtime.Vm;

namespace Mosa.ObjectFiles.Elf32.Format.Sections
{
    /// <summary>
    /// This section holds the "text" or executable instructions, of a program.
    /// Also referred to as ".text" on page 30 according to the specification in
    /// the TIS (Tool Interface Standard) ELF (Executable and Linking Format)
    /// Specification, 1-4
    /// </summary>
    class Elf32CodeSection : Elf32SymbolDefinitionSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Elf32CodeSection"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="flags">The flags.</param>
        public Elf32CodeSection(Elf32File file, string name, Elf32SectionType type, Elf32SectionFlags flags)
            : base(file, name, type, flags)
        {
        }
    }
}
