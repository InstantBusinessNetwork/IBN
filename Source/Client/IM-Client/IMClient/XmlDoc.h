// XmlDoc.h: interface for the CXmlDoc class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_XMLDOC_H__9A83D365_BF25_4F6B_9940_97498AA0C632__INCLUDED_)
#define AFX_XMLDOC_H__9A83D365_BF25_4F6B_9940_97498AA0C632__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "HttpDownloader.h"
//#include <msxml2.h>


class CXmlDoc :
	public CCmdTarget
{
	DECLARE_DISPATCH_MAP()

public:
	void WaitForComplete(DWORD Timeout = 10000);
	void Abort();
	HRESULT GetNodeData(LPCTSTR szNodeName, VARIANT *varNodeData, long index=0);
	HRESULT GetXML(BSTR* xml);
	void WorkFunction();
	CXmlDoc();
	virtual ~CXmlDoc();

	HRESULT GetListItem(LPCTSTR szNodeName,long index, LPCTSTR szChildNode,CString &strValue);
	HRESULT GetNodeAttribute(LPCTSTR szNodeName, LPCTSTR szAttributeName, CString &strAttributeValue, long index = 0);
	BOOL Init(HWND hWnd, UINT msg);
	BOOL Load(LPCTSTR szUrl);
	HRESULT GetNodeText(LPCTSTR szNodeName, CString &strNodeText, long index = 0);
	HRESULT GetNodeXML(LPCTSTR szNodeName, CString &strNodeXML, long index = 0);
	
protected:
	long GetError();
	static DWORD WINAPI WorkThread(LPVOID p);
	HRESULT OnReadyStateChange();
	BOOL AdviseConnectionPoint(BOOL bAdvise);

	CHttpDownloader m_http;
	CString m_strUrl;
	HRESULT m_hr;
	BOOL m_bInited;
	DWORD m_dwCookie;
	IXMLDOMDocument *m_pDoc;
	HWND m_hWnd;
	UINT m_msg;

	HANDLE hThread;
};

//////////////////////////////////////////////////////////////////////////
class CStreamDoc
{
public:
	void WaitForComplete(DWORD Timeout = 10000);
	void Abort();
	HRESULT GetStream(LPSTREAM *pStream);
	CStreamDoc();
	virtual ~CStreamDoc();
	BOOL Init(HWND hWnd, UINT msg);
	BOOL Load(LPCTSTR szUrl);
private:
	void WorkFunction();
protected:		
	static DWORD WINAPI WorkThread(LPVOID p);
	CHttpDownloader m_http;
	CString			m_strUrl;
	HRESULT			m_hr;
	LPSTREAM		m_pDoc;
	HWND m_hWnd;
	UINT m_msg;

	HANDLE hThread;
};
	

#endif // !defined(AFX_XMLDOC_H__9A83D365_BF25_4F6B_9940_97498AA0C632__INCLUDED_)
