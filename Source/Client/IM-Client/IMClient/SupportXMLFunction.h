#ifndef _SUPPORTXML
#define _SUPPORTXML

#include <msxml2.h>

inline HRESULT GetAttribute(IXMLDOMNode* pNode, BSTR AttributeName, VARIANT* value)
{
	ATLASSERT(value != NULL);

	CComPtr<IXMLDOMElement> pElement = NULL;

	HRESULT hr = pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pElement);
		if(FAILED(hr)) return E_FAIL;
	hr = pElement->getAttribute(AttributeName,value);
		if(FAILED(hr)) return hr;
	
	return hr;
}

inline HRESULT SetAttribute(IXMLDOMNode* pNode, BSTR AttributeName, VARIANT value)
{
	ATLASSERT(pNode != NULL);
	
	CComPtr<IXMLDOMElement> pElement = NULL;
	
	HRESULT hr = pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pElement);
	if(FAILED(hr)) return E_FAIL;
	hr = pElement->setAttribute(AttributeName,value);
	if(FAILED(hr)) return hr;
	
	return hr;
}

inline HRESULT CreateDocument(IXMLDOMDocument* *pDoc,BSTR bsXML = NULL,IXMLDOMNode* *pNode = NULL)
{
	ATLASSERT(!(bsXML == NULL && pNode != NULL));

	CComPtr<IXMLDOMDocument> pDocIN = NULL;
	VARIANT_BOOL			 bResult = VARIANT_TRUE;
	HRESULT					 hr = S_OK;
	
	hr = CoCreateInstance(CLSID_DOMDocument40,NULL,CLSCTX_SERVER,IID_IXMLDOMDocument,(void**)&pDocIN);

	if(SUCCEEDED(hr))
	{
		if(bsXML != NULL)
		{
			hr = pDocIN->loadXML(bsXML,&bResult);
			//pDocIN->save(CComVariant(L"c:\\AlertXML.xml"));
			if(FAILED(hr) || bResult == VARIANT_FALSE)
			{
				ATLASSERT(FALSE);
				return E_INVALIDARG;
			}
			if(pNode)
			{
				hr = pDocIN->get_firstChild(pNode);
				if(FAILED(hr))
				{
					ATLASSERT(FALSE);
					return E_INVALIDARG;
				}
			}
		}
		
		if(pDoc)
			*pDoc = pDocIN.Detach();
		
		return hr;
	}
	return hr;
}

inline HRESULT insertSingleNode(IXMLDOMNode* pNode, BSTR name,BSTR URI_name,BSTR bsValue = NULL, IXMLDOMNode **pOutNode = NULL)
{
	CComPtr<IXMLDOMDocument> pDoc  = NULL;
	CComPtr<IXMLDOMNode>	 pTemp = NULL;
	CComPtr<IXMLDOMNode>	 pTemp1= NULL;
	HRESULT					 hr = S_OK;
	
	hr = pNode->get_ownerDocument(&pDoc);
	if(FAILED(hr))
	{
		hr = pNode->QueryInterface(IID_IXMLDOMDocument,(void**)&pDoc);
		ATLASSERT(SUCCEEDED(hr));
	}
	
	hr = pDoc->createNode(CComVariant(NODE_ELEMENT),name,URI_name,&pTemp);
	if(FAILED(hr)) return hr;
	hr = pNode->appendChild(pTemp,&pTemp1);
	if(FAILED(hr)) return hr;

	if(bsValue != NULL)
	hr = pTemp1->put_text(bsValue);
	if(FAILED(hr)) return hr;

	if(pOutNode != NULL)
	{ *pOutNode = pTemp1.Detach();}
	
	return hr;
}

inline HRESULT insertSingleAttribut(IXMLDOMNode* pNode, BSTR name,VARIANT value)
{
	CComPtr<IXMLDOMElement> pElement = NULL;
	HRESULT					hr = S_OK;
	
	ATLASSERT(pNode != NULL);
	hr = pNode->QueryInterface(IID_IXMLDOMElement,(void**)&pElement);
	ATLASSERT(SUCCEEDED(hr));
	hr = pElement->setAttribute(name,value);
	ATLASSERT(SUCCEEDED(hr));

	return hr;
}

