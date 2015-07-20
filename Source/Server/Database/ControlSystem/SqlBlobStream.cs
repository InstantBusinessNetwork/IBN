using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Defines constants for read, write, or read/write access to a text, ntext, or image column.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.
	/// </para>
	/// </remarks>
	[Flags]
	public enum SqlBlobAccess
	{
		/// <summary>
		/// Read access to the text, ntext, or image column. Data can be read from the column. Combine with Write for read/write access.
		/// </summary>
		Read = 1,

		/// <summary>
		/// Write access to the text, ntext, or image column. Data can be read from the column. Combine with Read for read/write access.
		/// </summary>
		Write = 2,

		/// <summary>
		/// Read and write access to the text, ntext, or image column. Data can be read from and written to the column.
		/// </summary>
		ReadWrite = 3
	}

	/// <summary>
	/// Exposes a Stream around a text, ntext, or image column, supporting both synchronous and asynchronous read and write operations.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A common practice to reduce the amount of memory used when writing a BLOB value is to write the BLOB to the database in "chunks". 
	/// The process of writing a BLOB to a database in this way depends on the capabilities of your database.
	/// </para>
	/// <para>
	/// Any public static (Shared in Visual Basic) members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
	/// </para>
	/// <para>
	/// Use the SqlBlobStream class to read, write, create or close text, ntext, or image values from a text, ntext, or image column, as well as to 
	/// manipulate other file-related operating system handles such as file.
	/// </para>
	/// <para>
	/// SqlBlobStream objects support random access to files using the Seek method. Seek allows the read/write position to be moved to any position within the file. 
	/// This is done with byte offset reference point parameters. The byte offset is relative to the seek reference point, which can be the beginning, the current 
	/// position, or the end of the underlying file, as represented by the three properties of the SeekOrigin class.
	/// </para>
	/// <para>
	/// For an example of using this class, see the Example section below. 
	/// </para>
	/// </remarks>
	/// <example>
	/// The following SqlBlobStream constructor grants read-only access to an column (SqlBlobAccess.Read).
	/// <code>
	///	string strConnectionString = "Data source=(local);Initial catalog=TestDB;User Id=sa;Password=";
	///	string strSqlTable = "Files";
	///	string strBLOBColumn = "Data";
	///
	///	try
	///	{
	///		using(SqlBlobStream stream = new SqlBlobStream(strConnectionString,
	///				strSqlTable,
	///				strBLOBColumn,
	///				SqlBlobAccess.Read,new SqlParameter("@FileId",12345)))
	///		{
	///			byte[] tmpBuffer = new byte[1024];
	///			int Length = 0;
	///			while((Length=stream.Read(buffer,0,1024))==1024)
	///			{
	///				//TODO: Save tmpBuffer
	///			}
	///		}
	///	}
	///	catch(Exception ex)
	///	{
	///		System.Diagnostics.Trace.WriteLine(ex);
	///		throw;
	///	}
	/// </code>
	/// </example>
	public class SqlBlobStream: Stream
	{
		private string						_connectionString		=	string.Empty;
		private SqlTransaction				_transaction			=	null;
		private string						_tableName				=	string.Empty;
		private string						_blobColumnName			=	string.Empty;
		private SqlParameterList			_primaryKey				=	new SqlParameterList();

		private bool						_canSeek				=	true;

		private	SqlBlobAccess				_blobAccess;

		private long						_lenght					=	0;
		private long						_position				=	0;

		// Write Operation's variables			
		private	byte[]						_pointer				=	null;

		static	byte[]						DBNullPointer			=	new byte[]{0xD7,0x03,0xAB,0x45,0x69,0x55,0x43,0x72,0xBD,0x5F,0x59,0xE3,0x6E,0xCB,0xBB,0xBF};


		/// <summary>
		/// Initializes a new instance of <b>SqlBlobStream</b> class.
		/// </summary>
		/// <remarks>
		/// Invoke <see cref="Mediachase.FileUploader.SqlBlobStream.Open"/> method to open stream.
		/// </remarks>
		public SqlBlobStream()
		{
		}

		/// <summary>
		/// Initializes a new instance of <b>SqlBlobStream</b> class for the specified 
		/// Connection String, Table Name, BLOB Data Column, with the specified read/write permission and Primary Keys.
		/// </summary>
		/// <param name="ConnectionString">A sql connection string that includes the source database name, and other parameters needed to establish the initial connection. The default value is an empty string.</param>
		/// <param name="TableName">A table name for the table that the current SqlBlobStream object will encapsulate.</param>
		/// <param name="BlobDataColumn">A column name for the column that the current SqlBlobStream object will encapsulate.</param>
		/// <param name="BlobAccess">A SqlBlobAccess constant that gets the CanRead and CanWrite properties of the SqlBlobStream object.</param>
		/// <param name="PrimaryKeys">A primary keys collection for the row that the current SqlBlobStream object will encapsulate.</param>
		public SqlBlobStream(string ConnectionString, string TableName, string BlobDataColumn, SqlBlobAccess BlobAccess, params SqlParameter[]	PrimaryKeys)
		{
			_connectionString	=	ConnectionString;
			_tableName			=	TableName;
			_blobColumnName		=	BlobDataColumn;
			_blobAccess			=	BlobAccess;

			if(PrimaryKeys!=null)
			{
				foreach(SqlParameter	prm in PrimaryKeys)
				{
					this.PrimaryKeys.Add(prm);
				}
			}

			Init();
		}

		/// <summary>
		/// Initializes a new instance of <b>SqlBlobStream</b> class for the specified 
		/// Transaction, Table Name, BLOB Data Column, with the specified read/write permission and Primary Keys.
		/// </summary>
		/// <param name="Transaction">A sql transaction. The default value is an null.</param>
		/// <param name="TableName">A table name for the table that the current <b>SqlBlobStream</b> object will encapsulate.</param>
		/// <param name="BlobDataColumn">A column name for the column that the current <b>SqlBlobStream</b> object will encapsulate.</param>
		/// <param name="BlobAccess">A <b>SqlBlobAccess</b>constant that gets the CanRead and CanWrite properties of the <b>SqlBlobStream</b> object.</param>
		/// <param name="PrimaryKeys">A primary keys collection for the row that the current <b>SqlBlobStream</b> object will encapsulate.</param>
		public SqlBlobStream(SqlTransaction Transaction, string TableName, string BlobDataColumn, SqlBlobAccess BlobAccess, params SqlParameter[]	PrimaryKeys)
		{
			_transaction		=	Transaction;
			_tableName			=	TableName;
			_blobColumnName		=	BlobDataColumn;
			_blobAccess			=	BlobAccess;

			if(PrimaryKeys!=null)
			{
				foreach(SqlParameter	prm in PrimaryKeys)
				{
					this.PrimaryKeys.Add(prm);
				}
			}

			Init();
		}

		/// <summary>
		/// Opens the closed connection.
		/// </summary>
		public void Open()
		{
			if(this.Pointer!=null)
				throw new Exception("The stream is open.");

			this.Init();
		}

		/// <summary>
		/// Gets the current SqlBlobStream's state.
		/// </summary>
		public bool IsOpen
		{
			get
			{
				return this.Pointer!=null;
			}
		}

		/// <summary>
		/// Initializes a new connection.
		/// </summary>
		protected void Init()
		{
			switch(this.BlobAccess)
			{
				case SqlBlobAccess.Write:
				case SqlBlobAccess.Read:
					_lenght = GetDataLength();
					_pointer = OpenPointer();
					break;
				default:
					throw new Exception();
			}

			OnInit();
		}

		/// <summary>
		/// Raises the Init event.
		/// </summary>
		protected virtual void OnInit()
		{
		}

		#region -- SqlBlobStream properties --
		/// <summary>
		/// Gets a current pointer to the BLOB value.
		/// </summary>
		public byte[] Pointer
		{
			get
			{
				return _pointer;
			}
		}

		/// <summary>
		/// Gets a current read/write permission.
		/// </summary>
		public virtual SqlBlobAccess BlobAccess
		{
			get
			{
				return _blobAccess;
			}
		}

		/// <summary>
		/// Gets the primary keys collection.
		/// </summary>
		public virtual SqlParameterList PrimaryKeys
		{
			get
			{
				return _primaryKey;
			}
		}

		/// <summary>
		/// Gets, sets a sql connection string that includes the source database name, and other parameters needed to establish the initial connection. 
		/// </summary>
		public virtual string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
			}
		}

		/// <summary>
		/// Gets, sets a current sql transaction. 
		/// </summary>
		public virtual SqlTransaction Transaction
		{
			get
			{
				return _transaction;
			}
			set
			{
				_transaction = value;
			}
		}

		/// <summary>
		/// Gets, sets a table name for the table that the current <b>SqlBlobStream</b> object will encapsulate.
		/// </summary>
		public virtual string TableName
		{
			get
			{
				return _tableName;
			}
			set
			{
				_tableName = value;
			}
		}

		/// <summary>
		/// Gets, sets a table column for the BLOB value that the current <b>SqlBlobStream</b> object will encapsulate.
		/// </summary>
		public virtual string ColumnName
		{
			get
			{
				return _blobColumnName;
			}
			set
			{
				_blobColumnName = value;
			}
		}
		#endregion

		#region -- Stream Methods --	
		/// <summary>
		/// Overridden. Reads a block of bytes from the stream and writes the data in a given buffer.
		/// </summary>
		/// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The byte offset in array at which to begin reading. </param>
		/// <param name="count">The maximum number of bytes to read. </param>
		/// <returns>The total number of bytes read into the buffer. This might be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if(this.Pointer==null)
				throw new ObjectDisposedException(null,"The stream is closed.");

			if(!this.CanRead)
				throw new NotSupportedException("SqlBlobStream is Writeonly.");

			if(this.Pointer==SqlBlobStream.DBNullPointer)
				return 0;

			return ReadText(buffer,offset,count);
		}
	
		/// <summary>
		/// Overridden. Writes a block of bytes to this stream using data from a buffer.
		/// </summary>
		/// <param name="buffer">The array to which bytes are written. </param>
		/// <param name="offset">The byte offset in array at which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if(this.Pointer==null)
				throw new ObjectDisposedException(null,"The stream is closed.");

			if(!this.CanWrite)
				throw new NotSupportedException("SqlBlobStream is Readonly.");

			if(this.Pointer!=DBNullPointer)
			{
				UpdateText(buffer,offset,count);
			}
			else
			{
				UpdateTextCommandWithNullValue(buffer,offset,count);
				_pointer = OpenPointer();
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return (_blobAccess==SqlBlobAccess.Read)&&(this.Pointer!=null);			
			}
		}
	
		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return _canSeek&&(this.Pointer!=null);
			}
		}
	
		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return (_blobAccess==SqlBlobAccess.Write)&&(this.Pointer!=null);
			}
		}
	
		/// <summary>
		/// Overridden. Gets or sets the current position of this stream.
		/// </summary>
		public override long Position
		{
			get
			{
				if(this.Pointer==null)
					throw new ObjectDisposedException(null,"The stream is closed.");

				return _position;
			}
			set
			{
				if(this.Pointer==null)
					throw new ObjectDisposedException(null,"The stream is closed.");
				_position	=	value;
			}
		}
	
		/// <summary>
		/// Overridden. Gets the length in bytes of the stream.
		/// </summary>
		public override long Length
		{
			get
			{
				if(this.Pointer==null)
					throw new ObjectDisposedException(null,"The stream is closed.");

				return _lenght;
			}
		}
	
		/// <summary>
		/// Overridden. Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
		}
	
		/// <summary>
		/// Overridden. Sets the current position of this stream to the given value.
		/// </summary>
		/// <param name="offset">The point relative to origin from which to begin seeking.</param>
		/// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for origin, using a value of type SeekOrigin.</param>
		/// <returns>The new position in the stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			if(this.Pointer==null)
				throw new ObjectDisposedException(null,"The stream is closed.");

			int	newPosition = 0;

			switch(origin)
			{
				case SeekOrigin.Begin:
					newPosition	=	(int)offset;
					break;
				case SeekOrigin.Current:
					newPosition	=	(int)(this.Position + offset);
					break;
				case SeekOrigin.End:
					newPosition	=	(int)(this.Length + offset);
					break;
				default:
					throw new ArgumentOutOfRangeException("origin", origin,string.Empty);
			}

			if(newPosition<0||newPosition>this.Length)
				throw new ArgumentOutOfRangeException("offset",offset,string.Empty);

			this._position = newPosition;
			return newPosition;
		}
	
		/// <summary>
		/// Overridden. Sets the length of this stream to the given value.
		/// </summary>
		/// <param name="value">The new length of the stream. </param>
		public override void SetLength(long value)
		{
			if(this.Pointer==null)
				throw new ObjectDisposedException(null,"The stream is closed.");

			if(this.CanWrite)
			{
				if(value!=this.Length)
				{
					SqlContext	context	=	new SqlContext(this.ConnectionString);
					try
					{
						context.Transaction = this.Transaction;

						// Step 1. Create and Init BLOB data Pointer
						SqlParameter ptrParmPointer = new SqlParameter("@Pointer", SqlDbType.Binary, 16);
						ptrParmPointer.Value		= this.Pointer;

						if(value<this.Length)
						{
							// Step 2. Create and Init Offset
							SqlParameter ptrParmOffset = new SqlParameter("@Offset", SqlDbType.Int);
							ptrParmOffset.Value		= value;

							// Step 3. Create and Init Count
							SqlParameter ptrParmCount = new SqlParameter("@Count", SqlDbType.Int);
							ptrParmCount.Value		= this.Length - value;

							// Step 4. Create and Init Bytes
							SqlParameter ptrParmBytes = new SqlParameter("@Bytes", SqlDbType.Image);
							ptrParmBytes.Value		= new byte[]{};

							SqlHelper.ExecuteNonQuery(context,CommandType.Text,GetUpdateTextCommand(),
								ptrParmPointer,ptrParmOffset,ptrParmCount,ptrParmBytes);

							this._lenght = value;
							this.Position = Math.Min(this.Position,this.Length);
						}
						else
						{
							// Step 2. Create and Init Offset
							SqlParameter ptrParmOffset = new SqlParameter("@Offset", SqlDbType.Int);
							ptrParmOffset.Value		= this.Length;

							// Step 3. Create and Init Count
							SqlParameter ptrParmCount = new SqlParameter("@Count", SqlDbType.Int);
							ptrParmCount.Value		= 0;

							// Step 4. Create and Init Bytes
							SqlParameter ptrParmBytes = new SqlParameter("@Bytes", SqlDbType.Image);
							ptrParmBytes.Value		= new byte[(value-this.Length)];

							SqlHelper.ExecuteNonQuery(context,CommandType.Text,GetUpdateTextCommand(),
								ptrParmPointer,ptrParmOffset,ptrParmCount,ptrParmBytes);

							this._lenght = value;
						}
					}
					finally
					{
						context.Transaction = null;
					}
				}
			}
			else
			{
				throw new NotSupportedException("SqlBlobStream is Readonly.");
			}
		}
		#endregion

	
		/// <summary>
		/// Releases the resources used by the SqlBlobStream.
		/// </summary>
		/// <param name="disposing">The disposing flag.</param>
		protected new virtual void Dispose(bool disposing)
		{
			this._blobAccess	=	(SqlBlobAccess)0;
			this._canSeek = false;
			this._pointer = null;
		}
 	
		/// <summary>
		/// Overridden. Closes the file and releases any resources associated with the current file stream.
		/// </summary>
		public override void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#region -- Common Command's methods
		/// <summary>
		/// Gets the sql command returning Data Length.
		/// </summary>
		/// <returns>The sql command.</returns>
		protected virtual string GetDataLengthCommand()
		{
			StringBuilder	sb	=	new StringBuilder(255);
			sb.AppendFormat("SELECT @DataLength = datalength([{0}]) FROM [{1}]",this.ColumnName,this.TableName);

			if(this.PrimaryKeys.Count>0)
			{
				bool bFirst	=	true;

				sb.Append(" WHERE");
				foreach(SqlParameter	prm in this.PrimaryKeys)
				{
					if(!bFirst)
						sb.Append(" AND");
					else
						bFirst = false;

					sb.AppendFormat(" [{0}] = {1}",prm.ParameterName.Substring(1),prm.ParameterName);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Gets the selected element's data length.
		/// </summary>
		/// <returns>The data length. (bytes)</returns>
		protected virtual int GetDataLength()
		{
			int retVal	=	0;

			SqlContext	context	=	new SqlContext(this.ConnectionString);
			try
			{
				context.Transaction = this.Transaction;

				SqlParameter ptrParm = new SqlParameter("@DataLength", SqlDbType.Int);
				ptrParm.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(context,CommandType.Text,GetDataLengthCommand(),this.PrimaryKeys.ToArray(ptrParm));

				if(ptrParm.Value!=DBNull.Value)
					retVal	=	(int)ptrParm.Value;
			}
			finally
			{
				context.Transaction = null;
			}

			return retVal;
		}

		/// <summary>
		/// Gets the sql command returning a text pointer.
		/// </summary>
		/// <returns>The sql command.</returns>
		protected virtual string GetOpenPointerCommand()
		{
			StringBuilder	sb	=	new StringBuilder(255);
			sb.AppendFormat("SELECT @Pointer = TEXTPTR([{0}]) FROM [{1}]",this.ColumnName,this.TableName);

			if(this.PrimaryKeys.Count>0)
			{
				bool bFirst	=	true;

				sb.Append(" WHERE");
				foreach(SqlParameter	prm in this.PrimaryKeys)
				{
					if(!bFirst)
						sb.Append(" AND");
					else
						bFirst = false;

					sb.AppendFormat(" [{0}] = {1}",prm.ParameterName.Substring(1),prm.ParameterName);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Opens the text pointer.
		/// </summary>
		/// <returns>The text pointer.</returns>
		protected virtual byte[] OpenPointer()
		{
			byte[] retVal	=	null;

			SqlContext	context	=	new SqlContext(this.ConnectionString);
			try
			{
				context.Transaction = this.Transaction;

				SqlParameter ptrParm = new SqlParameter("@Pointer", SqlDbType.Binary, 16);
				ptrParm.Direction = ParameterDirection.Output;

				SqlHelper.ExecuteNonQuery(context,CommandType.Text,GetOpenPointerCommand(),this.PrimaryKeys.ToArray(ptrParm));

				if(ptrParm.Value!=DBNull.Value)
					retVal	=	(byte[])ptrParm.Value;
				else
					retVal	=	SqlBlobStream.DBNullPointer;
			}
			finally
			{
				context.Transaction = null;
			}
			return retVal;
		}
		#endregion

		#region -- Write Command's methods
		/// <summary>
		/// Gets the sql command updating a blob element.
		/// </summary>
		/// <returns>The sql command.</returns>
		protected virtual string GetUpdateTextCommand()
		{
			StringBuilder	sb	=	new StringBuilder(255);
			sb.AppendFormat("UPDATETEXT [{1}].[{0}] @Pointer @Offset @Count @Bytes",this.ColumnName,this.TableName);

			return sb.ToString();
		}

		/// <summary>
		/// Gets the sql command inserting a blob element.
		/// </summary>
		/// <returns>The sql command.</returns>
		protected virtual string GetUpdateTextCommandWithNullValue()
		{
			StringBuilder	sb	=	new StringBuilder(255);
			sb.AppendFormat("UPDATE [{1}] SET [{0}] = @{0}",this.ColumnName,this.TableName);

			if(this.PrimaryKeys.Count>0)
			{
				bool bFirst	=	true;

				sb.Append(" WHERE");
				foreach(SqlParameter	prm in this.PrimaryKeys)
				{
					if(!bFirst)
						sb.Append(" AND");
					else
						bFirst = false;

					sb.AppendFormat(" [{0}] = {1}",prm.ParameterName.Substring(1),prm.ParameterName);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Writes the array into the BLOB column.
		/// </summary>
		/// <param name="buffer">The array to which bytes are written. </param>
		/// <param name="offset">The byte offset in array at which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		protected virtual void UpdateText(byte[] buffer, long offset, long count)
		{
			SqlContext	context	=	new SqlContext(this.ConnectionString);
			try
			{
				context.Transaction = this.Transaction;

				// Step 1. Create and Init BLOB data Pointer
				SqlParameter ptrParmPointer = new SqlParameter("@Pointer", SqlDbType.Binary, 16);
				ptrParmPointer.Value		= this.Pointer;

				// Step 2. Create and Init Offset
				SqlParameter ptrParmOffset = new SqlParameter("@Offset", SqlDbType.Int);
				ptrParmOffset.Value		= this.Position;

				// Step 3. Create and Init Count
				SqlParameter ptrParmCount = new SqlParameter("@Count", SqlDbType.Int);
				ptrParmCount.Value		= Math.Min(count,this.Length-this.Position);

				// Step 4. Create and Init Bytes
				SqlParameter ptrParmBytes = new SqlParameter("@Bytes", SqlDbType.Image);
				if(buffer.Length==count&&offset==0)
				{
					ptrParmBytes.Value		= buffer;
				}
				else
				{
					byte[] tmpBuffer	=	new byte[count];
					Buffer.BlockCopy(buffer,(int)offset,tmpBuffer,0,(int)count);
					ptrParmBytes.Value		= tmpBuffer;
				}

				SqlHelper.ExecuteNonQuery(context,CommandType.Text,GetUpdateTextCommand(),
					ptrParmPointer,ptrParmOffset,ptrParmCount,ptrParmBytes);

				this.Position	+=	count;
				if(this.Position>this.Length)
					this._lenght = this.Position;
			}
			finally
			{
				context.Transaction = null;
			}
		}

		/// <summary>
		/// Writes the array into the BLOB column.
		/// </summary>
		/// <param name="buffer">The array to which bytes are written. </param>
		/// <param name="offset">The byte offset in array at which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		protected virtual void UpdateTextCommandWithNullValue(byte[] buffer, long offset, long count)
		{
			SqlContext	context	=	new SqlContext(this.ConnectionString);
			try
			{
				context.Transaction = this.Transaction;

				// Step 4. Create and Init Bytes
				SqlParameter ptrParmBytes = new SqlParameter(string.Format("@{0}",this.ColumnName), SqlDbType.Image);
				if(buffer.Length==count&&offset==0)
				{
					ptrParmBytes.Value		= buffer;
				}
				else
				{
					byte[] tmpBuffer	=	new byte[count];
					Buffer.BlockCopy(buffer,(int)offset,tmpBuffer,0,(int)count);
					ptrParmBytes.Value		= tmpBuffer;
				}

				SqlHelper.ExecuteNonQuery(context,CommandType.Text,GetUpdateTextCommandWithNullValue(),
					this.PrimaryKeys.ToArray(ptrParmBytes));

				this.Position	+=	count;
				if(this.Position>this.Length)
					this._lenght = this.Position;
			}
			finally
			{
				context.Transaction = null;
			}
		}

		#endregion

		#region -- Read Command's methods
		/// <summary>
		/// Gets the sql command reading a blob element.
		/// </summary>
		/// <returns>The sql command.</returns>
		protected virtual string GetReadTextCommand()
		{
			StringBuilder	sb	=	new StringBuilder(255);
			sb.AppendFormat("READTEXT  [{1}].[{0}] @Pointer @Offset @Bytes",this.ColumnName,this.TableName);

			return sb.ToString();
		}

		/// <summary>
		/// Reads the array from the BLOB column.
		/// </summary>
		/// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The byte offset in array at which to begin reading.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <returns>The total number of bytes read into the buffer. This might be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached.</returns>
		protected virtual int ReadText(byte[] buffer, int offset, int count)
		{
			int retVal	=	0;

			SqlContext	context	=	new SqlContext(this.ConnectionString);
			try
			{
				context.Transaction = this.Transaction;

				// Step 1. Create and Init BLOB data Pointer
				SqlParameter ptrParmPointer = new SqlParameter("@Pointer", SqlDbType.Binary, 16);
				ptrParmPointer.Value		= this.Pointer;

				// Step 2. Create and Init Offset
				SqlParameter ptrParmOffset = new SqlParameter("@Offset", SqlDbType.Int);
				ptrParmOffset.Value		= this.Position;

				// Step 2. Create and Init Bytes
				SqlParameter ptrParmBytes = new SqlParameter("@Bytes", SqlDbType.Int);
				int bCount		= (int)(Math.Min(count,this.Length-this.Position));
				ptrParmBytes.Value	= bCount;

				if(bCount>0)
				{
					using(IDataReader reader = SqlHelper.ExecuteReader(context,CommandType.Text,GetReadTextCommand(),
							  ptrParmPointer,ptrParmOffset,ptrParmBytes))
					{
						if(reader.Read())
						{
							byte[] tmpBuffer	=	(byte[])reader[this.ColumnName];

							retVal	=	tmpBuffer.Length;
							Buffer.BlockCopy(tmpBuffer,0,buffer,offset,retVal);
						}
					}

					this.Position	+=	retVal;
				}
			}
			finally
			{
				context.Transaction = null;
			}

			return retVal;
		}

		#endregion
	}
}
