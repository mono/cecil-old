#region license
//
// (C) db4objects Inc. http://www.db4o.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.ControlFlow {

	[TestFixture]
	public class ControlFlowTestFixture : BaseControlFlowTestFixture {

		[Test]
		public void Switch ()
		{
			RunTestCase ("Switch");
		}

		[Test]
		public void StaticField ()
		{
			RunTestCase ("StaticField");
		}

		[Test]
		public void StringCast ()
		{
			RunTestCase ("StringCast");
		}

		[Test]
		public void StringTryCast ()
		{
			RunTestCase ("StringTryCast");
		}

		[Test]
		public void FloatGreaterThan ()
		{
			RunTestCase ("FloatGreaterThan");
		}

		[Test]
		public void FloatEquals ()
		{
			RunTestCase ("FloatEquals");
		}

		[Test]
		public void BoolOrLessOrEqualThan ()
		{
			RunTestCase ("BoolOrLessOrEqualThan");
		}

		[Test]
		public void OptimizedNestedOr ()
		{
			RunTestCase ("OptimizedNestedOr");
		}

		[Test]
		public void NestedOrGreaterThan ()
		{
			RunTestCase ("NestedOrGreaterThan");
		}

		[Test]
		public void InRange ()
		{
			RunTestCase ("InRange");
		}

		[Test]
		public void OptimizedAnd ()
		{
			RunTestCase ("OptimizedAnd");
		}

		[Test]
		public void BoolAndGreaterOrEqualThan ()
		{
			RunTestCase ("BoolAndGreaterOrEqualThan");
		}

		[Test]
		public void OptimizedOr ()
		{
			RunTestCase ("OptimizedOr");
		}

		[Test]
		public void NotStringEquality ()
		{
			RunTestCase ("NotStringEquality");
		}

		[Test]
		public void IsNull ()
		{
			RunTestCase ("IsNull");
		}

		[Test]
		public void FieldAccessor ()
		{
			RunTestCase ("FieldAccessor");
		}

		[Test]
		public void NotEqual ()
		{
			RunTestCase ("NotEqual");
		}

		[Test]
		public void GreaterThanOrEqual ()
		{
			RunTestCase ("GreaterThanOrEqual");
		}

		[Test]
		public void LessThanOrEqual ()
		{
			RunTestCase ("LessThanOrEqual");
		}

		[Test]
		public void Empty ()
		{
			RunTestCase ("Empty");
		}

		[Test]
		public void SimpleReturn ()
		{
			RunTestCase ("SimpleReturn");
		}

		[Test]
		public void SimpleCalculation ()
		{
			RunTestCase ("SimpleCalculation");
		}

		[Test]
		public void SimpleCondition ()
		{
			RunTestCase ("SimpleCondition");
		}

		[Test]
		public void SingleAnd ()
		{
			RunTestCase ("SingleAnd");
		}

		[Test]
		public void SingleOr ()
		{
			RunTestCase ("SingleOr");
		}

		[Test]
		public void MultipleOr ()
		{
			RunTestCase ("MultipleOr");
		}

		[Test]
		public void MixedAndOr ()
		{
			RunTestCase ("MixedAndOr");
		}

		[Test]
		public void SimpleIf ()
		{
			RunTestCase ("SimpleIf");
		}

		[Test]
		public void TwoIfs ()
		{
			RunTestCase ("TwoIfs");
		}

		[Test]
		public void FalseIf ()
		{
			RunTestCase ("FalseIf");
		}

		[Test]
		public void IfNestedCondition ()
		{
			RunTestCase ("IfNestedCondition");
		}

		[Test]
		public void ThreeReturns ()
		{
			RunTestCase ("ThreeReturns");
		}

		[Test]
		public void TernaryExpression ()
		{
			RunTestCase ("TernaryExpression");
		}

		[Test]
		public void SideEffectExpression ()
		{
			RunTestCase ("SideEffectExpression");
		}

		[Test]
		public void SimpleWhile ()
		{
			RunTestCase ("SimpleWhile");
		}

		[Test]
		public void FlowTest ()
		{
			RunTestCase ("FlowTest");
		}

		[Test]
		public void PropertyPredicate ()
		{
			RunTestCase ("PropertyPredicate");
		}

		[Test]
		public void MultipleAndOr ()
		{
			RunTestCase ("MultipleAndOr");
		}

		[Test]
		public void StringPredicate ()
		{
			RunTestCase ("StringPredicate");
		}

		[Test]
		public void InPlaceAdd ()
		{
			RunTestCase ("InPlaceAdd");
		}

		[Test]
		public void MathOperators ()
		{
			RunTestCase ("MathOperators");
		}
	}
}
