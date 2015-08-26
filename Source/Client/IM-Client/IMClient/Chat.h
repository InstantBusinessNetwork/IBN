// Chat.h: interface for the CChat class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_CHAT_H__E733705A_5AAB_4B44_A20D_627833E824E6__INCLUDED_)
#define AFX_CHAT_H__E733705A_5AAB_4B44_A20D_627833E824E6__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "UserCollection.h"

class CChat  
{
public:
	CChat(const CChat &srcChat);
	CChat(IChat* pChat);
	CChat();
	virtual ~CChat();
public:
	IChat* Detach();
	void Attach(IChat* pChat);
public:
	void SetInterface(IChat *pChat);
	void RefreshColors(LPCTSTR UserRole, int UserId);
	HWND GetChatWindow();
	HWND SetChatWindow(HWND hWnd);
	void AddNewEvent(DWORD dwId, BSTR FirstName, BSTR LastName, BSTR bsMessage, DWORD dwTime, DWORD dwColor);
	LPDISPATCH GetMessagesInterface();
	BOOL AddNewMessages(BSTR bsMessages);
	BOOL GetMessages(BSTR* pbsMessages);
	BOOL LoadMessages(LPCTSTR UserRole, int UserId, BSTR bsChatMessages);
	CUserCollection& GetUsers();
	void LoadUser(IUsers* pUsers);
	LONG SetStatus(LONG newVal);
	CString GetShowName() const;
	LONG SetTID(LONG newVal);
	LONG GetTID();
	LONG GetStatus();
	LONG GetTime() const;
	LONG GetOwnerId() const;
	CComBSTR GetDescription() const;
	CComBSTR GetName() const;
	CComBSTR GetId() const;
	HRESULT GetValueBSTR(BSTR ValName, BSTR* pOut) const;
	HRESULT GetValueLONG(BSTR ValName, LONG* pOut) const;
	HRESULT GetValue(BSTR ValName, VARIANT* pOut) const;
	const CComPtr<IChat>& GetInterface() const;
	const CChat& operator=(const CChat &pSrc);
	BOOL operator==(const CChat &pSrc) const;
	BOOL operator!=(const CChat &pSrc) const;
	IChat* operator->() const;
private:
	CComPtr<IChat>				m_pChat;
	CComBSTR					m_bsId;
	LONG						m_lChatStatus;
	LONG						m_lTID;
	CUserCollection				m_ContactList;
	//HWND						m_hWndDelChat;
	//HWND						m_hWndMsgChat;

	CComPtr<IXMLDOMDocument>	m_pHistoryXML;

	
};

#endif // !defined(AFX_CHAT_H__E733705A_5AAB_4B44_A20D_627833E824E6__INCLUDED_)
