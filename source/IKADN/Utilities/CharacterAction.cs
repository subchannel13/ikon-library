using System;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Action to be performed with a character.
	/// </summary>
	public enum CharacterAction
	{
		/// <summary>
		/// Perform no action. Reader won't advance to the next character.
		/// </summary>
		NoAction = 0,

		/// <summary>
		/// Don't add anything to the result and continue reading.
		/// </summary>
		Skip = 1,

		/// <summary>
		/// Add the character as it is to the ressult.
		/// </summary>
		AcceptAsIs = 2,

		/// <summary>
		/// Add specified character to th result instead of read character.
		/// </summary>
		Substitute = 3,

		/// <summary>
		/// Mask for input interpreting actions (accept, substitute, skip).
		/// </summary>
		AllInputActions = 3,

		/// <summary>
		/// Reading is finished
		/// </summary>
		Stop = 4,

		/// <summary>
		/// Combination of skipping a character and stopping the reading.
		/// </summary>
		SkipStop = Stop | Skip,
	}
}
