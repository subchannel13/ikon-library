using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ikon.Ston.Values;
using Ikon;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class TextTests
	{
		const char Q = '"';

		[TestMethod]
		public void TextType()
		{
			string input = Q + "some text" + Q;

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(value.TypeName, Text.ValueTypeName);
		}

		[TestMethod]
		public void TextReadValueSimple()
		{
			string inputValue = "some text";
			string input = Q + inputValue + Q;

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Text;

			Assert.AreEqual(value.GetText, inputValue);
		}
		
		[TestMethod]
		public void TextReadValueEscapes()
		{
			string inputValue = "some text\\nnew line\\ttab\\n\\ranother line and \\\" qoute";
			string expectedValue = "some text\nnew line\ttab\n\ranother line and \" qoute";
			string input = Q + inputValue + Q;

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Text; 

			Assert.AreEqual(value.GetText, expectedValue);
		}

		[TestMethod]
		public void TextWriteValueSimple()
		{
			string inputValue = "some text";
			string ikonData = Q + inputValue + Q;
			StringBuilder output = new StringBuilder();

			IkonWriter writer = new IkonWriter(new StringWriter(output));
			var value = new Text(inputValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void TextWriteValueEscapes()
		{
			string rawIkonData = "some text\\nnew line\\ttab\\n\\ranother line and \\\" qoute";
			string inputValue = "some text\nnew line\ttab\n\ranother line and \" qoute";
			string ikonData = Q + rawIkonData + Q;
			StringBuilder output = new StringBuilder();

			IkonWriter writer = new IkonWriter(new StringWriter(output));
			var value = new Text(inputValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}
	}
}
