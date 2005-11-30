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

using System;
using System.Collections;
using Cecil.FlowAnalysis.ActionFlow;

namespace Cecil.FlowAnalysis.Impl.ActionFlow {
	/// <summary>
	/// </summary>
	internal class ActionBlockCollection : CollectionBase, IActionBlockCollection {
		public ActionBlockCollection ()
		{
		}

		public IActionBlock this [int index]
		{
			get {
				return (IActionBlock)InnerList [index];
			}
		}

		public int IndexOf (IActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			return InnerList.IndexOf (block);
		}

		public void Add (IActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			InnerList.Add (block);
		}

		public void Insert (int index, IActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			InnerList.Insert (index, block);
		}

		public void Remove (IActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			InnerList.Remove (block);
		}

		public IActionBlock[] ToArray ()
		{
			return (IActionBlock[])InnerList.ToArray (typeof(IActionBlock));
		}
	}
}
