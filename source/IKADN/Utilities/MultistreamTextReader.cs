// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Ikadn.Utilities
{
	sealed class MultistreamTextReader : IDisposable
	{
		public const int EndOfStreamResult = -1;
		public const int None = -2;

		private readonly IEnumerator<NamedStream> readers;
		private NamedStream currentReader;

		public MultistreamTextReader(IEnumerable<NamedStream> readers)
		{
			this.readers = readers.GetEnumerator();
			this.LastCharacter = None;
			this.nextReader();
		}

		public int LastCharacter { get; private set; }
		public int Index { get; private set; }
		public int Line { get; private set; }
		public int Column { get; private set; }

		public string StreamName
		{
			get { return this.currentReader?.StreamName; }
		}

		public int Peek()
		{
			this.skipEmptyReaders();

			if (this.currentReader != null)
				return this.currentReader.Stream.Peek();
			else
				return EndOfStreamResult;
		}

		public char Read()
		{
			if (this.LastCharacter != None)
				this.Index++;

			if (this.LastCharacter == '\n')
			{
				this.Line++;
				this.Column = 0;
			}
			else if (this.LastCharacter != None && !char.IsControl((char)this.LastCharacter))
			{
				Column++;
			}

			this.skipEmptyReaders();
			this.LastCharacter = this.currentReader.Stream.Read();
			return (char)this.LastCharacter;
		}

		public bool HasStream { get { return this.currentReader != null; } }

		public void Dispose()
		{
			while (this.currentReader != null)
				this.nextReader();

			this.readers.Dispose();
		}

		private void nextReader()
		{
			if (this.currentReader != null)
				this.currentReader.Stream.Dispose();

			this.currentReader = this.readers.MoveNext() ? this.readers.Current : null;
			this.Index = 0;
			this.Line = 0;
			this.Column = 0;
			this.LastCharacter = None;
		}

		private void skipEmptyReaders()
		{
			while (this.currentReader != null && this.currentReader.Stream.Peek() == EndOfStreamResult)
				this.nextReader();
		}
	}
}
