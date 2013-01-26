using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ikon
{
	/// <summary>
	/// Represents a first-in, first-out collection of IKON values.
	/// </summary>
	public class ValueQueue : IEnumerable<Value>
	{
		LinkedList<Value> values = new LinkedList<Value>();
		Dictionary<string, Queue<Value>> typedQueue = new Dictionary<string, Queue<Value>>();
		Dictionary<Value, LinkedListNode<Value>> indices = new Dictionary<Value, LinkedListNode<Value>>();

		/// <summary>
		/// Initializes a new empty instance of Ikon.ValueQueue.
		/// </summary>
		public ValueQueue()
		{ }

		/// <summary>
		/// Initializes an instance of Ikon.ValueQueue filled with given elements.
		/// </summary>
		/// <param name="values">IKON values for populating the Ikon.ValueQueue.</param>
		public ValueQueue(IEnumerable<Value> values)
		{
			if (values == null)
				throw new ArgumentNullException("values");

			foreach (Value value in values)
				Enqueue(value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A System.Collections.Generic.IEnumerator&lt;T&gt; that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Value> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

		/// <summary>
		/// Gets the number of elements contained in the Ikon.ValueQueue.
		/// </summary>
		public int Count
		{
			get { return values.Count; }
		}

		/// <summary>
		/// Tests whether there are any elements in Ikon.ValueQueue.
		/// </summary>
		public bool IsEmpty
		{
			get { return values.Count == 0; }
		}

		/// <summary>
		/// Gets the number of elements on given type contained in the Ikon.ValueQueue.
		/// </summary>
		/// <param name="typeName">Type name of IKON values to count.</param>
		/// <returns>Number of elements in question.</returns>
		public int CountOf(string typeName)
		{
			return (typedQueue.ContainsKey(typeName)) ? typedQueue[typeName].Count : 0;
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Ikon.ValueQueue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the Ikon.ValueQueue.</returns>
		public Value Dequeue()
		{
			Value value = values.First.Value;
			
			values.RemoveFirst();
			indices.Remove(value);
			
			return typedQueue[value.TypeName].Dequeue();
		}

		/// <summary>
		/// Removes and returns the first element of the Ikon.ValueQueue with specified type name.
		/// </summary>
		/// <param name="typeName">Type name of IKON value to dequeue.</param>
		/// <returns>The IKON value.</returns>
		public Value Dequeue(string typeName)
		{
			Value value = typedQueue[typeName].Dequeue();
			
			values.Remove(indices[value]);
			indices.Remove(value);

			return value;
		}

		/// <summary>
		/// Adds an object to the end of the Ikon.ValueQueue.
		/// </summary>
		/// <param name="item">The object to add to the Ikon.ValueQueue. The value can be null for reference types.
		/// </param>
		public void Enqueue(Value item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			if (!typedQueue.ContainsKey(item.TypeName))
				typedQueue.Add(item.TypeName, new Queue<Value>());
			
			values.AddLast(item);
			indices.Add(item, values.Last);
			typedQueue[item.TypeName].Enqueue(item);
		}
	}
}
