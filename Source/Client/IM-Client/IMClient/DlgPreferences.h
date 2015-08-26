//{{AFX_INCLUDES()
#include "mcbutton.h"
//}}AFX_INCLUDES
#if !defined(AFX_DLGPREFERENCES_H__050A37DC_2F67_4754_ABCF_EF030F6DC3B1__INCLUDED_)
#define AFX_DLGPREFERENCES_H__050A37DC_2F67_4754_ABCF_EF030F6DC3B1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DlgPreferences.h : header file
//

#include "OFSNcDlg2.h"
#include <msxml2.h>

/////////////////////////////////////////////////////////////////////////////
// CDlgPreferences dialog

#define PREFERENCES_PARENT COFSNcDlg2

class CDlgPreferences : public PREFERENCES_PARENT
{
// Construction
public:
	CDlgPreferences(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDlgPreferences)
	enum { IDD = IDD_PREFERENCES };
	CStatic	m_placeholder;
	CListCtrl	m_list;
	CMcButton	m_btnX;
	CMcButton	m_btnOK;
	CMcButton	m_btnCancel;
	CMcButton	m_btnApply;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgPreferences)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void LoadButtons(IXMLDOMNode *pXmlRoot);
	void LoadSkin(IXMLDOMNode *pXmlRoot);

//	CArray<CMcSettingsPage*, CMcSettingsPage*> m_aPages;
	
	// Generated message map functions
	//{{AFX_MSG(CDlgPreferences)
	virtual void OnOK();
	virtual void OnCancel();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGPREFERENCES_H__050A37DC_2F67_4754_ABCF_EF030F6DC3B1__INCLUDED_)
