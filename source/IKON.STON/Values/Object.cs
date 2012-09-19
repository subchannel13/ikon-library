using System.Collections.Generic;
using IKON.STON.Factories;
using IKON.Utils;

namespace IKON.STON.Values
{
	/// <summary>
	/// IKSTON composite value with key-value pairs of nested IKON values.
	/// </summary>
	public class Object : Value
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
		public Object(string className)
		{
			this.className = className;
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
		/// Builder method fo adding an element to IKSTON object.
		/// </summary>
		/// <param name="key">Key of the value.</param>
		/// <param name="value">Element's value.</param>
		/// <returns>Instance of the same IKSTON object method is called for.</returns>
		public Object Add(string key, Value value)
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
		/// <param name="composer">Target composer.</param>
		public override void Compose(Composer composer)
		{
			composer.WriteLine(ObjectFactory.OpeningSign.ToString());
			composer.Indentation.Increase();

			foreach (string key in members.Keys)
			{
				composer.Write(key);
				composer.Write(" ");
				composer.Write(members[key]);
				composer.EndLine();
			}

			composer.Indentation.Decrease();
			composer.Write(ObjectFactory.ClosingChar.ToString());
		}
	}
}
