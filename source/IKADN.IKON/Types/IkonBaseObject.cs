// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// Base class for IKON objects. Adds reference names on top the IKADN objects.
	/// </summary>
	public abstract class IkonBaseObject : IkadnBaseObject
	{
		/// <summary>
		/// Character that marks the beginning of the reference name (anchor).
		/// </summary>
		public static readonly char AnchorSign = '@';

		private readonly HashSet<string> referenceNames = new HashSet<string>();

		/// <summary>
		/// Set of names that can be used as reference to the object.
		/// </summary>
		public ICollection<string> ReferenceNames
		{
			get { return this.referenceNames; }
		}

		/// <summary>
		/// Writes object's reference names (if any) to the output stream.
		/// </summary>
		/// <param name="writer">Wrapper around the target output stream.</param>
		protected void WriteReferences(IkadnWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (this.referenceNames != null && this.referenceNames.Count > 0)
				foreach (string name in this.referenceNames)
					writer.Write(" " + AnchorSign + name);
		}
	}
}
