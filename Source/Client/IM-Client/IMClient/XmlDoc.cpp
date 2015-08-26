// XmlDoc.cpp: implementation of the CXmlDoc class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "XmlDoc.h"
#include <msxml2.h>
#include <comdef.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

BEGIN_DISPATCH_MAP(CXmlDoc, CCmdTarget)
	DISP_FUNCTION_ID(CXmlDoc, "Ready State Event Handler", DISPID_READYSTATECHANGE, OnReadyStateChange, VT_EMPTY, 0)
END_DISPATCH_MAP()

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CXmlDoc::CXmlDoc()
{
	EnableAutomation();
	m_bInited = FALSE;
	m_dwCookie = 0;
	m_pDoc = NULL;
	m_hWnd = 0;
	m_msg = 0;
	hThread = NULL;
}

CXmlDoc::~CXmlDoc()
{
	Abort();
	WaitForComplete();

	if(m_dwCookie)
		AdviseConnectionPoint(FALSE);
	if(m_pDoc)
	{
		m_pDoc->Release();
		m_pDoc = NULL;
	}
	CloseHandle(hThread);
}

BOOL CXmlDoc::AdviseConnectionPoint(BOOL bAdvise)
{
	BOOL bResult = FALSE;
	IConnectionPointContainer *pIConnectionPointContainer = NULL;
	IConnectionPoint *pCP = NULL;
	
	try
	{
		m_hr = m_pDoc->QueryInterface(IID_IConnectionPointContainer, (void**)&pIConnectionPointContainer);
		SUCCEEDED(m_hr) ? 0 : throw m_hr;
		
		if(pIConnectionPointContainer)
		{
			m_hr = pIConnectionPointContainer->FindConnectionPoint(DIID_XMLDOMDocumentEvents, &pCP);
			SUCCEEDED(m_hr) ? 0 : throw m_hr;
			if(SUCCEEDED(m_hr) && pCP)
			{
				if(bAdvise)
				{
					IDispatch *pDisp = GetIDispatch(TRUE);
					m_hr = pCP->Advise((IUnknown*)pDisp, &m_dwCookie);
				}
				else
					m_hr = pCP->Unadvise(m_dwCookie);
				bResult = SUCCEEDED(m_hr) ? TRUE : FALSE;
				pCP->Release();
				pCP = NULL;
			}
			pIConnectionPointContainer->Release();
			pIConnectionPointContainer = NULL;
		}
	}
	catch(...)
	{
		if(pIConnectionPointContainer)
			pIConnectionPointContainer->Release();
		if(pCP)
			pCP->Release();
	}
	return bResult;
}

