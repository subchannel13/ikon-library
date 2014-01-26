using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ikadn.Ikon.Types;
using Ikadn;
using Ikadn.Ikon.Factories;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class TextTests
	{
		const char Q = '"';
		static string N = Environment.NewLine;

		[TestMethod]
		public void TextType()
		{
			string input = Q + "some text" + Q;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(value.Tag, IkonText.TypeTag);
		}

		[TestMethod]
		public void TextReadValueSimple()
		{
			string inputValue = "some text";
			string input = Q + inputValue + Q;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonText;

			Assert.AreEqual(value.To<string>(), inputValue);
		}
		
		[TestMethod]
		public void TextReadValueEscapes()
		{
			string inputValue = "some text\\nnew line\\ttab\\n\\ranother line and \\\" qoute";
			string expectedValue = "some text\nnew line\ttab\n\ranother line and \" qoute";
			string input = Q + inputValue + Q;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonText;

			Assert.AreEqual(value.To<string>(), expectedValue);
		}

		[TestMethod]
		public void TextReadValueEscapedUnicode16()
		{
			string inputValue = "some text \\u2026 \\uffff \\uAAAA more text";
			string expectedValue = "some text … \uffff \uAAAA more text";
			string input = Q + inputValue + Q;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonText;

			Assert.AreEqual(value.To<string>(), expectedValue);
		}

		[TestMethod]
		public void TextReadValueEscapedUnicode32()
		{
			string inputValue = "some text \\U00000043 \\U0002336A \\UFFFFFFff more text";
			string expectedValue = "some text \u0000C \u0002\u336A \uFFFF\uFFFF more text";
			string input = Q + inputValue + Q;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonText;

			Assert.AreEqual(value.To<string>(), expectedValue);
		}

		[TestMethod]
		public void TextWriteValueSimple()
		{
			string inputValue = "some text";
			string ikonData = Q + inputValue + Q;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonText(inputValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void TextWriteValueEscapes()
		{
			string rawIkonData = "some text\\ttab and \\\" qoute";
			string inputValue = "some text\ttab and \" qoute";
			string ikonData = Q + rawIkonData + Q;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonText(inputValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void TextWriteValueMultiline()
		{
			string NT = N + "\t";
			string rawData = NT + "some text" + NT + "new line\ttab" + NT + "another line and \" qoute";
			string inputValue = "some text" + N + "new line\ttab" + N + "another line and \" qoute";
			string expectedData = TextBlockFactory.OpeningSign + rawData + N + TextBlockFactory.ClosingChar;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonText(inputValue);
			value.Compose(writer);

			Assert.AreEqual(expectedData, output.ToString().Trim());
		}
	}
}
