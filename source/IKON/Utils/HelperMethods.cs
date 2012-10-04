using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ikon.Utilities
{
	/// <summary>
	/// Helper class with utility methods for parsing and composing
	/// text streams with IKON syntax.
	/// </summary>
	public static class HelperMethods
	{
		/// <summary>
		/// Value that System.IO.TextReader read methods return when end of the stream is reached.
		/// </summary>
		public const int EndOfStreamResult = -1;
		/// <summary>
		/// Character that marks the beginning of the reference name.
		/// </summary>
		public const char ReferenceSign = '@';

		static ICollection<char> IdentifierChars = DefineIdentifierChars();

		/// <summary>
		/// Returns all consequentive reference names from the current state of the input stream.
		/// </summary>
		/// <param name="reader">The input stream.</param>
		/// <returns>Reference names.</returns>
		public static IEnumerable<string> ParseReferences(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			while(true)
			{
				WhiteSpaceSkipResult skipResult = SkipWhiteSpaces(reader);

				if (skipResult == WhiteSpaceSkipResult.EndOfStream)
					break;

				char sign = (char)reader.Peek();

				if (sign == ReferenceSign)
				{
					reader.Read();
					yield return ParseIdentifier(reader);
				}
				else
					break;
			}
		}

		/// <summary>
		/// Skips whitespaces and reads an identifier from the input stream. Throws System.FormatException if there is
		/// no valid identifier.
		/// </summary>
		/// <param name="reader">The input stream.</param>
		/// <returns>An identifier value</returns>
		public static string ParseIdentifier(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (SkipWhiteSpaces(reader) == WhiteSpaceSkipResult.EndOfStream)
				throw new EndOfStreamException();

			StringBuilder stringBuilder = new StringBuilder();

			for (int nextCharCode = reader.Peek();
				nextCharCode != EndOfStreamResult && IdentifierChars.Contains((char)nextCharCode);
				nextCharCode = reader.Peek())
			{
				stringBuilder.Append((char)reader.Read());
			}

			if (stringBuilder.Length == 0)
				throw new FormatException();

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Reads input for the next non-white character. Throws System.FormatException if end 
		/// of stream is reached before a such character.
		/// </summary>
		/// <param name="reader">The input stream.</param>
		/// <returns>A non-white character.</returns>
		public static char NextNonwhite(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (SkipWhiteSpaces(reader) == WhiteSpaceSkipResult.EndOfStream)
				throw new FormatException();

			return (char)reader.Peek();
		}

		/// <summary>
		/// Skips consequentive whitespace characters from the input.
		/// </summary>
		/// <param name="reader">The input stream.</param>
		/// <returns>Descrtipion of the skipping process.</returns>
		public static WhiteSpaceSkipResult SkipWhiteSpaces(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			do
			{
				int currentChar = reader.Peek();

				if (currentChar == EndOfStreamResult) return WhiteSpaceSkipResult.EndOfStream;

				if (char.IsWhiteSpace((char)currentChar))
					reader.Read();
				else
					return WhiteSpaceSkipResult.NonwhiteChar;

			} while (true);
		}

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
