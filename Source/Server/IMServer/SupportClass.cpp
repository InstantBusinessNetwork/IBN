// SupportClass.cpp: implementation of the SupportClass class.
//
//////////////////////////////////////////////////////////////////////


#include "stdafx.h"
#include "ibn_server.h"
#include "SupportClass.h"
#include "RS2XML_DEFS.h"
#include "ISAPIRequest.h"
#include "ADOUtil.h"
#include "Counter.h"
//#include "_IbnStorageSvc.h"
//#include "_IbnStorageSvc_i.c"

#include "EventLog.h"
#include "ExternalDeclarations.h"

#include "PasswordUtil.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#define CHECK_HR_THROW(hr, err)\
	if(hr != S_OK) throw(err);

void FileTimeToTimet( time_t* t, LPFILETIME pft )
{
	LONGLONG ll = 0;
	ll = (LONGLONG)pft->dwLowDateTime | ((LONGLONG)pft->dwHighDateTime << 32);

	ll = ll - 116444736000000000;
	*t = (LONG)(ll / 10000000);
}

long GetGMTtime_t()
{
	SYSTEMTIME sTime;
	FILETIME   fTime;
	time_t     time;

	GetSystemTime(&sTime);
	SystemTimeToFileTime(&sTime, &fTime);
	FileTimeToTimet(&time, &fTime);

	return (long)time;
}

#define CDRW_LOG_MESSAGE 1
#define WM_SEND_LOG WM_USER + 200

#define stAWAY	4
#define stNA	5
#define stWEB	10
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
//_bstr_t CSupportClass::m_bs_OM_AUTHENTICATION;
_bstr_t CSupportClass::m_bs_OM_GET_ID_AND_SALT_BY_LOGIN;
_bstr_t CSupportClass::m_bs_OM_ADD_ACTIVE_USER;
_bstr_t CSupportClass::m_bs_OM_GET_USER_INFO;
_bstr_t CSupportClass::m_bs_OM_CHECK_AUDIT;
_bstr_t CSupportClass::m_bs_OM_LOGOFF;
_bstr_t CSupportClass::m_bs_OM_CHANGE_STATUS;
_bstr_t CSupportClass::m_bs_OM_SEND_MESSAGE;
_bstr_t CSupportClass::m_bs_OM_ADD_AUTH_REPLY;
_bstr_t CSupportClass::m_bs_OM_LOAD_LIST;
_bstr_t CSupportClass::m_bs_OM_LOAD_AUTH_LIST;
_bstr_t CSupportClass::m_bs_OM_LOAD_FILES;
_bstr_t CSupportClass::m_bs_OM_LOAD_SIDS;
_bstr_t CSupportClass::m_bs_OM_SYNC_HISTORY;
_bstr_t CSupportClass::m_bs_OM_ADD_AUTH_REQUEST;
_bstr_t CSupportClass::m_bs_OM_ADD_USER;
_bstr_t CSupportClass::m_bs_OM_CONFIRM_MESSAGE;
_bstr_t CSupportClass::m_bs_OM_CONFIRM_FILE;
_bstr_t CSupportClass::m_bs_OM_DELETE_AUTH_REQUEST;
_bstr_t CSupportClass::m_bs_OM_LOAD_LIST_REV;
_bstr_t CSupportClass::m_bs_OM_CHECK_IGNORE;
_bstr_t CSupportClass::m_bs_OM_ADD_FILE;
_bstr_t CSupportClass::m_bs_OM_DELETE_USER;
_bstr_t CSupportClass::m_bs_OM_LOAD_ACTIVE_USERS_BY_ROLE;
_bstr_t CSupportClass::m_bs_OM_LOAD_MESS;
_bstr_t CSupportClass::m_bs_OM_LOAD_AUTH_LIST_REV;


CComBSTR CSupportClass::m_bs_event;
CComBSTR CSupportClass::m_bs_users;
CComBSTR CSupportClass::m_bs_chats;
CComBSTR CSupportClass::m_bs_messages;
CComBSTR CSupportClass::m_bs_files;
CComBSTR CSupportClass::m_bs_sessions;

LONG   CSupportClass::OpenThread = 0L;
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CSupportClass::CSupportClass():m_bIsInit(FALSE)
{
	LogThreadID = 0;
	OpenThread   = 0;
}

CSupportClass::~CSupportClass()
{
	//if(szLocalStoragePath)		{delete[] szLocalStoragePath; szLocalStoragePath = NULL;}
	Terminate();
}


BOOL CSupportClass::Logic()
{
	GET_ISAPI_REQUEST();

	if (m_bs_c_message			== pRequest->m_bsCommand)	{return z_NewMessage();}

	if (m_bs_c_send_file		== pRequest->m_bsCommand)	{return z_SentFile();}

	if (m_bs_c_delete_user		== pRequest->m_bsCommand)	{return z_DeleteUser();}

	if (m_bs_c_del_user_r		== pRequest->m_bsCommand)	{return z_DelAuthReq();}

	if (m_bs_c_load_list		== pRequest->m_bsCommand)	{return z_LoadList();}    

	if (m_bs_c_confirm_message	== pRequest->m_bsCommand) 	{return z_ConfirmMessage();}

	if (m_bs_c_confirm_promo	== pRequest->m_bsCommand) 	{return z_ConfirmPromo();}

	if (m_bs_c_confirm_file		== pRequest->m_bsCommand) 	{return z_ConfirmFile();}

	if (m_bs_c_change_status	== pRequest->m_bsCommand) 	{return z_ChangeStatus();}

	if (m_bs_c_details			== pRequest->m_bsCommand) 	{return z_GetUserInfo();}

	if (m_bs_c_logoff			== pRequest->m_bsCommand) 	{return z_LogOff();}

	if (m_bs_c_add_user			== pRequest->m_bsCommand) 	{return z_AddUser();}

	if (m_bs_c_add_user_r		== pRequest->m_bsCommand) 	{return z_AddAuthRely();}

	if (m_bs_c_ch_create		== pRequest->m_bsCommand) 	{return z_ch_create();}
	if (m_bs_c_ch_edit			== pRequest->m_bsCommand) 	{return z_ch_edit();}
	if (m_bs_c_ch_leave			== pRequest->m_bsCommand) 	{return z_ch_leave();}
	if (m_bs_c_ch_invite		== pRequest->m_bsCommand) 	{return z_ch_invite();}
	if (m_bs_c_ch_accept		== pRequest->m_bsCommand) 	{return z_ch_accept();}
	if (m_bs_c_ch_status		== pRequest->m_bsCommand) 	{return z_ch_status();}
	if (m_bs_c_ch_message		== pRequest->m_bsCommand) 	{return z_ch_message();}


	return pRequest->SetError(ERR_WRONG_XML);
}

void CSupportClass::z_logon(CISAPIRequest* pRequest)
{
	CParamArray m_ParamArray;
	_RecordsetPtr RecSet;
	_RecordsetPtr userIdSaltRecSet;

	CComBSTR bsLogin;
	CComBSTR bsPassword;

	CComBSTR bsDomain;
	CComBSTR bsName;

	long Status = 0;
	long tempStatus = 0;

	HRESULT hr;
	LONG ltime = GetGMTtime_t(); 
	ATLTRACE("logon test\r\n");

	try
	{
		//UNPACK XML USER
		hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode, CComBSTR(L"login"), &bsLogin);
		CHECK_HR_THROW(hr, ERR_WRONG_XML);

		hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode, CComBSTR(L"password"), &bsPassword);
		CHECK_HR_THROW(hr, ERR_WRONG_XML);

		hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode, CComBSTR(L"status"), &Status);
		CHECK_HR_THROW(hr, ERR_WRONG_XML);

#define _ASP_MODEL 
#ifdef _ASP_MODEL

		wchar_t* m_char = wcschr((BSTR)bsLogin, L'@');
		if(m_char == NULL)
			CHECK_HR_THROW(hr,ERR_WRONG_XML);

		bsName = CComBSTR(((int)(m_char - bsLogin)), bsLogin);
		bsDomain = CComBSTR(m_char+1);

		/*   [dbo].OM_AUTHENTICATION 
		@LOGIN NVARCHAR(50),
		@PASSWORD  NVARCHAR(50),
		@DOMAIN NVARCHAR(255),
		@RETVAL INT OUTPUT
		*/
		m_ParamArray.AddBSTR50(bsName);
		//m_ParamArray.AddBSTR50(bsPassword);
		m_ParamArray.Add(adVarWChar, adParamInput, 255, _variant_t(bsDomain));
#else
		/*


		*/
		/********************************************	
		/*Verifing UserName and Password

		OM_AUTHENTICATION 
		@LOGIN VARCHAR(50),
		@PASSWORD  VARCHAR(50),
		@RETVAL INT OUTPUT*/
		m_ParamArray.AddBSTR50(bsLogin);
		m_ParamArray.AddBSTR50(bsPassword);
#endif
		//pRequest->m_nUserID = CADOUtil::RunSP_ReturnLong(m_bs_OM_AUTHENTICATION, &m_ParamArray);
		//if (pRequest->m_nUserID == -1)
		//	throw(ERR_WRONG_PASSWORD);

		CADOUtil::RunSP_ReturnRS(m_bs_OM_GET_ID_AND_SALT_BY_LOGIN, userIdSaltRecSet, &m_ParamArray);

		if(CADOUtil::CheckRecordSetState(userIdSaltRecSet))
		{
			CComBSTR salt = L"", hash = L"", pass = L"";

			try
			{
				pRequest->m_nUserID = userIdSaltRecSet->Fields->GetItem(_bstr_t(L"user_id"))->Value.lVal;

				_variant_t varSalt = userIdSaltRecSet->Fields->GetItem(_bstr_t(L"salt"))->Value;
				_variant_t varHash = userIdSaltRecSet->Fields->GetItem(_bstr_t(L"hash"))->Value;
				_variant_t varPass = userIdSaltRecSet->Fields->GetItem(_bstr_t(L"password"))->Value;

				if(varSalt.vt == VT_BSTR)
					salt = varSalt.bstrVal;
				if(varHash.vt == VT_BSTR)
					hash = varHash.bstrVal;
				if(varPass.vt == VT_BSTR)
					pass = varPass.bstrVal;
			}
			catch(...)
			{
			}

			userIdSaltRecSet->Close();
			userIdSaltRecSet = NULL;

			if(hash == L"")
			{
				if(pass!=bsPassword)
					throw(ERR_WRONG_PASSWORD);
			}
			else
			{
				BOOL bResult = FALSE;
				HRESULT hrPassCheck = PasswordUtil_Check(bsPassword, salt, hash, &bResult);

				if(FAILED(hrPassCheck))
				{
					CString auditMessage;
					auditMessage.Format(_T("Hash Check Error, Code: 0x%X."), hrPassCheck);
					CEventLog::AddAppLog(auditMessage, FAILED_LOGIN, EVENTLOG_WARNING_TYPE);
				}

				if(!bResult)
					throw(ERR_WRONG_PASSWORD);
			}
			
		}
		else
			throw ERR_WRONG_PASSWORD;

		// Check licences
		g_ActiveSessions.CheckLicense();

		int maxRetryCounter = 1;
