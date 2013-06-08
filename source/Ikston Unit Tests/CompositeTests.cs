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
	public class CompositeTests
	{
		static string SampleSerializedObject = "{ Model" + Environment.NewLine +
			"\t" + "name \"A-10\"" + Environment.NewLine +
			"\t" + "x =2.1" + Environment.NewLine +
			"\t" + "y =-3.4" + Environment.NewLine +
			"\t" + "materials [" + Environment.NewLine +
			"\t\t" + "=0" + Environment.NewLine +
			"\t\t" + "=1" + Environment.NewLine +
			"\t\t" + "=2" + Environment.NewLine +
			"\t" + "]" + Environment.NewLine +
			"\t" + "material0 { Material" + Environment.NewLine +
			"\t\t" + "alpha =1" + Environment.NewLine +
			"\t" + "}" + Environment.NewLine +
			"}";

		[TestMethod]
		public void CompositeReadSampleType()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext();

			Assert.AreEqual("Model", value.Tag);
		}

		[TestMethod]
		public void CompositeReadSampleSubvaluesCount()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			Assert.AreEqual(5, value.Keys.Count);
		}

		[TestMethod]
		public void CompositeReadSampleContainsName()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			Assert.IsTrue(value.Keys.Contains("name"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsX()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			Assert.IsTrue(value.Keys.Contains("x"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsY()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			Assert.IsTrue(value.Keys.Contains("y"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsMaterials()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			Assert.IsTrue(value.Keys.Contains("materials"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsMaterial0()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			Assert.IsTrue(value.Keys.Contains("material0"));
		}

		[TestMethod]
		public void CompositeReadSampleValueName()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			var subvalue = value["name"] as TextValue;

			Assert.AreEqual("A-10", subvalue.To<string>());
		}

		[TestMethod]
		public void CompositeReadSampleValueX()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			var subvalue = value["x"] as NumericValue;

			Assert.AreEqual(2.1, subvalue.To<double>());
		}

		[TestMethod]
		public void CompositeReadSampleValueY()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			var subvalue = value["y"] as NumericValue;

			Assert.AreEqual(-3.4, subvalue.To<double>());
		}

		[TestMethod]
		public void CompositeReadSampleSubarrayCardinality()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			var subvalue = value["materials"] as ArrayValue;

			Assert.AreEqual(3, subvalue.Count);
		}

		[TestMethod]
		public void CompositeReadSampleSubvalueType()
		{
			Parser parser = new Ikadn.Ikon.Parser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as ObjectValue;

			var subvalue = value["material0"] as ObjectValue;

			Assert.AreEqual("Material", subvalue.Tag);
		}

		[TestMethod]
		public void CompositeWriteSample()
		{
			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var subarray = new ArrayValue();
			subarray.Add(new NumericValue(0));
			subarray.Add(new NumericValue(1));
			subarray.Add(new NumericValue(2));
			
			var subvalue = new ObjectValue("Material");
			subvalue["alpha"] = new NumericValue(1);

			var value = new ObjectValue("Model");
			value["name"] = new TextValue("A-10");
			value["x"] = new NumericValue(2.1);
			value["y"] = new NumericValue(-3.4);
			value["materials"] = subarray;
			value["material0"] = subvalue;
			value.Compose(writer);

			Assert.AreEqual(SampleSerializedObject, output.ToString().Trim());
		}
	}
}
