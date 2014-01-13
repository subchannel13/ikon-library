using System;
using System.Text;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON textual object.
	/// </summary>
	public class IkonText : IkonBaseObject
	{
		/// <summary>
		/// Tag for IKON text objects.
		/// </summary>
		public const string TypeTag = "IKON.Text";

		private string text;

		/// <summary>
		/// Constructs IKON textual object with specified contents.
		/// </summary>
		/// <param name="text">Contents</param>
		public IkonText(string text)
		{
			this.text = text;
		}

		/// <summary>
		/// Tag of the IKADN object instance.
		/// </summary>
		public override object Tag
		{
			get { return TypeTag; }
		}

		/// <summary>
		/// Converts IKON text object to specified type. Supported target types:
		/// 
		/// System.string
		/// Ikadn.Ikon.Types.IkonText
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);

			if (target == typeof(string))
				return (T)(object)text;
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Implicit conversion from IKADN textual object to System.String.
		/// </summary>
		public static implicit operator string(IkonText ikonText)
		{
			if (ikonText == null)
				return null;

			return ikonText.text;
		}

		/// <summary>
		/// Writes an IKON text object to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			if (this.text.Contains("\n"))
				composeBlock(writer);
			else
				composeLine(writer);
			
			WriteReferences(writer);
		}
		
		private void composeLine(IkadnWriter writer)
		{
			StringBuilder sb = new StringBuilder(text);
			sb.Replace(@"\", @"\\");
			sb.Replace(@"""", @"\""");
			sb.Replace("\n", @"\n");
			sb.Replace("\r", @"\r");
			sb.Replace("\t", @"\t");

			writer.Write(TextFactory.OpeningSign.ToString());
			writer.Write(sb.ToString());
			writer.Write(TextFactory.ClosingChar.ToString());
		}
		
		void composeBlock(IkadnWriter writer)
		{
			StringBuilder sb = new StringBuilder(Environment.NewLine + text);
			sb.Replace(
				Environment.NewLine,
				Environment.NewLine + writer.Indentation.ToString() + "\t");

			writer.Write(TextBlockFactory.OpeningSign.ToString());
			writer.Write(sb.ToString());
		}
	}
}