retry:
		if((maxRetryCounter--) == 0)
			throw ERR_OUT_GLOBAL_6;

		/********************************************
		/*Add User

		OM_ADD_ACTIVE_USER 
		@USER_ID INT,
		@SID CHAR(36),
		@STATUS TINYINT,
		@RETVAL INT OUTPUT*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddGUID(CComBSTR(pRequest->m_szSessionID));
		m_ParamArray.AddLong(Status);
		m_ParamArray.AddLong(ltime);

		try
		{
			CADOUtil::RunSP_ReturnLong(m_bs_OM_ADD_ACTIVE_USER, &m_ParamArray);
			//Add Active User in RAM DB
			g_ActiveSessions.AddAcitveSession(pRequest->m_nUserID, pRequest->m_szSessionID);
#ifdef _IBN_PERFORMANCE_MONITOR
			if (CCounter::GetHasRealTimeMonitoring())
				::InterlockedIncrement((LONG*)CCounter::m_ulActiveUser);//CCounter::AddActiveUser();
#endif
		}
		catch(long ErrorCode)
		{
			switch(ErrorCode)
			{
			case ERR_PRIMARY_KEY_CONSTRAINT:
				m_ParamArray.RemoveAll();
				m_ParamArray.AddLong(pRequest->m_nUserID);
				m_ParamArray.AddLong(1);
				m_ParamArray.AddLong(GetGMTtime_t());
				try
				{
					CADOUtil::RunSP_ReturnRS(m_bs_OM_GET_USER_INFO, RecSet, &m_ParamArray);
				}
				catch(...)
				{}

				if(CADOUtil::CheckRecordSetState(RecSet))
				{
					try
					{
						tempStatus = RecSet->Fields->GetItem(_bstr_t(L"status"))->Value.lVal;
					}
					catch(...)
					{}

					if(tempStatus == stAWAY || tempStatus == stNA || tempStatus == stWEB)
					{
						//TODO: Send Message
						//SendSystemMessages(ID,_bstr_t(""));

						try
						{
							m_ParamArray.RemoveAll();
							m_ParamArray.AddLong(pRequest->m_nUserID);
							m_ParamArray.AddLong(ltime);
							CADOUtil::RunSP_ReturnLong(m_bs_OM_LOGOFF, &m_ParamArray);

							g_ActiveSessions.DeleteActiveSession(pRequest->m_nUserID);
#ifdef _IBN_PERFORMANCE_MONITOR
							if (CCounter::GetHasRealTimeMonitoring())
								::InterlockedDecrement((LONG*)CCounter::m_ulActiveUser);
#endif 
						}
						catch(...)
						{
						}

						ErrorCode = 0;
						RecSet->Close();
						RecSet = NULL;
						maxRetryCounter++;
						goto retry;
					}

					RecSet->Close();
					RecSet = NULL;
					throw ERR_OUT_ALREADY_IN;
				}
				break;

			case ERR_WRONG_ID:
			case ERR_WRONG_SID:
				m_ParamArray.RemoveAll();
				m_ParamArray.AddLong(pRequest->m_nUserID);
				m_ParamArray.AddLong(ltime);
				try
				{
					CADOUtil::RunSP_ReturnLong(m_bs_OM_LOGOFF, &m_ParamArray);

					g_ActiveSessions.DeleteActiveSession(pRequest->m_nUserID);
#ifdef _IBN_PERFORMANCE_MONITOR
					if (CCounter::GetHasRealTimeMonitoring())
						::InterlockedDecrement((LONG*)CCounter::m_ulActiveUser);
#endif

				}
				catch(...)
				{}

				goto retry;
				break;
			default:
				throw ErrorCode;
				break;
			}
		}

		//////////////////////////////////////////////
		//Get Off line events
		SendOffLineEvents();

		//////////////////////////////////////////////
		//Send Self Status;
		y_EventChangedState(Status);

		//////////////////////////////////////////////

		/*	OM_GET_USER_INFO 
		@USER_ID INT,
		@INFO_TYPE INT*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(1);
		m_ParamArray.AddLong(GetGMTtime_t());

		hr = CreateResponsePacket();
		CHECK_HR_THROW(hr, ERR_UNNKOWN_XML);

		CADOUtil::RunSP_ReturnRS(m_bs_OM_GET_USER_INFO, RecSet, &m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{	
			CRS2XML::Parse(RecSet, pRequest->m_WorkNode, g_r2x_userBasicInfo);
			RecSet->Close();
			RecSet = NULL;
		}
		else
			throw ERR_SQL_UNKNOWN_PROBLEM;

		return;
	}
	catch(long ErrorCode)
	{
		if(ErrorCode==ERR_WRONG_PASSWORD)
		{
			if(CheckAuditIsActive(bsDomain))
			{
				CHAR remodeAddr[2048] = "";
				DWORD remodeAddrLen = 2047;
				pRequest->m_pECB->GetServerVariable(pRequest->m_pECB->ConnID, "REMOTE_ADDR", remodeAddr, &remodeAddrLen);

				CString strLogin = bsLogin;
				CString strRemodeAddr = remodeAddr;

				CString auditMessage;
				auditMessage.Format(_T("Login: %s. IP: %s"), (LPCTSTR)strLogin, (LPCTSTR)strRemodeAddr);

				CEventLog::AddAppLog(auditMessage, FAILED_LOGIN, EVENTLOG_WARNING_TYPE);
			}
		}

		throw ErrorCode;
	}
	catch(...)
	{}
	throw ERR_UNKNOW;
}

BOOL CSupportClass::CheckAuditIsActive(BSTR bsDomain)
{
	if(bsDomain==NULL)
		return TRUE;

	CParamArray m_ParamArray;
	m_ParamArray.Add(adVarWChar, adParamInput, 255, _variant_t(bsDomain));

	try
	{
		return 1 == CADOUtil::RunSP_ReturnLong(m_bs_OM_CHECK_AUDIT, &m_ParamArray);
	}
	catch(...)
	{
	}

	return FALSE;
}

BOOL CSupportClass::z_ChangeStatus()
{
	GET_ISAPI_REQUEST();
	CParamArray m_ParamArray;
	_RecordsetPtr RecSet = NULL;
	long Status = NULL;
	HRESULT hr;

	//UNPACK XML USER

	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode, CComBSTR(L"status"), &Status);
	CHECK_HR_THROW(hr, ERR_WRONG_XML);

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();

	/*OM_CHANGE_STATUS 
	@USER_ID INT,
	@STATUS INT,
	@RETVAL INT OUTPUT*/

	m_ParamArray.AddLong(pRequest->m_nUserID);
	m_ParamArray.AddLong(Status);
	if(1 == CADOUtil::RunSP_ReturnLong(m_bs_OM_CHANGE_STATUS, &m_ParamArray))
	{
		y_EventChangedState(Status);

		return TRUE;
	}
	return FALSE;
}

BOOL CSupportClass::z_NewMessage()
{
	GET_ISAPI_REQUEST();
	HRESULT						hr = S_OK;
	CComBSTR					bsMID,bsBody,bsCID;
	CComPtr<IXMLDOMNode>		pNode,pEventNode,pTemp;
	LONG						nToID;
	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	CHGLOBAL					hGlobal;
	DWORD						dwSize  = 0;
	PBYTE						pBuffer = NULL;
	BOOL						m_bCHAT = FALSE;

	LONG ltime = GetGMTtime_t(); 
	LONG DB_time = 0;

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"message/@mid"),&bsMID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"message/body"),&bsBody);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = pRequest->m_WorkNode->selectSingleNode(CComBSTR(L"message/recipients/user"),&pNode);
	if(pNode == NULL)
	{
		hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"message/chat/@cid"),&bsCID);
		if(hr != S_OK)
			CHECK_HR_THROW(hr,ERR_WRONG_XML);

		m_bCHAT = TRUE;
	}

	if(!m_bCHAT)
	{
		while(pNode != NULL)
		{
			hr = CXMLUtil::GetValueByPath(pNode,CComBSTR(L"@id"),&nToID);
			if(hr != S_OK || nToID == 0)
			{
				hr = pNode->get_nextSibling(&pTemp);
				if(hr != S_OK) break;
				pNode.Release();
				pNode = pTemp;
				pTemp.Release();
				continue;
			}



			/*OM_SEND_MESSAGE
			@FROM_USER_ID INT,
			@TO_USER_ID INT,
			@MESS_ID CHAR(36),
			@MESS_TEXT NTEXT(16),
			@SEND_TIME INT,
			@USER_SID CHAR(36),*/
			m_ParamArray.RemoveAll();
			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddLong(nToID);
			m_ParamArray.AddGUID((BSTR)bsMID);
			m_ParamArray.Add(adLongVarWChar,adParamInput,bsBody.Length(),_variant_t(bsBody));
			m_ParamArray.AddLong(ltime);
			m_ParamArray.AddGUID(CComBSTR(pRequest->m_szSessionID));

			try
			{
				CADOUtil::RunSP_ReturnRS(m_bs_OM_SEND_MESSAGE,RecSet,&m_ParamArray);
			}
			catch(LONG err)
			{
				if(err == ERR_UNABLE_CREATE_CONN)
					throw err;
				RecSet = NULL;
			}
			catch(...)
			{
				RecSet = NULL;
			};


			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				if(pEventNode == NULL)
				{
					FieldPtr field = RecSet->Fields->GetItem(_bstr_t(L"send_time"));
					DB_time = field->Value.lVal;

					CreateEventPacket(&pEventNode);
					CRS2XML::ParseMulti(RecSet,pEventNode , g_r2x_message,CComBSTR(L"event"));
					hr = CXMLUtil::XML2HGlobal(pEventNode,&hGlobal,&dwSize);
					CHECK_HR_THROW(hr,ERR_UNKNOW);
					pBuffer = (PBYTE)hGlobal.Lock();
				}

				try
				{
					if(DB_time == ltime)
						g_ActiveSessions.SendEvent(nToID,pBuffer,dwSize-2);
				}
				catch(...){}

				RecSet->Close();
				RecSet = NULL;
			}

			hr = pNode->get_nextSibling(&pTemp);
			if(hr != S_OK) break;
			pNode.Release();
			pNode = pTemp;
			pTemp.Release();
		} 

		CreateResponsePacket();
		TCHAR Value[30];
		_ltot_s(DB_time, Value, 30, 10);
		hr = CXMLUtil::AppendNode(pRequest->m_WorkNode, CComBSTR(L"time"), NULL, NULL, CComBSTR(Value));
		CHECK_HR_THROW(hr, ERR_UNKNOW);
	}
	else //chat message
	{
		/*IBN_SEND_CHAT_MESS
		@CUID as char(36) ,
		@MID as char(36) ,
		@USER_ID as int ,
		@MESS_TEXT as NTEXT(16) ,
		@SEND_TIME as int */

		m_ParamArray.RemoveAll();
		m_ParamArray.AddGUID((BSTR)bsCID);
		m_ParamArray.AddGUID((BSTR)bsMID);
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.Add(adLongVarWChar,adParamInput,bsBody.Length(),_variant_t(bsBody));
		m_ParamArray.AddLong(ltime);

		CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_SEND_CHAT_MESS"),RecSet,&m_ParamArray);

		DB_time = RecSet->Fields->GetItem(_bstr_t(L"SEND_TIME"))->Value.lVal;
		if(ltime == DB_time)
		{	
			//g_r2x_chat_message
			CreateEventPacket(&pEventNode);
			CRS2XML::ParseMulti(RecSet, pEventNode, g_r2x_chat_message,m_bs_event);

			hr = CXMLUtil::XML2HGlobal(pEventNode,&hGlobal,&dwSize);
			CHECK_HR_THROW(hr,ERR_UNKNOW);

			SendChatBroadcast(bsCID,(PBYTE)hGlobal.Lock(),dwSize);

			hGlobal.UnLock();
			//GlobalFree(hGlobal);
			pEventNode.Release();
		}
		RecSet->Close();
		RecSet = NULL;
		pRequest->m_pXMLDoc.Release();
		pRequest->m_WorkNode.Release();
	}
	return TRUE;
}



BOOL CSupportClass::z_AddAuthRely()
{
	GET_ISAPI_REQUEST();
	CParamArray					m_ParamArray;
	_RecordsetPtr				RecSet = NULL;
	CComBSTR					bsResult;
	LONG						AddID,Result = 0;
	HRESULT						hr = S_OK;
	HGLOBAL						hGlobal = NULL;
	PBYTE						pBuffer = NULL;
	DWORD						dwSize = 0;

	//UNPACK XML USER
	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"user/@id"),&AddID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"result"),&bsResult);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	if(CComBSTR(L"accept") == bsResult) Result = 1;
	else
		if(CComBSTR(L"deny")   == bsResult) Result = 2;

	if(Result == 0) throw(ERR_WRONG_XML);

	/*OM_ADD_AUTH_REPLY
	@FROM_USER_ID INT,
	@TO_USER_ID INT,
	@FLAG INT,
	@RETVAL INT OUTPUT*/
	m_ParamArray.AddLong(pRequest->m_nUserID);
	m_ParamArray.AddLong(AddID);
	m_ParamArray.AddLong(Result);

	CADOUtil::RunSP_ReturnRS(m_bs_OM_ADD_AUTH_REPLY,RecSet,&m_ParamArray);
	if(CADOUtil::CheckRecordSetState(RecSet))
	{
		CComPtr<IXMLDOMNode> pNode;
		hr = CreateEventPacket(&pNode);
		CHECK_HR_THROW(hr,ERR_UNKNOW);

		CRS2XML::ParseMulti(RecSet, pNode, g_r2x_adduserr,m_bs_event);
		hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
		CHECK_HR_THROW(hr,ERR_UNKNOW);
		pBuffer = (PBYTE)GlobalLock(hGlobal);

		try
		{
			g_ActiveSessions.SendEvent(AddID,pBuffer,dwSize-2);
		}
		catch(...){}

		if(pBuffer != NULL)
		{
			GlobalUnlock(hGlobal);
			GlobalFree(hGlobal);
		}

		RecSet->Close();
		RecSet = NULL;
	}	

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();

	return TRUE;
}

//////////////////////////////////////////////////////////////////////
// Load lista [contact, ignore, Offline Files, Sessions, Messages]

