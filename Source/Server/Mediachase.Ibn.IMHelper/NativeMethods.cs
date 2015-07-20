using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mediachase.Ibn
{
	internal class NativeMethods
	{
		private NativeMethods()
		{
		}

		[DllImport("kernel32.dll")]
		internal static extern IntPtr CreateFile( // HANDLE
			String lpFileName, // LPCTSTR [in]
			UInt32 dwDesiredAccess, // DWORD [in]
			UInt32 dwShareMode, // DWORD [in]
			IntPtr lpSecurityAttributes, // LPSECURITY_ATTRIBUTES [in]
			UInt32 dwCreationDisposition, // DWORD [in]
			UInt32 dwFlagsAndAttributes, // DWORD [in]
			IntPtr hTemplateFile // HANDLE [in]
			);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WaitNamedPipe(
			String lpNamedPipeName, // LPCTSTR [in]
			UInt32 nTimeOut // DWORD [in]
			);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(
			IntPtr hObject // HANDLE [in]
			);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool TransactNamedPipe(
			IntPtr hNamedPipe, // HANDLE [in]
			byte[] lpInBuffer, // LPVOID [in]
			UInt32 nInBufferSize, // DWORD [in]
			byte[] lpOutBuffer, // LPVOID [out]
			UInt32 nOutBufferSize, // DWORD [in]
			UInt32[] lpBytesRead, // LPDWORD [out]
			IntPtr lpOverlapped // LPOVERLAPPED [in]
			);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetNamedPipeHandleState(
			IntPtr hNamedPipe, // HANDLE [in]
			UInt32[] lpMode, // LPDWORD [in]
			UInt32[] lpMaxCollectionCount, // LPDWORD [in]
			UInt32[] lpCollectDataTimeout // LPDWORD [in]
			);
	}
}
