// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

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
		private LabeledQueue<object, IkadnBaseObject> bufferedObjects = new LabeledQueue<object, IkadnBaseObject>();

		private readonly IkadnReader reader;

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
		/// <param name="reader">Input stream with IKADN syntax</param>
		/// <param name="factories">Collection of object factories</param>
		public IkadnParser(TextReader reader, IEnumerable<IIkadnObjectFactory> factories)
			: this(new[] { new NamedStream(reader, null) }, factories)
		{
			//no extra operation
		}

		/// <summary>
		/// Constructs parser without registerd object factories.
		/// </summary>
		/// <param name="streams">Named input streams with IKADN syntax</param>
		public IkadnParser(IEnumerable<NamedStream> streams)
			: this(streams, new IIkadnObjectFactory[0])
		{
			//no extra operation
		}

		/// <summary>
		/// Constructs parser with multiple input documents and registers object
		/// factories to it.
		/// </summary>
		/// <param name="streams">Named input  streams with IKADN syntax</param>
		/// <param name="factories">Collection of object factories</param>
		public IkadnParser(IEnumerable<NamedStream> streams, IEnumerable<IIkadnObjectFactory> factories)
		{
			if (streams == null)
				throw new ArgumentNullException(nameof(streams));
			if (factories == null)
				throw new ArgumentNullException(nameof(factories));

			this.reader = new IkadnReader(streams);

			foreach (var factory in factories)
				this.reader.RegisterFactory(factory);
		}

		/// <summary>
		/// Parses input streams to the end.
		/// </summary>
		/// <returns>Queue of parsed IKADN objects.</returns>
		public LabeledQueue<object, IkadnBaseObject> ParseAll()
		{
			var queue = this.bufferedObjects;
			this.bufferedObjects = new LabeledQueue<object, IkadnBaseObject>();

			while (this.HasNext())
			{
				var dataObj = this.ParseNext();
				queue.Enqueue(dataObj.Tag, dataObj);
			}

			return queue;
		}

		/// <summary>
		/// Checks whether the parser can produce more IKADN objects.
		/// 
		/// Objects can either be read from input streams or are already buffered.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasNext()
		{
			return this.bufferedObjects.Count != 0 || this.reader.HasNextObject();
		}

		/// <summary>
		/// Checks whether the parser can produce more IKADN objects with a
		/// specific label.
		/// 
		/// Objects can either be read from input streams or are already buffered.
		/// </summary>
		/// <param name="label">Desired object label</param>
		/// <returns>True if it is possible.</returns>
		public bool HasNext(object label)
		{
			while (this.bufferedObjects.CountOf(label) == 0)
				if (this.reader.HasNextObject())
				{
					var dataObj = this.reader.ReadObject();
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
		/// the input streams is encountered while parsing.
		/// </summary>
		/// <returns>An IKADN object.</returns>
		public IkadnBaseObject ParseNext()
		{
			if (!this.HasNext())
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + this.reader.PositionDescription + ".");

			if (this.bufferedObjects.Count != 0)
				return this.bufferedObjects.Dequeue();

			return this.reader.ReadObject();
		}

		/// <summary>
		/// Produces next IKADN object with specific label, either buffered or
		/// by parsing from input streams.
		/// 
		/// Throws System.IO.EndOfStreamException if end of
		/// the input streams is encountered while parsing.
		/// </summary>
		/// <param name="labels">Desired object label</param>
		/// <returns>An IKADN object</returns>
		public IkadnBaseObject ParseNext(object labels)
		{
			if (!this.HasNext(labels))
				throw new EndOfStreamException("Trying to read beyond the end of stream. Last read character was at " + this.reader.PositionDescription + ".");

			return this.bufferedObjects.Dequeue(labels);
		}

		/// <summary>
		/// Registers an object factory to the parser. If there is already
		/// a factory with the same sign, it will be replaced.
		/// </summary>
		/// <param name="factory">An IKADN object factory</param>
		protected virtual void RegisterFactory(IIkadnObjectFactory factory)
		{
			this.reader.RegisterFactory(factory);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the System.IO.TextReader and optionally
		/// releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; 
		/// false to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			this.reader.Dispose();
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
