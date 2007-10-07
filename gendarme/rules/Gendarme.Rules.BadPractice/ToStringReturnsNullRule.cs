// 
// Gendarme.Rules.BadPractice.ToStringReturnsNullRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//
// Copyright (c) <2007> Nidhi Rawal
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.BadPractice {
	public class ToStringReturnsNullRule: IMethodRule
	{
		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection ();
			Instruction prevIns;
			int offset = 0;
			bool nullReturned = false;
			foreach (Instruction ins in method.Body.Instructions)
				if (method.Name == "ToString")
					if (ins.OpCode == OpCodes.Ret) {
						prevIns = ins.Previous;
						if (prevIns.OpCode != OpCodes.Ldnull) {
							string opCodeSt;
							if (prevIns.OpCode == OpCodes.Call || prevIns.OpCode == OpCodes.Callvirt)
								opCodeSt = ReturnSt (method, prevIns.Previous);
							else 
								opCodeSt = ReturnSt (method, prevIns);
							if (opCodeSt == null && prevIns.Operand.ToString () != "System.String System.Convert::ToString(System.Object)")
								nullReturned = true;
							if (MethodReturnsNull (method, opCodeSt)) {
								nullReturned = true;
								offset = ins.Offset;
							}
						}
						else {
							nullReturned = true;
							offset = ins.Offset;
						}
					}
			if (nullReturned) {
				Location location = new Location (method.DeclaringType.FullName, method.Name, offset);
				Message message = new Message ("ToString () seems to returns null in some condition", location, MessageType.Error);
				messageCollection.Add (message);
			}
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
		
		public string ReturnSt (MethodDefinition method, Instruction instruc)
		{ 
			Hashtable opCodes = InitializeHashTable (method);
			string stOpCode = null;
			switch (instruc.OpCode.Code) {
			case Code.Ldloc_0:
				stOpCode = opCodes [0].ToString ();
				break;
			case Code.Ldloc_1:
				stOpCode = opCodes [1].ToString ();
				break;
			case Code.Ldloc_2:
				stOpCode = opCodes [2].ToString ();
				break;
			case Code.Ldloc_3:
				stOpCode = opCodes [3].ToString ();
				break;
			case Code.Ldloc_S:
				string [] s = instruc.Operand.ToString ().Split ('_');
				int i = Convert.ToInt32 (s [1]);
				stOpCode = opCodes [i].ToString ();
				break;
			case Code.Ldfld:
				TypeDefinition type = (TypeDefinition) method.DeclaringType;
				foreach (MethodDefinition ctor in type.Constructors)
					foreach (Instruction ctorins in ctor.Body.Instructions)
						if (ctorins.Operand !=null && ctorins.Operand.ToString () == instruc.Operand.ToString ())
							stOpCode = ctorins.Operand.ToString ();
				break;
			case Code.Ldsfld:
				stOpCode = String.Empty;
				break;
			}
			return stOpCode;
		}
		
		public Hashtable InitializeHashTable (MethodDefinition method)
		{
			int count = 0;
			Hashtable hash = new Hashtable ();			
			if (method.Name == "ToString") {
				foreach (Instruction ins in method.Body.Instructions) {
					if (ins.OpCode.Code.ToString().Substring(0,2) == "St") {
						if (ins.OpCode.Code == Code.Stloc_S) {
							hash.Add (count, ins.Operand.ToString ());
							count ++;
						} 
						else {
							hash.Add (count, ins.OpCode.Code.ToString ());
							count ++;
						}
					}
				}
			}
			return hash;
		}
		
		public bool MethodReturnsNull (MethodDefinition method, string code)
		{
			bool isNull = false;
			Instruction prevInstruc;
			foreach (Instruction ins in method.Body.Instructions) {
				prevInstruc = ins.Previous;
				if (ins.Operand != null && ins.Operand.ToString () == null && ins.OpCode == OpCodes.Stfld)
					isNull = false;
				else if (ins.OpCode.Code.ToString () == code || (ins.OpCode.ToString ().StartsWith ("st") && ins.Operand !=null && ins.Operand.ToString () == code)) {
					if (prevInstruc.OpCode.Code == Code.Ldnull)
						isNull = true;
					else if (prevInstruc.OpCode.Code == Code.Call || prevInstruc.OpCode.Code == Code.Callvirt) {
						code = ReturnSt (method, prevInstruc.Previous);
						isNull = MethodReturnsNull (method, code);
					}
				}
			}
			return isNull;
		}
	
	}
}