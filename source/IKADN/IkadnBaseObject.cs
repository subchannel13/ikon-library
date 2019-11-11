// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// LGPL License. See License.txt in the project root for license information.

using System;

namespace Ikadn
{
	/// <summary>
	/// Base class for IKADN objects.
	/// </summary>
	public abstract class IkadnBaseObject
	{
		/// <summary>
		/// Tag of the IKADN object.
		/// </summary>
		public abstract object Tag
		{
			get;
		}

		/// <summary>
		/// Converts IKADN object to specified type.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public abstract T To<T>();

		/// <summary>
		/// Writes objects's content to the output stream.
		/// </summary>
		/// <param name="writer">Wrapper around target output stream</param>
		protected abstract void DoCompose(IkadnWriter writer);

		/// <summary>
		/// Writes an IKADN object to the output stream.
		/// </summary>
		/// <param name="writer">Wrapper around target output stream</param>
		public void Compose(IkadnWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			DoCompose(writer);
			
			writer.EndLine();
		}
	}
}
