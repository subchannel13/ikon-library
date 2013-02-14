namespace Ikon.Utilities
{
	/// <summary>
	/// Describes whitespace skipping run.
	/// </summary>
	public enum ReaderDoneReason
	{
		/// <summary>
		/// Skipping whitespaces encountered end of stream/input.
		/// </summary>
		EndOfStream,

		/// <summary>
		/// Skipping or reading was successful, character that doesn't satisfy
		/// skipping or reading conditions has been found.
		/// </summary>
		UnmatchedChar
	}
}
