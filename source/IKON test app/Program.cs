using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Ikon.Ston;
using Ikon.Ston.Values;

namespace IKON_test_app
{
	class Program
	{
		static void Main(string[] args)
		{
			StreamReader reader = new StreamReader(args[0]);

			Parser parser = new Parser(reader);
			var value = parser.ParseNext().As<ObjectValue>();

			printStar(value);
			Console.WriteLine();

			var value2 = parser.ParseNext();
			var value3 = parser.ParseNext();

			reader.Close();

			StringWriter writer = new StringWriter();
			var ikonWriter = new Ikon.IkonWriter(writer);

			value2.Compose(ikonWriter);
			value3.Compose(ikonWriter);
			value.Compose(ikonWriter);
			Console.Write(writer.ToString());

			reader = new StreamReader(args[0]);
			parser = new Parser(reader);
			var values = parser.ParseAll();

			Console.WriteLine("Random string: " + values.Dequeue(TextValue.ValueTypeName).As<string>());
			reader.Close();
			Console.ReadKey();
		}

		static void printStar(ObjectValue starData) {
			Console.WriteLine("size: " + starData["velicina"].As<int>());
			Console.WriteLine("position: " + starData["x"].As<int>() + ", " + starData["y"].As<int>());
			Console.WriteLine("name: " + starData["ime"].As<string>());
			Console.WriteLine();
			Console.WriteLine("List of planets: ");
			foreach (var planetData in starData["planeti"].As<ArrayValue>())
				printPlanet(planetData.As<ObjectValue>());
		}

		static void printPlanet(ObjectValue planetData)
		{
			Console.WriteLine("size: " + planetData["velicina"].As<int>());
		}
	}
}
