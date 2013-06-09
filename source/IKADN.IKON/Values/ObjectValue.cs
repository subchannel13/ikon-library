using System.Collections.Generic;
using Ikadn.Ikon.Factories;
using Ikadn.Utilities;
using System;

namespace Ikadn.Ikon.Values
{
	/// <summary>
	/// IKON composite value with key-value pairs of nested IKADN values.
	/// </summary>
	public class ObjectValue : IkonBaseValue
	{
		/// <summary>
		/// The name of the data class contained in this instance
		/// </summary>
		private string className;

		/// <summary>
		/// Collection of the nested IKADN values.
		/// </summary>
		private IDictionary<string, IkadnBaseObject> members = new Dictionary<string, IkadnBaseObject>();

		/// <summary>
		/// Constructs IKON composite value marked as specified class of data.
		/// </summary>
		/// <param name="className">Name of the data class.</param>
		public ObjectValue(string className)
		{
			this.className = className;
		}

		/// <summary>
		/// Converts IKON object value to specified type. Supported target types:
		/// 
		/// Ikadn.Ikon.Values.ObjectValue
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);

			if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Gets or sets nested IKADN value.
		/// </summary>
		/// <param name="memberName">Key of the value</param>
		/// <returns>Nested IKADN value</returns>
		public IkadnBaseObject this[string memberName]
		{
			get { return members[memberName]; }
			set
			{
				if (members.ContainsKey(memberName))
					members[memberName] = value;
				else
					members.Add(memberName, value);
			}
		}

		/// <summary>
		/// Builder method for adding an element to IKON object.
		/// </summary>
		/// <param name="key">Key of the value.</param>
		/// <param name="value">Element's value.</param>
		/// <returns>Instance of the same IKON object method is called for.</returns>
		public ObjectValue Add(string key, IkadnBaseObject value)
		{
			members.Add(key, value);
			return this;
		}

		/// <summary>
		/// Gets the collection of keys of the nested IKADN values.
		/// </summary>
		public ICollection<string> Keys
		{
			get { return members.Keys; }
		}

		/// <summary>
		/// Gets the name of the data class contained in this instance.
		/// </summary>
		public override object Tag
		{
			get { return className; }
		}

		/// <summary>
		/// Writes an IKON composite value to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.Write(ObjectFactory.OpeningSign.ToString());
			writer.Write(" ");
			writer.WriteLine(className);
			writer.Indentation.Increase();

			foreach (string key in members.Keys)
			{
				writer.Write(key);
				writer.Write(" ");
				members[key].Compose(writer);
			}

			writer.Indentation.Decrease();
			writer.Write(ObjectFactory.ClosingChar.ToString());

			WriteReferences(writer);
		}
	}
}
