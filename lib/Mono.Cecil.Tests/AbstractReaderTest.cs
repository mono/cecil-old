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

    public abstract class AbstractReaderTest  {

        private Image m_image;

        internal Image Image {
            get { return m_image; }
        }

        [SetUp]
        public void SetUpTest ()
        {
            if (m_image == null) {
                ImageReader ir = new ImageReader (@"D:\hello.exe");
                m_image = ir.GetImage ();
                //m_image = Image.GetImage(@"D:\hello.exe");
                //m_image = Image.GetImage(@"D:\a.netmodule");
                //m_image = Image.GetImage(@"D:\b.exe");
            }
        }
    }
}
