using System;
using Mono.Cecil;
using Gtk;

namespace Monoxide.Framework.PlugIns {

	public interface IPlugIn {

		void AddAssembly (AssemblyDefinition assembly);

		string Name { get; }

		IDisplay[] Displays { get; }
	}

	// displays (physical)

	public interface IDisplay {
		string Name { get; }
		bool Display { get; set; }
	}

	public interface IGraphicDisplay : IDisplay {
		void SetUp (Image image);
	}

	public interface ICustomDisplay : IDisplay {
		void SetUp (Notebook notebook);
	}

	// views (logical)

	public interface IView {
	}

	public interface IAssemblyView : IView {
		void Render (AssemblyDefinition assembly);
	}

	public interface ITypeView : IView {
		void Render (TypeDefinition type);
	}

	public interface IMethodView : IView {
		void Render (MethodDefinition method);
	}
}
