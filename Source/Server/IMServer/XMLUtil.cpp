// XMLUtil.cpp: implementation of the CXMLUtil class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "XMLUtil.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
CComPtr<IClassFactory> CXMLUtil::m_pClassFactory;

CXMLUtil::CXMLUtil()
{

}

CXMLUtil::~CXMLUtil()
{

}

HRESULT CXMLUtil::GetTextByPath(IXMLDOMNode *pNode, BSTR bsPath, BSTR *lpbsText)
{
	HRESULT hr = E_INVALIDARG;

	ATLASSERT(pNode != NULL);
	ATLASSERT(lpbsText != NULL);

	if(pNode != NULL && lpbsText != NULL)
	{
		ATLASSERT(*lpbsText == NULL);
		if(*lpbsText == NULL)
		{
			CComPtr<IXMLDOMNode> pTempNode;
			hr = pNode->selectSingleNode(bsPath,&pTempNode);
			if(hr == S_OK)
			{
				hr = pTempNode->get_text(lpbsText);
			}
		}
	}

	return hr;
}

HRESULT CXMLUtil::GetValueByPath(IXMLDOMNode *pNode, BSTR bsPath, LONG *lpValue)
{
	HRESULT hr = E_INVALIDARG;

	ATLASSERT(pNode != NULL);
	ATLASSERT(lpValue != NULL);

	if(pNode != NULL && lpValue != NULL)
	{
		CComPtr<IXMLDOMNode> pTempNode;
		hr = pNode->selectSingleNode(bsPath, &pTempNode);
		if(hr == S_OK)
		{
			CComBSTR bsText;
			hr = pTempNode->get_text(&bsText);
			if(hr == S_OK)
			{
				*lpValue = _wtol(bsText);
			}
		}
	}

	return S_OK;
}

HRESULT CXMLUtil::MEM2XML(IXMLDOMDocument* pDoc, PBYTE pBuffer, DWORD dwBufferSize)
{
	HRESULT hr = E_INVALIDARG;

	ATLASSERT(pDoc != NULL);
	ATLASSERT(pBuffer != NULL);
	ATLASSERT(dwBufferSize != 0);

	if(pDoc != NULL && pBuffer != NULL && dwBufferSize != 0)
	{
		try
		{
			CComPtr<IPersistStreamInit> pPsi;
			CComPtr<IStream> pStm;
			ULONG ulWritten;
			LARGE_INTEGER li = {0, 0};

			hr = CreateStreamOnHGlobal(NULL, TRUE, &pStm);
			if(hr == S_OK)
			{
				hr = pStm->Write(pBuffer, dwBufferSize, &ulWritten);
				if(hr == S_OK)
				{
					hr = pStm->Seek(li, STREAM_SEEK_SET, NULL);
					if(hr == S_OK)
					{
						hr = pDoc->QueryInterface(IID_IPersistStreamInit, (void **)&pPsi);
						if(hr == S_OK)
						{
							hr = pPsi->Load(pStm);
						}
					}
				}
			}
		}
		catch(...)
		{
		}
	}

	return hr;
}

HRESULT CXMLUtil::SetFreeThreadedClassFactory()
{
	ATLASSERT(m_pClassFactory == NULL);
	HRESULT hr = S_OK;
	CComPtr<IUnknown> pUnknown;

	hr = CoGetClassObject(__uuidof(FreeThreadedDOMDocument),// CLSID_FreeThreadedDOMDocument,
		CLSCTX_INPROC,
		NULL,
		IID_IUnknown,
		(void**)&pUnknown);
	if(hr != S_OK) return hr;

	return pUnknown->QueryInterface(IID_IClassFactory,(void**)&m_pClassFactory);
}

HRESULT CXMLUtil::AppendNode(IXMLDOMNode *pNode, BSTR bsNodeName, BSTR bsURI, IXMLDOMNode **lpOutNode, BSTR bsText)
{
	HRESULT hr = S_OK;
	CComPtr<IXMLDOMDocument> pDoc;
	CComPtr<IXMLDOMNode> pTempNode;

	hr = pNode->get_ownerDocument(&pDoc);

	//if(hr == S_OK && pDoc == NULL) fixed by Artyom
	if(SUCCEEDED(hr) && pDoc == NULL)
		hr = pNode->QueryInterface(IID_IXMLDOMDocument,(void**)&pDoc);

	if(hr != S_OK) return hr;

	hr = pDoc->createNode(CComVariant(NODE_ELEMENT),bsNodeName,bsURI,&pTempNode);
	if(hr != S_OK) return hr;

	if(bsText != NULL)
	{
		hr = pTempNode->put_text(bsText);
		if(hr != S_OK) return hr;
	}

	return pNode->appendChild(pTempNode,lpOutNode);
}

