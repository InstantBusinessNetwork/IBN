// FileDownloader.cpp: implementation of the CFileDownloader class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ofstv.h"
#include "FileDownloader.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFileDownloader::CFileDownloader()
{
	m_strUrl		=	_T("");
	m_MessageWnd	=	0;
	m_RetMessage	=	0;
	m_hWorkTread	=	NULL;
	m_dwWorkTreadId	=	0;
	m_Data			=	NULL;	
}

CFileDownloader::~CFileDownloader()
{
	if(m_Data)
	{
		m_Data->Release();
		m_Data = NULL;
	}
}

//////////////////////////////////////////////////////////////////////////
BOOL CFileDownloader::Init(HWND hWnd, UINT msg)
{
	m_MessageWnd = hWnd;
	m_RetMessage = msg;
	return TRUE;
}

BOOL CFileDownloader::Load(LPCTSTR szUrl)
{
	if(IsDownload())
		return FALSE;

	if(szUrl!=NULL)
		m_strUrl = szUrl;

	if(m_strUrl.IsEmpty())
		return FALSE;


	m_hWorkTread = CreateThread(NULL,0,trWorkThread,(LPVOID)this,0,&m_dwWorkTreadId);
	
	return IsDownload();
}

BOOL CFileDownloader::Save(LPCTSTR szPath)
{
	if(!m_Data)
		return FALSE;

	HANDLE hFile = CreateFile(szPath,GENERIC_WRITE,FILE_SHARE_READ,NULL,CREATE_ALWAYS,FILE_ATTRIBUTE_NORMAL,NULL);

	if(hFile==INVALID_HANDLE_VALUE)
		return FALSE;

	LARGE_INTEGER	li = {0,0};
	m_Data->Seek(li,STREAM_SEEK_SET ,NULL);

	const ULONG	BufferSize	=	60000;

	BYTE	Buffer[BufferSize];
	DWORD	dwRealReadToBuffer	=	0;

	do 
	{
		if(SUCCEEDED(m_Data->Read(Buffer,BufferSize,&dwRealReadToBuffer)))
			WriteFile(hFile,Buffer,dwRealReadToBuffer,&dwRealReadToBuffer,NULL);
	} 
	while(BufferSize==dwRealReadToBuffer);

	CloseHandle(hFile);

	return TRUE;
}


DWORD CFileDownloader::trWorkThread(LPVOID lParam)
{
	((CFileDownloader*)lParam)->DownloadBody();
	return 0;
}

BOOL CFileDownloader::IsDownload()
{
	if(m_hWorkTread)
		return TRUE;
	else
		return FALSE;
}

void CFileDownloader::DownloadBody()
{
	if(m_Data)
	{
		m_Data->Release();
		m_Data = NULL;
	}

	HRESULT ErrorCode = m_HTTPDownloader.Load(m_strUrl,&m_Data);

	CloseHandle(m_hWorkTread);
	m_hWorkTread = NULL;

	PostMessage(m_MessageWnd,m_RetMessage,ErrorCode,NULL);
	
}

BOOL CFileDownloader::StopDownload(DWORD WaitTime)
{
	if(IsDownload())
	{
		m_HTTPDownloader.Abort();
		DWORD dwExitCode = WaitForSingleObject(m_hWorkTread,WaitTime);
		if(dwExitCode==WAIT_TIMEOUT)
		{
			TerminateThread(m_hWorkTread,2);
			CloseHandle(m_hWorkTread);
			m_hWorkTread = NULL;
		}
	}
	return TRUE;
}

BOOL CFileDownloader::Clear()
{
	if(IsDownload())
		return FALSE;

	if(m_Data)
	{
		m_Data->Release();
		m_Data = NULL;
	}
	return TRUE;
}
