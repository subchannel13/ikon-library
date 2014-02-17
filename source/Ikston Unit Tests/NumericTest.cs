using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ikadn;
using System.IO;
using Ikadn.Ikon.Types;
using System.Globalization;

namespace Ikston_Unit_Tests
{
	[TestClass]
	public class NumericTest
	{
		[TestMethod]
		public void NumericType()
		{
			string input = "=2";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(IkonInteger.TypeTag, value.Tag);
		}

		[TestMethod]
		public void NumericReadValueShort()
		{
			short expectedValue = 2;
			string input = "=" + expectedValue;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<short>());
		}

		[TestMethod]
		public void NumericReadValueInt()
		{
			int expectedValue = 1234567;
			string input = "=" + expectedValue;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<int>());
		}

		[TestMethod]
		public void NumericReadValueIntNegative()
		{
			int expectedValue = -1234567;
			string input = "=-1234567";

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<int>());
		}

		[TestMethod]
		public void NumericReadValueLong()
		{
			long expectedValue = 123456789012L;
			string input = "=" + expectedValue;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<long>());
		}

		[TestMethod]
		public void NumericReadValueDecimal()
		{
			decimal expectedValue = 1.23456789012m;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<decimal>());
		}

		[TestMethod]
		public void NumericReadValueFloat()
		{
			float expectedValue = 1.2345f;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<float>());
		}

		[TestMethod]
		public void NumericReadValueDouble()
		{
			double expectedValue = 1.23456789;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<double>());
		}

		[TestMethod]
		public void NumericReadValueDoubleNegative()
		{
			double expectedValue = -1.23456789;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<double>());
		}

		[TestMethod]
		public void NumericReadValueDoubleInfinity()
		{
			double expectedValue = double.PositiveInfinity;
			string input = "=" + IkonFloat.PositiveInfinity;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<double>());
		}

		[TestMethod]
		public void NumericReadValueDoubleNegativeInfinity()
		{
			double expectedValue = double.NegativeInfinity;
			string input = "=" + IkonFloat.NegativeInfinity;

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<double>());
		}

		[TestMethod]
		public void NumericReadValueDoubleNaN()
		{
			double expectedValue = double.NaN;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(expectedValue, value.To<double>());
		}

		[TestMethod]
		public void NumericWriteValueLong()
		{
			long rawValue = 1234567890L;
			string ikonData = "=" + rawValue;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonInteger(rawValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueLongNegative()
		{
			long rawValue = -1234567890L;
			string ikonData = "=-1234567890";
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonInteger(rawValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueDouble()
		{
			double rawValue = 1.234567890;
			string ikonData = "=" + rawValue.ToString(NumberFormatInfo.InvariantInfo);
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonFloat(rawValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueDoubleNegative()
		{
			double rawValue = -1.234567890;
			string ikonData = "=" + rawValue.ToString(NumberFormatInfo.InvariantInfo);
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonFloat(rawValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueDoubleInfinity()
		{
			string ikonData = "=" + IkonFloat.PositiveInfinity;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonFloat(double.PositiveInfinity);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueDoubleNegativeInfinity()
		{
			string ikonData = "=" + IkonFloat.NegativeInfinity;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonFloat(double.NegativeInfinity);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueDoubleNaN()
		{
			string ikonData = "=" + IkonFloat.NotANumber;
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonFloat(double.NaN);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueDecimal()
		{
			decimal rawValue = 1.234567890m;
			string ikonData = "=" + rawValue.ToString(NumberFormatInfo.InvariantInfo);
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonDecimal(rawValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}

		[TestMethod]
		public void NumericWriteValueDecimalNegative()
		{
			decimal rawValue = -1.234567890m;
			string ikonData = "=" + rawValue.ToString(NumberFormatInfo.InvariantInfo);
			StringBuilder output = new StringBuilder();

			IkadnWriter writer = new IkadnWriter(new StringWriter(output));
			var value = new IkonDecimal(rawValue);
			value.Compose(writer);

			Assert.AreEqual(ikonData, output.ToString().Trim());
		}
	}
}
