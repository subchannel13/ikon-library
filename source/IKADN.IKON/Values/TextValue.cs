using System.Text;
using Ikadn.Ikon.Factories;
using Ikadn.Utilities;
using System;

namespace Ikadn.Ikon.Values
{
	/// <summary>
	/// IKON textual value.
	/// </summary>
	public class TextValue : IkonBaseValue
	{
		/// <summary>
		/// Type name of IKON text values.
		/// </summary>
		public const string ValueTypeName = "IKON.Text";

		private string text;

		/// <summary>
		/// Constructs IKON textual value with specified contents.
		/// </summary>
		/// <param name="value">Contents</param>
		public TextValue(string value)
		{
			this.text = value;
		}

		/// <summary>
		/// Type name of the IKADN value instance.
		/// </summary>
		public override string TypeName
		{
			get { return ValueTypeName; }
		}

		/// <summary>
		/// Converts IKON text value to specified type. Supported target types:
		/// 
		/// System.string
		/// Ikadn.Ikon.Values.TextValue
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
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + TypeName);
		}

		/// <summary>
		/// Implicit conversion from IKADN textual value to System.String value.
		/// </summary>
		public static implicit operator string(TextValue textValue)
		{
			if (textValue == null)
				return null;

			return textValue.text;
		}

		/// <summary>
		/// Writes an IKON text value to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			StringBuilder sb = new StringBuilder(text);
			sb.Replace(@"\", @"\\");
			sb.Replace(@"""", @"\""");
			sb.Replace("\n", @"\n");
			sb.Replace("\r", @"\r");
			sb.Replace("\t", @"\t");

			writer.Write(TextFactory.OpeningSign.ToString());
			writer.Write(sb.ToString());
			writer.Write(TextFactory.ClosingChar.ToString());

			WriteReferences(writer);
		}
	}
}
