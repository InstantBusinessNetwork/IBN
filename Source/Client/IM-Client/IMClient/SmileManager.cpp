// SmileManager.cpp: implementation of the CSmileManager class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "SmileManager.h"
#include "cdib.h"
#include "LoadSkins.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif


extern CString GetCurrentSkin();
extern COfsTvApp  theApp;

extern  CSmileManager CurrentSmileManager;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
CSmileInfo FindSmileImage(LPCWSTR HtmlText, int& StartPos, int& Length)
{
	int Index = 0;
	byte FindLevel = 0;

	int SmileIdStartPos = 0;
	int SmileIdEndPos = 0;

	WCHAR SmileIdBuffer[256] = L"";

	while(HtmlText[Index]!=0)
	{
		switch(FindLevel)
		{
		// Find <img
		case 0:
			if(HtmlText[Index]==L'<')
			{
				FindLevel++;
				StartPos = Index;
			}
			else
				FindLevel = 0;
			break;
		case 1:
			if(HtmlText[Index]==L'i'||HtmlText[Index]==L'I')
				FindLevel++;
			else
				FindLevel = 0;
			break;
		case 2:
			if(HtmlText[Index]==L'm'||HtmlText[Index]==L'M')
				FindLevel++;
			else
				FindLevel = 0;
			break;
		case 3:
			if(HtmlText[Index]==L'g'||HtmlText[Index]==L'G')
				FindLevel++;
			else
				FindLevel = 0;
			break;
		// Find id=
		case 4:
			if(HtmlText[Index]==L'i'||HtmlText[Index]==L'I')
				FindLevel++;
			else if (HtmlText[Index]==L'>')
				FindLevel = 0;
			break;
		case 5:
			if(HtmlText[Index]==L'd'||HtmlText[Index]==L'D')
				FindLevel++;
			else 
				FindLevel = 0;
			break;
		case 6:
			if(HtmlText[Index]==L'=')
				FindLevel++;
			else if (HtmlText[Index]==L'>')
				FindLevel = 0;
			break;
		// Skip " and extract SmileId
		case 7:
			if(HtmlText[Index]!=L' ')
			{
				SmileIdStartPos = Index;

				if(HtmlText[Index]==L'"'||HtmlText[Index]==L'\'')
				{
					SmileIdStartPos++;
					FindLevel++;
				}

				FindLevel++;
			}
			else if (HtmlText[Index]==L'>')
				FindLevel = 0;
			break;
		// Without "
		case 8:
			if(HtmlText[Index]==L' ')
			{
				SmileIdEndPos = Index-1;
				FindLevel = 10;
			}
			else if (HtmlText[Index]==L'>')
				FindLevel = 0;
			break;
		// With "
		case 9:
			if(HtmlText[Index]==L'"'||HtmlText[Index]==L'\'')
			{
				SmileIdEndPos = Index-1;
				FindLevel = 10;
			}
			break;
		case 10:
			if (HtmlText[Index]==L'>')
			{
				Length = Index - StartPos + 1;

				USES_CONVERSION;
				// WE Have found smile-image tag
				wcsncpy(SmileIdBuffer, HtmlText + SmileIdStartPos, SmileIdEndPos - SmileIdStartPos +1);

				return CurrentSmileManager.GetSmile(W2CT(SmileIdBuffer));
			}
			break;
		default:
			FindLevel = 0;
			break;
		}

		Index++;
	}

	return CSmileInfo::Empty;
}


CSmileManager CurrentSmileManager;

CSmileInfo CSmileInfo::Empty = CSmileInfo();

CSmileManager::CSmileManager()
{

}

CSmileManager::~CSmileManager()
{

}

CSmileInfoList& CSmileManager::GetSmiles()
{
	return m_SmileList;
}

CSmileInfo& CSmileManager::GetSmile(LPCTSTR SmileId)
{
	for(CSmileInfoListEnum item = m_SmileList.begin();item!=m_SmileList.end(); item++)
	{
		if((*item).GetId()==CString(SmileId))
		{
			return *item;
		}
	}

	return CSmileInfo::Empty;
}

