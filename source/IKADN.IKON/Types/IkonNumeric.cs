using System;
using System.Collections.Generic;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON numeric object.
	/// </summary>
	public class IkonNumeric : IkonBaseObject
	{
		/// <summary>
		/// Tag for IKON numeric objects.
		/// </summary>
		public const string TypeTag = "IKON.Numeric";

		/// <summary>
		/// Textual representation of IKON numeric for positive infinity.
		/// </summary>
		public const string PositiveInfinity = "Inf";

		/// <summary>
		/// Textual representation of IKON numeric for negative infinity.
		/// </summary>
		public const string NegativeInfinity = "-Inf";

		/// <summary>
		/// Textual representation of IKON numeric for not a number.
		/// </summary>
		public const string NotANumber = "NaN";

		private string textualRepresentation;

		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="textualRepresentation">Textual representation of the object</param>
		protected internal IkonNumeric(string textualRepresentation)
		{
			this.textualRepresentation = textualRepresentation;
		}

		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonNumeric(decimal value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonNumeric(double value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonNumeric(float value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonNumeric(long value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonNumeric(int value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonNumeric(short value)
		{
			this.textualRepresentation = value.ToString(NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Tag of the IKADN object instance.
		/// </summary>
		public override object Tag
		{
			get { return TypeTag; }
		}

		/// <summary>
		/// Converts IKON numeric object to specified type. Supported target types:
		/// 
		/// System.decimal
		/// System.double
		/// System.float
		/// System.int
		/// System.long
		/// System.short
		/// Ikadn.Ikon.Types.IkonNumeric
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);

			if (converters.ContainsKey(target))
				return (T)converters[target](textualRepresentation);
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Gets System.Decimal value from IKADN numeric object.
		/// </summary>
		private static decimal GetDecimal(string textualRepresentation)
		{
			return decimal.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Gets System.Double value from IKADN numeric object.
		/// </summary>
		private static double GetDouble(string textualRepresentation)
		{
			switch (textualRepresentation) {
				case PositiveInfinity:
					return double.PositiveInfinity;
				case NegativeInfinity:
					return double.NegativeInfinity;
				case NotANumber:
					return double.NaN;
				default:
					return double.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat);
			}
		}

		/// <summary>
		/// Gets System.Single value from IKADN numeric object.
		/// </summary>
		private static float GetFloat(string textualRepresentation)
		{
			switch (textualRepresentation) {
				case PositiveInfinity:
					return float.PositiveInfinity;
				case NegativeInfinity:
					return float.NegativeInfinity;
				case NotANumber:
					return float.NaN;
				default:
					return float.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat);
			}
		}

		/// <summary>
		/// Gets System.Int32 value from IKADN numeric object.
		/// </summary>
		private static int GetInt(string textualRepresentation)
		{
			return int.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Gets System.Int64 value from IKADN numeric object.
		/// </summary>
		private static long GetLong(string textualRepresentation)
		{
			return long.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat);
		}
		
		/// <summary>
		/// Gets System.Int16 value from IKADN numeric object.
		/// </summary>
		private static short GetShort(string textualRepresentation)
		{
			return short.Parse(textualRepresentation, NumericFactory.NumberStyle, NumericFactory.NumberFormat);
		}

		/// <summary>
		/// Writes an IKON numeric object to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.Write(NumericFactory.OpeningSign.ToString());
			writer.Write(textualRepresentation);

			WriteReferences(writer);
		}

		private static Dictionary<Type, Func<string, object>> converters = new Dictionary<Type, Func<string, object>>() {
			{typeof(decimal), x => (object)GetDecimal(x)},
			{typeof(double), x => (object)GetDouble(x)},
			{typeof(float), x => (object)GetFloat(x)},
			{typeof(int), x => (object)GetInt(x)},
			{typeof(long), x => (object)GetLong(x)},
			{typeof(short), x => (object)GetShort(x)},
			{typeof(string), x => (object)x},
		};
	}
}
