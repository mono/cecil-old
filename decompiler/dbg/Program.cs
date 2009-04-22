using System;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Cecil.Decompiler;
using Cecil.Decompiler.Languages;
using Cecil.Decompiler.Cil;

namespace Cecil.Decompiler.Debug {

	class Program {

		static void Bar ()
		{
		}

		static int Baz (int foo)
		{
			return Gazonk (foo);
		}

		static int Gazonk (int g)
		{
			return 42;
		}

		static int Bang (int b)
		{
			if (b == 12)
				return Gazonk (b);

			return b;
		}

		static int Shabang (int b)
		{
			if (b == 12)
				b = 321;
			else
				b = 14;

			return b;
		}

		static void Fiouz ()
		{
			Baz (2);
			Gazonk (3);
			Bar ();
		}

		static void Foo (int a)
		{
			Console.WriteLine (a);
			if ((a > 0 && a < 120) && a > 160) {
				a = 42;
			} else {
				a = 24;
			}
			Console.WriteLine (a);
		}

		static void FooFoo (int a)
		{
			Console.WriteLine (a);
			if (a > 0) {
				a = 42;
			}
			Console.WriteLine (a);
		}

		static void FooFooFoo (int a)
		{
			Console.WriteLine (a);
			while (a > 0) {
				a = a - 1;
			}
			Console.WriteLine (a);
		}

		static void FooFooFooFoo (int a)
		{
			Console.WriteLine (a);
			for (int i = 0; i < a; i++) {
				a = a - 1;
			}
			Console.WriteLine (a);
		}

		static void FooBang (IEnumerable<string> strings)
		{
			Console.WriteLine ("a");
			foreach (var str in strings)
				Console.WriteLine (str);
			Console.WriteLine ("b");
		}

		static void FooFooFooFooFoo (int a)
		{
			while (a < 100) {
				do {
					a *= 10;
				} while (a < 10);
				a += 2;
			}
		}

		static void Patapon (int a)
		{
			Console.WriteLine ("before for");
			for (int i = 0; i < a; i++) {
				Console.WriteLine ("before while");
				while (i > 42) {
					Console.WriteLine ("before do while");
					do {
						i++;
					} while (i > 12);
					Console.WriteLine ("after do while");
				}
				Console.WriteLine ("after while");

				if (i == 12)
					continue;

				Console.WriteLine ("after continue");

				//if (i == 24)
				//	break;
			}
			Console.WriteLine ("after for");
		}

		static void Shazam (int boutch)
		{
			//int a = 42;
			//return (a < 0 ? (a < 100 ? 12 : 24) : (a > 0 ? 10 : 42));
			//Console.WriteLine (a > 12 ? (a > 24 ? "c" : "a") : "b");
			//Console.WriteLine ("first");
			if (boutch > 0) {
				Console.WriteLine ("first in if");
				if (boutch > 12) {
					Console.WriteLine ("> 12");
				} else if (boutch > 14) {
					Console.WriteLine ("> 14");
				} else {
					Console.WriteLine ("> else");
				}
				Console.WriteLine ("last in if");
			} else {
				Console.WriteLine ("first in else");
				if (boutch < 12) {
					Console.WriteLine ("< 12");
				} else if (boutch < 14) {
					Console.WriteLine ("< 14");
				} else {
					Console.WriteLine ("< else");
				}
				Console.WriteLine ("last in else");
			}
			Console.WriteLine ("last");
		}

		static void TaBon (int boutch)
		{
			if (boutch > 0) {
				Console.WriteLine ("tadam");
				return;
			}

			Console.WriteLine ("end");
		}

		static void Pan (int a)
		{
			Console.WriteLine (a);
			switch (a) {
			case 0:
				Console.WriteLine ("0");
				break;
			case 1:
				Console.WriteLine ("1");
				break;
			default:
				Console.WriteLine ("?");
				break;
			}
			Console.WriteLine (a);
		}

