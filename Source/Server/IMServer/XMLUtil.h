// XMLUtil.h: interface for the CXMLUtil class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_XMLUTIL_H__1B1B3283_68E1_455F_9F66_F82FA95C1D03__INCLUDED_)
#define AFX_XMLUTIL_H__1B1B3283_68E1_455F_9F66_F82FA95C1D03__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
class CHGLOBAL
{
public:
	CHGLOBAL()
	{m_hGlobal = NULL;};

	CHGLOBAL(HGLOBAL hGlobal)
	{m_hGlobal = hGlobal;};

	~CHGLOBAL()
	{
		Free();
	};

	HGLOBAL* operator&()
	{
		return &m_hGlobal;
	};

	void Free()
	{
		if(m_hGlobal != NULL)
		{
			GlobalFree(m_hGlobal);
			m_hGlobal = NULL;
		}
	}

	LPVOID Lock()
	{
		ATLASSERT(m_hGlobal != NULL);
		if(m_hGlobal != NULL)
			return GlobalLock(m_hGlobal);
		else
			return NULL;
	};

	void UnLock()
	{
		ATLASSERT(m_hGlobal != NULL);
		if(m_hGlobal != NULL)
			GlobalUnlock(m_hGlobal);
	};

	SIZE_T getSize()
	{
		ATLASSERT(m_hGlobal != NULL);
		if(m_hGlobal != NULL)
			return GlobalSize(m_hGlobal);
		else
			return 0;
	};
private:
	HGLOBAL m_hGlobal;
};
class CXMLUtil
{
public:
	static HRESULT XML2HGlobal(IXMLDOMNode* pNode, HGLOBAL* lphGlobal, LPDWORD pdwBufferSize);
	static HRESULT XML2MEM(IXMLDOMNode* pNode, PBYTE* pBuffer, LPDWORD pdwBufferSize);
	static HRESULT AppendNode(IXMLDOMNode* pNode, BSTR bsNodeName, BSTR bsURI = NULL, IXMLDOMNode** lpOutNode = NULL, BSTR bsText = NULL);
	static HRESULT SetFreeThreadedClassFactory();

	static HRESULT MEM2XML(IXMLDOMDocument* pDoc, PBYTE pBuffer, DWORD dwBufferSize);
	static HRESULT GetValueByPath(IXMLDOMNode *pNode, BSTR bsPath, LONG *lpValue);
	static HRESULT GetTextByPath(IXMLDOMNode* pNode, BSTR bsPath, BSTR* lpbsText);
	static CComPtr<IClassFactory> m_pClassFactory;

	CXMLUtil();
	virtual ~CXMLUtil();
};

#endif // !defined(AFX_XMLUTIL_H__1B1B3283_68E1_455F_9F66_F82FA95C1D03__INCLUDED_)
