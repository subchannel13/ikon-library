using System;
using System.Collections.Generic;
using System.IO;
using Ikon.Utilities;

namespace Ikon
{
	/// <summary>
	/// Basic parser for texts streams with IKON syntax.
	/// </summary>
	public class Parser : IDisposable
	{
		private Value lastTryValue = null;
		/// <summary>
		/// Collection on value factories.
		/// </summary>
		protected IDictionary<char, IValueFactory> Factories { get; private set; }

		/// <summary>
		/// Input stream that is being parsed.
		/// </summary>
		public IkonReader Reader { get; private set; }

		/// <summary>
		/// Constructs parser without registerd value factories.
		/// </summary>
		/// <param name="reader">Input stream with IKON syntax</param>
		public Parser(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			this.Reader = new IkonReader(reader);

			this.Factories = new Dictionary<char, IValueFactory>();
		}

		/// <summary>
		/// Constructs parser and registers value factories to it.
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
		/// Registers a value factory to the parser. If parser already
		/// has a factory with the same sign, it will be replaced.
		/// </summary>
		/// <param name="factory">A value factory.</param>
		public void RegisterFactory(IValueFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			if (this.Factories.ContainsKey(factory.Sign))
				this.Factories[factory.Sign] = factory;
			else
				this.Factories.Add(factory.Sign, factory);
		}

		/// <summary>
		/// Parses whole input stream.
		/// </summary>
		/// <returns>Queue of parsed IKON values.</returns>
		public ValueQueue ParseAll()
		{
			ValueQueue values = new ValueQueue();

			while (HasNext())
				values.Enqueue(ParseNext());

			return values;
		}

		/// <summary>
		/// Checks whether parser can read more IKON values from the input stream.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasNext()
		{
			this.lastTryValue = this.lastTryValue ?? this.TryParseNext();

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
			Value res = this.lastTryValue ?? this.TryParseNext();
			this.lastTryValue = null;

			if (res == null) throw new EndOfStreamException();
			
			return res;
		}

		/// <summary>
		/// Trys to parse next IKON value from the input stream. 
		/// 
		/// Throws System.FormatException if there is no value factory
		/// that can parse curren state of the input.
		/// </summary>
		/// <returns>Return an IKON value if there is one, null otherwise.</returns>
		protected virtual Value TryParseNext()
		{
			WhiteSpaceSkipResult skipResult = this.Reader.SkipWhiteSpaces();

			if (skipResult == WhiteSpaceSkipResult.EndOfStream)
				return null;

			char sign = Reader.Read();
			if (!Factories.ContainsKey(sign)) throw new FormatException("No factory defined for a value starting with " + sign);

			return Factories[sign].Parse(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the System.IO.TextReader and optionally
		/// releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; 
		/// false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			Reader.Dispose();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
