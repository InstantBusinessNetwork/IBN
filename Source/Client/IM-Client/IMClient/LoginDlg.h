//{{AFX_INCLUDES()
#include "mcbutton.h"
//}}AFX_INCLUDES
#if !defined(AFX_LOGINDLG_H__AEADEAB3_D1CD_4664_AA31_ABAE4F64B48C__INCLUDED_)
#define AFX_LOGINDLG_H__AEADEAB3_D1CD_4664_AA31_ABAE4F64B48C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// LoginDlg.h : header file
//
#include "OFSNcDlg2.h"
#include "McLoginAutoCompleteSource.h"

/////////////////////////////////////////////////////////////////////////////
// CLoginDlg dialog
#define WM_INETLOGIN  (WM_USER + 100)
#define WM_CANCELLOGIN (WM_USER + 101)

class CLoginDlg : public COFSNcDlg2
{
// Construction
public:
	BOOL IsBlock();
	void UnBlock();
	void Block();
	CLoginDlg(CWnd* pParent = NULL);   // standard constructor
	~CLoginDlg();

	CString GetUserLogin()
	{
		int StartPortPos = -1;
		if((StartPortPos = m_LoginStr.Find(_T(":")))!=-1)
		{
			return m_LoginStr.Left(StartPortPos);
		}

		return m_LoginStr;
	}

// Dialog Data
	//{{AFX_DATA(CLoginDlg)
	enum { IDD = IDD_DIALOG_LOGIN };
	CLabel	m_title;
	CEdit	m_LoginEdit;
	CEdit	m_PasswordEdit;
	CMcButton	m_btnCancel;
	CMcButton	m_btnX;
	CMcButton	m_btnLogin;
	CMcButton	m_btnSSL;
	CString	m_LoginStr;
	CString	m_PasswordStr;
	CMcButton	m_btnSavePassword;
	CMcButton	m_btnNetOption;
	//}}AFX_DATA

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CLoginDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	//}}AFX_VIRTUAL

// Implementation
protected:
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	BOOL m_Block;
	
	CComPtr<IAutoComplete>		m_pAutoComplete;
	CMcLoginAutoCompleteSource	m_AutoCompleteSource;
	
	void ShowLoginTooltip(BOOL bShow	=	TRUE);
//	CImageList	m_ConnectImage;
//	int			intProgressIndex;

//	CBitmap		m_pFonBMP;
//	void LoadSkin();
	// Generated message map functions
	//{{AFX_MSG(CLoginDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnClickButtonX();
	afx_msg void OnClickButtonCancel();
	afx_msg void OnClickButtonLogin();
	afx_msg void OnClickButtonNetoption();
	afx_msg void OnClose();
	virtual void OnOK();
	virtual void OnCancel();
	void OnLButtonDown(UINT nFlags, CPoint point);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_LOGINDLG_H__AEADEAB3_D1CD_4664_AA31_ABAE4F64B48C__INCLUDED_)
