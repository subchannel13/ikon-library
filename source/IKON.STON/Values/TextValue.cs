using System.Text;
using Ikon.Ston.Factories;
using Ikon.Utilities;
using System;

namespace Ikon.Ston.Values
{
	/// <summary>
	/// IKSTON textual value.
	/// </summary>
	public class TextValue : Value
	{
		/// <summary>
		/// Type name of IKSTON text values.
		/// </summary>
		public const string ValueTypeName = "IKSTON.Text";

		private string text;

		/// <summary>
		/// Constructs IKSTON textual value with specified contents.
		/// </summary>
		/// <param name="value">Contents</param>
		public TextValue(string value)
		{
			this.text = value;
		}

		/// <summary>
		/// Type name of the IKON value instance.
		/// </summary>
		public override string TypeName
		{
			get { return ValueTypeName; }
		}

		/// <summary>
		/// Converts IKSTON object value to specified type. Supported target types:
		/// 
		/// System.string
		/// Ikon.Ston.Values.TextValue
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T As<T>()
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
		/// Implicit conversion from IKON textual value to System.String value.
		/// </summary>
		public static implicit operator string(TextValue textValue)
		{
			return textValue.text;
		}

		/// <summary>
		/// Writes an IKSTON text value to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkonWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("composer");

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
	}
}
