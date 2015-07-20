// ADOUtil.cpp: implementation of the CADOUtil class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ADOUtil.h"

//#pragma data_seg(".ADOUTIL")
//DWORD dwMSSQLProcessIDOwner = 0;
#pragma data_seg()

BOOL bADOUtilDatabaseIsLockad = FALSE;

CParam::CParam()
{
}

CParam::~CParam()
{
}

CParamArray::CParamArray()
{
	m_LastIndex = 0;
}

CParamArray::~CParamArray()
{
}

void CParamArray::RemoveAll()
{
	m_LastIndex = 0;
}
void CParamArray::Add(DataTypeEnum in_Type, ParameterDirectionEnum in_Direction, long in_Size, _variant_t Value)
{
	m_ParamArray[m_LastIndex].Type = in_Type;
	m_ParamArray[m_LastIndex].Direction = in_Direction;
	m_ParamArray[m_LastIndex].Size = in_Size;
	m_ParamArray[m_LastIndex].Value = Value;
	m_LastIndex ++;
}
void CParamArray::AddBSTR50(BSTR Value)
{
	Add(adVarWChar,adParamInput,50,_variant_t(Value));
}

void CParamArray::AddLong(long Value)
{
	Add(adInteger,adParamInput,4,_variant_t(Value));
}

void CParamArray::AddGUID(BSTR Value)
{
	Add(adWChar,adParamInput,36,_variant_t(Value));
}
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
TCHAR CADOUtil::szConnectionString[500];
TCHAR CADOUtil::szCompanyId[500];
BOOL CADOUtil::bGlobalCompany;

CADOUtil::CADOUtil()
{

}

CADOUtil::~CADOUtil()
{

}

long CADOUtil::RunCommand_ReturnLong(_bstr_t CmdText, CParamArray *ParamArray)
{
	if(!CheckCurrentProcess())
		throw ERR_NEW_PROCESS_DETECTED;

	DWORD			dRet = 0;
	_ConnectionPtr	pConn  = NULL;
	_CommandPtr		pComm = NULL;
	_ParameterPtr	pPrm  = NULL;
	ErrorPtr		pErr  = NULL;
	CParam*			pr    = NULL;
	_variant_t		vIndex;
	_variant_t		vValue;
	long			ErrSQLState = 0;
	long			ErrorCode = 0;
	_bstr_t			bsEpmty;

	long count;

	//********************************************************************
	//Connectin to Pool
	try
	{
		_bstr_t bstrConnString(szConnectionString);
		pConn.CreateInstance(__uuidof(Connection));
		if(pConn == NULL)
		{
			throw(ErrorCode = ERR_UNABLE_CREATE_CONN);
		}
		pConn->ConnectionTimeout = 7;
		pConn->Open(bstrConnString, L"", L"", adConnectUnspecified);
	}
	catch(...)
	{
		Beep(2000,100);
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL1,"Unable set connection to SQL Error = Unknown");
		ErrorCode = ERR_UNABLE_CREATE_CONN;
	}

	if(ErrorCode != 0)
		throw ErrorCode;

	//Execute Command
	try
	{
		pComm.CreateInstance(__uuidof(Command));
		if(pComm == NULL)
		{
			throw(ErrorCode = ERR_UNABLE_CREATE_COMM);
		}

		pComm->ActiveConnection = pConn;
		pComm->CommandText = CmdText;

		if (ParamArray != NULL)
		{
			for (int i=0; i < ParamArray->GetSize(); i++)
			{
				pr = (*ParamArray)[i];
				pPrm = pComm->CreateParameter(bsEpmty, pr->Type, pr->Direction, pr->Size, pr->Value);
				pComm->Parameters->Append(pPrm);
			}
		}

		pConn->Errors->Clear();

		_variant_t  vtEmpty (DISP_E_PARAMNOTFOUND, VT_ERROR);
		_variant_t  vtEmpty2(DISP_E_PARAMNOTFOUND, VT_ERROR);

		_RecordsetPtr reader = pComm->Execute(&vtEmpty, &vtEmpty2, adCmdText);

		int nFieldCount = reader->Fields->GetCount();

		if(nFieldCount > 0)
		{
			dRet = (DWORD) reader->Fields->GetItem(_variant_t((long)0))->Value;
		}
	}
	catch(_com_error)
	{
		try
		{
			count = pConn->Errors->Count;
			if(count)
			{
				vIndex = _variant_t((LONG)0);
				pErr = pConn->Errors->GetItem(vIndex);
				ErrSQLState = atol((LPCSTR)pErr->SQLState);
				/*
				//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,
				"SQL (LONG) Error %d [Error #%d Description \"%s\" (Source: %s)"
				"(SQL State: %s)](NativeError: %s)",
				e.Error(),
				pErr->Number,
				(LPCSTR)pErr->Description,
				(LPCSTR)pErr->Source,
				(LPCSTR)pErr->SQLState,
				(LPCSTR)pErr->NativeError);*/

			}
			pConn->Close();
			if (pComm->ActiveConnection != NULL)
			{
				pComm->PutRefActiveConnection(NULL);
			}
		}
		catch(...)
		{
			ErrSQLState = 1;
		}
	}
	catch(long errorCode)
	{
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,"SQL (LONG) Error Unable Create Command object");
		pConn->Close();
		throw(errorCode);
	}
	catch(...)
	{
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,"SQL (LONG) Error Unknown");
		ErrSQLState = 1;
	}

	if(ErrSQLState)
	{
		switch(ErrSQLState)
		{
		case 23000: //Violation of PRIMARY KEY constraint
			throw (ErrorCode = ERR_PRIMARY_KEY_CONSTRAINT);
			break;
		default: //Code 
			throw (ErrorCode = ERR_SQL_UNKNOWN_PROBLEM);
			break;
		}
	}

	//----------------
	//Close Connection
	pComm->PutRefActiveConnection(NULL);
	pConn->Close();
	return dRet;
}


