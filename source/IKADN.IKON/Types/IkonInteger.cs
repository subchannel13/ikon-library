using System;
using System.Collections.Generic;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	public class IkonInteger : IkonBaseObject
	{
		//TODO: check docs
		/// <summary>
		/// Tag for IKON numeric objects.
		/// </summary>
		public const string TypeTag = "IKON.Numeric";

		private long value;

		//TODO: check docs
		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonInteger(long value)
		{
			this.value = value;
		}

		//TODO: check docs
		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonInteger(int value)
		{
			this.value = value;
		}

		//TODO: check docs
		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonInteger(short value)
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

		//TODO: check docs
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
				return (T)converters[target](value);
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		//TODO: check docs
		/// <summary>
		/// Writes an IKON numeric object to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.Write(NumericFactory.OpeningSign.ToString());
			writer.Write(value.ToString(NumericFactory.NumberFormat));

			WriteReferences(writer);
		}

		private static Dictionary<Type, Func<long, object>> converters = new Dictionary<Type, Func<long, object>>() {
			{typeof(byte), x => (object)Convert.ToByte(x)},	
			{typeof(sbyte), x => (object)Convert.ToSByte(x)},
			{typeof(char), x => (object)Convert.ToChar(x)},
			{typeof(decimal), x => (object)Convert.ToDecimal(x)},
			{typeof(double), x => (object)Convert.ToDouble(x)},
			{typeof(float), x => (object)Convert.ToSingle(x)},
			{typeof(int), x => (object)Convert.ToInt32(x)},
			{typeof(uint), x => (object)Convert.ToUInt32(x)},
			{typeof(long), x => (object)x},
			{typeof(ulong), x => (object)Convert.ToUInt64(x)},
			{typeof(short), x => (object)Convert.ToInt16(x)},
			{typeof(ushort), x => (object)Convert.ToUInt16(x)},
		};
	}
}
