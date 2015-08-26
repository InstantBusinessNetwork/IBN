// LoadSkins.cpp: implementation of the LoadSkins class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "LoadSkins.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif
#include <urlmon.h>
#include <wininet.h>
#include "resource.h"
#include <commctrl.h>

#define EDIT_BOX_LIMIT 0x7FFF    //  The Edit box limit

#define MAX_SKIN_HASH_SIZE	(32)

typedef struct  _StreamCashItem
{
	CComBSTR			Path;
	CComPtr<IStream>	Data;
} StreamCashItem;

typedef std::list<StreamCashItem >		CStreamCashList;
typedef CStreamCashList::iterator		CStreamCashListIterator;

static CStreamCashList		m_StreamCashList;

// %%Classes: ----------------------------------------------------------------
class CDownload {
public:
    CDownload();
    ~CDownload();
    HRESULT DoDownload(BSTR URL, IStream** ppStream, HANDLE hBackEvent, long* ErrorCode);
	
private:
    IMoniker*            m_pmk;
    IBindCtx*            m_pbc;
    IBindStatusCallback* m_pbsc;
	
};

class CBindStatusCallback : public IBindStatusCallback {
public:
    // IUnknown methods
    STDMETHODIMP    QueryInterface(REFIID riid,void ** ppv);
    STDMETHODIMP_(ULONG)    AddRef()    { return m_cRef++; }
    STDMETHODIMP_(ULONG)    Release()   
	{ 
		if (--m_cRef == 0) 
		{ delete this; return 0; } 
		return m_cRef; 
	}
	
    // IBindStatusCallback methods
    STDMETHODIMP    OnStartBinding(DWORD dwReserved, IBinding* pbinding);
    STDMETHODIMP    GetPriority(LONG* pnPriority);
    STDMETHODIMP    OnLowResource(DWORD dwReserved);
    STDMETHODIMP    OnProgress(ULONG ulProgress, ULONG ulProgressMax, ULONG ulStatusCode,
		LPCWSTR pwzStatusText);
    STDMETHODIMP    OnStopBinding(HRESULT hrResult, LPCWSTR szError);
    STDMETHODIMP    GetBindInfo(DWORD* pgrfBINDF, BINDINFO* pbindinfo);
    STDMETHODIMP    OnDataAvailable(DWORD grfBSCF, DWORD dwSize, FORMATETC *pfmtetc,
		STGMEDIUM* pstgmed);
    STDMETHODIMP    OnObjectAvailable(REFIID riid, IUnknown* punk);
	
    // constructors/destructors
    CBindStatusCallback(IStream** pStream, long* ErrorCode,  HANDLE hBackEvent);
    ~CBindStatusCallback();
	
    // data members
    DWORD           m_cRef;
	long*			m_ErrorCode;
    IBinding*       m_pbinding;
    IStream*        m_pstm;
	IStream**		m_ppStream;
    DWORD           m_cbOld;
	HANDLE			m_hBackEvent;
};


