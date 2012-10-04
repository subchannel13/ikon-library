using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Ikon.Ston;

namespace IKON_test_app
{
	class Program
	{
		static void Main(string[] args)
		{
			StreamReader reader = new StreamReader(args[0]);

			Parser parser = new Parser(reader);
			var value = parser.ParseNext().AsObject();
			
			Console.WriteLine(value["ime"].AsText());
			Console.WriteLine();

			var value2 = parser.ParseNext();
			var value3 = parser.ParseNext();

			reader.Close();

			StringWriter writer = new StringWriter();
			var composer = new Ikon.Composer(writer);

			composer.Write(value2, "jen");
			composer.Write(value3, "dva");
			composer.Write(value, "tri");
			composer.EndLine();

			Console.Write(writer.ToString());
			Console.ReadKey();
		}
	}
}
