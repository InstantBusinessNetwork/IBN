using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// 
	/// </summary>
	public class DBFile
	{
		private const int iBufferSize = 655360; // 640 KB

		public static int Create(
			string FileName,
			int    ContentTypeId)
		{
			return DbHelper2.RunSpInteger("FileCreate",
				DbHelper2.mp("@FileName",      SqlDbType.NVarChar, 250, FileName),
				DbHelper2.mp("@ContentTypeId", SqlDbType.Int, ContentTypeId));
		}

		public static void Delete(int FileId)
		{
			DbHelper2.RunSp("FileDelete", 
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
		}

		/// <summary>
		/// Reader returns fields:
		///		FileId, FileName, ContentType, Size
		/// </summary>
		public static IDataReader GetInfo(int FileId)
		{
			return DbHelper2.RunSpDataReader("FileGetInfo", DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
		}

		public static IDataReader GetBinaryData(int FileId)
		{
			return DbHelper2.RunSpDataReaderBlob("FileGetData", 
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId));
		}

		public static void WriteToStream(int FileId, Stream stream)
		{
			byte[] outbyte = new byte[iBufferSize]; 

			using(IDataReader reader = GetBinaryData(FileId))
			{
				if(reader != null)
				{
					if(reader.Read())
					{
						long read, startIndex = 0;
						do
						{
							read = reader.GetBytes(0, startIndex, outbyte, 0, iBufferSize);
							stream.Write(outbyte, 0, System.Convert.ToInt32(read));
							startIndex += read;
						} while(read == iBufferSize);
					}
				}
			}
		}

		public static void ReadFromStream(int FileId, Stream stream)
		{
			DbTransaction.Demand();

			SqlParameter PointerOutParam  = DbHelper2.mp("@Pointer", SqlDbType.VarBinary, 16, null);
			PointerOutParam.Direction = ParameterDirection.Output;

			DbHelper2.RunSp("FileUpdateDataPrepare",
				DbHelper2.mp("@FileId", SqlDbType.Int, FileId),
				DbHelper2.mp("@Size", SqlDbType.Int, System.Convert.ToInt32(stream.Length)),
				PointerOutParam);

			// Set up UPDATETEXT command, parameters, and open BinaryReader.

			SqlParameter PointerParam = DbHelper2.mp("@Pointer", SqlDbType.Binary, 16, null);
			SqlParameter OffsetParam = DbHelper2.mp("@Offset", SqlDbType.Int, null);
			SqlParameter DeleteParam = DbHelper2.mp("@Delete", SqlDbType.Int, 1);
			SqlParameter BytesParam  = DbHelper2.mp("@Bytes", SqlDbType.Image, null);
			
			System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
			int Offset = 0;
			OffsetParam.Value = Offset;
			SqlCommand cmd = null;
			
			// Read buffer full of data and execute UPDATETEXT statement.
			
			Byte [] Buffer = br.ReadBytes(iBufferSize);
			while(Buffer.Length > 0)
			{
				PointerParam.Value = PointerOutParam.Value;
				BytesParam.Value = Buffer;
				
				cmd = DbHelper2.RunSp(cmd,
					"FileUpdateData",
					PointerParam,
					OffsetParam,
					DeleteParam,
					BytesParam);

				DeleteParam.Value = 0; //Do not delete any other data.
				Offset += Buffer.Length;
				OffsetParam.Value = Offset;
				Buffer = br.ReadBytes(iBufferSize);
			}

			br.Close();
		}
	}
}
