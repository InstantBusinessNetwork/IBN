//{{AFX_INCLUDES()
#include "mcbutton.h"
#include "webbrowser2.h"
//}}AFX_INCLUDES
#if !defined(AFX_WEBWINDOW_H__0B3CFF16_C985_4517_BFFC_5465180CF565__INCLUDED_)
#define AFX_WEBWINDOW_H__0B3CFF16_C985_4517_BFFC_5465180CF565__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// WebWindow.h : header file
//

#include "Resource.h"
#include "ResizableImage.h"
#include "ResizableDialog.h"
#include "OFSNcDlg2.h"
#include <msxml2.h>
#include "XMLDOC.h"

/////////////////////////////////////////////////////////////////////////////
// CWebWindow dialog

#define WEB_WINDOW_PARENT COFSNcDlg2

#include "webbrowser2.h"	// Added by ClassView
#include "MpaWebEvent.h"	// Added by ClassView
#include "mcmessengerhost.h"

class CWebWindow : public WEB_WINDOW_PARENT
{
// Construction
public:
	void CreateAutoKiller(LPCTSTR szSettingsURL, CWnd *pMessageParent, CWnd *pParent, long x, long y, long cx, long cy, LPCTSTR title, LPCTSTR url, BOOL bModal, BOOL bTopMost,  BOOL bResizable, UINT DialogTypeId = 0, BOOL bBrowserRect = FALSE, BOOL bShowToolbar = TRUE, BOOL bMaximazed = FALSE);
	void LoadXML(LPCTSTR URL);
	static void SetVariable(LPCTSTR Name, LPCTSTR Value);
	CWebWindow(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CWebWindow)
	enum { IDD = IDD_WEB_WINDOW };
	CLabel	m_title;
	CMcButton	m_btnMin;
	CMcButton	m_btnX;
	CMcButton	m_btnMax;
	CMcButton	m_btnBack;
	CMcButton	m_btnForward;
	CMcButton	m_btnRefresh;
	CMcButton	m_btnStop;
	CMcButton	m_btnRestore;
	CWebBrowser2	m_browser;
	//}}AFX_DATA

	// Addon Browser Data [4/22/2002]
	CComVariant	varFilesData;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CWebWindow)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	

// Implementation
protected:
	int m_nFileDownload;
	BOOL m_bNavigateStarted;

	BOOL m_bChild;
	CString m_strTitle;
	BOOL m_bIEBack;
	BOOL m_bIEForward;
	BOOL m_bIEStop;
	BOOL m_bIERefresh;
	void ShowToolbar(BOOL bShow);
	BOOL m_bShowToolbar;
	BOOL m_bBrowserRect;
	long m_nIEVersion;
	void SetBrowserRect(CRect r);
	CRect m_rTarget;
	CRect m_rBrowser;
	CRect m_rWindow;
	void TryLoadXML();
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	void LoadButtons(IXMLDOMNode *pXmlRoot);
	void Unadvise();
	void InitMpaWebEvent();
	CWnd *m_pMessageParent;
	IMpaWebCustomizerPtr m_pWebCustomizer;
	BOOL m_bFileDownload;

	CMcMessengerDropTarget		*m_InWindowDropTarget;
	BOOL						m_bCatchNavigate;
	BOOL						m_bCatchWindowOpen;
	
//	CBitmap *m_pBackground;
//	CBitmap *m_pBranding;
//	CString m_strSkinSettings;
	CRect m_InitialRect;
	BOOL m_bAutoKill;
//	CResizableImage m_ReszImage;
	long m_SendStatus;
	long m_Handle;
	CXmlDoc *m_pXMLDoc;
	
	CComBSTR m_AttachLoadXML;
	//BOOL	m_bDocumentComplete;
	BOOL	m_bQueryAttachXML;

	afx_msg void OnWebProgressChange(long lProgress, long lProgressMax);
	afx_msg void OnWebBeforeNavigate2(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel);
	afx_msg void OnWebNewWindow2(LPDISPATCH FAR* ppDisp, BOOL FAR* Cancel);
	afx_msg void OnWebWindowClosing(BOOL IsChildWindow, BOOL FAR* Cancel);
	afx_msg void OnWebWindowSetResizable(BOOL Resizable);
	afx_msg void OnWebWindowSetLeft(long Left);
	afx_msg void OnWebWindowSetTop(long Top);
	afx_msg void OnWebWindowSetWidth(long Width);
	afx_msg void OnWebWindowSetHeight(long Height);
	afx_msg void OnWebTitleChange(LPCTSTR Text);
	afx_msg void OnWebNavigateComplete2(LPDISPATCH pDisp, VARIANT FAR* URL);
	// Generated message map functions
	//{{AFX_MSG(CWebWindow)
	virtual void OnOK();
	virtual void OnCancel();
	afx_msg void OnClickButtonX();
	afx_msg void OnClickButtonMin();
	virtual BOOL OnInitDialog();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnClose();
	afx_msg void OnDestroy();
	afx_msg void OnClickButtonMax();
	afx_msg void OnClickButtonBack();
	afx_msg void OnClickButtonForward();
	afx_msg void OnClickButtonStop();
	afx_msg void OnClickButtonRefresh();
	afx_msg void OnClickButtonRestore();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnWebToolBar(BOOL ToolBar);
	afx_msg void OnPaint();
	afx_msg void OnWebFileDownload(BOOL b, BOOL FAR* Cancel);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	//afx_msg BOOL OnToolTipNotify(UINT id, NMHDR *pNMHDR, LRESULT *pResult);
	HRESULT OnXMLLoadCopleted(WPARAM w,LPARAM l);
	DECLARE_MESSAGE_MAP()
private:
	BOOL m_bSizeSet;
	BOOL m_bPositionSet;
	CString m_strURL;
	CWnd* GetMessageParent() const;
	CMpaWebEvent m_MpaWebEvent;
	DWORD m_dwSessionCookie;

public:
	HRESULT NavigateNewWindow(LPCTSTR strUrl, BOOL bSimpleMode	=	TRUE);
	BOOL Navigate(LPCTSTR Url);
	BOOL CreateAsChild(CWnd *pParent, CWnd *pMessageParent);
	void SetTitle(LPCTSTR szTitle);
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
	void OnCmdShowContextMenu(long dwID, long x, long y, IUnknown* pcmdtReserved, IDispatch* pdispReserved, long* pShow);
	void OnCmdDoAction(LPCTSTR ActionName, LPCTSTR Params, BSTR* Result);
	void OnCmdGetDropTarget(IDropTarget* pDropTarget, IDropTarget** ppDropTarget);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_WEBWINDOW_H__0B3CFF16_C985_4517_BFFC_5465180CF565__INCLUDED_)
