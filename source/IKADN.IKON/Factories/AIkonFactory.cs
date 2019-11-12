// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using Ikadn.Ikon.Types;
using System;

namespace Ikadn.Ikon.Factories
{
	public abstract class AIkonFactory : IIkadnObjectFactory
	{
		private readonly Action<string, IkadnBaseObject> registerName;

		protected AIkonFactory(Action<string, IkadnBaseObject> registerName)
		{
			this.registerName = registerName;
		}

		public abstract char Sign { get; }

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

		protected abstract IkadnBaseObject ParseObject(IkadnReader reader);
	}
}
