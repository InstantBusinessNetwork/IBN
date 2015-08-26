// ATL_NetLib.cpp : Implementation of DLL Exports.


// Note: Proxy/Stub Information
//      To merge the proxy/stub code into the object DLL, add the file 
//      dlldatax.c to the project.  Make sure precompiled headers 
//      are turned off for this file, and add _MERGE_PROXYSTUB to the 
//      defines for the project.  
//
//      If you are not running WinNT4.0 or Win95 with DCOM, then you
//      need to remove the following define from dlldatax.c
//      #define _WIN32_WINNT 0x0400
//
//      Further, if you are running MIDL without /Oicf switch, you also 
//      need to remove the following define from dlldatax.c.
//      #define USE_STUBLESS_PROXY
//
//      Modify the custom build rule for ATL_NetLib.idl by adding the following 
//      files to the Outputs.
//          ATL_NetLib_p.c
//          dlldata.c
//      To build a separate proxy/stub DLL, 
//      run nmake -f ATL_NetLibps.mk in the project directory.

#include "stdafx.h"
#include "resource.h"
#include <initguid.h>
#include "ATL_NetLib.h"
//#include "dlldatax.h"

#include "ATL_NetLib_i.c"
#include "Session.h"
#include "Users.h"
//#include "CallBackEvents.h"
#include "Promo.h"
#include "Message.h"
#include "File.h"
#include "Messages.h"
#include "Files.h"
#include "SIDs.h"
#include "Promos.h"
#include "SID.h"
#include "IChat.h"
#include "IChats.h"

#ifdef _MERGE_PROXYSTUB
extern "C" HINSTANCE hProxyDll;
#endif

CComModule _Module;

BEGIN_OBJECT_MAP(ObjectMap)
OBJECT_ENTRY(CLSID_Session, CSession)

OBJECT_ENTRY_NON_CREATEABLE(CUser)
OBJECT_ENTRY_NON_CREATEABLE(CUsers)

OBJECT_ENTRY_NON_CREATEABLE(CPromo)
OBJECT_ENTRY_NON_CREATEABLE(CPromos)

OBJECT_ENTRY_NON_CREATEABLE(CMessage)
OBJECT_ENTRY_NON_CREATEABLE(CMessages)

OBJECT_ENTRY_NON_CREATEABLE(CFile)
OBJECT_ENTRY_NON_CREATEABLE(CFiles)

OBJECT_ENTRY_NON_CREATEABLE(ClocalSID)
OBJECT_ENTRY_NON_CREATEABLE(ClocalSIDs)
OBJECT_ENTRY_NON_CREATEABLE(CChat)
OBJECT_ENTRY_NON_CREATEABLE(CChats)
END_OBJECT_MAP()

/////////////////////////////////////////////////////////////////////////////
// DLL Entry Point

extern "C"
BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
    lpReserved;
#ifdef _MERGE_PROXYSTUB
    if (!PrxDllMain(hInstance, dwReason, lpReserved))
//		MCTRACE("WINAPI DllMain(HINSTANCE hInstance,fasle end");
        return FALSE;
#endif
    if (dwReason == DLL_PROCESS_ATTACH)
    {
		//MCTRACE("WINAPI DllMain(HINSTANCE hInstance,");
#ifdef _DEBUG
		// Get current flag
		int tmpFlag = _CrtSetDbgFlag( _CRTDBG_REPORT_FLAG );
		
		// Turn on leak-checking bit
		tmpFlag |= _CRTDBG_LEAK_CHECK_DF;
		
		// Turn off CRT block checking bit
		tmpFlag &= ~_CRTDBG_CHECK_CRT_DF;
		
		// Set flag to the new value
		_CrtSetDbgFlag( tmpFlag );
#endif
        _Module.Init(ObjectMap, hInstance, &LIBID_ATL_NETLIBLib);
        DisableThreadLibraryCalls(hInstance);
    }
    else if (dwReason == DLL_PROCESS_DETACH)
        _Module.Term();
//	MCTRACE("WINAPI DllMain(HINSTANCE hInstance,end");
    return TRUE;    // ok
}

/////////////////////////////////////////////////////////////////////////////
// Used to determine whether the DLL can be unloaded by OLE

