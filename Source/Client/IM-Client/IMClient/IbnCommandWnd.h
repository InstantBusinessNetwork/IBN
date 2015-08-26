#if !defined(AFX_IBNCOMMANDWND_H__866CAD5C_C122_487C_B84B_316D543C2C58__INCLUDED_)
#define AFX_IBNCOMMANDWND_H__866CAD5C_C122_487C_B84B_316D543C2C58__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// IbnCommandWnd.h : header file
//

#define MC_IBNCOMMAND_CLASS_NAME         _T("#32770")
#define MC_IBNCOMMAND_WINDOW_NAME        _T("{19791104-A59E-42e5-BB49-19086A3E7300}")


class CMainDlg;
/////////////////////////////////////////////////////////////////////////////
// CIbnCommandWnd window

class CIbnCommandWnd : public CWnd
{
// Construction
public:
	CIbnCommandWnd(CMainDlg *pMessenger);

// Attributes
private:
	CMainDlg *pMessenger;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CIbnCommandWnd)
	//}}AFX_VIRTUAL

// Implementation
public:
	BOOL Create();
	BOOL SendScreenCaptureCommand(CComBSTR Command, CComBSTR Mode, CComBSTR RecipientsXml);
	virtual ~CIbnCommandWnd();

	// Generated message map functions
protected:
	CComBSTR BuildClientInfoXml();
	CComBSTR BuildContactListXml();
	void SendFile(CComPtr<IXMLDOMNode>	&pRootNode);

	//{{AFX_MSG(CIbnCommandWnd)
	afx_msg BOOL OnCopyData(CWnd *pWnd, COPYDATASTRUCT *pCopyDataStruct);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_IBNCOMMANDWND_H__866CAD5C_C122_487C_B84B_316D543C2C58__INCLUDED_)
