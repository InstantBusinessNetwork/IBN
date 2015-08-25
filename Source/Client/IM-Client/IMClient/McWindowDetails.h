// McWindowDetails.h: interface for the CMcWindowDetails class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MCWINDOWDETAILS_H__F3F9EEF6_4EA2_495E_A031_39B1D1B01E82__INCLUDED_)
#define AFX_MCWINDOWDETAILS_H__F3F9EEF6_4EA2_495E_A031_39B1D1B01E82__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

enum _ENUM_SUPPORT_WINDETAILS_MESSAGE
{
	WM_SWM_GETDETAILS = WM_USER + 0x1000,
	WM_SWM_REFRESH,
	WM_SWM_SETBODY
};

class CMcWindowDetails  
{
public:
	const CRect& GetWindowRect() const;
	HWND GetHWND() const;
	long GetStyle() const;
	long GetType() const;
	CMcWindowDetails(long Type, CWnd* Wnd);
	CMcWindowDetails(const CMcWindowDetails &Src);
	virtual ~CMcWindowDetails();
	const CMcWindowDetails& operator=(const CMcWindowDetails &Src);
private:
	long	m_lType;
	long	m_lStyle;
	HWND	m_hWnd;
	CRect	m_Rect;
};


class CMcWindowAgent  
{
public:
	CMcWindowAgent(HWND hWnd);
	virtual ~CMcWindowAgent();
public:
	BOOL Action(UINT dwActionId, WPARAM WParam = NULL, LPARAM LParam = NULL);
	BOOL SetForegroundWindow();
	BOOL ShowWindow(int nCmdShow);
	BOOL Refresh();
	BOOL GetDetails(CMcWindowDetails &Details);
	BOOL IsValid();
private:
	HWND	m_hWnd;
};

#endif // !defined(AFX_MCWINDOWDETAILS_H__F3F9EEF6_4EA2_495E_A031_39B1D1B01E82__INCLUDED_)
