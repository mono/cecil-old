/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using Mono.Cecil;

	internal abstract class BaseReflectionReader : BaseReflectionVisitor, IDetailReader {

		public virtual void ReadSemantic (EventDefinition evt)
		{
		}

		public virtual void ReadSemantic (PropertyDefinition prop)
		{
		}

		public virtual void ReadMarshalSpec (ParameterDefinition param)
		{
		}

		public virtual void ReadMarshalSpec (FieldDefinition field)
		{
		}

		public virtual void ReadLayout (TypeDefinition type)
		{
		}

		public virtual void ReadLayout (FieldDefinition field)
		{
		}

		public virtual void ReadConstant (FieldDefinition field)
		{
		}

		public virtual void ReadConstant (PropertyDefinition prop)
		{
		}

		public virtual void ReadConstant (ParameterDefinition param)
		{
		}

		public virtual void ReadInitialValue (FieldDefinition field)
		{
		}
	}
}
