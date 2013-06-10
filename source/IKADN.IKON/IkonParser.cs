using System;
using System.Collections.Generic;
using System.IO;
using Ikadn;
using Ikadn.Ikon.Factories;
using Ikadn.Utilities;
using System.Text;
using Ikadn.Ikon.Types;

namespace Ikadn.Ikon
{
	/// <summary>
	/// Parser that can parse input with IKON syntax.
	/// </summary>
	public class IkonParser : Ikadn.IkadnParser
	{
		/// <summary>
		/// Character that marks the beginning of the reference name.
		/// </summary>
		public const char ReferenceSign = '@';

		/// <summary>
		/// Collection of named objects.
		/// </summary>
		protected IDictionary<string, IkadnBaseObject> NamedObjects { get; private set; }

		/// <summary>
		/// Constructs IKON parser with default IKON object factories.
		/// </summary>
		/// <param name="reader"></param>
		public IkonParser(TextReader reader)
			: base(reader, new IIkadnObjectFactory[] {
 			new ObjectFactory(),
			new TextFactory(), 
			new NumericFactory(),
			new ArrayFactory(),
			new ReferencedFactory() })
		{
			this.NamedObjects = new Dictionary<string, IkadnBaseObject>();
		}

		/// <summary>
		/// Constructs IKON parser and registers additional object factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKADN syntax.</param>
		/// <param name="factories">Collection of object factories.</param>
		public IkonParser(TextReader reader, IEnumerable<IIkadnObjectFactory> factories)
			: this(reader)
		{
			if (factories == null)
				throw new ArgumentNullException("factories");

			foreach (var factory in factories)
				RegisterFactory(factory);
		}

		/// <summary>
		/// Trys to parse next IKADN object from the input stream. 
		/// 
		/// Throws System.FormatException if there is no object factory
		/// that can parse current state of the input.
		/// </summary>
		/// <returns>Return an IKADN object if there is one, null otherwise.</returns>
		protected override IkadnBaseObject TryParseNext()
		{
			IkadnBaseObject dataObj = base.TryParseNext();
			if (dataObj == null)
				return null;

			if (IkonReference.TypeTag.Equals(dataObj.Tag))
				return GetNamedObject(dataObj.To<string>());

			while (this.Reader.SkipWhiteSpaces() != ReaderDoneReason.EndOfStream &&
					this.Reader.Peek() == ReferenceSign) {
				this.Reader.Read();
				NamedObjects.Add(ReadIdentifier(this.Reader), dataObj);
			}

			return dataObj;
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
			if (NamedObjects.ContainsKey(name))
				return NamedObjects[name];
			else
				throw new KeyNotFoundException("Object named '" + name + "' not found");
		}

		private static ISet<char> IdentifierChars = DefineIdentifierChars();

		/// <summary>
		/// Reads the input stream for an IKON identifier. Throws System.FormatException if there is
		/// no valid identifier.
		/// </summary>
		/// <returns>An identifier name</returns>
		public static string ReadIdentifier(IkadnReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (reader.SkipWhiteSpaces() == ReaderDoneReason.EndOfStream)
				throw new EndOfStreamException("Unexpected end of stream at " + reader.PositionDescription + " while reading IKON identifier.");

			string identifier = reader.ReadWhile(IdentifierChars);
			if (identifier.Length == 0)
				throw new FormatException("Unexpected character at " + reader.PositionDescription + ", while reading IKON identifier");

			return identifier;
		}

		private static HashSet<char> DefineIdentifierChars()
		{
			HashSet<char> res = new HashSet<char>();

			res.Add('_');
			for (char c = 'a'; c <= 'z'; c++) res.Add(c);
			for (char c = 'A'; c <= 'Z'; c++) res.Add(c);
			for (char c = '0'; c <= '9'; c++) res.Add(c);

			return res;
		}
	}
}
