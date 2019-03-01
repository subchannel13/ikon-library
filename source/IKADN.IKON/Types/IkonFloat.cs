using System;
using Ikadn.Ikon.Factories;
using System.Collections.Generic;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON numeric object (floating point).
	/// </summary>
	public class IkonFloat : IkonBaseObject
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

		private readonly double value;

		/// <summary>
		/// Constructs IKON floating point numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonFloat(double value)
		{
			this.value = value;
		}

		/// <summary>
		/// Constructs IKON floating point numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonFloat(float value)
		{
			this.value = value;
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
		/// byte
		/// char
		/// decimal
		/// double
		/// float
		/// int
		/// long
		/// sbyte
		/// short
		/// uint
		/// ulong
		/// ushort
		/// Ikadn.Ikon.Types.IkonFloat
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);
			if (converters.ContainsKey(target))
				return (T)converters[target](value);
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Writes an IKON numeric object to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.Write(NumericFactory.OpeningSign);
			
			if (double.IsNaN(value))
				writer.Write(NotANumber);
			else if (double.IsNegativeInfinity(value))
				writer.Write(NegativeInfinity);
			else if (double.IsPositiveInfinity(value))
				writer.Write(PositiveInfinity);
			else
				writer.Write(value.ToString(NumericFactory.NumberFormat));

			WriteReferences(writer);
		}

		private static Dictionary<Type, Func<double, object>> converters = new Dictionary<Type, Func<double, object>> {
			{typeof(byte), x => (object)Convert.ToByte(x)},	
			{typeof(sbyte), x => (object)Convert.ToSByte(x)},
			{typeof(char), x => (object)Convert.ToChar(x)},
			{typeof(decimal), x => (object)Convert.ToDecimal(x)},
			{typeof(double), x => (object)x},
			{typeof(float), x => (object)Convert.ToSingle(x)},
			{typeof(int), x => (object)Convert.ToInt32(x)},
			{typeof(uint), x => (object)Convert.ToUInt32(x)},
			{typeof(long), x => (object)Convert.ToInt64(x)},
			{typeof(ulong), x => (object)Convert.ToUInt64(x)},
			{typeof(short), x => (object)Convert.ToInt16(x)},
			{typeof(ushort), x => (object)Convert.ToUInt16(x)},
		};
	}
}
