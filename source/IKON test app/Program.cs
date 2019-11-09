using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Ikadn.Ikon;
using Ikadn.Ikon.Types;

namespace IKON_test_app
{
	class Program
	{
		static void Main(string[] args)
		{
			var reader = new StreamReader(args[0]);

			var parser = new IkonParser(reader);
			parser.ParseNext().To<string>();

			var compositeData = parser.ParseNext().To<IkonComposite>();

			printStar(compositeData);
			Console.WriteLine();

			var value2 = parser.ParseNext();
			var value3 = parser.ParseNext();

			reader.Close();

			StringWriter writer = new StringWriter();
			var ikonWriter = new Ikadn.IkadnWriter(writer);

			compositeData.Compose(ikonWriter);
			Console.Write(writer.ToString());

			reader = new StreamReader(args[0]);
			parser = new IkonParser(reader);
			var values = parser.ParseAll();

			Console.WriteLine("Random string: " + values.Dequeue(IkonText.TypeTag).To<string>());
			reader.Close();
			Console.ReadKey();
		}

		static void printStar(IkonComposite starData) {
			Console.WriteLine("size: " + starData["size"].To<int>());
			Console.WriteLine("position: " + starData["x"].To<int>() + ", " + starData["y"].To<int>());
			Console.WriteLine("name: " + starData["name"].To<string>());
			Console.WriteLine("desc: " + starData["desc"].To<string>());
			Console.WriteLine();
			Console.WriteLine("List of planets: ");
			foreach (var planetData in starData["planets"].To<IkonArray>())
				printPlanet(planetData.To<IkonComposite>());
		}

		static void printPlanet(IkonComposite planetData)
		{
			Console.WriteLine("size: " + planetData["size"].To<int>());
		}
	}
}
