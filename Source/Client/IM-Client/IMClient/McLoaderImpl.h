#if !defined(AFX_MCLOADERIMPL_H__3C3E20D9_772A_4A6B_8E1B_B1D84A93394D__INCLUDED_)
#define AFX_MCLOADERIMPL_H__3C3E20D9_772A_4A6B_8E1B_B1D84A93394D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// McLoaderImpl.h : header file
//



/////////////////////////////////////////////////////////////////////////////
// CMcLoaderImpl command target

class CMcLoaderImpl : public CCmdTarget
{
	DECLARE_DYNCREATE(CMcLoaderImpl)

public:
	CMcLoaderImpl(IFormSender	*pFormSender	=	NULL);           
	virtual ~CMcLoaderImpl();
	
	// Operations
public:
	BSTR GetTitle();
	BSTR GetFilePath();
	HRESULT SetXML(BSTR	XML);
	HRESULT Send(HWND Hwnd, UINT ProgressMsg, UINT CompletedMsg, LPCTSTR strHost, LPCTSTR strSID);

	operator IForm*() const
	{
		return m_pForm;
	}

	IForm** operator&()
	{
		return &m_pForm;
	}

	IForm* operator->()
	{
		return m_pForm;
	}
	
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMcLoaderImpl)
	//}}AFX_VIRTUAL
protected:
	IFormPtr			m_pForm;				
	//IMCHTTPLoaderPtr	m_pMcLoader;

	DWORD				dwSessionCookie;
	HWND				m_MsgHwnd;
	UINT				m_ProgressMsg;
	UINT				m_CompletedMsg;

	CComBSTR			m_bsUrl;
	CComBSTR			m_bsFilePath;
	CComBSTR			m_bsTitle;
	CComPtr<IStream>	m_pStreamXML;

	//CComVariant			m_varReceivingData;

	long				m_lFullSize;
	long				m_lUploadSize;

protected:
	void CloseEventContainer();
	void InitEventContainer();

	virtual void OnPushTOQueue();
	virtual void OnUploadEnd();
	virtual void OnUploadStep(long Size);
	virtual void OnUploadBegin(long Size);
	virtual void OnDownloadEnd();
	virtual void OnDownloadStep(long Size);
	virtual void OnDownloadBegin(long Size);
	virtual void OnCompleted( long ErrorType, long ErrorCode);

// Interface Maps
protected:
	
	DECLARE_DISPATCH_MAP()		
	DECLARE_INTERFACE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MCLOADERIMPL_H__3C3E20D9_772A_4A6B_8E1B_B1D84A93394D__INCLUDED_)