BOOL CSupportClass::z_LoadList()
{
	GET_ISAPI_REQUEST();
	HRESULT hr = S_OK;
	CComBSTR			  bsListType;
	CParamArray			  m_ParamArray;
	_RecordsetPtr		  RecSet	= NULL;

	try
	{
		hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"list/@type"),&bsListType);
		CHECK_HR_RETURN(hr,ERR_WRONG_XML);

		///////////////////////////////////////
		//Contact List
		if(bsListType == CComBSTR(L"contact"))
		{
			CreateResponsePacket();

			/*OM_LOAD_LIST 
			@USER_ID INT,
			@LIST_TYPE TINYINT*/
			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddLong(1);
			m_ParamArray.AddLong(GetGMTtime_t());

			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_LIST,RecSet,&m_ParamArray);

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_userBasicInfo,m_bs_users);
				RecSet->Close();
				RecSet = NULL;
			}
			else
			{
				CRS2XML::ParseList(NULL, pRequest->m_WorkNode, g_r2x_userBasicInfo,m_bs_users);
			}

			/*OM_LOAD_AUTH_LIST
			@USER_ID INT,
			@STATUS INT*/
			m_ParamArray.RemoveAll();
			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddLong(0);
			m_ParamArray.AddLong(GetGMTtime_t());

			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_AUTH_LIST,RecSet,&m_ParamArray);

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_userBasicInfo,m_bs_users);
				RecSet->Close();
				RecSet = NULL;
			}
		}

		///////////////////////////////////////
		//Ignore List
		if(bsListType == CComBSTR(L"chats"))
		{
			CreateResponsePacket();

			/*IBN_LOAD_CHATS
			@USER_ID as int
			SELECT CUID, [NAME], [DESC], OWNER_ID, BEGIN_TIME*/
			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddLong(0);

			CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LOAD_CHATS"),RecSet,&m_ParamArray);

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_chat_full,m_bs_chats);
				RecSet->Close();
				RecSet = NULL;
			}
			else
			{
				CRS2XML::ParseList(NULL, pRequest->m_WorkNode, g_r2x_chat_full,m_bs_chats);
			}
		}

		///////////////////////////////////////
		//Ignore List
		if(bsListType == CComBSTR(L"ignore"))
		{
			CreateResponsePacket();

			/*PROCEDURE OM_LOAD_LIST 
			@USER_ID INT,
			@LIST_TYPE TINYINT*/
			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddLong(1);
			m_ParamArray.AddLong(GetGMTtime_t());

			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_LIST,RecSet,&m_ParamArray);

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_userBasicInfo,m_bs_users);
				RecSet->Close();
				RecSet = NULL;
			}
			else
			{
				CRS2XML::ParseList(NULL, pRequest->m_WorkNode, g_r2x_userBasicInfo,m_bs_users);
			}
		}

		/////////////////////////////////////
		//OffLine Files List
		if(bsListType == CComBSTR(L"files"))
		{
			CreateResponsePacket();

			/*OM_LOAD_FILES 
			@TO_USER_ID INT,
			@FILE_STATUS INT*/
			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddLong((long)3);

			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_FILES,RecSet,&m_ParamArray);

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_file,m_bs_files);
				RecSet->Close();
				RecSet = NULL;
			}
			else
			{
				CRS2XML::ParseList(NULL, pRequest->m_WorkNode, g_r2x_file,m_bs_files);
			}
		}

		/////////////////////////////////////
		//Sessions List
		if(bsListType == CComBSTR(L"sessions"))
		{
			long From, To;

			hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"from"),&From);
			CHECK_HR_RETURN(hr,ERR_WRONG_XML);
			hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"to"),&To);
			CHECK_HR_RETURN(hr,ERR_WRONG_XML);

			CreateResponsePacket();

			/*OM_LOAD_SIDS 
			@USER_ID INT,
			@FROM INT,
			@TO INT*/
			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddLong(From);
			m_ParamArray.AddLong(To);

			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_SIDS,RecSet,&m_ParamArray);

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_session,m_bs_sessions);
				RecSet->Close();
				RecSet = NULL;
			}
			else
			{
				CRS2XML::ParseList(NULL, pRequest->m_WorkNode, g_r2x_session,m_bs_sessions);
			}
		}

		/////////////////////////////////////
		//Messages List
		if(bsListType == CComBSTR(L"messages"))
		{
			CComBSTR bsSid;
			hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"sid"),&bsSid);
			CHECK_HR_RETURN(hr,ERR_WRONG_XML);

			CreateResponsePacket();
			/*OM_SYNC_HISTORY 
			@USER_ID INT,
			@SID CHAR(36)*/


			m_ParamArray.AddLong(pRequest->m_nUserID);
			m_ParamArray.AddGUID((BSTR)bsSid);

			CADOUtil::RunSP_ReturnRS(m_bs_OM_SYNC_HISTORY,RecSet,&m_ParamArray);

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_message,m_bs_messages);
				RecSet->Close();
				RecSet = NULL;
			}
			else
			{
				CRS2XML::ParseList(NULL, pRequest->m_WorkNode, g_r2x_message,m_bs_messages);
			}
		}

		CXMLUtil::AppendNode(pRequest->m_WorkNode,CComBSTR(L"list"),NULL,NULL,bsListType);

		return TRUE;
	}
	catch(...)
	{
		ATLASSERT(FALSE);
	} 
	return FALSE;
}

BOOL CSupportClass::z_AddUser()
{
	GET_ISAPI_REQUEST();
	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	HRESULT						hr = S_OK;

	CComPtr<IXMLDOMNode>		pNode;
	CComBSTR					bsBody,bsListType;
	LONG						AddID;
	HGLOBAL						hGlobal = NULL;
	DWORD						dwSize = 0;


	//UNPACK XML USER
	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"user/@id"),&AddID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"list/@type"),&bsListType);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);


	if(bsListType == CComBSTR(L"contact"))
	{

		if(!CanISend(pRequest->m_nUserID,AddID))
		{
			pRequest->m_pXMLDoc.Release();
			pRequest->m_WorkNode.Release();
			return TRUE;
		}

		hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"body"),&bsBody);
		CHECK_HR_THROW(hr,ERR_WRONG_XML);


		/*OM_ADD_AUTH_REQUEST
		@FROM_USER_ID INT,
		@TO_USER_ID INT,
		@MESS_TEXT nVARCHAR(1024)*/

		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(AddID);
		m_ParamArray.Add(adVarWChar,adParamInput,1024,_variant_t(bsBody));

		CADOUtil::RunSP_ReturnRS(m_bs_OM_ADD_AUTH_REQUEST,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{

			LONG Result = 0;

			//recordset deny_flag
			try
			{
				Result = RecSet->Fields->GetItem(_bstr_t(L"deny_flag"))->Value.lVal;
			}
			catch(...)
			{
			}
			hr = CreateEventPacket(&pNode);
			CHECK_HR_THROW(hr,ERR_UNKNOW);
			LONG		SendTo;


			if(Result == 0)
			{
				CRS2XML::ParseMulti(RecSet, pNode, g_r2x_adduser,m_bs_event);
				RecSet->Close();
				RecSet = NULL;

				SendTo = AddID;
			}
			else
			{
				CRS2XML::ParseMulti(RecSet, pNode, g_r2x_adduserr,m_bs_event);
				RecSet->Close();
				RecSet = NULL;

				SendTo = pRequest->m_nUserID;
			}

			hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
			CHECK_HR_THROW(hr,ERR_UNKNOW);
			try
			{
				g_ActiveSessions.SendEvent(SendTo,(PBYTE)GlobalLock(hGlobal),dwSize-2);
			}
			catch(...){}
			GlobalUnlock(hGlobal);
			GlobalFree(hGlobal);

			pRequest->m_WorkNode.Release();
			pRequest->m_pXMLDoc.Release();
			return TRUE;
		}
	}

	if(bsListType == CComBSTR(L"ignore"))
	{

		/*CREATE PROCEDURE OM_ADD_USER
		@USER_ID INT,
		@CONT_USER_ID INT,
		@LIST_TYPE INT,
		@RETVAL INT OUTPUT*/

		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(AddID);
		m_ParamArray.AddLong(2);

		if(CADOUtil::RunSP_ReturnLong(m_bs_OM_ADD_USER,&m_ParamArray) != -1)
		{
			pRequest->m_pXMLDoc.Release();
			pRequest->m_WorkNode.Release();
		}
		else
			throw(ERR_WRONG_ID);

		pRequest->m_WorkNode.Release();
		pRequest->m_pXMLDoc.Release();
		return TRUE;
	}
	throw(ERR_WRONG_XML);

}

BOOL CSupportClass::z_LogOff()
{
	GET_ISAPI_REQUEST();
	CParamArray		m_ParamArray;
	LONG ltime = GetGMTtime_t(); 
	_RecordsetPtr	RecSet,RecSet1;
	CComPtr<IXMLDOMNode> pNode;
	HGLOBAL			hGlobal;
	DWORD			dwSize;
	HRESULT			hr;

	try
	{
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(ltime);
		CADOUtil::RunSP_ReturnLong(m_bs_OM_LOGOFF,&m_ParamArray);

		g_ActiveSessions.DeleteActiveSession(pRequest->m_nUserID);
#ifdef _IBN_PERFORMANCE_MONITOR
		if (CCounter::GetHasRealTimeMonitoring())
			::InterlockedDecrement((LONG*)CCounter::m_ulActiveUser);
#endif

		y_EventChangedState(0);
	}
	catch(...)
	{
	}


	try
	{
		long ID = pRequest->m_nUserID;
		/* IBN_LOAD_CHATS
		@USER_ID as int,
		@MODE as tinyint*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(ID);
		m_ParamArray.AddLong(1);
		CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LOAD_CHATS"),RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			do
			{
				_variant_t CID = RecSet->Fields->GetItem(_bstr_t(_T("CUID")))->GetValue();
				try
				{/* IBN_CHANGE_CHAT_STATUS 
				 @CUID char(36),
				 @USER_ID INT,
				 @USER_STATUS tinyint,
				 @RETVAL INT OUTPUT*/
					m_ParamArray.RemoveAll();
					m_ParamArray.AddGUID((BSTR)CID.bstrVal);
					m_ParamArray.AddLong(ID);
					m_ParamArray.AddLong(0);
					m_ParamArray.AddLong(ltime);
					CADOUtil::RunSP_ReturnLong(_bstr_t(L"IBN_CHANGE_CHAT_STATUS")
						,&m_ParamArray);


				}
				catch(...)
				{
				}

				try
				{
					/*IBN_GET_CHAT_USER_INFO
					@CUID as char(36),
					@USER_ID as int*/
					m_ParamArray.RemoveAll();
					m_ParamArray.AddGUID((BSTR)CID.bstrVal);
					m_ParamArray.AddLong(ID);
					CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_GET_CHAT_USER_INFO")
						,RecSet1
						,&m_ParamArray);

					hr = CreateEventPacket(&pNode);
					CHECK_HR_THROW(hr,ERR_UNKNOW);

					CRS2XML::ParseMulti(RecSet1, pNode, g_r2x_chat_status,m_bs_event);

					hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
					CHECK_HR_THROW(hr,ERR_UNKNOW);

					SendChatBroadcast(CID.bstrVal,(PBYTE)GlobalLock(hGlobal),dwSize);

					GlobalUnlock(hGlobal);
					GlobalFree(hGlobal);
					pNode.Release();

				}
				catch(...)
				{}

				RecSet->MoveNext();
			}
			while(!RecSet->EndOfFile);	

			RecSet->Close();
			RecSet = NULL;
		}

	}
	catch(...)
	{
	}
	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();

	return TRUE;
}

BOOL CSupportClass::z_ConfirmMessage()
{
	GET_ISAPI_REQUEST();
	CParamArray					m_ParamArray;
	CComBSTR					bsMID;
	HRESULT						hr = S_OK;

	//UNPACK XML USER
	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"message/@mid"),&bsMID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	/*OM_CONFIRM_MESSAGE
	@MID CHAR(36),
	@SID CHAR(36),
	@USER_ID INT,
	@RETVAL INT OUTPUT*/

	m_ParamArray.AddGUID(bsMID);
	m_ParamArray.AddGUID(CComBSTR(pRequest->m_szSessionID));
	m_ParamArray.AddLong(pRequest->m_nUserID);
	try
	{
		CADOUtil::RunSP_ReturnLong(m_bs_OM_CONFIRM_MESSAGE,&m_ParamArray);
	}catch(...){};

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();
	return TRUE;
}

BOOL CSupportClass::z_ConfirmPromo()
{
	/*	GET_ISAPI_REQUEST();
	CParamArray					m_ParamArray;
	CComBSTR					bsPID;
	HRESULT						hr = S_OK;

	//UNPACK XML USER
	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"promo/@pid"),&bsPID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	/*OM_CONFIRM_PROMO
	@PID CHAR(36),
	@SID CHAR(36),
	@USER_ID INT,
	@RETVAL INT OUTPUT*/

	/*	m_ParamArray.AddGUID((BSTR)bsPID);
	m_ParamArray.AddGUID(CComBSTR(pRequest->m_szSessionID));
	m_ParamArray.AddLong(pRequest->m_nUserID);
	try
	{
	CADOUtil::RunSP_ReturnLong(_bstr_t(L"M_CONFIRM_PROMO"),&m_ParamArray);
	}catch(...){};

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();*/
	return TRUE;
}

