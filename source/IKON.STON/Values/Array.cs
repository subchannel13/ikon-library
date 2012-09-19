using System.Collections.Generic;
using IKON.STON.Factories;
using IKON.Utils;

namespace IKON.STON.Values
{
	/// <summary>
	/// Array of IKON values.
	/// </summary>
	public class Array : Value
	{
		/// <summary>
		/// Type name of IKSTON arrays.
		/// </summary>
		public const string ValueTypeName = "IKSTON.Array";

		private IList<Value> values;

		/// <summary>
		/// Constructs IKSTON array of IKON values
		/// </summary>
		/// <param name="values">Initial array contents.</param>
		public Array(IList<Value> values)
		{
			this.values = values;
		}

		/// <summary>
		/// Type name of the IKON value instance.
		/// </summary>
		public override string TypeName
		{
			get { return ValueTypeName; }
		}

		/// <summary>
		/// Gets the array of IKON values.
		/// </summary>
		public IList<Value> GetList
		{
			get { return values; }
		}

		/// <summary>
		/// Builder method fo adding one or more elements to IKSTON array.
		/// </summary>
		/// <param name="values">Elements to be added.</param>
		/// <returns>Instance of the same IKSTON array method is called for.</returns>
		public Array Add(params Value[] values)
		{
			foreach (var item in values)
				this.values.Add(item);
			
			return this;
		}

		/// <summary>
		/// Writes an IKSTON array to the composer.
		/// </summary>
		/// <param name="composer">Target composer.</param>
		public override void Compose(Composer composer)
		{
			composer.WriteLine(ArrayFactory.OpeningSign.ToString());
			composer.Indentation.Increase();

			foreach (Value value in values)
				composer.Write(value);

			composer.Indentation.Decrease();
			composer.Write(ArrayFactory.ClosingChar.ToString());
		}
	}
}
