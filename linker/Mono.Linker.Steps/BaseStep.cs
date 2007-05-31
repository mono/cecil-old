using Mono.Cecil;

namespace Mono.Linker.Steps {

	public abstract class BaseStep : IStep {

		private LinkContext _context;

		public LinkContext Context {
			get { return _context; }
		}

		public void Process (LinkContext context)
		{
			_context = context;

			if (!ConditionToProcess ())
				return;

			Process ();

			foreach (AssemblyDefinition assembly in context.GetAssemblies ())
				ProcessAssembly (assembly);
		}

		protected virtual bool ConditionToProcess ()
		{
			return true;
		}

		protected virtual void Process ()
		{
		}

		protected virtual void ProcessAssembly (AssemblyDefinition assembly)
		{
		}
	}
}
