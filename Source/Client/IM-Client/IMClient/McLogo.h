#if !defined(AFX_MCLOGO_H__0C7B2BD1_9494_4947_8110_97A841A0833D__INCLUDED_)
#define AFX_MCLOGO_H__0C7B2BD1_9494_4947_8110_97A841A0833D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// McLogo.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CMcLogo window

class CMcLogo : public CStatic
{
// Construction
public:
	CMcLogo();

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMcLogo)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CMcLogo();

	// Generated message map functions
protected:
	//{{AFX_MSG(CMcLogo)
	afx_msg void OnPaint();
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
public:
	COLORREF m_BkgColor;
	CPictureHolder m_Image;
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MCLOGO_H__0C7B2BD1_9494_4947_8110_97A841A0833D__INCLUDED_)
