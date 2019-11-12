// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON reference object.
	/// </summary>
	public class IkonReference : IkonBaseObject
	{
		/// <summary>
		/// Tag for IKON referecne objects.
		/// </summary>
		public static readonly string TypeTag = "IKON.Reference";

		/// <summary>
		/// Name of the object reference.
		/// </summary>
		private readonly string name;

		/// <summary>
		/// Constructs IKON reference object with specified name.
		/// </summary>
		/// <param name="name">Name of the referenced object.</param>
		public IkonReference(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Tag of the IKADN object instance.
		/// </summary>
		public override object Tag
		{
			get { return TypeTag; }
		}

		/// <summary>
		/// Converts IKON reference object to specified type. Supported target types:
		/// 
		/// System.string
		/// Ikadn.Ikon.Types.IkonText
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			Type target = typeof(T);

			if (target == typeof(string))
				return (T)(object)name;
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Writes an IKON reference object to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new System.ArgumentNullException("writer");

			writer.Write(ReferencedFactory.OpeningSign);
			writer.Write(name);

			WriteReferences(writer);
		}
	}
}
