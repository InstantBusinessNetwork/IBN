//=--------------------------------------------------------------------------=
//  (C) Copyright 1997-1998 Microsoft Corporation. All Rights Reserved.
//=--------------------------------------------------------------------------=
// Implementation of the ambient properties IDispatch on a
// control site.

#include "stdafx.h"
#include <docobj.h>
#include "site.h"
#include "OfsDhtmlCtrl.h"
//#include "CEdit.h"


/*
 * CImpAmbientIDispatch::CImpAmbientIDispatch
 * CImpAmbientIDispatch::~CImpAmbientIDispatch
 *
 * Parameters (Constructor):
 *  pSite           PCSite of the site we're in.
 *  pUnkOuter       LPUNKNOWN to which we delegate.
 */

CImpAmbientIDispatch::CImpAmbientIDispatch( PCSite pSite, LPUNKNOWN pUnkOuter)
{
    m_cRef = 0;
    m_pSite = pSite;
    m_pUnkOuter = pUnkOuter;
    return;
}

CImpAmbientIDispatch::~CImpAmbientIDispatch(void)
{
    return;
}

/*
 * CImpAmbientIDispatch::QueryInterface
 * CImpAmbientIDispatch::AddRef
 * CImpAmbientIDispatch::Release
 */

STDMETHODIMP CImpAmbientIDispatch::QueryInterface(REFIID riid, void** ppv)
{
    return m_pUnkOuter->QueryInterface(riid, ppv);
}

STDMETHODIMP_(ULONG) CImpAmbientIDispatch::AddRef(void)
{
    ++m_cRef;
    return m_pUnkOuter->AddRef();
}

STDMETHODIMP_(ULONG) CImpAmbientIDispatch::Release(void)
{
    m_cRef--;
    return m_pUnkOuter->Release();
}


/*
 * CImpAmbientIDispatch::GetTypeInfoCount
 * CImpAmbientIDispatch::GetTypeInfo
 * CImpAmbientIDispatch::GetIDsOfNames
 *
 * Unimplemented members, not needed for ambient properties.
 */

STDMETHODIMP CImpAmbientIDispatch::GetTypeInfoCount(UINT *pctInfo)
{
    *pctInfo=0;
    return NOERROR;
}

STDMETHODIMP CImpAmbientIDispatch::GetTypeInfo(UINT itinfo
    , LCID lcid, ITypeInfo **pptInfo)
{
    *pptInfo=NULL;
    return ResultFromScode(E_NOTIMPL);
}

STDMETHODIMP CImpAmbientIDispatch::GetIDsOfNames(REFIID riid
    , OLECHAR **rgszNames, UINT cNames, LCID lcid, DISPID *rgDispID)
{
    *rgszNames=NULL;
    *rgDispID=NULL;
    return ResultFromScode(E_NOTIMPL);
}


/*
 * CImpAmbientIDispatch::Invoke
 *
 * Purpose:
 *  Calls a method in the dispatch interface or manipulates a
 *  property.
 *
 * Parameters:
 *  dispIDMember    DISPID of the method or property of interest.
 *  riid            REFIID reserved, must be NULL.
 *  lcid            LCID of the locale.
 *  wFlags          USHORT describing the context of the invocation.
 *  pDispParams     DISPPARAMS * to the array of arguments.
 *  pVarResult      VARIANT * in which to store the result.  Is
 *                  NULL if the caller is not interested.
 *  pExcepInfo      EXCEPINFO * to exception information.
 *  puArgErr        UINT * in which to store the index of an
 *                  invalid parameter if DISP_E_TYPEMISMATCH
 *                  is returned.
 *
 * Return Value:
 *  HRESULT         NOERROR or a general error code.
 */


STDMETHODIMP CImpAmbientIDispatch::Invoke(DISPID dispIDMember, REFIID riid
    , LCID lcid, unsigned short wFlags, DISPPARAMS *pDispParams
    , VARIANT *pVarResult, EXCEPINFO *pExcepInfo, UINT *puArgErr)
{
    HRESULT     hr;
    VARIANT     varResult;

    if (IID_NULL!=riid)
        return ResultFromScode(E_INVALIDARG);

    if(NULL==pVarResult)
      pVarResult=&varResult;

    VariantInit(pVarResult);

    //The most common case is boolean, use as an initial type
    V_VT(pVarResult)=VT_BOOL;

    /*
     * Process the requested ambient property.  Anything but a
     * request for a property is invalid, so we can check that
     * before looking at the specific ID.  We can only get away
     * with this because all properties are read-only.
     */

    if (!(DISPATCH_PROPERTYGET & wFlags))
        return ResultFromScode(DISP_E_MEMBERNOTFOUND);

    hr=NOERROR;

    switch (dispIDMember)
	{
        case DISPID_AMBIENT_USERMODE:
            V_BOOL(pVarResult)= VARIANT_FALSE;
            break;

        default:
            hr=ResultFromScode(DISP_E_MEMBERNOTFOUND);
            break;
	}

    return hr;
}
