using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class ParserTest
	{
		static string TreeValueSampleDocument = "{First}" + Environment.NewLine +
			"{Second}" + Environment.NewLine +
			"{Third}";

		[TestMethod]
		public void ParseEmpty()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(""));

			Assert.AreEqual(false, parser.HasNext());
		}

		[TestMethod]
		public void ParseSecond()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader("{First} {Second}"));
			var value = parser.ParseNext("Second");

			Assert.AreEqual("Second", value.Tag);
		}

		[TestMethod]
		public void ParseThird()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSampleDocument));
			var value = parser.ParseNext("Third");

			Assert.AreEqual("Third", value.Tag);
		}

		[TestMethod]
		public void ParseThirdThenParseAll()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSampleDocument));
			var x = parser.ParseNext("Third");
			var value = parser.ParseAll();

			Assert.AreEqual(2, value.Count);
		}

		[TestMethod]
		public void HasNext()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSampleDocument));

			Assert.AreEqual(true, parser.HasNext());
		}

		[TestMethod]
		public void HasNextTag()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSampleDocument));

			Assert.AreEqual(true, parser.HasNext("Third"));
		}
	}
}
