// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Ikadn.Utilities;

namespace Ikadn
{
	/// <summary>
	/// Helper class with utility methods for parsing and composing
	/// text streams with IKADN syntax.
	/// </summary>
	public class IkadnReader : IDisposable
	{
		/// <summary>
		/// Value returned by System.IO.TextReader methods when the end of the stream is reached.
		/// </summary>
		public const int EndOfStreamResult = -1;
		
		private const int HashSearchThreshold = 3;

		private readonly MultistreamTextReader reader;
		private readonly Dictionary<char, IIkadnObjectFactory> factories = new Dictionary<char, IIkadnObjectFactory>();

		private IkadnBaseObject bufferedObject = null;
		private bool recordIndentation = true;
		private readonly StringBuilder indentation = new StringBuilder();

		/// <summary>
		/// Wraps TextReader with IkadnReader
		/// </summary>
		/// <param name="reader">Input stream</param>
		public IkadnReader(TextReader reader) :
			this(new NamedStream[] { new NamedStream(reader, null)})
		{
			//No extra operation
		}

		/// <summary>
		/// Wraps one or more NamedStream instances with IkadnReader
		/// </summary>
		/// <param name="namedStreams">Name input streams</param>
		public IkadnReader(IEnumerable<NamedStream> namedStreams)
		{
			if (namedStreams == null)
				throw new ArgumentNullException(nameof(namedStreams));

			this.reader = new MultistreamTextReader(namedStreams);
		}

		/// <summary>
		/// Registers an object factory to the reader. If there is already
		/// a factory with the same sign, it will be replaced.
		/// </summary>
		/// <param name="factory">An object factory</param>
		public void RegisterFactory(IIkadnObjectFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			if (this.factories.ContainsKey(factory.Sign))
				this.factories[factory.Sign] = factory;
			else
				this.factories.Add(factory.Sign, factory);
		}

		#region Position report properties

		/// <summary>
		/// Index of the last read character.
		/// </summary>
		public int Index => this.reader.Index;
		/// <summary>
		/// Line of the last read character.
		/// </summary>
		public int Line => this.reader.Line;
		/// <summary>
		/// Column within the line of the last read character.
		/// </summary>
		public int Column => this.reader.Column;

		/// <summary>
		/// Name of the current input stream.
		/// </summary>
		public string StreamName => this.reader.StreamName;

		/// <summary>
		/// Leading whitespaces of currnet line.
		/// </summary>
		public string LineIndentation => this.indentation.ToString();

		/// <summary>
		/// Text that describes position (stream name, line, column and index) of the
		/// last successfuly read character from the stream.
		/// </summary>
		public string PositionDescription
		{
			get {
				if (this.reader.HasStream)
				{
					var name = this.StreamName != null ? "stream " + this.StreamName + ", " : "";
					return name + "line " + (Line + 1).ToString(CultureInfo.InvariantCulture) + 
						", column " + (Column + 1).ToString(CultureInfo.InvariantCulture) + 
						" (index: " + Index.ToString(CultureInfo.InvariantCulture) + ")";
				}
				else
					return "end of stream";
			}
		}

		#endregion

		#region IKADN stream peeking methods
		/// <summary>
		/// Indicates whether is it possible to read another character from the
		/// input stream.
		/// </summary>
		public bool HasNext
		{
			get
			{
				return this.reader.Peek() != MultistreamTextReader.EndOfStreamResult;
			}
		}

		/// <summary>
		/// Gets next character in the input stream without moving to the next.
		/// </summary>
		public char Peek()
		{
			return (char)this.reader.Peek();
		}

		/// <summary>
		/// Skips white space characters in the input stream and peeks the next character. 
		/// Throws System.FormatException if end of stream is reached before a non-white 
		/// character is found.
		/// </summary>
		/// <returns>A non-white character</returns>
		public char PeekNextNonwhite()
		{
			var skipResult = this.SkipWhiteSpaces();
			if (skipResult.EndOfStream)
				throw new FormatException();

			return this.Peek();
		}
		#endregion

