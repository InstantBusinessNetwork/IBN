#if !defined(AFX_MESSAGEDLG_H__64AA05F6_BB69_4D7F_9DEA_21EADE80C488__INCLUDED_)
#define AFX_MESSAGEDLG_H__64AA05F6_BB69_4D7F_9DEA_21EADE80C488__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MessageDlg.h : header file
//
#include "OFSNcDlg.h"
#include "ResizableImage.h"
#include "McButton.h"
#include "Label.h"
/////////////////////////////////////////////////////////////////////////////
// CMessageDlg dialog

class CMessageDlg : public CDialog
{
// Construction
public:
	void SetAutoCloseTime(long SecTime);
	int Show(UINT nType = MB_OK);
	void SetText(LPCTSTR strMessage);
	int Show(LPCTSTR strMessage,UINT nType = MB_OK);
	CMessageDlg(CWnd* pParent = NULL);   // standard constructor
    CMessageDlg(UINT ID,CWnd* pParent = NULL);   

// Dialog Data
	//{{AFX_DATA(CMessageDlg)
	enum { IDD = IDD_MESSAGE_DIALOG };
	CLabel	m_TimeStatic;
	CString		m_MessageText;
	BOOL		m_ShowNext;
	CString		m_strExitTime;
	CButton	m_mb1;
	CButton	m_mb21;
	CButton	m_mb22;
	CButton	m_mbDontShowAgain;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMessageDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	UINT IDResource;
	int ClickButton;
	int ReturnByClick(int Button);
    UINT Type;

	long m_ExitTime;
	// Generated message map functions
	void OnB1();
	void OnB21();
	void OnB22();
	void OnB31();
	void OnB32();
	void OnB33();
	//{{AFX_MSG(CMessageDlg)
	afx_msg void OnMove(int x, int y);
	virtual BOOL OnInitDialog();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnDontshowAgain();
	DECLARE_EVENTSINK_MAP();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MESSAGEDLG_H__64AA05F6_BB69_4D7F_9DEA_21EADE80C488__INCLUDED_)
