using System;
using System.Text;
using Ikadn.Ikon.Types;

namespace Ikadn.Ikon.Factories
{
	public class TextBlockFactory : IIkadnObjectFactory
	{
		public const char OpeningSign = '§';
		public const char ClosingChar = '\\';
		
		public char Sign {
			get {
				return OpeningSign;
			}
		}
		
		public IkadnBaseObject Parse(IkadnParser parser)
		{
			StringBuilder text = new StringBuilder();
			string textIndent = 
				parser.Reader.LineIndentation + 
				readIndentSpec(parser.Reader.ReadUntil('\n','\r').Trim());
			
			skipEndOfLine(parser.Reader);
			
			while(checkIndentation(parser.Reader, textIndent)) {
				text.AppendLine(parser.Reader.ReadUntil('\n','\r'));
				skipEndOfLine(parser.Reader);
			}
			
			if (text.Length >= Environment.NewLine.Length)
				text.Length -= Environment.NewLine.Length;
			
			return new IkonText(text.ToString());
		}
		
		private bool checkIndentation(IkadnReader reader, string indentation)
		{
			foreach(char expectedSpace in indentation)
				if (reader.Peek() == expectedSpace)
					reader.Read();
				else
					if (reader.Peek() == ClosingChar) {
						reader.Read();
						return false;
					}
					else
						throw new FormatException(); //FIXME: exceptiom message
			
			return true;
		}
		
		private void skipEndOfLine(IkadnReader reader)
		{
			char nextChar = reader.Peek();
			
			if (nextChar == '\r') {
				reader.Read();
				nextChar = reader.Peek();
			}
			
			if (nextChar != '\n')
				throw new FormatException(); //FIXME: add message
			
			reader.Read();
		}
		
		private string readIndentSpec(string indentSpec)
		{
			if (string.IsNullOrEmpty(indentSpec))
				return "\t";
			
			return indentSpec.Replace("\\t", "\t").Replace("\\s", " ");
		}
	}
}