		#region IKADN stream reading methods
		/// <summary>
		/// Reads next character from the input stream.
		/// </summary>
		/// <returns>Read character.</returns>
		public char Read()
		{
			if (this.reader.LastCharacter == '\n') {
				this.indentation.Length = 0;
				this.recordIndentation = true;
			}
			else if (this.reader.LastCharacter != MultistreamTextReader.None) {
				char c = (char)this.reader.LastCharacter;
				
				if (this.recordIndentation)
					if (char.IsWhiteSpace(c))
						this.indentation.Append(c);
					else
						this.recordIndentation = false;
			}

			return this.reader.Read();
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met. 
		/// </summary>
		/// <param name="acceptableCharacters">Characters that can be read</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(params char[] acceptableCharacters)
		{
			if (acceptableCharacters == null)
				throw new ArgumentNullException(nameof(acceptableCharacters));

			if (acceptableCharacters.Length == 0)
				throw new ArgumentException("No readable characters specified", nameof(acceptableCharacters));

			if (acceptableCharacters.Length < HashSearchThreshold)
				return this.ReadWhile(acceptableCharacters.Contains);
			else
				return this.ReadWhile(new HashSet<char>(acceptableCharacters).Contains);
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="acceptableCharacters">Set of characters that can be read</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(ICollection<char> acceptableCharacters)
		{
			if (acceptableCharacters == null)
				throw new ArgumentNullException(nameof(acceptableCharacters));
			if (acceptableCharacters.Count == 0)
				throw new ArgumentException("No readable characters specified", nameof(acceptableCharacters));

			return this.ReadWhile(acceptableCharacters.Contains);
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="readCondition">Set of characters that can be read</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(Predicate<char> readCondition)
		{
			if (readCondition == null)
				throw new ArgumentNullException(nameof(readCondition));

			return this.ReadConditionally(c =>
			{
				return new ReadingDecision((char)c, readCondition((char)c) ?
					CharacterAction.AcceptAsIs :
					CharacterAction.Stop
				);
			});
		}

		/// <summary>
		/// Reads characters from the input stream until one of terminating character
		/// is found. 
		/// </summary>
		/// <param name="terminatingCharacters">Characters that ends reading</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadUntil(params int[] terminatingCharacters)
		{
			if (terminatingCharacters == null)
				throw new ArgumentNullException(nameof(terminatingCharacters));
			if (terminatingCharacters.Length == 0)
				throw new ArgumentException("No terminating characters specified", nameof(terminatingCharacters));

			if (terminatingCharacters.Length < HashSearchThreshold)
				return this.ReadUntil(terminatingCharacters.Contains);
			else
				return this.ReadUntil(new HashSet<int>(terminatingCharacters).Contains);
		}

		/// <summary>
		/// Reads characters from the input stream until one of terminating character
		/// is found. 
		/// </summary>
		/// <param name="terminatingCondition">Characters that ends reading</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadUntil(Predicate<int> terminatingCondition)
		{
			if (terminatingCondition == null)
				throw new ArgumentNullException(nameof(terminatingCondition));

			return this.ReadConditionally(c =>
			{
				if (terminatingCondition(c))
					return new ReadingDecision((char)c, CharacterAction.Stop);
				else if (c == MultistreamTextReader.EndOfStreamResult)
					throw new EndOfStreamException("Unexpected end of stream at " + PositionDescription);
				else
					return new ReadingDecision((char)c, CharacterAction.AcceptAsIs);
			});
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="readingController">Returns whether character should be read
		/// (if predicate is true). When predicate evaluates to flase, reading stops.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadConditionally(Func<int, ReadingDecision> readingController)
		{
			if (readingController == null)
				throw new ArgumentNullException(nameof(readingController));

			var readChars = new StringBuilder();
			while (true) {
				int currentChar = this.reader.Peek();

				if (currentChar == MultistreamTextReader.EndOfStreamResult)
					return readChars.ToString();

				var action = readingController(currentChar);
				switch(action.Decision & CharacterAction.AllInputActions) {
					case CharacterAction.AcceptAsIs:
						readChars.Append(this.Read());
						break;
					case CharacterAction.Skip:
						this.Read();
						break;
					case CharacterAction.Substitute:
						readChars.Append(action.Character);
						this.Read();
						break;
				}

				if ((action.Decision & CharacterAction.Stop) != 0)
					return readChars.ToString();
			}
		}
		#endregion

		#region IKADN stream skipping methods
		/// <summary>
		/// Skips consequentive whitespace characters from the input stream. 
		/// </summary>
		/// <returns>Descrtipion of the skipping process.</returns>
		public SkipResult SkipWhiteSpaces()
		{
			return this.SkipWhile(char.IsWhiteSpace);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="skippableCharacters">Character(s) that should be skipped.</param>
		/// <returns>Descrtipion of the skipping process.</returns>
		public SkipResult SkipWhile(params char[] skippableCharacters)
		{
			if (skippableCharacters == null)
				throw new ArgumentNullException(nameof(skippableCharacters));
			if (skippableCharacters.Length == 0)
				throw new ArgumentException("No skippable characters specified", nameof(skippableCharacters));

			if (skippableCharacters.Length < HashSearchThreshold)
				return this.SkipWhile(skippableCharacters.Contains);
			else
				return this.SkipWhile(new HashSet<char>(skippableCharacters).Contains);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="skippableCharacters">Set of characters that should be skipped</param>
		/// <returns>Descrtipion of the skipping process.</returns>
		public SkipResult SkipWhile(ICollection<char> skippableCharacters)
		{
			if (skippableCharacters == null)
				throw new ArgumentNullException(nameof(skippableCharacters));
			if (skippableCharacters.Count == 0)
				throw new ArgumentException("No skippable characters specified", nameof(skippableCharacters));

			return this.SkipWhile(skippableCharacters.Contains);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="skipCondition">Returns whether character should be skipped
		/// (if predicate is true). When predicate evaluates to flase, process stops.</param>
		/// <returns>Descrtipion of the skipping process.</returns>
		public SkipResult SkipWhile(Predicate<char> skipCondition)
		{
			if (skipCondition == null)
				throw new ArgumentNullException(nameof(skipCondition));

			StringBuilder skipped = new StringBuilder();
			while (true) {
				int currentChar = this.reader.Peek();

				if (currentChar == MultistreamTextReader.EndOfStreamResult)
					return new SkipResult(skipped.ToString(), true);

				if (skipCondition((char)currentChar))
					skipped.Append(this.Read());
				else
					return new SkipResult(skipped.ToString(), false);
			}
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="terminatingCharacters">Character that stop skipping process</param>
		public SkipResult SkipUntil(params int[] terminatingCharacters)
		{
			if (terminatingCharacters == null)
				throw new ArgumentNullException(nameof(terminatingCharacters));
			if (terminatingCharacters.Length == 0)
				throw new ArgumentException("No terminating characters specified", nameof(terminatingCharacters));

			if (terminatingCharacters.Length < HashSearchThreshold)
				return this.SkipUntil(terminatingCharacters.Contains);
			else
				return this.SkipUntil(new HashSet<int>(terminatingCharacters).Contains);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="terminatingCharacters">Set of characters that stop skipping process</param>
		public SkipResult SkipUntil(ICollection<int> terminatingCharacters)
		{
			if (terminatingCharacters == null)
				throw new ArgumentNullException(nameof(terminatingCharacters));
			if (terminatingCharacters.Count == 0)
				throw new ArgumentException("No terminating characters specified", nameof(terminatingCharacters));

			return this.SkipUntil(terminatingCharacters.Contains);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="stopCondition">Returns whether the terminating condition
		/// is met (if predicate is true).</param>
		public SkipResult SkipUntil(Predicate<int> stopCondition)
		{
			if (stopCondition == null)
				throw new ArgumentNullException(nameof(stopCondition));

			var skipped = new StringBuilder();
			while (true) {
				int currentChar = this.reader.Peek();

				if (!stopCondition(currentChar))
					skipped.Append(this.Read());
				else if (currentChar == MultistreamTextReader.EndOfStreamResult)
					return new SkipResult(skipped.ToString(), true);
				else
					return new SkipResult(skipped.ToString(), false);
			}
		}
		#endregion

		#region IKADN object reading methods
		/// <summary>
		/// Checks whether the reader can read another IKADN object from input
		/// stream.
		/// </summary>
		/// <returns>True if it is possible.</returns>
		public bool HasNextObject()
		{
			this.tryReadObject();

			return this.bufferedObject != null;
		}

		/// <summary>
		/// Reads next IKADN object from input streams.
		/// </summary>
		/// <returns>IKADN object</returns>
		public IkadnBaseObject ReadObject()
		{
			this.tryReadObject();

			if (this.bufferedObject == null)
				throw new FormatException("Trying to read IKADN object beyond end of stream.");

			var result = this.bufferedObject;
			this.bufferedObject = null;

			return result;
		}

		private void tryReadObject()
		{
			if (this.bufferedObject != null || this.SkipWhiteSpaces().EndOfStream)
				return;

			char sign = this.Read();
			if (!this.factories.ContainsKey(sign))
				throw new FormatException("No factory defined for an object starting with " + sign + " at " + this.PositionDescription + ".");

			this.bufferedObject = this.factories[sign].Parse(this);
		}
		#endregion

		#region Disposable pattern
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
		#endregion
	}
}
