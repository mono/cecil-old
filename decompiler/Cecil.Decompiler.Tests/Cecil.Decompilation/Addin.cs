using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace Cecil.Decompiler.Tests.Cecil.Decompilation
{
    public abstract class DecompilationAddin : IAddin,ITestCaseBuilder
    {
        public bool Install(IExtensionHost host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            var builders = host.GetExtensionPoint("TestCaseBuilders");
            
            if (builders == null) 
                return false;

            builders.Install(this);
            return true;
        }

        public bool CanBuildFrom(MethodInfo method)
        {
            if (method == null)
                return false;

            return IsDecompilationTestMethod(method);
        }

        public Test BuildFrom(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            return DecompilationTestFactory.CreateTestSuite(method);
        }

        static bool IsDecompilationTestMethod(MethodInfo method)
        {
            bool result = method.GetCustomAttributes(typeof (DecompilationTestAttribute), true).Length > 0;
            return result;
        }

    }
}
