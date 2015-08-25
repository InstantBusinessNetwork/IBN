// McAppBar.cpp : implementation file
//

#include "stdafx.h"
#include "ofstv.h"
#include "McAppBar.h"
#include "MemDc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMcAppBar

CMcAppBar::CMcAppBar()
{
}

CMcAppBar::~CMcAppBar()
{
}


BEGIN_MESSAGE_MAP(CMcAppBar, CStatic)
//{{AFX_MSG_MAP(CMcAppBar)
ON_WM_PAINT()
ON_WM_SIZE()
ON_WM_LBUTTONDOWN()
ON_WM_LBUTTONUP()
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMcAppBar message handlers

void CMcAppBar::OnPaint() 
{
	CPaintDC dc(this); // device context for painting

	CRect clientRect;
	GetClientRect(&clientRect);
	
	CRgn rgn;
	rgn.CreateRectRgnIndirect(&clientRect);
	dc.SelectClipRgn(&rgn,RGN_COPY);
	
	CMemoryDC	memDc(&dc);
	memDc.FillSolidRect(&clientRect,m_dwBkgColor);

	m_AppCtrl.Draw(&memDc);
}

void CMcAppBar::OnSize(UINT nType, int cx, int cy) 
{
	CStatic::OnSize(nType, cx, cy);

	if(!m_AppCtrl.IsCreated())
		m_AppCtrl.Create(0,0,this);

	m_AppCtrl.SetMaxShowItem(m_AppCtrl.GetMaxShowItemFromCY(cy));
}

BOOL CMcAppBar::PreTranslateMessage(MSG* pMsg) 
{
	m_AppCtrl.TranslateMessage(pMsg);
	
	return CStatic::PreTranslateMessage(pMsg);
}

void CMcAppBar::OnLButtonDown(UINT nFlags, CPoint point) 
{
	CPoint	TstPoint	=	point;
	ClientToScreen(&TstPoint);
	GetParent()->ScreenToClient(&TstPoint);
	GetParent()->SendMessage(WM_LBUTTONDOWN,(WPARAM)nFlags,MAKELPARAM(TstPoint.x,TstPoint.y));
	
	CStatic::OnLButtonDown(nFlags, point);
}

void CMcAppBar::OnLButtonUp(UINT nFlags, CPoint point) 
{
	CPoint	TstPoint	=	point;
	ClientToScreen(&TstPoint);
	GetParent()->ScreenToClient(&TstPoint);
	GetParent()->SendMessage(WM_LBUTTONUP,(WPARAM)nFlags,MAKELPARAM(TstPoint.x,TstPoint.y));
	
	CStatic::OnLButtonUp(nFlags, point);
}

