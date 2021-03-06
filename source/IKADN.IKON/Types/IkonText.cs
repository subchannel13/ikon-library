﻿// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;
using System.Text;
using Ikadn.Ikon.Factories;

namespace Ikadn.Ikon.Types
{
	/// <summary>
	/// IKON textual object.
	/// </summary>
	public class IkonText : IkonBaseObject
	{
		/// <summary>
		/// Tag for IKON text objects.
		/// </summary>
		public static readonly string TypeTag = "IKON.Text";

		private static readonly string[] LineEnds = new[] { "\r\n", "\n" };
		
		private readonly string text;

		/// <summary>
		/// Constructs IKON textual object with specified contents.
		/// </summary>
		/// <param name="text">Contents</param>
		public IkonText(string text)
		{
			this.text = text;
		}

		/// <summary>
		/// Tag of the IKADN object instance.
		/// </summary>
		public override object Tag
		{
			get { return TypeTag; }
		}

		/// <summary>
		/// Converts IKON text object to specified type. Supported target types:
		/// 
		/// System.string
		/// Ikadn.Ikon.Types.IkonText
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Converted value</returns>
		public override T To<T>()
		{
			var target = typeof(T);

			if (target == typeof(string))
				return (T)(object)this.text;
			else if (target.IsAssignableFrom(this.GetType()))
				return (T)(object)this;
			else
				throw new InvalidOperationException("Cast to " + target.Name + " is not supported for " + Tag);
		}

		/// <summary>
		/// Implicit conversion from IKADN textual object to System.String.
		/// </summary>
		public static implicit operator string(IkonText ikonText)
		{
			if (ikonText == null)
				return null;

			return ikonText.text;
		}

		/// <summary>
		/// Writes an IKON text object to the composer.
		/// </summary>
		/// <param name="writer">Target composer.</param>
		protected override void DoCompose(IkadnWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			if (this.text.Contains("\n"))
				composeBlock(writer);
			else
				composeLine(writer);

			this.WriteReferences(writer);
		}
		
		private void composeLine(IkadnWriter writer)
		{
			var sb = new StringBuilder(text);
			sb.Replace(@"\", @"\\");
			sb.Replace(@"""", @"\""");
			sb.Replace("\n", @"\n");
			sb.Replace("\r", @"\r");
			sb.Replace("\t", @"\t");

			writer.Write(TextFactory.OpeningSign);
			writer.Write(sb.ToString());
			writer.Write(TextFactory.ClosingChar);
		}

		private void composeBlock(IkadnWriter writer)
		{
			writer.WriteLine(TextBlockFactory.OpeningSign);
			writer.Indentation.Increase();
			
			foreach(var line in this.text.Split(LineEnds, StringSplitOptions.None))
				writer.WriteLine(line);

			writer.Indentation.Decrease();
			writer.Write(TextBlockFactory.ClosingChar);
		}

		/// <summary>
		/// Returns textual contents of the Ikadn.Ikon.Types.IkonText
		/// </summary>
		/// <returns>Text of the object</returns>
		public override string ToString()
		{
			return this.text;
		}
	}
}
