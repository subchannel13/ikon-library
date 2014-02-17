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
		/// <summary>
		/// Value returned by System.IO.TextReader methods when the end of the stream is reached.
		/// </summary>
		public const int EndOfStreamResult = -1;

		private TextReader reader;
		private int lastCharacter = EndOfStreamResult;

		private bool recordIndentation = true;
		private StringBuilder indentation = new StringBuilder();

		/// <summary>
		/// Wraps TextReader with IkonReader
		/// </summary>
		/// <param name="reader">Input stream</param>
		public IkadnReader(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			this.reader = reader;
			this.Index = 0;
			this.Line = 0;
			this.Column = 0;
		}

		#region Position report properties

		/// <summary>
		/// Index of the last read character.
		/// </summary>
		public int Index { get; private set; }
		/// <summary>
		/// Line of the last read character.
		/// </summary>
		public int Line { get; private set; }
		/// <summary>
		/// Column within the line of the last read character.
		/// </summary>
		public int Column { get; private set; }

		/// <summary>
		/// Leading whitespaces of currnet line.
		/// </summary>
		public string LineIndentation 
		{
			get
			{
				return indentation.ToString();
			}
		}

		/// <summary>
		/// Gets text that describes position (line, column and index) of the last
		/// successfuly read character from the stream.
		/// </summary>
		public string PositionDescription
		{
			get {
				return "line " + (Line + 1) + ", column " + (Column + 1) + " (index: " + Index + ")";
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
			get { return reader.Peek() != EndOfStreamResult; }
		}

		/// <summary>
		/// Gets next character in the input stream without moving to the next.
		/// </summary>
		public char Peek()
		{
			return (char)reader.Peek();
		}

		/// <summary>
		/// Skips white space characters in the input stream and peeks the next character. 
		/// Throws System.FormatException if end of stream is reached before a non-white 
		/// character is found.
		/// </summary>
		/// <returns>A non-white character</returns>
		public char PeekNextNonwhite()
		{
			var skipResult = SkipWhiteSpaces();
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
			if (lastCharacter != EndOfStreamResult)
				Index++;

			if (lastCharacter == '\n') {
				Line++;
				Column = 0;
				indentation.Length = 0;
				recordIndentation = true;
			}
			else if (lastCharacter != EndOfStreamResult) {
				char c = (char)lastCharacter;
				
				if (!char.IsControl(c))
					Column++;

				if (recordIndentation)
					if (char.IsWhiteSpace(c))
						indentation.Append(c);
					else
						recordIndentation = false;
			}

			lastCharacter = reader.Read();
			return (char)lastCharacter;
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

			return ReadWhile(new HashSet<char>(acceptableCharacters).Contains);
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

			return ReadWhile(acceptableCharacters.Contains);
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="readCondition">Set of characters that can be read.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(Predicate<char> readCondition)
		{
			if (reader == null)
				throw new ArgumentNullException("readCondition");

			return ReadConditionally(c =>
			{
				return new ReadingDecision((char)c, (readCondition((char)c)) ?
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
			return ReadUntil(terminatingCharactersSet.Contains);
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

			return ReadConditionally(c =>
			{
				if (terminatingCondition(c))
					return new ReadingDecision((char)c, CharacterAction.Stop);
				else if (c == EndOfStreamResult)
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

			StringBuilder readChars = new StringBuilder();
			while (true) {
				int currentChar = reader.Peek();

				if (currentChar == EndOfStreamResult)
					return readChars.ToString();

				ReadingDecision action = readingController(currentChar);
				switch(action.Decision & CharacterAction.AllInputActions) {
					case CharacterAction.AcceptAsIs:
						readChars.Append(Read());
						break;
					case CharacterAction.Skip:
						Read();
						break;
					case CharacterAction.Substitute:
						readChars.Append(action.Character);
						Read();
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
			return SkipWhile(char.IsWhiteSpace);
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

			return SkipWhile(new HashSet<char>(skippableCharacters).Contains);
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

			return SkipWhile(skippableCharacters.Contains);
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
				int currentChar = reader.Peek();

				if (currentChar == EndOfStreamResult)
					return new SkipResult(skipped.ToString(), true);

				if (skipCondition((char)currentChar))
					skipped.Append(Read());
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

			return SkipUntil(new HashSet<int>(terminatingCharacters).Contains);
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

			return SkipUntil(terminatingCharacters.Contains);
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

			StringBuilder skipped = new StringBuilder();
			while (true) {
				int currentChar = reader.Peek();

				if (!stopCondition(currentChar))
					skipped.Append(Read());
				else if (currentChar == EndOfStreamResult)
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
			reader.Dispose();
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
		#endregion
	}
}
