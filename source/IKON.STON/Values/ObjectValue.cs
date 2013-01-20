using System.Collections.Generic;
using Ikon.Ston.Factories;
using Ikon.Utilities;
using System;

namespace Ikon.Ston.Values
{
	/// <summary>
	/// IKSTON composite value with key-value pairs of nested IKON values.
	/// </summary>
	public class ObjectValue : Value
	{
		/// <summary>
		/// The name of the data class contained in this instance
		/// </summary>
		protected readonly string className;
		/// <summary>
		/// Collection of the nested IKON values.
		/// </summary>
		protected IDictionary<string, Value> members = new Dictionary<string, Value>();

		/// <summary>
		/// Constructs IKSTON composite value marked as specified class of data.
		/// </summary>
		/// <param name="className">Name of the data class.</param>
		public ObjectValue(string className)
		{
			this.className = className;
		}

		/// <summary>
		/// Converts IKSTON object value to specified type. Supported target types:
		/// 
		/// Ikon.Ston.Values.ObjectValue
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T As<T>()
		{
			Type target = typeof(T);

			if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + TypeName);
		}

		/// <summary>
		/// Gets or sets nested IKON value.
		/// </summary>
		/// <param name="memberName">Key of the value</param>
		/// <returns>Nested IKON value</returns>
		public Value this[string memberName]
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
		/// Builder method for adding an element to IKSTON object.
		/// </summary>
		/// <param name="key">Key of the value.</param>
		/// <param name="value">Element's value.</param>
		/// <returns>Instance of the same IKSTON object method is called for.</returns>
		public ObjectValue Add(string key, Value value)
		{
			members.Add(key, value);
			return this;
		}

		/// <summary>
		/// Gets the collection of keys of the nested IKON values.
		/// </summary>
		public ICollection<string> Keys
		{
			get { return members.Keys; }
		}

		/// <summary>
		/// Gets the name of the data class contained in this instance.
		/// </summary>
		public override string TypeName
		{
			get { return className; }
		}

		/// <summary>
		/// Writes an IKSTON composite value to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkonWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("composer");

			writer.Write(ObjectFactory.OpeningSign.ToString());
			writer.Write(" ");
			writer.WriteLine(className);
			writer.Indentation.Increase();

			foreach (string key in members.Keys)
			{
				writer.Write(key);
				writer.Write(" ");
				//writer.Write();
				members[key].Compose(writer);
				//writer.EndLine();
			}

			writer.Indentation.Decrease();
			writer.Write(ObjectFactory.ClosingChar.ToString());
		}
	}
}
