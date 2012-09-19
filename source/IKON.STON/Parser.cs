using System.IO;
using System.Collections.Generic;
using IKON;
using IKON.STON.Factories;

namespace IKON.STON
{
	/// <summary>
	/// Parser that can parse input with IKSTON syntax.
	/// </summary>
	public class Parser : IKON.Parser
	{
		/// <summary>
		/// Constructs IKSTON parser with default IKSTON value factories.
		/// </summary>
		/// <param name="reader"></param>
		public Parser(TextReader reader)
			: base(reader, new IValueFactory[] {
 			new ObjectFactory(),
			new TextFactory(), 
			new NumericFactory(),
			new ArrayFactory(),
			new ReferencedFactory() })
		{ }

		/// <summary>
		/// Constructs IKSTON parser and registers addoditonal value factories to it.
		/// </summary>
		/// <param name="reader">Input stream with IKON syntax.</param>
		/// <param name="factories">Collection of value factories.</param>
		public Parser(TextReader reader, IEnumerable<IValueFactory> factories)
			: this(reader)
		{
			foreach (var factory in factories)
				RegisterFactory(factory);
		}
	}
}
