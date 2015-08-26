// User.cpp : Implementation of CUser
#include "stdafx.h"
#include "ATL_NetLib.h"
#include "User.h"

/////////////////////////////////////////////////////////////////////////////
// CUser

STDMETHODIMP CUser::get_Value(BSTR bsName, VARIANT *pVal)
{
	if(CComBSTR(bsName) ==  CComBSTR(_T("nick_name")))
	{
		return CComVariant(m_sUser.m_UserName).Detach(pVal);
	}
	else
		if(CComBSTR(bsName) ==  CComBSTR(_T("first_name")))
		{
			return CComVariant(m_sUser.m_FirstName).Detach(pVal);
		}
		else
			if(CComBSTR(bsName) ==  CComBSTR(_T("last_name")))
			{
				return CComVariant(m_sUser.m_LastName).Detach(pVal);
			}
			else
				if(CComBSTR(bsName) ==  CComBSTR(_T("role/name")))
				{
					return CComVariant(m_sUser.m_Role).Detach(pVal);
				}
				else
					if(CComBSTR(bsName) ==  CComBSTR(_T("email")))
					{
						return CComVariant(m_sUser.m_EMail).Detach(pVal);
					}
					else
						if(CComBSTR(bsName) ==  CComBSTR(_T("status")))
						{
							return CComVariant(m_sUser.m_Status).Detach(pVal);
						}
						else
							if(CComBSTR(bsName) ==  CComBSTR(_T("@id")))
							{
								return CComVariant(m_sUser.m_ID).Detach(pVal);
							}
							else
								if(CComBSTR(bsName) ==  CComBSTR(_T("role/@id")))
								{
									return CComVariant(m_sUser.m_Role_ID).Detach(pVal);
								}
								else
									if(CComBSTR(bsName) ==  CComBSTR(_T("time")))
									{
										return CComVariant(m_sUser.m_time).Detach(pVal);
									}
									else
										return E_INVALIDARG;

	return S_OK;
}

STDMETHODIMP CUser::put_Value(BSTR bsName, VARIANT newVal)
{
	if(CComBSTR(bsName) ==  CComBSTR(_T("nick_name")))
	{
		if(newVal.vt != VT_BSTR) return E_INVALIDARG;
		m_sUser.m_UserName = CComBSTR(newVal.bstrVal);
	}
	else
		if(CComBSTR(bsName) ==  CComBSTR(_T("first_name")))
		{
			if(newVal.vt != VT_BSTR) return E_INVALIDARG;
			m_sUser.m_FirstName = CComBSTR(newVal.bstrVal);
		}
		else
			if(CComBSTR(bsName) ==  CComBSTR(_T("last_name")))
			{
				if(newVal.vt != VT_BSTR) return E_INVALIDARG;
				m_sUser.m_LastName = CComBSTR(newVal.bstrVal);
			}
			else
				if(CComBSTR(bsName) ==  CComBSTR(_T("role/name")))
				{
					if(newVal.vt != VT_BSTR) return E_INVALIDARG;
					m_sUser.m_Role = CComBSTR(newVal.bstrVal);
				}
				else
					if(CComBSTR(bsName) ==  CComBSTR(_T("email")))
					{
						if(newVal.vt != VT_BSTR) return E_INVALIDARG;
						m_sUser.m_EMail = CComBSTR(newVal.bstrVal);
					}
					else
						if(CComBSTR(bsName) ==  CComBSTR(_T("status")))
						{
							if(newVal.vt != VT_I4) return E_INVALIDARG;
							m_sUser.m_Status = newVal.lVal;
						}
						else
							if(CComBSTR(bsName) ==  CComBSTR(_T("@id")))
							{
								if(newVal.vt != VT_I4) return E_INVALIDARG;
								m_sUser.m_ID = newVal.lVal;
							}
							else
								if(CComBSTR(bsName) ==  CComBSTR(_T("role/@id")))
								{
									if(newVal.vt != VT_I4) return E_INVALIDARG;
									m_sUser.m_Role_ID = newVal.lVal;
								}
								else
									if(CComBSTR(bsName) ==  CComBSTR(_T("time")))
									{
										if(newVal.vt != VT_I4) return E_INVALIDARG;
										m_sUser.m_time = newVal.lVal;
									}
									else
										return E_INVALIDARG;
								
	return S_OK;
}
