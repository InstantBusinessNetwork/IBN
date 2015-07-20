using System;
using System.IO;
using System.Text;
using System.Collections;

namespace Mediachase.Net.Mail
{
	internal class StreamLineReader
	{
		private byte[] _buffer = null;
		private int	   _size = 0;
		private int	   _unparsed = 0;
		private bool   _CRLFWasFound	=	false;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="strmSource"></param>
		public StreamLineReader(byte[] buffer, int offset, int size)
		{
			_buffer = buffer;
			_size   = size;
			_unparsed = offset;
		}



		public int UnParsed
		{
			get
			{
				return _unparsed;
			}
		}


		/// <summary>
		/// Reads byte[] line from stream.
		/// </summary>
		/// <returns>Return null if end of stream reached.</returns>
		public byte[] ReadLine()
		{
			_CRLFWasFound = false;

			if(_unparsed >= _size)
				return null;

			MemoryStream lineBuf = new MemoryStream();
			byte      prevByte = 0;

			int currByteInt = _buffer[_unparsed++];

			while(_unparsed <= _size)
			{
				lineBuf.WriteByte((byte)currByteInt);

				// Line found
				if((prevByte == (byte)'\r' && (byte)currByteInt == (byte)'\n'))
				{
					//byte[] retVal = new byte[lineBuf.Count-2];    // Remove <CRLF> 
					//lineBuf.CopyTo(0,retVal,0,lineBuf.Count-2);
					lineBuf.SetLength(lineBuf.Length - 2);// Remove <CRLF> 
					lineBuf.Capacity = (int)lineBuf.Length;

					_CRLFWasFound = true;
					return lineBuf.Capacity == 0 ? new byte[0] : lineBuf.GetBuffer();
				}

				if(_unparsed>=_size)
					break;

				// Store byte
				prevByte = (byte)currByteInt;

				// Read next byte
				currByteInt =  _buffer[_unparsed++];				
			}

			// Line isn't terminated with <CRLF> and has some chars left, return them.
			if(lineBuf.Length > 0)
			{
				//byte[] retVal = new byte[lineBuf.Count];  
				//lineBuf.CopyTo(0,retVal,0,lineBuf.Count);
				//return retVal;
				lineBuf.Capacity = (int)lineBuf.Length;
				return lineBuf.GetBuffer();
			}

			return null;
		}

		public bool CRLFWasFound
		{
			get
			{
				return _CRLFWasFound;
			}
		}
	}

	/// <summary>
	/// Summary description for LineParser.
	/// </summary>
	internal class LineParser
	{
		private StringBuilder	_buffer	=	new StringBuilder();

		protected	string	_LineTerminator	= "\r\n";

		public LineParser()
		{
		}

		public string LineTerminator
		{
			get
			{
				return _LineTerminator;
			}

			set
			{
				_LineTerminator = value;
			}
		}

		public void Write(char[] s)
		{
			_buffer.Append(s);
		}

		public void Write(string s)
		{
			_buffer.Append(s);
		}

		public string ReadLine()
		{
			string strTmpString  =	_buffer.ToString();
			int i = strTmpString.IndexOf(_LineTerminator);
			if (i == -1)
				return null;

			_buffer.Remove(0, i + _LineTerminator.Length);

			return strTmpString.Substring(0, i);
		}
	}
}
