using System;
using System.Collections.Generic;
using System.IO;
using Ikadn.Utilities;

namespace Ikadn
{
	/// <summary>
	/// Base class for IKADN values.
	/// </summary>
	public abstract class IkadnBaseValue
	{
		/// <summary>
		/// Type name of the IKADN value instance.
		/// </summary>
		public abstract object Tag
		{
			get;
		}

		/// <summary>
		/// Converts IKADN value to specified type.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public abstract T To<T>();

		/// <summary>
		/// Writes value's content to the output stream.
		/// </summary>
		/// <param name="writer">Wrapped around target output stream.</param>
		protected abstract void DoCompose(IkadnWriter writer);

		/// <summary>
		/// Writes an IKADN value to the output stream.
		/// </summary>
		/// <param name="writer">Wrapped around target output stream.</param>
		public void Compose(IkadnWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			DoCompose(writer);
			
			writer.EndLine();
		}
	}
}
