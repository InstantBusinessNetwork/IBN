//{{AFX_INCLUDES()
#include "webbrowser2.h"
#include "mcbutton.h"
#include "resource.h"
//}}AFX_INCLUDES
#if !defined(AFX_MESSAGESPLITDLG3_H__57334E4B_6FAA_4A15_AC98_5AB99FAD07E9__INCLUDED_)
#define AFX_MESSAGESPLITDLG3_H__57334E4B_6FAA_4A15_AC98_5AB99FAD07E9__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MessageSplitDlg.h : header file
//
#include "OFSNcDlg2.h"

/////////////////////////////////////////////////////////////////////////////
#include "OfsNCDlg.h"
#include ".\\OFSDhtmlEditCtrl\\OFSDhtmlCtrl.h"
//#include "ChildView.h"
#include "Label.h"
#include "ResizableImage.h"
// CMessageSplitDlg3 dialog

class CMainDlg;

class CMessageSplitDlg3 : public COFSNcDlg2
{
// Construction
public:
	BOOL Create(CWnd *pParentWnd, BOOL bDisableEditOnStart	=	FALSE);
	void SetBody(LPCTSTR strBody);
	void SetFon(HBITMAP hFon);
	BOOL Refresh();
//	CChildView m_HistoryChild;
//	CWebBrowser2 m_HistoryChild;
	void SetRecipient(CUser &user);
	void SetSender(CUser &user);
	CMessageSplitDlg3(CMainDlg *pMessenger,CWnd* pParent = NULL);   // standard constructor
	~CMessageSplitDlg3();

// Dialog Data
	//{{AFX_DATA(CMessageSplitDlg3)
	enum { IDD = IDD_DIALOG_MESSAGE_SPLIT3 };
	CLabel	m_SenderUserInfo;
	CLabel	m_UserInfo;
	CStatic	m_EditFrame;
	CMcButton	m_btnX;
	CMcButton	m_Send;
	CMcButton	m_Options;
	CMcButton	m_Menu;
	CMcButton	m_btnMin;
	CComboBox	m_FontCombo;
	CComboBox	m_SizeCombo;
	CButton	m_SendButton;
	CMcButton	m_btnColor;
	CMcButton	m_btnBold;
	CMcButton	m_btnItalic;
	CMcButton	m_btnUnderline;
	CMcButton	m_btnSmiles;
	//}}AFX_DATA
	
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMessageSplitDlg3)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	
	// Implementation
protected:
	BOOL	m_bEnableIfActiavte;
	void InitMpaWebEvent();
	void CloseMpaWebEvent();

	BOOL m_bEnableNavigateHistory;
	
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	BOOL	m_bWasCtrlEnter, m_bWasCtrlExit;
	CString m_strSetBody;
	BOOL	m_bSendBody;
	LPARAM OnSWMSetBody(WPARAM w, LPARAM l);
	CResizableImage	m_ResizeFon;
	BOOL	bIsKillWinodow;
	CSize sFon;
	CBitmap *pFonBmp;
	HRESULT OnAutoRefresh(WPARAM w, LPARAM l);
	IMpaWebCustomizerPtr m_WebCustomizer;
	BOOL bInitEdit;
	CFont m_font;
	void KillWindow();
	long           Handle; 
	long           MessageTime;    
	CMainDlg *pMessenger;
	CUser          m_Recipient,m_Sender;
	BOOL bBlock;
	void UnBlock();
	void Block();

	CImageList				FontStateList;
	IMessagePtr				pMessage;
	ISessionPtr				pSession;
	CWebBrowser2			m_History;
    COfsDhtmlEditCtrl		m_edit;
	IMpaWebCustomizerPtr	m_pWebCustomizer;
	DWORD					m_dwSessionCookie;

	CComBSTR m_InitialDefaultFontName;
	int m_InitialDefaultFontSize;
	
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	void OnSendButton();
	// Generated message map functions
	//{{AFX_MSG(CMessageSplitDlg3)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnOk();
	afx_msg LRESULT OnNetEvent(WPARAM w,LPARAM l);
	afx_msg void OnColorButton();
	afx_msg void OnSelendokFontCombo();
	afx_msg void OnSelendokSizeCombo();
	afx_msg void OnEditmenuCopy();
	afx_msg void OnUpdateEditmenuCopy(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuCut();
	afx_msg void OnUpdateEditmenuCut(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuDelete();
	afx_msg void OnUpdateEditmenuDelete(CCmdUI* pCmdUI);
	afx_msg void OnEditmenuPast();
	afx_msg void OnUpdateEditmenuPast(CCmdUI* pCmdUI);
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnMove(int x, int y);
	afx_msg void OnPaint();
	afx_msg void OnClickMcclose();
	afx_msg void OnClickMcsend();
	afx_msg void OnClickMcoptions();
	afx_msg void OnClickMcmenu();
	afx_msg void OnClickMcmini();
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnDropFiles( HDROP hDropInfo );
	afx_msg void OnInsertsmileCheck();
	afx_msg void OnActivate( UINT nState, CWnd* pWndOther, BOOL bMinimized );
	afx_msg BOOL OnNcActivate(BOOL bActive);
	afx_msg void OnClickBtnColor();
	afx_msg void OnClickBtnBold();
	afx_msg void OnClickBtnItalic();
	afx_msg void OnClickBtnUnderline();
	afx_msg void OnClickBtnSmiles();
	afx_msg void OnBeforeNavigate2History(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel);
	afx_msg void OnDocumentComplete2History(LPDISPATCH pDisp, VARIANT FAR* URL);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	afx_msg void OnShowContextMenu(long dwID, long x, long y, IUnknown *pcmdTarget, IDispatch *pdispReserved, long* pShow);
	afx_msg void OnCmdGetVariable(LPCTSTR bstrVarName, BSTR* bstrVarValue);
	DECLARE_DISPATCH_MAP()		
	DECLARE_INTERFACE_MAP()

	LPARAM OnSetRecipient(WPARAM w, LPARAM l);
	LRESULT OnSWMRefreh(WPARAM w, LPARAM l);
	HRESULT OnEditUpdate(WPARAM w, LPARAM l);
	void OnSmileItem(UINT nID);

	DECLARE_MESSAGE_MAP()
private:
	BOOL	m_bDisableEditOnStart;
	BOOL	m_bEnableRefresh;

	CCoolMenuManager m_CoolMenuManager;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MESSAGESPLITDLG3_H__57334E4B_6FAA_4A15_AC98_5AB99FAD07E9__INCLUDED_)
