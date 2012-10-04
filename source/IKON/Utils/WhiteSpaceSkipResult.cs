namespace Ikon.Utilities
{
	/// <summary>
	/// Describes whitespace skipping run.
	/// </summary>
	public enum WhiteSpaceSkipResult
	{
		/// <summary>
		/// Skipping whitespaces encountered end of stream/input.
		/// </summary>
		EndOfStream,

		/// <summary>
		/// Skipping whitespaces has found non-whitespace character.
		/// </summary>
		NonwhiteChar
	}
}
