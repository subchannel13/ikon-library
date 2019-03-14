using System;
using System.Collections.Generic;
using System.IO;
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
		private readonly MultistreamTextReader reader;

		private bool recordIndentation = true;
		private StringBuilder indentation = new StringBuilder();

		/// <summary>
		/// Wraps TextReader with IkonReader
		/// </summary>
		/// <param name="reader">Input stream</param>
		public IkadnReader(TextReader reader) : 
			this(new NamedStream[] { new NamedStream(reader, null)})
		{
			//No extra operation
		}

		/// <summary>
		/// Wraps one or more NamedStream instances with IkonReader
		/// </summary>
		/// <param name="namedStreams"></param>
		public IkadnReader(IEnumerable<NamedStream> namedStreams)
		{
			if (namedStreams == null)
				throw new ArgumentNullException("namedStreams");

			this.reader = new MultistreamTextReader(namedStreams);
		}

		#region Position report properties

		/// <summary>
		/// Index of the last read character.
		/// </summary>
		public int Index { get { return this.reader.Index; } }
		/// <summary>
		/// Line of the last read character.
		/// </summary>
		public int Line { get { return this.reader.Line; } }
		/// <summary>
		/// Column within the line of the last read character.
		/// </summary>
		public int Column { get { return this.reader.Column; } }

		/// <summary>
		/// Name of the current reader.
		/// </summary>
		public string StreamName { get { return this.reader.StreamName; } }

		/// <summary>
		/// Leading whitespaces of currnet line.
		/// </summary>
		public string LineIndentation 
		{
			get
			{
				return this.indentation.ToString();
			}
		}

		/// <summary>
		/// Gets text that describes position (line, column and index) of the last
		/// successfuly read character from the stream.
		/// </summary>
		public string PositionDescription
		{
			get {
				if (this.reader.HasStream)
				{
					var name = this.StreamName != null ? "stream " + this.StreamName + ", " : "";
					return name + "line " + (Line + 1) + ", column " + (Column + 1) + " (index: " + Index + ")";
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
		/// <param name="acceptableCharacters">Characters that can be read.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(params char[] acceptableCharacters)
		{
			if (acceptableCharacters == null)
				throw new ArgumentNullException("acceptableCharacters");

			if (acceptableCharacters.Length == 0)
				throw new ArgumentException("No readable characters specified", "acceptableCharacters");

			return this.ReadWhile(new HashSet<char>(acceptableCharacters).Contains);
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="acceptableCharacters">Set of characters that can be read.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(ICollection<char> acceptableCharacters)
		{
			if (acceptableCharacters == null)
				throw new ArgumentNullException("acceptableCharacters");
			if (acceptableCharacters.Count == 0)
				throw new ArgumentException("No readable characters specified", "acceptableCharacters");

			return this.ReadWhile(acceptableCharacters.Contains);
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="readCondition">Set of characters that can be read.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(Predicate<char> readCondition)
		{
			if (readCondition == null)
				throw new ArgumentNullException("readCondition");

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
		/// <param name="terminatingCharacters">Characters that ends reading.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadUntil(params int[] terminatingCharacters)
		{
			if (terminatingCharacters == null)
				throw new ArgumentNullException("terminatingCharacters");
			if (terminatingCharacters.Length == 0)
				throw new ArgumentException("No terminating characters specified", "terminatingCharacters");

			var terminatingCharactersSet = new HashSet<int>(terminatingCharacters);
			return this.ReadUntil(terminatingCharactersSet.Contains);
		}

		/// <summary>
		/// Reads characters from the input stream until one of terminating character
		/// is found. 
		/// </summary>
		/// <param name="terminatingCondition">Characters that ends reading.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadUntil(Predicate<int> terminatingCondition)
		{
			if (terminatingCondition == null)
				throw new ArgumentNullException("terminatingCondition");

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
				throw new ArgumentNullException("readingController");

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
				throw new ArgumentNullException("skippableCharacters");
			if (skippableCharacters.Length == 0)
				throw new ArgumentException("No skippable characters specified", "skippableCharacters");

			return this.SkipWhile(new HashSet<char>(skippableCharacters).Contains);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="skippableCharacters">Set of characters that should be skipped.</param>
		/// <returns>Descrtipion of the skipping process.</returns>
		public SkipResult SkipWhile(ICollection<char> skippableCharacters)
		{
			if (skippableCharacters == null)
				throw new ArgumentNullException("skippableCharacters");
			if (skippableCharacters.Count == 0)
				throw new ArgumentException("No skippable characters specified", "skippableCharacters");

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
				throw new ArgumentNullException("skipCondition");

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
		/// <param name="terminatingCharacters">Character that stop skipping process.</param>
		public SkipResult SkipUntil(params int[] terminatingCharacters)
		{
			if (terminatingCharacters == null)
				throw new ArgumentNullException("terminatingCharacters");
			if (terminatingCharacters.Length == 0)
				throw new ArgumentException("No terminating characters specified", "terminatingCharacters");

			return this.SkipUntil(new HashSet<int>(terminatingCharacters).Contains);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="terminatingCharacters">Set of characters that stop skipping process.</param>
		public SkipResult SkipUntil(ICollection<int> terminatingCharacters)
		{
			if (terminatingCharacters == null)
				throw new ArgumentNullException("terminatingCharacters");
			if (terminatingCharacters.Count == 0)
				throw new ArgumentException("No terminating characters specified", "terminatingCharacters");

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
				throw new ArgumentNullException("stopCondition");

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
