// Machine generated IDispatch wrapper class(es) created by Microsoft Visual C++

// NOTE: Do not modify the contents of this file.  If this class is regenerated by
//  Microsoft Visual C++, your modifications will be overwritten.

/////////////////////////////////////////////////////////////////////////////
// CMpaDhtmlCtrl wrapper class

class CMpaDhtmlCtrl : public CWnd
{
protected:
	DECLARE_DYNCREATE(CMpaDhtmlCtrl)
public:
	CLSID const& GetClsid()
	{
		static CLSID const clsid
			= { 0x8b398fb3, 0x518e, 0x42a6, { 0xa7, 0x40, 0x5a, 0x84, 0xcb, 0x29, 0x59, 0xce } };
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

// Operations
public:
	void LoadFromStream(LPCTSTR bstrFrame, LPUNKNOWN pUnkStream);
	void Navigate2(VARIANT* URL, VARIANT* Flags, VARIANT* TargetFrameName, VARIANT* PostData, VARIANT* Headers);
	void Navigate(LPCTSTR URL, VARIANT* Flags, VARIANT* TargetFrameName, VARIANT* PostData, VARIANT* Headers);
	void PutHtmlInElement(LPDISPATCH pdispElement, LPCTSTR bstrHtml);
	void GoBack();
	void GoForward();
	void Stop();
};