BOOL CSupportClass::z_ConfirmFile()
{
	GET_ISAPI_REQUEST();
	CParamArray					m_ParamArray;
	CComBSTR					bsFID,bsResult;
	HRESULT						hr = S_OK;
	LONG						Result = 0;

	//UNPACK XML USER
	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"file/@fid"),&bsFID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"result"),&bsResult);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);


	Result = 0;
	if(bsResult == CComBSTR(L"delete"))
		Result = 2 ;
	else
		if(bsResult == CComBSTR(L"offline"))
			Result = 3;

	if(Result == 0)
		throw(ERR_WRONG_XML);

	/*OM_CONFIRM_FILE 
	@FID CHAR(36),
	@FILE_STATUS INT,
	@USER_ID INT,
	@RETVAL INT OUTPUT*/

	m_ParamArray.AddGUID((BSTR)bsFID);
	m_ParamArray.AddLong(Result);
	m_ParamArray.AddLong(pRequest->m_nUserID);

	try
	{
		LONG count = CADOUtil::RunSP_ReturnLong(m_bs_OM_CONFIRM_FILE, &m_ParamArray);
		if(count == 0)
		{
			CDBFile::DeleteFile(bsFID);
		}
	}
	catch(...){};

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();
	return TRUE;
}



BOOL CSupportClass::z_DelAuthReq()
{
	GET_ISAPI_REQUEST();
	CParamArray					m_ParamArray;
	_RecordsetPtr				RecSet	= NULL;
	CComPtr<IXMLDOMNode>		pNode;
	LONG						AddID = 0, Result = 0;
	HRESULT						hr = S_OK;
	HGLOBAL						hGlobal = NULL;
	DWORD						dwSize =0;


	//UNPACK XML USER

	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"user/@id"),&AddID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	m_ParamArray.AddLong(pRequest->m_nUserID);
	m_ParamArray.AddLong(AddID);

	Result =CADOUtil::RunSP_ReturnLong(m_bs_OM_DELETE_AUTH_REQUEST,&m_ParamArray);

	switch(Result)
	{
	case 1:
	case 3: //Accept
		/*OM_ADD_USER
		@USER_ID INT,
		@CONT_USER_ID INT,
		@LIST_TYPE INT,*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(AddID);
		m_ParamArray.AddLong(1);
		m_ParamArray.AddLong((Result == 1)?1:0);

		Result =CADOUtil::RunSP_ReturnLong(m_bs_OM_ADD_USER,&m_ParamArray);

		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(AddID);
		m_ParamArray.AddLong(1);
		m_ParamArray.AddLong(GetGMTtime_t());

		//SEND CHANGE CONTACT
		CADOUtil::RunSP_ReturnRS(m_bs_OM_GET_USER_INFO,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{	
			hr = CreateEventPacket(&pNode);
			CHECK_HR_THROW(hr,ERR_UNKNOW);

			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_userBasicInfo,m_bs_event);
			hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
			if(hr != S_OK) break;
			try
			{
				g_ActiveSessions.SendEvent(pRequest->m_nUserID,(PBYTE)GlobalLock(hGlobal),dwSize-2);
			}
			catch(...)
			{
			}
			GlobalUnlock(hGlobal);
			GlobalFree(hGlobal);

		}


		break;
	case 2: //Deny

		break;
	default:
		break;
	}
	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();
	return TRUE;

}

DWORD WINAPI CSupportClass::DelSessionsCallBack(LPVOID param)
{
	BOOL			ExitFlag = false;
	CParamArray		m_ParamArray;
	MSG				msg;
	LONG			id;
	CSupportClass	*pSupportClass;
	LONG			ltime;
	_RecordsetPtr	RecSet,RecSet1;
	CComPtr<IXMLDOMNode> pNode;
	HGLOBAL			hGlobal;
	DWORD			dwSize;
	HRESULT			hr;
	_bstr_t bsText,bsLogin;
	pSupportClass = (CSupportClass*)param;

	::InterlockedIncrement(&OpenThread);
	/*HRESULT hr = */::CoInitializeEx(NULL,COINIT_MULTITHREADED);

	while(!ExitFlag && GetMessage(&msg, NULL, NULL, NULL))
	{
		switch(msg.message)
		{
		case WM_CONNECT_TIMEOUT:
			id = (LONG)msg.wParam;
			ltime = GetGMTtime_t();

			try
			{
				/*OM_LOGOFF
				@USER_ID INT,
				@RETVAL INT OUTPUT*/
				m_ParamArray.RemoveAll();
				m_ParamArray.AddLong(id);
				m_ParamArray.AddLong(ltime);
				CADOUtil::RunSP_ReturnLong(m_bs_OM_LOGOFF, &m_ParamArray);

				g_ActiveSessions.DeleteActiveSession(id);
#ifdef _IBN_PERFORMANCE_MONITOR
				if (CCounter::GetHasRealTimeMonitoring())
					::InterlockedDecrement((LONG*)CCounter::m_ulActiveUser);
#endif

				pSupportClass->y_EventChangedState(0, id);
				//				pSupportClass->AddToLog(5,"DelSession from SQL");
			}
			catch(...)
			{
				//				pSupportClass->AddToLog(5,"Unable DelSession from SQL");
			}

			try
			{
				/* IBN_LOAD_CHATS
				@USER_ID as int,
				@MODE as tinyint*/
				m_ParamArray.RemoveAll();
				m_ParamArray.AddLong(id);
				m_ParamArray.AddLong(1);
				CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LOAD_CHATS"), RecSet, &m_ParamArray);

				if(CADOUtil::CheckRecordSetState(RecSet))
				{
					do
					{
						_variant_t CID = RecSet->Fields->GetItem(_bstr_t(_T("CUID")))->GetValue();
						try
						{/* IBN_CHANGE_CHAT_STATUS 
						 @CUID char(36),
						 @USER_ID INT,
						 @USER_STATUS tinyint,
						 @RETVAL INT OUTPUT*/
							m_ParamArray.RemoveAll();
							m_ParamArray.AddGUID((BSTR)CID.bstrVal);
							m_ParamArray.AddLong(id);
							m_ParamArray.AddLong(0);
							m_ParamArray.AddLong(ltime);
							CADOUtil::RunSP_ReturnLong(_bstr_t(L"IBN_CHANGE_CHAT_STATUS"), &m_ParamArray);


						}
						catch(...)
						{
						}

						try
						{
							/*IBN_GET_CHAT_USER_INFO
							@CUID as char(36),
							@USER_ID as int*/
							m_ParamArray.RemoveAll();
							m_ParamArray.AddGUID((BSTR)CID.bstrVal);
							m_ParamArray.AddLong(id);
							CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_GET_CHAT_USER_INFO"), RecSet1, &m_ParamArray);

							hr = CreateEventPacket(&pNode);
							CHECK_HR_THROW(hr,ERR_UNKNOW);

							CRS2XML::ParseMulti(RecSet1, pNode, g_r2x_chat_status,m_bs_event);

							hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
							CHECK_HR_THROW(hr,ERR_UNKNOW);

							SendChatBroadcast(CID.bstrVal,(PBYTE)GlobalLock(hGlobal),dwSize);

							GlobalUnlock(hGlobal);
							GlobalFree(hGlobal);
							pNode.Release();

						}
						catch(...){}

						RecSet->MoveNext();
					}
					while(!RecSet->EndOfFile);

					RecSet->Close();
					RecSet = NULL;
				}

			}
			catch(...)
			{
			}
			break;
		case WM_UPDATE_USER:
			UpdateUser((LONG)msg.lParam, _bstr_t(_T("2")), _bstr_t(_T("")));
			break;
		case WM_UPDATE_GROUP:
			UpdateGroup((LONG)msg.lParam, _bstr_t(_T("1")), _bstr_t(_T("")));
			break;
		case WM_UPDATE_STUBS:
			UpdateGroup((LONG)msg.lParam, _bstr_t(_T("3")), _bstr_t(_T("")));
			break;
		case WM_UPDATE_USERSTUBS:
			UpdateUser((LONG)msg.lParam, _bstr_t(_T("3")), _bstr_t(_T("")));
			break;
		case WM_UPDATE_ALERT:
			bsText.Attach((BSTR)msg.wParam);
			UpdateGroup((LONG)msg.lParam, _bstr_t(_T("4")), bsText);
			break;
		case WM_UPDATE_NET_ALERT:
			bsText.Attach((BSTR)msg.wParam);
			UpdateUser((LONG)msg.lParam, _bstr_t(_T("4")), bsText);
			break;
		case WM_UPDATE_NET_MESSAGE:
			try
			{
				bsText.Attach((BSTR)msg.wParam);
				SendWebMessage(((FromToID*)msg.lParam)->m_FromID,
					((FromToID*)msg.lParam)->m_ToID,
					bsText);
				delete (FromToID*)msg.lParam;
			}
			catch(...)
			{
			}
			break;
		case WM_LOGOFF_NET_MESSAGE:
			pSupportClass->LogOffUser((LONG)msg.lParam);
			break;
		case WM_STOP_ACTIVITY:
			// Step 1. Logoff All Users
			g_ActiveSessions.Terminate(3000);
			CADOUtil::LockDatabase();
			
			// Step 2. Mark ADO Util as Release
			break;
		case WM_BEGIN_THREAD:
			break;
		case WM_END_THREAD:
			ExitFlag = true;
			break;
		// OZ: 2009-03-17
		case WM_CHANGE_STATUS:
			UpdateUserStatus(static_cast<LONG>(msg.lParam), static_cast<LONG>(msg.wParam));
			break;
		default:
			DispatchMessage(&msg);
			break;
		}
	}

	::CoUninitialize();
	::InterlockedDecrement(&OpenThread);

	return 0;
}

BOOL CSupportClass::y_EventChangedState(long Status, LONG UserID)
{
	if(UserID == NULL)
	{
		GET_ISAPI_REQUEST();
		UserID = pRequest->m_nUserID;
	}

	CComPtr<IXMLDOMNode>  pNode;
	CParamArray			  m_ParamArray;
	_RecordsetPtr		  RecSet;
	PBYTE				  buff = NULL;
	DWORD				  size = 0;
	HRESULT				  hr = S_OK;
	try
	{
		//============================================================
		//Get Self Info

		/*OM_GET_USER_INFO 
		@USER_ID INT,
		@INFO_TYPE INT*/
		m_ParamArray.AddLong(UserID);
		m_ParamArray.AddLong(1);
		m_ParamArray.AddLong(GetGMTtime_t());

		CADOUtil::RunSP_ReturnRS(m_bs_OM_GET_USER_INFO,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			FieldPtr field = RecSet->Fields->GetItem(_bstr_t(L"status"));
			if(Status == 2) Status = 0;

			field->put_Value(_variant_t(Status));

			if(S_OK != CreateEventPacket(&pNode))
				return FALSE;

			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_userBasicInfo,m_bs_event);
			RecSet->Close();
			RecSet = NULL;
		}
		else
			return FALSE;

		//===========================================================
		//Send Event
		HGLOBAL hGlobal;
		hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&size);
		if(hr != S_OK) return FALSE;


		try
		{
			buff = (PBYTE) GlobalLock(hGlobal);

			/*OM_LOAD_LIST_REV 
			@USER_ID INT,
			@LIST_TYPE TINYINT*/
			m_ParamArray.RemoveAll();
			m_ParamArray.AddLong(UserID);
			m_ParamArray.AddLong(1);

			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_LIST_REV,RecSet,&m_ParamArray);
			m_ParamArray.RemoveAll();

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				while(!RecSet->GetEndOfFile())
				{
					try
					{
						_variant_t id = RecSet->Fields->GetItem(_bstr_t(L"USER_ID"))->GetValue();	
						g_ActiveSessions.SendEvent((long)id,buff,size-2);

					}catch(...){};
					RecSet->MoveNext();
				}
				RecSet->Close();
				RecSet = NULL;
			} 
		}
		catch(...)
		{
		}

		GlobalUnlock(hGlobal);
		GlobalFree(hGlobal);
		return TRUE;
	}
	catch(...)
	{}
	return FALSE;
}

BOOL CSupportClass::CanISend(long From_ID, long To_ID)
{
	CParamArray		m_ParamArray;
	BOOL			retVal = FALSE;
	/*OM_CHECK_IGNORE 
	@USER_ID INT,
	@CONT_USER_ID INT,
	@LIST_TYPE TINYINT,
	@RETVAL INT OUTPUT*/

	m_ParamArray.AddLong(From_ID);
	m_ParamArray.AddLong(To_ID);
	m_ParamArray.AddLong((long)2);
	try
	{
		if(CADOUtil::RunSP_ReturnLong(m_bs_OM_CHECK_IGNORE,&m_ParamArray) == 0)
			retVal = TRUE;
	}
	catch(...)
	{}
	return retVal;
}

