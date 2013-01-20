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
			parser.ParseNext();
			parser.ParseNext();
			var value = parser.ParseNext().As<ObjectValue>();
			
			Console.WriteLine(value["ime"].As<string>());
			Console.WriteLine(value["planeti"].As<IList<Ikon.Value>>()[0].As<ObjectValue>()["velicina"].As<int>());
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
			Console.ReadKey();
		}
	}
}
