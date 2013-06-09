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
	public class ReferenceTests
	{
		static string NamingTestInput =
			"\"some text\" @text" + Environment.NewLine +
			"=3 @number" + Environment.NewLine +
			"=Inf @notNumber" + Environment.NewLine +
			"[] @array" + Environment.NewLine +
			"{ NestedStuff" + Environment.NewLine +
			"\t" + "atr1 =5 @nestedNumber" + Environment.NewLine +
			"\t" + "atr2 \"more text\" @nestedText" + Environment.NewLine +
			"} @composite @otherName@noSpace";

		static string ReferencingTestInput =
			"\"the probe\" @probe" + Environment.NewLine +
			"#probe" + Environment.NewLine +
			"[" + Environment.NewLine +
			"\t" + "#probe" + Environment.NewLine +
			"]" + Environment.NewLine +
			"{ NestedStuff" + Environment.NewLine +
			"\t" + "child #probe" + Environment.NewLine +
			"}";

		[TestMethod]
		public void ReferenceNamingText()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(TextValue.ValueTypeName, parser.GetNamedValue("text").Tag);
		}

		[TestMethod]
		public void ReferenceNamingNumber()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(NumericValue.ValueTypeName, parser.GetNamedValue("number").Tag);
		}

		[TestMethod]
		public void ReferenceNamingInfinity()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(NumericValue.ValueTypeName, parser.GetNamedValue("notNumber").Tag);
		}

		[TestMethod]
		public void ReferenceNamingArray()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(ArrayValue.ValueTypeName, parser.GetNamedValue("array").Tag);
		}

		[TestMethod]
		public void ReferenceNamingComposite()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual("NestedStuff", parser.GetNamedValue("composite").Tag);
		}

		[TestMethod]
		public void ReferenceNamingNestedNumber()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(NumericValue.ValueTypeName, parser.GetNamedValue("nestedNumber").Tag);
		}

		[TestMethod]
		public void ReferenceNamingNestedText()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual(TextValue.ValueTypeName, parser.GetNamedValue("nestedText").Tag);
		}

		[TestMethod]
		public void ReferenceNamingSecondName()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual("NestedStuff", parser.GetNamedValue("otherName").Tag);
		}

		[TestMethod]
		public void ReferenceNamingNoSpace()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(NamingTestInput));
			var values = parser.ParseAll();

			Assert.AreEqual("NestedStuff", parser.GetNamedValue("noSpace").Tag);
		}

		[TestMethod]
		public void ReferencingRootType()
		{
			var parser = new Ikadn.Ikon.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext();

			Assert.AreEqual(TextValue.ValueTypeName, rootValue.Tag);
		}

		[TestMethod]
		public void ReferencingRootValue()
		{
			IkadnParser parser = new Ikadn.Ikon.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext() as TextValue;

			Assert.AreEqual("the probe", rootValue.To<string>());
		}

		[TestMethod]
		public void ReferencingArrayElement()
		{
			IkadnParser parser = new Ikadn.Ikon.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext();
			var array = parser.ParseNext() as ArrayValue;

			Assert.AreEqual("the probe", (array[0] as TextValue).To<string>());
		}

		[TestMethod]
		public void ReferencingCompositionChild()
		{
			IkadnParser parser = new Ikadn.Ikon.Parser(new StringReader(ReferencingTestInput));
			var probe = parser.ParseNext();
			var rootValue = parser.ParseNext();
			var array = parser.ParseNext() as ArrayValue;
			var composite = parser.ParseNext() as ObjectValue;

			Assert.AreEqual("the probe", (composite["child"] as TextValue).To<string>());
		}


		[TestMethod]
		public void ReferenceWriteNameRoot()
		{
			string expected = "\"something\" @name";

			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var value = new TextValue("something");
			value.ReferenceNames.Add("name");
			value.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}

		[TestMethod]
		public void ReferenceWriteNameArray()
		{
			string expected = "[" + Environment.NewLine +
				"\t" + "\"something\" @name" + Environment.NewLine + 
				"]";

			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var namedValue = new TextValue("something");
			namedValue.ReferenceNames.Add("name");

			var array = new ArrayValue();
			array.Add(namedValue);
			array.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}

		[TestMethod]
		public void ReferenceWriteNameComposite()
		{
			string expected = "{ Composite" + Environment.NewLine +
				"\t" + "child \"something\" @name" + Environment.NewLine +
				"}";

			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var namedValue = new TextValue("something");
			namedValue.ReferenceNames.Add("name");

			var composite = new ObjectValue("Composite");
			composite["child"] = namedValue;
			composite.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}
	}
}
