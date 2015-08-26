//{{AFX_INCLUDES()
#include "mcbutton.h"
#include "mediaplayer2.h"
#include "shockwaveflash.h"
//}}AFX_INCLUDES
#if !defined(AFX_DLGTV_H__820FF0B3_CA73_4B33_8F1F_62F8E1343A79__INCLUDED_)
#define AFX_DLGTV_H__820FF0B3_CA73_4B33_8F1F_62F8E1343A79__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "OFSNcDlg2.h"

// DlgTV.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDlgTV dialog
#define DLG_TV_PARENT COFSNcDlg2

class CDlgTV : public DLG_TV_PARENT
{
// Construction
public:
	void MMPlay();
	void MMStop();
	void MMOpen(BSTR URL);

	BOOL CreateAutoKiller(BSTR URL, CWnd *pParentWnd, long ScreenCX = 0, long ScreenCY = 0);
	CDlgTV(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDlgTV)
	enum { IDD = IDD_TV };
	CStatic	m_placeholder;
	CMcButton	m_btnX;
	CMcButton	m_btnRestore;
	CMcButton	m_btnMin;
	CMcButton	m_btnMax;
	CMediaPlayer2	m_mediaplayer;
	CShockwaveFlash	m_flash;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDlgTV)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	void SetFrameSize(long cx, long cy);
	int m_nPlayer;
	void ShowPlayer(int nPlayer);
	int GetPlayer(BSTR URL);
	BOOL m_bAutoKill;
	void LoadSkin(IXMLDOMNode *pXmlRoot);
	BOOL m_bIsClosed;

	// Generated message map functions
	//{{AFX_MSG(CDlgTV)
	virtual void OnOK();
	virtual void OnCancel();
	afx_msg void OnClickButtonX();
	afx_msg void OnClickButtonRestore();
	afx_msg void OnClickButtonMin();
	afx_msg void OnClickButtonMax();
	afx_msg void OnClose();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DLGTV_H__820FF0B3_CA73_4B33_8F1F_62F8E1343A79__INCLUDED_)
