using Ikadn;
using Ikadn.Utilities;
using Ikadn.Ikon.Values;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN value factory for named value references.
	/// </summary>
	public class ReferencedFactory : IValueFactory
	{
		/// <summary>
		/// Sign for IKADN value reference.
		/// </summary>
		public const char OpeningSign = '#';

		/// <summary>
		/// Sign for IKADN value reference.
		/// </summary>
		public char Sign
		{
			get { return OpeningSign; }
		}

		/// <summary>
		/// Parses input for a IKON reference name.
		/// </summary>
		/// <param name="parser">IKADN parser instance.</param>
		/// <returns>Referenced IKADN value.</returns>
		public IkadnBaseValue Parse(Ikadn.Parser parser)
		{
			if (parser == null)
				throw new System.ArgumentNullException("parser");

			return new ReferenceValue(Ikadn.Ikon.Parser.ReadIdentifier(parser.Reader));
		}
	}
}
