//{{AFX_INCLUDES()
#include "webbrowser2.h"
//}}AFX_INCLUDES
#if !defined(AFX_POPUPMESSAGE_H__43F74507_0927_4DED_9590_2D2BD25BC1C5__INCLUDED_)
#define AFX_POPUPMESSAGE_H__43F74507_0927_4DED_9590_2D2BD25BC1C5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PopupMessage.h : header file
//
#include "label.h"
#include "OfsNcDlg2.h"
/////////////////////////////////////////////////////////////////////////////
// CPopupMessage dialog

class CPopupMessage : public COFSNcDlg2
{
// Construction
public:
	BOOL Show(LPCWSTR strXML, LPCWSTR strXSLT,DWORD dwTimeoutSec	=	10);
	BOOL InitClickMsg(UINT Message, WPARAM w, LPARAM l);
	BOOL Wait();
	BOOL Stop();
	BOOL Hide();
	//BOOL Show(LPCTSTR strMessage, DWORD dwTimeoutSec = 10);
	CPopupMessage(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CPopupMessage)
	enum { IDD = IDD_POPUP_MESSAGE };
	CWebBrowser2	m_browser;
	//}}AFX_DATA

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CPopupMessage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void OnWebNavigateComplete2(IDispatch *pDisp, VARIANT *URL);
	void OnWebMouseDown(DISPID id, VARIANT* pVarResult);
	void OnWebDocumentCompleted(IDispatch* pDisp, VARIANT* URL);
	IMpaWebCustomizerPtr m_pWebCustomizer;
	
	void KillWindow();
	void SetCY(long Value);
	enum {PMS_NONE,PMS_SHOW,PMS_HIDE,PMS_WAIT} pmState;

	void LoadSkin(IXMLDOMNode *pXmlRoot);
	// Generated message map functions
	//{{AFX_MSG(CPopupMessage)
	//afx_msg void OnInfolinkStatic();
	virtual BOOL OnInitDialog();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnDestroy();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnActivate(UINT nState, CWnd* pWndOther, BOOL bMinimized);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnSetFocus( CWnd* );
	//afx_msg void OnAppStatic();
	//afx_msg void OnTitleStatic();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
private:
	long	m_dwTimeout;
	long	m_MinCY;
	long	m_MaxCY;
	long	m_CurrentCY;

	UINT	m_MsgId;
	LPARAM	m_MsgL;
	WPARAM	m_MsgW;
	
	CComBSTR	m_bsHTML;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_POPUPMESSAGE_H__43F74507_0927_4DED_9590_2D2BD25BC1C5__INCLUDED_)