CSmileInfo& CSmileManager::GetSmile(int SmileIndex)
{
	for(CSmileInfoListEnum item = m_SmileList.begin();item!=m_SmileList.end(); item++)
	{
		int ItemIndex = (*item).GetIndex(); 
		if(SmileIndex==ItemIndex)
		{
			return *item;
		}
	}

	return CSmileInfo::Empty;
}


BOOL CSmileManager::Init()
{
	HRESULT hr = S_OK;
	VARIANT_BOOL b;
	
	CString str = IBN_SCHEMA;
	str += (LPCTSTR)GetCurrentSkin();
	str += (LPCTSTR)_T("/Shell/Smiles/smile_list.xml");

	CComPtr<IXMLDOMDocument> xmlDoc = NULL, xmlPrefDoc = NULL;
	hr = xmlDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);

	if(SUCCEEDED(hr))
	{
		xmlDoc->put_async(VARIANT_FALSE);

		CComVariant	varLoad = (LPCTSTR)str;
		hr = xmlDoc->load(varLoad, &b);

		if(SUCCEEDED(hr))
		{
			CComPtr<IXMLDOMNodeList> xmlSmiles = NULL;

			hr = xmlDoc->selectNodes(_bstr_t("Smiles/Smile"), &xmlSmiles);

			if(SUCCEEDED(hr))
			{
				long length	=	0;
				xmlSmiles->get_length(&length);

				for(int index=0; index<length; index++)
				{
					CComPtr<IXMLDOMNode> xmlSmile = NULL;
					xmlSmiles->get_item(index,&xmlSmile);

					CComQIPtr<IXMLDOMElement> xmlSmileElement = xmlSmile;

					if(xmlSmileElement)
					{
						USES_CONVERSION;

						CComVariant varId, varSmile, varText;

						hr = xmlSmileElement->getAttribute(CComBSTR("Id"), &varId);
						hr = xmlSmileElement->getAttribute(CComBSTR("Smile"), &varSmile);
						hr = xmlSmileElement->getAttribute(CComBSTR("Text"), &varText);

						CSmileInfo newSmileItem(index,
							W2CT(varId.bstrVal), 
							W2CT(varSmile.bstrVal), 
							W2CT(varText.bstrVal));

						m_SmileList.push_back(newSmileItem);
					}
				}

				LoadHitCount();

				m_SmileList.sort(std::greater<CSmileInfo>());
			}
		}
	}

	

	return SUCCEEDED(hr);
}

void CSmileManager::LoadHitCount()
{
	CString strSmilesXML = GetRegFileText(_T("Cookies"),_T("Smiles"));
	
	CComBSTR	bsXML = strSmilesXML;
	VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
	
	CComPtr<IXMLDOMDocument>	pDoc		=	NULL;
	pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	pDoc->loadXML(bsXML,&varLoad);
	
	if(varLoad!=VARIANT_TRUE)
	{

		bsXML = L"<Smiles>" 
					L"<Smile Id=\"001\" Hit=\"100\" LastAccess=\"0\" />" 
					L"<Smile Id=\"002\" Hit=\"95\" LastAccess=\"0\" />"  
					L"<Smile Id=\"013\" Hit=\"90\" LastAccess=\"0\" />"  
					L"<Smile Id=\"014\" Hit=\"85\" LastAccess=\"0\" />"  
					L"<Smile Id=\"100\" Hit=\"80\" LastAccess=\"0\" />"  
					L"<Smile Id=\"021\" Hit=\"75\" LastAccess=\"0\" />"  
					L"<Smile Id=\"009\" Hit=\"70\" LastAccess=\"0\" />"  
					L"<Smile Id=\"010\" Hit=\"65\" LastAccess=\"0\" />"  
					L"<Smile Id=\"003\" Hit=\"60\" LastAccess=\"0\" />"  
					L"<Smile Id=\"015\" Hit=\"55\" LastAccess=\"0\" />"  
					L"<Smile Id=\"024\" Hit=\"50\" LastAccess=\"0\" />"  
					L"<Smile Id=\"020\" Hit=\"45\" LastAccess=\"0\" />"  
					L"<Smile Id=\"007\" Hit=\"40\" LastAccess=\"0\" />"  
					L"<Smile Id=\"057\" Hit=\"35\" LastAccess=\"0\" />"  
					L"<Smile Id=\"107\" Hit=\"30\" LastAccess=\"0\" />"  
					L"<Smile Id=\"104\" Hit=\"25\" LastAccess=\"0\" />"  
					L"<Smile Id=\"016\" Hit=\"20\" LastAccess=\"0\" />"  
					L"<Smile Id=\"101\" Hit=\"15\" LastAccess=\"0\" />"  
				L"</Smiles>";
		pDoc->loadXML(bsXML,&varLoad);
	}

	for(CSmileInfoListEnum item = GetSmiles().begin();item != GetSmiles().end();item++)
	{
		CComPtr<IXMLDOMNode>	smileNode = NULL;

		CComBSTR bsPath = L"Smiles/Smile[@Id='";
		bsPath += (*item).GetId();
		bsPath += L"']";
	
		pDoc->selectSingleNode(bsPath,&smileNode);

		if(smileNode!=NULL)
		{
			CComQIPtr<IXMLDOMElement>	xmlSmileElement = smileNode;

			CComVariant varHit, varLA;

			xmlSmileElement->getAttribute(CComBSTR("Hit"), &varHit);
			xmlSmileElement->getAttribute(CComBSTR("LastAccess"), &varLA);

			(*item).SetSort(_wtol(varHit.bstrVal), _wtol(varLA.bstrVal));
		}
	}
}

