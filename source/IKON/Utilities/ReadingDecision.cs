using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ikon.Utilities
{
	/// <summary>
	/// Describes action to be performed for certain character.
	/// </summary>
	public struct ReadingDecision
	{
		/// <summary>
		/// Character in question or new character is supstitution is in to be perforemd.
		/// </summary>
		public char Character;

		/// <summary>
		/// The action to be performed.
		/// </summary>
		public CharacterAction Decision;

		/// <summary>
		/// Initializes reading decision result.
		/// </summary>
		/// <param name="character">Character in question or new character is supstitution is in to be perforemd.</param>
		/// <param name="decision">The action to be performed.</param>
		public ReadingDecision(char character, CharacterAction decision)
		{
			this.Character = character;
			this.Decision = decision;
		}
	}
}
