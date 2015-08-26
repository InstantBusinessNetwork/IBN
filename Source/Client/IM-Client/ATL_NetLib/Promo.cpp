// Promo.cpp : Implementation of CPromo
#include "stdafx.h"
#include "ATL_NetLib.h"
#include "Promo.h"
#include "Session.h"

/////////////////////////////////////////////////////////////////////////////
// CPromo

STDMETHODIMP CPromo::get_PID(BSTR *pVal)
{
	return m_sPromo.m_PID.CopyTo(pVal);
}

STDMETHODIMP CPromo::put_PID(BSTR newVal)
{
	if(newVal == NULL) return E_INVALIDARG;
	m_sPromo.m_PID.Empty();
	m_sPromo.m_PID = newVal;
	
	return S_OK;
}

STDMETHODIMP CPromo::get_date_time(long *pVal)
{
	if(pVal == NULL) return E_POINTER;
	*pVal = m_sPromo.m_Time;
	return S_OK;
}

STDMETHODIMP CPromo::get_SID(BSTR *pVal)
{
	return m_sPromo.m_SID.CopyTo(pVal);
}

STDMETHODIMP CPromo::get_Subject(BSTR *pVal)
{
	return m_sPromo.m_Subject.CopyTo(pVal);
}

STDMETHODIMP CPromo::put_Subject(BSTR newVal)
{
	m_sPromo.m_Subject.Empty();
	m_sPromo.m_Subject = newVal;
	return S_OK;
}

STDMETHODIMP CPromo::get_Product_ID(long *pVal)
{
	if(pVal == NULL) return E_POINTER;
	*pVal = m_sPromo.m_Product_ID;
	return S_OK;
}

STDMETHODIMP CPromo::put_Product_ID(long newVal)
{
	m_sPromo.m_Product_ID = newVal;
	return S_OK;
}

STDMETHODIMP CPromo::get_Body(BSTR *pVal)
{
	return m_sPromo.m_Body.CopyTo(pVal);
	return S_OK;
}

STDMETHODIMP CPromo::put_Body(BSTR newVal)
{
	m_sPromo.m_Body.Empty();
	m_sPromo.m_Body= newVal;
	return S_OK;
}

STDMETHODIMP CPromo::get_Recipients(IUsers **pVal)
{
	return m_pRecipients.CopyTo(pVal);
}


STDMETHODIMP CPromo::get_Sender(IUser **pVal)
{
	if(pVal == NULL)
		return E_POINTER;
	
	HRESULT hr;
	IUser*	pUser = NULL;
	CUser*  m_pUser;
	hr = CUser::CreateInstance(&pUser);
	if(hr != NULL)
		return hr;

	m_pUser = static_cast<CUser*>(pUser);
	m_pUser->m_sUser = m_sPromo.m_sSender;

	*pVal = pUser;
	return S_OK;
}

STDMETHODIMP CPromo::get_ProductName(BSTR *pVal)
{
	return m_sPromo.m_ProductName.CopyTo(pVal);
}

STDMETHODIMP CPromo::put_ProductName(BSTR newVal)
{
	m_sPromo.m_ProductName.Empty();
	m_sPromo.m_ProductName = newVal;
	return S_OK;
}

STDMETHODIMP CPromo::Send(long *Handle)
{
	if(Handle == NULL)
		return E_POINTER;
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;
	try
	{
		m_pSession->m_IM_NET->m_CommandQueue.x_Promo(this,*Handle);
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
