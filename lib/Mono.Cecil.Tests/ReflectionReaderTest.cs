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

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Metadata;

    using NUnit.Framework;

    [TestFixture]
    public sealed class ReflectionReaderTest {

        private IAssemblyDefinition m_asmdef;

        [SetUp]
        public void SetUp ()
        {
            m_asmdef = AssemblyFactory.GetAssembly (@"D:\mscorlib.dll");
        }

        [Test]
        public void PourVoir ()
        {
            Console.WriteLine (m_asmdef.Name.FullName);
            Console.WriteLine ("modules : {0}", m_asmdef.Modules.Count);
            foreach (IModuleDefinition def in m_asmdef.Modules) {
                Console.WriteLine ("module name : {0}", def.Name);
                Console.WriteLine ("module guid : {0}", def.Mvid.ToString ());

                Console.WriteLine ("asm refs : {0}", def.AssemblyReferences.Count);
                foreach (IAssemblyNameReference name in def.AssemblyReferences) {
                    Console.WriteLine ("asm ref : {0}", name.FullName);
                }

                Console.WriteLine ("module ref : {0}", def.ModuleReferences.Count);
                foreach (IModuleReference mr in def.ModuleReferences) {
                    Console.WriteLine ("module ref : {0}", mr.Name);
                }

                foreach (ITypeDefinition type in def.Types) {
                    Console.WriteLine (type.FullName);
                }

                if (def.Main) {
                    ITypeDefinition array = def.Types ["System.Array"];
                    Console.WriteLine ("Array base type: " + array.BaseType.FullName);

                    ITypeDefinition arrayEnum = def.Types ["System.Array/ArrayEnumerator"];
                    Console.WriteLine ("ArrayEnum dec type: " + arrayEnum.DeclaringType.FullName);
                }
            }
        }
    }
}
