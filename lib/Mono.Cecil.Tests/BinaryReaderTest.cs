/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 * Temporary test, should be injected with AspectDNG
 *
 *****************************************************************************/

namespace Mono.Cecil.Tests {

    using System;

    using Mono.Cecil.Binary;

    using NUnit.Framework;

    [TestFixture]
    public sealed class BinaryReaderTest : AbstractReaderTest {

        [Test]
        public void DOSHeaderTest ()
        {
            Assert.AreEqual (new byte [60] {
                                 0x4d, 0x5a, 0x90, 0x00, 0x03, 0x00, 0x00,
                                 0x00, 0x04, 0x00, 0x00, 0x00, 0xff, 0xff,
                                 0x00, 0x00, 0xb8, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00 }, this.Image.DOSHeader.Start);
            Assert.AreEqual (128, this.Image.DOSHeader.Lfanew);
            Assert.AreEqual (new byte [64] {
                                 0x0e, 0x1f, 0xba, 0x0e, 0x00, 0xb4, 0x09,
                                 0xcd, 0x21, 0xb8, 0x01, 0x4c, 0xcd, 0x21,
                                 0x54, 0x68, 0x69, 0x73, 0x20, 0x70, 0x72,
                                 0x6f, 0x67, 0x72, 0x61, 0x6d, 0x20, 0x63,
                                 0x61, 0x6e, 0x6e, 0x6f, 0x74, 0x20, 0x62,
                                 0x65, 0x20, 0x72, 0x75, 0x6e, 0x20, 0x69,
                                 0x6e, 0x20, 0x44, 0x4f, 0x53, 0x20, 0x6d,
                                 0x6f, 0x64, 0x65, 0x2e, 0x0d, 0x0d, 0x0a,
                                 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                 0x00 }, this.Image.DOSHeader.End);
        }

        [Test]
        public void PEFileHeaderTest ()
        {
            Assert.AreEqual (0x14c, this.Image.PEFileHeader.Machine);
            Assert.AreEqual (0, this.Image.PEFileHeader.PointerToSymbolTable);
            Assert.AreEqual (0, this.Image.PEFileHeader.NumberOfSymbols);
        }

        [Test]
        public void POHStandardsFieldsTest ()
        {
            Assert.AreEqual (0x10b, this.Image.PEOptionalHeader.StandardFields.Magic);
            Assert.AreEqual (6, this.Image.PEOptionalHeader.StandardFields.LMajor);
            Assert.AreEqual (0, this.Image.PEOptionalHeader.StandardFields.LMinor);
        }

        [Test]
        public void POHNTSpecificFieldsTest ()
        {
            //Assert.AreEqual(0x400000, this.Image.PEOptionalHeader.NTSpecificFields.ImageBase);
            Assert.AreEqual (0x2000, this.Image.PEOptionalHeader.NTSpecificFields.SectionAlignment);

            Assert.IsTrue (this.Image.PEOptionalHeader.NTSpecificFields.FileAlignment == 0x200 ||
                           this.Image.PEOptionalHeader.NTSpecificFields.FileAlignment == 0x1000);

            Assert.AreEqual (4, this.Image.PEOptionalHeader.NTSpecificFields.OSMajor);
            Assert.AreEqual (0, this.Image.PEOptionalHeader.NTSpecificFields.OSMinor);
            Assert.AreEqual (0, this.Image.PEOptionalHeader.NTSpecificFields.UserMajor);
            Assert.AreEqual (0, this.Image.PEOptionalHeader.NTSpecificFields.UserMinor);
            Assert.AreEqual (4, this.Image.PEOptionalHeader.NTSpecificFields.SubSysMajor);
            Assert.AreEqual (0, this.Image.PEOptionalHeader.NTSpecificFields.SubSysMinor);
            Assert.AreEqual (0, this.Image.PEOptionalHeader.NTSpecificFields.Reserved);
            //Assert.AreEqual(0, this.Image.PEOptionalHeader.NTSpecificFields.FileChecksum);
            //Assert.AreEqual(0, this.Image.PEOptionalHeader.NTSpecificFields.DLLFlags);
            Assert.AreEqual (0x100000, this.Image.PEOptionalHeader.NTSpecificFields.StackReserveSize);
            Assert.AreEqual (0x1000, this.Image.PEOptionalHeader.NTSpecificFields.StackCommitSize);
            Assert.AreEqual (0x100000, this.Image.PEOptionalHeader.NTSpecificFields.HeapReserveSize);
            Assert.AreEqual (0x1000, this.Image.PEOptionalHeader.NTSpecificFields.HeapCommitSize);
            //Assert.AreEqual(0, this.Image.PEOptionalHeader.NTSpecificFields.LoaderFlags);
            Assert.AreEqual (0x10, this.Image.PEOptionalHeader.NTSpecificFields.NumberOfDataDir);
        }

        [Test]
        public void POHDataDirTest ()
        {
            Assert.AreEqual (DataDirectory.Zero, this.Image.PEOptionalHeader.DataDirectories.ExportTable);
            Assert.AreEqual (DataDirectory.Zero, this.Image.PEOptionalHeader.DataDirectories.Reserved);
        }

        [Test]
        public void SectionsTest ()
        {
            foreach (Section s in this.Image.Sections) {
                Assert.AreEqual (RVA.Zero, s.PointerToLineNumbers);
                Assert.AreEqual (0, s.NumberOfLineNumbers);
            }
        }

        [Test]
        public void CLIHeaderTest ()
        {
            Assert.AreEqual (2, this.Image.CLIHeader.MajorRuntimeVersion);
            Assert.AreEqual (0, this.Image.CLIHeader.MinorRuntimeVersion);
            Assert.AreEqual (DataDirectory.Zero, this.Image.CLIHeader.CodeManagerTable);

            if (this.Image.CLIHeader.ImageHash.Length > 0) {
                Assert.AreEqual (128, this.Image.CLIHeader.ImageHash.Length);
            } else {
                Assert.AreEqual (0, this.Image.CLIHeader.ImageHash.Length);
            }
        }
    }
}
