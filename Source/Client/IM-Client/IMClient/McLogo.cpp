// McLogo.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "McLogo.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMcLogo

CMcLogo::CMcLogo()
{
	m_BkgColor = 0;
}

CMcLogo::~CMcLogo()
{
}


BEGIN_MESSAGE_MAP(CMcLogo, CStatic)
//{{AFX_MSG_MAP(CMcLogo)
ON_WM_PAINT()
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMcLogo message handlers

void CMcLogo::OnPaint() 
{
	CPaintDC dc(this); // device context for painting
	
	CRect	Client;
	GetClientRect(&Client);

	dc.SetBoundsRect(Client,DCB_SET);

	CRgn	reg;
	reg.CreateRectRgnIndirect(Client);
	
	dc.SelectClipRgn(&reg);

//	CRect rendRect(CPoint((Client.Width()-227)/2,0),CSize(227,36)), 
//		rectLeft(0,0,(Client.Width()-227)/2,36),
//		rectRight((Client.Width()-227)/2+227,0,Client.Width(),36);
	CRect rendRect(CPoint(0, 0), CSize(227,36)),
		rectRight(227, 0, Client.Width(), 36);
	
	//dc.FillSolidRect(rectLeft,m_BkgColor);
	dc.FillSolidRect(rectRight, m_BkgColor);
	if(m_Image.m_pPict)
		m_Image.Render(&dc, rendRect, rendRect);
	else
		dc.FillSolidRect(rendRect, m_BkgColor);
}
