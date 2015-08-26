// McLoaderImpl.cpp : implementation file
//

#include "stdafx.h"
#include "McLoaderImpl.h"
#include "GlobalFunction.h"
#include "resource.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMcLoaderImpl

IMPLEMENT_DYNCREATE(CMcLoaderImpl, CCmdTarget)

CMcLoaderImpl::CMcLoaderImpl(IFormSender	*pFormSender)
{
	dwSessionCookie = 0;
	EnableAutomation();
	try
	{
		m_pForm	=	pFormSender->CreateForm();
	}
	catch(_com_error&)
	{
		ASSERT(FALSE);
	}
	InitEventContainer();
}

CMcLoaderImpl::~CMcLoaderImpl()
{
	try
	{
		m_pForm->Stop();
	}
	catch (_com_error&) 
	{
		ASSERT(FALSE);
	}
	
	CloseEventContainer();
}

/////////////////////////////////////////////////////////////////////////////
// CMcLoaderImpl message handlers

BEGIN_DISPATCH_MAP(CMcLoaderImpl, CCmdTarget)
//{{AFX_DISPATCH_MAP(CMcLoaderImpl)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnPushTOQueue",		1, OnPushTOQueue,    VT_EMPTY, VTS_NONE)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnUploadBegin",		2, OnUploadBegin,    VT_EMPTY, VTS_I4)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnUploadStep",		3, OnUploadStep,    VT_EMPTY, VTS_I4)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnUploadEnd",		4, OnUploadEnd,    VT_EMPTY, VTS_NONE)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnDownloadBegin",   5, OnDownloadBegin,    VT_EMPTY, VTS_I4)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnDownloadStep",	6, OnDownloadStep,    VT_EMPTY, VTS_I4)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnDownloadEnd",		7, OnDownloadEnd,    VT_EMPTY, VTS_NONE)
DISP_FUNCTION_ID(CMcLoaderImpl,"OnCompleted",		8, OnCompleted, VT_EMPTY, VTS_I4 VTS_I4)
//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()

BEGIN_INTERFACE_MAP(CMcLoaderImpl, CCmdTarget)
//{{AFX_INTERFACE_MAP(CMcLoaderImpl)
INTERFACE_PART(CMcLoaderImpl, __uuidof(_IFormEvents), Dispatch)
//}}AFX_INTERFACE_MAP
END_INTERFACE_MAP()

//////////////////////////////////////////////////////////////////////////
/*void CMcLoaderImpl::OnProgress ( BOOL Receive, long FullSize,long size )
{
	if(!Receive)
	{
		if(IsWindow(m_MsgHwnd))
		{
			SIZEL	Size = {FullSize,size};
			::PostMessage(m_MsgHwnd,m_ProgressMsg,WPARAM(this),LPARAM(&Size));
		}
	}
}*/

/*void CMcLoaderImpl::OnCompleted (enum CompletedConstants state,long ErrorCode )
{
	if(state==ccCompleted)
	{
		if(IsWindow(m_MsgHwnd))
		{
			if (ErrorCode==S_OK&&m_varReceivingData.punkVal) 
			{
				CComPtr<IStream> pStream = (LPSTREAM)m_varReceivingData.punkVal;
				LARGE_INTEGER	li = {0,0};
				pStream->Seek(li,STREAM_SEEK_SET ,NULL);
				
				BYTE Buff[2];
				DWORD dwRealRead;
				pStream->Read(Buff,2,&dwRealRead);
				
				if(memcmp(Buff,_T("OK"),2)!=0)
					ErrorCode = E_FAIL;
			}

			::PostMessage(m_MsgHwnd,m_CompletedMsg,WPARAM(this),LPARAM(ErrorCode));
		}
	}
}*/

