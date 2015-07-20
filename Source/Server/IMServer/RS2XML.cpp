// RS2XML1.cpp: implementation of the CRS2XML class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "RS2XML.h"
#include "xmlutil.h"
#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CRS2XML::CRS2XML()
{

}

CRS2XML::~CRS2XML()
{

}

BOOL CRS2XML::ParseList(const _RecordsetPtr &pRS, const CComPtr<IXMLDOMNode> pNode, const CR2XItem *pItems, CComBSTR bsListName)
{
	//HRESULT	hr = S_OK;
	CComPtr<IXMLDOMNode>		pChild;

	pNode->get_firstChild(&pChild);
	if(pChild)
	{
		CComBSTR bsNodeName;
		pChild->get_nodeName(&bsNodeName);
		if(!(bsListName == bsNodeName))
			pChild.Release();
	}

	if (pChild == NULL)		
		CXMLUtil::AppendNode(pNode,bsListName,NULL,&pChild);
	

	// If NULL - return сразу [2000.11.21-15:18]
	if (pRS == NULL)
		return FALSE;
	
	// Move to first record [18:46 - 14.11.2000]
	pRS->MoveFirst();
	do
	{
		BOOL bRetVal = CRS2XML::Parse(pRS, pChild, pItems);
		// If error occured - exit сразу [18:47 - 14.11.2000]
		if (!bRetVal)
			return FALSE;
		pRS->MoveNext();
	} while (!pRS->EndOfFile);
	return TRUE;
}

BOOL CRS2XML::ParseMulti(const _RecordsetPtr &pRS, const CComPtr<IXMLDOMNode> pNode, const CR2XItem *pItems, CComBSTR bsMultiName)
{
	CComPtr<IXMLDOMNode>			pChild;
	
	// Move to first record [2000.11.20-16:29]
	pRS->MoveFirst();
	while (!pRS->EndOfFile)
	{
		CXMLUtil::AppendNode(pNode,bsMultiName,NULL,&pChild);

		BOOL bRetVal = CRS2XML::Parse(pRS, pChild, pItems);
		pChild.Release();
		
		// If error occured - exit immediately [2000.11.20-16:29]
		if (!bRetVal)
			return FALSE;
		pRS->MoveNext();
	}
	return TRUE;
}

inline HRESULT GetFieldValue(const FieldsPtr& pFields, LPTSTR szFiledName, _variant_t& value)
{
	Field* pField = NULL;
	HRESULT hr = pFields->get_Item(_variant_t(szFiledName), &pField);
	if(SUCCEEDED(hr))
	{
		hr = pField->get_Value(&value);
		pField->Release();
	}
	return hr;
}

BOOL CRS2XML::Parse(const _RecordsetPtr &pRS, const CComPtr<IXMLDOMNode> pNode, const CR2XItem *pItems)
{
	if (pRS == NULL)
		return FALSE;

	if (!pItems)
		return FALSE;

	CComPtr<IXMLDOMNode>		pCurr;
	CComPtr<IXMLDOMElement>		pElem;
	_variant_t retVal;
	HRESULT	hr;

	int nItemsCount = 0;
	while (pItems[nItemsCount++].m_dwItemPos != R2XPOS_BODY_END);
	nItemsCount -= 2;

	// Too few items in array - exit [16:43 - 14.11.2000]
	if (nItemsCount < 1)
		return FALSE;
	
	// Insert root item [16:45 - 14.11.2000]
	//pNode->selectSingleNode(CComBSTR(pItems[0].m_szItemName),&pCurr);
	//if(pCurr == NULL)
	CXMLUtil::AppendNode(pNode,CComBSTR(pItems[0].m_szItemName),NULL,&pCurr);

	FieldsPtr pFields = pRS->Fields;
	int nI;

	for (nI = 1; nI < nItemsCount + 1; nI++)
	{
		switch (pItems[nI].m_dwItemPos)
		{
		// Tag [16:49 - 14.11.2000]
		case R2XPOS_TAG:
			{
				retVal = _variant_t(0);
				if(SUCCEEDED(GetFieldValue(pFields, pItems[nI].m_szFieldName, retVal)))
				{
					if((retVal.vt == VT_BSTR && SysStringLen(retVal.bstrVal) != 0) ||
						retVal.vt != VT_BSTR && retVal.vt != VT_NULL && retVal.vt != VT_EMPTY 
						&& retVal.lVal != 0)
					{
						_variant_t vTest;
						hr = VariantChangeType(&vTest,&retVal,NULL,VT_BSTR);
						if(hr == S_OK)
							CXMLUtil::AppendNode(pCurr,CComBSTR(pItems[nI].m_szItemName),NULL,NULL,vTest.bstrVal);
					}
				}
			}
			break;
		// Tag attribute [16:54 - 14.11.2000]
		case R2XPOS_ATTR:
			{
				if(SUCCEEDED(GetFieldValue(pFields, pItems[nI].m_szFieldName, retVal)))
				{
					if((retVal.vt == VT_BSTR && SysStringLen(retVal.bstrVal) != 0) ||
						retVal.vt != VT_BSTR && retVal.vt != VT_NULL && retVal.vt != VT_EMPTY 
						&& retVal.lVal != 0)
					{
						pCurr->QueryInterface(IID_IXMLDOMElement, (void**)&pElem);
						pElem->setAttribute(CComBSTR(pItems[nI].m_szItemName), retVal);
						pElem.Release();
					}
				}
			}
			break;
		// If nested class - call self [16:55 - 14.11.2000]
		case R2XPOS_CHILD:
			{
				Parse(pRS, pCurr, (CR2XItem*)pItems[nI].m_szItemName);
			}
			break;
		}
	}
	
	return TRUE;
}
