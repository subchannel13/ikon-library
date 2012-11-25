using System.Collections.Generic;
using Ikon.Ston.Factories;
using Ikon.Utilities;

namespace Ikon.Ston.Values
{
	/// <summary>
	/// Array of IKON values.
	/// </summary>
	public class ArrayValue : Value
	{
		/// <summary>
		/// Type name of IKSTON arrays.
		/// </summary>
		public const string ValueTypeName = "IKSTON.Array";

		private IList<Value> elements;

		/// <summary>
		/// Constructs IKSTON array of IKON values
		/// </summary>
		/// <param name="values">Initial array contents.</param>
		public ArrayValue(IList<Value> values)
		{
			this.elements = values;
		}

		/// <summary>
		/// Constructs IKSTON array
		/// </summary>
		public ArrayValue()
		{
			this.elements = new List<Value>();
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
			get { return elements; }
		}

		/// <summary>
		/// Builder method fo adding one or more elements to IKSTON array.
		/// </summary>
		/// <param name="values">Elements to be added.</param>
		/// <returns>Instance of the same IKSTON array method is called for.</returns>
		public ArrayValue Add(params Value[] values)
		{
			if (values == null)
				throw new System.ArgumentNullException("values");

			foreach (var item in values)
				this.elements.Add(item);
			
			return this;
		}

		/// <summary>
		/// Writes an IKSTON array to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkonWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("composer");

			writer.WriteLine(ArrayFactory.OpeningSign.ToString());
			writer.Indentation.Increase();

			foreach (Value value in elements)
				value.Compose(writer);

			writer.Indentation.Decrease();
			writer.Write(ArrayFactory.ClosingChar.ToString());
		}
	}
}
