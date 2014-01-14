using System;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	public class IkonFloat : IkonBaseObject
	{
		//TODO: check docs
		/// <summary>
		/// Tag for IKON numeric objects.
		/// </summary>
		public const string TypeTag = "IKON.Numeric";

		//TODO: check docs
		/// <summary>
		/// Textual representation of IKON numeric for positive infinity.
		/// </summary>
		public const string PositiveInfinity = "Inf";

		//TODO: check docs
		/// <summary>
		/// Textual representation of IKON numeric for negative infinity.
		/// </summary>
		public const string NegativeInfinity = "-Inf";

		//TODO: check docs
		/// <summary>
		/// Textual representation of IKON numeric for not a number.
		/// </summary>
		public const string NotANumber = "NaN";

		private double value;

		//TODO: check docs
		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonFloat(double value)
		{
			this.value = value;
		}

		//TODO: check docs
		/// <summary>
		/// Constructs IKON numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonFloat(float value)
		{
			this.value = value;
		}

		//TODO: check docs
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

			if (NumericFactory.NumberTypes.Contains(target))
				return (T)(object)value;
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
	}
}