BOOL CSupportClass::z_SentFile()
{
	//return TRUE;
	GET_ISAPI_REQUEST();

	CComBSTR				bsFID,bsBody,bsRealName;
	LONG					nSize,nID;
	CComPtr<IXMLDOMNode>	pNode,pEventNode,pTemp;
	_RecordsetPtr			RecSet;
	CParamArray				m_ParamArray;
	CHGLOBAL				hGlobal;
	DWORD					dwSize  = 0;
	PBYTE					pBuffer = NULL;
	HRESULT					hr = S_OK;
	LONG ltime = GetGMTtime_t(); 

	//UNPACK XML USER
	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"file/@fid"),&bsFID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"file/body"),&bsBody);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"file/real_name"),&bsRealName);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"file/size"),&nSize);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = pRequest->m_WorkNode->selectSingleNode(CComBSTR(L"file/recipients/user"),&pNode);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);


	while(pNode)
	{
		hr = CXMLUtil::GetValueByPath(pNode,CComBSTR(L"@id"),&nID);
		if(hr != S_OK || nID == 0)
		{
			hr = pNode->get_nextSibling(&pTemp);
			if(hr != S_OK) break;
			pNode.Release();
			pNode = pTemp;
			continue;
		}

		//if(CanISend(pRequest->m_nUserID,nID))
		//{
		/*
		OM_ADD_FILE
		@FROM_USER_ID INT,
		@TO_USER_ID INT,
		@FILE_ID CHAR(36),			-- stored filename in directory
		@FILE_NAME VARCHAR(255),		-- real filename
		@FILE_DESCR nVARCHAR(1024),
		@FILE_DATE INT,
		@FILE_SIZE INT*/

		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(nID);
		m_ParamArray.AddGUID((BSTR)bsFID);
		m_ParamArray.Add(adVarWChar,adParamInput,255,_variant_t(bsRealName));
		m_ParamArray.Add(adVarWChar,adParamInput,1024,_variant_t(bsBody));
		m_ParamArray.AddLong(ltime);
		m_ParamArray.AddLong(nSize);

		try
		{
			CADOUtil::RunSP_ReturnRS(m_bs_OM_ADD_FILE,RecSet,&m_ParamArray);
		}
		catch(...)
		{
			RecSet = NULL;
		};


		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			pRequest->m_bEventSent = TRUE;

			if(pEventNode == NULL)
			{
				CreateEventPacket(&pEventNode);
				CRS2XML::ParseMulti(RecSet, pEventNode, g_r2x_file,m_bs_event);
				hr = CXMLUtil::XML2HGlobal(pEventNode,&hGlobal,&dwSize);
				CHECK_HR_THROW(hr,ERR_UNKNOW);
				pBuffer = (PBYTE)hGlobal.Lock();
			}

			try
			{
				g_ActiveSessions.SendEvent(nID,pBuffer,dwSize-2);
			}
			catch(...){}

			RecSet->Close();
			RecSet = NULL;
		} 

		hr = pNode->get_nextSibling(&pTemp);
		if(hr != S_OK) break;
		pNode.Release();
		pNode = pTemp;
		pTemp.Release();
	}


	CreateResponsePacket();
	TCHAR	Value[30];
	_ltot_s(ltime,Value,30,10);
	hr = CXMLUtil::AppendNode(pRequest->m_WorkNode,CComBSTR(L"time"),NULL,NULL,CComBSTR(Value));
	CHECK_HR_THROW(hr,ERR_UNKNOW);

	return TRUE;
}

BOOL CSupportClass::z_GetUserInfo()
{
	GET_ISAPI_REQUEST();
	LONG				UserID;
	CComBSTR			bsInfoType;
	CParamArray			m_ParamArray;
	_RecordsetPtr		RecSet;
	HRESULT				hr = S_OK;

	try
	{
		//UNPACK XML
		hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"user/@id"),&UserID);
		CHECK_HR_THROW(hr,ERR_WRONG_XML);

		hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"info_type"),&bsInfoType);
		CHECK_HR_THROW(hr,ERR_WRONG_XML);

		LONG	InfoType = 0;
		if(CComBSTR(L"short") == bsInfoType) InfoType = 1;
		else
			if(CComBSTR(L"full")  == bsInfoType) InfoType = 2;

		if(InfoType == 0) throw(ERR_WRONG_XML);

		/*OM_GET_USER_INFO 
		@USER_ID INT,
		@INFO_TYPE INT*/
		m_ParamArray.AddLong(UserID);
		m_ParamArray.AddLong(InfoType);
		m_ParamArray.AddLong(GetGMTtime_t());

		CADOUtil::RunSP_ReturnRS(m_bs_OM_GET_USER_INFO,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			FieldPtr field = RecSet->Fields->GetItem(_bstr_t(L"status"));
			field->put_Value(_variant_t(0));

			CreateResponsePacket();

			CRS2XML::Parse(RecSet,pRequest->m_WorkNode, g_r2x_userBasicInfo);
			RecSet->Close();
			RecSet = NULL;
			return TRUE;
		}
		throw ERR_UNKNOW;
	}
	catch(...){}

	throw ERR_UNKNOW;

}

BOOL CSupportClass::Initialize(DWORD& dwCallBackThreadID)
{
	if(m_bIsInit) return TRUE;

	InitSPName();
	InitCommandNames();

	HANDLE hThread = NULL;

	hThread = (HANDLE)_beginthreadex(NULL, 512000, 
		(unsigned int (__stdcall *)(void *)) DelSessionsCallBack,
		(LPVOID)this, 
		0, 
		(unsigned int *) &DelSessionsCallBackID);
	/*hThread = CreateThread(NULL,
	512000,
	DelSessionsCallBack,
	(LPVOID)this,
	NULL,
	&DelSessionsCallBackID);*/

	if(hThread == NULL) return FALSE;
	CloseHandle(hThread);

	dwCallBackThreadID = DelSessionsCallBackID;

	while(!::PostThreadMessage(DelSessionsCallBackID,WM_BEGIN_THREAD,0,0))
		Sleep(100);


	m_bIsInit = TRUE;

	return TRUE;

}

VOID CSupportClass::Terminate()
{
	if(!m_bIsInit) return;

	PostThreadMessage(DelSessionsCallBackID,WM_END_THREAD,0,0);
	PostThreadMessage(DelSessionsCallBackID,WM_END_THREAD,0,0);
	while(OpenThread>0)
		Sleep(100);

	m_bIsInit = FALSE;
}

void CSupportClass::x_StatusChanged(long ID, long Status)
{
	//MSXML::IXMLDOMDocumentPtr plDom = NULL;
	//MSXML::IXMLDOMNodePtr	  plNode = NULL;
	//plDom.CreateInstance(__uuidof(MSXML::DOMDocument));
	//if(plDom == NULL) return;
	//plNode = plDom->createNode(_variant_t((long)MSXML::NODE_ELEMENT),_bstr_t(L"packet"),_bstr_t(L""));
	//plDom->appendChild(plNode);
	//if(plNode == NULL) return;
	y_EventChangedState(Status,ID);
}

BOOL CSupportClass::z_DeleteUser()
{
	GET_ISAPI_REQUEST();
	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	HRESULT						hr = S_OK;
	LONG						DelID,ListType = 0;
	CComBSTR					bsListType;


	//UNPACK XML USER
	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"user/@id"),&DelID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"list/@type"),&bsListType);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	if(bsListType == CComBSTR(L"contact"))
		ListType = 1;
	else
		if(bsListType == CComBSTR(L"ignore"))
			ListType = 2;
		else
			throw(ERR_WRONG_XML);

	/*
	OM_DELETE_USER
	@USER_ID INT,
	@CONT_USER_ID INT,
	@LIST_TYPE INT,
	@RETVAL INT OUTPUT*/

	m_ParamArray.AddLong(pRequest->m_nUserID);
	m_ParamArray.AddLong(DelID);
	m_ParamArray.AddLong(ListType);

	try
	{
		CADOUtil::RunSP_ReturnLong(m_bs_OM_DELETE_USER,&m_ParamArray);
	}catch(...){};

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();
	return TRUE;
}

HRESULT CSupportClass::UpdateGroup(LONG groupId, BSTR code, BSTR description)
{
	HRESULT hr;
	CComPtr<IXMLDOMNode>	pPacketNode, pEventNode, pSystemNode;
	HGLOBAL					hGlobal = NULL;
	PBYTE					pBuffer = NULL;
	_RecordsetPtr			RecSet;
	CParamArray				m_ParamArray;
	DWORD					dwSize;

	GET_ISAPI_REQUEST();
	
	try
	{	
		hr = CreateEventPacket(&pPacketNode);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pPacketNode, CComBSTR(L"event"), NULL, &pEventNode);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pEventNode, CComBSTR(L"system"), NULL, &pSystemNode);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pSystemNode, CComBSTR(L"code"), NULL, NULL, code);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pSystemNode, CComBSTR(L"descr"), NULL, NULL, description);
		if(hr != S_OK) return hr;

		hr = CXMLUtil::XML2HGlobal(pEventNode, &hGlobal, &dwSize);
		if(hr != S_OK)
			return hr;

		pBuffer = (PBYTE)GlobalLock(hGlobal);

		try
		{
			m_ParamArray.AddLong(groupId);
			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_ACTIVE_USERS_BY_ROLE, RecSet, &m_ParamArray);
		}
		catch(...)
		{
		}

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			do
			{
				_variant_t valID = RecSet->Fields->GetItem(_bstr_t(_T("user_id")))->GetValue();

				try
				{
					g_ActiveSessions.SendEvent((long)valID, pBuffer, dwSize-2);
				}
				catch(...){}
				RecSet->MoveNext();
			}
			while(!RecSet->EndOfFile);

			RecSet->Close();
			RecSet = NULL;
		}
	}
	catch(...)
	{
	}

	if(hGlobal != NULL)
	{
		try
		{
			if(pBuffer != NULL)
				GlobalUnlock(hGlobal);
			GlobalFree(hGlobal);
		}
		catch(...)
		{
		}
	}

	return S_OK;
}

HRESULT CSupportClass::UpdateUser(LONG userId, BSTR code, BSTR descr)
{
	HRESULT hr;
	HGLOBAL hGlobal = NULL;
	DWORD dwSize;
	PBYTE pBuffer = NULL;

	CComPtr<IXMLDOMNode> pPacketNode, pEventNode, pSystemNode;
	GET_ISAPI_REQUEST();

	try
	{	
		hr = CreateEventPacket(&pPacketNode);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pPacketNode,CComBSTR(L"event"), NULL, &pEventNode);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pEventNode,CComBSTR(L"system"), NULL, &pSystemNode);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pSystemNode,CComBSTR(L"code"), NULL, NULL, code);
		if(hr != S_OK) return hr;
		hr = CXMLUtil::AppendNode(pSystemNode,CComBSTR(L"descr"), NULL, NULL, descr);
		if(hr != S_OK) return hr;

		hr = CXMLUtil::XML2HGlobal(pEventNode, &hGlobal, &dwSize);
		if(hr != S_OK) return hr;

		pBuffer = (PBYTE)GlobalLock(hGlobal);
		if(pBuffer != NULL)
			g_ActiveSessions.SendEvent(userId, pBuffer, dwSize-2);

	}
	catch(...)
	{
	}

	if(hGlobal != NULL)
	{
		try
		{
			if(pBuffer != NULL)
				GlobalUnlock(hGlobal);
			GlobalFree(hGlobal);
		}
		catch(...)
		{
		}
	}

	return S_OK;
}

HRESULT CSupportClass::SendWebMessage(LONG UserID,LONG ToID, BSTR Message)
{

	HRESULT						hr = S_OK;
	CComBSTR					bsBody,bsCID;
	CComPtr<IXMLDOMNode>		pNode,pEventNode,pTemp;
	//	LONG						nToID;
	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	CHGLOBAL					hGlobal;
	DWORD						dwSize  = 0;
	PBYTE						pBuffer = NULL;
	//BOOL						m_bCHAT = FALSE;

	LONG ltime = GetGMTtime_t(); 
	LONG DB_time = 0;	

	GUID	 SID;
	CoCreateGuid(&SID);

	OLECHAR oleTempFID[90];
	StringFromGUID2(SID,oleTempFID,90);
	oleTempFID[37] = L'\0';
	CComBSTR bsMID = CComBSTR(oleTempFID+1);

	/*OM_SEND_MESSAGE
	@FROM_USER_ID INT,
	@TO_USER_ID INT,
	@MESS_ID CHAR(36),
	@MESS_TEXT NTEXT(16),
	@SEND_TIME INT,
	@USER_SID CHAR(36),*/
	m_ParamArray.RemoveAll();
	m_ParamArray.AddLong(UserID);
	m_ParamArray.AddLong(ToID);
	m_ParamArray.AddGUID((BSTR)bsMID);
	m_ParamArray.Add(adLongVarWChar,adParamInput,::SysStringLen(Message),_variant_t(Message));
	m_ParamArray.AddLong(ltime);
	m_ParamArray.AddGUID((BSTR)bsMID);

	try
	{
		CADOUtil::RunSP_ReturnRS(m_bs_OM_SEND_MESSAGE,RecSet,&m_ParamArray);
	}
	catch(LONG err)
	{
		if(err == ERR_UNABLE_CREATE_CONN)
			throw err;
		RecSet = NULL;
	}
	catch(...)
	{RecSet = NULL;};


	if(CADOUtil::CheckRecordSetState(RecSet))
	{
		if(pEventNode == NULL)
		{
			FieldPtr field = RecSet->Fields->GetItem(_bstr_t(L"send_time"));
			DB_time = field->Value.lVal;

			CreateEventPacket(&pEventNode);
			CRS2XML::ParseMulti(RecSet,pEventNode , g_r2x_message,CComBSTR(L"event"));
			hr = CXMLUtil::XML2HGlobal(pEventNode,&hGlobal,&dwSize);
			CHECK_HR_THROW(hr,ERR_UNKNOW);
			pBuffer = (PBYTE)hGlobal.Lock();
		}

		try
		{
			if(DB_time == ltime)
				g_ActiveSessions.SendEvent(ToID,pBuffer,dwSize-2);
		}
		catch(...){}

		RecSet->Close();
		RecSet = NULL;
	}

	return S_OK;
}

