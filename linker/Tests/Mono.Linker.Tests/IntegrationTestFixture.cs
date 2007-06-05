
using System;
using System.Diagnostics;
using System.IO;
using Mono.Linker.Steps;
using NUnit.Framework;

namespace Mono.Linker.Tests {

	[TestFixture]
	public class IntegrationTestFixture : AbstractTestFixture {

		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			TestCasesRoot = "Integration";
		}

		[Test]
		public void TestHelloWorld ()
		{
			Test ("HelloWorld");
		}

		[Test]
		public void TestCrypto ()
		{
			Test ("Crypto");
		}

		protected override LinkContext GetContext()
		{
			LinkContext context = base.GetContext ();
			context.CoreAction = AssemblyAction.Link;
			return context;
		}

		protected override Pipeline GetPipeline ()
		{
			Pipeline p = new Pipeline ();
			p.AppendStep (new LoadReferencesStep ());
			p.AppendStep (new BlacklistStep ());
			p.AppendStep (new MarkStep ());
			p.AppendStep (new SweepStep ());
			p.AppendStep (new CleanStep ());
			p.AppendStep (new OutputStep ());
			return p;
		}

		protected override void Test (string testCase)
		{
			base.Test (testCase);
			string test = Path.Combine (GetTestCasePath (), "Test.exe");

			string original = Execute (GetTestCasePath (), "Test.exe");

			Pipeline.PrependStep (
				new ResolveFromAssemblyStep (
					test));

			Run ();

			string linked = Execute (Context.OutputDirectory, "Test.exe");

			Assert.AreEqual (original, linked);
		}

		static string Execute (string directory, string file)
		{
			Process p = new Process ();
			p.StartInfo.EnvironmentVariables ["MONO_PATH"] = directory;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.WorkingDirectory = directory;
			p.StartInfo.FileName = Path.Combine (directory, file);
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.UseShellExecute = false;

			p.Start ();
			p.WaitForExit ();

			return p.StandardOutput.ReadToEnd ();
		}

		static bool OnMono ()
		{
			return Type.GetType ("System.MonoType") != null;
		}
	}
}
