using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ikadn.Utilities
{
	public class SkipResult
	{
		public string SkippedText { get; private set; }
		public bool EndOfStream { get; private set; }

		public SkipResult(string skippedText, bool endOfStream)
		{
			this.SkippedText = skippedText;
			this.EndOfStream = endOfStream;
		}
	}
}
