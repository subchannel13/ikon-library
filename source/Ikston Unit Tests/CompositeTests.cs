using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ikadn.Ikon.Types;
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
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext();

			Assert.AreEqual("Model", value.Tag);
		}

		[TestMethod]
		public void CompositeReadSampleSubvaluesCount()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			Assert.AreEqual(5, value.Keys.Count);
		}

		[TestMethod]
		public void CompositeReadSampleContainsName()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			Assert.IsTrue(value.Keys.Contains("name"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsX()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			Assert.IsTrue(value.Keys.Contains("x"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsY()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			Assert.IsTrue(value.Keys.Contains("y"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsMaterials()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			Assert.IsTrue(value.Keys.Contains("materials"));
		}

		[TestMethod]
		public void CompositeReadSampleContainsMaterial0()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			Assert.IsTrue(value.Keys.Contains("material0"));
		}

		[TestMethod]
		public void CompositeReadSampleValueName()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			var subvalue = value["name"] as IkonText;

			Assert.AreEqual("A-10", subvalue.To<string>());
		}

		[TestMethod]
		public void CompositeReadSampleValueX()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			var subvalue = value["x"];

			Assert.AreEqual(2.1, subvalue.To<double>());
		}

		[TestMethod]
		public void CompositeReadSampleValueY()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			var subvalue = value["y"];

			Assert.AreEqual(-3.4, subvalue.To<double>());
		}

		[TestMethod]
		public void CompositeReadSampleSubarrayCardinality()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			var subvalue = value["materials"] as IkonArray;

			Assert.AreEqual(3, subvalue.Count);
		}

		[TestMethod]
		public void CompositeReadSampleSubvalueType()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader(SampleSerializedObject));
			var value = parser.ParseNext() as IkonComposite;

			var subvalue = value["material0"] as IkonComposite;

			Assert.AreEqual("Material", subvalue.Tag);
		}

		[TestMethod]
		public void CompositeWriteSample()
		{
			StringBuilder output = new StringBuilder();
			IkadnWriter writer = new IkadnWriter(new StringWriter(output));

			var subarray = new IkonArray();
			subarray.Add(new IkonInteger(0));
			subarray.Add(new IkonInteger(1));
			subarray.Add(new IkonInteger(2));
			
			var subvalue = new IkonComposite("Material");
			subvalue["alpha"] = new IkonFloat(1);

			var value = new IkonComposite("Model");
			value["name"] = new IkonText("A-10");
			value["x"] = new IkonFloat(2.1);
			value["y"] = new IkonFloat(-3.4);
			value["materials"] = subarray;
			value["material0"] = subvalue;
			value.Compose(writer);

			Assert.AreEqual(SampleSerializedObject, output.ToString().Trim());
		}

		[TestMethod]
		public void CompositeEnumerateSinglePair()
		{
			IkadnParser parser = new Ikadn.Ikon.IkonParser(new StringReader("{Test aKey=1}"));
			var value = parser.ParseNext().To<IkonComposite>();

			foreach (var pair in value)
				Assert.AreEqual("aKey", pair.Key);
		}
	}
}
