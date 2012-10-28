using System;
using System.Collections.Generic;
using System.IO;
using Ikon.Utilities;

namespace Ikon
{
	/// <summary>
	/// Base class for IKON values.
	/// </summary>
	public abstract class Value
	{
		/// <summary>
		/// Type name of the IKON value instance.
		/// </summary>
		public abstract string TypeName
		{
			get;
		}

		/// <summary>
		/// Writes value's content to the output stream.
		/// </summary>
		/// <param name="writer">Wrapped around target output stream.</param>
		protected abstract void DoCompose(IkonWriter writer);

		/// <summary>
		/// Writes an IKON value to the output stream.
		/// </summary>
		/// <param name="writer">Wrapped around target output stream.</param>
		/// <param name="referenceNames">List of reference names.</param>
		public void Compose(IkonWriter writer, params string[] referenceNames)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			DoCompose(writer);

			if (referenceNames != null && referenceNames.Length > 0)
				writer.WriteReferences(referenceNames);
			
			writer.EndLine();
		}
	}
}
