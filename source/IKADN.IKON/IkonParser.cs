// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

using Ikadn.Ikon.Factories;
using Ikadn.Utilities;

namespace Ikadn.Ikon
{
	/// <summary>
	/// Parser that can parse input with IKON syntax.
	/// </summary>
	public class IkonParser : IkadnParser
	{
		private readonly IDictionary<string, IkadnBaseObject> namedObjects = new Dictionary<string, IkadnBaseObject>();

		/// <summary>
		/// Constructs IKON parser with default IKON object factories.
		/// </summary>
		/// <param name="reader"></param>
		public IkonParser(TextReader reader)
			: this(new[] { new NamedStream(reader, null) }, new AIkonFactory[0])
		{
			//no extra operation
		}

		/// <summary>
		/// Constructs IKON parser with multiple input documents and default IKON
		/// object factories.
		/// </summary>
		/// <param name="streams">Input named streams with IKADN syntax.</param>
		public IkonParser(IEnumerable<NamedStream> streams)
			: this(streams, new AIkonFactory[0])
		{
			//no extra operation
		}

		/// <summary>
		/// Constructs IKON parser and registers additional object factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKADN syntax.</param>
		/// <param name="factories">Collection of object factories.</param>
		public IkonParser(TextReader reader, IEnumerable<AIkonFactory> factories)
			: this(new[] { new NamedStream(reader, null) }, factories)
		{
			//no extra operation
		}

		/// <summary>
		/// Constructs IKON parser with multiple input documents and registers
		/// additional object factories to it.
		/// </summary>
		/// <param name="streams">Input named streams with IKADN syntax.</param>
		/// <param name="factories">Collection of object factories.</param>
		public IkonParser(IEnumerable<NamedStream> streams, IEnumerable<AIkonFactory> factories)
			: base(streams)
		{
			if (factories == null)
				throw new ArgumentNullException(nameof(factories));

			this.namedObjects = new Dictionary<string, IkadnBaseObject>();

			base.RegisterFactory(new ArrayFactory(this.registerName));
			base.RegisterFactory(new CompositeFactory(this.registerName));
			base.RegisterFactory(new NumericFactory(this.registerName));
			base.RegisterFactory(new ReferencedFactory(x => this.namedObjects[x], this.registerName));
			base.RegisterFactory(new TextFactory(this.registerName));
			base.RegisterFactory(new TextBlockFactory(this.registerName));

			foreach (var factory in factories)
				base.RegisterFactory(factory);
		}

		/// <summary>
		/// Returns the IKADN object with specified reference name. 
		/// 
		/// Throws System.Collections.Generic.KeyNotFoundException 
		/// if such object doesn't exist.
		/// </summary>
		/// <param name="name">Name of the object reference</param>
		/// <returns>Desired IKADN object.</returns>
		public IkadnBaseObject GetNamedObject(string name)
		{
			if (namedObjects.ContainsKey(name))
				return namedObjects[name];
			else
				throw new KeyNotFoundException("Object named '" + name + "' not found");
		}

		/// <summary>
		/// Reads the input stream for an IKON identifier. Throws System.FormatException if there is
		/// no valid identifier.
		/// </summary>
		/// <returns>An identifier name</returns>
		public static string ReadIdentifier(IkadnReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			if (reader.SkipWhiteSpaces().EndOfStream)
				throw new EndOfStreamException("Unexpected end of stream at " + reader.PositionDescription + " while reading IKON identifier.");

			string identifier = reader.ReadWhile(IdentifierChars);
			if (identifier.Length == 0)
				throw new FormatException("Unexpected character at " + reader.PositionDescription + ", while reading IKON identifier");

			return identifier;
		}

		/// <summary>
		/// Do not use this overload, use one accepting Ikadn.Ikon.Factories.AIkonFactory
		/// instead.
		/// </summary>
		/// <param name="factory">An IKADN object factory</param>
		protected sealed override void RegisterFactory(IIkadnObjectFactory factory)
		{
			throw new NotSupportedException("Method overload not supported, use overload with " + nameof(AIkonFactory) + " instead");
		}

		/// <summary>
		/// Registers an object factory to the parser. If there is already
		/// a factory with the same sign, it will be replaced.
		/// </summary>
		/// <param name="factory">An IKON object factory</param>
		protected virtual void RegisterFactory(AIkonFactory factory)
		{
			base.RegisterFactory(factory);
		}

		private void registerName(string name, IkadnBaseObject ikadnObj)
		{
			this.namedObjects[name] = ikadnObj;
		}

		private static readonly ICollection<char> IdentifierChars = defineIdentifierChars();

		private static HashSet<char> defineIdentifierChars()
		{
			var res = new HashSet<char> { '_' };

			for (char c = 'a'; c <= 'z'; c++) res.Add(c);
			for (char c = 'A'; c <= 'Z'; c++) res.Add(c);
			for (char c = '0'; c <= '9'; c++) res.Add(c);

			return res;
		}
	}
}
