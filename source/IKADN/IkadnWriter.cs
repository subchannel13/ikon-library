// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// LGPL License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

using Ikadn.Utilities;

namespace Ikadn
{
	/// <summary>
	/// Base class for IKADN composers. Composers transform IKADN object to
	/// a plain text.
	/// </summary>
	public class IkadnWriter
	{
		/// <summary>
		/// Output stream where IKADN objects are being written.
		/// </summary>
		protected TextWriter Writer { get; private set; }

		/// <summary>
		/// Indentation level.
		/// </summary>
		public Indentation Indentation { get; protected set; }

		/// <summary>
		/// Temporary line contents.
		/// </summary>
		private StringBuilder Line { get; set; }

		/// <summary>
		/// Constructs basic IKADN composer.
		/// </summary>
		/// <param name="writer">Output stream.</param>
		public IkadnWriter(TextWriter writer)
		{
			this.Writer = writer;

			this.Line = new StringBuilder();
			this.Indentation = new Indentation();
		}
	
		/// <summary>
		/// Appends a character to the current line. Text entered with this method
		/// is buffered and is not written immediately to the output stream.
		/// To finalize the buffered data (and write it to output stream) call
		/// either EndLine or WrtieLine.
		/// </summary>
		/// <param name="character">A character to write</param>
		public void Write(char character)
		{
			this.Line.Append(character);
		}
		
		/// <summary>
		/// Appends a text to the current line. Text entered with this method
		/// is buffered and is not written immediately to the output stream.
		/// To finalize the buffered data (and write it to output stream) call
		/// either EndLine or WrtieLine.
		/// </summary>
		/// <param name="text">Raw text</param>
		public void Write(string text)
		{
			if (text == null)
				throw new ArgumentNullException(nameof(text));

			this.Line.Append(text);
		}

		/// <summary>
		/// Appends a character to the current line and writes buffered line to the
		/// output stream.
		/// </summary>
		/// <param name="character">Raw text.</param>
		public void WriteLine(char character)
		{
			this.Line.Append(character);
			this.EndLine();
		}
		
		/// <summary>
		/// Appends a text to the current line and writes buffered line to the
		/// output stream.
		/// </summary>
		/// <param name="text">Raw text</param>
		public void WriteLine(string text)
		{
			if (text == null)
				throw new ArgumentNullException(nameof(text));

			this.Line.Append(text);
			this.EndLine();
		}

		/// <summary>
		/// Writes buffered line to the output stream.
		/// </summary>
		public void EndLine()
		{
			this.Writer.Write(this.Indentation);
			this.Writer.WriteLine(this.Line);
			this.Line.Length = 0;
		}
	}
}
