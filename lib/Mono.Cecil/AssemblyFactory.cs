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
*****************************************************************************/

namespace Mono.Cecil {

    using System;

    using Mono.Cecil.Binary;
    using Mono.Cecil.Implem;
    using Mono.Cecil.Metadata;

    public class AssemblyFactory {

        private AssemblyFactory ()
        {
        }

        public static IAssemblyDefinition GetAssembly (string file, LoadingType loadType)
        {
            try {
                ImageReader brv = new ImageReader (file);
                StructureReader srv = new StructureReader (brv);
                AssemblyDefinition asm = new AssemblyDefinition (new AssemblyNameDefinition (), srv, loadType);

                asm.Accept (srv);
                return asm;
            } catch (ReflectionException) {
                throw;
            } catch (MetadataFormatException) {
                throw;
            } catch (ImageFormatException) {
                throw;
            } catch (Exception e) {
                throw new ReflectionException ("Can not disassemble assembly", e);
            }
        }

        public static IAssemblyDefinition GetAssembly (string file)
        {
            return GetAssembly (file, LoadingType.Lazy);
        }
    }
}
