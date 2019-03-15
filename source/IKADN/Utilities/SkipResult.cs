// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// LGPL License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Describes how character skipping process ended and what was skipped. 
	/// </summary>
	public class SkipResult
	{
		/// <summary>
		/// Skipped text.
		/// </summary>
		public string SkippedText { get; private set; }
		
		/// <summary>
		/// Indicates whether skipping ended due reaching end of stream.
		/// </summary>
		public bool EndOfStream { get; private set; }

		/// <summary>
		/// Builds skip result object
		/// </summary>
		/// <param name="skippedText">Skipped text</param>
		/// <param name="endOfStream">Indication whether end of stream was reached</param>
		public SkipResult(string skippedText, bool endOfStream)
		{
			this.SkippedText = skippedText;
			this.EndOfStream = endOfStream;
		}
	}
}
