#if !defined(AFX_WEBEVENT_H__E873D884_6A00_42C1_B801_D83591C18CF4__INCLUDED_)
#define AFX_WEBEVENT_H__E873D884_6A00_42C1_B801_D83591C18CF4__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

// EventContainer.h : header file
//
//
//class CChildView;
class CWebWindow;

/////////////////////////////////////////////////////////////////////////////
/// Класс по отлову Событий от ATL_NetLib.dll
class CMpaWebEvent : public CCmdTarget
{
	DECLARE_DYNCREATE(CMpaWebEvent)
public:
	CMpaWebEvent();           // protected constructor used by dynamic creation
    virtual ~CMpaWebEvent();
// Attributes
//	CChildView *m_pParent;
	CWebWindow *m_pParent;
// Operations
public:

//	void SetParent(CChildView *pParent);

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMpaWebEvent)
	//}}AFX_VIRTUAL

// Implementation
protected:
	
	void OnCmdBeginPromo(long product_id, LPCTSTR product_name);
	void OnCmdAddProduct(long product_id, LPCTSTR product_name);
	void OnCmdNewWindow(long x, long y, long cx, long cy, LPCTSTR title, LPCTSTR url, BOOL bModal, BOOL bTopMost,  BOOL bResizable);
    void OnCmdProductDetails(long product_id, LPCTSTR product_name, long role_id, LPCTSTR role_name);
	void OnCmdAddCompany(long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name);
    void OnCmdCompanyDetails(long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name);
    void OnCmdBeginDialogue(long user_id, LPCTSTR NickName, LPCTSTR FirstName, LPCTSTR LastName, LPCTSTR Email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name);
	void OnCmdAddContact(long user_id, LPCTSTR nick_name, LPCTSTR first_name, LPCTSTR last_name, LPCTSTR email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name);
    void OnCmdUserDetails(long user_id, LPCTSTR NickName, LPCTSTR FirstName, LPCTSTR LastName, LPCTSTR Email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name);
    void OnCmdLoadPromos(BSTR* pPromoList);
    void OnCmdGetUserID(long* id);
	void OnCmdGetUserRole(long* id);
    void OnCmdSendPromo(long longProductID, LPCTSTR bstrSubject, LPCTSTR bstrMessage, LPCTSTR bstrRecipients,long *pHandle);
    void OnCmdGetProgramSettings(long longSettingsID, BSTR* bstrSettingsXml);
    void OnCmdSendMessage(long londUserID, LPCTSTR bstrNickName, LPCTSTR bstrRole);
	void OnCmdSetProgramSettings(long longSettingsID, LPCTSTR bstrSettingsXml);
    void OnCmdSendFile(long longUserID, LPCTSTR bstrNickName, LPCTSTR bstrRole);
    void OnCmdSetVariable(LPCTSTR bstrVarName, LPCTSTR bstrVarValue);
    void OnCmdGetVariable(LPCTSTR bstrVarName, BSTR* bstrVarValue);
	void OnCmdCancelOperation(long Handle);
	void OnCmdCheckStatus(long Handle, long *Result);
	void OnCmdMainWindowNavigate(LPCTSTR URL, long bClose);
	void OnShowContextMenu(long dwID, long x, long y, long pcmdtReserved, long pdispReserved, long* pShow);
	void OnCmdDoAction(LPCTSTR ActionName, LPCTSTR Params, BSTR* Result);
	void OnGetDropTarget(long pDropTarget, long* ppDropTarget);
	// Generated message map functions
	//{{AFX_MSG(CMpaWebEvent)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	// Generated OLE dispatch map functions
	//{{AFX_DISPATCH(CMpaWebEvent)
    //}}AFX_DISPATCH
	DECLARE_DISPATCH_MAP()
	DECLARE_INTERFACE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_EVENTCONTAINER_H__E873D884_6A00_42C1_B801_D83591C18CF4__INCLUDED_)