inline HRESULT GetTextByPath(IXMLDOMNode* pNode, BSTR name,BSTR* value)
{
	CComPtr<IXMLDOMNode> pTemp = NULL;
	HRESULT					hr = S_OK;

	hr = pNode->selectSingleNode(name,&pTemp);
	if(hr != S_OK) return hr;
	hr = pTemp->get_text(value);
	return hr;
}

inline BOOL CheckNodeByPath(IXMLDOMNode* pNode, BSTR name, IXMLDOMNode* *pOutNode = NULL)
{
	ATLASSERT(pNode);
	CComPtr<IXMLDOMNode>	pTemp = NULL;
	HRESULT					hr = S_OK;
	
	hr = pNode->selectSingleNode(name,&pTemp);
	if(hr == S_FALSE) return FALSE;
	if(pOutNode) *pOutNode = pTemp.Detach();
	return TRUE;
}
inline HRESULT removeNode(IXMLDOMNode* pNode)
{
	CComPtr<IXMLDOMNode> pTempNode = NULL;
	HRESULT	hr = S_OK;

	hr = pNode->get_parentNode(&pTempNode);
	if(S_OK == hr)
	{
		return pTempNode->removeChild(pNode,NULL);
	}

	return hr;
}

inline HRESULT LinkInfo(IXMLDOMNode* pCommonNode, IXMLDOMNode* pPrivateNode, IXMLDOMNode* *pResultNode)
{
	ATLASSERT(pResultNode != NULL);

	CComPtr<IXMLDOMElement>		 pResultElement = NULL;
	CComPtr<IXMLDOMElement>		 pElement = NULL;
	CComPtr<IXMLDOMNode>		 pNode = NULL;
	CComPtr<IXMLDOMNode>		 pTemp = NULL;
	CComPtr<IXMLDOMNamedNodeMap> pMap = NULL;
	CComPtr<IXMLDOMNodeList>	 pList = NULL;
	CComBSTR					 bsName;
	CComBSTR					 bsValue;
	CComVariant					 vrValue;

	LONG	nCount = 0;
	HRESULT hr = S_OK;

	if(pCommonNode == NULL && pPrivateNode == NULL)
	{
		*pResultNode = NULL;
	}

	if(pCommonNode != NULL && pPrivateNode == NULL)
	{
		hr = pCommonNode->cloneNode(VARIANT_TRUE,pResultNode);
		return S_OK;
	}
	hr = pPrivateNode->cloneNode(VARIANT_TRUE,pResultNode);
	
	if(pCommonNode == NULL) return S_OK;

	hr = pCommonNode->QueryInterface(IID_IXMLDOMElement,(void**)&pResultElement);
	pResultElement->get_attributes(&pMap);


	//Attributes
	pMap->get_length(&nCount);
	if(nCount) 
	{
		hr = pPrivateNode->QueryInterface(IID_IXMLDOMElement,(void**)&pElement);
		for(int k=0;k<nCount;k++)
		{
			pMap->get_item(k,&pNode);
			pNode->get_nodeName(&bsName);
			pNode->get_text(&bsValue);
			if(S_OK != pElement->getAttribute(bsName,&vrValue))
			{
				pResultElement->setAttribute(bsName,CComVariant(bsValue));
			}
			pNode.Release();
		}
		pElement.Release();
	}
	pMap.Release();
	pResultElement.Release();

	//Child nodes
	pCommonNode->selectNodes(CComBSTR(_T("*")),&pList);
	pList->get_length(&nCount);

		for(int k=0;k<nCount;k++)
		{
			hr = pList->get_item(k,&pNode);
			hr = pNode->get_nodeName(&bsName);
			if(!CheckNodeByPath(pPrivateNode,bsName))
			{
				pNode->cloneNode(VARIANT_TRUE,&pTemp);
				(*pResultNode)->appendChild(pTemp,NULL);
				pTemp.Release();
			} 
			pNode.Release();
			
		}	 	
		pList.Release();
		
		return hr;
}

inline HRESULT GetDataByPath(IXMLDOMNode* pNode, BSTR name,VARIANT *varNodeData)
{
	if(!pNode||!varNodeData)
		return E_INVALIDARG;
		
	CComPtr<IXMLDOMNode> pTemp = NULL;
	HRESULT					hr = S_OK;
	
	hr = pNode->selectSingleNode(name,&pTemp);

	if(hr != S_OK) 
		return hr;

	hr = pTemp->get_nodeTypedValue(varNodeData);

	return hr;
}


#endif //_SUPPORTXML
