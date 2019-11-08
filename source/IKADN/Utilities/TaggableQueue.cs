﻿// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// LGPL License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Represents a first-in, first-out collection of taggable objects.
	/// </summary>
	/// <typeparam name="TTag">Tag type</typeparam>
	/// <typeparam name="TValue">Element type</typeparam>
	public class TaggableQueue<TTag, TValue> : IEnumerable<KeyValuePair<TTag, TValue>>
	{
		private readonly LinkedList<KeyValuePair<TTag, TValue>> elements = new LinkedList<KeyValuePair<TTag, TValue>>();
		private readonly Dictionary<TTag, LinkedList<TValue>> tagGroups = new Dictionary<TTag, LinkedList<TValue>>();
		private readonly Dictionary<TValue, IndexData> indices = new Dictionary<TValue, IndexData>();

		/// <summary>
		/// Initializes a new empty instance of Ikadn.Utilities.TaggableQueue.
		/// </summary>
		public TaggableQueue()
		{ }
		
		/// <summary>
		/// Initializes an instance of Ikadn.Utilities.TaggableQueue filled with given elements.
		/// </summary>
		/// <param name="elements">Objects and corresponding tags for populating the Ikadn.Utilities.TaggableQueue.</param>
		public TaggableQueue(IEnumerable<KeyValuePair<TTag, TValue>> elements)
		{
			if (this.elements == null)
				throw new ArgumentNullException("elements");

			foreach (var element in elements)
				this.Enqueue(element.Key, element.Value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A System.Collections.Generic.IEnumerator&lt;T&gt; that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<TTag, TValue>> GetEnumerator()
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

		/// <summary>
		/// Gets the number of elements contained in the Ikadn.Utilities.TaggableQueue.
		/// </summary>
		public int Count
		{
			get { return this.elements.Count; }
		}

		/// <summary>
		/// Checks whether there are any elements in Ikadn.Utilities.TaggableQueue.
		/// </summary>
		public bool IsEmpty
		{
			get { return this.elements.Count == 0; }
		}

		/// <summary>
		/// Gets the number of elements with a given tag contained in the Ikadn.Utilities.TaggableQueue.
		/// </summary>
		/// <param name="tag">Object tag.</param>
		/// <returns>Number of elements in question.</returns>
		public int CountOf(TTag tag)
		{
			return this.tagGroups.ContainsKey(tag) ? this.tagGroups[tag].Count : 0;
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the Ikadn.Utilities.TaggableQueue.
		/// </summary>
		/// <returns>The object that is removed from the beginning of the Ikadn.Utilities.TaggableQueue.</returns>
		public TValue Dequeue()
		{
			var element = this.elements.First.Value;

			this.elements.RemoveFirst();
			if (element.Key != null) {
				this.indices.Remove(element.Value);
				this.tagGroups[element.Key].RemoveFirst();
			}

			return element.Value;
		}

		/// <summary>
		/// Removes and returns the first element of the Ikadn.Utilities.TaggableQueue
		/// with specified tag.
		/// </summary>
		/// <param name="tag">Tag of an object to dequeue.</param>
		/// <returns>The object.</returns>
		public TValue Dequeue(TTag tag)
		{
			if (tag == null)
				return this.Dequeue();

			var group = this.tagGroups[tag];
			var element = group.First.Value;

			group.RemoveFirst();
			this.elements.Remove(this.indices[element].ElementIndex);
			this.indices.Remove(element);

			return element;
		}

		/// <summary>
		/// Adds an object to the end of the Ikadn.Utilities.TaggableQueue.
		/// </summary>
		/// <param name="item">The object to add to the end of Ikadn.Utilities.TaggableQueue.
		/// <param name="tag">Tag of the object</param>
		/// </param>
		public void Enqueue(TTag tag, TValue item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			this.elements.AddLast(new KeyValuePair<TTag, TValue>(tag, item));

			if (tag != null)
			{
				if (!this.tagGroups.TryGetValue(tag, out var group))
				{
					group = new LinkedList<TValue>();
					this.tagGroups.Add(tag, group);
				}

				group.AddLast(item);
				this.indices.Add(item, new IndexData(elements.Last, group.Last));
			}
		}

		/// <summary>
		/// Removes an object from the Ikadn.Utilities.TaggableQueue.
		/// </summary>
		/// <param name="item">The object to remove</param>
		public void Remove(TValue item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			var itemIndex = this.indices[item];

			this.elements.Remove(itemIndex.ElementIndex);
			this.tagGroups[itemIndex.ElementIndex.Value.Key].Remove(itemIndex.GroupIndex);
			this.indices.Remove(item);
		}

		class IndexData
		{
			public LinkedListNode<KeyValuePair<TTag, TValue>> ElementIndex { get; private set; }
			public LinkedListNode<TValue> GroupIndex { get; private set; }

			public IndexData(LinkedListNode<KeyValuePair<TTag, TValue>> elementIndex, LinkedListNode<TValue> groupIndex)
			{
				this.ElementIndex = elementIndex;
				this.GroupIndex = groupIndex;
			}
		}
	}
}
