// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN object factory for named object references.
	/// </summary>
	public class ReferencedFactory : AIkonFactory
	{
		/// <summary>
		/// Sign for IKADN object reference.
		/// </summary>
		public static readonly char OpeningSign = '#';

		private readonly Func<string, IkadnBaseObject> resolver;

		/// <summary>
		/// Sign for IKADN object reference.
		/// </summary>
		public override char Sign => OpeningSign;

		/// <summary>
		/// Constructs IKADN object factory for named object references.
		/// </summary>
		/// <param name="resolver">Function for resolving IKADN object from a name</param>
		public ReferencedFactory(Func<string, IkadnBaseObject> resolver, Action<string, IkadnBaseObject> registerName)
			: base(registerName)
		{
			this.resolver = resolver;
		}

		/// <summary>
		/// Parses input for a IKON reference name.
		/// </summary>
		/// <param name="reader">IKADN parser instance.</param>
		/// <returns>Referenced IKADN object.</returns>
		protected override IkadnBaseObject ParseObject(IkadnReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			return this.resolver(IkonParser.ReadIdentifier(reader));
		}
	}
}
