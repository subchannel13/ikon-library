using System.Collections.Generic;
using Ikadn.Ikon.Factories;
using Ikadn.Utilities;
using System;
using System.Reflection;

namespace Ikadn.Ikon.Values
{
	/// <summary>
	/// Array of IKADN values.
	/// </summary>
	public class ArrayValue : IkonBaseValue, IList<IkadnBaseObject>
	{
		/// <summary>
		/// Type name of IKON arrays.
		/// </summary>
		public const string ValueTypeName = "IKON.Array";

		private static MethodInfo baseConverterMethod = null;

		private IList<IkadnBaseObject> elements;

		/// <summary>
		/// Constructs IKON array of IKADN values
		/// </summary>
		/// <param name="values">Initial array contents.</param>
		public ArrayValue(IList<IkadnBaseObject> values)
		{
			this.elements = values;
		}

		/// <summary>
		/// Constructs IKON array
		/// </summary>
		public ArrayValue()
		{
			this.elements = new List<IkadnBaseObject>();
		}

		/// <summary>
		/// Type name of the IKADN value instance.
		/// </summary>
		public override object Tag
		{
			get { return ValueTypeName; }
		}

		/// <summary>
		/// Converts IKON array value to specified type. Supported target types:
		/// 
		/// System.Collections.Generic.IList&lt;Value&gt;
		/// Ikadn.Ikon.Values.ArrayValue
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);

			if (target.IsAssignableFrom(typeof(IList<IkadnBaseObject>)))
				return (T)elements;
			else if (target.IsArray) {
				if (baseConverterMethod == null)
					baseConverterMethod = typeof(IkadnBaseObject).GetMethod("To", new Type[] { });

				MethodInfo converterMethod = baseConverterMethod.MakeGenericMethod(target.GetElementType());
				Array array = Array.CreateInstance(target.GetElementType(), this.elements.Count);

				for (int i = 0; i < this.elements.Count; i++)
					array.SetValue(converterMethod.Invoke(this.elements[i], null), i);

				return (T)(object)array;
			}
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Builder method fo adding one or more elements to IKON array.
		/// </summary>
		/// <param name="values">Elements to be added.</param>
		/// <returns>Instance of the same IKON array method is called for.</returns>
		public ArrayValue Add(params IkadnBaseObject[] values)
		{
			if (values == null)
				throw new System.ArgumentNullException("values");

			foreach (var item in values)
				this.elements.Add(item);
			
			return this;
		}

		/// <summary>
		/// Writes an IKON array to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.WriteLine(ArrayFactory.OpeningSign.ToString());
			writer.Indentation.Increase();

			foreach (IkadnBaseObject value in elements)
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
		public int IndexOf(IkadnBaseObject item)
		{
			return this.elements.IndexOf(item);
		}

		/// <summary>
		/// Inserts an item to the System.Collections.Generic.IList&lt;T&gt; at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the System.Collections.Generic.IList&lt;T&gt;.</param>
		public void Insert(int index, IkadnBaseObject item)
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
		public IkadnBaseObject this[int index]
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
		public void Add(IkadnBaseObject item)
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
		public bool Contains(IkadnBaseObject item)
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
		public void CopyTo(IkadnBaseObject[] array, int arrayIndex)
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
		public bool Remove(IkadnBaseObject item)
		{
			return this.elements.Remove(item);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A System.Collections.Generic.IEnumerator&lt;T&gt; that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<IkadnBaseObject> GetEnumerator()
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
