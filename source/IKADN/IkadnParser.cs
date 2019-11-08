// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// LGPL License. See License.txt in the project root for license information.

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
		private TaggableQueue<object, IkadnBaseObject> bufferedObjects = new TaggableQueue<object, IkadnBaseObject>();
		private Queue<IkadnBaseObject> bufferedNextObjects = new Queue<IkadnBaseObject>();

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
			: this(new[] { new NamedStream(reader, null) }, new IIkadnObjectFactory[0])
		{
			//no extra operation
		}

		/// <summary>
		/// Constructs parser and registers object factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKADN syntax.</param>
		/// <param name="factories">Collection of object factories.</param>
		public IkadnParser(TextReader reader, IEnumerable<IIkadnObjectFactory> factories)
			: this(new[] { new NamedStream(reader, null) }, factories)
		{
			//no extra operation
		}

		/// <summary>
		/// Constructs parser with multiple input documents and registers object
		/// factories to it.
		/// </summary>
		/// <param name="streams">Input named streams with IKADN syntax.</param>
		/// <param name="factories">Collection of object factories.</param>
		public IkadnParser(IEnumerable<NamedStream> streams, IEnumerable<IIkadnObjectFactory> factories)
		{
			if (streams == null)
				throw new ArgumentNullException("streams");
			if (factories == null)
				throw new ArgumentNullException("factories");

			this.Reader = new IkadnReader(streams);

			this.Factories = new Dictionary<char, IIkadnObjectFactory>();

			foreach (var factory in factories)
				this.RegisterFactory(factory);
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
			var queue = this.bufferedObjects;
			this.bufferedObjects = new TaggableQueue<object, IkadnBaseObject>();
			this.bufferedNextObjects = new Queue<IkadnBaseObject>();

			while (this.HasMore())
			{
				var dataObj = this.ParseNext();
				queue.Enqueue(dataObj.Tag, dataObj);
			}

			return queue;
		}

		/// <summary>
		/// Checks whether parser can read more IKADN objects from input streams.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasNext()
		{
			this.TryParseNext();

			return this.bufferedNextObjects.Count != 0;
		}

		/// <summary>
		/// Checks whether parser can produce more IKADN objects, either buffered
		/// or by reading from input streams.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasMore()
		{
			if (this.bufferedObjects.Count == 0)
				this.TryParseNext();

			return this.bufferedObjects.Count != 0;
		}

		/// <summary>
		/// Checks whether the parser can read more IKADN objects with a specific
		/// tag from the input stream.
		/// </summary>
		/// <param name="tag">Desired object tag</param>
		/// <returns>True if it is possible.</returns>
		public bool HasMore(object tag)
		{
			while (this.bufferedObjects.CountOf(tag) == 0)
				if (this.tryParseMore() == null)
					return false;

			return true;
		}

		/// <summary>
		/// Produces next IKADN object, either buffered or by parsing a next one
		/// from input streams.
		/// 
		/// Throws System.IO.EndOfStreamException if end of
		/// the input stream is encountered while parsing.
		/// </summary>
		/// <returns>An IKADN object.</returns>
		public IkadnBaseObject ParseNext()
		{
			if (!this.HasMore())
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + this.Reader.PositionDescription + ".");

			return this.dequeBuffered(null);
		}

		/// <summary>
		/// Parses and returns next IKADN object from the input stream. 
		/// 
		/// Throws System.IO.EndOfStreamException if end of
		/// the input stream is encountered while parsing.
		/// </summary>
		/// <returns>An IKADN object.</returns>
		public IkadnBaseObject ParseImmediateNext()
		{
			if (this.bufferedNextObjects.Count > 0)
				return this.dequeBufferedNext();

			if (this.HasNext())
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + this.Reader.PositionDescription + ".");

			return this.dequeBufferedNext();
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
			if (!this.HasMore(tag))
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + this.Reader.PositionDescription + ".");

			return this.dequeBuffered(tag);
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
			if (this.bufferedNextObjects.Count != 0)
				return this.bufferedNextObjects.Peek();

			return this.tryParseMore();
		}

		/// <summary>
		/// Releases the unmanaged resources used by the System.IO.TextReader and optionally
		/// releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; 
		/// false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			this.Reader.Dispose();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private IkadnBaseObject tryParseMore()
		{
			var skipResult = this.Reader.SkipWhiteSpaces();

			if (skipResult.EndOfStream)
				return null;

			char sign = this.Reader.Read();
			if (!this.Factories.ContainsKey(sign))
				throw new FormatException("No factory defined for an object starting with " + sign + " at " + this.Reader.PositionDescription + ".");

			var dataObj = this.Factories[sign].Parse(this);
			this.bufferedNextObjects.Enqueue(dataObj);
			this.bufferedObjects.Enqueue(dataObj.Tag, dataObj);

			return dataObj;
		}

		private IkadnBaseObject dequeBuffered(object tag)
		{
			var dataObj = this.bufferedObjects.Dequeue(tag);
			if (this.bufferedNextObjects.Peek() == dataObj)
				this.bufferedNextObjects.Dequeue();

			return dataObj;
		}

		private IkadnBaseObject dequeBufferedNext()
		{
			var dataObj = this.bufferedNextObjects.Dequeue();
			this.bufferedObjects.Remove(dataObj);

			return dataObj;
		}
	}
}
