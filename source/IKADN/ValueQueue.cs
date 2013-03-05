using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ikadn
{
	/// <summary>
	/// Represents a first-in, first-out collection of IKADN values.
	/// </summary>
	public class ValueQueue : IEnumerable<IkadnBaseValue>
	{
		LinkedList<IkadnBaseValue> values = new LinkedList<IkadnBaseValue>();
		Dictionary<string, Queue<IkadnBaseValue>> typedQueue = new Dictionary<string, Queue<IkadnBaseValue>>();
		Dictionary<IkadnBaseValue, LinkedListNode<IkadnBaseValue>> indices = new Dictionary<IkadnBaseValue, LinkedListNode<IkadnBaseValue>>();

		/// <summary>
		/// Initializes a new empty instance of Ikadn.ValueQueue.
		/// </summary>
		public ValueQueue()
		{ }

		/// <summary>
		/// Initializes an instance of Ikadn.ValueQueue filled with given elements.
		/// </summary>
		/// <param name="values">IKADN values for populating the Ikadn.ValueQueue.</param>
		public ValueQueue(IEnumerable<IkadnBaseValue> values)
		{
			if (values == null)
				throw new ArgumentNullException("values");

			foreach (IkadnBaseValue value in values)
				Enqueue(value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A System.Collections.Generic.IEnumerator&lt;T&gt; that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<IkadnBaseValue> GetEnumerator()
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
		/// Gets the number of elements contained in the Ikadn.ValueQueue.
		/// </summary>
		public int Count
		{
			get { return values.Count; }
		}

		/// <summary>
		/// Tests whether there are any elements in Ikadn.ValueQueue.
		/// </summary>
		public bool IsEmpty
		{
			get { return values.Count == 0; }
		}

		/// <summary>
		/// Gets the number of elements on given type contained in the Ikadn.ValueQueue.
		/// </summary>
		/// <param name="typeName">Type name of IKADN values to count.</param>
		/// <returns>Number of elements in question.</returns>
		public int CountOf(string typeName)
		{
			return (typedQueue.ContainsKey(typeName)) ? typedQueue[typeName].Count : 0;
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Ikadn.ValueQueue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the Ikadn.ValueQueue.</returns>
		public IkadnBaseValue Dequeue()
		{
			IkadnBaseValue value = values.First.Value;
			
			values.RemoveFirst();
			indices.Remove(value);
			
			return typedQueue[value.TypeName].Dequeue();
		}

		/// <summary>
		/// Removes and returns the first element of the Ikadn.ValueQueue with specified type name.
		/// </summary>
		/// <param name="typeName">Type name of IKADN value to dequeue.</param>
		/// <returns>The IKADN value.</returns>
		public IkadnBaseValue Dequeue(string typeName)
		{
			IkadnBaseValue value = typedQueue[typeName].Dequeue();
			
			values.Remove(indices[value]);
			indices.Remove(value);

			return value;
		}

		/// <summary>
		/// Adds an object to the end of the Ikadn.ValueQueue.
		/// </summary>
		/// <param name="item">The object to add to the Ikadn.ValueQueue. The value can be null for reference types.
		/// </param>
		public void Enqueue(IkadnBaseValue item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			if (!typedQueue.ContainsKey(item.TypeName))
				typedQueue.Add(item.TypeName, new Queue<IkadnBaseValue>());
			
			values.AddLast(item);
			indices.Add(item, values.Last);
			typedQueue[item.TypeName].Enqueue(item);
		}
	}
}
