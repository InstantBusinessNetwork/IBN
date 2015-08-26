// Machine generated IDispatch wrapper class(es) created by Microsoft Visual C++

// NOTE: Do not modify the contents of this file.  If this class is regenerated by
//  Microsoft Visual C++, your modifications will be overwritten.

/////////////////////////////////////////////////////////////////////////////
// CMcTicker wrapper class
#ifndef _CMCTICKER_H
#define _CMCTICKER_H

class CMcTicker : public CWnd
{
protected:
	DECLARE_DYNCREATE(CMcTicker)
public:
	CLSID const& GetClsid()
	{
		static CLSID const clsid
			= { 0x91cbb1be, 0xbdf, 0x4b6e, { 0xb1, 0x7a, 0x31, 0x95, 0xd9, 0xef, 0xdf, 0x83 } };
		return clsid;
	}
	virtual BOOL Create(LPCTSTR lpszClassName,
		LPCTSTR lpszWindowName, DWORD dwStyle,
		const RECT& rect,
		CWnd* pParentWnd, UINT nID,
		CCreateContext* pContext = NULL)
	{ return CreateControl(GetClsid(), lpszWindowName, dwStyle, rect, pParentWnd, nID); }

    BOOL Create(LPCTSTR lpszWindowName, DWORD dwStyle,
		const RECT& rect, CWnd* pParentWnd, UINT nID,
		CFile* pPersist = NULL, BOOL bStorage = FALSE,
		BSTR bstrLicKey = NULL)
	{ return CreateControl(GetClsid(), lpszWindowName, dwStyle, rect, pParentWnd, nID,
		pPersist, bStorage, bstrLicKey); }

// Attributes
public:
	long GetBgColor();
	void SetBgColor(long);
	long GetSpeed();
	void SetSpeed(long);

// Operations
public:
	BOOL AddHead(LPCTSTR text, long color, LPCTSTR url);
	BOOL AddTail(LPCTSTR text, long color, LPCTSTR url);
	BOOL SetText(LPCTSTR text, long color, LPCTSTR url);
	void Scroll(long bEnable);
	CString GetStringUrl(long x, long y);
	BOOL Load(const VARIANT& url);
	long LoadFromString(LPCTSTR Data);
	long SetFont(long height, LPCTSTR name, long weight);
};

#endif // _CMCTICKER_H