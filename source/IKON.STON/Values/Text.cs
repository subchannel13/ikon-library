using System.Text;
using IKON.STON.Factories;
using IKON.Utils;

namespace IKON.STON.Values
{
	/// <summary>
	/// IKSTON textual value.
	/// </summary>
	public class Text : Value
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
		public Text(string value)
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
		/// Gets System.String value from IKSTON textual value.
		/// </summary>
		public string GetText
		{
			get { return text; }
		}

		/// <summary>
		/// Implicit conversion from IKON textual value to System.String value.
		/// </summary>
		public static implicit operator string(Text textValue)
		{
			return textValue.GetText;
		}

		/// <summary>
		/// Writes an IKSTON text value to the composer.
		/// </summary>
		/// <param name="composer">Target composer.</param>
		public override void Compose(Composer composer)
		{
			StringBuilder sb = new StringBuilder(text);
			sb.Replace(@"\", @"\\");
			sb.Replace(@"""", @"\""");
			sb.Replace("\n", @"\n");
			sb.Replace("\r", @"\r");
			sb.Replace("\t", @"\t");

			composer.Write(TextFactory.OpeningSign.ToString());
			composer.Write(sb.ToString());
			composer.Write(TextFactory.ClosingChar.ToString());
		}
	}
}
