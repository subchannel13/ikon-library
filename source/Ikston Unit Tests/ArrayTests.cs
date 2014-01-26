using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ikadn.Ikon.Types;
using Ikadn;
using System;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class ArrayTests
	{
		[TestMethod]
		public void ArrayType()
		{
			string input = "[ =2 ]";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(IkonArray.TypeTag, value.Tag);
		}

		[TestMethod]
		public void ArrayConvertIkadnBaseArray()
		{
			IkonArray array = new IkonArray();
			array.Add(new IkonInteger(1));
			array.Add(new IkonInteger(2));
			array.Add(new IkonInteger(3));
			array.Add(new IkonInteger(4));

			var converted = array.To<IkadnBaseObject[]>();
			Assert.AreEqual<int>(converted[0].To<int>(), 1);
		}

		[TestMethod]
		public void ArrayConvertTargetArray()
		{
			IkonArray array = new IkonArray();
			array.Add(new IkonInteger(1));
			array.Add(new IkonInteger(2));
			array.Add(new IkonInteger(3));
			array.Add(new IkonInteger(4));

			var converted = array.To<int[]>();
			Assert.AreEqual<int>(converted[0], 1);
		}

		[TestMethod]
		public void ArrayConvertTargetIEnumerable()
		{
			IkonArray array = new IkonArray();
			array.Add(new IkonText("a"));
			array.Add(new IkonText("b"));
			array.Add(new IkonText("c"));
			array.Add(new IkonText("d"));

			var converted = array.To<IEnumerable<string>>();
			Assert.AreEqual<string>(converted.First(), "a");
		}

		[TestMethod]
		public void ArrayConvertTargetIList()
		{
			IkonArray array = new IkonArray();
			array.Add(new IkonInteger(1));
			array.Add(new IkonInteger(2));
			array.Add(new IkonInteger(3));
			array.Add(new IkonInteger(4));

			var converted = array.To<IList<int>>();
			Assert.AreEqual<int>(converted[0], 1);
		}

		[TestMethod]
		public void ArrayCountEmpty()
		{
			string input = "[]";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonArray;

			Assert.AreEqual(0, value.Count);
		}

		[TestMethod]
		public void ArrayCountAllTypes()
		{
			string input = "[ =2 \"dfgfdg\" [] { nothing } ]";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonArray;

			Assert.AreEqual(4, value.Count);
		}

		[TestMethod]
		public void ArrayElementType0()
		{
			string input = "[ =2 \"dfgfdg\" [] { nothing } ]";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonArray;

			Assert.AreEqual(typeof(IkonInteger), value[0].GetType());
		}

		[TestMethod]
		public void ArrayElementType1()
		{
			string input = "[ =2 \"dfgfdg\" [] { nothing } ]";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonArray;

			Assert.AreEqual(typeof(IkonText), value[1].GetType());
		}

		[TestMethod]
		public void ArrayElementType2()
		{
			string input = "[ =2 \"dfgfdg\" [] { nothing } ]";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonArray;

			Assert.AreEqual(typeof(IkonArray), value[2].GetType());
		}

		[TestMethod]
		public void ArrayElementType3()
		{
			string input = "[ =2 \"asdfasdf\" [] { nothing } ]";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext() as IkonArray;

			Assert.AreEqual(typeof(IkonComposite), value[3].GetType());
		}

		[TestMethod]
		public void ArrayWriteEmpty()
		{
			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var value = new IkonArray();
			value.Compose(writer);

			Assert.AreEqual("[" + Environment.NewLine + "]", output.ToString().Trim());
		}

		[TestMethod]
		public void ArrayWriteWithNumerics()
		{
			string expected = "[" + Environment.NewLine +
				"\t=2" + Environment.NewLine +
				"\t=5" + Environment.NewLine +
				"\t=0.5" + Environment.NewLine +
				"]";

			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var value = new IkonArray();
			value.Add(new IkonInteger(2));
			value.Add(new IkonInteger(5));
			value.Add(new IkonFloat(0.5));
			value.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}

		[TestMethod]
		public void ArrayWriteWithTexts()
		{
			string expected = "[" + Environment.NewLine +
				"\t\"abc\"" + Environment.NewLine +
				"\t\"asdf\"" + Environment.NewLine +
				"]";

			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var value = new IkonArray();
			value.Add(new IkonText("abc"));
			value.Add(new IkonText("asdf"));
			value.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}

		[TestMethod]
		public void ArrayWriteNestedEmptyArray()
		{
			string expected = "[" + Environment.NewLine +
				"\t[" + Environment.NewLine +
				"\t]" + Environment.NewLine +
				"]";

			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var value = new IkonArray();
			value.Add(new IkonArray());
			value.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}

		[TestMethod]
		public void ArrayWriteNestedMixedArray()
		{
			string expected = "[" + Environment.NewLine +
				"\t[" + Environment.NewLine +
				"\t\t=2.5" + Environment.NewLine +
				"\t\t[" + Environment.NewLine +
				"\t\t\t=-0.4" + Environment.NewLine +
				"\t\t]" + Environment.NewLine +
				"\t\t[" + Environment.NewLine +
				"\t\t]" + Environment.NewLine +
				"\t\t\"foo\"" + Environment.NewLine +
				"\t]" + Environment.NewLine +
				"\t\"bar\"" + Environment.NewLine +
				"]";

			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var doubleNestedValue = new IkonArray();
			doubleNestedValue.Add(new IkonFloat(-0.4));

			var nestedValue = new IkonArray();
			nestedValue.Add(new IkonFloat(2.5));
			nestedValue.Add(doubleNestedValue);
			nestedValue.Add(new IkonArray());
			nestedValue.Add(new IkonText("foo"));
			
			var value = new IkonArray();
			value.Add(nestedValue);
			value.Add(new IkonText("bar"));
			
			value.Compose(writer);

			Assert.AreEqual(expected, output.ToString().Trim());
		}
	}
}
