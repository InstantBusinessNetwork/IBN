// CoolInet2.h: interface for the CCoolInet2 class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_COOLINET2_H__390CD6E9_40FB_42B9_A652_9A965A8737B7__INCLUDED_)
#define AFX_COOLINET2_H__390CD6E9_40FB_42B9_A652_9A965A8737B7__INCLUDED_

#include "EventContainer.h"

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
//////////////////////////////////////////////////////////////////////
// Класс Собирающей в себе : Транслятор, Событер - Диспитер, 
// и предостовляющий ISession* 
//////////////////////////////////////////////////////////////////////
bstr_t GUIDGen();

class CCoolInet2  
{
public:
	ISession* GetSession();
	BOOL SessionMarshaling(LPSTREAM* ppStm);
	BOOL WorkSession();
	CCoolInet2();
	virtual ~CCoolInet2();
	/// Команды косвенного обращения к Translatoru
	void InitEventWindow(HWND hEventWnd);
    void LockTranslator();
	void AddToTranslator(LONG Handle,HWND hReturnToWnd);
	void RemoveFromTranslator(LONG &Handle);
	void UnlockTranslator();
protected:
	DWORD     IDWorkThread;
	HANDLE    hWorkThread;
    IStream   **ppSession;
	BOOL      bGetMarshaling;
protected:
	HANDLE hExitWork;
	HANDLE hExitEvent;
	HANDLE hStartEvent;
	HANDLE hMarshalingEndEvent;
	HANDLE MarshalEvent;
	CRITICAL_SECTION m_lock;
protected:
	ISession             *p_InetSession; 
protected:	
	CEventContainer       m_InetSessionEvent;
protected:	
	void CloseSession();
	void CreateSession();
	void CloseEventContainer();
	void InitEventContainer();
	static DWORD WINAPI WorkThread(LPVOID pParam);
//	IConnectionPointPtr   m_pSessionConnectionPoint;
	DWORD                 dwSessionCookie;
    
};

#endif // !defined(AFX_COOLINET2_H__390CD6E9_40FB_42B9_A652_9A965A8737B7__INCLUDED_)
