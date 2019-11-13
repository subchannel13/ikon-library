// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using Ikadn.Ikon.Factories;
using System.Collections.Generic;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON numeric object (fixed point).
	/// </summary>
	public class IkonDecimal : IkonBaseObject
	{
		/// <summary>
		/// Tag for IKON numeric objects.
		/// </summary>
		public static readonly string TypeTag = "IKON.Numeric";

		private readonly decimal value;

		/// <summary>
		/// Constructs IKON fixed point numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonDecimal(decimal value)
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
		/// Ikadn.Ikon.Types.IkonDecimal
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			var target = typeof(T);

			if (Converters.ContainsKey(target))
				return (T)Converters[target](this.value);
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
			writer.Write(this.value.ToString(NumericFactory.NumberFormat));

			this.WriteReferences(writer);
		}

		private static readonly Dictionary<Type, Func<decimal, object>> Converters = new Dictionary<Type, Func<decimal, object>> {
			{typeof(byte), x => Convert.ToByte(x)},	
			{typeof(sbyte), x => Convert.ToSByte(x)},
			{typeof(char), x => Convert.ToChar(x)},
			{typeof(decimal), x => x},
			{typeof(double), x => Convert.ToDouble(x)},
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
