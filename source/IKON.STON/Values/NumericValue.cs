using Ikon.Ston.Factories;
using Ikon.Utilities;

namespace Ikon.Ston.Values
{
	/// <summary>
	/// IKSTON numeric value.
	/// </summary>
	public class NumericValue : Value
	{
		/// <summary>
		/// Type name of IKSTON numeric values.
		/// </summary>
		public const string ValueTypeName = "IKSTON.Numeric";

		private string textualRepresentation;

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="textualRepresentation">Textual representation of the value</param>
		protected internal NumericValue(string textualRepresentation)
		{
			this.textualRepresentation = textualRepresentation;
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public NumericValue(decimal value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public NumericValue(double value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public NumericValue(float value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public NumericValue(long value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public NumericValue(int value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKSTON numeric value.
		/// </summary>
		/// <param name="value">The value</param>
		public NumericValue(short value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
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
			get { return decimal.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Double value from IKON numeric value.
		/// </summary>
		public double GetDouble
		{
			get { return double.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Single value from IKON numeric value.
		/// </summary>
		public float GetFloat
		{
			get { return float.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Int32 value from IKON numeric value.
		/// </summary>
		public int GetInt
		{
			get { return int.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Int64 value from IKON numeric value.
		/// </summary>
		public long GetLong
		{
			get { return long.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets System.Int16 value from IKON numeric value.
		/// </summary>
		public short GetShort
		{
			get { return short.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat); }
		}

		/// <summary>
		/// Gets textual representation of contained numeric value.
		/// </summary>
		public string InvariantString
		{
			get	{ return textualRepresentation; }
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Decimal value.
		/// </summary>
		public static implicit operator decimal(NumericValue textValue)
		{
			return textValue.GetDecimal;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Double value.
		/// </summary>
		public static implicit operator double(NumericValue textValue)
		{
			return textValue.GetDouble;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Single value.
		/// </summary>
		public static implicit operator float(NumericValue textValue)
		{
			return textValue.GetFloat;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Int32 value.
		/// </summary>
		public static implicit operator int(NumericValue textValue)
		{
			return textValue.GetInt;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Int64 value.
		/// </summary>
		public static implicit operator long(NumericValue textValue)
		{
			return textValue.GetLong;
		}

		/// <summary>
		/// Implicit conversion from IKSTON numeric value to System.Int16 value.
		/// </summary>
		public static implicit operator short(NumericValue textValue)
		{
			return textValue.GetShort;
		}

		/// <summary>
		/// Writes an IKSTON numeric value to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkonWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("composer");

			writer.Write(NumericFactory.OpeningSign.ToString());
			writer.Write(InvariantString);
		}
	}
}