		static void Alarm (int a)
		{
			Console.WriteLine ("before try");
			try {
				Console.WriteLine ("in try");
			} catch (ArgumentException e) {
				Console.WriteLine ("catch {0}", e);
			} catch {
				Console.WriteLine ("global catch");
			} finally {
				Console.WriteLine ("finally");
			}
			Console.WriteLine ("after try");
		}

		static void Tapon (object o, object o2)
		{
			var data = o is Program;
			Console.WriteLine (data);
			Console.WriteLine (o2 is short);
		}

		public class Pif {
			public void Tzap ()
			{
			}
		}

		public struct Pouf {
			public int pouf;

			public int Poufy
			{
				get { return pouf; }
				set { pouf = value; }
			}

			public Pouf (int pouf)
			{
				this.pouf = pouf;
			}

			public void Tap ()
			{
			}
		}

		static void Tagadou ()
		{
			var pif = new Pif ();
			pif.Tzap ();

			var pouf = new Pouf ();
			pouf.Poufy = 42;

			Console.WriteLine (pouf.Poufy);

			pouf.Tap ();

			var poufy = new Pouf (42);
			Console.WriteLine (poufy.pouf);
		}

		static bool IsA (object o)
		{
			return o is Program;
		}

		static void And (int a)
		{
			if (a > 1 && a < 10) {
				Console.WriteLine ("t");
			} else
				Console.WriteLine ("f");
		}

		static void Or (int a)
		{
			if (a > 1 || a < 10) {
				Console.WriteLine ("t");
			} else
				Console.WriteLine ("f");
		}

		static string NullCoalescing (string str)
		{
			return str ?? "nil";
		}

		static int CondExp (int i)
		{
			return i > 0 ? 1 : i == 0 ? 0 : -1;
		}

		static void Szvitch (int a)
		{
			Console.WriteLine (a);
			switch (a) {
			case 72:
				Console.WriteLine ("0");
				break;
			case 42:
				Console.WriteLine ("1");
				break;
			//case 96:
			//    Console.WriteLine ("2");
			//    break;
			//case 106:
			//    Console.WriteLine ("3");
			//    break;
			//default:
			//    Console.WriteLine ("?");
			//    break;
			}
			Console.WriteLine (a);
		}

		static void ContinueFor (int a)
		{
			for (int i = 0; i < a; i++) {
				if (a == 10) {

					Console.WriteLine ("foo");

					if (a == 10) {
						Console.WriteLine ("ready to break");
						continue;
					} else {
						Console.WriteLine ("not breaked");
					}

					Console.WriteLine ("bar");

				} else
					Console.WriteLine ("body");
			}
		}

		static void BalleDeContinue (int a)
		{
			while (a < 100) {
				Console.WriteLine (a);
				if (a == 12) {
					Console.WriteLine (a);
					continue;
				}

				a--;
			}
		}

		static void TestIfElse (int a)
		{
			if (a == 12) {
				Console.WriteLine ("12");
			} else {
				Console.WriteLine ("else");
			}
		}

		static void Foreachy (IEnumerable<string> listy)
		{
			foreach (var stream in listy)
				Console.WriteLine (stream);
		}

		static void ForeachForeach (IEnumerable<IEnumerable<string>> lilisty)
		{
			foreach (var stream in lilisty)
				foreach (var str in stream)
					Console.WriteLine (str);
		}

		static void ForFor (int [] [] integers)
		{
			for (int i = 0; i < integers.Length; i++)
				for (int j = 0; j < integers [i].Length; j++)
					Console.WriteLine (integers [i] [j]);
		}

		static void PanPan (string [] s)
		{
			s [2] = "hello";
		}

		static void Fory (string [] strings)
		{
			for (int i = 0; i < strings.Length; i++) {
				Console.WriteLine (strings [i]);
			}
		}

		class Bonbon : IDisposable {

			public void Dispose ()
			{
				Console.WriteLine ("spose ? ahahha");
			}
		}

		static void Mange (Bonbon b)
		{
			using (b) {
				Console.WriteLine ("cronch {0}", b);
			}
		}

		static object john = new object ();

		static void Lost ()
		{
			lock (john) {
				Console.WriteLine ("couteau");
			}
		}

