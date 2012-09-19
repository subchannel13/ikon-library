using IKON;
using IKON.Utils;

namespace IKON.STON.Factories
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
		public Value Parse(IKON.Parser parser)
		{
			return parser.GetNamedValue(parser.ReadIdentifier());
		}
	}
}
