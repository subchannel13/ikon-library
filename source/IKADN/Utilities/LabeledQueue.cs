// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Represents a first-in, first-out collection of label objects.
	/// </summary>
	/// <typeparam name="TLabel">Label type</typeparam>
	/// <typeparam name="TValue">Element type</typeparam>
	public class LabeledQueue<TLabel, TValue> : IEnumerable<KeyValuePair<TLabel, TValue>>
	{
		private readonly LinkedList<KeyValuePair<TLabel, TValue>> elements = new LinkedList<KeyValuePair<TLabel, TValue>>();
		private readonly Dictionary<TLabel, LinkedList<TValue>> labelGroups = new Dictionary<TLabel, LinkedList<TValue>>();
		private readonly Dictionary<TValue, IndexData> indices = new Dictionary<TValue, IndexData>();

		/// <summary>
		/// Initializes a new empty instance of Ikadn.Utilities.LabeledQueue.
		/// </summary>
		public LabeledQueue()
		{ }
		
		/// <summary>
		/// Initializes an instance of Ikadn.Utilities.LabeledQueue filled with given elements.
		/// </summary>
		/// <param name="elements">Objects and corresponding labels for populating the Ikadn.Utilities.LabeledQueue</param>
		public LabeledQueue(IEnumerable<KeyValuePair<TLabel, TValue>> elements)
		{
			if (elements == null)
				throw new ArgumentNullException(nameof(elements));

			foreach (var element in elements)
				this.Enqueue(element.Key, element.Value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A System.Collections.Generic.IEnumerator&lt;T&gt; that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<TLabel, TValue>> GetEnumerator()
		{
			return this.elements.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>A System.Collections.IEnumerator object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.elements.GetEnumerator();
		}

		/// <summary>
		/// Gets the number of elements contained in the Ikadn.Utilities.LabeledQueue.
		/// </summary>
		public int Count =>  this.elements.Count;

		/// <summary>
		/// Checks whether there are any elements in Ikadn.Utilities.LabeledQueue.
		/// </summary>
		public bool IsEmpty => this.elements.Count == 0;

		/// <summary>
		/// Gets the number of elements with a given label contained in the Ikadn.Utilities.LabeledQueue.
		/// </summary>
		/// <param name="label">Label of interest</param>
		/// <returns>Number of elements in question.</returns>
		public int CountOf(TLabel label)
		{
			return this.labelGroups.ContainsKey(label) ? this.labelGroups[label].Count : 0;
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Ikadn.Utilities.LabeledQueue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the Ikadn.Utilities.LabeledQueue.</returns>
		public TValue Dequeue()
		{
			var element = this.elements.First.Value;

			this.elements.RemoveFirst();
			if (element.Key != null) {
				this.indices.Remove(element.Value);
				this.labelGroups[element.Key].RemoveFirst();
			}

			return element.Value;
		}

		/// <summary>
		/// Removes and returns the first element of the Ikadn.Utilities.LabeledQueue
		/// with a specified label.
		/// </summary>
		/// <param name="label">Label of an object to dequeue</param>
		/// <returns>The object.</returns>
		public TValue Dequeue(TLabel label)
		{
			if (label == null)
				return this.Dequeue();

			var group = this.labelGroups[label];
			var element = group.First.Value;

			group.RemoveFirst();
			this.elements.Remove(this.indices[element].ElementIndex);
			this.indices.Remove(element);

			return element;
		}

		/// <summary>
		/// Adds an object to the end of the Ikadn.Utilities.LabeledQueue.
		/// </summary>
		/// <param name="label">Label of the object</param>
		/// <param name="item">The object to add to the end of Ikadn.Utilities.LabeledQueue</param>
		public void Enqueue(TLabel label, TValue item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			this.elements.AddLast(new KeyValuePair<TLabel, TValue>(label, item));

			if (label != null)
			{
				if (!this.labelGroups.TryGetValue(label, out var group))
				{
					group = new LinkedList<TValue>();
					this.labelGroups.Add(label, group);
				}

				group.AddLast(item);
				this.indices.Add(item, new IndexData(elements.Last, group.Last));
			}
		}

		/// <summary>
		/// Removes an object from the Ikadn.Utilities.LabeledQueue.
		/// </summary>
		/// <param name="item">The object to remove</param>
		public void Remove(TValue item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			var itemIndex = this.indices[item];

			this.elements.Remove(itemIndex.ElementIndex);
			this.labelGroups[itemIndex.ElementIndex.Value.Key].Remove(itemIndex.GroupIndex);
			this.indices.Remove(item);
		}

		class IndexData
		{
			public LinkedListNode<KeyValuePair<TLabel, TValue>> ElementIndex { get; private set; }
			public LinkedListNode<TValue> GroupIndex { get; private set; }

			public IndexData(LinkedListNode<KeyValuePair<TLabel, TValue>> elementIndex, LinkedListNode<TValue> groupIndex)
			{
				this.ElementIndex = elementIndex;
				this.GroupIndex = groupIndex;
			}
		}
	}
}
