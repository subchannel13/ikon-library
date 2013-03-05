using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Values
{
	/// <summary>
	/// IKON reference value.
	/// </summary>
	public class ReferenceValue : IkonBaseValue
	{
		/// <summary>
		/// Type name of IKON text values.
		/// </summary>
		public const string ValueTypeName = "IKON.Reference";

		private string name;

		/// <summary>
		/// Constructs IKON reference value with specified name.
		/// </summary>
		/// <param name="name">Name of the referenced object.</param>
		public ReferenceValue(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Type name of the IKADN value instance.
		/// </summary>
		public override string TypeName
		{
			get { return ValueTypeName; }
		}

		/// <summary>
		/// Converts IKON reference value to specified type. Supported target types:
		/// 
		/// System.string
		/// Ikadn.Ikon.Values.TextValue
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
		/// Writes an IKON reference value to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.Write(ReferencedFactory.OpeningSign.ToString());
			writer.Write(name);

			WriteReferences(writer);
		}
	}
}
