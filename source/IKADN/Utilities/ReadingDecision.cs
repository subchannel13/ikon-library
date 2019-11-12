// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

using System;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Describes action to be performed for certain character.
	/// </summary>
	public struct ReadingDecision : IEquatable<ReadingDecision>
	{
		/// <summary>
		/// Character in question or new character is substitution is in to be performed.
		/// </summary>
		public char Character { get; private set; }

		/// <summary>
		/// The action to be performed.
		/// </summary>
		public CharacterAction Decision { get; private set; }

        /// <summary>
        /// Initializes reading decision result.
        /// </summary>
        /// <param name="character">Character in question or new character is substitution is in to be performed</param>
        /// <param name="decision">The action to be performed</param>
        public ReadingDecision(char character, CharacterAction decision)
		{
			this.Character = character;
			this.Decision = decision;
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance, or null</param>
		/// <returns>True if this and obj represent equal value</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is ReadingDecision))
				return false;
			return this.Equals((ReadingDecision)obj);
		}

		/// <summary>
		/// Tests whether this instance is equal to another Ikadn.Utilities.ReadingDecision
		/// instance.
		/// </summary>
		/// <param name="other">Other Ikadn.Utilities.ReadingDecision instance</param>
		/// <returns>True if this and other represent equal value.</returns>
		public bool Equals(ReadingDecision other)
		{
			return this.Decision == other.Decision && this.Character == other.Character;
		}

		/// <summary>
		/// Returns the hash code for the value of this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.Character.GetHashCode() * 31 + this.Decision.GetHashCode();
		}

		/// <summary>
		/// Returns a value indicating whether a left hand value is equal to a right hand value.
		/// </summary>
		/// <param name="left">Left hand value</param>
		/// <param name="right">Right hand value</param>
		/// <returns>True if a left hand and a right hand value are equal.</returns>
		public static bool operator ==(ReadingDecision left, ReadingDecision right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Returns a value indicating whether a left hand value is different than a right hand value.
		/// </summary>
		/// <param name="left">Left hand value</param>
		/// <param name="right">Right hand value</param>
		/// <returns>True if a left hand and a right hand value are not equal.</returns>
		public static bool operator !=(ReadingDecision left, ReadingDecision right)
		{
			return !left.Equals(right);
		}
	}
}
