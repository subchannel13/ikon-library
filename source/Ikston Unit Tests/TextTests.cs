using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ikadn.Ikon.Values;
using Ikadn;

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

			IkadnParser parser = new Ikadn.Ikon.Parser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(value.Tag, TextValue.ValueTypeName);
		}

		[TestMethod]
		public void TextReadValueSimple()
		{
			string inputValue = "some text";
			string input = Q + inputValue + Q;

			IkadnParser parser = new Ikadn.Ikon.Parser(new StringReader(input));
			var value = parser.ParseNext() as TextValue;

			Assert.AreEqual(value.To<string>(), inputValue);
		}
		
		[TestMethod]
		public void TextReadValueEscapes()
		{
			string inputValue = "some text\\nnew line\\ttab\\n\\ranother line and \\\" qoute";
			string expectedValue = "some text\nnew line\ttab\n\ranother line and \" qoute";
			string input = Q + inputValue + Q;

			IkadnParser parser = new Ikadn.Ikon.Parser(new StringReader(input));
			var value = parser.ParseNext() as TextValue;

			Assert.AreEqual(value.To<string>(), expectedValue);
		}

		[TestMethod]
		public void TextWriteValueSimple()
		{
			string inputValue = "some text";
			string ikonData = Q + inputValue + Q;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new TextValue(inputValue);
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

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new TextValue(inputValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}
	}
}
