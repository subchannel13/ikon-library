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
			var value = parser.ParseNext() as Ikon.Ston.Values.Object;
			
			Console.WriteLine(value["ime"] as Text);
			Console.WriteLine();

			var value2 = parser.ParseNext();
			var value3 = parser.ParseNext();

			reader.Close();

			StringWriter writer = new StringWriter();
			var ikonWriter = new Ikon.IkonWriter(writer);

			value2.Compose(ikonWriter, "jen");
			value3.Compose(ikonWriter, "dva");
			value.Compose(ikonWriter, "tri");

			Console.Write(writer.ToString());
			Console.ReadKey();
		}
	}
}
