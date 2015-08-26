// MeTranslator.h: interface for the CMeTranslator class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_METRANSLATOR_H__1A51B4DB_3041_4B7F_962A_B669BB3906A0__INCLUDED_)
#define AFX_METRANSLATOR_H__1A51B4DB_3041_4B7F_962A_B669BB3906A0__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <afxtempl.h>

enum NTL_Event_ID /// тип События от которого содержатся данные
{
	NTL_ENone = 0L,
	NTL_EChangeState,
	NTL_EChangedStatus,
	NTL_EMessage,
	NTL_EPromo,
	NTL_EFile,
	NTL_EAdd,
	NTL_EAddR,
	NTL_EReklama,
	NTL_EContactList,
	NTL_EIgnoryList,
	NLT_EDetails,
	NLT_ECommandOK,
	NLT_ECommandError,
	NLT_ESessionsList,
	NLT_EOffLineFiles,
	NLT_EMessagesList,
	NLT_EPromosList,
	NLT_ESelfStatus,
	NLT_ESysMess,
	NLT_EChatList,
	NLT_EChatStatus,
	NLT_EChatCreate,
	NLT_EChatUserStatus,
	NLT_EChatInvite,
	NLT_EChatLeave,
	NLT_EChatMessage,
	NLT_EChatFile,
	NLT_EChatAccept
};
///// Ошибки NLT
const HRESULT NLT_OK                    =    0L;
const HRESULT NLT_CONTAINER_ERROR       =   -1L;
const HRESULT NLT_EVENT_NO_SUPPORT      =   -2L;

///// Событие 
const DWORD WM_NLT_CONTAINER_EVENT = WM_USER + 0x0301;

/// Структура Хранилище Данных
struct NLT_Container
{
	NLT_Container():Handel(0), pMarshalStream(NULL),pMarshalStream2(NULL),pMarshalStream3(NULL),
		Long1(0),Long2(0),Long3(0),String1(L"")
	{
		EventType = NTL_ENone;
		UnMarshalEvent = CreateEvent(NULL,FALSE,FALSE,NULL);
	};
	~NLT_Container()
	{
		CloseHandle(UnMarshalEvent);
		if(pMarshalStream!=NULL)
		{
			pMarshalStream->Release();
			pMarshalStream = NULL;
		}
		if(pMarshalStream2!=NULL)
		{
			pMarshalStream2->Release();
			pMarshalStream2 = NULL;
		}
		if(pMarshalStream3!=NULL)
		{
			pMarshalStream3->Release();
			pMarshalStream3 = NULL;
		}
	};
	BOOL Send(HWND hWnd)
	{
		BOOL bSend = ::PostMessage(hWnd,WM_NLT_CONTAINER_EVENT,(WPARAM)this,NULL);
//		if(bSend&&pMarshalStream!=NULL)
//		{
//			WaitForSingleObject(UnMarshalEvent,INFINITE);
//		}
		return bSend;
	};


	/// Уже автоматически Увеличивает Счетчик, в потоке где будет
	/// использоваться этот объект остается лишь освободить
	HRESULT Marchaling(REFIID riid, LPUNKNOWN pUnk)
	{
		// 10.04.2002 added by Artyom
		if(pUnk == NULL)
			return E_POINTER;
		// end
		
		HRESULT hr = CoMarshalInterThreadInterfaceInStream(riid,pUnk,&pMarshalStream);
		ASSERT(SUCCEEDED(hr));
		return hr;
	};
	
	HRESULT UnMarchaling(REFIID riid,LPUNKNOWN *pUnk)
	{
		HRESULT hr = CoGetInterfaceAndReleaseStream(pMarshalStream,riid,(void**)pUnk);
//		ASSERT(SUCCEEDED(hr));
		if(SUCCEEDED(hr))
		{
			pMarshalStream = NULL;
		}
		
		return hr;
	};

	HRESULT Marchaling2(REFIID riid, LPUNKNOWN pUnk)
	{
		// 10.04.2002 added by Artyom
		if(pUnk == NULL)
			return E_POINTER;
		// end
		
		HRESULT hr = CoMarshalInterThreadInterfaceInStream(riid,pUnk,&pMarshalStream2);
		ASSERT(SUCCEEDED(hr));
		return hr;
	};
	
	HRESULT UnMarchaling2(REFIID riid,LPUNKNOWN *pUnk)
	{
		HRESULT hr = CoGetInterfaceAndReleaseStream(pMarshalStream2,riid,(void**)pUnk);
//		ASSERT(SUCCEEDED(hr));
		if(SUCCEEDED(hr))
		{
			pMarshalStream2 = NULL;
		}
		
		return hr;
	};

	HRESULT Marchaling3(REFIID riid, LPUNKNOWN pUnk)
	{
		// 10.04.2002 added by Artyom
		if(pUnk == NULL)
			return E_POINTER;
		// end
		
		HRESULT hr = CoMarshalInterThreadInterfaceInStream(riid,pUnk,&pMarshalStream3);
		ASSERT(SUCCEEDED(hr));
		return hr;
	};
	
	HRESULT UnMarchaling3(REFIID riid,LPUNKNOWN *pUnk)
	{
		HRESULT hr = CoGetInterfaceAndReleaseStream(pMarshalStream3,riid,(void**)pUnk);
//		ASSERT(SUCCEEDED(hr));
		if(SUCCEEDED(hr))
		{
			pMarshalStream3 = NULL;
		}
		
		return hr;
	};

	LONG          Handel;
	NTL_Event_ID  EventType;
	IStream      *pMarshalStream;
	IStream      *pMarshalStream2;
	IStream      *pMarshalStream3;
	LONG          Long1;
	LONG          Long2;
	LONG          Long3;
	_bstr_t       String1;
protected:
	HANDLE        UnMarshalEvent;
};

/// Хранилище Окон куда перенаправляются события 
/// При многопотоковой работе необходимо использовать
/// Lock и Unlock.
class CNetLibTranslator  
{
public:
	struct Translator_Item
	{
		Translator_Item():hReturnToWnd(NULL) 
		{};
		HWND   hReturnToWnd;
	};
private:
	HWND       hEventWnd;
	/// 1. Лист Сообщений.
	CMap <LONG,LONG,Translator_Item*,Translator_Item*> NLT_Map;
	CRITICAL_SECTION m_lock;
public:
	void  Lock();
	void  UnLock();
	void  NLT_SetEventWindow(HWND hWnd);
	HWND  NLT_GetWindow(LONG Handle);   /// only fot CEventContainer ...
	HWND  NLT_GetEventWindow();
	DWORD NLT_AddToTranslate(LONG Handle, HWND hReturnToWnd);
	DWORD NLT_Remove(LONG Handle);
	
	CNetLibTranslator();
	virtual ~CNetLibTranslator();
	
};

#endif