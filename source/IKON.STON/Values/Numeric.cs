using IKON.STON.Factories;
using IKON.Utils;

namespace IKON.STON.Values
{
	/// <summary>
	/// IKSTON numeric value.
	/// </summary>
	public class Numeric : Value
	{
		/// <summary>
		/// Type name of IKSTON numeric values.
		/// </summary>
		public const string ValueTypeName = "IKSTON.Numeric";

		private string texutalRepresentation;

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="texutalRepresentation">Textual representation of the value</param>
		protected internal Numeric(string texutalRepresentation)
		{
			this.texutalRepresentation = texutalRepresentation;
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public Numeric(decimal value)
		{
			this.texutalRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public Numeric(double value)
		{
			this.texutalRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public Numeric(float value)
		{
			this.texutalRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public Numeric(long value)
		{
			this.texutalRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public Numeric(int value)
		{
			this.texutalRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public Numeric(short value)
		{
			this.texutalRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Type name of the IKSTON numeric value instance.
		/// </summary>
		public override string TypeName
		{
			get { return ValueTypeName; }
		}

		/// <summary>
		/// Gets System.Decimal value from IKON numeric value.
		/// </summary>
		public decimal GetDecimal
		{
			get { return decimal.Parse(texutalRepresentation, NumericFactory.NumberStlye, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Double value from IKON numeric value.
		/// </summary>
		public double GetDouble
		{
			get { return double.Parse(texutalRepresentation, NumericFactory.NumberStlye, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Single value from IKON numeric value.
		/// </summary>
		public float GetFloat
		{
			get { return float.Parse(texutalRepresentation, NumericFactory.NumberStlye, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Int32 value from IKON numeric value.
		/// </summary>
		public int GetInt
		{
			get { return int.Parse(texutalRepresentation, NumericFactory.NumberStlye, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Int64 value from IKON numeric value.
		/// </summary>
		public long GetLong
		{
			get { return long.Parse(texutalRepresentation, NumericFactory.NumberStlye, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Int16 value from IKON numeric value.
		/// </summary>
		public short GetShort
		{
			get { return short.Parse(texutalRepresentation, NumericFactory.NumberStlye, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets textual representation of contained numeric value.
		/// </summary>
		public string InvariantString
		{
			get	{ return texutalRepresentation; }
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Decimal value.
		/// </summary>
		public static implicit operator decimal(Numeric textValue)
		{
			return textValue.GetDecimal;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Double value.
		/// </summary>
		public static implicit operator double(Numeric textValue)
		{
			return textValue.GetDouble;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Single value.
		/// </summary>
		public static implicit operator float(Numeric textValue)
		{
			return textValue.GetFloat;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Int32 value.
		/// </summary>
		public static implicit operator int(Numeric textValue)
		{
			return textValue.GetInt;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Int64 value.
		/// </summary>
		public static implicit operator long(Numeric textValue)
		{
			return textValue.GetLong;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Int16 value.
		/// </summary>
		public static implicit operator short(Numeric textValue)
		{
			return textValue.GetShort;
		}

		/// <summary>
		/// Writes an IKSTON numeric value to the composer.
		/// </summary>
		/// <param name="composer">Target composer.</param>
		public override void Compose(Composer composer)
		{
			composer.Write(NumericFactory.OpeningSign.ToString());
			composer.Write(InvariantString);
		}
	}
}
