// EventContainer.cpp : implementation file
//

#include "stdafx.h"
#include "MpaWebEvent.h"
//#include "ChildView.h"
#include "WebWindow.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
// CEventContainer

IMPLEMENT_DYNCREATE(CMpaWebEvent, CCmdTarget)

CMpaWebEvent::CMpaWebEvent()
{
	EnableAutomation();
	m_pParent = NULL;
}

CMpaWebEvent::~CMpaWebEvent()
{
}

BEGIN_MESSAGE_MAP(CMpaWebEvent, CCmdTarget)
	//{{AFX_MSG_MAP(CMpaWebEvent)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_DISPATCH_MAP(CMpaWebEvent, CCmdTarget)
	//{{AFX_DISPATCH_MAP(CEventContainer)
//    DISP_FUNCTION_ID(CMpaWebEvent, "", 2, OnBeforeNavigate2,   VT_EMPTY,  VTS_DISPATCH VTS_VARIANT VTS_VARIANT VTS_VARIANT VTS_VARIANT VTS_VARIANT VTS_PBOOL)
//    DISP_FUNCTION_ID(CMpaWebEvent, "", 3, OnCmdBeginPromo,   VT_EMPTY,    VTS_I4 VTS_BSTR)
//	DISP_FUNCTION_ID(CMpaWebEvent, "", 4, OnCmdAddProduct,    VT_EMPTY,    VTS_I4 VTS_BSTR)
//	DISP_FUNCTION_ID(CMpaWebEvent, "", 7, OnCmdAddCompany,       VT_EMPTY,VTS_I4 VTS_BSTR VTS_I4 VTS_BSTR)
//	DISP_FUNCTION_ID(CMpaWebEvent, "", 8, OnCmdCompanyDetails,   VT_EMPTY,VTS_I4 VTS_BSTR VTS_I4 VTS_BSTR)
	DISP_FUNCTION_ID(CMpaWebEvent, "", 1,  OnCmdSetVariable,    VT_EMPTY,VTS_BSTR VTS_BSTR)
	DISP_FUNCTION_ID(CMpaWebEvent, "", 2,  OnCmdGetVariable,    VT_EMPTY,VTS_BSTR VTS_PBSTR)	
    DISP_FUNCTION_ID(CMpaWebEvent, "",  3, OnCmdGetUserID,    VT_EMPTY,VTS_PI4)	
	DISP_FUNCTION_ID(CMpaWebEvent, "",  4, OnCmdGetUserRole,    VT_EMPTY,VTS_PI4)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 5,  OnCmdNewWindow,        VT_EMPTY,VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_BSTR VTS_BSTR VTS_BOOL VTS_BOOL VTS_BOOL)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 6,  OnCmdMainWindowNavigate,    VT_EMPTY,VTS_BSTR VTS_I4)	
    DISP_FUNCTION_ID(CMpaWebEvent, "", 7,  OnCmdLoadPromos,     VT_EMPTY, VTS_PBSTR)
    DISP_FUNCTION_ID(CMpaWebEvent, "", 8,  OnCmdSendPromo,    VT_EMPTY,VTS_I4 VTS_BSTR VTS_BSTR VTS_BSTR VTS_PI4)
	DISP_FUNCTION_ID(CMpaWebEvent, "", 9,  OnCmdCheckStatus,    VT_EMPTY,VTS_I4 VTS_PI4)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 10, OnCmdSendMessage,    VT_EMPTY,VTS_I4 VTS_BSTR VTS_BSTR)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 11, OnCmdSendFile,    VT_EMPTY,VTS_I4 VTS_BSTR VTS_BSTR)		
	DISP_FUNCTION_ID(CMpaWebEvent, "", 12, OnCmdSetProgramSettings,    VT_EMPTY,VTS_I4 VTS_BSTR)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 13, OnCmdGetProgramSettings,    VT_EMPTY,VTS_I4 VTS_PBSTR)
	DISP_FUNCTION_ID(CMpaWebEvent, "", 14, OnCmdAddContact,      VT_EMPTY,VTS_I4 VTS_BSTR VTS_BSTR VTS_BSTR VTS_BSTR VTS_I4 VTS_BSTR VTS_I4 VTS_BSTR)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 15, OnCmdBeginDialogue,    VT_EMPTY,VTS_I4 VTS_BSTR VTS_BSTR VTS_BSTR VTS_BSTR VTS_I4 VTS_BSTR VTS_I4 VTS_BSTR)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 16, OnCmdUserDetails,     VT_EMPTY,VTS_I4 VTS_BSTR VTS_BSTR VTS_BSTR VTS_BSTR VTS_I4 VTS_BSTR VTS_I4 VTS_BSTR)	
//	DISP_FUNCTION_ID(CMpaWebEvent, "", 16, OnCmdProductDetails,   VT_EMPTY,VTS_I4 VTS_BSTR VTS_I4 VTS_BSTR)	
	DISP_FUNCTION_ID(CMpaWebEvent, "", 17, OnCmdCancelOperation,   VT_EMPTY, VTS_I4)
	DISP_FUNCTION_ID(CMpaWebEvent,"", 18, OnShowContextMenu,    VT_EMPTY, VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_I4 VTS_PI4)	
	DISP_FUNCTION_ID(CMpaWebEvent,"", 19, OnCmdDoAction, VT_EMPTY, VTS_BSTR VTS_BSTR VTS_PBSTR)	
	DISP_FUNCTION_ID(CMpaWebEvent,"", 20, OnGetDropTarget, VT_EMPTY, VTS_I4 VTS_PI4)	
// DISP_FUNCTION_ID(CMpaWebEvent,"", 0x, On,    VT_EMPTY, VTS_)	
//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()

