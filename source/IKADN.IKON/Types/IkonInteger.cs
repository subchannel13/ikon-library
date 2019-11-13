// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON numeric object (integer).
	/// </summary>
	public class IkonInteger : IkonBaseObject
	{
		/// <summary>
		/// Tag for IKON numeric objects.
		/// </summary>
		public static readonly string TypeTag = "IKON.Numeric";

		private readonly long value;

		/// <summary>
		/// Constructs IKON integer numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonInteger(long value)
		{
			this.value = value;
		}

		/// <summary>
		/// Constructs IKON integer numeric object.
		/// </summary>
		/// <param name="value">The value</param>
		public IkonInteger(int value)
		{
			this.value = value;
		}

		/// <summary>
		/// Constructs IKON integer numeric object.
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
		/// Ikadn.Ikon.Types.IkonInteger
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

		private static readonly Dictionary<Type, Func<long, object>> Converters = new Dictionary<Type, Func<long, object>> {
			{typeof(byte), x => Convert.ToByte(x)},	
			{typeof(sbyte), x => Convert.ToSByte(x)},
			{typeof(char), x => Convert.ToChar(x)},
			{typeof(decimal), x => Convert.ToDecimal(x)},
			{typeof(double), x => Convert.ToDouble(x)},
			{typeof(float), x => Convert.ToSingle(x)},
			{typeof(int), x => Convert.ToInt32(x)},
			{typeof(uint), x => Convert.ToUInt32(x)},
			{typeof(long), x => x},
			{typeof(ulong), x => Convert.ToUInt64(x)},
			{typeof(short), x => Convert.ToInt16(x)},
			{typeof(ushort), x => Convert.ToUInt16(x)},
		};
	}
}
