namespace IKON.Utils
{
	/// <summary>
	/// Describes whitespace skipping run.
	/// </summary>
	public enum WhitespecSkipResult
	{
		/// <summary>
		/// Skipping whitespaces encountered end of stream/input.
		/// </summary>
		EndOfStream,

		/// <summary>
		/// Skipping whitespaces has found non-whitespace character.
		/// </summary>
		NonWhiteChar
	}
}
