//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=

#include "stdafx.h"
//#include "CEdit.h"
#include "OfsDhtmlCtrl.h"
#include "site.h"


HRESULT
CSite::HrFileToStream(LPCTSTR fileName, LPSTREAM* ppiStream)
{
	HRESULT hr = S_OK;
	CFile file;
	DWORD fileLen = 0;
	HGLOBAL hMem = NULL;
	LPVOID memBasePtr = NULL;
	UCHAR* bufp = NULL;

	if (file.Open(fileName, CFile::modeRead, NULL) == FALSE)
		return E_FAIL;

	fileLen = (file.GetLength() / 2048 + 1) * 2048;

	hMem = GlobalAlloc(GHND, fileLen);

	if (NULL == hMem)
	{
		file.Close();
		return E_OUTOFMEMORY;
	}

	VERIFY(memBasePtr = GlobalLock(hMem));

	bufp = (UCHAR*) memBasePtr;

	BOOL moreToRead = TRUE;
	while(moreToRead)
	{
			UINT bytesRead = 0;

			bytesRead = file.Read(bufp, 2048);			
			if (bytesRead < 2048)
				moreToRead = FALSE;
			else
				bufp += bytesRead;
	}

	file.Close();
	VERIFY(GlobalUnlock(hMem) == 0);
	hr = CreateStreamOnHGlobal(hMem, TRUE, ppiStream);

	return hr;
}


HRESULT
CSite::HrStreamToFile(LPSTREAM pStream, LPCTSTR fileName)
{
	HRESULT hr = S_OK;
	HGLOBAL hMem = NULL;
	LPVOID memBuf = NULL;
	DWORD memSize = 0;
	CFile file;

	if (file.Open(fileName, CFile::modeCreate | CFile::modeReadWrite, NULL) == FALSE)
		return E_FAIL;

	hr = GetHGlobalFromStream(pStream, &hMem);

	ASSERT(SUCCEEDED(hr));

	memSize = GlobalSize(hMem);
	VERIFY(memBuf = GlobalLock(hMem));

	file.Write(memBuf, memSize);

	// Reference count of hMem not checked here
	// since we can't assume how many times the
	// Stream has locked it

	GlobalUnlock(hMem); 
	file.Close();

	return hr;
}


HRESULT
CSite::HrFilterIn(LPCTSTR fileName, LPSTREAM* ppFilteredStream, DWORD dwFlags)
{
	HRESULT hr = S_OK;
	IStream* pInputStream = NULL;
	ITriEditDocument* pTriEditDoc = NULL;
	IUnknown* lpUnk = NULL;
	IUnknown* lpStreamUnk = NULL;
	
	lpUnk = GetObjectUnknown();

	if (NULL == lpUnk)
		return E_FAIL;

	// get ITriEditDoc
	if (FAILED(hr = lpUnk->QueryInterface(IID_ITriEditDocument, (LPVOID*) &pTriEditDoc)))
		return hr;

	if (FAILED(hr = HrFileToStream(fileName, &pInputStream)))
		goto cleanup;

	if (FAILED(hr = pTriEditDoc->FilterIn(pInputStream, &lpStreamUnk, dwFlags, NULL)))
		goto cleanup;

	if (FAILED(hr = lpStreamUnk->QueryInterface(IID_IStream, (LPVOID*) ppFilteredStream)))
		goto cleanup;

cleanup:

	ReleaseInterface(pTriEditDoc);
	ReleaseInterface(lpStreamUnk);
	ReleaseInterface(pInputStream);
	return hr;
}

HRESULT
CSite::HrFilterInStream(LPSTREAM  pInputStream, LPSTREAM* ppFilteredStream, DWORD dwFlags)
{
	HRESULT hr = S_OK;
//	IStream* pInputStream = NULL;
	ITriEditDocument* pTriEditDoc = NULL;
	IUnknown* lpUnk = NULL;
	IUnknown* lpStreamUnk = NULL;
	
	lpUnk = GetObjectUnknown();

	if (NULL == lpUnk)
		return E_FAIL;

	// get ITriEditDoc
	if (FAILED(hr = lpUnk->QueryInterface(IID_ITriEditDocument, (LPVOID*) &pTriEditDoc)))
		return hr;

//	if (FAILED(hr = HrFileToStream(fileName, &pInputStream)))
//		goto cleanup;

	if (FAILED(hr = pTriEditDoc->FilterIn(pInputStream, &lpStreamUnk, dwFlags, NULL)))
		goto cleanup;

	if (FAILED(hr = lpStreamUnk->QueryInterface(IID_IStream, (LPVOID*) ppFilteredStream)))
		goto cleanup;

cleanup:

	ReleaseInterface(pTriEditDoc);
	ReleaseInterface(lpStreamUnk);
//	ReleaseInterface(pInputStream);
	return hr;
}




HRESULT
CSite::HrFilterOut(LPSTREAM pSourceStream, LPCTSTR fileName, DWORD dwFlags)
{
	HRESULT hr = S_OK;
	IStream* pOutputStream = NULL;
	ITriEditDocument* pTriEditDoc = NULL;
	IUnknown* lpUnk = NULL;
	IUnknown* lpStreamUnk = NULL;
	
	lpUnk = GetObjectUnknown();

	if (NULL == lpUnk)
		return E_FAIL;

	// get ITriEditDoc
	if (FAILED(hr = lpUnk->QueryInterface(IID_ITriEditDocument, (LPVOID*) &pTriEditDoc)))
		return hr;

	if (FAILED(hr = pTriEditDoc->FilterOut(pSourceStream, &lpStreamUnk, dwFlags, NULL)))
		goto cleanup;

	if (FAILED(hr = lpStreamUnk->QueryInterface(IID_IStream, (LPVOID*) &pOutputStream)))
		goto cleanup;

	if (FAILED(hr = HrStreamToFile(pOutputStream, fileName)))
		goto cleanup;



cleanup:

	ReleaseInterface(pTriEditDoc);
	ReleaseInterface(lpStreamUnk);
	ReleaseInterface(pOutputStream);
	return hr;
}

HRESULT
CSite::HrFilterOutStream(LPSTREAM pSourceStream, LPSTREAM &pOutputStream, DWORD dwFlags)
{
	HRESULT hr = S_OK;
//	IStream* pOutputStream = NULL;
	ITriEditDocument* pTriEditDoc = NULL;
	IUnknown* lpUnk = NULL;
	IUnknown* lpStreamUnk = NULL;
	
	lpUnk = GetObjectUnknown();

	if (NULL == lpUnk)
		return E_FAIL;

	// get ITriEditDoc
	if (FAILED(hr = lpUnk->QueryInterface(IID_ITriEditDocument, (LPVOID*) &pTriEditDoc)))
		return hr;

	if (FAILED(hr = pTriEditDoc->FilterOut(pSourceStream, &lpStreamUnk, dwFlags, NULL)))
		goto cleanup;

	if (FAILED(hr = lpStreamUnk->QueryInterface(IID_IStream, (LPVOID*) &pOutputStream)))
		goto cleanup;

//	if (FAILED(hr = HrStreamToFile(pOutputStream, fileName)))
//		goto cleanup;



cleanup:

	ReleaseInterface(pTriEditDoc);
	ReleaseInterface(lpStreamUnk);
//	ReleaseInterface(pOutputStream);
	return hr;
}

