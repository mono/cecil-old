using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using System.Reflection;

namespace Cecil.Decompiler.Tests.Cecil.Decompilation
{
    public class DecompilationTestFactory
    {
        public static TestSuite CreateTestSuite(MethodInfo method)
        {
            var suite = new DecompilationTestSuite(method);
            NUnitFramework.ApplyCommonAttributes(method, suite);
            PopulateTestSuite(method, suite);
            return suite;
        }

        private static void PopulateTestSuite(MethodInfo method, DecompilationTestSuite suite)
        {
            DecompilationTestAttribute attribute = GetDecompilationTestAttribute(method);

            if (attribute == null)
                throw new ArgumentException();
            

            foreach (var value in Enum.GetValues(typeof(TestCaseType)))
            {
                var test = CreateTestCase(method, (TestCaseType)value,attribute.Filename,attribute.ResultFilename);
                if (test != null)
                    suite.Add(test);
            }
        }

        public static DecompilationTestAttribute GetDecompilationTestAttribute(MethodInfo method)
        {
            object[] attributes = method.GetCustomAttributes(typeof (DecompilationTestAttribute), true);
            DecompilationTestAttribute attribute = null;

            if (attributes != null && attributes.Length > 0)
            {
                attribute = attributes[0] as DecompilationTestAttribute;
            }
            return attribute;
        }

        private static TestCase CreateTestCase(MethodInfo method, TestCaseType testCaseType, string filename, string resultFilename)
        {
            return new DecompilationTestCase(method, testCaseType, filename, resultFilename);
        }
    }

    public enum TestCaseType
    {
        Debug,
        Release
    }
}
