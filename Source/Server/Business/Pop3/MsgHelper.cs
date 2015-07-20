using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for MsgHelper.
	/// </summary>
	
	class NativeCalls
	{
		public const int S_OK = 0;
		public const int STGM_SIMPLE = 0x08000000;
		public const int STGM_READ = 0x00000000;
		public const int STGM_READWRITE = 0x00000002;
		public const int STGM_TRANSACTED = 0x00010000;
		public const int STGM_CREATE = 0x00001000;
		public const int STGM_SHARE_EXCLUSIVE = 0x00000010;
		public const int STGM_DIRECT = 0x00000000;

		public const int STGC_OVERWRITE = 8;

		public const int STGTY_STORAGE	= 1;
		public const int STGTY_STREAM	= 2;
		public const int STGTY_LOCKBYTES = 3;
		public const int STGTY_PROPERTY	= 4;

		[ComVisible(false)]
			[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000A-0000-0000-C000-000000000046")]
			public interface ILockBytes
		{
			void ReadAt(long ulOffset, System.IntPtr pv, int cb, out UIntPtr pcbRead);
			void WriteAt(long ulOffset, System.IntPtr pv, int cb, out UIntPtr pcbWritten);
			void LockRegion(long libOffset, long cb, int dwLockType);
			void UnlockRegion(long libOffset, long cb, int dwLockType);
			void SetSize(long cb);
			void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
			void Flush();
		}

		[ComImport]
			[Guid("0000000b-0000-0000-C000-000000000046")]
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IStorage 
		{
			void CreateStream(
				/* [string][in] */ string pwcsName,
				/* [in] */ uint grfMode,
				/* [in] */ uint reserved1,
				/* [in] */ uint reserved2,
				/* [out] */ out System.Runtime.InteropServices.ComTypes.IStream ppstm);

			void OpenStream(
				/* [string][in] */ string pwcsName,
				/* [unique][in] */ IntPtr reserved1,
				/* [in] */ uint grfMode,
				/* [in] */ uint reserved2,
				/* [out] */ out System.Runtime.InteropServices.ComTypes.IStream ppstm);

			void CreateStorage(
				/* [string][in] */ string pwcsName,
				/* [in] */ uint grfMode,
				/* [in] */ uint reserved1,
				/* [in] */ uint reserved2,
				/* [out] */ out IStorage ppstg);

			void OpenStorage(
				/* [string][unique][in] */ string pwcsName,
				/* [unique][in] */ IStorage pstgPriority,
				/* [in] */ uint grfMode,
				/* [unique][in] */ IntPtr snbExclude,
				/* [in] */ uint reserved,
				/* [out] */ out IStorage ppstg);

			void CopyTo(
				/* [in] */ uint ciidExclude,
				/* [size_is][unique][in] */ Guid rgiidExclude,
				/* [unique][in] */ IntPtr snbExclude,
				/* [unique][in] */ IStorage pstgDest);

			void MoveElementTo(
				/* [string][in] */ string pwcsName,
				/* [unique][in] */ IStorage pstgDest,
				/* [string][in] */ string pwcsNewName,
				/* [in] */ uint grfFlags);

			void Commit(
				/* [in] */ uint grfCommitFlags);

			void Revert();

			void DestroyElement(
				/* [string][in] */ string pwcsName);

			void RenameElement(
				/* [string][in] */ string pwcsOldName,
				/* [string][in] */ string pwcsNewName);

			void SetElementTimes(
				/* [string][unique][in] */ string pwcsName,
				/* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pctime,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME patime,
                /* [unique][in] */ System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

			void SetClass(
				/* [in] */ Guid clsid);

			void SetStateBits(
				/* [in] */ uint grfStateBits,
				/* [in] */ uint grfMask);

			void Stat(
                /* [out] */ out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg,
				/* [in] */ uint grfStatFlag);

		}

		[DllImport("ole32.dll")]
		public static extern int StgOpenStorageOnILockBytes(ILockBytes plkbyt,
			IStorage pStgPriority, uint grfMode, IntPtr snbEnclude, uint reserved,
			out IStorage ppstgOpen);	

		[DllImport("ole32.dll")]
		public static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool
			fDeleteOnRelease, out ILockBytes ppLkbyt);
		[DllImport("kernel32.dll")]
		public static extern UIntPtr GlobalSize(IntPtr hMem);

        public static System.Runtime.InteropServices.ComTypes.FILETIME 
            DateTimeToFiletime(DateTime time) 
		{
            System.Runtime.InteropServices.ComTypes.FILETIME ft;
			long hFT1 = time.ToFileTimeUtc();
			ft.dwLowDateTime = (int) (hFT1 & 0xFFFFFFFF);
			ft.dwHighDateTime = (int) (hFT1 >> 32);
			return ft;
		}
	}

	internal class MsgHelper : IDisposable
	{
		NativeCalls.ILockBytes	comPtr = null;
		NativeCalls.IStorage strge = null;
		IntPtr glbPtr = IntPtr.Zero;
		MemoryStream properties = null;

		public MsgHelper(Stream stream)
		{
			glbPtr = Marshal.AllocHGlobal((int)stream.Length);
			
			NativeCalls.CreateILockBytesOnHGlobal(glbPtr, true, out comPtr);

			byte[] buffer = new byte[10240];
			int page = 10240;
			int size = (int)stream.Length;
			IntPtr ptr = new IntPtr(glbPtr.ToInt32());
			while (size > 0)
			{
				if (size < page)
					page = size;

				stream.Read(buffer, 0, page);
				Marshal.Copy(buffer, 0, ptr, page);

				size -= page;
				ptr = new IntPtr(ptr.ToInt32() + page);
			}
			Int32 hr = NativeCalls.StgOpenStorageOnILockBytes(comPtr, 				
				(NativeCalls.IStorage)null, 
				NativeCalls.STGM_READWRITE | NativeCalls.STGM_SHARE_EXCLUSIVE, 
				IntPtr.Zero,
				0,
				out strge);
	
			properties = new MemoryStream();


            System.Runtime.InteropServices.ComTypes.IStream ppstm = getStream("__properties_version1.0");
			readStream(ppstm, properties);
			Marshal.ReleaseComObject(ppstm);
		}

        void readStream(System.Runtime.InteropServices.ComTypes.IStream ppstm, MemoryStream dest)
		{
			int size = 0;

            System.Runtime.InteropServices.ComTypes.STATSTG statstgstream;
			ppstm.Stat(out statstgstream, 1);
			size = (int)statstgstream.cbSize;
		
			IntPtr pm = new IntPtr();
			byte[] buffer = new byte[1024];
			int page = 1024;
			while (size > 0)
			{
				if (size < page)
					page = size;

				ppstm.Read(buffer, page, pm);
				dest.Write(buffer, 0, page);

				size -= page;
			}
		}

        System.Runtime.InteropServices.ComTypes.IStream getStream(string header)
		{
            System.Runtime.InteropServices.ComTypes.IStream ppstm = null;
			strge.OpenStream(header,
				IntPtr.Zero,
				NativeCalls.STGM_READWRITE | NativeCalls.STGM_SHARE_EXCLUSIVE,
				(uint)0,
				out ppstm);

			return ppstm;
		}

		void modifyProperty(uint code, int value)
		{
			properties.Seek(0x20, SeekOrigin.Begin);

			BinaryReader reader = new BinaryReader(properties);
			while (reader.PeekChar() != -1)
			{
				int property = reader.ReadInt32();
				reader.ReadInt32();
				if (property==code) 
				{
					BinaryWriter writer = new BinaryWriter(properties);
					writer.Write(value);
					writer.Flush();
					return;
				}
				else properties.Seek(8, SeekOrigin.Current);
			}
		}

		void modifyProperty(uint code, int lvalue, int hvalue)
		{
			properties.Seek(0x20, SeekOrigin.Begin);

			BinaryReader reader = new BinaryReader(properties);
			while (reader.PeekChar() != -1)
			{
				int property = reader.ReadInt32();
				reader.ReadInt32();
				if (property==code) 
				{
					BinaryWriter writer = new BinaryWriter(properties);
					writer.Write(lvalue);
					writer.Write(hvalue);
					writer.Flush();
					return;
				}
				else properties.Seek(8, SeekOrigin.Current);
			}
		}

		protected void Set(string key, uint keyValue, string value)
		{
            System.Runtime.InteropServices.ComTypes.IStream ppstm = getStream(key);
			IntPtr		pm = new IntPtr();
			byte[]		p = Encoding.Default.GetBytes(value);

			ppstm.Write(p, p.Length, pm);
			ppstm.SetSize(p.Length);

			ppstm.Commit(0);
			Marshal.ReleaseComObject(ppstm);

			modifyProperty(keyValue, p.Length + 1);
		}

		protected void SetUTF(string key, uint keyValue, string value)
		{
            System.Runtime.InteropServices.ComTypes.IStream ppstm = getStream(key);
			IntPtr		pm = new IntPtr();
			byte[]		p = Encoding.UTF8.GetBytes(value);

			ppstm.Write(p, p.Length, pm);
			ppstm.SetSize(p.Length);

			ppstm.Commit(0);
			Marshal.ReleaseComObject(ppstm);

			modifyProperty(keyValue, p.Length + 1);
		}

		public void SetSubject(string subject)
		{
			Set("__substg1.0_0037001E", 0x0037001E, subject);
			Set("__substg1.0_0E1D001E", 0x0E1D001E, subject);
		}

		public void SetBody(string body)
		{
			Set("__substg1.0_1000001E", 0x1000001E, body);
		}

		public void SetHtmlBody(string htmlbody)
		{
			SetUTF("__substg1.0_1013001E", 0x1013001E, htmlbody);
		}

		public void SetSenderEmail(string email)
		{
			Set("__substg1.0_0C1F001E", 0x0C1F001E, email);
		}

		public void SetSenderName(string name)
		{
			Set("__substg1.0_0C1A001E", 0x0C1A001E, name);
		}

		public void SetReceiverEmail(string email)
		{
			Set("__substg1.0_0076001E", 0x0076001E, email);
		}

		public void SetReceiverName(string name)
		{
			Set("__substg1.0_0040001E", 0x0040001E, name);
		}
	
		public void SetDisplayTo(string name)
		{
			Set("__substg1.0_0E04001E", 0x0E04001E, name);
		}


		public void SetCreationTimes(DateTime date)
		{
            System.Runtime.InteropServices.ComTypes.FILETIME time = NativeCalls.DateTimeToFiletime(date);

			modifyProperty(0x00390040, time.dwLowDateTime, time.dwHighDateTime);	// PR_CLIENT_SUBMIT_TIME
			modifyProperty(0x30070040, time.dwLowDateTime, time.dwHighDateTime);	// PR_CREATION_TIME
			modifyProperty(0x30080040, time.dwLowDateTime, time.dwHighDateTime);	// PR_LAST_MODIFICATION_TIME
			modifyProperty(0x0E060040, time.dwLowDateTime, time.dwHighDateTime);	// PR_MESSAGE_DELIVERY_TIME
		}

		public void Commit()
		{
            System.Runtime.InteropServices.ComTypes.IStream ppstm = getStream("__properties_version1.0");
			IntPtr		pm = new IntPtr();
			byte[]		p = properties.ToArray();

			ppstm.Write(p, p.Length, pm);
			ppstm.Commit(0);

			strge.Commit(0);

			properties.Close();
			properties = null;
		}

		public void createMSG(Stream stream)
		{
			byte[] buffer = new byte[10240];
			int page = 10240;
			int size = (int)NativeCalls.GlobalSize(glbPtr).ToUInt32();
			IntPtr ptr = glbPtr;
			while (size > 0)
			{
				if (size < stream.Length)
					page = size;

				Marshal.Copy(ptr, buffer, 0, page);
				stream.Write(buffer, 0, page);

				size -= page;
				ptr = new IntPtr(ptr.ToInt32() + page);
			}
		}

		protected void releaseAll()
		{
			Marshal.ReleaseComObject(strge);
			strge = null;
			Marshal.ReleaseComObject(comPtr);
			comPtr = null;
			if (properties!=null)
			{
				properties.Close();
				properties = null;
			}
		}

		public void Dispose()
		{
			releaseAll();
		}
	}
}
