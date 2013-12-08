using Ikadn.Ikon.Types;

namespace Ikadn.Ikon.Factories
{
	/// <summary>
	/// IKADN object factory for named object references.
	/// </summary>
	public class ReferencedFactory : IIkadnObjectFactory
	{
		/// <summary>
		/// Sign for IKADN object reference.
		/// </summary>
		public const char OpeningSign = '#';

		/// <summary>
		/// Sign for IKADN object reference.
		/// </summary>
		public char Sign
		{
			get { return OpeningSign; }
		}

		/// <summary>
		/// Parses input for a IKON reference name.
		/// </summary>
		/// <param name="parser">IKADN parser instance.</param>
		/// <returns>Referenced IKADN object.</returns>
		public IkadnBaseObject Parse(Ikadn.IkadnParser parser)
		{
			if (parser == null)
				throw new System.ArgumentNullException("parser");

			return new IkonReference(Ikadn.Ikon.IkonParser.ReadIdentifier(parser.Reader));
		}
	}
}
