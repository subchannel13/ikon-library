using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Ikadn.Ikon;
using Ikadn.Ikon.Values;

namespace IKON_test_app
{
	class Program
	{
		static void Main(string[] args)
		{
			StreamReader reader = new StreamReader(args[0]);

			Parser parser = new Parser(reader);
			var value = parser.ParseNext().To<ObjectValue>();

			printStar(value);
			Console.WriteLine();

			var value2 = parser.ParseNext();
			var value3 = parser.ParseNext();

			reader.Close();

			StringWriter writer = new StringWriter();
			var ikonWriter = new Ikadn.IkadnWriter(writer);

			value2.Compose(ikonWriter);
			value3.Compose(ikonWriter);
			value.Compose(ikonWriter);
			Console.Write(writer.ToString());

			reader = new StreamReader(args[0]);
			parser = new Parser(reader);
			var values = parser.ParseAll();

			Console.WriteLine("Random string: " + values.Dequeue(TextValue.ValueTypeName).To<string>());
			reader.Close();
			Console.ReadKey();
		}

		static void printStar(ObjectValue starData) {
			Console.WriteLine("size: " + starData["velicina"].To<int>());
			Console.WriteLine("position: " + starData["x"].To<int>() + ", " + starData["y"].To<int>());
			Console.WriteLine("name: " + starData["ime"].To<string>());
			Console.WriteLine();
			Console.WriteLine("List of planets: ");
			foreach (var planetData in starData["planeti"].To<ArrayValue>())
				printPlanet(planetData.To<ObjectValue>());
		}

		static void printPlanet(ObjectValue planetData)
		{
			Console.WriteLine("size: " + planetData["velicina"].To<int>());
		}
	}
}
