/*
 * Location.cs: an encapsulation of several pieces of information used
 * to identify a location in an assembly.
 *
 * Authors:
 *   Aaron Tomb <atomb@soe.ucsc.edu>
 *   Sebastien Pouliot  <sebastien@ximian.com>
 * 
 * Copyright (c) 2005 Aaron Tomb
 * Copyright (C) 2007 Novell, Inc (http://www.novell.com)
 *
 * This is free software, distributed under the MIT/X11 license.
 * See the included LICENSE.MIT file for details.
 **********************************************************************/

using System;
using System.Text;

using Mono.Cecil;

namespace Gendarme.Framework {

	public class Location {
		private string type;
		private string method;
		private int offset;	// Offset of instruction into method

		public Location (string type, string method)
		{
			this.type = type;
			this.method = method;
			this.offset = -1;
		}

		public Location (string type, string method, int offset)
		{
			this.type = type;
			this.method = method;
			this.offset = offset;
		}

		public Location (TypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			this.type = type.FullName;
			this.method = String.Empty;
			this.offset = -1;
		}

		public Location (MethodDefinition method) :
			this (method, -1)
		{
		}

		public Location (MethodDefinition method, int offset)
		{
			if (method == null)
				throw new ArgumentNullException ("method");

			this.type = method.DeclaringType.FullName;
			this.method = method.Name;
			this.offset = offset;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();

			if (!String.IsNullOrEmpty (type))
				sb.Append (type);

			if (!String.IsNullOrEmpty (method)) {
				if (sb.Length > 0)
					sb.Append ("::");
				sb.Append (method);
			}

			if (offset >= 0) {
				if (sb.Length > 0)
					sb.Append (":");
				sb.AppendFormat ("{0:x4}", offset);
			}

			return sb.ToString ();
	        }
	}
}

