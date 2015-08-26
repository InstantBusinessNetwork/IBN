// CommandQueue.h: interface for the CCommandQueue class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_COMMANDQUEUE_H__23BBDF2E_E574_4036_B56A_C277CA30E6CC__INCLUDED_)
#define AFX_COMMANDQUEUE_H__23BBDF2E_E574_4036_B56A_C277CA30E6CC__INCLUDED_

//#include <msxml2.h>
#include "atl_netlib.h"
#include "message.h"
#include "promo.h"
#include "File.h"
#include "users.h"


#define WRONG_QUEUE			1
#define AlREADY_IN_QUEUE	2
#define WRONG_PARAM			3
#define UNKNOWN_PROBLEM		4


class CCommandQueue  
{
public:
	CCommandQueue();
	virtual ~CCommandQueue();

private:
	CRITICAL_SECTION	CS_Queue;
	int					m_Handle;

public:
	void UnPackSetChatStatus(long handle, IUsers** pUsers, BSTR* bsLog);
	VOID DeleteAllCommands(void *pWorkClass);
	
	HRESULT Init();
	HRESULT OutPutQueueInit();
	HRESULT InPutQueueInit();
	void GetSID(TCHAR* szSID);

	void UnPackEvents(void* pWorkClass);
	void UnPackList(IUnknown** pList, LPVOID pWorkClass, WPARAM handle, long& outListType, BSTR* pString);

	void UnPackUserDetails(long Handle, IUser** pUser);
	void UnPackTime(long Handle, long& Time);
	
	void UnPackSelfInfo(sUser& pUser, DWORD handle);
	void UnPackUser(sUser& user, CComPtr<IXMLDOMNode> pUserNode);
	void UnPackMessage(sMessage& message, CComPtr<IXMLDOMNode> pMNode);
	void UnPackPromo(sPromo& promo, CComPtr<IXMLDOMNode> pPNode);
	void UnPackFile(sFile& file, CComPtr<IXMLDOMNode> pNode);
	void UnPackSession(slocalSID& session, CComPtr<IXMLDOMNode> pNode);
	void UnPackChat(LONG Handle, IChat** pChat);
	void UnPackChat(sChat& chat, CComPtr<IXMLDOMNode> pNode);

	bool CheckQueueItem(CComBSTR CommandType,IXMLDOMDocument** pComm, DWORD& dwHandle, DWORD& dwType,BSTR *FileName = NULL, long* hBackWind = 0, long* size = 0, BSTR *bstrSID = NULL);
	void  AddEvent(IXMLDOMNode *pNode);
	void  AddAnswer(long Handle, long Type, IXMLDOMNode* pNode);
	LONG DeleteCommand(DWORD handle);
	BOOL  DeleteAnswer(DWORD handle);

	DWORD x_LogOn_Inter();
	DWORD x_LoadMessages(BSTR SID);
	DWORD x_LoadSIDs(long From, long To);
	DWORD x_SearchUser(IUser* pUser);
	DWORD x_UserDetails(long ID, long type);
	DWORD x_DeleteUserR(long ID);
	DWORD x_DeleteUser(long ID, long listtype);
	DWORD x_ChangeStatus(long Status);
	DWORD x_LoadList(ltListType ListType);
	DWORD x_LogOn(CComBSTR SID, CComBSTR UserName, CComBSTR Password, long Status);
	DWORD x_ConfirmFile(BSTR FID, long Flag);
	DWORD x_ConfirmPromo(BSTR PID);
	DWORD x_ConfirmMessage(BSTR MID);
	DWORD x_Promo(CPromo* pPromo, long& Handle);
	DWORD x_SendFile(CFile* pFile, long& Handle);
	DWORD x_ReceiveFile(CFile* pFile, long& Handle);
	DWORD x_Message(CMessage* pMessage, long& Handle);
	DWORD x_LogOff();
	DWORD x_AddUser(long ID, long listtype, BSTR body = NULL);
	DWORD x_AddUserR(long ID, long AnswerType);
	DWORD x_LastPromos(long Count);

	DWORD x_ChatCreate(BSTR CID, BSTR Name, BSTR Descr);
	DWORD x_ChatStatus(BSTR CID, LONG Status, LONG Param);
	DWORD x_ChatEdit(BSTR CID, BSTR Name, BSTR Descr);
	DWORD x_ChatInvite(sChat* pChat,BSTR bsInvitation);
	DWORD x_ChatAccept(BSTR CID, LONG Result);
	DWORD x_ChatLeave(BSTR CID);

	void  UnlockQueue();
	void  LockQueue();
	

	//OutPut Queue
	CComPtr<IXMLDOMNode>     pRootNode;
	CComPtr<IXMLDOMDocument> pDom;
	
	//InPut Queue
	CComPtr<IXMLDOMNode>	 pAnswerRootNode;
	CComPtr<IXMLDOMDocument> pAnswerDom;

	CComBSTR			m_bstrLogin;
	CComBSTR			m_bstrPassword;
	LONG				m_Status;
	LONG				LastStatus;
	LPVOID				lpSession;
};

#endif // !defined(AFX_COMMANDQUEUE_H__23BBDF2E_E574_4036_B56A_C277CA30E6CC__INCLUDED_)