HRESULT CXmlDoc::OnReadyStateChange()
{
	long value;
	IXMLDOMParseError *pIParseError = NULL;
	HRESULT m_hr = E_FAIL;
	
	try
	{
		m_hr = m_pDoc->get_readyState(&value);
		SUCCEEDED(m_hr) ? 0 : throw m_hr;
		if(value == 4 )
		{
			m_hr = m_pDoc->get_parseError(&pIParseError);
			SUCCEEDED(m_hr) ? 0 : throw m_hr;
			BSTR bs;
			m_hr = pIParseError->get_reason(&bs);
//			if(SUCCEEDED(m_hr))
//			{
//				_bstr_t str(bs);
//				::MessageBox(GetDesktopWindow(), str, _T("XML Parser"), MB_OK);
//			}
			m_hr = pIParseError->get_errorCode(&value);
			pIParseError->Release();
			pIParseError = NULL;
			if(!SUCCEEDED(m_hr))
			{
				throw m_hr;
			}

			if(m_hWnd && m_msg)
			{
				::PostMessage(m_hWnd, m_msg, (WPARAM)value, 0);
			}
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
		if(m_hWnd && m_msg)
		{
			::PostMessage(m_hWnd, m_msg, (WPARAM)m_hr, 0);
		}
		
		return m_hr;
	}
	return S_OK;
}

BOOL CXmlDoc::Init(HWND hWnd, UINT msg)
{
	if(!hWnd || !msg)
		return FALSE;

	m_hWnd = hWnd;
	m_msg = msg;

	if(m_bInited)
		return TRUE;
	
	m_hr = CoCreateInstance(CLSID_FreeThreadedDOMDocument40, NULL, CLSCTX_SERVER, IID_IXMLDOMDocument, (void**)&m_pDoc);
	if(FAILED(m_hr) || !m_pDoc)
		return FALSE;
	
	if(!AdviseConnectionPoint(TRUE))
	{
		m_pDoc->Release();
		m_pDoc = NULL;
		return FALSE;
	}

	m_bInited = TRUE;
	
	return TRUE;
}

BOOL CXmlDoc::Load(LPCTSTR szUrl)
{
	if(!m_bInited || !m_pDoc || !m_dwCookie || !lstrlen(szUrl))
		return FALSE;
	
	m_strUrl = szUrl;
	DWORD threadID;

	if(hThread = CreateThread(NULL, 0, WorkThread, this, 0, &threadID))
	{
		return TRUE;
	}
		

	return FALSE;
}

HRESULT CXmlDoc::GetNodeText(LPCTSTR szNodeName, CString &strNodeText, long index)
{
	strNodeText.Empty();
	if(!m_pDoc)
		return E_FAIL;

	IXMLDOMNode *pRoot = NULL, *pNode = NULL;
	IXMLDOMNodeList *pList = NULL;
	BSTR bstr;
	
	m_hr = m_pDoc->QueryInterface(IID_IXMLDOMNode, (void**)&pRoot);
	if(SUCCEEDED(m_hr) && pRoot)
	{
		m_hr = pRoot->selectNodes(_bstr_t(szNodeName), &pList);
		if(SUCCEEDED(m_hr) && pList)
		{
			m_hr = pList->get_item(index, &pNode);
			if(SUCCEEDED(m_hr) && pNode)
			{
				m_hr = pNode->get_text(&bstr);
				if(SUCCEEDED(m_hr))
				{
					strNodeText = bstr;
				}
				pNode->Release();
			}
			pList->Release();
		}
		pRoot->Release();
	}
	return m_hr;
}

HRESULT CXmlDoc::GetNodeXML(LPCTSTR szNodeName, CString &strNodeXML, long index)
{
	strNodeXML.Empty();
	if(!m_pDoc)
		return E_FAIL;
	
	IXMLDOMNode *pRoot = NULL, *pNode = NULL;
	IXMLDOMNodeList *pList = NULL;
	BSTR bstr;
	
	m_hr = m_pDoc->QueryInterface(IID_IXMLDOMNode, (void**)&pRoot);
	if(SUCCEEDED(m_hr) && pRoot)
	{
		m_hr = pRoot->selectNodes(_bstr_t(szNodeName), &pList);
		if(SUCCEEDED(m_hr) && pList)
		{
			m_hr = pList->get_item(index, &pNode);
			if(SUCCEEDED(m_hr) && pNode)
			{
				m_hr = pNode->get_xml(&bstr);
				if(SUCCEEDED(m_hr))
				{
					strNodeXML = bstr;
				}
				pNode->Release();
			}
			pList->Release();
		}
		pRoot->Release();
	}
	return m_hr;
}

HRESULT CXmlDoc::GetNodeAttribute(LPCTSTR szNodeName, LPCTSTR szAttributeName, CString &strAttributeValue, long index)
{
	strAttributeValue.Empty();
	if(!m_pDoc)
		return E_FAIL;
	
	IXMLDOMNode *pRoot = NULL, *pNode = NULL;
	IXMLDOMNodeList *pList = NULL;
	IXMLDOMElement *pEle = NULL;
	CComVariant var;
	
	m_hr = m_pDoc->QueryInterface(IID_IXMLDOMNode, (void**)&pRoot);
	if(SUCCEEDED(m_hr) && pRoot)
	{
		m_hr = pRoot->selectNodes(_bstr_t(szNodeName), &pList);
		if(SUCCEEDED(m_hr) && pList)
		{
			m_hr = pList->get_item(index, &pNode);
			if(SUCCEEDED(m_hr) && pNode)
			{
				m_hr = pNode->QueryInterface(IID_IXMLDOMElement, (void**)&pEle);
				if(SUCCEEDED(m_hr) && pEle)
				{
					m_hr = pEle->getAttribute(_bstr_t(szAttributeName),&var);
					if(SUCCEEDED(m_hr))
					{
						strAttributeValue = CString(var.bstrVal);
					}
					pEle->Release();
				}
				pNode->Release();
			}
			pList->Release();
		}
		pRoot->Release();
	}
	return m_hr;
}

DWORD CXmlDoc::WorkThread(LPVOID p)
{
	((CXmlDoc*)p)->WorkFunction();
	return 0;
}

void CXmlDoc::WorkFunction()
{
	HRESULT hr = S_FALSE;
	IStream *pStream = NULL;

	hr = m_http.Load(m_strUrl, &pStream);
	if(SUCCEEDED(hr) && pStream)
	{
		VARIANT_BOOL vbResult;
		m_hr = m_pDoc->load(CComVariant(pStream), &vbResult);
		pStream->Release();
		if(FAILED(m_hr) || vbResult != VARIANT_TRUE)
		{
			::PostMessage(m_hWnd, m_msg, (WPARAM)GetError(), 0);
		}
	}
	else
		::PostMessage(m_hWnd, m_msg, (WPARAM)hr, 0);

}

long CXmlDoc::GetError()
{
	HRESULT hr;
	IXMLDOMParseError *pIParseError = NULL;
	long value = E_FAIL;
	
	if(m_pDoc)
	{
		hr = m_pDoc->get_parseError(&pIParseError);
		if(SUCCEEDED(hr) && pIParseError)
		{
			m_hr = pIParseError->get_errorCode(&value);
			pIParseError->Release();
		}
	}
	return value;
}

HRESULT CXmlDoc::GetListItem(LPCTSTR szNodeName, long index, LPCTSTR szChildNode, CString &strValue)
{
	strValue.Empty();
	if(!m_pDoc)	return E_FAIL;
	
	IXMLDOMNode *pRoot = NULL, *pNode = NULL;
	IXMLDOMNodeList *pList = NULL;
	BSTR bstr;
	
	m_hr = m_pDoc->QueryInterface(IID_IXMLDOMNode, (void**)&pRoot);
	if(SUCCEEDED(m_hr) && pRoot)
	{
		m_hr = pRoot->selectNodes(_bstr_t(szNodeName), &pList);
		if(SUCCEEDED(m_hr) && pList)
		{
			long Lenght=0;
			m_hr = pList->get_length(&Lenght);
			if(SUCCEEDED(m_hr)&&index<Lenght)
			{
				m_hr = pList->get_item(index, &pNode);
				if(SUCCEEDED(m_hr) && pNode)
				{
					IXMLDOMNode *pTmpNode = NULL;
					m_hr = pNode->selectSingleNode(bstr_t(szChildNode),&pTmpNode);
					if(SUCCEEDED(m_hr)&&pTmpNode)
					{
						m_hr = pTmpNode->get_text(&bstr);
						strValue = bstr;
						pTmpNode->Release();
					}
					pNode->Release();
				}
			}
			else
				m_hr = E_FAIL;
			pList->Release();
		}
		pRoot->Release();
	}
	return m_hr;
}

HRESULT CXmlDoc::GetXML(BSTR* xml)
{
	if(m_pDoc)
	{
		HRESULT hr = m_pDoc->get_xml(xml);
			
		return hr;
	}

	return E_FAIL;
}

//////////////////////////////////////////////////////////////////////////
CStreamDoc::CStreamDoc()
{
	m_pDoc = NULL;
	m_hWnd = 0;
	m_msg = 0;

	hThread = NULL;
}


CStreamDoc::~CStreamDoc()
{
	Abort();
	WaitForComplete();
	CloseHandle(hThread);
	if(m_pDoc)
	{
		m_pDoc->Release();
		m_pDoc = NULL;
	}
}

HRESULT CStreamDoc::GetStream(LPSTREAM *pStream)
{
	if(m_pDoc)
	{
		return m_pDoc->QueryInterface(IID_IStream,(void**)pStream);
	}
	return E_FAIL;
}

void CStreamDoc::WorkFunction()
{
	HRESULT hr = S_FALSE;
	IStream *pStream = NULL;
	
	hr = m_http.Load(m_strUrl, &pStream);
	if(SUCCEEDED(hr) && pStream)
	{
		hr = pStream->QueryInterface(IID_IStream,(void**)&m_pDoc);
		::PostMessage(m_hWnd, m_msg, (WPARAM)hr, 0);
		pStream->Release();
	}
	else
		::PostMessage(m_hWnd, m_msg, (WPARAM)hr, 0);
}

BOOL CStreamDoc::Init(HWND hWnd, UINT msg)
{
	if(!hWnd || !msg)
		return FALSE;
	
	m_hWnd = hWnd;
	m_msg = msg;
	
	return TRUE;
}

BOOL CStreamDoc::Load(LPCTSTR szUrl)
{
	if(!lstrlen(szUrl))
		return FALSE;
	
	m_strUrl		=	szUrl;
	DWORD threadID	=	0;

	if(hThread = CreateThread(NULL, 0, WorkThread, this, 0, &threadID))
	{
		return TRUE;
	}
	
	return FALSE;
}

DWORD WINAPI CStreamDoc::WorkThread(LPVOID p)
{
	((CStreamDoc*)p)->WorkFunction();
	return 0;
}


HRESULT CXmlDoc::GetNodeData(LPCTSTR szNodeName, VARIANT *varNodeData, long index)
{
	if(!m_pDoc)
		return E_FAIL;
	
	IXMLDOMNode *pRoot = NULL, *pNode = NULL;
	IXMLDOMNodeList *pList = NULL;

	m_hr = m_pDoc->QueryInterface(IID_IXMLDOMNode, (void**)&pRoot);
	if(SUCCEEDED(m_hr) && pRoot)
	{
		m_hr = pRoot->selectNodes(_bstr_t(szNodeName), &pList);
		if(SUCCEEDED(m_hr) && pList)
		{
			m_hr = pList->get_item(index, &pNode);
			if(SUCCEEDED(m_hr) && pNode)
			{
				m_hr = pNode->get_nodeTypedValue(varNodeData);
				pNode->Release();
			}
			pList->Release();
		}
		pRoot->Release();
	}
	return m_hr;
}

void CXmlDoc::Abort()
{
	m_http.Abort();
}

void CXmlDoc::WaitForComplete(DWORD Timeout)
{
	DWORD dwValue = WaitForSingleObject(hThread,Timeout);
	if(dwValue==WAIT_TIMEOUT)
	{
		TerminateThread(hThread,2);
	}
}

void CStreamDoc::Abort()
{
	m_http.Abort();
}

void CStreamDoc::WaitForComplete(DWORD Timeout)
{
	DWORD dwValue = WaitForSingleObject(hThread,Timeout);
	if(dwValue==WAIT_TIMEOUT)
	{
		TerminateThread(hThread,2);
	}
}
