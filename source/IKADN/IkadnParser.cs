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

			foreach (var factory in factories)
				this.Reader.RegisterFactory(factory);
		}
		
		/// <summary>
		/// Parses whole input stream.
		/// </summary>
		/// <returns>Queue of parsed IKADN objects.</returns>
		public TaggableQueue<object, IkadnBaseObject> ParseAll()
		{
			var queue = this.bufferedObjects;
			this.bufferedObjects = new TaggableQueue<object, IkadnBaseObject>();

			while (this.HasNext())
			{
				var dataObj = this.ParseNext();
				queue.Enqueue(dataObj.Tag, dataObj);
			}

			return queue;
		}

		/// <summary>
		/// Checks whether parser can produce more IKADN objects, either buffered
		/// or by reading from input streams.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasNext()
		{
			return this.bufferedObjects.Count != 0 || this.Reader.HasNextObject();
		}

		/// <summary>
		/// Checks whether the parser can read more IKADN objects with a specific
		/// tag from the input stream.
		/// </summary>
		/// <param name="tag">Desired object tag</param>
		/// <returns>True if it is possible.</returns>
		public bool HasNext(object tag)
		{
			while (this.bufferedObjects.CountOf(tag) == 0)
				if (this.Reader.HasNextObject())
				{
					var dataObj = this.Reader.ReadObject();
					this.bufferedObjects.Enqueue(dataObj.Tag, dataObj);
				}
				else
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
			if (!this.HasNext())
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + this.Reader.PositionDescription + ".");

			if (this.bufferedObjects.Count != 0)
				return this.bufferedObjects.Dequeue();

			return this.Reader.ReadObject();
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
			if (!this.HasNext(tag))
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + this.Reader.PositionDescription + ".");

			return this.bufferedObjects.Dequeue(tag);
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
	}
}
