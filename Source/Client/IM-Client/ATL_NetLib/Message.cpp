// Message.cpp : Implementation of CMessage
#pragma warning(disable: 4530)
#include "stdafx.h"
#include "ATL_NetLib.h"
#include "Message.h"
#include "User.h"
#include "Session.h"
/////////////////////////////////////////////////////////////////////////////
// CMessage


STDMETHODIMP CMessage::get_Body(BSTR *pVal)
{
	ATLTRACE("Get Body");
	return m_sMessage.m_Body.CopyTo(pVal);
}

STDMETHODIMP CMessage::put_Body(BSTR newVal)
{
	ATLTRACE("Put Body");
  m_sMessage.m_Body= newVal;
  return (m_sMessage.m_Body || !newVal ? S_OK : E_OUTOFMEMORY);
}

STDMETHODIMP CMessage::get_date_time(long *pVal)
{
	if(pVal == NULL)
	return E_POINTER;

	*pVal = m_sMessage.m_Time;
	return S_OK;
}

STDMETHODIMP CMessage::get_Sender(IUser* *pVal)
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
	m_pUser->m_sUser = m_sMessage.m_Sender;

	*pVal = pUser;
	return S_OK;
}

STDMETHODIMP CMessage::get_Recipients(IUsers **pVal)
{
	return m_pRecipients.CopyTo(pVal);
}


STDMETHODIMP CMessage::get_MID(BSTR *pVal)
{
	return m_sMessage.m_MID.CopyTo(pVal);
}

STDMETHODIMP CMessage::put_MID(BSTR newVal)
{
	ATLTRACE("Put MID");
  m_sMessage.m_MID.Empty();
  m_sMessage.m_MID = newVal;
  return (m_sMessage.m_MID || !newVal ? S_OK : E_OUTOFMEMORY);
	
}

STDMETHODIMP CMessage::Send(long *Handle)
{
	ATLTRACE("Send Begin");
	if(Handle == NULL)
		return E_POINTER;
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;
	try
	{ 
		ATLTRACE("Send Befor Add");
		m_pSession->m_IM_NET->m_CommandQueue.x_Message(this,*Handle);
		ATLTRACE("Send Befor Send");
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

STDMETHODIMP CMessage::get_SID(BSTR *pVal)
{
	return m_sMessage.m_SID.CopyTo(pVal);

}


STDMETHODIMP CMessage::get_ID(long *pVal)
{
	*pVal = m_sMessage.m_nMID;

	return S_OK;
}

#pragma warning(default: 4530)