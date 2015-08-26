// Files.h: Definition of the CFiles class
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_FILES_H__481389BA_C44C_49A8_A6E3_8280E4A369BF__INCLUDED_)
#define AFX_FILES_H__481389BA_C44C_49A8_A6E3_8280E4A369BF__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "resource.h"       // main symbols
#include "collections.h"
#include "file.h"
#pragma warning(disable: 4530)
typedef CComEnumOnSTL<IEnumVARIANT, &IID_IEnumVARIANT, VARIANT,
                        _CopyVariantFromAdaptItf<IFile>,
                        list< CAdapt< CComPtr<IFile> > > >
        CComEnumVariantOnListOfFiles;

typedef ICollectionOnSTLImpl<IDispatchImpl<IFiles, &IID_IFiles>,
                             list< CAdapt< CComPtr<IFile> > >,
                             IFile*,
                             _CopyItfFromAdaptItf<IFile>,
                             CComEnumVariantOnListOfFiles>
        IFilesCollImpl;
#pragma warning(default: 4530)
/////////////////////////////////////////////////////////////////////////////
// CFiles

class ATL_NO_VTABLE CFiles : 
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CFiles>,
	public IFilesCollImpl
{
public:
	CFiles() {}
BEGIN_COM_MAP(CFiles)
	COM_INTERFACE_ENTRY(IFiles)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()
DECLARE_NO_REGISTRY()
DECLARE_PROTECT_FINAL_CONSTRUCT()
DECLARE_NOT_AGGREGATABLE(CFiles) 
	HRESULT CFiles::FinalRelease()
	{
		MCTRACE(5,"FileS LIST DELETED");
		return S_OK;
	};
  
	STDMETHODIMP CFiles::AddFile(IFile** ppFile) 
	{ 
		HRESULT hr = CFile::CreateInstance(ppFile);
		if( SUCCEEDED(hr) ) 
		{
			// Put the document on the list
			CComPtr<IFile>  spFile = *ppFile;
			m_coll.push_back(spFile);
		}
	  return hr;
	}
// IFiles
public:
};

#endif // !defined(AFX_FILES_H__481389BA_C44C_49A8_A6E3_8280E4A369BF__INCLUDED_)
