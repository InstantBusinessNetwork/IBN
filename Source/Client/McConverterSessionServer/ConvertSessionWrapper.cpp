// ConvertSessionWrapper.cpp : Implementation of CConvertSessionWrapper

#include "stdafx.h"
#include "ConvertSessionWrapper.h"
#include "ConvertSession.h"

#define USES_IID_IMessage
#include <MAPIGuid.h>
// CConvertSessionWrapper

STDMETHODIMP CConvertSessionWrapper::SaveMapiMessageTmpFile(VARIANT pUnkMessage, long ulEncType, BSTR* pTmpFileNameRetVal)
{
	HRESULT hr = S_FALSE;
	if(pTmpFileNameRetVal != NULL)
	{
		CComBSTR tmpFileName = GetTemporaryPath();
		if(tmpFileName.Length() != 0)
		{
			hr = SaveMapiMessage(tmpFileName, pUnkMessage, ulEncType);
			*pTmpFileNameRetVal = tmpFileName;
		}
	}
	return hr;
}

STDMETHODIMP CConvertSessionWrapper::SaveMapiMessage(BSTR bstrFileName,  VARIANT pObject, long encodingType)
{
	// TODO: Add your implementation code here
	CComPtr<IConverterSession> pConvSess;
	CComPtr<IStream> pStream;
	//LPMESSAGE pMessage;
	ENCODINGTYPE encType = (ENCODINGTYPE)encodingType;
	CComQIPtr<IMessage, &IID_IMessage> pMessage;
	LPUNKNOWN pUnk;

	HRESULT hr = Initialize();
	if (hr != S_OK)
	{
		return hr;
	}

	if (pObject.vt != (VT_DISPATCH))
	{
		return E_INVALIDARG;
	}

	pUnk = GetMessageUnk(pObject.pdispVal);
	pMessage = pUnk;

	if(pMessage == NULL)
	{
		return E_INVALIDARG;
	}

	USES_CONVERSION;
	LPTSTR fileName = OLE2T(bstrFileName);

	// hr = pUnkMessage->QueryInterface(IID_IMessage, (void**)&pMessage);
	if(pMessage != NULL)
	{

		hr = ::CoCreateInstance(CLSID_IConverterSession, NULL, CLSCTX_INPROC_SERVER, 
			IID_IConverterSession, (void**)&pConvSess);

		if(hr != S_OK)
		{
			return hr;
		}

		hr = OpenStreamOnFile(MAPIAllocateBuffer, MAPIFreeBuffer, STGM_CREATE |	STGM_READWRITE, fileName, NULL, &pStream);

		if (hr != S_OK)
		{
			return hr;
		}

		hr = pConvSess->SetEncoding(encType);

		hr = pConvSess->MAPIToMIMEStm(pMessage, pStream, CCSF_SMTP | CCSF_INCLUDE_BCC); 
	}

	MAPIUninitialize();
	pUnk->Release();

	return hr;
}

HRESULT CConvertSessionWrapper::Initialize()
{
#define MAPI_NO_COINIT 0x08
	MAPIINIT_0 MAPIINIT = { 0, MAPI_MULTITHREAD_NOTIFICATIONS | MAPI_NO_COINIT };
	HRESULT hr = MAPIInitialize(&MAPIINIT);
	if(hr == MAPI_E_UNKNOWN_FLAGS)
	{
		MAPIINIT.ulFlags &= ~MAPI_NO_COINIT;
		hr = MAPIInitialize(&MAPIINIT);		
	}	
	if (FAILED(hr)) hr = MAPIInitialize (NULL);

	return hr;
}

LPUNKNOWN CConvertSessionWrapper::GetMessageUnk(LPDISPATCH pDisp)
{
	DISPID		rgDispId;
	DISPPARAMS	dispparams = {NULL, NULL, 0, 0};
	VARIANT		vaResult;	
	LPOLESTR	pName = L"MAPIOBJECT";
	VariantInit(&vaResult);

	if (SUCCEEDED(pDisp->GetIDsOfNames(IID_NULL,&pName,1,LOCALE_SYSTEM_DEFAULT,&rgDispId)))
		if (SUCCEEDED(pDisp->Invoke(rgDispId, IID_NULL, 0, DISPATCH_PROPERTYGET, &dispparams, &vaResult, NULL, NULL)))
			if (vaResult.vt == VT_UNKNOWN)
			{
				return vaResult.punkVal;
			}

			return NULL;
}

CComBSTR CConvertSessionWrapper::GetTemporaryPath()
{

	CComBSTR tmpPath;
	CComBSTR tmpFileName;
	LPSTR tmpBuff = new TCHAR[MAX_PATH];
	if(::GetTempPath(MAX_PATH, tmpBuff) != 0)
	{
		USES_CONVERSION;
		tmpPath = T2W(tmpBuff);
		tmpFileName = GetTmpFileName();
		tmpPath.AppendBSTR(tmpFileName);
	}

	return tmpPath;
}

CComBSTR CConvertSessionWrapper::GetTmpFileName()
{
	int iErr, iTmpFileSize;
	CComBSTR pwTemplate(L"mail_tmp_XXXXXX");
	_wmktemp_s(pwTemplate, pwTemplate.Length() + 1);

	return pwTemplate;
}