#if !defined(AFX_SELECTSERVER_H__84834321_207F_4347_B596_577227CF1BE0__INCLUDED_)
#define AFX_SELECTSERVER_H__84834321_207F_4347_B596_577227CF1BE0__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SelectServer.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CSelectServer dialog

class CSelectServer : public CDialog
{
// Construction
public:
	CSelectServer(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CSelectServer)
	enum { IDD = IDD_SELECT_SERVER_DIALOG };
	CListBox	m_ServerList;
	CString	m_strServerName;
	//}}AFX_DATA

	CStringArray	m_ServerNameArr;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSelectServer)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CSelectServer)
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SELECTSERVER_H__84834321_207F_4347_B596_577227CF1BE0__INCLUDED_)
