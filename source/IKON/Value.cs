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
		/// Writes an IKON value to the composer.
		/// </summary>
		/// <param name="composer">Target composer.</param>
		public abstract void Compose(Composer composer);
	}
}
