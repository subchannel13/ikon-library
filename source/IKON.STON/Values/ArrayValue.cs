using System.Collections.Generic;
using Ikon.Ston.Factories;
using Ikon.Utilities;
using System;

namespace Ikon.Ston.Values
{
	/// <summary>
	/// Array of IKON values.
	/// </summary>
	public class ArrayValue : IkstonBaseValue, IList<Value>
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
		/// Converts IKSTON array value to specified type. Supported target types:
		/// 
		/// System.Collections.Generic.IList&lt;Value&gt;
		/// Ikon.Ston.Values.ArrayValue
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);

			if (target.IsAssignableFrom(typeof(IList<Value>)))
				return (T)elements;
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + TypeName);
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
				throw new System.ArgumentNullException("writer");

			writer.WriteLine(ArrayFactory.OpeningSign.ToString());
			writer.Indentation.Increase();

			foreach (Value value in elements)
				value.Compose(writer);

			writer.Indentation.Decrease();
			writer.Write(ArrayFactory.ClosingChar.ToString());

			WriteReferences(writer);
		}

		#region IList<Value> interface
		/// <summary>
		/// Determines the index of a specific item in the System.Collections.Generic.IList&lt;T&gt;.
		/// </summary>
		/// <param name="item">The object to locate in the System.Collections.Generic.IList&lt;T&gt;.</param>
		/// <returns>The index of item if found in the list; otherwise, -1.</returns>
		public int IndexOf(Value item)
		{
			return this.elements.IndexOf(item);
		}

		/// <summary>
		/// Inserts an item to the System.Collections.Generic.IList&lt;T&gt; at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the System.Collections.Generic.IList&lt;T&gt;.</param>
		public void Insert(int index, Value item)
		{
			this.elements.Insert(index, item);
		}

		/// <summary>
		/// Removes the System.Collections.Generic.IList&lt;T&gt; item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public void RemoveAt(int index)
		{
			this.elements.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public Value this[int index]
		{
			get
			{
				return this.elements[index];
			}
			set
			{
				this.elements[index] = value;
			}
		}

		/// <summary>
		/// Adds an item to the System.Collections.Generic.ICollection&lt;T&gt;.
		/// </summary>
		/// <param name="item">The object to add to the System.Collections.Generic.ICollection&lt;T&gt;.</param>
		public void Add(Value item)
		{
			this.elements.Add(item);
		}

		/// <summary>
		/// Removes all items from the System.Collections.Generic.ICollection&lt;T&gt;.
		/// </summary>
		public void Clear()
		{
			this.elements.Clear();
		}

		/// <summary>
		/// Determines whether the System.Collections.Generic.ICollection&lt;T&gt; contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the System.Collections.Generic.ICollection&lt;T&gt;.</param>
		/// <returns>true if item is found in the System.Collections.Generic.ICollection&lt;T&gt;; otherwise, false.</returns>
		public bool Contains(Value item)
		{
			return this.elements.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the System.Collections.Generic.ICollection&lt;T&gt; to an System.Array,
		/// starting at a particular System.Array index.
		/// </summary>
		/// <param name="array">The one-dimensional System.Array that is the destination of the elements copied
		/// from System.Collections.Generic.ICollection&lt;T&gt;. The System.Array must have zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(Value[] array, int arrayIndex)
		{
			this.elements.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Gets the number of elements contained in the System.Collections.Generic.ICollection&lt;T&gt;.
		/// </summary>
		public int Count
		{
			get { return this.elements.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the System.Collections.Generic.ICollection&lt;T&gt; is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return this.elements.IsReadOnly; }
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection&lt;T&gt;.
		/// </summary>
		/// <param name="item">The object to remove from the System.Collections.Generic.ICollection&lt;T&gt;.</param>
		/// <returns>true if item was successfully removed from the System.Collections.Generic.ICollection&lt;T&gt;; 
		/// otherwise, false. This method also returns false if item is not found in the original 
		/// System.Collections.Generic.ICollection&lt;T&gt;.
		/// </returns>
		public bool Remove(Value item)
		{
			return this.elements.Remove(item);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A System.Collections.Generic.IEnumerator&lt;T&gt; that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Value> GetEnumerator()
		{
			return this.elements.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.elements.GetEnumerator();
		}
		#endregion
	}
}