// ===========================================================================
//                     CBindStatusCallback Implementation
// ===========================================================================

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::CBindStatusCallback
// ---------------------------------------------------------------------------
CBindStatusCallback::CBindStatusCallback(IStream** ppStream, long* ErrorCode,  HANDLE hBackEvent)
{
	m_ErrorCode = ErrorCode;
	m_ppStream	= ppStream;
    m_pbinding = NULL;
	m_hBackEvent = hBackEvent;
    m_pstm = NULL;
    m_cRef = 1;
    m_cbOld = 0;
}  // CBindStatusCallback

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::~CBindStatusCallback
// ---------------------------------------------------------------------------
CBindStatusCallback::~CBindStatusCallback()
{
}  // ~CBindStatusCallback

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::QueryInterface
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::QueryInterface(REFIID riid, void** ppv)
{
    *ppv = NULL;
	
    if (riid==IID_IUnknown || riid==IID_IBindStatusCallback)
	{
        *ppv = this;
        AddRef();
        return S_OK;
	}
    return E_NOINTERFACE;
}  // CBindStatusCallback::QueryInterface

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::OnStartBinding
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::OnStartBinding(DWORD dwReserved, IBinding* pbinding)
{
    if (m_pbinding != NULL)
        m_pbinding->Release();
    m_pbinding = pbinding;
    if (m_pbinding != NULL)
	{
        m_pbinding->AddRef();
		
	}
    return S_OK;
}  // CBindStatusCallback::OnStartBinding

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::GetPriority
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::GetPriority(LONG* pnPriority)
{
	return E_NOTIMPL;
}  // CBindStatusCallback::GetPriority

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::OnLowResource
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::OnLowResource(DWORD dwReserved)
{
    return E_NOTIMPL;
}  // CBindStatusCallback::OnLowResource

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::OnProgress
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::OnProgress(ULONG ulProgress, ULONG ulProgressMax, ULONG ulStatusCode, LPCWSTR szStatusText)
{
    return(NOERROR);
}  // CBindStatusCallback::OnProgress

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::OnStopBinding
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::OnStopBinding(HRESULT hrStatus, LPCWSTR pszError)
{
	
	if (m_pbinding)
	{
		int i = m_pbinding->Release();
		m_pbinding = NULL;
	}
	SetEvent(m_hBackEvent);
    return S_OK;
}  // CBindStatusCallback::OnStopBinding

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::GetBindInfo
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::GetBindInfo(DWORD* pgrfBINDF, BINDINFO* pbindInfo)
{
	if (!pbindInfo || !pbindInfo->cbSize || !pgrfBINDF)
		return E_INVALIDARG;
	
		*pgrfBINDF = /*BINDF_ASYNCHRONOUS | BINDF_ASYNCSTORAGE |*/ BINDF_PULLDATA /*|
			BINDF_GETNEWESTVERSION  | BINDF_NOWRITECACHE*/;
			
			// remember incoming cbSize
			ULONG cbSize = pbindInfo->cbSize;
		// zero out structure
		memset(pbindInfo, 0, cbSize);
		
		// restore cbSize
		pbindInfo->cbSize = cbSize;
		pbindInfo->dwBindVerb = BINDVERB_GET;
		
		return S_OK;
}  // CBindStatusCallback::GetBindInfo

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::OnDataAvailable
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::OnDataAvailable(DWORD grfBSCF, DWORD dwSize, FORMATETC* pfmtetc, STGMEDIUM* pstgmed)
{
	HRESULT hr=S_OK;
	DWORD dStrlength=0;
	
	// Get the Stream passed
    if (BSCF_FIRSTDATANOTIFICATION & grfBSCF)
    {
        if (!m_pstm && pstgmed->tymed == TYMED_ISTREAM)
		{
			m_pstm = pstgmed->pstm;
            if (m_pstm)
                m_pstm->AddRef();
		}
    }
	
	if (BSCF_LASTDATANOTIFICATION & grfBSCF)
	{
        if (m_pstm)
		{
			//*m_ppStream = m_pstm;
			m_pstm->Release();
			*m_ErrorCode  = 0;
		}
		hr=S_OK;  // If it was the last data then we should return S_OK as we just finished reading everything
	}
	
    return hr;
}  // CBindStatusCallback::OnDataAvailable

// ---------------------------------------------------------------------------
// %%Function: CBindStatusCallback::OnObjectAvailable
// ---------------------------------------------------------------------------
STDMETHODIMP
CBindStatusCallback::OnObjectAvailable(REFIID riid, IUnknown* punk)
{
    return E_NOTIMPL;
}  // CBindStatusCallback::OnObjectAvailable


// ===========================================================================
//                           CDownload Implementation
// ===========================================================================

// ---------------------------------------------------------------------------
// %%Function: CDownload::CDownload
// ---------------------------------------------------------------------------
CDownload::CDownload()
{
    m_pmk = 0;
    m_pbc = 0;
    m_pbsc = 0;
}  // CDownload

// ---------------------------------------------------------------------------
// %%Function: CDownload::~CDownload
// ---------------------------------------------------------------------------
CDownload::~CDownload()
{
    if (m_pmk)
        m_pmk->Release();
    if (m_pbc)
        m_pbc->Release();
    if (m_pbsc)
	{
        int i = m_pbsc->Release();
		ASSERT(i==0);
		//		delete m_pbsc;
	}
}  // ~CDownload

