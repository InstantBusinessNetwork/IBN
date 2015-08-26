// Message.cpp: implementation of the CMessage class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "Message.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMessage::CMessage()
{
//	COfsmessengerDlg* pMainDlg=(COfsmessengerDlg*)AfxGetApp()->m_pMainWnd;
//	m_userRecipient=m_userSender=pMainDlg->m_User;
	m_strChatID="";
	m_strMessageID="";
	m_strMessage="";
	// come file use
	m_strFileName="";
	m_strFileUrl="";
	m_strExpireDate="";
	m_strFileSize="";

}

CMessage::CMessage(CString Sender,CString FileID,CString FileName,CString Size,CString Url)
{
    //m_userSender=Sender;
	m_strMessageID=FileID;
	// come file use
	m_strFileName=FileName;
	m_strFileUrl=Url;
	m_strExpireDate="";
	m_strFileSize=Size;

}

CMessage::~CMessage()
{

}

CComBSTR& CMessage::GetMessage()
{
	return m_strMessage;
}

CUser& CMessage::GetSender()
{
	return m_userSender;
}

void CMessage::SetRecipient(CUser& recip)
{
	m_userRecipient=recip;
}

void CMessage::SetCurentTime()
{
	m_timeSent=CTime::GetCurrentTime ();
}

void CMessage::SetTime(DWORD t)
{
	m_timeSent=(time_t)t;
}

void CMessage::SetMessageID(LPCTSTR lpszMessageID)
{
	m_strMessageID=lpszMessageID;
}

BOOL CMessage::IsChatMessage()
{
	return !m_strChatID.IsEmpty();
}

CMessage& CMessage::operator =(CMessage &src)
{
	m_strChatID=src.m_strChatID;
	m_strMessage=src.m_strMessage;
	m_strMessageID=src.m_strMessageID;
	m_userSender=src.m_userSender;
	m_userRecipient=src.m_userRecipient;
	m_timeSent=src.m_timeSent;
	m_strFileName=src.m_strFileName;
	m_strFileUrl=src.m_strFileUrl;
	m_strExpireDate=src.m_strExpireDate;
	m_strFileSize=src.m_strFileSize ;
	return src;
}

CMessage::CMessage(CMessage &src)
{
	*this=src;
}

CMessage::CMessage(IMessage *pMessage)
{
	IUserPtr pSender = NULL;
	HRESULT hr = pMessage->get_Sender(&pSender);
	
	if(FAILED(hr))
		_com_issue_error(hr);

	if(pSender==NULL)
		_com_issue_error(E_INVALIDARG);

	m_userSender = CUser(pSender);
	
	m_strMessageID      = (char*)pMessage->GetMID();
	m_strMessage        = (char*)pMessage->GetBody();
	m_timeSent          = (time_t)pMessage->Getdate_time();

	m_lID				= pMessage->GetID();
}


void CMessage::SetMessage(BSTR lpszMessage)
{
	m_strMessage=lpszMessage;
}

void CMessage::SetChatID(LPCTSTR lpszChatID)
{
	m_strChatID=lpszChatID;
}


CString CMessage::GetChatID()
{
	return m_strChatID;
}

DWORD CMessage::GetTime()
{
	return m_timeSent.GetTime ();
}

CUser& CMessage::GetRecipient()
{
	return m_userRecipient;
}

CString CMessage::GetRecipientId()
{
	return IsChatMessage()?m_strChatID:m_userRecipient.GetId();
}

BOOL CMessage::IsIncoming()//CUser &currUser)
{
//	COfsmessengerDlg* pMainDlg=(COfsmessengerDlg*)AfxGetApp()->m_pMainWnd;
//	m_userRecipient=m_userSender=pMainDlg->m_User;
	
	if(TRUE)return TRUE;
	else return FALSE;
}	

BOOL CMessage::IsOutgoing()
{
	return FALSE;//!IsIncoming();
}

CString CMessage::GetFileName()
{
	return m_strFileName;
}

CString CMessage::GetFileUrl ()
{
	return m_strFileUrl;
}

CString CMessage::GetDelDate ()
{
	return m_strExpireDate;
}

CString CMessage::GetFileSize ()
{
	return m_strFileSize;
}

CString CMessage::GetMessageID ()
{
	return m_strMessageID;
}


LONG CMessage::GetID()
{
	return m_lID;
}
