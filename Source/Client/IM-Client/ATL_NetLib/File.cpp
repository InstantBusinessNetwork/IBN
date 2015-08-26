// File.cpp : Implementation of CFile
#include "stdafx.h"
#include "ATL_NetLib.h"
#include "File.h"
#include "Session.h"

/////////////////////////////////////////////////////////////////////////////
// CFile


STDMETHODIMP CFile::get_FID(BSTR *pVal)
{
	return m_sFile.m_FID.CopyTo(pVal);
}

STDMETHODIMP CFile::put_FID(BSTR newVal)
{
  m_sFile.m_FID.Empty();
  m_sFile.m_FID = newVal;
  return (m_sFile.m_FID || !newVal ? S_OK : E_OUTOFMEMORY);
}

STDMETHODIMP CFile::get_RealName(BSTR *pVal)
{
	return m_sFile.m_RealName.CopyTo(pVal);
}

STDMETHODIMP CFile::put_RealName(BSTR newVal)
{
  m_sFile.m_RealName.Empty();
  m_sFile.m_RealName = newVal;
  return (m_sFile.m_RealName || !newVal ? S_OK : E_OUTOFMEMORY);
}

STDMETHODIMP CFile::get_Body(BSTR *pVal)
{
	return m_sFile.m_Body.CopyTo(pVal);
}

STDMETHODIMP CFile::put_Body(BSTR newVal)
{
  m_sFile.m_Body.Empty();
  m_sFile.m_Body = newVal;
  return (m_sFile.m_Body || !newVal ? S_OK : E_OUTOFMEMORY);
}

STDMETHODIMP CFile::get_date_time(long *pVal)
{
	if (pVal == NULL)
		return E_POINTER;

	*pVal = m_sFile.m_Time;
	return S_OK;
}

STDMETHODIMP CFile::get_Sender(IUser **pVal)
{
	if(pVal == NULL)
		return E_POINTER;

	HRESULT hr;
	IUser*  pUser = NULL;
	CUser*  mpUser = NULL;

	hr = CUser::CreateInstance(&pUser);
	if(hr != NULL)
		return hr;
	
	mpUser = static_cast<CUser*>(pUser);
	mpUser->m_sUser = m_sFile.m_sSender; //????????????????????????
	
	*pVal = pUser;
	return S_OK;
}

STDMETHODIMP CFile::get_Recipients(IUsers **pVal)
{
	return m_pRecipients.CopyTo(pVal);
}

STDMETHODIMP CFile::Send(long *Handle)
{
	if(Handle == NULL)
		return E_POINTER;

	if(m_pSession->GetState() != stConnected)
		return E_PENDING;
	try
	{
		m_pSession->m_IM_NET->m_CommandQueue.x_SendFile(this,*Handle);
		m_pSession->m_IM_NET->SetNewCommand();
		
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
		case WRONG_PARAM:
			return E_INVALIDARG;
			break;
			
		default:
			return E_INVALIDQUEUE;
			break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

STDMETHODIMP CFile::Receive(long *Handle)
{
	if(Handle == NULL)
		return E_POINTER;
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;

	m_pSession->m_IM_NET->m_CommandQueue.x_ReceiveFile(this,*Handle);
	m_pSession->m_IM_NET->SetNewCommand();
	return S_OK;
}

STDMETHODIMP CFile::get_SID(BSTR *pVal)
{
	return m_sFile.m_SID.CopyTo(pVal);
}

STDMETHODIMP CFile::put_hWnd(long newVal)
{
	m_sFile.hBackWind = newVal;
	return S_OK;
}

STDMETHODIMP CFile::get_Size(long *pVal)
{
	if (pVal == NULL)
		return E_POINTER;

	*pVal = m_sFile.m_size;
	return S_OK;

}
