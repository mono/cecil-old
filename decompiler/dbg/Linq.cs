using System;
using System.Collections.Generic;
using System.Linq;
using SLE = System.Linq.Expressions;
using System.Reflection;

using Cecil.Decompiler;
using Cecil.Decompiler.Ast;
using Cecil.Decompiler.Languages;

using Mono.Cecil;

namespace Cecil.Decompiler.Linq {

	class ExpressionConverter : BaseCodeVisitor {

		SLE.Expression [] parameters;
		MethodDefinition method;
		Stack<SLE.Expression> expressions = new Stack<SLE.Expression> ();

		private ExpressionConverter (SLE.Expression [] parameters, MethodDefinition method)
		{
			this.parameters = parameters;
			this.method = method;
		}

		void Push (SLE.Expression expression)
		{
			expressions.Push (expression);
		}

		SLE.Expression Pop ()
		{
			return expressions.Pop ();
		}

		public override void VisitBinaryExpression (BinaryExpression node)
		{
			Visit (node.Right);
			Visit (node.Left);

			Push (SLE.Expression.MakeBinary (
				ConvertBinaryType (node.Operator),
				Pop (),
				Pop ()));
		}

		public override void VisitLiteralExpression (LiteralExpression node)
		{
			Push (SLE.Expression.Constant (node.Value));
		}

		public override void VisitArgumentReferenceExpression (ArgumentReferenceExpression node)
		{
			Push (parameters [method.Parameters.IndexOf ((ParameterDefinition) node.Parameter)]);
		}

		SLE.ExpressionType ConvertBinaryType (BinaryOperator @operator)
		{
			switch (@operator) {
			case BinaryOperator.Add:
				return SLE.ExpressionType.Add;
			case BinaryOperator.Multiply:
				return SLE.ExpressionType.Multiply;
			default:
				throw new NotImplementedException ();
			}
		}

		public static SLE.Expression ConvertMethod (SLE.Expression [] parameters, MethodDefinition method)
		{
			var body = method.Body.Decompile (CSharp.GetLanguage (CSharpVersion.V1));

			if (body.Statements.Count != 1)
				throw new ArgumentException ();

			var @return = body.Statements [0] as ReturnStatement;
			if (@return == null)
				throw new ArgumentException ();

			var converter = new ExpressionConverter (parameters, method);
			converter.Visit (@return.Expression);
			return converter.Pop ();
		}
	}

	static class DelegateConverter {

		public static SLE.Expression<TDelegate> ToExpression<TDelegate> (TDelegate @delegate) where TDelegate : class
		{
			if (@delegate == null)
				throw new ArgumentNullException ();

			if (!typeof (Delegate).IsAssignableFrom (typeof (TDelegate)))
				throw new ArgumentException ();

			var dlg = @delegate as Delegate;

			return ToExpression<TDelegate> (dlg);
		}

		static SLE.Expression<TDelegate> ToExpression<TDelegate> (Delegate @delegate)
		{
			var parameters = CreateParameters (@delegate.Method.GetParameters ());
			var method = GetMethodDefinition (@delegate.Method);

			return SLE.Expression.Lambda<TDelegate> (
				ExpressionConverter.ConvertMethod (parameters, method),
				parameters);
		}

		static SLE.ParameterExpression [] CreateParameters (ParameterInfo [] originals)
		{
			var parameters = new SLE.ParameterExpression [originals.Length];
			for (int i = 0; i < originals.Length; i++)
				parameters [i] = SLE.Expression.Parameter (originals [i].ParameterType, originals [i].Name);

			return parameters;
		}

		static MethodDefinition GetMethodDefinition (MethodInfo original)
		{
			TypeDefinition type = GetTypeDefinition (original.DeclaringType);

			var matches = from MethodDefinition meth in type.Methods
						  where MethodMatch (meth, original)
						  select meth;

			return matches.FirstOrDefault ();
		}

		static AssemblyDefinition GetAssembly (Assembly assembly)
		{
			return AssemblyFactory.GetAssembly (assembly.ManifestModule.FullyQualifiedName);
		}

		static string GetFullName (Type type)
		{
			if (type.DeclaringType != null)
				return type.FullName.Replace ('+', '/');

			return type.FullName;
		}

		static TypeDefinition GetTypeDefinition (Type type)
		{
			var assembly = GetAssembly (type.Assembly);
			return assembly.MainModule.Types [GetFullName (type)];
		}

		static bool ParameterMatch (ParameterDefinition parameter, ParameterInfo info)
		{
			return parameter.ParameterType.FullName == GetFullName (info.ParameterType);
		}

		static bool ParametersMatch (ParameterDefinitionCollection parameters, ParameterInfo [] infos)
		{
			if (parameters.Count != infos.Length)
				return false;

			for (int i = 0; i < parameters.Count; i++)
				if (!ParameterMatch (parameters [i], infos [i]))
					return false;

			return true;
		}

		static bool MethodMatch (MethodDefinition method, MethodInfo info)
		{
			if (method.Name != info.Name)
				return false;
			if (method.ReturnType.ReturnType.Name != info.ReturnType.Name)
				return false;

			return ParametersMatch (method.Parameters, info.GetParameters ());
		}
	}

	class Program {

		static void Main ()
		{
			Func<int, int> magic = i => i * 42;

			SLE.Expression<Func<int, int>> exp = DelegateConverter.ToExpression (magic);

			Console.WriteLine (exp.ToString ());
			Console.WriteLine (exp.Compile ().Invoke (1));
		}
	}
}