//****************************************************************************************
//	Support SQL function
//	1. Set connection
//	2. Execute SP
//	3. Return RecordSet
//****************************************************************************************

long CADOUtil::RunSP_ReturnLong(_bstr_t SP_Name, CParamArray *ParamArray)
{
	if(!CheckCurrentProcess())
		throw ERR_NEW_PROCESS_DETECTED;

	DWORD			dRet = 0;
	_ConnectionPtr	pConn  = NULL;
	_CommandPtr		pComm = NULL;
	_ParameterPtr	pPrm  = NULL;
	ErrorPtr		pErr  = NULL;
	CParam*			pr    = NULL;
	_variant_t		vIndex;
	_variant_t		vValue;
	long			ErrSQLState = 0;
	long			ErrorCode = 0;
	_bstr_t			bsEpmty;

	long count;

	//********************************************************************
	//Connectin to Pool
	try 
	{
		_bstr_t bstrConnString(szConnectionString);
		pConn.CreateInstance(__uuidof(Connection));
		if(pConn == NULL)
		{
			throw(ErrorCode = ERR_UNABLE_CREATE_CONN);
		}
		pConn->ConnectionTimeout = 7;
		pConn->Open(bstrConnString, L"", L"", adConnectUnspecified);
	}
	catch(...)
	{
		Beep(2000, 100);
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL1,"Unable set connection to SQL Error = Unknown");
		ErrorCode = ERR_UNABLE_CREATE_CONN;
	}

	if(ErrorCode != 0)
		throw ErrorCode;

	//Execute Command
	try
	{
		pConn->CursorLocation = adUseClient;
		pComm.CreateInstance(__uuidof(Command));
		if(pComm == NULL)
		{
			throw(ErrorCode = ERR_UNABLE_CREATE_COMM);
		}

		pComm->ActiveConnection = pConn;
		pComm->CommandText = SP_Name;
		pComm->CommandType = adCmdStoredProc;

		if (ParamArray != NULL)
		{
			for (int i=0;i < ParamArray->GetSize();i++)
			{
				pr = (*ParamArray)[i];
				pPrm = pComm->CreateParameter(bsEpmty, pr->Type, pr->Direction, pr->Size, pr->Value);
				pComm->Parameters->Append(pPrm);
			}
		}

		pPrm = pComm->CreateParameter(_bstr_t(L"@RETVAL"), adInteger, adParamOutput, 4);
		pComm->Parameters->Append(pPrm);

		pConn->Errors->Clear();
		pComm->Execute(NULL, NULL, adExecuteNoRecords);

		pPrm=pComm->Parameters->Item[L"@RETVAL"];
		dRet = (DWORD)pPrm->Value.intVal;
	}
	catch(_com_error)
	{ 
		try
		{
			count = pConn->Errors->Count;
			if(count)
			{
				vIndex = _variant_t((LONG)0);
				pErr = pConn->Errors->GetItem(vIndex);
				ErrSQLState = atol((LPCSTR)pErr->SQLState);
				/*
				//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,
				"SQL (LONG) Error %d [Error #%d Description \"%s\" (Source: %s)"
				"(SQL State: %s)](NativeError: %s)",
				e.Error(),
				pErr->Number,
				(LPCSTR)pErr->Description,
				(LPCSTR)pErr->Source,
				(LPCSTR)pErr->SQLState,
				(LPCSTR)pErr->NativeError);*/

			}
			pConn->Close();
			if (pComm->ActiveConnection != NULL)
			{
				pComm->PutRefActiveConnection(NULL);
			}
		}
		catch(...)
		{
			ErrSQLState = 1;
		}
	}
	catch(long errorCode)
	{
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,"SQL (LONG) Error Unable Create Command object");
		pConn->Close();
		throw(errorCode);
	}
	catch(...)
	{
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,"SQL (LONG) Error Unknown");
		ErrSQLState = 1;
	}

	if(ErrSQLState != 0)
	{
		switch(ErrSQLState)
		{
		case 23000: //Violation of PRIMARY KEY constraint
			throw (ErrorCode = ERR_PRIMARY_KEY_CONSTRAINT);
			break;
		default: //Code 
			throw (ErrorCode = ERR_SQL_UNKNOWN_PROBLEM);
			break;
		}
	}

	//----------------
	//Close Connection
	pComm->PutRefActiveConnection(NULL);
	pConn->Close();
	return dRet;
}
//****************************************************************************************
//	Support SQL function
//	1. Set connection
//	2. Execute SP
//	3. Return RecordSet
//****************************************************************************************

