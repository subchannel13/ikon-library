using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ikadn.Ikon.Factories;
using Ikadn;
using Ikadn.Ikon.Types;
using System.IO;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class TextBlockTests
	{
		const char O = TextBlockFactory.OpeningSign;
		const char C = TextBlockFactory.ClosingChar;
		static string N = Environment.NewLine;
		static string NT = Environment.NewLine + "\t";

		[TestMethod]
		public void TextBlockType()
		{
			string input = O + NT + "some text" + N + C;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(value.Tag, IkonText.TypeTag);
		}

		[TestMethod]
		public void TextBlockReadSingleLine()
		{
			string inputValue = "some text";
			string input = O + NT + inputValue + N + C;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonText;

			Assert.AreEqual(value.To<string>(), inputValue);
		}

		[TestMethod]
		public void TextBlockReadMultiline()
		{
			string expectedValue = "some text" + N + N + "new lines" + N + "one more line";
			string input = O + NT + "some text" + NT + NT + "new lines" + NT + "one more line" + N + C;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonText;

			Assert.AreEqual(value.To<string>(), expectedValue);
		}

		[TestMethod]
		public void TextWriteMultiline()
		{
			string inputValue = "some text" + N + N + "new lines" + N + "one more line";
			string ikonData = O + NT + "some text" + NT + NT + "new lines" + NT + "one more line" + N + C;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonText(inputValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}
	}
}
