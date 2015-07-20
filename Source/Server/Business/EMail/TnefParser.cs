using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Data;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for TnefParser.
	/// </summary>
	public class TnefParser
	{
		public enum MapiType : short
		{
			Unspecified = 0x0000,
			NullProperty = 0x0001,
			Short = 0x0002, // (signed 16 bits)
			Int = 0x0003, // MAPI integer (signed 32 bits)
			Float = 0x0004, //MAPI float (4 bytes)
			Double = 0x0005,// MAPI double
			Currency = 0x0006,// MAPI currency (64 bits)
			Apptime = 0x0007,// MAPI application time
			Error = 0x000a,// MAPI error (32 bits)
			Boolean = 0x000b,// MAPI boolean (16 bits)
			Object = 0x000d,// MAPI embedded object
			Int8Byte = 0x0014,// MAPI 8 byte signed int
			String = 0x001e,// MAPI string
			UnicodeString = 0x001f,// MAPI unicode-string (null terminated)
			Systime = 0x0040,// MAPI time (64 bits)
			Clsid = 0x0048,// MAPI OLE GUID
			Binary = 0x0102,// MAPI binary 
		}

		private const int TNEF_SIGNATURE  =0x223e9f78;
		private const int LVL_MESSAGE     =0x01;
		private const int LVL_ATTACHMENT  =0x02;
		private const int _string			=0x00010000;
		private const int _BYTE			=0x00060000;
		private const int _WORD			=0x00070000;
		private const int _DWORD			=0x00080000;

		private const int AVERSION      =(_DWORD|0x9006);
		private const int AMCLASS       =(_WORD|0x8008);
		private const int ASUBJECT      =(_DWORD|0x8004);
		private const int AFILENAME     =(_string|0x8010);
		private const int ATTACHDATA    =(_BYTE|0x800f);

		private Stream fsTNEF;
		private ArrayList _attachments=new ArrayList();

		private int _fileLength;
		private string strSubject;

		private int _skipSignature = 0;

		private Hashtable _contentTypeHash = new Hashtable();

		private Hashtable _fileOffsetList = new Hashtable();
		

		private class FileOffset
		{
			public int Offset;
			public int Length;
		}

		private FileOffset _currentOffset = null;

		public static TnefParser Load(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
			{
				TnefParser retVal = new TnefParser(reader.ReadBytes((int)reader.BaseStream.Length));
				return retVal;
			}
		}

		public TnefParser(byte[] bytContents)
		{
			using(IDataReader reader = ContentType.GetListContentTypes())
			{
				while(reader.Read())
				{
					_contentTypeHash.Add(((string)reader["Extension"]).ToLower(), (string)reader["ContentTypeString"]);
				}
			}


			OpenStream(bytContents);
		}

		protected int SkipSignature
		{
			get{return _skipSignature;}
			set{_skipSignature=value;}
		}

		public AttachmentInfo[] Attachments
		{
			get 
			{
				return (AttachmentInfo[])_attachments.ToArray(typeof(AttachmentInfo));
			}
		}

		private bool OpenStream(byte[] bytContents)
		{
			fsTNEF = new MemoryStream(bytContents);
			_fileLength=bytContents.Length;
			return true;
		}

		public byte[] GetAttachmentBody(int index)
		{
			string FileName = this.Attachments[index].FileName;

			FileOffset offset = (FileOffset)_fileOffsetList[FileName];

			byte[] retVal  = new byte[offset.Length];

			long realPossition = fsTNEF.Position;

			fsTNEF.Position = offset.Offset;

			fsTNEF.Read(retVal, 0, offset.Length);

			fsTNEF.Position = realPossition;

			return retVal;
		}

		public bool Parse()
		{
			try
			{
				byte[] buf = new byte[4];
				int d;

				if (FindSignature())
				{
					if (SkipSignature < 2)
					{
						d = GetI32();
						if (SkipSignature < 1)
						{
							if (d != TNEF_SIGNATURE)
							{
								//PrintResult("Seems not to be a TNEF file\n");
								return false;
							}
						}
					}

					d = GetI16();
					//PrintResult("TNEF Key is: {0}\n", d);
					for (; ; )
					{
						if (StreamReadBytes(buf, 1) == 0)
							break;

						d = (int)buf[0];

						switch (d)
						{
							case LVL_MESSAGE:
								//PrintResult("{0}: Decoding Message Attributes\n",fsTNEF.Position);
								DecodeMessage();
								break;
							case LVL_ATTACHMENT:
								//PrintResult("Decoding Attachment\n");
								DecodeAttachment();
								break;
							default:
								//PrintResult("Coding Error in TNEF file\n");
								return false;
						}
					}
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex,"ThefParser");

				return false;
			}
		}

		protected bool FindSignature()
		{
			bool ret=false;
			long lpos=0;

			int d;

			try
			{
				for (lpos=0; ; lpos++) 
				{

					if (fsTNEF.Seek(lpos,SeekOrigin.Begin)==-1)
					{
						//PrintResult("No signature found\n");
						return false;
					}

					d = GetI32();
					if (d == TNEF_SIGNATURE) 
					{
						//PrintResult("Signature found at {0}\n", lpos);
						break;
					}
				}
				ret=true;
			}
			catch(Exception)
			{
				//Utility.LogError("FindSignature():"+e.Message);
				ret=false;
			}

			fsTNEF.Position=lpos;

			return ret;
		}

		private string GetContentTypeByFileName(string FileName)
		{
			string retVal = (string)_contentTypeHash[string.Empty];

			string ext = Path.GetExtension(FileName);
			if(ext!=null&&ext.Length>0)
			{
				ext = ext.Substring(1,ext.Length-1).ToLower();
				if(_contentTypeHash.ContainsKey(ext))
					retVal = (string)_contentTypeHash[ext];
			}

			return retVal;
		}


		private void DecodeAttachment() 
		{  
			//int attachmentFileLength  = 0;

			byte[] buf=new byte[4096];
			int len;
			int i,chunk;
    
			int d = GetI32();

			switch (d) 
			{
				case ASUBJECT:
					{
						len = GetI32();

						StreamReadBytes(buf, len);

						byte[] subjectBuffer = new byte[len - 1];

						Array.Copy(buf, subjectBuffer, (long)len - 1);

						strSubject = Encoding.Default.GetString(subjectBuffer);

						//PrintResult("Found subject: {0}", strSubject);

						GetI16();     /* checksum */
					}
					break;

				case AFILENAME:
					{
						len = GetI32();
						StreamReadBytes(buf, len);
						//PrintResult("File-Name: {0}\n", buf);

						byte[] fileNameBuffer = null;
						string strFileName = null;

						if (len > 1)
						{
							fileNameBuffer = new byte[len - 1];
							Array.Copy(buf, fileNameBuffer, (long)len - 1);
						}

						if (fileNameBuffer != null)
							strFileName = Encoding.Default.GetString(fileNameBuffer);

						if (string.IsNullOrEmpty(strFileName))
							strFileName = string.Format("Unknown{0}.dat", _fileOffsetList.Count + 1);


						//PrintResult("{0}: WRITING {1}\n", BasePath, strFileName);

						//new attachment found because attachment data goes before attachment name
						//					_attachment.FileName=strFileName;
						//					_attachment.Subject=strSubject;


						if (_currentOffset != null)
						{
							AttachmentInfo attachment = new AttachmentInfo(strFileName, GetContentTypeByFileName(strFileName), _currentOffset.Length);
							_attachments.Add(attachment);

							_fileOffsetList.Add(strFileName, _currentOffset);
							_currentOffset = null;
						}

						GetI16();     /* checksum */
					}
					break;

				case ATTACHDATA:
					{
						len = GetI32();
						//PrintResult("ATTACH-DATA: {0} bytes\n", len);

						//_attachment=new TNEFAttachment();
						//_attachment.FileContent=new byte[len];
						_currentOffset = new FileOffset();
						_currentOffset.Length = len;
						_currentOffset.Offset = (int)fsTNEF.Position;

						//attachmentFileLength = len;

						for (i = 0; i < len; )
						{
							chunk = len - i;
							if (chunk > buf.Length) chunk = buf.Length;

							StreamReadBytes(buf, chunk);

							//Array.Copy(buf,0,_attachment.FileContent,i,chunk);

							i += chunk;
						}

						GetI16();     /* checksum */
					}
					break;
				case 0x00069005: // Mime Properties
					{
						len = GetI32();   // data length

						// MAPI_ATTACH_LONG_FILENAME = 07 37 | 01 00 00 00 | File Name Length | File Name

						byte[] cmpBuffer = new byte[] { 0x07, 0x37, 0x01, 0x00, 0x00, 0x00 };
						int cmpIndex = 0;

						bool lastAttachmentHasAutoName = LastAttachmentHasAutoName();

						for (i = 0; i < len; i += 1)
						{
							int v = GetI8();

							if (lastAttachmentHasAutoName ||
								_currentOffset != null)
							{
								if (cmpIndex < cmpBuffer.Length)
								{
									if (cmpBuffer[cmpIndex] == (byte)v)
										cmpIndex++;
									else
										cmpIndex = 0;
								}

								if (cmpIndex == cmpBuffer.Length)
								{
									int fileNameLen = GetI8();
									fileNameLen += GetI8() << 8;
									fileNameLen += GetI8() << 16;
									fileNameLen += GetI8() << 32;

									if (fileNameLen > 0)
									{
										byte[] attFileNameBuffer = new byte[fileNameLen];

										StreamReadBytes(attFileNameBuffer, fileNameLen);

										string strFileName = Encoding.Default.GetString(attFileNameBuffer, 0, fileNameLen - 1);

										if (!string.IsNullOrEmpty(strFileName))
										{
											if (_currentOffset == null)
											{
												AttachmentInfo preInfo = (AttachmentInfo)_attachments[_attachments.Count - 1];

												_attachments[_attachments.Count - 1] = new AttachmentInfo(strFileName, GetContentTypeByFileName(strFileName), preInfo.Size);
											}
											else
											{
												AttachmentInfo attachment = new AttachmentInfo(strFileName, GetContentTypeByFileName(strFileName), _currentOffset.Length);
												_attachments.Add(attachment);

												_fileOffsetList.Add(strFileName, _currentOffset);
												_currentOffset = null;
											}
										}


										i += 4;
										i += fileNameLen;
									}

									lastAttachmentHasAutoName = false;
								}
							}
						}

						// TODO: Parse Mime Atributes attentively
						// int mimePropCount = GetI32();
						//for (int mimePropIndex = 0; mimePropIndex < mimePropCount; mimePropIndex++)
						//{
						//    MapiType attType = (MapiType)GetI16();
						//    Int16 attName = GetI16();
						//}

						GetI16();     /* checksum */
					}
					break;
				default:
					DecodeAttribute(d);
					break;
			}
		}

		private bool LastAttachmentHasAutoName()
		{
			return _attachments.Count > 0 &&
				((AttachmentInfo)_attachments[_attachments.Count - 1]).FileName.StartsWith("Unknown");
		}

		private void DecodeMessage() 
		{  
			int d;

			d = GetI32();

			DecodeAttribute(d);
		}

		private void DecodeAttribute (int d) 
		{
			byte[] buf=new byte[4000];
			int len;
			int v;
			int i;

			len = GetI32();   /* data length */

			switch(d&0xffff0000)
			{
				case _BYTE:
					//PrintResult("Attribute {0} =", d&0xffff);
					for (i=0; i < len; i+=1) 
					{
						v = GetI8();

//						if (i< 10) PrintResult(" {0}", v);
//						else if (i==10) PrintResult("...");
					}
					//PrintResult("\n");
					break;
				case _WORD:
					//PrintResult("Attribute {0} =", d&0xffff);
					for (i=0; i < len; i+=2) 
					{
						if(len==1)
							v = GetI8();
						else
							v = GetI16();

//						if (i < 6) PrintResult(" {0}", v);
//						else if (i==6) PrintResult("...");
					}
					//PrintResult("\n");
					break;
				case _DWORD:
					//PrintResult("Attribute {0} =", d&0xffff);
					for (i=0; i < len; i+=4) 
					{
						if(len==1)
							v = GetI8();
						else if(len==2)
							v = GetI16();
						else
							v = GetI32();

//						if (i < 4) PrintResult(" {0}", v);
//						else if (i==4) PrintResult("...");
					}
					//PrintResult("\n");
					break;
				case _string:
					StreamReadBytes(buf, len);

					//PrintResult("Attribute {0} = {1}\n", d&0xffff, Encoding.Default.GetString(buf));
					break;
				default:
					StreamReadBytes(buf, len);
					//PrintResult("Attribute {0}\n", d);
					break;
			}

			GetI16();     /* checksum */
		}


		private int GetInt32(byte[] p)
		{
			return (p[0]+(p[1]<<8)+(p[2]<<16)+(p[3]<<24));
		}

		private short GetInt16(byte[] p)
		{
			return (short)(p[0]+(p[1]<<8));
		}

		private int GetI32 () 
		{
			byte[] buf=new byte[4];

			if(StreamReadBytes(buf,4)!=1)
			{
				//Utility.LogError("GetI32():unexpected end of input\n");
				return 1;
			}
			return GetInt32(buf);
		}

		private int GetI16 () 
		{
			byte[] buf=new byte[2];

			if(StreamReadBytes(buf,2)!=1)
			{
				//Utility.LogError("GetI16():unexpected end of input\n");
				return 1;
			}
			return GetInt16(buf);
		}

		private int GetI8 () 
		{
			byte[] buf=new byte[1];

			if(StreamReadBytes(buf,1)!=1)
			{
				//Utility.LogError("GetI8():unexpected end of input\n");
				return 1;
			}
			return (int)buf[0];
		}

		private int StreamReadBytes(byte[] buffer, int size)
		{
			try
			{
				if(fsTNEF.Position+size<=_fileLength)					
				{
					fsTNEF.Read(buffer,0,size);
					return 1;
				}
				else
					return 0;
			}
			catch(Exception)				
			{				
				//Utility.LogError("StreamReadBytes():"+e.Message);
				return 0;
			}
		}
	}
}
