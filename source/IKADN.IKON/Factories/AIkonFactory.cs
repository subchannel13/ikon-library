﻿// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using Ikadn.Ikon.Types;
using System;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// Base IKON object factory class that automatically parses object anchors.
	/// </summary>
	public abstract class AIkonFactory : IIkadnObjectFactory
	{
		private readonly Action<string, IkadnBaseObject> registerName;

		/// <summary>
		/// Constructs Ikadn.Ikon.Factories.AIkonFactory with anchor registration
		/// callback.
		/// </summary>
		/// <param name="registerName">Callback for registering IKADN object anchor</param>
		protected AIkonFactory(Action<string, IkadnBaseObject> registerName)
		{
			this.registerName = registerName;
		}

		/// <summary>
		/// Character that identifies an object parsable by the factory.
		/// </summary>
		public abstract char Sign { get; }

		/// <summary>
		/// Parses IKADN object and it's anchors from input streams.
		/// </summary>
		/// <param name="reader">IKADN reader instance</param>
		/// <returns>IKADN object generated by factory.</returns>
		public IkadnBaseObject Parse(IkadnReader reader)
		{
			var parsedObject = this.ParseObject(reader);

			while (!reader.SkipWhiteSpaces().EndOfStream && reader.Peek() == IkonBaseObject.AnchorSign)
			{
				reader.Read();
				this.registerName(IkonParser.ReadIdentifier(reader), parsedObject);
			}

			return parsedObject;
		}

		/// <summary>
		/// Parses IKADN object type specific to this factory.
		/// </summary>
		/// <param name="reader">IKADN reader instance</param>
		/// <returns>IKADN object generated by factory.</returns>
		protected abstract IkadnBaseObject ParseObject(IkadnReader reader);
	}
}
