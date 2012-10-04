﻿namespace Ikon
{
	/// <summary>
	/// Interface for IKON value factories usable by IKON parser.
	/// </summary>
	public interface IValueFactory
	{
		/// <summary>
		/// Character that identifies value parcable by factory.
		/// </summary>
		char Sign { get; }

		/// <summary>
		/// Parses input for a IKON value.
		/// </summary>
		/// <param name="parser">IKON parser instance.</param>
		/// <returns>IKON value generated by factory.</returns>
		Value Parse(Parser parser);
	}
}
