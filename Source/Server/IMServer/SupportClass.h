// SupportClass.h: interface for the CSupportClass class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SUPPORTCLASS_H__DDEB614A_D2CF_4F52_B4DB_69C42B4B457F__INCLUDED_)
#define AFX_SUPPORTCLASS_H__DDEB614A_D2CF_4F52_B4DB_69C42B4B457F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
#include <afxtempl.h>
#include "XMLUtil.h"
#include "ISAPIExt.h"

#define CHECKHR(x) {hr = x; if (FAILED(hr)) {ASSERT(FALSE);goto CleanUp;}}
#define SAFERELEASE(p) {if (p) {(p)->Release(); p = NULL;}}

#define WM_BEGIN_THREAD  WM_USER+211
#define WM_END_THREAD	 WM_USER+212

struct FromToID
{
	FromToID(LONG FromID,LONG ToID)
	{
		m_FromID = FromID;
		m_ToID = ToID;
	}
	LONG m_FromID;
	LONG m_ToID;
};
//////////////////////////////////////////////
// Global Instance to force load/unload of OLE
/*struct InitOle {
	InitOle()  {  CoInitialize(NULL);;}
	~InitOle() {  CoUninitialize();}
};

struct CompletionStruct
{
	PBYTE Buff;
	long  size;
	BOOL  KeepAlive;
};*/
//////////////////////////////////////////////
//Param for Run SP



//////////////////////////////////////////////
//Set of Support Functions for NET & DBSQL
class CSupportClass  
{
public:
	CSupportClass();
	virtual ~CSupportClass();
	
	BOOL Initialize(DWORD& dwCallBackThreadID);
	VOID Terminate();

	BOOL Logic();
public:
//	LPTSTR  szLocalStoragePath;
	
	static HRESULT UpdateUser(LONG UserID,BSTR code, BSTR descr);
	static HRESULT UpdateGroup(LONG GroupID,BSTR code, BSTR descr);
	static HRESULT SendWebMessage(LONG UserID,LONG ToID, BSTR Message);
	static HRESULT UpdateUserStatus(LONG UserID, LONG Status);
	HRESULT LogOffUser(LONG UserId);
protected:
	void x_StatusChanged(long ID, long Status);

	BOOL CanISend(long From_ID, long To_ID);
	BOOL y_EventChangedState(long Status, LONG UserID = NULL);
	DWORD LogThreadID;
	DWORD DelSessionsCallBackID;
	static DWORD WINAPI DelSessionsCallBack(LPVOID param);
	static LONG OpenThread;
	BOOL	m_bIsInit;
private:

public:
	void z_SendFileFirstStep();
	void z_ReceiveFileFirstStep(long PacketSize);
	void z_logon(CISAPIRequest* pRequest);
private:
	BOOL z_DeleteUser();
	BOOL z_ConfirmFile();
	BOOL z_AddAuthRely();
	BOOL z_ConfirmPromo();
	BOOL z_ConfirmMessage();
	BOOL z_LogOff();
	BOOL z_AddUser();
	BOOL z_LoadList();
	BOOL z_NewMessage();
	BOOL z_SentFile();
	BOOL z_ChangeStatus();
	BOOL z_DelAuthReq();
	BOOL z_GetUserInfo();
protected:
	BOOL CheckAuditIsActive(BSTR bsDomain);
	//BOOL BeginFileWrite();
	//BOOL SendMessageTo(LONG UserID, IXMLDOMNode* pNode);
	static HRESULT CreateEventPacket(IXMLDOMNode **lpEventNode);
	HRESULT CreateResponsePacket();
	BOOL    SendOffLineEvents();
private:
	// SP names
	static _bstr_t m_bs_OM_AUTHENTICATION;
	static _bstr_t m_bs_OM_GET_ID_AND_SALT_BY_LOGIN;
	static _bstr_t m_bs_OM_ADD_ACTIVE_USER;
	static _bstr_t m_bs_OM_GET_USER_INFO;
	static _bstr_t m_bs_OM_CHECK_AUDIT;
	static _bstr_t m_bs_OM_LOGOFF;
	static _bstr_t m_bs_OM_CHANGE_STATUS;
	static _bstr_t m_bs_OM_SEND_MESSAGE;
	static _bstr_t m_bs_OM_ADD_AUTH_REPLY;
	static _bstr_t m_bs_OM_LOAD_LIST;
	static _bstr_t m_bs_OM_LOAD_AUTH_LIST;
	static _bstr_t m_bs_OM_LOAD_FILES;
	static _bstr_t m_bs_OM_LOAD_SIDS;
	static _bstr_t m_bs_OM_SYNC_HISTORY;
	static _bstr_t m_bs_OM_ADD_AUTH_REQUEST;
	static _bstr_t m_bs_OM_ADD_USER;
	static _bstr_t m_bs_OM_CONFIRM_MESSAGE;
	static _bstr_t m_bs_OM_CONFIRM_FILE;
	static _bstr_t m_bs_OM_DELETE_AUTH_REQUEST;
	static _bstr_t m_bs_OM_LOAD_LIST_REV;
	static _bstr_t m_bs_OM_CHECK_IGNORE;
	static _bstr_t m_bs_OM_ADD_FILE;
	static _bstr_t m_bs_OM_DELETE_USER;
	static _bstr_t m_bs_OM_LOAD_ACTIVE_USERS_BY_ROLE;
	static _bstr_t m_bs_OM_LOAD_MESS;
	static _bstr_t m_bs_OM_LOAD_AUTH_LIST_REV;
	//protocol elements
	static CComBSTR m_bs_event;
	static CComBSTR m_bs_users;
	static CComBSTR m_bs_chats;
	static CComBSTR m_bs_messages;
	static CComBSTR m_bs_files;
	static CComBSTR m_bs_sessions;
	//Commands name
	CComBSTR m_bs_c_message;
	CComBSTR m_bs_c_send_file;
	CComBSTR m_bs_c_delete_user;
	CComBSTR m_bs_c_del_user_r;
	CComBSTR m_bs_c_load_list;
	CComBSTR m_bs_c_confirm_message;
	CComBSTR m_bs_c_confirm_promo;
	CComBSTR m_bs_c_confirm_file;
	CComBSTR m_bs_c_change_status;
	CComBSTR m_bs_c_details;
	CComBSTR m_bs_c_logoff;
	CComBSTR m_bs_c_add_user;
	CComBSTR m_bs_c_add_user_r;

	CComBSTR m_bs_c_ch_create;
	CComBSTR m_bs_c_ch_leave;
	CComBSTR m_bs_c_ch_edit;
	CComBSTR m_bs_c_ch_invite;
	CComBSTR m_bs_c_ch_accept;
	CComBSTR m_bs_c_ch_status;
	CComBSTR m_bs_c_ch_message;

public:
	HRESULT InitSPName(void);
	HRESULT InitCommandNames(void);
	BOOL z_ch_create(void);
	BOOL z_ch_edit(void);
	BOOL z_ch_leave(void);
	BOOL z_ch_invite(void);
	BOOL z_ch_accept(void);
	BOOL z_ch_status(void);
	BOOL z_ch_message(void);
	static int SendChatBroadcast(CComBSTR bsCID, PBYTE pBuff, int dwSize);
	int CreateChatFolder(int UserID, BSTR bsCID);
	int AddChatUser(int UserID, BSTR bsCID);
	int DeleteChatUser(int UserID, BSTR bsCID);
};

#endif // !defined(AFX_SUPPORTCLASS_H__DDEB614A_D2CF_4F52_B4DB_69C42B4B457F__INCLUDED_)