HRESULT CSupportClass::LogOffUser(LONG UserId)
{
	//  [3/18/2004]
	CParamArray m_ParamArray;

	try
	{
		LONG ltime = GetGMTtime_t();

		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(UserId);
		m_ParamArray.AddLong(ltime);
		CADOUtil::RunSP_ReturnLong(m_bs_OM_LOGOFF,&m_ParamArray);

		g_ActiveSessions.DeleteActiveSession(UserId);
#ifdef _IBN_PERFORMANCE_MONITOR
		if (CCounter::GetHasRealTimeMonitoring())
			::InterlockedDecrement((LONG*)CCounter::m_ulActiveUser);
#endif

		y_EventChangedState(0,UserId);
	}
	catch(...)
	{
		return E_FAIL;
	}

	return S_OK;
}

BOOL CSupportClass::SendOffLineEvents()
{
	GET_ISAPI_REQUEST();

	CComPtr<IXMLDOMNode> pNode;
	//MSXML::IXMLDOMNodePtr pNodePtr;
	HRESULT hr = S_OK;
	try
	{	
		hr = CreateEventPacket(&pNode);
		if(hr != S_OK)
			return FALSE;


		//	pNodePtr.Attach((MSXML::IXMLDOMNodePtr)pNode.p,true);

		_RecordsetPtr RecSet, RecSetUser = NULL;
		CParamArray m_ParamArray;

		//LOAD OFFLINE MESSAGE
		/*OM_LOAD_MESS 
		@USER_ID INT*/
		m_ParamArray.AddLong(pRequest->m_nUserID);
		CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_MESS, RecSet, &m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_message,m_bs_event);
			RecSet->Close();
			RecSet = NULL;
		}	

		//LOAD OFFLINE FILES
		/*OM_LOAD_FILES 
		@TO_USER_ID INT,
		@FILE_STATUS INT*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(0);
		CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_FILES,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{	
			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_file,m_bs_event);
			RecSet->Close();
			RecSet = NULL;
		}


		/*
		OM_LOAD_AUTH_LIST_REV	-- Who wants me ?
		@USER_ID INT
		*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);

		CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_AUTH_LIST_REV,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{	
			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_adduser,m_bs_event);
			RecSet->Close();
			RecSet = NULL;
		}

		/*
		CREATE PROCEDURE OM_LOAD_AUTH_LIST
		@USER_ID INT,
		@STATUS INT
		*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(1);
		m_ParamArray.AddLong(GetGMTtime_t());

		CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_AUTH_LIST,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{	
			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_adduserr,m_bs_event);
			RecSet->Close();
			RecSet = NULL;
		}
		/*
		IBN_LOAD_CHAT_INVITES
		@USER_ID as int 
		*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddLong(pRequest->m_nUserID);

		CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LOAD_CHAT_INVITES"),RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{	
			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_chat_invite, m_bs_event);
			RecSet->Close();
			RecSet = NULL;
		}

		HGLOBAL hGlobal;
		DWORD   dwSize;
		hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
		if(hr != S_OK)
			return FALSE;

		try
		{
			g_ActiveSessions.SendEvent(pRequest->m_nUserID, (PBYTE)GlobalLock(hGlobal), dwSize - 2);
		}
		catch(...)
		{
		}

		GlobalUnlock(hGlobal);
		GlobalFree(hGlobal);
		return TRUE;
		//return SendMessageTo(pRequest->m_nUserID,pNode);
	}
	catch(...)
	{
	}
	return FALSE;
}

HRESULT CSupportClass::CreateResponsePacket()
{
	GET_ISAPI_REQUEST();
	CComPtr<IXMLDOMNode> pTempNode;
	HRESULT hr = S_OK;

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();

	hr = CXMLUtil::m_pClassFactory->CreateInstance(NULL,IID_IXMLDOMDocument, (void**)&pRequest->m_pXMLDoc);
	
	if(hr != S_OK) 
	{
		CEventLog::AddAppLog(_T("CreateResponsePacket.CreateInstance"), hr, EVENTLOG_ERROR_TYPE);
		return hr;
	}

	CComPtr<IXMLDOMDocument2> pDoc2;
	hr = pRequest->m_pXMLDoc->QueryInterface(__uuidof(IXMLDOMDocument2), (void**)&pDoc2);
	if(hr != S_OK) 
	{
		CEventLog::AddAppLog(_T("CreateResponsePacket.QueryInterface"), hr, EVENTLOG_ERROR_TYPE);
		return hr;
	}

	hr = pDoc2->setProperty(CComBSTR(L"SelectionLanguage"), CComVariant(CComBSTR(L"XPath")));
	if(hr != S_OK) 
	{
		CEventLog::AddAppLog(_T("CreateResponsePacket.SelectionLanguage"), hr, EVENTLOG_ERROR_TYPE);
		return hr;
	}

	hr = CXMLUtil::AppendNode(pRequest->m_pXMLDoc, CComBSTR(L"packet"), NULL, &pTempNode);
	if(hr != S_OK) 
	{
		CEventLog::AddAppLog(_T("CreateResponsePacket.AppendNode-packet"), hr, EVENTLOG_ERROR_TYPE);
		return hr;
	}

	hr = CXMLUtil::AppendNode(pTempNode, CComBSTR(L"response"), NULL, &pRequest->m_WorkNode);
	if(hr != S_OK) 
	{
		CEventLog::AddAppLog(_T("CreateResponsePacket.AppendNode-response"), hr, EVENTLOG_ERROR_TYPE);
		return hr;
	}

	return hr;
}

HRESULT CSupportClass::CreateEventPacket(IXMLDOMNode **lpEventNode)
{
	CComPtr<IXMLDOMDocument> pTempDoc;
	HRESULT hr = S_OK;

	hr = CXMLUtil::m_pClassFactory->CreateInstance(NULL,IID_IXMLDOMDocument,(void**)&pTempDoc);
	if(hr != S_OK) return hr;

	CComPtr<IXMLDOMDocument2> pDoc2;
	hr = pTempDoc->QueryInterface(__uuidof(IXMLDOMDocument2),(void**)&pDoc2);
	hr = pDoc2->setProperty(CComBSTR(L"SelectionLanguage"),CComVariant(CComBSTR(L"XPath")));

	return CXMLUtil::AppendNode(pTempDoc,CComBSTR(L"packet"),NULL,lpEventNode);
}


void CSupportClass::z_ReceiveFileFirstStep(long PacketSize)
{
	GET_ISAPI_REQUEST();
	HRESULT hr = S_OK;
	CComBSTR bsFID;
	CComPtr<IXMLDOMNode> pFileNode;
	CComPtr<IXMLDOMElement> pFileElement;

	if(!(pRequest->m_bsCommand == CComBSTR(L"c_send_file")))
		throw(ERR_WRONG_XML);

	hr = pRequest->m_WorkNode->selectSingleNode(CComBSTR(L"file"), &pFileNode);
	CHECK_HR_THROW(hr, ERR_WRONG_XML);

	long nSize;
	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode, CComBSTR(L"file/size"), &nSize);
	CHECK_HR_THROW(hr, ERR_WRONG_XML);

	if(((DWORD)(PacketSize + nSize + 5)) != pRequest->m_pECB->cbTotalBytes)
		CHECK_HR_THROW(E_FAIL, ERR_UNKNOW);

	hr = pFileNode->QueryInterface(IID_IXMLDOMElement, (void**)&pFileElement);
	CHECK_HR_THROW(hr, ERR_WRONG_XML);

	GUID SID;
	CoCreateGuid(&SID);

	OLECHAR oleTempFID[90];
	StringFromGUID2(SID, oleTempFID, 90);
	oleTempFID[37] = L'\0';
	bsFID = CComBSTR(oleTempFID + 1);

	hr = pFileElement->setAttribute(CComBSTR(L"fid"), CComVariant(bsFID));
	CHECK_HR_THROW(hr, ERR_WRONG_XML);

	pRequest->m_szFileORurl = new CHAR[600];
	pRequest->m_bsFID = bsFID;
}



void CSupportClass::z_SendFileFirstStep()
{
	GET_ISAPI_REQUEST();
	CComBSTR bsFID;
	HRESULT hr = S_OK;

	//UNPACK XML USER
	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode, CComBSTR(L"file/@fid"), &bsFID);
	CHECK_HR_THROW(hr, ERR_WRONG_XML);

	TCHAR szServerName[600];
	DWORD size = 600;
	pRequest->m_pECB->GetServerVariable(pRequest->m_pECB, "SERVER_NAME", (LPVOID)szServerName, &size);

	// Oleg Zhuk SSL Addon [9/13/2004]
	TCHAR szHTTPS[20] = _T("");
	size = 20;
	pRequest->m_pECB->GetServerVariable(pRequest->m_pECB, "HTTPS", (LPVOID)szHTTPS, &size);

	// Oleg Zhuk Port Addon [2009-01-32]
	TCHAR szPort[10] = _T("");
	size = 10;
	pRequest->m_pECB->GetServerVariable(pRequest->m_pECB, "SERVER_PORT", (LPVOID)szPort, &size);

	pRequest->m_szFileORurl = new CHAR[600];

	// Artyom: OLE2A was replaced with CW2A
	{
		CW2A ansiFileId((BSTR)bsFID);
		LPCSTR szFileId = (LPCSTR)ansiFileId;
		if(_tcsicmp(szHTTPS, _T("on")) == 0)
			sprintf_s(pRequest->m_szFileORurl, 600, "https://%s:%s/BinaryStorage.aspx?fid=%s", szServerName, szPort, szFileId);
		else
			sprintf_s(pRequest->m_szFileORurl, 600, "http://%s:%s/BinaryStorage.aspx?fid=%s", szServerName, szPort, szFileId);
		// SSL Addon [9/13/2004]
	}

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();
}

HRESULT CSupportClass::InitSPName(void)
{
	//m_bs_OM_AUTHENTICATION = _bstr_t(L"OM_AUTHENTICATION");
	m_bs_OM_GET_ID_AND_SALT_BY_LOGIN = _bstr_t(L"OM_GET_ID_AND_SALT_BY_LOGIN");
	m_bs_OM_ADD_ACTIVE_USER = _bstr_t(L"OM_ADD_ACTIVE_USER");
	m_bs_OM_GET_USER_INFO = _bstr_t(L"OM_GET_USER_INFO");
	m_bs_OM_CHECK_AUDIT = _bstr_t(L"OM_CHECK_AUDIT");
	m_bs_OM_LOGOFF = _bstr_t(L"OM_LOGOFF");
	m_bs_OM_CHANGE_STATUS = _bstr_t(L"OM_CHANGE_STATUS");
	m_bs_OM_SEND_MESSAGE = _bstr_t(L"OM_SEND_MESSAGE");
	m_bs_OM_ADD_AUTH_REPLY = _bstr_t(L"OM_ADD_AUTH_REPLY");
	m_bs_OM_LOAD_LIST = _bstr_t(L"OM_LOAD_LIST");
	m_bs_OM_LOAD_AUTH_LIST = _bstr_t(L"OM_LOAD_AUTH_LIST");
	m_bs_OM_LOAD_FILES = _bstr_t(L"OM_LOAD_FILES");
	m_bs_OM_LOAD_SIDS = _bstr_t(L"OM_LOAD_SIDS");
	m_bs_OM_SYNC_HISTORY = _bstr_t(L"OM_SYNC_HISTORY");
	m_bs_OM_ADD_AUTH_REQUEST = _bstr_t(L"OM_ADD_AUTH_REQUEST");
	m_bs_OM_ADD_USER = _bstr_t(L"OM_ADD_USER");
	m_bs_OM_CONFIRM_MESSAGE = _bstr_t(L"OM_CONFIRM_MESSAGE");
	m_bs_OM_CONFIRM_FILE = _bstr_t(L"OM_CONFIRM_FILE");
	m_bs_OM_DELETE_AUTH_REQUEST = _bstr_t(L"OM_DELETE_AUTH_REQUEST");
	m_bs_OM_LOAD_LIST_REV = _bstr_t(L"OM_LOAD_LIST_REV");
	m_bs_OM_CHECK_IGNORE = _bstr_t(L"OM_CHECK_IGNORE");
	m_bs_OM_ADD_FILE = _bstr_t(L"OM_ADD_FILE");
	m_bs_OM_DELETE_USER = _bstr_t(L"OM_DELETE_USER");
	m_bs_OM_LOAD_ACTIVE_USERS_BY_ROLE = _bstr_t(L"OM_LOAD_ACTIVE_USERS_BY_IMGROUP"); // Was Modified [1/7/2004]
	m_bs_OM_LOAD_MESS = _bstr_t(L"OM_LOAD_MESS");
	m_bs_OM_LOAD_AUTH_LIST_REV = _bstr_t(L"OM_LOAD_AUTH_LIST_REV");	


	m_bs_event = CComBSTR(L"event");
	m_bs_users = CComBSTR(L"users");
	m_bs_chats = CComBSTR(L"chats");
	m_bs_messages = CComBSTR(L"messages");
	m_bs_files = CComBSTR(L"files");
	m_bs_sessions = CComBSTR(L"sessions");

	return S_OK;
}

HRESULT CSupportClass::InitCommandNames(void)
{
	m_bs_c_message = CComBSTR(L"c_message");
	m_bs_c_send_file = CComBSTR(L"c_send_file");
	m_bs_c_delete_user = CComBSTR(L"c_delete_user");
	m_bs_c_del_user_r = CComBSTR(L"c_del_user_r");
	m_bs_c_load_list = CComBSTR(L"c_load_list");
	m_bs_c_confirm_message = CComBSTR(L"c_confirm_message");
	m_bs_c_confirm_promo = CComBSTR(L"c_confirm_promo");
	m_bs_c_confirm_file = CComBSTR(L"c_confirm_file");
	m_bs_c_change_status = CComBSTR(L"c_change_status");
	m_bs_c_details = CComBSTR(L"c_details");
	m_bs_c_logoff = CComBSTR(L"c_logoff");
	m_bs_c_add_user = CComBSTR(L"c_add_user");
	m_bs_c_add_user_r = CComBSTR(L"c_add_user_r");

	m_bs_c_ch_create = CComBSTR(L"c_ch_create");
	m_bs_c_ch_edit   = CComBSTR(L"c_ch_edit");
	m_bs_c_ch_leave  = CComBSTR(L"c_ch_leave");
	m_bs_c_ch_invite = CComBSTR(L"c_ch_invite");
	m_bs_c_ch_accept = CComBSTR(L"c_ch_accept");
	m_bs_c_ch_status = CComBSTR(L"c_ch_status");
	m_bs_c_ch_message = CComBSTR(L"c_ch_message");
	return S_OK;
}

BOOL CSupportClass::z_ch_create(void)
{
	GET_ISAPI_REQUEST();
	HRESULT						hr = S_OK;
	CComBSTR					bsCID,bsName,bsDescr;

	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	//CHGLOBAL					hGlobal;
	//DWORD						dwSize  = 0;
	//PBYTE						pBuffer = NULL;


	LONG ltime = GetGMTtime_t(); 
	//LONG DB_time = 0;	

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/@cid"),&bsCID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/name"),&bsName);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/descr"),&bsDescr);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	//CreateChatFolder(pRequest->m_nUserID,bsCID);
	/*
	IBN_CREATE_CHAT
	@CUID as char(36) ,
	@NAME as nvarchar(50) ,
	@DESC as nvarchar(1000) ,
	@OWNER_ID as int ,
	@BEGIN_TIME as int

	CUID, [NAME], [DESC], OWNER_ID
	*/
	m_ParamArray.RemoveAll();
	m_ParamArray.AddGUID((BSTR)bsCID);
	m_ParamArray.Add(adVarWChar,adParamInput,50,_variant_t(bsName));
	m_ParamArray.Add(adVarWChar,adParamInput,1000,_variant_t(bsDescr));
	m_ParamArray.AddLong(pRequest->m_nUserID);
	m_ParamArray.AddLong(ltime);

	hr = CreateResponsePacket();
	CHECK_HR_THROW(hr,ERR_UNNKOWN_XML);

	CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_CREATE_CHAT"),RecSet,&m_ParamArray);

	if(CADOUtil::CheckRecordSetState(RecSet))
	{	
		CRS2XML::Parse(RecSet, pRequest->m_WorkNode, g_r2x_chat_full);
		RecSet->Close();
		RecSet = NULL;
	}
	else
		throw ERR_SQL_UNKNOWN_PROBLEM;

	return 0;
}

