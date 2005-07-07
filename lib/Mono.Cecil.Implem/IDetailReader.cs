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

	internal interface IDetailReader {

		void ReadSemantic (EventDefinition evt);
		void ReadSemantic (PropertyDefinition prop);

		void ReadMarshalSpec (ParameterDefinition param);
		void ReadMarshalSpec (FieldDefinition field);

		void ReadLayout (TypeDefinition type);
		void ReadLayout (FieldDefinition field);

		void ReadConstant (FieldDefinition field);
		void ReadConstant (PropertyDefinition prop);
		void ReadConstant (ParameterDefinition param);
	}
}
