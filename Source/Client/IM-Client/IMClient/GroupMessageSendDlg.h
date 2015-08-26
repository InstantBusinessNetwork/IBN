//{{AFX_INCLUDES()
#include "mcbutton.h"
#include "ccootree.h"
//}}AFX_INCLUDES
#if !defined(AFX_GROUPMESSAGESENDDLG_H__BCFCBDFB_B8A3_4B25_BDE0_F0CAC1363835__INCLUDED_)
#define AFX_GROUPMESSAGESENDDLG_H__BCFCBDFB_B8A3_4B25_BDE0_F0CAC1363835__INCLUDED

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MessageSplitDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
#include "OfsNCDlg.h"
#include ".\\OFSDhtmlEditCtrl\\OFSDhtmlCtrl.h"
#include "Label.h"
#include "ResizableImage.h"
#include "UserCollection.h"
// CGroupMessageSendDlg dialog

class CGroupMessageSendDlg : public COFSNcDlg2
{
// Construction
public:
	void SetRecipientGroup(LPCTSTR strName, CUserCollection* pUsers);
	BOOL Create(CWnd *pParentWnd);
	void SetFontFace();
	void SetFontSize();
	void SetFontUnderline();
	void SetFontItalic();
	void SetFontBold();
	void InsertSmile();
	void SelectColor();
	void Send();
	void SelectAll(BOOL bSelect);
	void UpdateGroupCheck();
	BOOL EnableRecepients();
	void SetBody(LPCTSTR strBody, BOOL bAutoSend	=	FALSE);
	void SetFon(HBITMAP hFon);
	void SetRecipientGroup(LPCTSTR strName);
	CGroupMessageSendDlg(CMainDlg *pMessenger,CWnd* pParent = NULL);   // standard constructor
	~CGroupMessageSendDlg();

// Dialog Data
	//{{AFX_DATA(CGroupMessageSendDlg)
	enum { IDD = IDD_GROUP_MESSAGE_SEND_DIALOG };
	CComboBox	m_FontCombo;
	CComboBox	m_SizeCombo;
	CLabel	m_UserInfo;
	CCCooTree	m_treectrl;
	CMcButton	m_btnBold;
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
	//}}AFX_DATA
	
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGroupMessageSendDlg)
public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	
	// Implementation
protected:
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	void UpdateGroupID(LPCTSTR strName);
	CMapStringToPtr	m_GroupTIDMap;
	CMapStringToPtr	m_UserCheckInGroup;

	CUser* CGroupMessageSendDlg::FindUserInVisualContactList(long TID);
	void UpdateID(long UserId);
	CUserCollection	m_ContactList;
	void BuildTree();
	void CreateTree();
	BOOL	m_bWasCtrlEnter, m_bWasCtrlExit;
	BOOL	m_bAutoSend;
	CString m_strSetBody;
	LPARAM OnSWMSetBody(WPARAM w, LPARAM l);
//	CResizableImage	m_ResizeFon;
	BOOL	bIsKillWinodow;
//	CSize sFon;
//	CBitmap *pFonBmp;
//	IMpaWebCustomizerPtr m_WebCustomizer;
	BOOL bInitEdit;
	CFont m_font;
	void KillWindow();
	long           Handle; 
	long           MessageTime;    
	CMainDlg *pMessenger;
	CString			m_strRecepientGroupName;
	BOOL bBlock;
	void UnBlock();
	void Block();
	CImageList FontStateList;
	IMessagePtr pMessage;
	ISessionPtr pSession;
    COfsDhtmlEditCtrl  m_edit;
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	// Generated message map functions
	//{{AFX_MSG(CGroupMessageSendDlg)
	afx_msg void OnOk();
	afx_msg LRESULT OnNetEvent(WPARAM w,LPARAM l);
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
	afx_msg void OnClickMcmenu();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnDropFiles( HDROP hDropInfo );
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
