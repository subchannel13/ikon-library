using System;
using System.Collections.Generic;
using System.IO;
using Ikadn;
using Ikadn.Ikon.Factories;
using Ikadn.Utilities;
using System.Text;
using Ikadn.Ikon.Values;

namespace Ikadn.Ikon
{
	/// <summary>
	/// Parser that can parse input with IKON syntax.
	/// </summary>
	public class Parser : Ikadn.Parser
	{
		/// <summary>
		/// Character that marks the beginning of the reference name.
		/// </summary>
		public const char ReferenceSign = '@';

		/// <summary>
		/// Collection of named objects.
		/// </summary>
		protected IDictionary<string, IkadnBaseValue> NamedValues { get; private set; }

		/// <summary>
		/// Constructs IKON parser with default IKON value factories.
		/// </summary>
		/// <param name="reader"></param>
		public Parser(TextReader reader)
			: base(reader, new IValueFactory[] {
 			new ObjectFactory(),
			new TextFactory(), 
			new NumericFactory(),
			new ArrayFactory(),
			new ReferencedFactory() })
		{
			this.NamedValues = new Dictionary<string, IkadnBaseValue>();
		}

		/// <summary>
		/// Constructs IKON parser and registers addoditonal value factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKADN syntax.</param>
		/// <param name="factories">Collection of value factories.</param>
		public Parser(TextReader reader, IEnumerable<IValueFactory> factories)
			: this(reader)
		{
			if (factories == null)
				throw new ArgumentNullException("factories");

			foreach (var factory in factories)
				RegisterFactory(factory);
		}

		/// <summary>
		/// Trys to parse next IKADN value from the input stream. 
		/// 
		/// Throws System.FormatException if there is no value factory
		/// that can parse curren state of the input.
		/// </summary>
		/// <returns>Return an IKADN value if there is one, null otherwise.</returns>
		protected override IkadnBaseValue TryParseNext()
		{
			IkadnBaseValue value = base.TryParseNext();
			if (value == null)
				return null;

			if (ReferenceValue.ValueTypeName.Equals(value.Tag))
				return GetNamedValue(value.To<string>());

			while (this.Reader.SkipWhiteSpaces() != ReaderDoneReason.EndOfStream &&
					this.Reader.Peek() == ReferenceSign) {
				this.Reader.Read();
				NamedValues.Add(ReadIdentifier(this.Reader), value);
			}

			return value;
		}

		/// <summary>
		/// Returns the IKADN value with specified reference name. 
		/// 
		/// Throws System.Collections.Generic.KeyNotFoundException 
		/// if such value doesn't exist.
		/// </summary>
		/// <param name="name">Name of the value reference</param>
		/// <returns>Desired IKADN value.</returns>
		public IkadnBaseValue GetNamedValue(string name)
		{
			if (NamedValues.ContainsKey(name))
				return NamedValues[name];
			else
				throw new KeyNotFoundException("Value named '" + name + "' not found");
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
