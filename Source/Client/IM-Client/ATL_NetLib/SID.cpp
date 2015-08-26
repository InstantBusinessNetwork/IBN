// SID.cpp : Implementation of CSID
#include "stdafx.h"
#include "ATL_NetLib.h"
#include "SID.h"

/////////////////////////////////////////////////////////////////////////////
// CSID


STDMETHODIMP ClocalSID::get_Count(long *pVal)
{
	if(pVal == NULL)
		return E_POINTER;
	*pVal = m_slocalSID.m_Count;
	return S_OK;
}

STDMETHODIMP ClocalSID::get_SID(BSTR *pVal)
{
	return m_slocalSID.m_SID.CopyTo(pVal);
}
