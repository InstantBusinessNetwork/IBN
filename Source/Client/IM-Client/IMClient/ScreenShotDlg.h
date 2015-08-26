//{{AFX_INCLUDES()
#include "webbrowser2.h"
#include "mcbutton.h"
#include "ccootree.h"
#include "resource.h"
//}}AFX_INCLUDES
#if !defined(AFX_SCREENSHOTDLG_H__313B8891_02D2_4C89_A96D_0B6EE5AF9AB1__INCLUDED_)
#define AFX_SCREENSHOTDLG_H__313B8891_02D2_4C89_A96D_0B6EE5AF9AB1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ScreenShotDlg.h : header file
//

class CMainDlg;

#include "OFSNcDlg2.h"
#include "CxImageCtrl.h"
/////////////////////////////////////////////////////////////////////////////
// CScreenShotDlg dialog

class CScreenShotDlg : public COFSNcDlg2
{
// Construction
public:
	typedef enum EDlgMode
	{
		SendFile	=	0,
		AssignToDo	=	1,
		CreateIssue =	2,
		PublishToIBNLibrary = 3,
	} DlgMode;
	
	CScreenShotDlg(CMainDlg *pMessenger,DlgMode Mode, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CScreenShotDlg)
	enum { IDD = IDD_SCREENSHOT_DIALOG };
	CCxImageCtrl	m_ImageStatic;
	CMcButton	m_btnX;
	CMcButton	m_btnMin;
	CMcButton	m_btnMax;
	CMcButton	m_btnRestore;
	CMcButton	m_Menu;
	
	CMcButton	m_btnSave;
	CMcButton	m_btnUndo;
	CMcButton	m_btnRedo;
	CMcButton	m_btnScroll;
	CMcButton	m_btnCrop;
	CMcButton	m_btnZoomOut;
	CMcButton	m_btnZoomIn;
	CMcButton	m_btnText;
	CMcButton	m_btnPen;
	CMcButton	m_btnPenColor;
	
	CMcButton	m_btnSend;
	
	CCCooTree	m_treectrl;

	// New View [6/11/2004]
	CMcButton	m_checkSend;
	CMcButton	m_checkIssue;
	CMcButton	m_checkToDo;
	CMcButton	m_checkPublish;
	
	//}}AFX_DATA

	CString				m_strRecepientGroupName;
	long				m_RecepientUserId;
	CUserCollection		m_ContactList;
	BOOL				EnableRecepients();
	
	
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CScreenShotDlg)
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

private:
	BOOL	bIsKillWinodow;

	CString			m_strSetBody;
	BOOL			m_bWasCtrlEnter, m_bWasCtrlExit;
	CMapStringToPtr	m_GroupTIDMap;
	CMapStringToPtr	m_UserCheckInGroup;

	CMainDlg				*pMessenger;
	CScreenShotDlg::DlgMode			m_Mode;
	
	int				m_iZoomValueIndex;
// Implementation
protected:
	virtual void OnCancel();
	virtual BOOL OnInitDialog();

	void	SetRecipientGroup(LPCTSTR strName);
	void	KillWindow();
	void	LoadSkin(IXMLDOMNode *pXmlRoot);
	void	BuildTree();
	void	CreateTree();
	void	UpdateID(long UserId);
	CUser*	FindUserInVisualContactList(long TID);
	void	UpdateGroupCheck();
	void	UpdateGroupID(LPCTSTR strName);
	void	Send();
	void	CreateCapture();
	void	UpdateButtons();
	CString	SaveImageToTmpFolder();
	
	// Generated message map functions
	//{{AFX_MSG(CScreenShotDlg)
	afx_msg void OnOk();
	afx_msg void OnClickMcclose ();
	afx_msg void OnClickMcmini ();
	afx_msg void OnClickMcmaxi ();
	afx_msg void OnClickMcmaximini ();
	afx_msg void OnClickMcmenu ();
	afx_msg void OnClickMcsave();
	afx_msg void OnClickMcundo ();
	afx_msg void OnClickMcredo ();
	afx_msg void OnClickMcscroll ();
	afx_msg void OnClickMccrop ();
	afx_msg void OnClickMczoomout ();
	afx_msg void OnClickMczoomin ();
	afx_msg void OnClickMctext ();
	afx_msg void OnClickMcpen ();
	afx_msg void OnClickMcpencolor();
	afx_msg void OnClickMcsend ();

	afx_msg void OnClickCheckSend ();
	afx_msg void OnClickCheckIssue ();
	afx_msg void OnClickCheckToDo();
	afx_msg void OnClickCheckPublish ();
	
	afx_msg void OnMenuCcootreectrl (long TID, BOOL bGroupe);
	afx_msg void OnActionCcootreectrl (long TID, BOOL bGroupe);
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg	LRESULT	OnUndoUpdated(WPARAM w, LPARAM l);
	//}}AFX_MSG
	DECLARE_EVENTSINK_MAP()
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SCREENSHOTDLG_H__313B8891_02D2_4C89_A96D_0B6EE5AF9AB1__INCLUDED_)
