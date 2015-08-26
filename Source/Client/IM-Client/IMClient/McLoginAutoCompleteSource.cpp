// McLoginAutoCompleteSource.cpp: implementation of the CMcLoginAutoCompleteSource class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "McLoginAutoCompleteSource.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMcLoginAutoCompleteSource::CMcLoginAutoCompleteSource()
{
	
}

CMcLoginAutoCompleteSource::~CMcLoginAutoCompleteSource()
{

}

BEGIN_INTERFACE_MAP(CMcLoginAutoCompleteSource, CCmdTarget)
	INTERFACE_PART(CMcLoginAutoCompleteSource, IID_IEnumString, EnumString)
//	INTERFACE_PART(CMcLoginAutoCompleteSource, IID_IACList, ACList)
END_INTERFACE_MAP()


STDMETHODIMP_(ULONG) CMcLoginAutoCompleteSource::XEnumString::AddRef()
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, EnumString)
	return pThis->ExternalAddRef();
}

STDMETHODIMP_(ULONG) CMcLoginAutoCompleteSource::XEnumString::Release()
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, EnumString)
	return pThis->ExternalRelease();
}

STDMETHODIMP CMcLoginAutoCompleteSource::XEnumString::QueryInterface(REFIID iid, LPVOID* ppvObj)
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, EnumString)
	return pThis->ExternalQueryInterface(&iid, ppvObj);
}

/*STDMETHODIMP_(ULONG) CMcLoginAutoCompleteSource::XACList::AddRef()
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, ACList)
	return pThis->ExternalAddRef();
}*/

/*STDMETHODIMP_(ULONG) CMcLoginAutoCompleteSource::XACList::Release()
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, ACList)
	return pThis->ExternalRelease();
}*/

/*STDMETHODIMP CMcLoginAutoCompleteSource::XACList::QueryInterface(REFIID iid, LPVOID* ppvObj)
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, ACList)
	return pThis->ExternalQueryInterface(&iid, ppvObj);
}*/

STDMETHODIMP CMcLoginAutoCompleteSource::XEnumString::Next( /* [in] */ ULONG celt,/* [length_is][size_is][out] */ LPOLESTR  *rgelt,/* [out] */ ULONG  *pceltFetched)
{
	USES_CONVERSION;

	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, EnumString)

	if(pThis->m_strArray.GetSize()==0)
		return S_FALSE;

	*pceltFetched = min(celt, pThis->m_strArray.GetSize());

	for(ULONG iArraySize = 0; iArraySize<*pceltFetched;iArraySize++)
	{
		ULONG strSize = (ULONG) sizeof(WCHAR) * (pThis->m_strArray.GetAt(0).GetLength() + 1);

		rgelt[iArraySize] = (LPWSTR)::CoTaskMemAlloc(strSize);

		memcpy(rgelt[iArraySize], T2COLE(pThis->m_strArray.GetAt(0)),strSize);
		
		pThis->m_strArray.RemoveAt(0);
	}

	return S_OK;
}

STDMETHODIMP CMcLoginAutoCompleteSource::XEnumString::Skip(/* [in] */ ULONG celt)
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, EnumString)

	while(celt-->0&&pThis->m_strArray.GetSize()>0)
	{
		pThis->m_strArray.RemoveAt(0);
	}

	if(celt==0)
		return S_OK;
	else
		return S_FALSE;
}

void CMcLoginAutoCompleteSource::ReloadArray(LPCOLESTR strTemplate)
{
	m_strArray.RemoveAll();

	// Load New Sudgestions From Register
	HKEY	hAllAvailableLogin	=	NULL;
	
	CString strLoginsXML = GetRegFileText(_T("Cookies"),_T("Logins"));
	
	CComPtr<IXMLDOMDocument>	pDoc		=	NULL;
	pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	CComBSTR	bsXML = strLoginsXML;
	VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
	
	if(SUCCEEDED(pDoc->loadXML(bsXML,&varLoad))&&varLoad==VARIANT_TRUE)
	{
		CComPtr<IXMLDOMNodeList>	nodeList	=	NULL;
		pDoc->selectNodes(CComBSTR(L"logins/login"),&nodeList);
		
		if(nodeList!=NULL)
		{
			long length = 0;
			nodeList->get_length(&length);
			for(long iItemPos=0;iItemPos < length;iItemPos++)
			{
				CComPtr<IXMLDOMNode>	nodeItem	=	NULL;
				nodeList->get_item(iItemPos,&nodeItem);
				
				if(nodeItem!=NULL)
				{
					CComBSTR bsText;
					nodeItem->get_text(&bsText);
					
					CString str = (LPCOLESTR)bsText;
					m_strArray.Add(str);
				}
			}
		}
	}
}

STDMETHODIMP CMcLoginAutoCompleteSource::XEnumString::Reset( void)
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, EnumString)
	pThis->ReloadArray(NULL);
	return S_OK;
}

STDMETHODIMP CMcLoginAutoCompleteSource::XEnumString::Clone(/* [out] */ IEnumString  **ppenum)
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, EnumString)
	return S_OK;
}

/*STDMETHODIMP CMcLoginAutoCompleteSource::XACList::Expand(LPCOLESTR pszExpand)
{
	METHOD_PROLOGUE(CMcLoginAutoCompleteSource, ACList)
	return S_OK;
}*/