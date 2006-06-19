using System;
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gendarme.Rules.Exceptions {
	
	internal class SEHGuardedBlock : ISEHGuardedBlock {
	
		private IInstruction start;
		private IInstruction end;
		private SEHHandlerBlockCollection handler_blocks;
		
		public SEHGuardedBlock ()
		{
			handler_blocks = new SEHHandlerBlockCollection ();
		}
		
		public IInstruction Start {
			get { return start; }
			set { start = value; }			
		}

		public IInstruction End {
			get { return end; }
			set { end = value; }
		}
		
		public ISEHHandlerBlock[] SEHHandlerBlocks {
			get {
				ISEHHandlerBlock[] ret =
					new ISEHHandlerBlock [handler_blocks.Count];
				handler_blocks.CopyTo (ret, 0);
				return ret;
			}
		}

		public SEHHandlerBlockCollection SEHHandlerBlocksInternal
		{
			get { return handler_blocks; }
		}
	}
}
