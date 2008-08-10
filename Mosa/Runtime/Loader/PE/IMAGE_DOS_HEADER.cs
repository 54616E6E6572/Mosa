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
using System.IO;

namespace Mosa.Runtime.Loader.PE {
	/// <summary>
	/// The DOS header structure of a portable executable file.
	/// </summary>
	struct IMAGE_DOS_HEADER {
		#region Constants

		/// <summary>
		/// MZ magic dos header value.
		/// </summary>
		private const ushort DOS_HEADER_MAGIC = 0x5A4D;

		#endregion // Constants

		#region Data members

		/// <summary>
		/// Magic number
		/// </summary>
		public ushort e_magic;
            
		/// <summary>
		/// Bytes on last page of file.
		/// </summary>
		public ushort e_cblp;

		/// <summary>
		/// Pages in file
		/// </summary>
		public ushort e_cp;

		/// <summary>
		/// Relocations
		/// </summary>
		public ushort e_crlc;

		/// <summary>
		/// Size of header in paragraphs
		/// </summary>
		public ushort e_cparhdr;

		/// <summary>
		/// Minimum extra paragraphs needed
		/// </summary>
		public ushort e_minalloc;

		/// <summary>
		/// Maximum extra paragraphs needed
		/// </summary>
		public ushort e_maxalloc;

		/// <summary>
		/// Initial (relative) SS value
		/// </summary>
		public ushort e_ss;

		/// <summary>
		/// Initial SP value
		/// </summary>
		public ushort e_sp;

		/// <summary>
		/// Checksum
		/// </summary>
		public ushort e_csum;

		/// <summary>
		/// Initial IP value
		/// </summary>
		public ushort e_ip;

		/// <summary>
		/// Initial (relative) CS value
		/// </summary>
		public ushort e_cs;

		/// <summary>
		/// File address of relocation table
		/// </summary>
		public ushort e_lfarlc;

		/// <summary>
		/// Overlay number
		/// </summary>
		public ushort e_ovno;

		/// <summary>
		/// Reserved words
		/// </summary>
		public ushort e_res00;
		public ushort e_res01;
		public ushort e_res02;
		public ushort e_res03;

		/// <summary>
		/// OEM identifier (for e_oeminfo)
		/// </summary>
		public ushort e_oemid;

		/// <summary>
		/// OEM information; e_oemid specific
		/// </summary>
		public ushort e_oeminfo;

		/// <summary>
		/// Reserved public words
		/// </summary>
		public ushort e_res20;
		public ushort e_res21;
		public ushort e_res22;
		public ushort e_res23;
		public ushort e_res24;
		public ushort e_res25;
		public ushort e_res26;
		public ushort e_res27;
		public ushort e_res28;
		public ushort e_res29;

		/// <summary>
		/// File address of new exe header
		/// </summary>
		public uint e_lfanew;

		#endregion // Data members

		#region Methods

		/// <summary>
		/// Loads and validates the DOS header.
		/// </summary>
		public void Load(BinaryReader reader)
		{
			e_magic = reader.ReadUInt16();
			e_cblp = reader.ReadUInt16();
			e_cp = reader.ReadUInt16();
			e_crlc = reader.ReadUInt16();
			e_cparhdr = reader.ReadUInt16();
			e_minalloc = reader.ReadUInt16();
			e_maxalloc = reader.ReadUInt16();
			e_ss = reader.ReadUInt16();
			e_sp = reader.ReadUInt16();
			e_csum = reader.ReadUInt16();
			e_ip = reader.ReadUInt16();
			e_cs = reader.ReadUInt16();
			e_lfarlc = reader.ReadUInt16();
			e_ovno = reader.ReadUInt16();
			e_res00 = reader.ReadUInt16();
			e_res01 = reader.ReadUInt16();
			e_res02 = reader.ReadUInt16();
			e_res03 = reader.ReadUInt16();
			e_oemid = reader.ReadUInt16();
			e_oeminfo = reader.ReadUInt16();
			e_res20 = reader.ReadUInt16();
			e_res21 = reader.ReadUInt16();
			e_res22 = reader.ReadUInt16();
			e_res23 = reader.ReadUInt16();
			e_res24 = reader.ReadUInt16();
			e_res25 = reader.ReadUInt16();
			e_res26 = reader.ReadUInt16();
			e_res27 = reader.ReadUInt16();
			e_res28 = reader.ReadUInt16();
			e_res29 = reader.ReadUInt16();
			e_lfanew = reader.ReadUInt32();

			if (DOS_HEADER_MAGIC != e_magic)
				throw new BadImageFormatException();
		}

		#endregion // Methods
	}
}
