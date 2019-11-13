// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

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
		public static readonly string TypeTag = "IKON.Numeric";

		/// <summary>
		/// Textual representation of IKON numeric for positive infinity.
		/// </summary>
		public static readonly string PositiveInfinity = "Inf";

		/// <summary>
		/// Textual representation of IKON numeric for negative infinity.
		/// </summary>
		public static readonly string NegativeInfinity = "-Inf";

		/// <summary>
		/// Textual representation of IKON numeric for not a number.
		/// </summary>
		public static readonly string NotANumber = "NaN";

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
			var target = typeof(T);
			if (converters.ContainsKey(target))
				return (T)converters[target](this.value);
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
				throw new ArgumentNullException(nameof(writer));

			writer.Write(NumericFactory.OpeningSign);
			
			if (double.IsNaN(this.value))
				writer.Write(NotANumber);
			else if (double.IsNegativeInfinity(this.value))
				writer.Write(NegativeInfinity);
			else if (double.IsPositiveInfinity(this.value))
				writer.Write(PositiveInfinity);
			else
				writer.Write(this.value.ToString(NumericFactory.NumberFormat));

			this.WriteReferences(writer);
		}

		private static readonly Dictionary<Type, Func<double, object>> converters = new Dictionary<Type, Func<double, object>> {
			{typeof(byte), x => Convert.ToByte(x)},	
			{typeof(sbyte), x => Convert.ToSByte(x)},
			{typeof(char), x => Convert.ToChar(x)},
			{typeof(decimal), x => Convert.ToDecimal(x)},
			{typeof(double), x => x},
			{typeof(float), x => Convert.ToSingle(x)},
			{typeof(int), x => Convert.ToInt32(x)},
			{typeof(uint), x => Convert.ToUInt32(x)},
			{typeof(long), x => Convert.ToInt64(x)},
			{typeof(ulong), x => Convert.ToUInt64(x)},
			{typeof(short), x => Convert.ToInt16(x)},
			{typeof(ushort), x => Convert.ToUInt16(x)},
		};
	}
}
