#if !defined(AFX_EVENTCONTAINER_H__E873D884_6A00_42C1_B801_D83591C18CF4__INCLUDED_)
#define AFX_EVENTCONTAINER_H__E873D884_6A00_42C1_B801_D83591C18CF4__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// EventContainer.h : header file
//
#include "NetLibTranslator.h"
/////////////////////////////////////////////////////////////////////////////
// CEventContainer command target
/////////////////////////////////////////////////////////////////////////////
/// Функция для Автоматического разМаршалинга Интефейсов из NLT_Container'ов
/// [in] pContainer - Указатель на контейнер.
/// [out]ppv        - Указатель на интерфейс.
/// Возвращает  NLT_OK (0L)           - Все нормально
///             NLT_CONTAINER_ERROR   - Пустой указатель на контейнер
///             NLT_EVENT_NO_SUPPORT  - Данный контейнер не поддерживается :(
///             или Ошибка при работе с Com'oм
HRESULT AutoUnMarchaling(NLT_Container *pContainer,LPUNKNOWN* ppv);
HRESULT AutoUnMarchaling2(NLT_Container *pContainer,LPUNKNOWN* ppv);
HRESULT AutoUnMarchaling3(NLT_Container *pContainer,LPUNKNOWN* ppv);

/////////////////////////////////////////////////////////////////////////////
/// Класс по отлову Событий от ATL_NetLib.dll

class CEventContainer : public CCmdTarget
{
	DECLARE_DYNCREATE(CEventContainer)
public:
	CEventContainer();           // protected constructor used by dynamic creation
    virtual ~CEventContainer();
	// Attributes
public:
	CNetLibTranslator m_NetLibTranslator;
	// Operations
public:
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CEventContainer)
	//}}AFX_VIRTUAL
	
	// Implementation
protected:
	void OneChatAccept(IChat* pChat, IUser* pUser, long Result);
	void OneChatFile(IChat* pChat);
	void OneChatMessage(IChat* pChat, IMessage* pMessage);
	void OneChatLeave(IUser* pUser, IChat* pChat);
	void OneChatInvite(IChat* pChat, IUser* pUser, IUser* pInvitedFriend, LPCTSTR Invitation);
	void OneChatUserStatus(IUser* pUser, IChat* pChat);
	void OnChatCreate(long Handle, IChat* pChat);
	void OnChatStatus(long Handle, IUsers* pUsers, LPCTSTR bsLog);
	void OnChatList(IChats* pChats);
	void OneSysMess(long Code, LPCTSTR Description);
	void OnChangedState(long State,long ErrorType,long ErrorCode);
	void One_ChangedStatus(IUser* pUser);
	void One_Message(IMessage* pMessage);
	void One_Promo(IPromo* pPromo, LPCTSTR PromoAsString);
	void One_File(IFile* pFile);
	void One_Add(IUser* pUser, LPCTSTR bstrMessage);
	void One_AddR(IUser* User, long nResult);
	void One_Reklama(LPCTSTR bstrURL);
	void OnContactList(IUsers* pUsers);
	void OnIgnoreList(long Handle, IUsers* pUsers);
	void OnDetails(long Handle, IUser* pUser,long type);
	void OnCommandOk(long Handle, long AddVal);
	void OnCommandError(long Handle,long ErrorType,long ErrorCode);
	void OnSessionsList(long Handle, IlocalSIDs* plocalSIDs);
	void OnOffLineFiles(long Handle, IFiles* pFiles);
	void OnMessagesList(long Handle, IMessages* pMessages);
	void OnPromosList(long Handle, IPromos* pPromos,LPCTSTR PromoAsString);
	void OnSelfStatus(long Status);
	//void OnDetails(IUser* pUser);
	// Generated message map functions
	//{{AFX_MSG(CEventContainer)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	// Generated OLE dispatch map functions
	//{{AFX_DISPATCH(CEventContainer)
    //}}AFX_DISPATCH
	DECLARE_DISPATCH_MAP()
	DECLARE_INTERFACE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_EVENTCONTAINER_H__E873D884_6A00_42C1_B801_D83591C18CF4__INCLUDED_)
