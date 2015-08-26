//{{AFX_INCLUDES()
#include "mcbutton.h"
//}}AFX_INCLUDES
#if !defined(AFX_DLGABOUT_H__9BC55935_667C_4A2C_B477_74FF069FC960__INCLUDED_)
#define AFX_DLGABOUT_H__9BC55935_667C_4A2C_B477_74FF069FC960__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DlgAbout.h : header file
//

#include "OFSNcDlg2.h"
#include "Label.h"

/////////////////////////////////////////////////////////////////////////////
// CDlgAbout dialog

class CDlgAbout : public COFSNcDlg2
{
// Construction
public:
	CDlgAbout(CWnd* pParent = NULL);   // standard constructor
	
// Dialog Data
	//{{AFX_DATA(CDlgAbout)
	enum { IDD = IDD_ABOUTBOX };
	CLabel	m_Version;
	CLabel	m_ServerVersion;
	CMcButton	m_btnX;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgAbout)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void LoadSkin(IXMLDOMNode *pXmlRoot);

	// Generated message map functions
	//{{AFX_MSG(CDlgAbout)
	virtual BOOL OnInitDialog();
	afx_msg void OnClickBtnX();
	afx_msg void OnPaint();
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGABOUT_H__9BC55935_667C_4A2C_B477_74FF069FC960__INCLUDED_)
