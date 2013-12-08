using System;
using System.Collections.Generic;
using System.IO;

using Ikadn.Utilities;

namespace Ikadn
{
	/// <summary>
	/// Basic parser for texts streams with IKADN syntax.
	/// </summary>
	public class IkadnParser : IDisposable
	{
		private TaggableQueue<object, IkadnBaseObject> bufferedObjects = new TaggableQueue<object,IkadnBaseObject>();

		/// <summary>
		/// Collection on object factories.
		/// </summary>
		protected IDictionary<char, IIkadnObjectFactory> Factories { get; private set; }

		/// <summary>
		/// Input stream that is being parsed.
		/// </summary>
		public IkadnReader Reader { get; private set; }

		/// <summary>
		/// Constructs parser without registerd object factories.
		/// </summary>
		/// <param name="reader">Input stream with IKADN syntax</param>
		public IkadnParser(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			this.Reader = new IkadnReader(reader);

			this.Factories = new Dictionary<char, IIkadnObjectFactory>();
		}

		/// <summary>
		/// Constructs parser and registers object factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKADN syntax.</param>
		/// <param name="factories">Collection of object factories.</param>
		public IkadnParser(TextReader reader, IEnumerable<IIkadnObjectFactory> factories)
			: this(reader)
		{
			if (factories == null)
				throw new ArgumentNullException("factories");

			foreach (var factory in factories)
				RegisterFactory(factory);
		}

		/// <summary>
		/// Registers an object factory to the parser. If parser already
		/// has a factory with the same sign, it will be replaced.
		/// </summary>
		/// <param name="factory">An object factory.</param>
		public void RegisterFactory(IIkadnObjectFactory factory)
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
		/// <returns>Queue of parsed IKADN objects.</returns>
		public TaggableQueue<object, IkadnBaseObject> ParseAll()
		{
			var queue = new TaggableQueue<object, IkadnBaseObject>(bufferedObjects);

			while (HasNext()) {
				var dataObj = ParseNext();
				queue.Enqueue(dataObj.Tag, dataObj);
			}

			return queue;
		}

		/// <summary>
		/// Checks whether parser can read more IKADN objects from the input stream.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasNext()
		{
			if (this.bufferedObjects.Count == 0) {
				var dataObj = this.TryParseNext();
				if (dataObj != null)
					this.bufferedObjects.Enqueue(dataObj.Tag, dataObj);
			}

			return (this.bufferedObjects.Count != 0);
		}

		/// <summary>
		/// Parses and returns next IKADN object from the input stream. 
		/// 
		/// Throws System.IO.EndOfStreamException if end of
		/// the input stream is encountered while parsing.
		/// </summary>
		/// <returns>An IKADN object</returns>
		public IkadnBaseObject ParseNext()
		{
			if (this.bufferedObjects.Count > 0)
				return this.bufferedObjects.Dequeue();

			IkadnBaseObject res = this.TryParseNext();

			if (res == null)
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + Reader.PositionDescription + ".");
			
			return res;
		}

		/// <summary>
		/// Parses and returns next IKADN object from the input stream. 
		/// 
		/// Throws System.IO.EndOfStreamException if end of
		/// the input stream is encountered while parsing.
		/// </summary>
		/// <param name="tag">Desired object tag</param>
		/// <returns>An IKADN object</returns>
		public IkadnBaseObject ParseNext(object tag)
		{
			while (this.bufferedObjects.CountOf(tag) == 0) {
				IkadnBaseObject dataObj = this.TryParseNext();
				
				if (dataObj == null)
					throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + Reader.PositionDescription + ".");

				bufferedObjects.Enqueue(dataObj.Tag, dataObj);
			}
				
			return this.bufferedObjects.Dequeue(tag);
		}

		/// <summary>
		/// Trys to parse next IKADN object from the input stream. Returns null 
		/// if there is none (end of input stream reached).
		/// 
		/// Throws System.FormatException if there is no object factory
		/// that can parse current state of the input.
		/// </summary>
		/// <returns>An IKADN object if there is one, null otherwise.</returns>
		protected virtual IkadnBaseObject TryParseNext()
		{
			ReaderDoneReason skipResult = this.Reader.SkipWhiteSpaces();

			if (skipResult == ReaderDoneReason.EndOfStream)
				return null;

			char sign = Reader.Read();
			if (!Factories.ContainsKey(sign))
				throw new FormatException("No factory defined for an object starting with " + sign + " at " + Reader.PositionDescription + ".");

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
