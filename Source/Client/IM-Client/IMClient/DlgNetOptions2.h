//{{AFX_INCLUDES()
#include "mcbutton.h"
//}}AFX_INCLUDES
#if !defined(AFX_DLGNETOPTIONS2_H__921824D2_7FB0_411C_B21E_1F060BC97A40__INCLUDED_)
#define AFX_DLGNETOPTIONS2_H__921824D2_7FB0_411C_B21E_1F060BC97A40__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DlgNetOptions2.h : header file
//

#include "OFSNcDlg2.h"

/////////////////////////////////////////////////////////////////////////////
// CDlgNetOptions2 dialog

class CDlgNetOptions2 : public COFSNcDlg2
{
// Construction
public:
	CDlgNetOptions2(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDlgNetOptions2)
	enum { IDD = IDD_NETOPTIONS_2 };
	CEdit	m_edUser;
	CEdit	m_edServ;
	CEdit	m_edPort;
	CEdit	m_edPass;
	CMcButton	m_radio1;
	CMcButton	m_radio2;
	CMcButton	m_radio3;
	CMcButton	m_checkbox;
	CMcButton	m_checkssl;
	CString	m_strPass;
	CString	m_strServ;
	CString	m_strUser;
	CMcButton	m_btnX;
	CMcButton	m_btnOK;
	CMcButton	m_btnCancel;
	CString	m_strPort;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgNetOptions2)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	void UpdateControls();
	BOOL m_bUseFirewall;
	BOOL m_bUseSSL;
	DWORD m_dwAccessType;

	// Generated message map functions
	//{{AFX_MSG(CDlgNetOptions2)
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	afx_msg void OnClickButtonX();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnClickRadio1();
	afx_msg void OnClickRadio2();
	afx_msg void OnClickRadio3();
	afx_msg void OnClickCheckbox();
	afx_msg void OnClickButtonOk();
	afx_msg void OnClickButtonCancel();
	afx_msg void OnClickUseSSL();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGNETOPTIONS2_H__921824D2_7FB0_411C_B21E_1F060BC97A40__INCLUDED_)
