using System;
using System.Web.UI;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Custom writer that allows turning on/off newlines.
	/// </summary>
	public class HtmlInlineWriter : HtmlTextWriter
	{
		private bool _AllowNewLine = false;

		/// <summary>
		/// Initializes a new instance of the HtmlInlineWriter.
		/// </summary>
		/// <param name="writer">The source writer to use.</param>
		public HtmlInlineWriter(HtmlTextWriter writer)
			: base(writer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the HtmlInlineWriter.
		/// </summary>
		/// <param name="writer">The source writer to use.</param>
		/// <param name="tabString">The string to use for tabs.</param>
		public HtmlInlineWriter(HtmlTextWriter writer, string tabString)
			: base(writer, tabString)
		{
		}

		/// <summary>
		/// Allows new lines to be written.
		/// </summary>
		public virtual bool AllowNewLine
		{
			get { return _AllowNewLine; }
			set { _AllowNewLine = value; }
		}

		/// <summary>
		/// Creates a new line.
		/// </summary>
		public override void WriteLine()
		{
			if (AllowNewLine)
			{
				base.WriteLine();
			}
		}

		public override void WriteLine(bool b)
		{
			if (AllowNewLine)
			{
				base.WriteLine(b);
			}
		}

		public override void WriteLine(char ch)
		{
			if (AllowNewLine)
			{
				base.WriteLine(ch);
			}
		}

		public override void WriteLine(char[] ch)
		{
			if (AllowNewLine)
			{
				base.WriteLine(ch);
			}
		}

		public override void WriteLine(Decimal d)
		{
			if (AllowNewLine)
			{
				base.WriteLine(d);
			}
		}

		public override void WriteLine(double d)
		{
			if (AllowNewLine)
			{
				base.WriteLine(d);
			}
		}

		public override void WriteLine(int i)
		{
			if (AllowNewLine)
			{
				base.WriteLine(i);
			}
		}

		public override void WriteLine(long l)
		{
			if (AllowNewLine)
			{
				base.WriteLine(l);
			}
		}

		public override void WriteLine(object obj)
		{
			if (AllowNewLine)
			{
				base.WriteLine(obj);
			}
		}

		public override void WriteLine(float f)
		{
			if (AllowNewLine)
			{
				base.WriteLine(f);
			}
		}

		public override void WriteLine(string s)
		{
			if (AllowNewLine)
			{
				base.WriteLine(s);
			}
		}

		public override void WriteLine(string s, object obj)
		{
			if (AllowNewLine)
			{
				base.WriteLine(s, obj);
			}
		}

		public override void WriteLine(string s, object[] obj)
		{
			if (AllowNewLine)
			{
				base.WriteLine(s, obj);
			}
		}

		public override void WriteLine(char[] ch, int i, int n)
		{
			if (AllowNewLine)
			{
				base.WriteLine(ch, i, n);
			}
		}

		public override void WriteLine(string s, object o1, object o2)
		{
			if (AllowNewLine)
			{
				base.WriteLine(s, o1, o2);
			}
		}

		public override void WriteLine(string s, object o1, object o2, object o3)
		{
			if (AllowNewLine)
			{
				base.WriteLine(s, o1, o2, o3);
			}
		}
	}
}