// Note: we add support for IID_IEventContainer to support typesafe binding
//  from VBA.  This IID must match the GUID that is attached to the 
//  dispinterface in the .ODL file.
// Необходимо Добавить Интерфейсы Событий к которым вы хотите подключится
// используя IConnectionPoint
BEGIN_INTERFACE_MAP(CMpaWebEvent, CCmdTarget)
    INTERFACE_PART(CMpaWebEvent, __uuidof(_IMpaWebCustomizerEvents), Dispatch)
END_INTERFACE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CEventContainer message handlers

void CMpaWebEvent::OnCmdAddCompany(long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
	//Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdAddCompany(company_id, company_name, role_id, role_name);
}

void CMpaWebEvent::OnCmdAddProduct(long product_id, LPCTSTR product_name)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdAddProduct(product_id, product_name);
}

void CMpaWebEvent::OnCmdAddContact(long user_id, LPCTSTR nick_name, LPCTSTR first_name, LPCTSTR last_name, LPCTSTR email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdAddContact(user_id, nick_name, first_name, last_name, email, company_id, company_name, role_id, role_name);
}

void CMpaWebEvent::OnCmdNewWindow(long x, long y, long cx, long cy, LPCTSTR title, LPCTSTR url, BOOL bModal, BOOL bTopMost,  BOOL bResizable )
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdNewWindow(x, y, cx, cy, title, url, bModal, bTopMost, bResizable);
}

void CMpaWebEvent::OnCmdBeginPromo(long product_id, LPCTSTR product_name)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdBeginPromo(product_id, product_name);
}

void CMpaWebEvent::OnCmdProductDetails(long product_id, LPCTSTR product_name, long role_id, LPCTSTR role_name)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdProductDetails(product_id, product_name, role_id, role_name);
}

void CMpaWebEvent::OnCmdCompanyDetails(long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdCompanyDetails(company_id, company_name, role_id, role_name);
}

void CMpaWebEvent::OnCmdBeginDialogue(long user_id, LPCTSTR NickName, LPCTSTR FirstName, LPCTSTR LastName, LPCTSTR Email, 
		long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdBeginDialogue(user_id, NickName, FirstName, LastName, Email,
	   company_id,  company_name, role_id, role_name);
}

void CMpaWebEvent::OnCmdUserDetails(long user_id, LPCTSTR NickName, LPCTSTR FirstName, LPCTSTR LastName, 
	LPCTSTR Email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name)
{
//	Beep(100,100);
    if(m_pParent!=NULL)
	m_pParent->OnCmdUserDetails(user_id, NickName, FirstName, LastName, 
	Email, company_id, company_name, role_id, role_name);
}

void CMpaWebEvent::OnCmdLoadPromos(BSTR* pPromoList)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdLoadPromos(pPromoList);
}

void CMpaWebEvent::OnCmdGetUserID(long* id)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdGetUserID(id);
}

void CMpaWebEvent::OnCmdGetUserRole(long* id)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdGetUserRole(id);
}

void CMpaWebEvent::OnCmdSendPromo(long longProductID, LPCTSTR bstrSubject, LPCTSTR bstrMessage, LPCTSTR bstrRecipients,long *pHandle)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdSendPromo(longProductID, bstrSubject, bstrMessage, bstrRecipients, pHandle);
}

void CMpaWebEvent::OnCmdGetProgramSettings(long longSettingsID, BSTR* bstrSettingsXml)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdGetProgramSettings(longSettingsID, bstrSettingsXml);
}

void CMpaWebEvent::OnCmdSendMessage(long londUserID, LPCTSTR bstrNickName, LPCTSTR bstrRole)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdSendMessage(londUserID, bstrNickName, bstrRole);
}

void CMpaWebEvent::OnCmdSetProgramSettings(long longSettingsID, LPCTSTR bstrSettingsXml)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdSetProgramSettings(longSettingsID, bstrSettingsXml);
}

void CMpaWebEvent::OnCmdSendFile(long longUserID, LPCTSTR bstrNickName, LPCTSTR bstrRole)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdSendFile(longUserID, bstrNickName, bstrRole);
}

void CMpaWebEvent::OnCmdSetVariable(LPCTSTR bstrVarName, LPCTSTR bstrVarValue)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdSetVariable(bstrVarName, bstrVarValue);
}

void CMpaWebEvent::OnCmdGetVariable(LPCTSTR bstrVarName, BSTR* bstrVarValue)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdGetVariable(bstrVarName, bstrVarValue);
}


void CMpaWebEvent::OnCmdCancelOperation(long Handle)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdCancelOperation(Handle);
}

void CMpaWebEvent::OnCmdCheckStatus(long Handle, long *Result)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdCheckStatus(Handle, Result);
}

void CMpaWebEvent::OnCmdMainWindowNavigate(LPCTSTR URL, long bClose)
{
//	Beep(100,100);
	if(m_pParent!=NULL)
	m_pParent->OnCmdMainWindowNavigate(URL,bClose);
}

void CMpaWebEvent::OnShowContextMenu(long dwID, long x, long y, long pcmdtReserved, long pdispReserved, long* pShow)
{
	if(m_pParent != NULL)
		m_pParent->OnCmdShowContextMenu(dwID, x, y, (IUnknown*)pcmdtReserved, (IDispatch*)pdispReserved, pShow);
}

void CMpaWebEvent::OnCmdDoAction(LPCTSTR ActionName, LPCTSTR Params, BSTR* Result)
{
	if(m_pParent!=NULL)
		m_pParent->OnCmdDoAction(ActionName, Params, Result);
}

void CMpaWebEvent::OnGetDropTarget(long pDropTarget, long* ppDropTarget)
{
	if(m_pParent != NULL)
		m_pParent->OnCmdGetDropTarget((IDropTarget*)pDropTarget, (IDropTarget**)ppDropTarget);
}
