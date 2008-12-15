using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Core;

namespace Cecil.Decompiler.Tests.Cecil.Decompilation
{
   public class DecompilationTestSuite:TestSuite
    {
        public DecompilationTestSuite(MethodInfo method):base(method.DeclaringType.FullName,method.Name)
        {
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            if (this.Parent != null)
                this.Fixture = this.Parent.Fixture;

            return base.Run(listener, filter);
        }
    }
}
