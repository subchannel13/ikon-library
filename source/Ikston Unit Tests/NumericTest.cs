using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ikon;
using System.IO;
using Ikon.Ston.Values;
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

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext();

			Assert.AreEqual(Numeric.ValueTypeName, value.TypeName);
		}

		[TestMethod]
		public void NumericReadValueShort()
		{
			short expectedValue = 2;
			string input = "=" + expectedValue;

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetShort);
		}

		[TestMethod]
		public void NumericReadValueInt()
		{
			int expectedValue = 1234567;
			string input = "=" + expectedValue;

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetInt);
		}

		[TestMethod]
		public void NumericReadValueIntNegative()
		{
			int expectedValue = -1234567;
			string input = "=" + expectedValue;

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetInt);
		}

		[TestMethod]
		public void NumericReadValueLong()
		{
			long expectedValue = 123456789012L;
			string input = "=" + expectedValue;

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetLong);
		}

		[TestMethod]
		public void NumericReadValueDecimal()
		{
			decimal expectedValue = 1.23456789012m;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetDecimal);
		}

		[TestMethod]
		public void NumericReadValueFloat()
		{
			float expectedValue = 1.2345f;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetFloat);
		}

		[TestMethod]
		public void NumericReadValueDouble()
		{
			double expectedValue = 1.23456789;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetDouble);
		}

		[TestMethod]
		public void NumericReadValueDoubleNegative()
		{
			double expectedValue = -1.23456789;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetDouble);
		}

		[TestMethod]
		public void NumericReadValueDoubleInfinity()
		{
			double expectedValue = double.PositiveInfinity;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetDouble);
		}

		[TestMethod]
		public void NumericReadValueDoubleNegativeInfinity()
		{
			double expectedValue = double.NegativeInfinity;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetDouble);
		}

		[TestMethod]
		public void NumericReadValueDoubleNaN()
		{
			double expectedValue = double.NaN;
			string input = "=" + expectedValue.ToString(NumberFormatInfo.InvariantInfo);

			Parser parser = new Ikon.Ston.Parser(new StringReader(input));
			var value = parser.ParseNext() as Numeric;

			Assert.AreEqual(expectedValue, value.GetDouble);
		}
	}
}
