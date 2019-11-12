// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

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
			return this.indentation;
		}

		/// <summary>
		/// Increases indentation by one level.
		/// </summary>
		public void Increase()
		{
			this.indentation += IndentChar;
		}

		/// <summary>
		/// Decreases indentation by one level.
		/// </summary>
		public void Decrease()
		{
			this.indentation = this.indentation.Remove(this.indentation.Length - 1);
		}
	}
}