// ---------------------------------------------------------------------------
// %%Function: CDownload::DoDownload
// ---------------------------------------------------------------------------
HRESULT
CDownload::DoDownload(BSTR URL, IStream** pStream, HANDLE hBackEvent, long* ErrorCode)
{
    HRESULT hr;
	
    hr = CreateURLMoniker(NULL, URL, &m_pmk);
    if (FAILED(hr))
        goto LErrExit;
	
    m_pbsc = new CBindStatusCallback(pStream, ErrorCode, hBackEvent);
    if (m_pbsc == NULL)
	{
        hr = E_OUTOFMEMORY;
        goto LErrExit;
	}
	
    hr = CreateBindCtx(0, &m_pbc);
    if (FAILED(hr))
        goto LErrExit;
	
    hr = RegisterBindStatusCallback(m_pbc,
		m_pbsc,
		0,
		0L);
    if (FAILED(hr))
        goto LErrExit;
	
    hr = m_pmk->BindToStorage(m_pbc, 0, IID_IStream, (void**)pStream);
    if (FAILED(hr))
        goto LErrExit;
	
    return hr;  
	
LErrExit:
    if (m_pbc != NULL)
	{
        m_pbc->Release();
        m_pbc = NULL;
	}
    if (m_pbsc != NULL)
	{
        m_pbsc->Release();
        m_pbsc = NULL;
	}
    if (m_pmk != NULL)
	{
        m_pmk->Release();
        m_pmk = NULL;
	}
	if (*pStream)
	{
		(*pStream)->Release();
		*pStream = NULL;
	}
    return hr;
}  // CDownload::DoDownload


//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

LoadSkins::LoadSkins()
{
	InitializeCriticalSection(&m_lock);
	
}

LoadSkins::~LoadSkins()
{
	DeleteCriticalSection(&m_lock);
}




HRESULT LoadSkins::Load(BSTR URL, IStream **pStream, long* ErrorCode)
{
	if(pStream == NULL || URL == NULL)
		return E_INVALIDARG;
	
	CDownload	m_Download;
	HANDLE		hBackEvent;
	HRESULT		hr;

	ASSERT(*pStream==NULL);
	
	EnterCriticalSection(&m_lock);
	
	try
	{
		// Step 1. Check Stream in the Cash [7/26/2002]
		for(CStreamCashListIterator ItemIndex = m_StreamCashList.begin();
		ItemIndex!=m_StreamCashList.end();ItemIndex++)
		{
			StreamCashItem	Item = (*ItemIndex);
			if(Item.Path==CComBSTR(URL))
			{
				
				hr = Item.Data->Clone(pStream);
				if(hr==S_OK)
				{
					// May Be Remove ???? [7/26/2002]
					//LARGE_INTEGER	dlibMove	=	{0};
					//*pStream->Seek(dlibMove,0,0);
					*ErrorCode	=	0;

					//if(ItemIndex!= m_StreamCashList.begin())
					//{
					//	m_StreamCashList.push_front((*ItemIndex));
					//	m_StreamCashList.erase(ItemIndex);
					//}
					return hr;
				}
				else
				{
					///TRACE("\r\n $$$ LoadSkins::Load/Remove Item From Cash $$$");
					// Remove Item From Cash [7/26/2002]
					m_StreamCashList.erase(ItemIndex);
				}
			}
		}
		
		// Step 2. Load Stream from Skin file [7/26/2002]
		hBackEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
		
		hr = m_Download.DoDownload(URL, pStream, hBackEvent, ErrorCode);
		if(hr == S_OK)
		{
			::WaitForSingleObject(hBackEvent,INFINITE);
			
			if(*ErrorCode==0&&*pStream)
			{
				//TRACE("\r\n LoadSkins::Load/New Item Present in the Hash");
				StreamCashItem	newItem;
				
				newItem.Path	=	URL;
				hr = (*pStream)->Clone(&newItem.Data);
				m_StreamCashList.push_front(newItem);
				
				// Check Mas Cash List size [7/26/2002]
				if(m_StreamCashList.size()>MAX_SKIN_HASH_SIZE)
				{
					m_StreamCashList.pop_back();
				}
			}
		}
		CloseHandle(hBackEvent);
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	
	LeaveCriticalSection(&m_lock);
	
	return hr;
}
