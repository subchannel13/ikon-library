using System;
using System.Linq;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Describes action to be performed for certain character.
	/// </summary>
	public struct ReadingDecision
	{
		/// <summary>
		/// Character in question or new character is substitution is in to be perforemd.
		/// </summary>
		public char Character;

		/// <summary>
		/// The action to be performed.
		/// </summary>
		public CharacterAction Decision;

		/// <summary>
		/// Initializes reading decision result.
		/// </summary>
		/// <param name="character">Character in question or new character is substitution is in to be perforemd.</param>
		/// <param name="decision">The action to be performed.</param>
		public ReadingDecision(char character, CharacterAction decision)
		{
			this.Character = character;
			this.Decision = decision;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance, or null.</param>
		/// <returns>True if this and obj represent equal value.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is ReadingDecision))
				return false;
			var other = (ReadingDecision)obj;

			return this.Decision == other.Decision && this.Character == other.Character;
		}

		/// <summary>
		/// Returns the hash code for the value of this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return Character.GetHashCode() * 31 + Decision.GetHashCode();
		}

		/// <summary>
		/// Returns a value indicating whether a left hand value is equal to a right hand value.
		/// </summary>
		/// <param name="left">Left hand value.</param>
		/// <param name="right">Right hand value.</param>
		/// <returns>True if a left hand and a right hand value are equal.</returns>
		public static bool operator ==(ReadingDecision left, ReadingDecision right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Returns a value indicating whether a left hand value is different than a right hand value.
		/// </summary>
		/// <param name="left">Left hand value.</param>
		/// <param name="right">Right hand value.</param>
		/// <returns>True if a left hand and a right hand value are not equal.</returns>
		public static bool operator !=(ReadingDecision left, ReadingDecision right)
		{
			return !left.Equals(right);
		}    
	}
}
