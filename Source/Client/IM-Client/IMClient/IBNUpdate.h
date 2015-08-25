#if !defined(AFX_IBNUPDATE_H__74FE0997_2A18_4888_AA72_882FA7C9E85F__INCLUDED_)
#define AFX_IBNUPDATE_H__74FE0997_2A18_4888_AA72_882FA7C9E85F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// IBNUpdate.h : header file
//
#include "FileDownloader.h"

/////////////////////////////////////////////////////////////////////////////
// CIBNUpdate dialog

class CIBNUpdate : public CDialog
{
// Construction
public:
	void InitBeforCreate(LPCTSTR strMcUpdateURL, LPCTSTR strMcUpdatePath, LPCTSTR strMcUpdateParam);
	CIBNUpdate(CWnd* pParent = NULL);   // standard constructor
	~CIBNUpdate();

// Dialog Data
	//{{AFX_DATA(CIBNUpdate)
	enum { IDD = IDD_MCUPDATE_DIALOG };
	CStatic	m_stDownload;
	CStatic	m_stStart;
	CAnimateCtrl	m_Anim;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CIBNUpdate)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	CFileDownloader		m_McUpdateDownload;
	CString				m_strMcUpdateURL;
	CString				m_strMcUpdatePath;
	CString				m_strMcUpdateParam;

	// Generated message map functions
	//{{AFX_MSG(CIBNUpdate)
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	afx_msg void OnStartButton();
	virtual void OnCancel();
	afx_msg void OnClose();
	//}}AFX_MSG
	LRESULT OnMcUpdateLoaded(WPARAM w, LPARAM l);
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_IBNUPDATE_H__74FE0997_2A18_4888_AA72_882FA7C9E85F__INCLUDED_)
