// McWindowDetails.cpp: implementation of the CMcWindowDetails class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "McWindowDetails.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMcWindowDetails::CMcWindowDetails(long Type, CWnd* Wnd)
{
	ASSERT(Wnd!=NULL);
	ASSERT(::IsWindow(Wnd->m_hWnd));
	m_lType		=	Type;
	m_lStyle	=	Wnd->GetStyle();
	m_hWnd		=	Wnd->GetSafeHwnd();
	Wnd->GetWindowRect(&m_Rect);
}

CMcWindowDetails::CMcWindowDetails(const CMcWindowDetails &Src)
{
	*this = Src;
}

CMcWindowDetails::~CMcWindowDetails()
{
	
}

//////////////////////////////////////////////////////////////////////
// Property
//////////////////////////////////////////////////////////////////////


long CMcWindowDetails::GetType() const
{
	return m_lType;
}

long CMcWindowDetails::GetStyle() const
{
	return m_lStyle;
}

HWND CMcWindowDetails::GetHWND() const
{
	return m_hWnd;
}

const CRect& CMcWindowDetails::GetWindowRect() const
{
	return m_Rect;
}

const CMcWindowDetails& CMcWindowDetails::operator=(const CMcWindowDetails &Src)
{
	m_lType		=	Src.GetType();
	m_lStyle	=	Src.GetStyle();
	m_hWnd		=	Src.GetHWND();
	m_Rect		=	Src.GetWindowRect();
	return *this;
}

//////////////////////////////////////////////////////////////////////
// CMcWindowAgent Class
//////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMcWindowAgent::CMcWindowAgent(HWND hWnd):m_hWnd(hWnd)
{

}

CMcWindowAgent::~CMcWindowAgent()
{

}

BOOL CMcWindowAgent::IsValid()
{
	return ::IsWindow(m_hWnd);
}


BOOL CMcWindowAgent::GetDetails(CMcWindowDetails &Details)
{
	ASSERT(IsValid());
	return ::SendMessage(m_hWnd,WM_SWM_GETDETAILS,(WPARAM)&Details,NULL);
}

BOOL CMcWindowAgent::Refresh()
{
	ASSERT(IsValid());
	return ::SendMessage(m_hWnd,WM_SWM_REFRESH,NULL,NULL);
}

BOOL CMcWindowAgent::ShowWindow(int nCmdShow)
{
	ASSERT(IsValid());
	return ::ShowWindow(m_hWnd,nCmdShow);
}

BOOL CMcWindowAgent::SetForegroundWindow()
{
	ASSERT(IsValid());
	return ::SetForegroundWindow(m_hWnd);
}

BOOL CMcWindowAgent::Action(UINT dwActionId, WPARAM WParam, LPARAM LParam)
{
	ASSERT(IsValid());
	return ::SendMessage(m_hWnd,dwActionId,WParam,LParam);
}
