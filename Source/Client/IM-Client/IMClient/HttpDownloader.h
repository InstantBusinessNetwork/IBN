// HttpDownloader.h: interface for the CHttpDownloader class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_HTTPDOWNLOADER_H__FECDAEE1_D128_4C33_BD01_0257769458FF__INCLUDED_)
#define AFX_HTTPDOWNLOADER_H__FECDAEE1_D128_4C33_BD01_0257769458FF__INCLUDED_

#include <wininet.h>
#include <comdef.h>

#define COMMAND_BUFF_SIZE_PART  1024

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

typedef enum
{
	HTTP_DOWNLOADER_OP_IDLE = 0,
	HTTP_DOWNLOADER_OP_CONNECT,
	HTTP_DOWNLOADER_OP_OPEN_REQUEST,
	HTTP_DOWNLOADER_OP_SEND_REQUEST,
	HTTP_DOWNLOADER_OP_READ_DATA,
	HTTP_DOWNLOADER_OP_GET_STATUS
} HTTP_DOWNLOADER_OPERATION;

class CHttpDownloader  
{
public:
	CHttpDownloader();
	~CHttpDownloader();

	struct HttpDownloaderContext
	{
		HTTP_DOWNLOADER_OPERATION op;
		HANDLE hEvent;
		DWORD dwError;	// Set in internet callback func
	} m_context;
	
protected:
	BOOL WaitForComplete(DWORD dwMilliseconds);
	static void __stdcall CallbackFunc(HINTERNET hInternet, DWORD dwContext, DWORD dwInternetStatus, LPVOID lpvStatusInformation, DWORD dwStatusInformationLength);
	HANDLE m_hEvent;
	void Clear();
	DWORD m_dwTotalSize;
	DWORD m_dwDownloaded;
	HWND m_hWnd;	// Window handle for progress notification
	UINT m_nMessage;	// Message for progress notification. WPARAM = Downloaded, LPARAM = Total Size
	
	struct tagRequestInfo
	{
		CRITICAL_SECTION *pCritSect;
		DWORD port;
		_bstr_t server;
		_bstr_t url;
		LPCTSTR md5;
		BOOL	bUseSSL;
		
		tagRequestInfo()
		{
			port = 0;
			pCritSect = NULL;
			md5 = NULL;
			bUseSSL = FALSE;
		}
	} m_request;

//	static HttpDownloaderContext m_context;
	
	HINTERNET m_hInternet;
	HINTERNET m_hConnect;
	HINTERNET m_hRequest;
	
	long m_longAbort;
	IStream *m_pStream;
	
	HRESULT WorkFunction();
	HRESULT ParseUrl(LPCTSTR szUrlIn);
	HRESULT ReadData(_bstr_t &strBuffer);
	HRESULT ConnectToServer(_bstr_t &strBuffer);
	DWORD GetHttpStatus();
	public:
		DWORD m_ProxyType;
		CString m_ProxyName;

		DWORD m_dwConnectRetryCount;
		DWORD m_dwTimeout;
		void Abort();
		void EnableProgress(HWND hWnd, UINT nMessage);
		HRESULT Load(LPCTSTR szUrl, IStream **ppStream, LPCTSTR szMD5 = NULL);
};

#endif // !defined(AFX_HTTPDOWNLOADER_H__FECDAEE1_D128_4C33_BD01_0257769458FF__INCLUDED_)