void CMcLoaderImpl::InitEventContainer()
{
	CComPtr<IConnectionPointContainer>	pCPContainer = NULL;
	CComPtr<IConnectionPoint>			pSessionConnectionPoint;

	m_pForm->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		HRESULT hr = pCPContainer->FindConnectionPoint(__uuidof(_IFormEvents),&pSessionConnectionPoint);
		if(SUCCEEDED(hr))
		{
			CComPtr<IUnknown> pInterEvent = GetInterface(&IID_IUnknown);
			hr = pSessionConnectionPoint->Advise(pInterEvent ,&dwSessionCookie);
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	else
	{
		ASSERT(FALSE);
	}
}

void CMcLoaderImpl::CloseEventContainer()
{
	CComPtr<IConnectionPointContainer>	pCPContainer = NULL;
	CComPtr<IConnectionPoint>			pSessionConnectionPoint;

	m_pForm->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		HRESULT hr = pCPContainer->FindConnectionPoint(__uuidof(_IFormEvents),&pSessionConnectionPoint);

		if(SUCCEEDED(hr))
		{
			hr = pSessionConnectionPoint->Unadvise(dwSessionCookie);
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	else
	{
		ASSERT(FALSE);
	}
	dwSessionCookie = 0;
}

HRESULT CMcLoaderImpl::Send(HWND Hwnd, UINT ProgressMsg, UINT CompletedMsg, LPCTSTR strHost, LPCTSTR strSID)
{
	USES_CONVERSION;

	m_MsgHwnd		=	Hwnd;
	m_ProgressMsg	=	ProgressMsg;
	m_CompletedMsg	=	CompletedMsg;
	
	HRESULT hr	=	S_OK;

	CComBSTR	bsURL	=	m_bsUrl;

	Replace(bsURL.m_str, L"#sid#", T2W(const_cast<LPTSTR>(strSID)));
	Replace(bsURL.m_str, L"#host#", T2W(const_cast<LPTSTR>(strHost)));

	try
	{
		m_pForm->put_url(bsURL);
		m_pForm->Send();
	}
	catch(_com_error &e)
	{
		hr = e.Error();
	}

	return hr;
}

HRESULT CMcLoaderImpl::SetXML(BSTR XML)
{
	/************************************************************************/
	/*<Data>
		<URL>#host#/Apps/News/NewsUpload.asp?sid=#sid#</URL>
		<Files>
			<File>
				<FileName>c:/ex.txt</FileName>
				<TypeId>1</TypeId>
				<Title>File ex.txt</Title>
		    </File>

		</Files>
	  </Data>
	*/
	/************************************************************************/

	HRESULT hr	=	E_FAIL;

	// Step 1. Get Url for uloading [4/24/2002]

	CComPtr<IXMLDOMDocument>	pDoc	=	NULL;
	hr = pDoc.CoCreateInstance(CLSID_DOMDocument40, NULL, CLSCTX_INPROC_SERVER);
	
	if(pDoc!=NULL)
	{
		VARIANT_BOOL	varSuc	=	VARIANT_FALSE;
		// Step 0. Load New XML [3/2/2002]
		hr = pDoc->loadXML(XML,&varSuc);
		if(varSuc==VARIANT_TRUE)
		{
			CComPtr<IXMLDOMNode>	pNodeUrl, pTitleUrl;

			pDoc->QueryInterface(IID_IXMLDOMNode,(void**)&pTitleUrl);

			GetTextByPath(pTitleUrl,CComBSTR(L"//Title"),&m_bsTitle);

			hr =  pDoc->selectSingleNode(CComBSTR(L"*/URL"),&pNodeUrl);
			
			if(pNodeUrl!=NULL)
			{
				hr	=	pNodeUrl->get_text(&m_bsUrl);
				
				// Step 2. Add XML To Send [4/24/2002]
				try
				{
					m_pForm->AddField(L"Stream",XML);
				}
				catch (_com_error	&e) 
				{
					hr	=	 e.Error();
					return hr;
				}
				
				// Step 3. If Enable a File then Add the File To Upload [4/24/2002]
				CComPtr<IXMLDOMNodeList>	pNodeFileList	=	NULL;
				
				hr =  pDoc->selectNodes(CComBSTR(L"*/Files/File"),&pNodeFileList);
				if(pNodeFileList!=NULL)
				{
					long	lListLehgth	=	0;
					pNodeFileList->get_length(&lListLehgth);
					
					for(long	i=0;i<lListLehgth;i++)
					{
						CComPtr<IXMLDOMNode>		pNodeFile	=	NULL;
						
						hr	=	pNodeFileList->get_item(i,&pNodeFile);
						if(pNodeFile!=NULL)
						{
							CComBSTR	bsFilePath, bsFormName;

							GetTextByPath(pNodeFile,CComBSTR(L"FileName"),&bsFilePath);
							GetTextByPath(pNodeFile,CComBSTR(L"FormName"),&bsFormName);

							LPWSTR pStrMimeType = NULL;
							FindMimeFromData(NULL, bsFilePath, NULL, 0, NULL, 0, &pStrMimeType, 0);

							try
							{
								m_pForm->AddFile((BSTR)bsFilePath,bsFormName.Length()?(BSTR)bsFormName:L"File",pStrMimeType?pStrMimeType:L"Application/Octet-Stream");
							}
							catch (_com_error&) 
							{
								ASSERT(FALSE);
							}
						}
					}
				}

			}
		}
	}

	return	hr;
}


BSTR CMcLoaderImpl::GetFilePath()
{
	return m_bsFilePath;
}

void CMcLoaderImpl::OnPushTOQueue()
{
	
}

void CMcLoaderImpl::OnUploadBegin(long Size)
{
	m_lFullSize	=	Size;
	
	if(IsWindow(m_MsgHwnd))
	{
		::PostMessage(m_MsgHwnd,m_ProgressMsg,WPARAM(this),LPARAM(0));
	}
}

void CMcLoaderImpl::OnUploadStep(long Size)
{
	m_lUploadSize	=	Size;
	if(IsWindow(m_MsgHwnd))
	{
		::PostMessage(m_MsgHwnd,m_ProgressMsg,WPARAM(this),LPARAM(m_lUploadSize));
	}
}

void CMcLoaderImpl::OnUploadEnd()
{
	m_lUploadSize	=	m_lFullSize;
	
	if(IsWindow(m_MsgHwnd))
	{
		::PostMessage(m_MsgHwnd,m_ProgressMsg,WPARAM(this),LPARAM(m_lFullSize));
	}
}

void CMcLoaderImpl::OnDownloadBegin(long Size)
{
	
}

void CMcLoaderImpl::OnDownloadStep(long Size)
{
}

void CMcLoaderImpl::OnDownloadEnd()
{
	
}

void CMcLoaderImpl::OnCompleted(long ErrorType, long ErrorCode)
{
	if (ErrorCode==S_OK) 
	{
		long HttpStatus	=	0;
		try
		{
			m_pForm->get_HTTPStatus(&HttpStatus);
		}
		catch (_com_error&) 
		{
		}
		
		if(HttpStatus!=200)
		{
			ErrorCode	=	E_FAIL;
		}
	}

	if(IsWindow(m_MsgHwnd))
	{
		::PostMessage(m_MsgHwnd,m_CompletedMsg,WPARAM(this),LPARAM(ErrorCode));
	}
}

BSTR CMcLoaderImpl::GetTitle()
{
	return m_bsTitle;
}
