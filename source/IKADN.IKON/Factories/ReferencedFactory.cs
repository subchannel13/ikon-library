// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// LGPL License. See License.txt in the project root for license information.

using System;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN object factory for named object references.
	/// </summary>
	public class ReferencedFactory : IIkadnObjectFactory
	{
		/// <summary>
		/// Sign for IKADN object reference.
		/// </summary>
		public static readonly char OpeningSign = '#';

		private readonly Func<string, IkadnBaseObject> resolver;

		/// <summary>
		/// Sign for IKADN object reference.
		/// </summary>
		public char Sign
		{
			get { return OpeningSign; }
		}

		/// <summary>
		/// Constructs IKADN object factory for named object references.
		/// </summary>
		/// <param name="resolver">Function for resolving IKADN object from a name</param>
		public ReferencedFactory(Func<string, IkadnBaseObject> resolver)
		{
			this.resolver = resolver;
		}

		/// <summary>
		/// Parses input for a IKON reference name.
		/// </summary>
		/// <param name="parser">IKADN parser instance.</param>
		/// <returns>Referenced IKADN object.</returns>
		public IkadnBaseObject Parse(Ikadn.IkadnParser parser)
		{
			if (parser == null)
				throw new System.ArgumentNullException("parser");

			return this.resolver(IkonParser.ReadIdentifier(parser.Reader));
		}
	}
}
