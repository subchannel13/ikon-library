// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// LGPL License. See License.txt in the project root for license information.

using System.IO;

namespace Ikadn.Utilities
{
	/// <summary>
	/// Wrapper around System.IO.TextReader stream which allow identification a
	/// stream.
	/// </summary>
	public class NamedStream
	{
		/// <summary>
		/// Wrapped stream.
		/// </summary>
		public TextReader Stream { get; private set; }

		/// <summary>
		/// Stream identification.
		/// </summary>
		public string StreamName { get; private set; }

		/// <summary>
		/// Initializes a new empty instance of Ikadn.Utilities.NamedStream.
		/// </summary>
		/// <param name="stream">Stream to wrap</param>
		/// <param name="sourceName">Stream identification</param>
		public NamedStream(TextReader stream, string sourceName)
		{
			this.Stream = stream;
			this.StreamName = sourceName;
		}
	}
}
