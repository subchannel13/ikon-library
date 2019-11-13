// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON composite object with key-value pairs of nested IKADN objects.
	/// </summary>
	public class IkonComposite : IkonBaseObject, IEnumerable<KeyValuePair<string, IkadnBaseObject>>
	{
		/// <summary>
		/// Data defined tag.
		/// </summary>
		private readonly string dataTag;

		/// <summary>
		/// Collection of the nested IKADN objects.
		/// </summary>
		private readonly IDictionary<string, IkadnBaseObject> members = new Dictionary<string, IkadnBaseObject>();

		/// <summary>
		/// Constructs IKON composite object marked as specified class of data.
		/// </summary>
		/// <param name="dataTag">Tag for IKON composite object.</param>
		public IkonComposite(string dataTag)
		{
			this.dataTag = dataTag;
		}

		/// <summary>
		/// Converts IKON composite object to specified type. Supported target types:
		/// 
		/// Ikadn.Ikon.Values.ObjectValue
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			var target = typeof(T);

			if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Gets or sets nested IKADN object.
		/// </summary>
		/// <param name="memberName">Key of the object</param>
		/// <returns>Nested IKADN object</returns>
		public IkadnBaseObject this[string memberName]
		{
			get { return this.members[memberName]; }
			set
			{
				if (this.members.ContainsKey(memberName))
					this.members[memberName] = value;
				else
					this.members.Add(memberName, value);
			}
		}

		/// <summary>
		/// Builder method for adding an element to IKON object.
		/// </summary>
		/// <param name="key">Key of the object.</param>
		/// <param name="item">IKADN object.</param>
		/// <returns>Instance of the same IKON composite object method is called for.</returns>
		public IkonComposite Add(string key, IkadnBaseObject item)
		{
			this.members.Add(key, item);
			return this;
		}

		/// <summary>
		/// Gets the collection of keys of the nested IKADN objects.
		/// </summary>
		public ICollection<string> Keys
		{
			get { return this.members.Keys; }
		}

		/// <summary>
		/// Tag of the IKON composite object instance.
		/// </summary>
		public override object Tag
		{
			get { return this.dataTag; }
		}

		/// <summary>
		/// Writes an IKON composite object to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			writer.Write(CompositeFactory.OpeningSign);
			writer.Write(" ");
			writer.WriteLine(this.dataTag);
			writer.Indentation.Increase();

			foreach (string key in this.members.Keys)
			{
				writer.Write(key);
				writer.Write(" ");
				this.members[key].Compose(writer);
			}

			writer.Indentation.Decrease();
			writer.Write(CompositeFactory.ClosingChar);

			WriteReferences(writer);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the key-value pairs of
		/// Ikadn.Ikon.Types.IkonComposite.
		/// </summary>
		/// <returns>An System.Collections.IEnumerator object that can be used to iterate through the key-value pairs.</returns>
		public IEnumerator<KeyValuePair<string, IkadnBaseObject>> GetEnumerator()
		{
			return this.members.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the key-value pairs of
		/// Ikadn.Ikon.Types.IkonComposite.
		/// </summary>
		/// <returns>An System.Collections.IEnumerator object that can be used to iterate through the key-value pairs.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.members.GetEnumerator();
		}
	}
}
