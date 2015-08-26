#if !defined(AFX_EDITMESSAGETEMPLATEDLG_H__91A12C95_82CA_48CE_99F8_49385821334E__INCLUDED_)
#define AFX_EDITMESSAGETEMPLATEDLG_H__91A12C95_82CA_48CE_99F8_49385821334E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// EditMessageTemplateDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CEditMessageTemplateDlg dialog

class CEditMessageTemplateDlg : public CDialog
{
// Construction
public:
	CEditMessageTemplateDlg(CWnd* pParent = NULL);   // standard constructor
	CEditMessageTemplateDlg(CString strName, CString strText, CWnd* pParent =NULL);

// Dialog Data
	//{{AFX_DATA(CEditMessageTemplateDlg)
	enum { IDD = IDD_EDIT_MES_TEMPLATE_DIALOG };
	CString	m_strName;
	CString	m_strText;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CEditMessageTemplateDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CEditMessageTemplateDlg)
	virtual void OnOK();
	virtual void OnCancel();
	virtual BOOL OnInitDialog();
	void OnChangeName();
	void OnChangeText();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_EDITMESSAGETEMPLATEDLG_H__91A12C95_82CA_48CE_99F8_49385821334E__INCLUDED_)
