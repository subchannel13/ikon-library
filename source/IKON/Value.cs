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
		/// Converts IKON value to specified type.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public abstract T To<T>();

		/// <summary>
		/// Writes value's content to the output stream.
		/// </summary>
		/// <param name="writer">Wrapped around target output stream.</param>
		protected abstract void DoCompose(IkonWriter writer);

		/// <summary>
		/// Writes an IKON value to the output stream.
		/// </summary>
		/// <param name="writer">Wrapped around target output stream.</param>
		public void Compose(IkonWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			DoCompose(writer);
			
			writer.EndLine();
		}
	}
}
