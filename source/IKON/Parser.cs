using System;
using System.Collections.Generic;
using System.IO;
using IKON.Utils;

namespace IKON
{
	/// <summary>
	/// Basic parser for texts streams with IKON syntax.
	/// </summary>
	public class Parser : IDisposable
	{
		/// <summary>
		/// IKON value parsed from input when testing whether input contains anoter IKON value.
		/// </summary>
		protected Value lastTryValue = null;
		/// <summary>
		/// Collection on value factories.
		/// </summary>
		protected IDictionary<char, IValueFactory> factories = new Dictionary<char, IValueFactory>();
		/// <summary>
		/// Collection of named objects.
		/// </summary>
		protected IDictionary<string, Value> namedValues = new Dictionary<string, Value>();

		/// <summary>
		/// Input stream that is being parsed.
		/// </summary>
		protected TextReader reader;

		/// <summary>
		/// Constructs parser without registerd value factories.
		/// </summary>
		/// <param name="reader">Input stream with IKON syntax</param>
		public Parser(TextReader reader)
		{
			this.reader = reader;
		}

		/// <summary>
		/// Constructs parser and registers value factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKON syntax.</param>
		/// <param name="factories">Collection of value factories.</param>
		public Parser(TextReader reader, IEnumerable<IValueFactory> factories)
			: this(reader)
		{ 
			foreach (var factory in factories)
				RegisterFactory(factory);
		}

		/// <summary>
		/// Registers a value factory to the parser. If parser already
		/// has a factory with the same sign, it will be replaced.
		/// </summary>
		/// <param name="factory">A value factory.</param>
		public void RegisterFactory(IValueFactory factory)
		{
			if (this.factories.ContainsKey(factory.Sign))
				this.factories[factory.Sign] = factory;
			else
				this.factories.Add(factory.Sign, factory);
		}

		/// <summary>
		/// Parses whole input stream.
		/// </summary>
		/// <returns>Queue of parsed IKON values.</returns>
		public Queue<Value> ParseAll()
		{
			Queue<Value> res = new Queue<Value>();

			while (HasNext())
				res.Enqueue(ParseNext());

			return res;
		}

		/// <summary>
		/// Checks whether parser can read more IKON values from the input stream.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasNext()
		{
			this.lastTryValue = this.lastTryValue ?? this.tryParseNext();

			return (this.lastTryValue != null);
		}

		/// <summary>
		/// Parses and returns next IKON value from the input stream. 
		/// 
		/// Throws System.IO.EndOfStreamException if end of
		/// the input stream is encountered while parsing.
		/// </summary>
		/// <returns>An IKON value</returns>
		public Value ParseNext()
		{
			Value res = this.lastTryValue ?? this.tryParseNext();
			this.lastTryValue = null;

			if (res == null) throw new EndOfStreamException();
			
			return res;
		}

		/// <summary>
		/// Indicates whether is it possible to read another character from the
		/// input stream.
		/// </summary>
		public bool CanRead
		{
			get { return reader.Peek() != HelperMethods.EndOfStreamInt; }
		}

		/// <summary>
		/// Gets next character in the input stream without moving to the next.
		/// </summary>
		public char PeakReader
		{
			get { return (char)reader.Peek(); }
		}

		/// <summary>
		/// Reads next character from the input stream.
		/// </summary>
		/// <returns>Read character.</returns>
		public char ReadChar()
		{
			return (char)reader.Read();
		}

		/// <summary>
		/// Skips consequentive whitespace characters from the input stream. 
		/// </summary>
		public void SkipWhitespaces()
		{
			HelperMethods.SkipWhitespaces(reader);
		}

		/// <summary>
		/// Reads the input stream for an IKON identifier. Throws System.FormatException if there is
		/// no valid identifier.
		/// </summary>
		/// <returns>An identifier name</returns>
		public string ReadIdentifier()
		{
			return HelperMethods.ParseIdentifier(reader);
		}

		/// <summary>
		/// Reads the input stream for the next non-whitespace character. Throws System.FormatException 
		/// if end of stream is reached before a such character.
		/// </summary>
		/// <returns>A non-white character</returns>
		public char ReadNextNonWhite()
		{
			return HelperMethods.NextNonWhite(reader);
		}

		/// <summary>
		/// Returns the IKON value with specified reference name. 
		/// 
		/// Throws System.Collections.Generic.KeyNotFoundException 
		/// if such value doesn't exist.
		/// </summary>
		/// <param name="name">Name of the value reference</param>
		/// <returns>Desired IKON value.</returns>
		public Value GetNamedValue(string name)
		{
			if (namedValues.ContainsKey(name))
				return namedValues[name];
			else
				throw new KeyNotFoundException("Value named '" + name + "' not found");
		}

		/// <summary>
		/// Trys to parse next IKON value from the input stream. 
		/// 
		/// Throws System.FormatException if there is no value factory
		/// that can parse curren state of the input.
		/// </summary>
		/// <returns>Return an IKON value if there is one, null otherwise.</returns>
		protected Value tryParseNext()
		{
			WhitespecSkipResult skipResult = HelperMethods.SkipWhitespaces(this.reader);

			if (skipResult == WhitespecSkipResult.EndOfStream)
				return null;

			char sign = (char)reader.Read();
			if (!factories.ContainsKey(sign)) throw new FormatException("No factory defined for a value starting with " + sign);

			Value res = factories[sign].Parse(this);

			foreach (string refernceName in HelperMethods.ParseReferences(this.reader))
				namedValues.Add(refernceName, res);

			return res;
		}

		public virtual void Dispose()
		{
			reader.Close();
		}
	}
}
