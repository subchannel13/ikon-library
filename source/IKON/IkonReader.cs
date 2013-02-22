using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ikon.Utilities;

namespace Ikon
{
	/// <summary>
	/// Helper class with utility methods for parsing and composing
	/// text streams with IKON syntax.
	/// </summary>
	public class IkonReader : IDisposable
	{
		/// <summary>
		/// Value returned by System.IO.TextReader methods when the end of the stream is reached.
		/// </summary>
		private const int EndOfStreamResult = -1;

		private TextReader reader;
		private int lastCharacter = EndOfStreamResult;

		/// <summary>
		/// Wraps TextReader with IkonReader
		/// </summary>
		/// <param name="reader">Input stream</param>
		public IkonReader(TextReader reader)
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
		/// Gets text that describes position (line, column and index) of the last
		/// successfuly read character from the stream.
		/// </summary>
		public string PositionDescription
		{
			get {
				return "line " + Line + ", column " + Column + " (index: " + Index + ")";
			}
		}

		#endregion

		#region IKON stream peeking methods
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
		/// character i found.
		/// </summary>
		/// <returns>A non-white character</returns>
		public char PeekNextNonwhite()
		{
			if (SkipWhiteSpaces() == ReaderDoneReason.EndOfStream)
				throw new FormatException();

			return this.Peek();
		}
		#endregion

		#region IKON stream reading methods
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
			}
			else if (lastCharacter != EndOfStreamResult && !char.IsControl((char)lastCharacter))
				Column++;

			lastCharacter = reader.Read();
			return (char)lastCharacter;
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met. 
		/// </summary>
		/// <param name="readCondition">Characters that can be read.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(params char[] readCondition)
		{
			if (readCondition == null)
				throw new ArgumentNullException("readCondition");

			if (readCondition.Length == 0)
				throw new ArgumentException("No readable characters specified", "readCondition");

			return ReadWhile(new HashSet<char>(readCondition));
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="readCondition">Set of characters that can be read.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(ISet<char> readCondition)
		{
			if (reader == null)
				throw new ArgumentNullException("readCondition");

			return ReadWhile(c => {
				if (readCondition.Contains(c))
					return new ReadingDecision(c, CharacterAction.AcceptAsIs);
				else
					return new ReadingDecision(c, CharacterAction.Stop);
			});
		}

		/// <summary>
		/// Reads characters from the input stream until the stopping condition is met.
		/// </summary>
		/// <param name="readingController">Returns whether character should be read
		/// (if predicate is true). When predicate evaluates to flase, reading stops.</param>
		/// <returns>Successfully read part of the stream.</returns>
		public string ReadWhile(Func<char, ReadingDecision> readingController)
		{
			if (readingController == null)
				throw new ArgumentNullException("readingController");

			StringBuilder readChars = new StringBuilder();
			while (true) {
				int currentChar = reader.Peek();

				if (currentChar == EndOfStreamResult)
					return readChars.ToString();

				ReadingDecision action = readingController((char)currentChar);
				switch(action.Decision & CharacterAction.AllInputActions) {
					case CharacterAction.AcceptAsIs:
						readChars.Append(Read());
						break;
					case CharacterAction.Skip:
						Read();
						break;
					case CharacterAction.Supstitute:
						readChars.Append(action.Character);
						Read();
						break;
				}

				if (action.Decision.HasFlag(CharacterAction.Stop))
					return readChars.ToString();
			}
		}
		#endregion

		#region IKON stream skipping methods
		/// <summary>
		/// Skips consequentive whitespace characters from the input stream. 
		/// </summary>
		/// <returns>Descrtipion of the skipping process.</returns>
		public ReaderDoneReason SkipWhiteSpaces()
		{
			return SkipWhile(char.IsWhiteSpace);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="skippableCharacters">Character(s) that should be skipped.</param>
		/// <returns>Descrtipion of the skipping process.</returns>
		public ReaderDoneReason SkipWhile(params char[] skippableCharacters)
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
		public ReaderDoneReason SkipWhile(ISet<char> skippableCharacters)
		{
			if (skippableCharacters == null)
				throw new ArgumentNullException("skippableCharacters");

			return SkipWhile(skippableCharacters.Contains);
		}

		/// <summary>
		/// Skips consequentive characters from the input stream.
		/// </summary>
		/// <param name="skipCondition">Returns whether character should be skipped
		/// (if predicate is true). When predicate evaluates to flase, process stops.</param>
		/// <returns>Descrtipion of the skipping process.</returns>
		public ReaderDoneReason SkipWhile(Predicate<char> skipCondition)
		{
			if (skipCondition == null)
				throw new ArgumentNullException("skipCondition");

			while (true) {
				int currentChar = reader.Peek();

				if (currentChar == EndOfStreamResult) 
					return ReaderDoneReason.EndOfStream;

				if (skipCondition((char)currentChar))
					Read();
				else
					return ReaderDoneReason.Successful;

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
