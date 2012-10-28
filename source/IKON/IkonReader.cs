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
		/// Value that System.IO.TextReader read methods return when end of the stream is reached.
		/// </summary>
		public const int EndOfStreamResult = -1;
		/// <summary>
		/// Character that marks the beginning of the reference name.
		/// </summary>
		public const char ReferenceSign = '@';

		private static ICollection<char> IdentifierChars = DefineIdentifierChars();

		private TextReader reader;

		/// <summary>
		/// Wraps TextReader with IkonReader
		/// </summary>
		/// <param name="reader">Input stream</param>
		public IkonReader(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			this.reader = reader;
		}

		#region Character reading methods
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
		/// Reads next character from the input stream.
		/// </summary>
		/// <returns>Read character.</returns>
		public char Read()
		{
			return (char)reader.Read();
		}
		#endregion

		#region IKON lexics reader methods
		/// <summary>
		/// Skips consequentive whitespace characters from the input stream. 
		/// </summary>
		/// <returns>Descrtipion of the skipping process.</returns>
		public WhiteSpaceSkipResult SkipWhiteSpaces()
		{
			do {
				int currentChar = reader.Peek();

				if (currentChar == EndOfStreamResult) return WhiteSpaceSkipResult.EndOfStream;

				if (char.IsWhiteSpace((char)currentChar))
					reader.Read();
				else
					return WhiteSpaceSkipResult.NonwhiteChar;

			} while (true);
		}

		/// <summary>
		/// Skips white space characters in the input stream and peeks the next character. 
		/// Throws System.FormatException if end of stream is reached before a non-white 
		/// character i found.
		/// </summary>
		/// <returns>A non-white character</returns>
		public char PeekNextNonwhite()
		{
			if (SkipWhiteSpaces() == WhiteSpaceSkipResult.EndOfStream)
				throw new FormatException();

			return this.Peek();
		}

		/// <summary>
		/// Reads the input stream for an IKON identifier. Throws System.FormatException if there is
		/// no valid identifier.
		/// </summary>
		/// <returns>An identifier name</returns>
		public string ReadIdentifier()
		{
			if (SkipWhiteSpaces() == WhiteSpaceSkipResult.EndOfStream)
				throw new EndOfStreamException();

			StringBuilder stringBuilder = new StringBuilder();

			while (HasNext && IdentifierChars.Contains(this.Peek())) {
				stringBuilder.Append(this.Read());
			}

			if (stringBuilder.Length == 0)
				throw new FormatException();

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Returns all consequentive reference names from the current state of the input stream.
		/// </summary>
		/// <returns>Reference names.</returns>
		public IEnumerable<string> ReadReferences()
		{
			while (true) {
				WhiteSpaceSkipResult skipResult = SkipWhiteSpaces();

				if (skipResult == WhiteSpaceSkipResult.EndOfStream)
					break;

				char sign = (char)reader.Peek();

				if (sign == ReferenceSign) {
					reader.Read();
					yield return ReadIdentifier();
				}
				else
					break;
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

		private static HashSet<char> DefineIdentifierChars()
		{
			HashSet<char> res = new HashSet<char>();

			res.Add('_');
			for (char c = 'a'; c <= 'z'; c++) res.Add(c);
			for (char c = 'A'; c <= 'Z'; c++) res.Add(c);
			for (char c = '0'; c <= '9'; c++) res.Add(c);

			return res;
		}
	}
}