BOOL CSupportClass::z_ch_edit(void)
{
	GET_ISAPI_REQUEST();
	HRESULT						hr = S_OK;
	CComBSTR					bsCID,bsName,bsDescr;

	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;

	//LONG ltime = GetGMTtime_t(); 
	//LONG DB_time = 0;	

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/@cid"),&bsCID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/name"),&bsName);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/descr"),&bsDescr);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	/*IBN_EDIT_CHAT
	@CUID as char(36) ,
	@NAME as nvarchar(50) ,
	@DESC as nvarchar(1000) ,
	@OWNER_ID as int*/

	hr = CreateResponsePacket();
	CHECK_HR_THROW(hr,ERR_UNNKOWN_XML);

	m_ParamArray.RemoveAll();
	m_ParamArray.AddGUID((BSTR)bsCID);
	m_ParamArray.Add(adVarWChar,adParamInput,50,_variant_t(bsName));
	m_ParamArray.Add(adVarWChar,adParamInput,1000,_variant_t(bsDescr));
	m_ParamArray.AddLong(pRequest->m_nUserID);

	CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_EDIT_CHAT"),RecSet,&m_ParamArray);

	if(CADOUtil::CheckRecordSetState(RecSet))
	{	
		CRS2XML::Parse(RecSet, pRequest->m_WorkNode, g_r2x_chat_full);
		RecSet->Close();
		RecSet = NULL;
	}
	else
		throw ERR_SQL_UNKNOWN_PROBLEM;
	return 0;
}

BOOL CSupportClass::z_ch_leave(void)
{
	GET_ISAPI_REQUEST();
	HRESULT						hr = S_OK;
	CComBSTR					bsCID;
	CComPtr<IXMLDOMNode>		pNode;
	//LONG ltime = GetGMTtime_t(); 
	//LONG DB_time = 0;	

	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	HGLOBAL						hGlobal = NULL;
	DWORD						dwSize = 0;

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/@cid"),&bsCID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	//DeleteChatUser(pRequest->m_nUserID,bsCID);
	/*IBN_LEAVE_CHAT
	@CUID as char(36),
	@USER_ID as int*/
	m_ParamArray.RemoveAll();
	m_ParamArray.AddGUID((BSTR)bsCID);
	m_ParamArray.AddLong(pRequest->m_nUserID);

	CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LEAVE_CHAT"),RecSet,&m_ParamArray);

	hr = CreateEventPacket(&pNode);
	CHECK_HR_THROW(hr,ERR_UNNKOWN_XML);

	if(CADOUtil::CheckRecordSetState(RecSet))
	{	
		CRS2XML::ParseMulti(RecSet, pNode, g_r2x_chat_leave,m_bs_event);
		RecSet->Close();
		RecSet = NULL;

		hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
		CHECK_HR_THROW(hr,ERR_UNKNOW);

		SendChatBroadcast(bsCID,(PBYTE)GlobalLock(hGlobal),dwSize);

		GlobalUnlock(hGlobal);
		GlobalFree(hGlobal);
		pNode.Release();
	}
	else
		throw ERR_SQL_UNKNOWN_PROBLEM;

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();
	return 0;
}
BOOL CSupportClass::z_ch_invite(void)
{
	GET_ISAPI_REQUEST();
	HRESULT						hr = S_OK;
	CComBSTR					bsCID,bsInvitation;
	LONG						nToID = 0;
	CComPtr<IXMLDOMNode>		pNode,pEventNode,pEventNodeBroadCast, pTemp;
	PBYTE						pBuffer;
	CComBSTR					pTest;

	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	CHGLOBAL						m_hGlobal = NULL;
	DWORD						dwSize = 0;

	//LONG ltime = GetGMTtime_t(); 
	//LONG DB_time = 0;	

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/@cid"),&bsCID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"body"),&bsInvitation);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = pRequest->m_WorkNode->selectSingleNode(CComBSTR(L"recipients/user"),&pNode);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	while(pNode != NULL)
	{
		hr = CXMLUtil::GetValueByPath(pNode,CComBSTR(L"@id"),&nToID);
		if(hr != S_OK || nToID == 0)
		{
			hr = pNode->get_nextSibling(&pTemp);
			if(hr != S_OK) break;
			pNode.Release();
			pNode = pTemp;
			pTemp.Release();
			continue;
		}

		/*IBN_INVITE_CHAT_USER
		@CUID as char(36) ,
		@FROM_USER_ID as int ,
		@TO_USER_ID as int ,
		@INVITE_TEXT as nvarchar(1024)*/

		m_ParamArray.RemoveAll();
		m_ParamArray.AddGUID((BSTR)bsCID);
		m_ParamArray.AddLong(pRequest->m_nUserID);
		m_ParamArray.AddLong(nToID);
		m_ParamArray.Add(adVarWChar,adParamInput,1024,_variant_t(bsInvitation));









		try
		{
			CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_INVITE_CHAT_USER"),RecSet,&m_ParamArray);
		}
		catch(LONG err)
		{
			if(err == ERR_UNABLE_CREATE_CONN)
				throw err;
			RecSet = NULL;
		}
		catch(...)
		{RecSet = NULL;};


		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			//if(pEventNode == NULL)
			//{
			//FieldPtr field = RecSet->Fields->GetItem(_bstr_t(L"send_time"));
			//DB_time = field->Value.lVal;
			pEventNode.Release();
			pEventNodeBroadCast.Release();

			CreateEventPacket(&pEventNode);
			CreateEventPacket(&pEventNodeBroadCast);

			CRS2XML::ParseMulti(RecSet,pEventNode , g_r2x_chat_invite,CComBSTR(L"event"));
			CRS2XML::ParseMulti(RecSet,pEventNodeBroadCast , g_r2x_chat_invite_broadcast,CComBSTR(L"event"));

			//}

			try
			{

				//if(DB_time == ltime)
				//{
				hr = CXMLUtil::XML2HGlobal(pEventNode,&m_hGlobal,&dwSize);
				CHECK_HR_THROW(hr,ERR_UNKNOW);
				pBuffer = (PBYTE)m_hGlobal.Lock();
				try
				{
					g_ActiveSessions.SendEvent(nToID, pBuffer, dwSize - 2);
				}
				catch(...){}

				m_hGlobal.UnLock();
				m_hGlobal.Free();

				hr = CXMLUtil::XML2HGlobal(pEventNodeBroadCast,&m_hGlobal,&dwSize);
				CHECK_HR_THROW(hr,ERR_UNKNOW);
				pBuffer = (PBYTE)m_hGlobal.Lock();
				SendChatBroadcast(bsCID,pBuffer,dwSize);
				m_hGlobal.UnLock();
				m_hGlobal.Free();
				//}
			} 
			catch(...){}



			RecSet->Close();
			RecSet = NULL;
		}

		hr = pNode->get_nextSibling(&pTemp);
		if(hr != S_OK) break;
		pNode.Release();
		pNode = pTemp;
		pTemp.Release();
	}  

	pRequest->m_pXMLDoc.Release();
	pRequest->m_WorkNode.Release();
	return 0;
}
BOOL CSupportClass::z_ch_accept(void)
{
	GET_ISAPI_REQUEST();
	HRESULT						hr = S_OK;
	CComBSTR					bsCID,bsResult;
	LONG						nResult;

	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	CComPtr<IXMLDOMNode>		pNode;
	HGLOBAL						hGlobal = NULL;
	DWORD						dwSize = 0;

	//LONG ltime = GetGMTtime_t(); 
	//LONG DB_time = 0;	

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/@cid"),&bsCID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"result"),&bsResult);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	if(bsResult == CComBSTR(L"accept"))
		nResult = 1;
	else
		if(bsResult == CComBSTR(L"deny"))
			nResult = 0;
		else
			throw (long)ERR_WRONG_XML;

	/*
	IBN_CHAT_USER_REPLY
	@CUID char(36),
	@USER_ID INT,
	@ACCEPTED BIT
	*/
	m_ParamArray.RemoveAll();
	m_ParamArray.AddGUID((BSTR)bsCID);
	m_ParamArray.AddLong(pRequest->m_nUserID);
	m_ParamArray.AddLong(nResult);

	CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_CHAT_USER_REPLY"),RecSet,&m_ParamArray);

	hr = CreateEventPacket(&pNode);
	CHECK_HR_THROW(hr,ERR_UNNKOWN_XML);

	if(CADOUtil::CheckRecordSetState(RecSet))
	{	
		CRS2XML::ParseMulti(RecSet, pNode, g_r2x_chat_accept, m_bs_event);

		hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
		CHECK_HR_THROW(hr,ERR_UNKNOW);

		SendChatBroadcast(bsCID,(PBYTE)GlobalLock(hGlobal),dwSize);

		GlobalUnlock(hGlobal);
		GlobalFree(hGlobal);
		pNode.Release();
		RecSet->Close();
		RecSet = NULL;
	}
	else
		throw ERR_SQL_UNKNOWN_PROBLEM;

	if(nResult == 1)
	{
		hr = CreateResponsePacket();
		CHECK_HR_THROW(hr,ERR_UNKNOW);

		/*IBN_GET_CHAT_INFO
		@CUID char(36)
		*/

		m_ParamArray.RemoveAll();
		m_ParamArray.AddGUID((BSTR)bsCID);

		CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_GET_CHAT_INFO"),RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{	
			CRS2XML::Parse(RecSet, pRequest->m_WorkNode, g_r2x_chat_full);
			RecSet->Close();
			RecSet = NULL;
		}
		//AddChatUser(pRequest->m_nUserID,bsCID);
	}
	else
	{
		pRequest->m_pXMLDoc.Release();
		pRequest->m_WorkNode.Release();
	}
	return 0;
}
BOOL CSupportClass::z_ch_status(void)
{
	GET_ISAPI_REQUEST();
	HRESULT						hr = S_OK;

	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	CComPtr<IXMLDOMNode>		pNode;
	HGLOBAL						hGlobal = NULL;
	DWORD						dwSize = 0;
	LONG ltime = GetGMTtime_t();
	CComBSTR					bsCID;
	LONG						nStatus = 0, nParam = 0;

	hr = CXMLUtil::GetTextByPath(pRequest->m_WorkNode,CComBSTR(L"chat/@cid"),&bsCID);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"status"),&nStatus);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	hr = CXMLUtil::GetValueByPath(pRequest->m_WorkNode,CComBSTR(L"param"),&nParam);
	CHECK_HR_THROW(hr,ERR_WRONG_XML);

	if(nStatus == 0)
		nParam = 0;

	if(!(nStatus == 0 || nStatus == 1))
		throw((long)ERR_WRONG_XML);

	/* IBN_CHANGE_CHAT_STATUS 
	@CUID char(36),
	@USER_ID INT,
	@USER_STATUS tinyint,
	@RETVAL INT OUTPUT
	*/
	m_ParamArray.RemoveAll();
	m_ParamArray.AddGUID((BSTR)bsCID);
	m_ParamArray.AddLong(pRequest->m_nUserID);
	m_ParamArray.AddLong(nStatus);
	m_ParamArray.AddLong(ltime);

	if(1 == CADOUtil::RunSP_ReturnLong(_bstr_t(L"IBN_CHANGE_CHAT_STATUS"),&m_ParamArray))
	{
		/*IBN_GET_CHAT_USER_INFO
		@CUID as char(36),
		@USER_ID as int*/
		m_ParamArray.RemoveAll();
		m_ParamArray.AddGUID((BSTR)bsCID);
		m_ParamArray.AddLong(pRequest->m_nUserID);
		CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_GET_CHAT_USER_INFO")
			,RecSet
			,&m_ParamArray);

		hr = CreateEventPacket(&pNode);
		CHECK_HR_THROW(hr,ERR_UNKNOW);

		CRS2XML::ParseMulti(RecSet, pNode, g_r2x_chat_status,m_bs_event);

		hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&dwSize);
		CHECK_HR_THROW(hr,ERR_UNKNOW);

		SendChatBroadcast(bsCID,(PBYTE)GlobalLock(hGlobal),dwSize);

		GlobalUnlock(hGlobal);
		GlobalFree(hGlobal);
		pNode.Release();

		if(nStatus == 1)
		{
			CreateResponsePacket();
			/*IBN_LOAD_CHAT_USERS
			@CUID as char(36),
			@USER_STATUS as bit*/
			m_ParamArray.RemoveAll();
			m_ParamArray.AddGUID((BSTR)bsCID);
			m_ParamArray.AddLong(0);
			CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LOAD_CHAT_USERS")
				,RecSet
				,&m_ParamArray);


			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_userBasicInfo,CComBSTR(L"users"));
				RecSet->Close();
				RecSet = NULL;
			}

			if(nParam > 0)
			{
				/*IBN_LOAD_CHAT_MESS
				@CUID as char(36),
				@COUNT as int*/
				m_ParamArray.RemoveAll();
				m_ParamArray.AddGUID((BSTR)bsCID);
				m_ParamArray.AddLong(nParam);
				CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LOAD_CHAT_MESS")
					,RecSet
					,&m_ParamArray);


				if(CADOUtil::CheckRecordSetState(RecSet))
				{
					CRS2XML::ParseList(RecSet, pRequest->m_WorkNode, g_r2x_ChatMessLog,CComBSTR(L"log"));
					//					pRequest->m_WorkNode->get_text(&bsCID);
					RecSet->Close();
					RecSet = NULL;
				}
			}			
		}
		else
		{
			pRequest->m_pXMLDoc.Release();
			pRequest->m_WorkNode.Release();
		}
	}

	return 0;
}
BOOL CSupportClass::z_ch_message(void)
{
	GET_ISAPI_REQUEST();
	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;

	//HRESULT						hr = S_OK;

	return 0;
}

