using Ikon;
using Ikon.Utilities;
using Ikon.Ston.Values;

namespace Ikon.Ston.Factories
{
	/// <summary>
	/// IKON value factory for named value references.
	/// </summary>
	public class ReferencedFactory : IValueFactory
	{
		/// <summary>
		/// Sign for IKON value reference.
		/// </summary>
		public const char OpeningSign = '#';

		/// <summary>
		/// Sign for IKON value reference.
		/// </summary>
		public char Sign
		{
			get { return OpeningSign; }
		}

		/// <summary>
		/// Parses input for a IKSTON reference name.
		/// </summary>
		/// <param name="parser">IKON parser instance.</param>
		/// <returns>Referenced IKON value.</returns>
		public Value Parse(Ikon.Parser parser)
		{
			if (parser == null)
				throw new System.ArgumentNullException("parser");

			return new ReferenceValue(Ikon.Ston.Parser.ReadIdentifier(parser.Reader));
		}
	}
}
