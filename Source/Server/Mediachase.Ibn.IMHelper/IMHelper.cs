using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Configuration;

namespace Mediachase.Ibn
{
	internal enum IbnCommand
	{
		UpdateWebStub = 1,		//	GroupID
		UpdateUser = 2,			//	UserID
		UpdateGroup = 3,		//	GroupID
		SendAlertToGroup = 4,	//	GroupID, bsParam
		SendAlertToUser = 5,	//	UserID, bsParam
		SendMessage = 6,		//	ToID, FromID, Message
		LogOff = 7,				//	UserId
		UpdateUserWebStub = 8	//	UserID
	};

	/// <summary>
	/// Invokes IbnClient commands.
	/// </summary>
	public static class IMHelper
	{
		#region constants

		internal const string IbnServerPipeName = "\\\\.\\pipe\\IBNServer" + IbnConst.VersionMajorMinor;
		internal const UInt32 PIPE_READMODE_MESSAGE = 0x00000002;
		internal const uint GENERIC_READ = (0x80000000);
		internal const uint GENERIC_WRITE = (0x40000000);
		internal const uint OPEN_EXISTING = 3;
		internal const int ERROR_PIPE_BUSY = 231;

		#endregion

		#region public

		/// <summary>
		/// Updates the web stub.
		/// </summary>
		/// <param name="groupId">The group id.</param>
		/// <returns></returns>
		public static int UpdateWebStub(int groupId)
		{
			return SendCommand(IbnCommand.UpdateWebStub, groupId, null, null);
		}

		/// <summary>
		/// Updates the user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static int UpdateUser(int userId)
		{
			return SendCommand(IbnCommand.UpdateUser, userId, null, null);
		}

		/// <summary>
		/// Updates the group.
		/// </summary>
		/// <param name="groupId">The group id.</param>
		/// <returns></returns>
		public static int UpdateGroup(int groupId)
		{
			return SendCommand(IbnCommand.UpdateGroup, groupId, null, null);
		}

		/// <summary>
		/// Sends the alert to group.
		/// </summary>
		/// <param name="groupId">The group id.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public static int SendAlertToGroup(int groupId, string message)
		{
			return SendCommand(IbnCommand.SendAlertToGroup, groupId, null, message);
		}

		/// <summary>
		/// Sends the alert to user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public static int SendAlertToUser(int userId, string message)
		{
			return SendCommand(IbnCommand.SendAlertToUser, userId, null, message);
		}

		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="recipientId">The recipient id.</param>
		/// <param name="senderId">The sender id.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public static int SendMessage(int recipientId, int senderId, string message)
		{
			return SendCommand(IbnCommand.SendMessage, recipientId, senderId, message);
		}

		/// <summary>
		/// Logs the off.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static int LogOff(int userId)
		{
			return SendCommand(IbnCommand.LogOff, userId, null, null);
		}

		/// <summary>
		/// Updates the user web stub.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static int UpdateUserWebStub(int userId)
		{
			return SendCommand(IbnCommand.UpdateUserWebStub, userId, null, null);
		}

		#endregion

		#region private

		/// <summary>
		/// Gets the name of the ibn server pipe.
		/// </summary>
		/// <returns></returns>
		public static string GetIbnServerPipeName()
		{
			string retVal = IbnServerPipeName;

			// Read companyUid from Config file
			string companyUid = ConfigurationManager.AppSettings["CompanyUid"];
			if (!string.IsNullOrEmpty(companyUid))
			{
				retVal += "_";
				retVal += companyUid;
			}

			return retVal;
		}

		/// <summary>
		/// Sends the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="id1">The id1.</param>
		/// <param name="id2">The id2.</param>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		private static int SendCommand(IbnCommand command, int id1, int? id2, string text)
		{
			int result;
			IntPtr hPipe = OpenPipe();

			try
			{
				byte[] data = CreateBuffer(command, id1, id2, text);
				byte[] outBuffer = { 0, 0, 0, 0 };
				UInt32[] bytesRead = { 0 };

				if (!NativeMethods.TransactNamedPipe(hPipe, data, Convert.ToUInt32(data.Length), outBuffer, Convert.ToUInt32(outBuffer.Length), bytesRead, IntPtr.Zero))
				{
					int error = Marshal.GetLastWin32Error();
					throw new IMHelperException(string.Format(CultureInfo.InvariantCulture, "Pipe '{2}' command error. Command: {0}. Last Win32 error: {1}.", command, error, GetIbnServerPipeName()));
				}
				else
				{
					result = Convert.ToInt32((UInt32)outBuffer[0] | ((UInt32)outBuffer[1] << 8) | ((UInt32)outBuffer[2] << 16) | ((UInt32)outBuffer[3] << 24));
				}
			}
			finally
			{
				NativeMethods.CloseHandle(hPipe);
			}
			return result;
		}

		/// <summary>
		/// Opens the pipe.
		/// </summary>
		/// <returns></returns>
		private static IntPtr OpenPipe()
		{
			IntPtr fileHandle;
			IntPtr invalidHandleValue = new IntPtr(-1);

			while (true)
			{
				fileHandle = NativeMethods.CreateFile(GetIbnServerPipeName(), GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
				int error = Marshal.GetLastWin32Error();

				if (fileHandle != invalidHandleValue)
				{
					UInt32[] mode = { PIPE_READMODE_MESSAGE };
					NativeMethods.SetNamedPipeHandleState(fileHandle, mode, null, null);
					break;
				}

				if (error != 0 && error != ERROR_PIPE_BUSY)
				{
					throw new IMHelperException(string.Format(CultureInfo.InvariantCulture, "Cannot open named pipe '{1}'. Last Win32 error: {0}", error, GetIbnServerPipeName()));
				}

				// Wait a little [9/20/2004]
				if (!NativeMethods.WaitNamedPipe(GetIbnServerPipeName(), 3000))
				{
					error = Marshal.GetLastWin32Error();
					throw new IMHelperException(string.Format(CultureInfo.InvariantCulture, "Wait for named pipe '{1}' timed out. Last Win32 error: {0}", error, GetIbnServerPipeName()));
				}
			}

			return fileHandle;
		}

		/// <summary>
		/// Creates the buffer.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="number1">The number1.</param>
		/// <param name="number2">The number2.</param>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		private static byte[] CreateBuffer(IbnCommand command, int number1, int? number2, string text)
		{
			int dataLength = 4;
			if (number2 != null)
				dataLength += 4;
			if (text != null)
				dataLength += (text.Length + 1) * 2;

			using (MemoryStream stream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.Unicode))
				{
					writer.Write((Int32)1);
					writer.Write((Int32)command);
					writer.Write((Int32)dataLength);
					writer.Write((Int32)number1);
					if (number2 != null)
						writer.Write((Int32)number2.Value);
					if (text != null)
						writer.Write(Encoding.Unicode.GetBytes(text));
					writer.Write((byte)0);
					writer.Write((byte)0);

					writer.Flush();
				}
				return stream.ToArray();
			}
		}

		#endregion
	}
}