		static Type GetTypeOfObject ()
		{
			return typeof (object);
		}

		public void TryInWhileInTry (int a)
		{
			try {
				while (a > 10) {
					try {
						a--;
					} catch (Exception e2) {
						Console.WriteLine (e2);
					}
				}
			} catch (Exception e) {
				Console.WriteLine (e);
			}
		}

		static void Main (string [] args)
		{
			var method = GetProgramMethod ("ForeachForeach");

			var cfg = ControlFlowGraph.Create (method);

			FormatControlFlowGraph (Console.Out, cfg);

			Console.WriteLine ("--------------------");

			var store = AnnotationStore.CreateStore (cfg, BlockOptimization.Detailed);
			PrintAnnotations (method, store);

			var language = CSharp.GetLanguage (CSharpVersion.V1);

			//var body = method.Body.Decompile (language);

			var writer = language.GetWriter (new PlainTextFormatter (Console.Out));

			writer.Write (method);
		}

		public static void PrintAnnotations (MethodDefinition method, AnnotationStore store)
		{
			foreach (Instruction instruction in method.Body.Instructions) {
				Console.Write ("L_{0}: {1} {2}", instruction.Offset.ToString ("x4"), instruction.OpCode.Name, FormatOperand (instruction.Operand));

				Annotation annotation;
				if (store.TryGetAnnotation (instruction, out annotation)) {
					Console.Write (" - ");
					Console.Write (annotation);

					object data;
					if (store.TryGetData<object> (instruction, out data)) {
						Console.Write (" - ");
						if (data is ConditionData)
							PrintConditionData ((ConditionData) data);
						else if (data is LoopData)
							PrintLoopData ((LoopData) data);
						else
							Console.Write (data);
					}
				}

				Console.WriteLine ();
			}
		}

		static void PrintConditionData (ConditionData data)
		{
			Console.Write ("Then {0}:{1} ", data.Then.Start.Index, data.Then.End.Index);
			if (data.Else != null)
				Console.Write ("Else {0}:{1}", data.Else.Start.Index, data.Else.End.Index);
			Console.WriteLine ();
		}

		static void PrintLoopData (LoopData data)
		{
			Console.WriteLine ("Body {0}:{1}", data.Body.Start.Index, data.Body.End.Index);
		}

		static string FormatOperand (object o)
		{
			if (o == null)
				return "";

			var instruction = o as Instruction;
			if (instruction != null)
				return "L_" + instruction.Offset.ToString ("x4");

			return o.ToString ();
		}

		static MethodDefinition GetProgramMethod (string name)
		{
			return GetProgramAssembly ().MainModule.Types ["Cecil.Decompiler.Debug.Program"].Methods.GetMethod (name) [0];
		}

		static IAssemblyResolver resolver = new DefaultAssemblyResolver ();

		static AssemblyDefinition GetProgramAssembly ()
		{
			var assembly = AssemblyFactory.GetAssembly (typeof (Program).Module.FullyQualifiedName);
			assembly.Resolver = resolver;
			return assembly;
		}

		public static string ToString (ControlFlowGraph cfg)
		{
			StringWriter writer = new StringWriter ();
			FormatControlFlowGraph (writer, cfg);
			return writer.ToString ();
		}

		public static void FormatControlFlowGraph (TextWriter writer, ControlFlowGraph cfg)
		{
			foreach (InstructionBlock block in cfg.Blocks) {
				writer.WriteLine ("block {0}:", block.Index);
				writer.WriteLine ("\tbody:");
				foreach (Instruction instruction in block) {
					writer.Write ("\t\t");
					var data = cfg.GetData (instruction);
					writer.Write ("[{0}:{1}] ", data.StackBefore, data.StackAfter);
					Formatter.WriteInstruction (writer, instruction);
					writer.WriteLine ();
				}
				InstructionBlock [] successors = block.Successors;
				if (successors.Length > 0) {
					writer.WriteLine ("\tsuccessors:");
					foreach (InstructionBlock successor in successors) {
						writer.WriteLine ("\t\tblock {0}", successor.Index);
					}
				}
			}
		}
	}
}
