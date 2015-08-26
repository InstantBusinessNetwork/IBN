#if !defined(AFX_MCAPPBAR_H__006CAA31_1539_4064_B07E_80946641CA3D__INCLUDED_)
#define AFX_MCAPPBAR_H__006CAA31_1539_4064_B07E_80946641CA3D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// McAppBar.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CMcAppBar window

#include "McAppCtrl.h"

class CMcAppBar : public CStatic
{
// Construction
public:
	CMcAppBar();

// Attributes
public:
	CMcAppCtrl m_AppCtrl;
	COLORREF	m_dwBkgColor;
// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMcAppBar)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	//}}AFX_VIRTUAL

// Implementation
public:
	
	virtual ~CMcAppBar();

	// Generated message map functions
protected:
	//{{AFX_MSG(CMcAppBar)
	afx_msg void OnPaint();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MCAPPBAR_H__006CAA31_1539_4064_B07E_80946641CA3D__INCLUDED_)
