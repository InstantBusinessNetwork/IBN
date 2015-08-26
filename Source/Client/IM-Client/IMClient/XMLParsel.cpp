#include "stdafx.h"
#include "XMLParsel.h"

#define CHECKERROR(pointer) if(pointer==NULL) return -1


HRESULT OFSXMLParsel::GetButtonOFS(IXMLDOMNodeList* pListNode,int Index,
					 bstr_t &Name, bstr_t &Path, bstr_t &Image, long &XPos, long &YPos)
{
	CComPtr<IXMLDOMNode> m_ButtonNode = NULL, m_TmpNode = NULL;
	bstr_t strLong;
	
	pListNode->get_item(Index,&m_ButtonNode);
	CHECKERROR(m_ButtonNode);
	
	CComPtr<IXMLDOMElement>  m_Element = NULL;
	m_ButtonNode->QueryInterface(IID_IXMLDOMElement,(void**)&m_Element);
	CHECKERROR(m_Element);
	CComVariant varPid;
	m_Element->getAttribute(CComBSTR("Name"),&varPid);
	Name = varPid.bstrVal;

	 m_ButtonNode->selectSingleNode(CComBSTR("Path"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("Path Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	CComBSTR	bsText; 
	m_TmpNode->get_text(&bsText);
	Path = bsText;
	m_TmpNode = NULL;
	bsText.Empty();

	m_ButtonNode->selectSingleNode(bstr_t("Image"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("Image Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	Image = bsText;
	m_TmpNode = NULL;
	bsText.Empty();

	m_ButtonNode->selectSingleNode(bstr_t("XPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("XPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	XPos = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_ButtonNode->selectSingleNode(bstr_t("YPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("YPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	YPos = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	return NO_ERROR;
}

HRESULT OFSXMLParsel::GetEditOFS(IXMLDOMNodeList* pListNode,int Index,
					 bstr_t &Name, bstr_t &Image, long &XPos, long &YPos,
					 long &EditXPos,long &EditYPos, long &EditXLen, long &EditYLen)
{
	CComPtr<IXMLDOMNode> m_EditNode = NULL, m_TmpNode = NULL;
	bstr_t strLong;

	pListNode->get_item(Index,&m_EditNode);
	CHECKERROR(m_EditNode);

	CComPtr<IXMLDOMElement>  m_Element = NULL;
	m_EditNode->QueryInterface(IID_IXMLDOMElement,(void**)&m_Element);
	CHECKERROR(m_Element);
	CComVariant varPid;
	m_Element->getAttribute(_bstr_t("Name"),&varPid);
	Name = varPid.bstrVal;
	
	m_EditNode->selectSingleNode(bstr_t("Image"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("Image Node Error"));
#endif	
	CComBSTR	bsText; 
	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	Image = bsText;
	m_TmpNode = NULL;
	bsText.Empty();

	m_EditNode->selectSingleNode(bstr_t("XPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("XPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	XPos = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_EditNode->selectSingleNode(bstr_t("YPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("YPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	YPos = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_EditNode->selectSingleNode(bstr_t("EditXPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("EditXPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	EditXPos = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_EditNode->selectSingleNode(bstr_t("EditYPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("EditYPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	EditYPos = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_EditNode->selectSingleNode(bstr_t("EditXLen"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("EditXLen Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	EditXLen = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_EditNode->selectSingleNode(bstr_t("EditYLen"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("EditYLen Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	EditYLen = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	return NO_ERROR;
}


HRESULT OFSXMLParsel::GetItemList(IXMLDOMNode* BaseNode, bstr_t &ListName,
					IXMLDOMNodeList **m_List)
{
	CComPtr<IXMLDOMNode> m_TmpNode = NULL;
	BaseNode->selectSingleNode(ListName,&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("Can'not Find Collection"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_childNodes(m_List);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("Can'not Find Child Collection"));
#endif	
	CHECKERROR(m_List);
	return 0;
}

HRESULT OFSXMLParsel::GetTickerOFS(IXMLDOMNodeList* pListNode,int Index,
					 CRect &rSize, COLORREF &FonColor, BOOL &bScrool , DWORD &Speed, 
					 long &FontHeight, long &FontWidth, _bstr_t &bstrFontName)
{
/*           <FontHeight>10</FontHeight>
           <FontWidth>8</FontWidth>
           <FontName>Arial</FontName> 
*/

	CComPtr<IXMLDOMNode> m_TickerNode = NULL, m_TmpNode = NULL;
	bstr_t strLong;
	
	pListNode->get_item(Index,&m_TickerNode);
	CHECKERROR(m_TickerNode);
	
	CComPtr<IXMLDOMElement>  m_Element = NULL;
	m_TickerNode->QueryInterface(IID_IXMLDOMElement,(void**)&m_Element);
	CHECKERROR(m_Element);
	CComVariant varPid;
	m_Element->getAttribute(_bstr_t("Name"),&varPid);
	bstr_t Name = varPid.bstrVal;

	CComBSTR	bsText;

	m_TickerNode->selectSingleNode(bstr_t("XPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("XPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	rSize.left = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	
	m_TickerNode->selectSingleNode(bstr_t("YPos"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("YPos Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	rSize.top = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	

	m_TickerNode->selectSingleNode(bstr_t("XLen"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("XLen Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	rSize.right = atol((char*)strLong) + rSize.left ;
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_TickerNode->selectSingleNode(bstr_t("YLen"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("YLen Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	rSize.bottom = atol((char*)strLong) + rSize.top;
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_TickerNode->selectSingleNode(bstr_t("FonColorHex"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("YLen Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	char *pEnd = NULL;
	FonColor = strtoul((char*)strLong,&pEnd,16);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_TickerNode->selectSingleNode(bstr_t("Scroll"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("Scroll Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	bScrool = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_TickerNode->selectSingleNode(bstr_t("Speed"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("Speed Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	Speed = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();

	m_TickerNode->selectSingleNode(bstr_t("FontHeight"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("FontHeight Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	FontHeight = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_TickerNode->selectSingleNode(bstr_t("FontWidth"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("FontWidth Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	strLong = bsText;
	FontWidth = atol((char*)strLong);
	m_TmpNode = NULL;
	bsText.Empty();
	
	m_TickerNode->selectSingleNode(bstr_t("FontName"),&m_TmpNode);
#ifdef _DEVELOVER_VERSION_L1
	if(m_TmpNode==NULL) AfxMessageBox(_T("FontName Node Error"));
#endif	
	CHECKERROR(m_TmpNode);
	m_TmpNode->get_text(&bsText);
	bstrFontName = bsText;
	m_TmpNode = NULL;
	bsText.Empty();
	
	return NO_ERROR;
}

HRESULT OFSXMLParsel::GetConfig(bstr_t &bstrServer,bstr_t &sbtrPort,bstr_t &Path, bstr_t &ProxyType,
				  bstr_t &ProxyServer,bstr_t &ProxyBypass)
{
	return E_NOTIMPL;
}