STDAPI DllCanUnloadNow(void)
{
#ifdef _MERGE_PROXYSTUB
    if (PrxDllCanUnloadNow() != S_OK)
        return S_FALSE;
#endif
    return (_Module.GetLockCount()==0) ? S_OK : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////////
// Returns a class factory to create an object of the requested type

STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
#ifdef _MERGE_PROXYSTUB
    if (PrxDllGetClassObject(rclsid, riid, ppv) == S_OK)
//		MCTRACE("DllGetClassObject 1");
        return S_OK;
#endif
//		MCTRACE("DllGetClassObject 2");
    return _Module.GetClassObject(rclsid, riid, ppv);
}

/////////////////////////////////////////////////////////////////////////////
// DllRegisterServer - Adds entries to the system registry

STDAPI DllRegisterServer(void)
{
#ifdef _MERGE_PROXYSTUB
    HRESULT hRes = PrxDllRegisterServer();
    if (FAILED(hRes))
        return hRes;
#endif
    // registers object, typelib and all interfaces in typelib
    return _Module.RegisterServer(TRUE);
}

/////////////////////////////////////////////////////////////////////////////
// DllUnregisterServer - Removes entries from the system registry

STDAPI DllUnregisterServer(void)
{
#ifdef _MERGE_PROXYSTUB
    PrxDllUnregisterServer();
#endif
    return _Module.UnregisterServer(TRUE);
}

long g_ByteSent = 0;
long g_ByteReceived = 0;
long g_LatestSent = 0;
long g_LatestReceived = 0;

long g_FileSent = 0;
long g_FileReceived = 0;

long g_MessageSend = 0;
long g_MessageReceveived = 0;

long g_ByteSentPerSec[200];
long g_ByteReceivedPerSec[200];

long g_forByteSentPerSec[200];
long g_forByteReceivedPerSec[200];

CComAutoCriticalSection SendLock;
CComAutoCriticalSection ReceivedLock;

void g_AddSentBytes(long bytes)
{
	InterlockedExchangeAdd(&g_ByteSent,bytes);

	long sec = GetTickCount() / 1000;
	long Point = sec % 200;
	long prevPoint = g_LatestSent % 200;

	SendLock.Lock();
	if(sec - g_LatestSent >= 200)
	{
		memset(g_ByteSentPerSec,0,200*4);

		g_ByteSentPerSec[Point] = bytes;
	}
	else
	if(Point == prevPoint)
	{
		g_ByteSentPerSec[Point] += bytes;
	}
	else
	if(Point > prevPoint)
	{
		for(long k = prevPoint +1; k< Point; k++)
		g_ByteSentPerSec[k] = 0;
		
		g_ByteSentPerSec[Point] = bytes;
	}
	else
	{
		for(long k = 0; k< Point; k++)
		g_ByteSentPerSec[k] = 0;

		for(long k = prevPoint+ 1; k < 200; k++)
		g_ByteSentPerSec[k] = 0;
		
		g_ByteSentPerSec[Point] = bytes;
	}

	g_LatestSent = sec;
	SendLock.Unlock();
}


void g_AddReceivedBytes(long bytes)
{
	InterlockedExchangeAdd(&g_ByteReceived,bytes);

	long sec = GetTickCount() / 1000;
	long Point = sec % 200;
	long prevPoint = g_LatestReceived % 200;


	ReceivedLock.Lock();
	if(sec - g_LatestReceived >= 200)
	{
		memset(g_ByteReceivedPerSec,0,200*4);

		g_ByteReceivedPerSec[Point] = bytes;
	}
	else
	if(Point == prevPoint)
	{
		g_ByteReceivedPerSec[Point] += bytes;
	}
	else
	if(Point > prevPoint)
	{
		for(long k = prevPoint +1; k< Point; k++)
		g_ByteReceivedPerSec[k] = 0;
		
		g_ByteReceivedPerSec[Point] = bytes;
	}
	else
	{
		for(long k = 0; k< Point; k++)
		g_ByteReceivedPerSec[k] = 0;

		for(long k = prevPoint+ 1; k < 200; k++)
		g_ByteReceivedPerSec[k] = 0;
		
		g_ByteReceivedPerSec[Point] = bytes;
	}

	g_LatestReceived = sec;

	ReceivedLock.Unlock();
}