﻿// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Ikadn.Ikon.Types;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN object factory for IKON arrays.
	/// </summary>
	public class ArrayFactory : AIkonFactory
	{
		/// <summary>
		/// Sign for IKADN array.
		/// </summary>
		public static readonly char OpeningSign = '[';

		/// <summary>
		/// Closing character for IKON array in textual
		/// representation.
		/// </summary>
		public static readonly char ClosingChar = ']';

		/// <summary>
		/// Sign for IKADN array.
		/// </summary>
		public override char Sign => OpeningSign;

		public ArrayFactory(Action<string, IkadnBaseObject> registerName) : base(registerName)
		{
			//no extra operation
		}

		/// <summary>
		/// Parses input for a IKADN object.
		/// </summary>
		/// <param name="reader">IKADN parser instance.</param>
		/// <returns>IKADN object generated by factory.</returns>
		protected override IkadnBaseObject ParseObject(IkadnReader reader)
		{
			if (reader == null)
				throw new System.ArgumentNullException("reader");

			var values = new List<IkadnBaseObject>();

			while (reader.PeekNextNonwhite() != ClosingChar)
			{
				string startPosition = reader.PositionDescription;
				if (!reader.HasNextObject())
					throw new FormatException("Characters from " + startPosition + " to " + reader.PositionDescription + " couldn't be parsed as IKADN value");

				values.Add(reader.ReadObject());
			}
			reader.Read();

			return new IkonArray(values);
		}
	}
}
