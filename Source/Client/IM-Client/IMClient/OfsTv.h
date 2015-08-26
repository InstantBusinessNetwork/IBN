// OfsTv.h : main header file for the OFSTV application
//

#if !defined(AFX_OFSTV_H__FCF991CB_4392_4089_8404_DB499A1F2243__INCLUDED_)
#define AFX_OFSTV_H__FCF991CB_4392_4089_8404_DB499A1F2243__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"       // main symbols
#include "./Inet/CoolInet2.h"
#include "McUpdateWindow.h"
#include "IBNToHandlerWindow.h"


extern CCoolInet2 theNet2;
//extern LoadSkins  m_LoadSkin;

/////////////////////////////////////////////////////////////////////////////
// COfsTvApp:
// See OfsTv.cpp for the implementation of this class
//

class COfsTvApp : public CWinApp
{
public:
	COfsTvApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COfsTvApp)
	public:
	virtual BOOL InitInstance();
	virtual BOOL InitInstanceInternal();
	virtual int ExitInstance();
	virtual int Run();
	//}}AFX_VIRTUAL

// Implementation
protected:
// 	CRITICAL_SECTION CritSect;

public:
	CMcUpdateWindow *m_pUpdateWindow;
	CIBNToHandlerWindow *m_pIbnToHandlerWindow;
	//{{AFX_MSG(COfsTvApp)
	afx_msg void OnAppAbout();
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_OFSTV_H__FCF991CB_4392_4089_8404_DB499A1F2243__INCLUDED_)
