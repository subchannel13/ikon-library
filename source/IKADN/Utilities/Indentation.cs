using System;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Indentation helper.
	/// </summary>
	public class Indentation
	{
		const char IndentChar = '\t';

		string indentation = "";

		/// <summary>
		/// Gets indentation string.
		/// </summary>
		/// <returns>Indentation string.</returns>
		public override string ToString()
		{
			return indentation;
		}

		/// <summary>
		/// Increases indentation by one level.
		/// </summary>
		public void Increase()
		{
			indentation += IndentChar;
		}

		/// <summary>
		/// Decreases indentation by one level.
		/// </summary>
		public void Decrease()
		{
			indentation = indentation.Remove(indentation.Length - 1);
		}
	}
}
