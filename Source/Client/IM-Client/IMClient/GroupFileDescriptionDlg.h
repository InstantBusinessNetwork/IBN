//{{AFX_INCLUDES()
#include "mcbutton.h"
#include "ccootree.h"
//}}AFX_INCLUDES
#if !defined(AFX_GROUPFILEDESCRIPTIONDLG_H__A3808E64_9B01_4CDD_A936_ED8850C95FD7__INCLUDED_)
#define AFX_GROUPFILEDESCRIPTIONDLG_H__A3808E64_9B01_4CDD_A936_ED8850C95FD7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MessageSplitDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
#include "OFSNCDlg2.h"
#include "Label.h"
#include "ResizableImage.h"
#include "UserCollection.h"
// CGroupFileDescriptionDlg dialog

#include "coolmenu.h"

class CGroupFileDescriptionDlg : public COFSNcDlg2
{
// Construction
public:
	void Send();
	void SelectAll(BOOL bSelect);
	void SetFontFace();
	void SetFontSize();
	void SetFontUnderline();
	void SetFontItalic();
	void SetFontBold();
	void InsertSmile();
	void SelectColor();
	BOOL EnableRecepients();
	void SetBody(LPCTSTR strBody);
	void SetFon(HBITMAP hFon);
	void SetRecipientGroup(LPCTSTR strName);
	CGroupFileDescriptionDlg(CMainDlg *pMessenger,CWnd* pParent = NULL);   // standard constructor
	~CGroupFileDescriptionDlg();

// Dialog Data
	//{{AFX_DATA(CGroupFileDescriptionDlg)
	enum { IDD = IDD_GROUP_FILE_SEND_DIALOG };
	CComboBox	m_FontCombo;
	CComboBox	m_SizeCombo;
	CLabel	m_UserInfo;
	CCCooTree	m_treectrl;
	CMcButton	m_btnColor;
	CMcButton	m_btnItalic;
	CMcButton	m_btnMenu;
	CMcButton	m_btnMin;
	CMcButton	m_btnOptions;
	CMcButton	m_btnSelectAll;
	CMcButton	m_btnSelectNone;
	CMcButton	m_btnSend;
	CMcButton	m_btnSmiles;
	CMcButton	m_btnUnderline;
	CMcButton	m_btnX;
	CString	m_strFileName;
	CMcButton	m_btnBold;
	//}}AFX_DATA
	
//	CFont m_font;
    COfsDhtmlEditCtrl m_edit;
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGroupFileDescriptionDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	
	// Implementation
public:
	CString GetFileDescription();
	CString				m_strRecepientGroupName;
	CUserCollection		m_ContactList;

protected:
	CString m_strSetBody;
	LPARAM OnSWMSetBody(WPARAM w, LPARAM l);
	BOOL m_bInitEdit;
	BOOL m_bWasCtrlEnter, m_bWasCtrlExit;
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	CMapStringToPtr	m_GroupTIDMap;
	CMapStringToPtr	m_UserCheckInGroup;
	
	void UpdateGroupCheck();
	void UpdateGroupID(LPCTSTR strName);
	CUser* CGroupFileDescriptionDlg::FindUserInVisualContactList(long TID);
	void UpdateID(long UserId);
	
	void BuildTree();
	void CreateTree();
	CMainDlg *pMessenger;
	
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	// Generated message map functions
	//{{AFX_MSG(CGroupFileDescriptionDlg)
	afx_msg void OnOk();
	afx_msg void OnSelEndOkFontCombo();
	afx_msg void OnSelEndOkSizeCombo();
	afx_msg void OnEditMenuCopy();
	afx_msg void OnEditMenuCut();
	afx_msg void OnEditMenuDelete();
	afx_msg void OnEditMenuPaste();
	afx_msg void OnUpdateEditMenuCopy(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditMenuCut(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditMenuDelete(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditMenuPaste(CCmdUI* pCmdUI);
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnMove(int x, int y);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnActionCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnDoDropCcootreectrl(long TID, BOOL bGroupe, LPUNKNOWN pDataObject, long DropEffect, long LastKeyState);
	afx_msg void OnMenuCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnSelectCcootreectrl(long TID, BOOL bGroupe);
	afx_msg void OnClickBtnX();
	afx_msg void OnClickBtnMin();
	afx_msg void OnClickBtnOptions();
	afx_msg void OnClickBtnMenu();
	afx_msg void OnClickBtnSelectAll();
	afx_msg void OnClickBtnSelectNone();
	afx_msg void OnClickBtnColor();
	afx_msg void OnClickBtnBold();
	afx_msg void OnClickBtnItalic();
	afx_msg void OnClickBtnUnderline();
	afx_msg void OnClickBtnSmiles();
	afx_msg void OnClickBtnSend();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	HRESULT OnEditUpdate(WPARAM w, LPARAM l);
	void OnSmileItem(UINT nID);
	DECLARE_MESSAGE_MAP()
private:
	CCoolMenuManager m_CoolMenuManager;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // 
