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
    using Mono.Cecil.Metadata;

    using NUnit.Framework;

    [TestFixture]
    public sealed class MetadataReaderTest : AbstractReaderTest {

        [Test]
        public void MDHeaderTest() {
            Assert.AreEqual(0x424a5342, this.Image.MetadataRoot.Header.Signature);
            Assert.AreEqual(0, this.Image.MetadataRoot.Header.Reserved);
            Assert.AreEqual(0, this.Image.MetadataRoot.Header.Flags);
            //Console.WriteLine("streams {0}", this.Image.MetadataRoot.Header.Streams);
        }

        [Test]
        public void MDStreamHeaderTest() {
            /*foreach (MetadataStream mds in this.Image.MetadataRoot.Streams) {
                Console.WriteLine("stream name : {0}", mds.Header.Name);
            }*/
        }

        [Test]
        public void GuidsHeapTest() {
            GuidHeap heap = this.Image.MetadataRoot.Streams.GuidHeap;
            Assert.AreEqual(0, heap.Guids.Count);
            Assert.AreEqual(0, heap.Data.Length % 16);
            /*int count = heap.Data.Length / 16;
            for (int i = 0 ; i < count ; i++) {
                Guid id = heap[i];
                Console.WriteLine(id);
            }
            Assert.AreEqual(count, heap.Guids.Count);*/
        }

        [Test]
        public void TablesHeapTest() {
            TablesHeap heap = this.Image.MetadataRoot.Streams.TablesHeap;
            Assert.AreEqual(0, heap.Reserved);
            Assert.AreEqual(1, heap.MajorVersion);
            Assert.AreEqual(0, heap.MinorVersion);
            //Assert.AreEqual(1, heap.Reserved2);

            foreach (IMetadataTable table in heap.Tables) {
                Console.WriteLine("table {0} has {1} rows", table.GetType().FullName,
                    table.Rows.Count);
            }
        }

        [Test]
        public void StringsHeapTest() {
            /*StringsHeap heap = this.Image.MetadataRoot.Streams.StringsHeap;
            foreach (string s in heap.Strings.Values) {
                Console.WriteLine("str : {0}", s);
            }*/
        }

    }
}
