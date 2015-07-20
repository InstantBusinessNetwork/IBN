using System;
using System.Collections;

namespace Mediachase.MetaDataPlus.Import
{
	/// <summary>
	/// Summary description for FillResult.
	/// </summary>

	public struct MdpImportWarning
	{
		public int RowIndex;
		public string Message;

		public MdpImportWarning(int rowIndex, string message)
		{
			RowIndex = rowIndex;
			Message = message;
		}
	};

	public class FillResult
	{
		private int			_totalRows;
		private int			_successfulRows;
		private int			_errorRows;
		private ArrayList	_exceptionList;
		private ArrayList	_warningList;

		internal ArrayList warningList
		{
			get
			{
				if (_warningList == null)
					_warningList = new ArrayList();

				return _warningList;
			}
		}

		public int TotalRows
		{
			get
			{
				return _totalRows;
			}
		}

		public int SuccessfulRows
		{
			get
			{
				return _successfulRows;
			}
		}

		public int ErrorRows
		{
			get
			{
				return _errorRows;
			}
		}

		public string[] Errors
		{
			get
			{
				if (_exceptionList != null)
				{
					string[] errors = new string[_exceptionList.Count];
					for (int Index = 0 ; Index < _exceptionList.Count ; Index++)
					{
						errors[Index] = ((Exception)_exceptionList[Index]).ToString();
					}
					return errors;
				}
				else return null;
			}
		}

		public MdpImportWarning[] Warnings
		{
			get
			{
				if (_warningList != null) 
				{
					return (MdpImportWarning[])_warningList.ToArray(typeof(MdpImportWarning));
				}
				else return null;
			}
		}

		public Exception[] Exceptions
		{
			get
			{
				if (_exceptionList != null)
				{
					Exception[] errors = new Exception[_exceptionList.Count];
					for (int Index = 0 ; Index < _exceptionList.Count ; Index++)
					{
						errors[Index] = (Exception)_exceptionList[Index];
					}
					return errors;
				}
				else return null;
			}
		}

		internal void SuccessfulRow()
		{
			if (_totalRows > _successfulRows)
				_successfulRows++;
		}

		internal void ErrorRow()
		{
			_errorRows++;
		}

		internal void ErrorException(Exception ex)
		{
			if (_exceptionList==null)
				_exceptionList = new ArrayList();

			_exceptionList.Add(ex);
		}

		public FillResult(int totalRows)
		{
			_totalRows = totalRows;
		}
	}
}
