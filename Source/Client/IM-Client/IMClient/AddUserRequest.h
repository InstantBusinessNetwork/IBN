//{{AFX_INCLUDES()
#include "webbrowser2.h"
//}}AFX_INCLUDES
#if !defined(AFX_ADDUSERREQUEST_H__EDC9A951_2F0D_4865_9669_395BDA4E8E64__INCLUDED_)
#define AFX_ADDUSERREQUEST_H__EDC9A951_2F0D_4865_9669_395BDA4E8E64__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// AddUserRequest.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CAddUserRequest dialog
#include "OfsNcDlg.h"
#include "ResizableImage.h"
#include "McButton.h"
#include "Label.h"

//#include ".\\OFSDhtmlEditCtrl\\OFSDhtmlCtrl.h"
#include "user.h"	// Added by ClassView

class CMainDlg;


class CAddUserRequest : public CResizableDialog
{
// Construction
public:
	CAddUserRequest(CMainDlg* pParent);   // standard constructor

	void SetSender(CUser &user, BSTR Data);
// Dialog Data
	//{{AFX_DATA(CAddUserRequest)
	enum { IDD = IDD_ADDUSERREQUEST_DIALOG };
	CStatic	m_frameEdit;
	CLabel		m_Nick;
	CButton	m_Accept;
	CButton	m_Deny;
	CButton	m_Details;
	CButton	m_AddToContact;
	CWebBrowser2	m_edit;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAddUserRequest)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation

public:
	CString GetMessageText();
	CUser m_User;
protected:
	void InitMpaWebEvent();
	void CloseMpaWebEvent();

	IMpaWebCustomizerPtr	m_pWebCustomizer;
	DWORD					m_dwSessionCookie;
	CComBSTR				m_bsHTML;
	
	BOOL	m_bAddUserCommand;
	void KillWindow();
	CMainDlg *pMessenger;
	ISessionPtr pSession;
	long	Handle;
	void UnBlock();
	void Block();
    //COfsDhtmlEditCtrl m_edit;
	BOOL				bIsKillWinodow;
	// Generated message map functions
	//{{AFX_MSG(CAddUserRequest)
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	afx_msg void OnDany();
	afx_msg void OnEditmenuCopy();
	afx_msg void OnUpdateEditmenuCopy(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuCut();
	afx_msg void OnUpdateEditmenuCut(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuDelete();
	afx_msg void OnUpdateEditmenuDelete(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuPast();
	afx_msg void OnUpdateEditmenuPast(CCmdUI* pCmdUI);
	afx_msg void OnAuthorizationRequest();
	afx_msg void OnGetuserdetailsButton();
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnClickMcclose();
	afx_msg void OnClickMcmini();
	afx_msg void OnClickMcaccept();
	afx_msg void OnClickMcdeny();
	afx_msg void OnClickMcadd();
	afx_msg void OnClickMcdetails();
	afx_msg void OnMcUserDetails();
	afx_msg void OnMcAddToContact();
	afx_msg void OnMcAccept();
	afx_msg void OnMcDeny();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	LRESULT OnNetEvent(WPARAM w,LPARAM l);
	afx_msg void OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow);
	void OnWebDocumentCompleted(IDispatch* pDisp, VARIANT* URL);
	DECLARE_INTERFACE_MAP()
	DECLARE_DISPATCH_MAP()	
		
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ADDUSERREQUEST_H__EDC9A951_2F0D_4865_9669_395BDA4E8E64__INCLUDED_)