void CSmileManager::SaveHitCount()
{
	USES_CONVERSION;

	VARIANT_BOOL	varLoad	=	VARIANT_FALSE;
	
	CComPtr<IXMLDOMDocument>	pDoc		=	NULL;
	pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	pDoc->loadXML(CComBSTR(L"<Smiles/>"),&varLoad);

	CComPtr<IXMLDOMNode> xmlSmilesNode;

	pDoc->selectSingleNode(CComBSTR(L"Smiles"), &xmlSmilesNode);

	for(CSmileInfoListEnum item = GetSmiles().begin();item != GetSmiles().end();item++)
	{
		CComPtr<IXMLDOMNode> xmlSmileNode;
		insertSingleNode(xmlSmilesNode,CComBSTR(L"Smile"),NULL,NULL, &xmlSmileNode);

		if((*item).GetHit()!=0)
		{
			insertSingleAttribut(xmlSmileNode, CComBSTR(L"Id"), CComVariant((*item).GetId()));
			insertSingleAttribut(xmlSmileNode, CComBSTR(L"Hit"), CComVariant((*item).GetHit()));
			insertSingleAttribut(xmlSmileNode, CComBSTR(L"LastAccess"), CComVariant((*item).GetLastAccess()));
		}
	}

	CComBSTR bsXML;
	pDoc->get_xml(&bsXML);

	SetRegFileText(_T("Cookies"),_T("Smiles"),W2CT(bsXML));

	m_SmileList.sort(std::greater<CSmileInfo>());
}

void CSmileManager::IncHitCount(LPCTSTR SmileId)
{
	CSmileInfo& smile = GetSmile(SmileId);
	smile.IncSort();

	SaveHitCount();
}

CBitmap* CSmileManager::GetSmilePreview(LPCTSTR SmileId)
{
	CSmileInfo smInfo = GetSmile(SmileId);

	if(smInfo==CSmileInfo::Empty)
		return NULL;

	CBitmap* pRetVal = new CBitmap();

	IStreamPtr pStream = NULL;
	long Error = 0;

	LoadSkins LoadSkin;

	LoadSkin.Load(bstr_t(IBN_SCHEMA)+bstr_t(GetProductLanguage())+ bstr_t("/Shell/Smiles/Preview/") + bstr_t(SmileId) + bstr_t(".bmp"),&pStream,&Error);

	if(pStream!=NULL)
	{	
		CDib dib(pStream);
		CPaintDC dc(theApp.GetMainWnd());

		pRetVal->Attach(dib.GetHBITMAP(dc));
	}
	else
	{
		delete pRetVal;
		pRetVal = NULL;
	}


	return pRetVal;
}


