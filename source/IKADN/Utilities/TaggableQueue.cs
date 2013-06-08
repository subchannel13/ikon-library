using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Represents a first-in, first-out collection of IKADN values.
	/// </summary>
	public class TaggableQueue<K, V> : IEnumerable<KeyValuePair<K, V>>
	{
		LinkedList<KeyValuePair<K, V>> elements = new LinkedList<KeyValuePair<K, V>>();
		Dictionary<K, Queue<V>> tagGroups = new Dictionary<K, Queue<V>>();
		Dictionary<V, LinkedListNode<KeyValuePair<K, V>>> indices = new Dictionary<V, LinkedListNode<KeyValuePair<K, V>>>();

		/// <summary>
		/// Initializes a new empty instance of Ikadn.ValueQueue.
		/// </summary>
		public TaggableQueue()
		{ }
		
		/// <summary>
		/// Initializes an instance of Ikadn.ValueQueue filled with given elements.
		/// </summary>
		/// <param name="range">IKADN values for populating the Ikadn.ValueQueue.</param>
		public TaggableQueue(IEnumerable<KeyValuePair<K, V>> range)
		{
			if (range == null)
				throw new ArgumentNullException("range");

			foreach (var element in range)
				Enqueue(element.Key, element.Value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A System.Collections.Generic.IEnumerator&lt;T&gt; that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return elements.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return elements.GetEnumerator();
		}

		/// <summary>
		/// Gets the number of elements contained in the Ikadn.ValueQueue.
		/// </summary>
		public int Count
		{
			get { return elements.Count; }
		}

		/// <summary>
		/// Tests whether there are any elements in Ikadn.ValueQueue.
		/// </summary>
		public bool IsEmpty
		{
			get { return elements.Count == 0; }
		}

		/// <summary>
		/// Gets the number of elements on given type contained in the Ikadn.ValueQueue.
		/// </summary>
		/// <param name="tag">Type name of IKADN values to count.</param>
		/// <returns>Number of elements in question.</returns>
		public int CountOf(K tag)
		{
			return (tagGroups.ContainsKey(tag)) ? tagGroups[tag].Count : 0;
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Ikadn.ValueQueue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the Ikadn.ValueQueue.</returns>
		public V Dequeue()
		{
			var value = elements.First.Value;
			
			elements.RemoveFirst();
			if (value.Key != null) {
				indices.Remove(value.Value);
				tagGroups[value.Key].Dequeue();
			}

			return value.Value;
		}

		/// <summary>
		/// Removes and returns the first element of the Ikadn.ValueQueue with specified type name.
		/// </summary>
		/// <param name="tag">Type name of IKADN value to dequeue.</param>
		/// <returns>The IKADN value.</returns>
		public V Dequeue(K tag)
		{
			if (tag == null)
				return Dequeue();

			V value = tagGroups[tag].Dequeue();
			
			elements.Remove(indices[value]);
			indices.Remove(value);

			return value;
		}

		/// <summary>
		/// Adds an object to the end of the Ikadn.ValueQueue.
		/// </summary>
		/// <param name="item">The object to add to the Ikadn.ValueQueue. The value can be null for reference types.
		/// <param name="tag">Tag of the object</param>
		/// </param>
		public void Enqueue(K tag, V item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			elements.AddLast(new KeyValuePair<K, V>(tag, item));
			
			if (tag != null) {
				if (!tagGroups.ContainsKey(tag))
					tagGroups.Add(tag, new Queue<V>());
				indices.Add(item, elements.Last);
				tagGroups[tag].Enqueue(item);
			}
		}
	}
}
