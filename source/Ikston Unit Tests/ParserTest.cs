using System;
using System.IO;
using Ikadn.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class ParserTest
	{
		static string TreeValueSingleDocument = "{First}" + Environment.NewLine +
			"{Second}" + Environment.NewLine +
			"{Third}";

		readonly NamedStream[] TreeValueSeparateDocuments = new[] {
				new NamedStream(new StringReader("{First}"), "stream 0"),
				new NamedStream(new StringReader("{Second}"), "stream 1"),
				new NamedStream(new StringReader("{Third}"), "stream 2")
			};

	[TestMethod]
		public void ParseEmpty()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(""));

			Assert.AreEqual(false, parser.HasNext());
		}

		[TestMethod]
		public void ParseEmptyMultistream()
		{
			var parser = new Ikadn.Ikon.IkonParser(new[] {
				new NamedStream(new StringReader(""), "0"),
				new NamedStream(new StringReader(""), "1"),
				new NamedStream(new StringReader(""), "2")
			});

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
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSingleDocument));
			var value = parser.ParseNext("Third");

			Assert.AreEqual("Third", value.Tag);
		}

		[TestMethod]
		public void ParseThirdMultistream()
		{
			var parser = new Ikadn.Ikon.IkonParser(TreeValueSeparateDocuments);
			var value = parser.ParseNext("Third");

			Assert.AreEqual("Third", value.Tag);
		}

		[TestMethod]
		public void ParseThirdThenParseAll()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSingleDocument));
			var x = parser.ParseNext("Third");
			var value = parser.ParseAll();

			Assert.AreEqual(2, value.Count);
		}

		[TestMethod]
		public void HasNext()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSingleDocument));

			Assert.AreEqual(true, parser.HasNext());
		}

		[TestMethod]
		public void HasNextTag()
		{
			var parser = new Ikadn.Ikon.IkonParser(new StringReader(TreeValueSingleDocument));

			Assert.AreEqual(true, parser.HasNext("Third"));
		}

		[TestMethod]
		public void HasNextTagMultistream()
		{
			var parser = new Ikadn.Ikon.IkonParser(TreeValueSeparateDocuments);

			Assert.AreEqual(true, parser.HasNext("Third"));
		}
	}
}
