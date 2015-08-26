#if !defined(AFX_FILEDESCRIPTIODLG_H__BF321A70_55A1_4B3B_89B6_E35C8A16EDFC__INCLUDED_)
#define AFX_FILEDESCRIPTIODLG_H__BF321A70_55A1_4B3B_89B6_E35C8A16EDFC__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// FileDescriptioDlg.h : header file
//
#include "OfsNcDlg.h"
#include "ResizableImage.h"
#include "McButton.h"
#include "Label.h"

/////////////////////////////////////////////////////////////////////////////
// CFileDescriptioDlg dialog

class CFileDescriptioDlg : public CResizableDialog
{
// Construction
public:
	int DoModalEditMode(LPCTSTR Text = _T(""));
	int DoModalReadMode(LPCTSTR Text = _T(""));
	LPCTSTR GetDescription();
	CFileDescriptioDlg(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CFileDescriptioDlg)
	enum { IDD = IDD_FILEDESCRIPTION_DIALOG };
	CButton	m_btnSend;
	CLabel	m_FileName;
	CEdit	m_ctrlDescription;
	CString	m_strFileDescription;
	CString	m_strFileName;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFileDescriptioDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	BOOL m_bReadOnlyMode;

	CResizableImage		m_ResizeFon;
	// Generated message map functions
	//{{AFX_MSG(CFileDescriptioDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FILEDESCRIPTIODLG_H__BF321A70_55A1_4B3B_89B6_E35C8A16EDFC__INCLUDED_)
