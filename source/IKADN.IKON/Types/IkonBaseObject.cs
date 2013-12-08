using System;
using System.Collections.Generic;
using System.Linq;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// Base class for IKON objects. Adds reference names on top the IKADN objects.
	/// </summary>
	public abstract class IkonBaseObject : IkadnBaseObject
	{
		private HashSet<string> referenceNames = new HashSet<string>();

		/// <summary>
		/// Set of names that can be used as reference to the object.
		/// </summary>
		public ISet<string> ReferenceNames
		{
			get { return referenceNames; }
		}

		/// <summary>
		/// Writes object's reference names (if any) to the output stream.
		/// </summary>
		/// <param name="writer">Wrapper around the target output stream.</param>
		protected void WriteReferences(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			if (referenceNames != null && referenceNames.Count > 0)
				foreach (string name in referenceNames)
					writer.Write(" " + IkonParser.ReferenceSign + name);
		}
	}
}
