using System;
using System.Collections;
using System.Reflection;
using Gendarme.Framework;
using Gendarme.Rules.Exceptions;
using Mono.Cecil;
using NUnit.Framework;

namespace Gendarme.Rules.Exceptions.Tests {

	[TestFixture]	
	public class TestPatterns {
	
		private IMethodRule rule;
		private IAssemblyDefinition assembly;
		private IModuleDefinition module;
		private ITypeDefinition type;
		
		public TestPatterns ()
		{
		}

		// Test setup
		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			module = assembly.MainModule;
			string fullname = "Gendarme.Rules.Exceptions.Tests.TestPatterns";
			type = module.Types [fullname];
			rule = new DontDestroyStackTrace ();
		}

		// Test infrastructure
		private IMethodDefinition GetMethodToTest (string name)
		{
			return type.Methods.GetMethod (name, new Type [0]);
		}

		// Individual test cases
		[Test]
		public void TestThrowOriginalEx ()
		{
			string testName = "ThrowOriginalEx";
			IMethodDefinition method = GetMethodToTest (testName);

			// Should result in 1 warning message
			IList list = rule.CheckMethod (assembly, module, type, method, new MinimalRunner ());
			Assert.IsTrue (list != null, "Warnings were not generated for the test named " + testName);
			Assert.AreEqual (list.Count, 1, "One warning should have been generated for the test named " + testName);
		}

		[Test]
		public void TestThrowOriginalExWithJunk ()
		{
			string testName = "ThrowOriginalExWithJunk";
			IMethodDefinition method = GetMethodToTest (testName);
			
			// Should result in 1 warning message
			IList list = rule.CheckMethod (assembly, module, type, method, new MinimalRunner ());
			Assert.IsTrue (list != null, "Warnings were not generated for the test named " + testName);
			Assert.AreEqual (list.Count, 1, "One warning should have been generated for the test named " + testName);
		}

		[Test]
		public void TestRethrowOriginalEx ()
		{
			string testName = "RethrowOriginalEx";
			IMethodDefinition method = GetMethodToTest (testName);

			// Should result in 0 warning messages
			IList list = rule.CheckMethod (assembly, module, type, method, new MinimalRunner ());
			Assert.IsTrue (list == null, "Warnings were generated for the test named " + testName);
		}

		[Test]
		public void TestThrowOriginalExAndRethrowWithJunk ()
		{
			string testName = "ThrowOriginalExAndRethrowWithJunk";
			IMethodDefinition method = GetMethodToTest (testName);

			// Should result in one warning message
			IList list = rule.CheckMethod (assembly, module, type, method, new MinimalRunner ());
			Assert.IsTrue (list != null, "Warnings were not generated for the test named " + testName);
			Assert.AreEqual (list.Count, 1, "One warning should have been generated for the test named " + testName);
		}

		[Test]
		public void TestRethrowOriginalExAndThrowWithJunk ()
		{
			string testName = "RethrowOriginalExAndThrowWithJunk";
			IMethodDefinition method = GetMethodToTest (testName);

			// Should result in one warning message
			IList list = rule.CheckMethod (assembly, module, type, method, new MinimalRunner ());
			Assert.IsTrue (list != null, "Warnings were not generated for the test named " + testName);
			Assert.AreEqual (list.Count, 1, "One warning should have been generated for the test named " + testName);
		}

		// Functions whose IL is used by the test cases for the DontDestroyStackTrace rule

		public void ThrowOriginalEx ()
		{
			int i = 0;
			try  {
				i = Int32.Parse("Broken!");
			}
			catch (Exception ex) {
				// Throw exception immediately.
				// This should trip the DontDestroyStackTrace rule.
				throw ex;
			}
		}

		public void ThrowOriginalExWithJunk ()
		{
			int i = 0;
			try {
				i = Int32.Parse ("Broken!");
			}
			catch (Exception ex) {
				int j = 0;
				for (int k=0; k<10; k++) {
					// throw some junk into the catch block, to ensure that
					j += 10;
					Console.WriteLine (j);
				}

				// This should trip the DontDestroyStackTrace rule, because we're
				// throwing the original exception.
				throw ex;
			}
		}

		public void RethrowOriginalEx ()
		{
			int i = 0;
			try {
				i = Int32.Parse ("Broken!");
			}
			catch (Exception ex) {
				// This should NOT trip the DontDestroyStackTrace rule, because we're
				// rethrowing the original exception.
				throw;
			}
		}

		public void ThrowOriginalExAndRethrowWithJunk ()
		{
			int i = 0;
			try {
				i = Int32.Parse ("Broken!");
			}
			catch (Exception ex) {
				int j = 0;
				for (int k=0; k<10; k++) {
					// throw some junk into the catch block, to ensure that
					j += 10;
					Console.WriteLine (j);
					if ((i % 1234) > 56) {
						// This should trip the DontDestroyStackTrace rule, because we're
						// throwing the original exception.
						throw ex;
					}
				}

				// More junk - just to ensure that alternate paths through
				// this catch block end up at a throw and a rethrow
				throw;
			}
		}

		public void RethrowOriginalExAndThrowWithJunk ()
		{
			int i = 0;
			try {
				i = Int32.Parse ("Broken!");
			}
			catch (Exception ex) {
				int j = 0;
				for (int k=0; k<10; k++) {
					// throw some junk into the catch block, to ensure that
					j += 10;
					Console.WriteLine (j);
					if ((i % 1234) > 56) {
						// More junk - just to ensure that alternate paths through
						// this catch block end up at a throw and a rethrow
						throw;
					}
				}

				// This should trip the DontDestroyStackTrace rule, because we're
				// throwing the original exception.
				throw ex;
			}
		}
	}
}
