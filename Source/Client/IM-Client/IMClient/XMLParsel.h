#ifndef _XML_PARSLE_H
#define _XML_PARSLE_H

namespace OFSXMLParsel
{
HRESULT GetButtonOFS(IXMLDOMNodeList* pListNode,int Index,
					 bstr_t &Name, bstr_t &Path, bstr_t &Image, long &XPos, long &YPos);
					 
HRESULT GetEditOFS(IXMLDOMNodeList* pListNode,int Index,
					 bstr_t &Name, bstr_t &Image, long &XPos, long &YPos,
					 long &EditXPos,long &EditYPos, long &EditXLen, long &EditYLen);

HRESULT GetItemList(IXMLDOMNode* BaseNode, bstr_t &ListName,
					IXMLDOMNodeList** m_List);
HRESULT GetTickerOFS(IXMLDOMNodeList* pListNode,int Index,
					 CRect &rSize, COLORREF &FonColor, BOOL &bScrool , DWORD &Speed, 
					 long &FontHeight, long &FontWidth, _bstr_t &bstrFontName);
HRESULT GetConfig(bstr_t &bstrServer,bstr_t &sbtrPort,bstr_t &Path, bstr_t &ProxyType,
				  bstr_t &ProxyServer,bstr_t &ProxyBypass);

}
#endif //_XML_PARSLE_H