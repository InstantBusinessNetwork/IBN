

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0366 */
/* at Tue Aug 19 11:28:26 2008
 */
/* Compiler settings for .\McConverterSessionServer.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __McConverterSessionServer_h__
#define __McConverterSessionServer_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IConvertSessionWrapper_FWD_DEFINED__
#define __IConvertSessionWrapper_FWD_DEFINED__
typedef interface IConvertSessionWrapper IConvertSessionWrapper;
#endif 	/* __IConvertSessionWrapper_FWD_DEFINED__ */


#ifndef __ConvertSessionWrapper_FWD_DEFINED__
#define __ConvertSessionWrapper_FWD_DEFINED__

#ifdef __cplusplus
typedef class ConvertSessionWrapper ConvertSessionWrapper;
#else
typedef struct ConvertSessionWrapper ConvertSessionWrapper;
#endif /* __cplusplus */

#endif 	/* __ConvertSessionWrapper_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

#ifndef __IConvertSessionWrapper_INTERFACE_DEFINED__
#define __IConvertSessionWrapper_INTERFACE_DEFINED__

/* interface IConvertSessionWrapper */
/* [unique][helpstring][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IConvertSessionWrapper;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E8960C15-6EE6-492D-B074-E233652753BB")
    IConvertSessionWrapper : public IDispatch
    {
    public:
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SaveMapiMessage( 
            /* [in] */ BSTR bstrFileName,
            /* [in] */ VARIANT pUnkMessage,
            /* [in] */ long ulEncType) = 0;
        
        virtual /* [helpstring][id] */ HRESULT STDMETHODCALLTYPE SaveMapiMessageTmpFile( 
            /* [in] */ VARIANT pUnkMessage,
            /* [in] */ long ulEncType,
            /* [retval][out] */ BSTR *tmpFileName) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IConvertSessionWrapperVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IConvertSessionWrapper * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IConvertSessionWrapper * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IConvertSessionWrapper * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IConvertSessionWrapper * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IConvertSessionWrapper * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IConvertSessionWrapper * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IConvertSessionWrapper * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SaveMapiMessage )( 
            IConvertSessionWrapper * This,
            /* [in] */ BSTR bstrFileName,
            /* [in] */ VARIANT pUnkMessage,
            /* [in] */ long ulEncType);
        
        /* [helpstring][id] */ HRESULT ( STDMETHODCALLTYPE *SaveMapiMessageTmpFile )( 
            IConvertSessionWrapper * This,
            /* [in] */ VARIANT pUnkMessage,
            /* [in] */ long ulEncType,
            /* [retval][out] */ BSTR *tmpFileName);
        
        END_INTERFACE
    } IConvertSessionWrapperVtbl;

    interface IConvertSessionWrapper
    {
        CONST_VTBL struct IConvertSessionWrapperVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IConvertSessionWrapper_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define IConvertSessionWrapper_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define IConvertSessionWrapper_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define IConvertSessionWrapper_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define IConvertSessionWrapper_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define IConvertSessionWrapper_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define IConvertSessionWrapper_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define IConvertSessionWrapper_SaveMapiMessage(This,bstrFileName,pUnkMessage,ulEncType)	\
    (This)->lpVtbl -> SaveMapiMessage(This,bstrFileName,pUnkMessage,ulEncType)

#define IConvertSessionWrapper_SaveMapiMessageTmpFile(This,pUnkMessage,ulEncType,tmpFileName)	\
    (This)->lpVtbl -> SaveMapiMessageTmpFile(This,pUnkMessage,ulEncType,tmpFileName)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE IConvertSessionWrapper_SaveMapiMessage_Proxy( 
    IConvertSessionWrapper * This,
    /* [in] */ BSTR bstrFileName,
    /* [in] */ VARIANT pUnkMessage,
    /* [in] */ long ulEncType);


void __RPC_STUB IConvertSessionWrapper_SaveMapiMessage_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring][id] */ HRESULT STDMETHODCALLTYPE IConvertSessionWrapper_SaveMapiMessageTmpFile_Proxy( 
    IConvertSessionWrapper * This,
    /* [in] */ VARIANT pUnkMessage,
    /* [in] */ long ulEncType,
    /* [retval][out] */ BSTR *tmpFileName);


void __RPC_STUB IConvertSessionWrapper_SaveMapiMessageTmpFile_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __IConvertSessionWrapper_INTERFACE_DEFINED__ */



#ifndef __McConverterSessionServerLib_LIBRARY_DEFINED__
#define __McConverterSessionServerLib_LIBRARY_DEFINED__

/* library McConverterSessionServerLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_McConverterSessionServerLib;

EXTERN_C const CLSID CLSID_ConvertSessionWrapper;

#ifdef __cplusplus

class DECLSPEC_UUID("1B2174BD-86E7-4DEE-81BE-46FF3B70960A")
ConvertSessionWrapper;
#endif
#endif /* __McConverterSessionServerLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