HRESULT CXMLUtil::XML2MEM(IXMLDOMNode* pNode, PBYTE* pBuffer, LPDWORD pdwBufferSize)
{
	HRESULT hr = E_INVALIDARG;

	ATLASSERT(pNode != NULL);
	ATLASSERT(pBuffer != NULL);
	ATLASSERT(pdwBufferSize != NULL);

	if(pNode != NULL && pBuffer != NULL && pdwBufferSize != NULL)
	{
		ATLASSERT(*pBuffer == NULL);
		if(*pBuffer == NULL)
		{
			CComPtr<IXMLDOMDocument> pDoc;
			CComPtr<IPersistStreamInit> pPsi;
			CComPtr<IStream> pStm;

			LARGE_INTEGER li = {0, 0};
			HGLOBAL hGlobal;
			ULARGE_INTEGER uli;

			try
			{
				hr = pNode->get_ownerDocument(&pDoc);
				if(SUCCEEDED(hr))
				{
					// Create stream in global memory and save the xml document to it
					hr = CreateStreamOnHGlobal(NULL, TRUE, &pStm);
					if(hr == S_OK)
					{
						hr = pDoc->QueryInterface(IID_IPersistStreamInit, (void **)&pPsi);
						if(hr == S_OK)
						{
							hr = pPsi->Save(pStm, TRUE);
							if(hr == S_OK)
							{
								hr = pStm->Seek(li, STREAM_SEEK_CUR, &uli);
								if(hr == S_OK)
								{
									*pdwBufferSize = uli.LowPart;

									hr = GetHGlobalFromStream(pStm, &hGlobal);
									if(hr == S_OK)
									{
										*pBuffer = new BYTE[*pdwBufferSize];
										if (*pBuffer != NULL)
										{
											LPVOID pData = GlobalLock(hGlobal);
											if(pData != NULL)
											{
												::memcpy(*pBuffer, pData, *pdwBufferSize);
												GlobalUnlock(hGlobal);
											}
											else
												hr = HRESULT_FROM_WIN32(GetLastError());
										}
										else
											hr = E_OUTOFMEMORY;
									}
								}
							}
						}
					}
				}
			}
			catch(...)
			{
			}
		}
	}

	return hr;
}

HRESULT CXMLUtil::XML2HGlobal(IXMLDOMNode* pNode, HGLOBAL* lphGlobal, LPDWORD pdwBufferSize)
{
	HRESULT hr = E_INVALIDARG;

	ATLASSERT(lphGlobal != NULL);
	ATLASSERT(pdwBufferSize != NULL);

	if(lphGlobal != NULL && pdwBufferSize != NULL)
	{
		*pdwBufferSize = 0;

		if(pNode == NULL)
		{
			hr = S_OK;
		}
		else
		{
			CComPtr<IXMLDOMDocument> pDoc;
			CComPtr<IPersistStreamInit> pPsi;
			CComPtr<IStream> pStm;

			LARGE_INTEGER li = {0, 0};
			ULARGE_INTEGER uli;

			try
			{
				hr = pNode->get_ownerDocument(&pDoc);
				if(SUCCEEDED(hr))
				{
					hr = CreateStreamOnHGlobal(NULL, FALSE, &pStm);
					if(hr == S_OK)
					{
						hr = pDoc->QueryInterface(IID_IPersistStreamInit, (void **)&pPsi);
						if(hr == S_OK)
						{
							hr = pPsi->Save(pStm, TRUE);
							if(hr == S_OK)
							{
								hr = pStm->Seek(li, STREAM_SEEK_CUR, &uli);
								if(hr == S_OK)
								{
									*pdwBufferSize = uli.LowPart;

									hr = GetHGlobalFromStream(pStm, lphGlobal);
								}
							}
						}
					}
				}
			}
			catch(...)
			{
			}
		}
	}

	return hr;
}
