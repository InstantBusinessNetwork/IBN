// Message.h: interface for the CMessage class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MESSAGE_H__6665E7A7_1F17_4B5B_86E1_E12F6555CB0F__INCLUDED_)
#define AFX_MESSAGE_H__6665E7A7_1F17_4B5B_86E1_E12F6555CB0F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
#include "User.h"
class CMessage : public CObject  
{
	friend class CXMLClient;
public:
	LONG GetID();
	CString GetSID();
	BOOL     IsOutgoing();
	BOOL     IsIncoming();
	CString  GetRecipientId();
	CUser&   GetRecipient();
	DWORD  GetTime();
	CString  GetChatID();
	void     SetMessage(BSTR lpszMessage);
	void     SetChatID(LPCTSTR lpszChatID);
	void     SetMessageID(LPCTSTR lpszMessageID);
	void     SetCurentTime();
	void     SetTime(DWORD t);

	CMessage();
	CMessage(IMessage *pMessage);
	CMessage(CMessage& src);
	CMessage(CString Sender,CString FileID,CString FileName,CString Size,CString Url);
	virtual ~CMessage();

	CMessage& operator =(CMessage &src);

	BOOL     IsChatMessage();
	void     SetRecipient(CUser& recip);
	CUser&   GetSender();
	CComBSTR&  GetMessage();
	CString  GetMessageID ();
	CString  GetFileName();
	CString  GetFileUrl();
	CString  GetDelDate();
	CString  GetFileSize();

protected:
	CTime    m_timeSent;
	CString  m_strChatID;
	CString  m_strMessageID;
	CComBSTR  m_strMessage;
	CUser    m_userRecipient;
	CUser    m_userSender;
	// come file use
	CString  m_strFileName;
	CString  m_strFileUrl;
	CString  m_strExpireDate;
	CString  m_strFileSize;
	LONG	 m_lID;
};

typedef CList <CMessage *,CMessage *> CMessageList;

#endif // !defined(AFX_MESSAGE_H__6665E7A7_1F17_4B5B_86E1_E12F6555CB0F__INCLUDED_)
