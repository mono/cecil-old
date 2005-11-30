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

using Cecil.FlowAnalysis.ControlFlow;

namespace Cecil.FlowAnalysis.ActionFlow {
	public interface IActionFlowGraph {
		/// <summary>
		/// The control flow graph upon which this action flow graph
		/// was built.
		/// </summary>
		IControlFlowGraph ControlFlowGraph { get; }

		/// <summary>
		/// Action blocks.
		/// </summary>
		IActionBlockCollection Blocks { get; }

		/// <summary>
		/// Checks if the specified block is the target of
		/// a branch or conditional branch block (only the Then path
		/// is considered).
		/// </summary>
		/// <param name="block">a block</param>
		/// <returns>true if the block is the target of a branch</returns>
		bool IsBranchTarget (IActionBlock block);
	}
}
