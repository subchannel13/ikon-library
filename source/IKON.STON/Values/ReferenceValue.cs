using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ikon.Ston.Factories;

namespace Ikon.Ston.Values
{
	/// <summary>
	/// IKSTON reference value.
	/// </summary>
	public class ReferenceValue : IkstonBaseValue
	{
		/// <summary>
		/// Type name of IKSTON text values.
		/// </summary>
		public const string ValueTypeName = "IKSTON.Reference";

		private string name;

		/// <summary>
		/// Constructs IKSTON reference value with specified name.
		/// </summary>
		/// <param name="name">Name of the referenced object.</param>
		public ReferenceValue(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Type name of the IKON value instance.
		/// </summary>
		public override string TypeName
		{
			get { return ValueTypeName; }
		}

		/// <summary>
		/// Converts IKSTON reference value to specified type. Supported target types:
		/// 
		/// System.string
		/// Ikon.Ston.Values.TextValue
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);

			if (target == typeof(string))
				return (T)(object)name;
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + TypeName);
		}

		/// <summary>
		/// Writes an IKSTON reference value to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkonWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.Write(ReferencedFactory.OpeningSign.ToString());
			writer.Write(name);

			WriteReferences(writer);
		}
	}
}