void CADOUtil::RunSP_ReturnRS(_bstr_t SP_Name, _RecordsetPtr& out_pRs, CParamArray *ParamArray)
{
	if(!CheckCurrentProcess())
		throw ERR_NEW_PROCESS_DETECTED;

	_ConnectionPtr	pConn	= NULL;
	_ParameterPtr	pPrm	= NULL;
	_ParameterPtr	pprm	= NULL;
	ErrorPtr		pErr	= NULL;
	_CommandPtr		pComm	= NULL;
	CParam*			pr		= NULL;
	LONG			ErrSQLState = 0;
	LONG			count = 0;
	LONG			ErrorCode = 0;
	_variant_t		vIndex;
	_bstr_t			bsEmpty;

	//********************************************************************
	//Connectin to Pool
	try 
	{
		_bstr_t bstrConnString(szConnectionString);
		pConn.CreateInstance(__uuidof(Connection));
		if(pConn == NULL)
		{
			throw(ErrorCode = ERR_UNABLE_CREATE_CONN);
		}
		pConn->ConnectionTimeout = 7;
		pConn->Open(bstrConnString, L"", L"", adConnectUnspecified);
	}
	catch(...)
	{
		Beep(2000, 100);
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL1,"Unable set connection to SQL Error = Unknown");
		ErrorCode = ERR_UNABLE_CREATE_CONN;
	}

	if(ErrorCode)
		throw ErrorCode;

	//*******************************************************************	
	//Execut command
	try
	{
		pComm.CreateInstance(__uuidof(Command));
		if(pComm == NULL)
		{
			throw(ErrorCode = ERR_UNABLE_CREATE_COMM);
		}
		out_pRs.CreateInstance(__uuidof(Recordset));
		if(out_pRs == NULL)
		{
			throw(ErrorCode = ERR_UNABLE_CREATE_RECSET);
		}
		pComm->ActiveConnection = pConn;
		pComm->CommandText = SP_Name;
		pComm->CommandType = adCmdStoredProc;

		if(ParamArray != NULL)
		{
			DWORD Count = ParamArray->GetSize();
			for (DWORD i=0; i < Count; i++)
			{
				pr = (*ParamArray)[i];
				pprm = pComm->CreateParameter(bsEmpty, pr->Type, pr->Direction, pr->Size, pr->Value);
				pComm->Parameters->Append(pprm);
			}
		}
		out_pRs->CursorLocation = adUseClient;
		out_pRs->Open(_variant_t((IDispatch *)pComm, true), vtMissing, adOpenForwardOnly, adLockReadOnly, NULL);
		out_pRs->PutRefActiveConnection(NULL);
		pConn->Close();
		return;
	}

	//Handle Execut error
	catch(_com_error)
	{
		try
		{
			count = pConn->Errors->Count;
			if(count)
			{
				vIndex = _variant_t((LONG)0);
				pErr = pConn->Errors->GetItem(vIndex);
				ErrSQLState = atol((LPCSTR)pErr->SQLState);
				/*
				//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,
				"SQL (RS) Error %d [Error #%d Description \"%s\" (Source: %s)"
				"(SQL State: %s)](NativeError: %s)",
				e.Error(),
				pErr->Number,
				(LPCSTR)pErr->Description,
				(LPCSTR)pErr->Source,
				(LPCSTR)pErr->SQLState,
				(LPCSTR)pErr->NativeError);*/

			}
			pConn->Close();
			if (pComm->ActiveConnection != NULL)
			{
				pComm->PutRefActiveConnection(NULL);
			}
		}
		catch(...)
		{
			ErrSQLState = 1;
		}
	}
	catch(long errorCode)
	{
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,"SQL (RS) Error Unable Create Command object");
		pConn->Close();
		throw(errorCode);
	}
	catch(...)
	{
		//m_ExternalLink.Add2Log(WL_ERROR_LEVEL3,"SQL (RS) Error Unknown");
		ErrSQLState = 1;
	}

	if(ErrSQLState)
	{
		switch(ErrSQLState)
		{
		case 23000: //Violation of PRIMARY KEY constraint
			throw (ErrorCode = ERR_PRIMARY_KEY_CONSTRAINT);
			break;
		default: //Code
			throw (ErrorCode = ERR_SQL_UNKNOWN_PROBLEM);
			break;
		}
	}
}

BOOL CADOUtil::CheckRecordSetState(_RecordsetPtr RecSet)
{
	if (RecSet != NULL && (RecSet->EndOfFile == VARIANT_FALSE))
	{	
		if (RecSet->EndOfFile == VARIANT_FALSE)
		{
			RecSet->MoveFirst();
			return TRUE;
		}
	}
	return FALSE;
}

void CADOUtil::LockDatabase()
{
	// TODO: Save dwMSSQLProcessIDOwner From Company Database
	//dwMSSQLProcessIDOwner = GetCurrentProcessId();
	bADOUtilDatabaseIsLockad = TRUE;
}

BOOL CADOUtil::CheckCurrentProcess()
{
	// TODO: Read dwMSSQLProcessIDOwner From Company Database
	return !bADOUtilDatabaseIsLockad;//dwMSSQLProcessIDOwner == GetCurrentProcessId();
}