int CSupportClass::SendChatBroadcast(CComBSTR bsCID, PBYTE pBuff, int dwSize)
{
	_RecordsetPtr				RecSet;
	CParamArray					m_ParamArray;
	try
	{
		try
		{	
			/*IBN_LOAD_CHAT_USERS
			@CUID as char(36),
			@USER_STATUS as bit*/

			m_ParamArray.AddGUID(bsCID);
			m_ParamArray.AddLong(1);
			CADOUtil::RunSP_ReturnRS(_bstr_t(L"IBN_LOAD_CHAT_USERS")
				,RecSet
				,&m_ParamArray);
		}
		catch(...)
		{
		}

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			do
			{
				_variant_t valID = RecSet->Fields->GetItem(_bstr_t(_T("user_id")))->GetValue();

				try
				{
					g_ActiveSessions.SendEvent((long)valID, pBuff, dwSize - 2);
				}
				catch(...){}
				RecSet->MoveNext();
			}
			while(!RecSet->EndOfFile);

			RecSet->Close();
			RecSet = NULL;
		}
	}
	catch(...)
	{
	}
	return 0;
}

HRESULT CSupportClass::UpdateUserStatus(LONG UserID, LONG Status)
{
	if(UserID == NULL)
	{
		GET_ISAPI_REQUEST();
		UserID = pRequest->m_nUserID;
	}

	CComPtr<IXMLDOMNode>  pNode;
	CParamArray			  m_ParamArray;
	_RecordsetPtr		  RecSet;
	PBYTE				  buff = NULL;
	DWORD				  size = 0;
	HRESULT				  hr = S_OK;
	try
	{
		//============================================================
		//Get Self Info

		/*OM_GET_USER_INFO 
		@USER_ID INT,
		@INFO_TYPE INT*/
		m_ParamArray.AddLong(UserID);
		m_ParamArray.AddLong(1);
		m_ParamArray.AddLong(GetGMTtime_t());

		CADOUtil::RunSP_ReturnRS(m_bs_OM_GET_USER_INFO,RecSet,&m_ParamArray);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			FieldPtr field = RecSet->Fields->GetItem(_bstr_t(L"status"));
			if(Status == 2) Status = 0;

			field->put_Value(_variant_t(Status));

			if(S_OK != CreateEventPacket(&pNode))
				return FALSE;

			CRS2XML::ParseMulti(RecSet, pNode, g_r2x_userBasicInfo,m_bs_event);
			RecSet->Close();
			RecSet = NULL;
		}
		else
			return E_FAIL;

		//===========================================================
		//Send Event
		HGLOBAL hGlobal;
		hr = CXMLUtil::XML2HGlobal(pNode,&hGlobal,&size);
		if(hr != S_OK) return E_FAIL;


		try
		{
			buff = (PBYTE) GlobalLock(hGlobal);

			/*OM_LOAD_LIST_REV 
			@USER_ID INT,
			@LIST_TYPE TINYINT*/
			m_ParamArray.RemoveAll();
			m_ParamArray.AddLong(UserID);
			m_ParamArray.AddLong(1);

			CADOUtil::RunSP_ReturnRS(m_bs_OM_LOAD_LIST_REV,RecSet,&m_ParamArray);
			m_ParamArray.RemoveAll();

			if(CADOUtil::CheckRecordSetState(RecSet))
			{
				while(!RecSet->GetEndOfFile())
				{
					try
					{
						_variant_t id = RecSet->Fields->GetItem(_bstr_t(L"USER_ID"))->GetValue();	
						g_ActiveSessions.SendEvent((long)id,buff,size-2);

					}catch(...){};
					RecSet->MoveNext();
				}
				RecSet->Close();
				RecSet = NULL;
			} 
		}
		catch(...)
		{
		}

		GlobalUnlock(hGlobal);
		GlobalFree(hGlobal);
		return S_OK;
	}
	catch(...)
	{}
	return E_FAIL;
}
/*
int CSupportClass::CreateChatFolder(int UserID, BSTR bsCID)
{
_RecordsetPtr	  RecSet;
CParamArray		  m_ParamArray;
_variant_t		  vDomain,vUserLogin;
CComBSTR		  bsPath(L"Conferences\\");

CComPtr<IManager> pSPManager;
HRESULT			  hr = S_OK;

m_ParamArray.RemoveAll();
m_ParamArray.AddLong(UserID);
CADOUtil::RunSP_ReturnRS(_bstr_t(L"OM_GET_LOGIN_DOMAIN"),RecSet,&m_ParamArray);

if(CADOUtil::CheckRecordSetState(RecSet))
{	
vUserLogin = RecSet->Fields->GetItem(_bstr_t(_T("login")))->GetValue();
vDomain = RecSet->Fields->GetItem(_bstr_t(_T("domain")))->GetValue();	
RecSet->Close();
}
else
throw ERR_SQL_UNKNOWN_PROBLEM;


hr = CoCreateInstance(CLSID_CManager,NULL,CLSCTX_LOCAL_SERVER,IID_IManager,(void**)&pSPManager); 
CHECK_HR_THROW(hr,ERR_UNKNOW);

bsPath.AppendBSTR(bsCID);

hr = pSPManager->CreateFolderAsSystem(vDomain,CComVariant(bsPath));
if(hr != 0x800700b7)
CHECK_HR_THROW(hr,ERR_UNKNOW);

hr = pSPManager->SetRightsForFolder(CComVariant(bsPath),vUserLogin,vDomain,2);
CHECK_HR_THROW(hr,ERR_UNKNOW);

return 0;
}

int CSupportClass::AddChatUser(int UserID, BSTR bsCID)
{
_RecordsetPtr	  RecSet;
CParamArray		  m_ParamArray;
_variant_t		  vDomain,vUserLogin;
CComBSTR		  bsPath(L"Conferences\\");

CComPtr<IManager> pSPManager;
HRESULT			  hr = S_OK;

m_ParamArray.RemoveAll();
m_ParamArray.AddLong(UserID);
CADOUtil::RunSP_ReturnRS(_bstr_t(L"OM_GET_LOGIN_DOMAIN"),RecSet,&m_ParamArray);

if(CADOUtil::CheckRecordSetState(RecSet))
{	
vUserLogin = RecSet->Fields->GetItem(_bstr_t(_T("login")))->GetValue();
vDomain = RecSet->Fields->GetItem(_bstr_t(_T("domain")))->GetValue();	
RecSet->Close();
}
else
throw ERR_SQL_UNKNOWN_PROBLEM;


hr = CoCreateInstance(CLSID_CManager,NULL,CLSCTX_LOCAL_SERVER,IID_IManager,(void**)&pSPManager); 
CHECK_HR_THROW(hr,ERR_UNKNOW);

bsPath.AppendBSTR(bsCID);

hr = pSPManager->SetRightsForFolder(CComVariant(bsPath),vUserLogin,vDomain,2);
CHECK_HR_THROW(hr,ERR_UNKNOW);

return 0;
}

int CSupportClass::DeleteChatUser(int UserID, BSTR bsCID)
{
_RecordsetPtr	  RecSet;
CParamArray		  m_ParamArray;
_variant_t		  vDomain,vUserLogin;
CComBSTR		  bsPath(L"Conferences\\");

CComPtr<IManager> pSPManager;
HRESULT			  hr = S_OK;

m_ParamArray.RemoveAll();
m_ParamArray.AddLong(UserID);
CADOUtil::RunSP_ReturnRS(_bstr_t(L"OM_GET_LOGIN_DOMAIN"),RecSet,&m_ParamArray);

if(CADOUtil::CheckRecordSetState(RecSet))
{	
vUserLogin = RecSet->Fields->GetItem(_bstr_t(_T("login")))->GetValue();
vDomain = RecSet->Fields->GetItem(_bstr_t(_T("domain")))->GetValue();	
RecSet->Close();
}
else
throw ERR_SQL_UNKNOWN_PROBLEM;


hr = CoCreateInstance(CLSID_CManager,NULL,CLSCTX_LOCAL_SERVER,IID_IManager,(void**)&pSPManager); 
CHECK_HR_THROW(hr,ERR_UNKNOW);

bsPath.AppendBSTR(bsCID);

hr = pSPManager->SetRightsForFolder(CComVariant(bsPath),vUserLogin,vDomain,-1);
CHECK_HR_THROW(hr,ERR_UNKNOW);

return 0;
}
*/