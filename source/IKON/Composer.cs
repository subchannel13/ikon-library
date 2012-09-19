using System.IO;
using System.Collections.Generic;
using System.Text;
using IKON.Utils;

namespace IKON
{
	/// <summary>
	/// Base class for IKON composers. Composers transform IKON values to
	/// a text.
	/// </summary>
	public class Composer
	{
		/// <summary>
		/// Output stream where IKON values are being written.
		/// </summary>
		protected TextWriter writer;

		/// <summary>
		/// Indentation level.
		/// </summary>
		public Indentation Indentation { get; protected set; }

		/// <summary>
		/// Temporary line contents.
		/// </summary>
		protected StringBuilder line = new StringBuilder();

		/// <summary>
		/// Constructs basic IKON composer.
		/// </summary>
		/// <param name="writer">Output stream.</param>
		public Composer(TextWriter writer)
		{
			this.writer = writer;
			this.Indentation = new Indentation();
		}

		/// <summary>
		/// Writes an IKON value to the output stream.
		/// </summary>
		/// <param name="ikonValue">An IKON value.</param>
		public void Write(Value ikonValue)
		{
			ikonValue.Compose(this);
			EndLine();
		}

		/// <summary>
		/// Writes an IKON value with reference names to the output stream.
		/// </summary>
		/// <param name="ikonValue">An IKON value.</param>
		/// <param name="referenceNames">List of reference names.</param>
		public void Write(Value ikonValue, params string[] referenceNames)
		{
			ikonValue.Compose(this);

			foreach (string name in referenceNames)
				Write(" " + HelperMethods.ReferenceSign + name);
			EndLine();
		}
		
		/// <summary>
		/// Appends a text to the current line. Text entered with this method
		/// is buffered and is not written immediately to the output stream.
		/// To finalize the buffered data (and write it to output stream) call
		/// either EndLine or WrtieLine.
		/// </summary>
		/// <param name="text">Raw text.</param>
		public void Write(string text)
		{
			line.Append(text);
		}

		/// <summary>
		/// Appends a text to the current line and writes buffered line to the
		/// output stream.
		/// </summary>
		/// <param name="text">Raw text.</param>
		public void WriteLine(string text)
		{
			line.Append(text);
			EndLine();
		}

		/// <summary>
		/// Writes buffered line to the output stream.
		/// </summary>
		public void EndLine()
		{
			if (line.Length > 0)
			{
				writer.Write(Indentation);
				writer.WriteLine(line);
				line.Clear();
			}
		}
	}
}
