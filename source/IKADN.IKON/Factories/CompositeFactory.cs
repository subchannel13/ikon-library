﻿// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using Ikadn.Ikon.Types;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN object factory for composite IKON objects.
	/// </summary>
	public class CompositeFactory : AIkonFactory
	{
		/// <summary>
		/// Sign for IKADN composite object.
		/// </summary>
		public static readonly char OpeningSign = '{';

		/// <summary>
		/// Closing character for IKON composite object in textual
		/// representation.
		/// </summary>
		public static readonly char ClosingChar = '}';

		/// <summary>
		/// Sign for IKADN composite object.
		/// </summary>
		public override char Sign => OpeningSign;

		/// <summary>
		/// Constructs IKON composite object factory.
		/// </summary>
		/// <param name="registerName">Callback for registering IKADN object anchor</param>
		public CompositeFactory(Action<string, IkadnBaseObject> registerName) : base(registerName)
		{
			//no extra operation
		}

		/// <summary>
		/// Parses input for a IKADN object.
		/// </summary>
		/// <param name="reader">IKADN parser instance.</param>
		/// <returns>IKADN value generated by factory.</returns>
		protected override IkadnBaseObject ParseObject(IkadnReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			var res = new IkonComposite(IkonParser.ReadIdentifier(reader));

			while (reader.PeekNextNonwhite() != ClosingChar)
			{
				string memberName = IkonParser.ReadIdentifier(reader);
				
				string startPosition = reader.PositionDescription;
				if (reader.HasNextObject())
					res[memberName] = reader.ReadObject();
				else
					throw new FormatException("Characters from " + startPosition + " to " + reader.PositionDescription + " couldn't be parsed as IKADN value");
			}
			reader.Read();

			return res;
		}
	}
}
