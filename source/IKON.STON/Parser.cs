using System;
using System.Collections.Generic;
using System.IO;
using Ikon;
using Ikon.Ston.Factories;
using Ikon.Utilities;
using System.Text;
using Ikon.Ston.Values;

namespace Ikon.Ston
{
	/// <summary>
	/// Parser that can parse input with IKSTON syntax.
	/// </summary>
	public class Parser : Ikon.Parser
	{
		/// <summary>
		/// Character that marks the beginning of the reference name.
		/// </summary>
		public const char ReferenceSign = '@';

		/// <summary>
		/// Collection of named objects.
		/// </summary>
		protected IDictionary<string, IkonBaseValue> NamedValues { get; private set; }

		/// <summary>
		/// Constructs IKSTON parser with default IKSTON value factories.
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
			this.NamedValues = new Dictionary<string, IkonBaseValue>();
		}

		/// <summary>
		/// Constructs IKSTON parser and registers addoditonal value factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKON syntax.</param>
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
		/// Trys to parse next IKON value from the input stream. 
		/// 
		/// Throws System.FormatException if there is no value factory
		/// that can parse curren state of the input.
		/// </summary>
		/// <returns>Return an IKON value if there is one, null otherwise.</returns>
		protected override IkonBaseValue TryParseNext()
		{
			IkonBaseValue value = base.TryParseNext();
			if (value == null)
				return null;

			if (value.TypeName == ReferenceValue.ValueTypeName)
				return GetNamedValue(value.To<string>());

			while (this.Reader.SkipWhiteSpaces() != ReaderDoneReason.EndOfStream &&
					this.Reader.Peek() == ReferenceSign) {
				this.Reader.Read();
				NamedValues.Add(ReadIdentifier(this.Reader), value);
			}

			return value;
		}

		/// <summary>
		/// Returns the IKON value with specified reference name. 
		/// 
		/// Throws System.Collections.Generic.KeyNotFoundException 
		/// if such value doesn't exist.
		/// </summary>
		/// <param name="name">Name of the value reference</param>
		/// <returns>Desired IKON value.</returns>
		public IkonBaseValue GetNamedValue(string name)
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
		public static string ReadIdentifier(IkonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (reader.SkipWhiteSpaces() == ReaderDoneReason.EndOfStream)
				throw new EndOfStreamException("Unexpected end of stream at " + reader.PositionDescription + " while reading IKSTON identifier.");

			string identifier = reader.ReadWhile(IdentifierChars);
			if (identifier.Length == 0)
				throw new FormatException("Unexpected character at " + reader.PositionDescription + ", while reading IKSTON identifier");

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
