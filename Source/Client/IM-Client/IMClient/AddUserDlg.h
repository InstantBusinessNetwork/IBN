//{{AFX_INCLUDES()
#include "mcticker.h"
//}}AFX_INCLUDES
#if !defined(AFX_ADDUSERDLG_H__C2EAB833_2AB0_426E_B135_0D48DE834AD3__INCLUDED_)
#define AFX_ADDUSERDLG_H__C2EAB833_2AB0_426E_B135_0D48DE834AD3__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// AddUserDlg.h : header file
//

#include "OfsNcDlg.h"
#include "ResizableImage.h"
#include "McButton.h"
#include "Label.h"

#include ".\\OFSDhtmlEditCtrl\\OFSDhtmlCtrl.h"
#include "user.h"	// Added by ClassView
#include "coolmenu.h"

class CMainDlg;
/////////////////////////////////////////////////////////////////////////////
// CAddUserDlg dialog

class CAddUserDlg : public CResizableDialog
{
// Construction
public:
	void AddNewContact(long user_id, LPCTSTR nick_name, LPCTSTR first_name, LPCTSTR last_name, LPCTSTR email, long company_id, LPCTSTR company_name, long role_id, LPCTSTR role_name);
	CAddUserDlg(CMainDlg* pParent);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CAddUserDlg)
	enum { IDD = IDD_ADDUSER_DIALOG };
	CStatic	m_frameEdit;
	CLabel	m_Nick;
	CButton	m_ColorButton;
	CButton	m_BoldButton;
	CButton	m_ItalicButton;
	CButton	m_UnderLineButton;
	CButton	m_CancelButton;
	CButton m_InsertSmileButton;
	CButton	m_Ok;
	CButton	m_Cancel;
	CComboBox	m_FontCombo;
	CComboBox	m_SizeCombo;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAddUserDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
public:
	CComBSTR GetMessageText();
	CUser m_User;
protected:
	BOOL	bBlock;
	BOOL	m_bWasCtrlEnter, m_bWasCtrlExit;
	
	void KillWindow();
	CMainDlg	*pMessenger;
	ISessionPtr		pSession;
	long			Handle;
	CImageList		FontStateList;
	CFont			m_font;
	BOOL			bInitEdit;
	
	void UnBlock();
	void Block();
	
    COfsDhtmlEditCtrl	m_edit;
	BOOL				bIsKillWinodow;
	CResizableImage		m_ResizeFon;
	// Generated message map functions
	//{{AFX_MSG(CAddUserDlg)
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	afx_msg void OnAuthorization();
	afx_msg void OnColorButton();
	afx_msg void OnBoldCheck();
	afx_msg void OnItalicCheck();
	afx_msg void OnUnderlineCheck();
	afx_msg void OnEditmenuCopy();
	afx_msg void OnUpdateEditmenuCopy(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuCut();
	afx_msg void OnUpdateEditmenuCut(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuDelete();
	afx_msg void OnUpdateEditmenuDelete(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuPast();
	afx_msg void OnUpdateEditmenuPast(CCmdUI* pCmdUI);
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnClickMcmini();
	afx_msg void OnClickMcclose();
	afx_msg void OnClickMcOk();
	afx_msg void OnClickMcCancel();
	afx_msg void OnInsertsmileCheck();
	afx_msg void OnSelendokFontCombo();
	afx_msg void OnSelendokSizeCombo();
	afx_msg void OnClickAuth();
	afx_msg void OnClickCancel();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	LRESULT OnNetEvent(WPARAM w,LPARAM l);
	HRESULT OnEditUpdate(WPARAM w, LPARAM l);
	void	OnSmileItem(UINT nID);
	
	DECLARE_MESSAGE_MAP()
private:
	CCoolMenuManager m_CoolMenuManager;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ADDUSERDLG_H__C2EAB833_2AB0_426E_B135_0D48DE834AD3__INCLUDED_)
