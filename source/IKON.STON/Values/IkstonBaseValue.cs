﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ikon.Ston.Values
{
	/// <summary>
	/// Base class for IKSTON values. Add reference names on top the IKON values.
	/// </summary>
	public abstract class IkstonBaseValue : Value
	{
		private HashSet<string> referenceNames = new HashSet<string>();

		/// <summary>
		/// Set of objects that can be used as reference to the value.
		/// </summary>
		public ISet<string> ReferenceNames
		{
			get { return referenceNames; }
		}

		/// <summary>
		/// Writes value's reference names (if any) to the output stream.
		/// </summary>
		/// <param name="writer">Wrapped around target output stream.</param>
		protected void WriteReferences(IkonWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			if (referenceNames != null && referenceNames.Count > 0)
				foreach (string name in referenceNames)
					writer.Write(" " + Parser.ReferenceSign + name);
		}
	}
}
