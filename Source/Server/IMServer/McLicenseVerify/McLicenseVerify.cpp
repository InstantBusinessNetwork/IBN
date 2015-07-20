// McLicenseVerify.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include <tchar.h>
#include <atlenc.h>
#include ".\mcactivation.h"
#include ".\mclicenseverify.h"

const LPCTSTR sLicense = _T("License");

HRESULT LoadDataFile(
	IN LPCTSTR szFileName,
	OUT LPBYTE pData,
	IN OUT DWORD* pcbData,
	OUT LPBOOL pFileExists)
{
	HRESULT hr = E_FAIL;
	if(pFileExists != NULL)
		*pFileExists = FALSE;

	if(pcbData != NULL)
	{
		HANDLE hFile = CreateFile(szFileName, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
		if(hFile == INVALID_HANDLE_VALUE)
			hr = HRESULT_FROM_WIN32(::GetLastError());
		else
		{
			if(pFileExists != NULL)
				*pFileExists = TRUE;

			DWORD size = GetFileSize(hFile, NULL);
			if(pData != NULL)
			{
				if(size <= *pcbData)
				{
					DWORD dwToRead = *pcbData, dwRead;
					if(ReadFile(hFile, pData, dwToRead, &dwRead, NULL))
					{
						*pcbData = dwRead;
						hr = S_OK;
					}
				}
			}
			else
			{
				*pcbData = size;
				hr = S_OK;
			}
			CloseHandle(hFile);
		}
	}
	return hr;
}

HRESULT ParseLicenseFile(LPBYTE pFile, DWORD cbFile, LPBYTE* ppData, LPDWORD pcbData, LPBYTE* ppHeader, LPDWORD pcbHeader, LPBYTE* ppXml, LPDWORD pcbXml)
{
	HRESULT hr;
	LPCSTR szLicensePacket = reinterpret_cast<LPCSTR>(pFile);
	int nSrcLen = static_cast<int>(cbFile);
	int nDestLen = Base64DecodeGetRequiredLength(nSrcLen);
	LPBYTE pbDest = new BYTE[nDestLen];
	if(pbDest == NULL)
		hr = E_OUTOFMEMORY;
	else
	{
		hr = E_FAIL;
		if(Base64Decode(szLicensePacket, nSrcLen, pbDest, &nDestLen))
		{
			DWORD dwDataSize = static_cast<DWORD>(nDestLen);
			if(nDestLen >= sizeof(McActivatorPacket))
			{
				PMcActivatorPacket pPacket = reinterpret_cast<PMcActivatorPacket>(pbDest);
				if(pPacket->cbSize == sizeof(McActivatorPacket)
					&& pPacket->dwHeaderOffset + pPacket->dwHeaderSize <= dwDataSize
					&& pPacket->dwXmlOffset + pPacket->dwXmlSize <= dwDataSize)
				{
					*ppData = pbDest;
					*pcbData = dwDataSize;
					*ppHeader = pbDest + pPacket->cbSize + pPacket->dwHeaderOffset;
					*pcbHeader = pPacket->dwHeaderSize;
					*ppXml = pbDest + pPacket->cbSize + pPacket->dwXmlOffset;
					*pcbXml = pPacket->dwXmlSize;
					pbDest = NULL;
					hr = S_OK;
				}
			}
		}
		delete[] pbDest;
	}
	return hr;
}

HRESULT GetDllDir(LPTSTR* pszPath)
{
	HRESULT hr = S_OK;
	LPTSTR sz = NULL;

	HMODULE hModule = LoadLibrary(_T("McLicenseVerify.dll"));
	if(hModule == NULL)
		hr = HRESULT_FROM_WIN32(GetLastError());
	else
	{
		DWORD dwBufLen = 0, dwStrLen = 0;
		while(dwBufLen == dwStrLen)
		{
			delete[] sz;
			dwBufLen += MAX_PATH;
			sz = new TCHAR[dwBufLen];
			if(sz == NULL)
			{
				hr = E_OUTOFMEMORY;
				break;
			}
			else
			{
				dwStrLen = GetModuleFileName(hModule, sz, dwBufLen);
				if(dwStrLen == 0)
				{
					hr = HRESULT_FROM_WIN32(GetLastError());
					delete[] sz;
					sz = NULL;
					break;
				}
			}
		}
	}
	if(sz != NULL)
	{
		LPTSTR szTmp = _tcsrchr(sz, _T('\\'));
		if(szTmp != NULL)
			*szTmp = _T('\0');
	}
	*pszPath = sz;
	return hr;
}

HRESULT LoadLicenseFile(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	OUT LPGUID pProductGuid,
	OUT LPBYTE* ppData,
	OUT DWORD* pcbData,
	OUT LPBYTE* ppHeader,
	OUT DWORD* pcbHeader,
	OUT LPBYTE* ppXml,
	OUT DWORD* pcbXml,
	OUT LPBOOL pFileExists)
{
	HRESULT hr;
	GUID productGuid;
	DWORD cbFile, cbData, cbHeader, cbXml;
	LPBYTE pFile = NULL, pData = NULL, pHeader = NULL, pXml = NULL;

	hr = S2GUID(szProductGuid, &productGuid);
	if(SUCCEEDED(hr))
	{
		// Get license file size
		hr = LoadDataFile(szLicenseFile, pFile, &cbFile, pFileExists);
		if(SUCCEEDED(hr))
		{
			pFile = new BYTE[cbFile];
			if(pFile == NULL)
				hr = E_OUTOFMEMORY;
			else
			{
				// Load license file
				hr = LoadDataFile(szLicenseFile, pFile, &cbFile, pFileExists);
				if(SUCCEEDED(hr))
				{
					hr = ParseLicenseFile(pFile, cbFile, &pData, &cbData, &pHeader, &cbHeader, &pXml, &cbXml);
					if(SUCCEEDED(hr))
					{
						if(pProductGuid != NULL)
							memcpy(pProductGuid, &productGuid, sizeof(GUID));

						if(ppData != NULL)
						{
							*ppData = pData;
							pData = NULL;
						}
						else
						{
							pHeader = NULL;
							pXml = NULL;
						}
						if(ppHeader != NULL)
							*ppHeader = pHeader;
						if(ppXml != NULL)
							*ppXml = pXml;

						if(pcbData != NULL)
							*pcbData = cbFile;
						if(pcbHeader != NULL)
							*pcbHeader = cbHeader;
						if(pcbXml != NULL)
							*pcbXml = cbXml;
					}
				}
				delete[] pFile;
			}
			if(pData != NULL)
				delete[] pData;
		}
	}
	return hr;
}

HRESULT GetLicenseDataSizeFromRegistry(
	IN LPCTSTR szProductGuid,
	OUT DWORD* pcbLicenseData)
{
	McTRACE(_T("McLicenseVerify::GetLicenseDataSizeFromRegistry::Begin. szProductGuid: %s"), szProductGuid);

	HRESULT hr = E_FAIL;
	*pcbLicenseData = 0;

	LPTSTR szLicenseGuid = McRegGetString(HKEY_LOCAL_MACHINE, sProductsKeyName, szProductGuid, NULL);
	if(szLicenseGuid == NULL)
	{
		hr = HRESULT_FROM_WIN32(GetLastError());
		McTRACE(_T("McLicenseVerify::GetLicenseDataSizeFromRegistry::McRegGetString returned NULL."));
	}
	else
	{
		LPTSTR szLicenseFile = GetLicenseFileName(szLicenseGuid);
		if(szLicenseFile == NULL)
		{
			hr = HRESULT_FROM_WIN32(GetLastError());
			McTRACE(_T("McLicenseVerify::GetLicenseDataSizeFromRegistry::GetLicenseFileName returned NULL."));
		}
		else
		{
			hr = LoadDataFile(szLicenseFile, NULL, pcbLicenseData, NULL);
			McDelString(szLicenseFile);
		}
		McDelString(szLicenseGuid);
	}

	McTRACE(_T("McLicenseVerify::GetLicenseDataSizeFromRegistry::End. hr = %d"), hr);
	return hr;
}

HRESULT GetLicenseDataFromRegistry(
	IN LPCTSTR szProductGuid,
	IN LPCTSTR szContainer,
	OUT LPBYTE pLicenseData,
	IN OUT DWORD* pcbLicenseData
	, OUT DWORD* pExpirationDate
	)
{
	McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::Begin. szProductGuid: %s, szContainer: %s)"), szProductGuid, szContainer);

	int ret = ERROR_SUCCESS;
	HRESULT hr = E_FAIL;
	CString str;
	HKEY hKey;
	DWORD cbData;
	DWORD type;

	LPTSTR szLicenseGuid = McRegGetString(HKEY_LOCAL_MACHINE, sProductsKeyName, szProductGuid, NULL);

	if(szLicenseGuid == NULL)
	{
		hr = HRESULT_FROM_WIN32(GetLastError());
		McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::McRegGetString returned NULL. hr = %d"), hr);
	}
	else
	{
		McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::szLicenseGuid == %s"), szLicenseGuid);

		str.Format(sLicensesKeyTemplate, szLicenseGuid);
		hKey = McRegOpenSubKey(HKEY_LOCAL_MACHINE, str, KEY_QUERY_VALUE);

		if(hKey == NULL)
		{
			hr = HRESULT_FROM_WIN32(GetLastError());
			McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::McRegOpenSubKey returned NULL. hr = %d"), hr);
		}
		else
		{
			ret = RegQueryValueEx(hKey, sLicense, 0, &type, NULL, &cbData);
			if(ret == ERROR_SUCCESS && type == REG_BINARY)
			{
				McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::RegQueryValueEx cbData = %d"),cbData);

				LPBYTE pData = new BYTE[cbData];
				if(pData != NULL)
				{
					SecureZeroMemory(pData, cbData);
					ret = RegQueryValueEx(hKey, sLicense, 0, NULL, pData, &cbData);
					if(ret == ERROR_SUCCESS)
					{
						McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::RegQueryValueEx cbData = %d"),cbData);

						CCryptProv prov;

						McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::McInitializeCryptProv szContainer=%s"),szContainer);
						hr = McInitializeCryptProv(&prov, szContainer);

						if(SUCCEEDED(hr))
						{
							McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::GetLicenseFileName szLicenseGuid=%s"),szLicenseGuid);
							LPTSTR szLicenseFile = GetLicenseFileName(szLicenseGuid);

							if(szLicenseFile != NULL)
							{
								McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::GetSecretKey szLicenseGuid=%s, szLicenseFile = %s"),szLicenseGuid, szLicenseFile);
								LPTSTR szSecretKey = GetSecretKey(szLicenseGuid);

								if(szSecretKey != NULL)
								{
									GUID productGuid;
									hr = S2GUID(szProductGuid, &productGuid);
									if(SUCCEEDED(hr))
									{
										McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::McVerifyLicenseData"));
										hr = McVerifyLicenseData(prov, productGuid, szLicenseFile, HKEY_LOCAL_MACHINE, szSecretKey, pData, cbData, pExpirationDate);

										if(SUCCEEDED(hr))
										{
											McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::LoadDataFile"));
											hr = LoadDataFile(szLicenseFile, pLicenseData, pcbLicenseData, NULL);

											if(FAILED(hr))
												McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::LoadDataFile hr=%d"),hr);
										}
										else
										{
											McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::McVerifyLicenseDataError hr=%d"),hr);
										}
									}
									else
									{
										McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::S2GUIDError hr=%d"),hr);
									}

									McDelString(szSecretKey);
								}
								else
								{
									McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::GetSecretKeyError szSecretKey == NULL"));
								}

								McDelString(szLicenseFile);
							}
							else
							{
								McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::GetLicenseFileName returned NULL. hr = %d"),hr);
							}
						}
						else
						{
							McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::McInitializeCryptProvError hr = %d"),hr);
						}
					}
					else
					{
						McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::RegQueryValueExError ret = %d"),ret);
					}

					delete[] pData;
				}
				else
				{
					McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::Error LPBYTE pData = new BYTE[cbData]"));
				}
			}
			else
			{
				McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::RegQueryValueExError. hr = %d"), ret);
			}
		}
		McDelString(szLicenseGuid);
	}

	if(ret != ERROR_SUCCESS)
		hr = HRESULT_FROM_WIN32(ret);

	McTRACE(_T("McLicenseVerify::GetLicenseDataFromRegistry::End. hr: %d"), hr);
	return hr;
}

HRESULT GetLicenseDataSizeFromFile(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	OUT DWORD* pcbLicenseData,
	OUT LPBOOL pFileExists)
{
	return LoadLicenseFile(szLicenseFile, szProductGuid, NULL, NULL, NULL, NULL, NULL, NULL, pcbLicenseData, pFileExists);
}

HRESULT GetLicenseDataFromFile(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	IN LPCTSTR szContainer,
	OUT LPBYTE pLicenseData,
	IN OUT DWORD* pcbLicenseData,
	OUT LPBOOL pFileExists
	, OUT DWORD* pExpirationDate
	)
{
	HRESULT hr;
	GUID productGuid;
	DWORD cbData, cbHeader, cbXml;
	LPBYTE pData = NULL, pHeader = NULL, pXml = NULL;

	hr = LoadLicenseFile(szLicenseFile, szProductGuid, &productGuid, &pData, &cbData, &pHeader, &cbHeader, &pXml, &cbXml, pFileExists);
	if(SUCCEEDED(hr))
	{
		if(*pcbLicenseData < cbXml)
			hr = E_INVALIDARG;
		else
		{
			CCryptProv prov;
			hr = McInitializeCryptProv(&prov, szContainer);
			if(SUCCEEDED(hr))
			{
				hr = McVerifyLicenseData(prov, &productGuid, pHeader, cbHeader, pXml, cbXml, pExpirationDate);
				if(SUCCEEDED(hr))
					memcpy(pLicenseData, pXml, cbXml);
			}
		}
		delete[] pData;
	}
	return hr;
}

MCLICENSEVERIFY_API int GetLicenseDataSize(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	OUT DWORD* pcbLicenseData)
{
	McTRACE(_T("McLicenseVerify::GetLicenseDataSize::Begin. szLicenseFile: %s, szProductGuid: %s"), szLicenseFile, szProductGuid);
#ifdef DEBUG
	//Sleep(10000);
#endif
	HRESULT hr;
	BOOL fileExists = FALSE;
	if(szLicenseFile != NULL)
		hr = GetLicenseDataSizeFromFile(szLicenseFile, szProductGuid, pcbLicenseData, &fileExists);
	else
		hr = GetLicenseDataSizeFromRegistry(szProductGuid, pcbLicenseData);

	McTRACE(_T("McLicenseVerify::GetLicenseDataSize::End. hr: %d"), hr);
	return hr;
}

MCLICENSEVERIFY_API int GetLicenseData(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	IN LPCTSTR szContainer,
	OUT LPBYTE pLicenseData,
	IN OUT DWORD* pcbLicenseData)
{
	McTRACE(_T("McLicenseVerify::GetLicenseData::Begin. szLicenseFile: %s, szProductGuid: %s"), szLicenseFile, szProductGuid);

	szContainer = _T("{688AB952-EE73-48ac-A5DD-31D364A099CC}");

	HRESULT hr;
	BOOL fileExists = FALSE;
	if(szLicenseFile != NULL)
	{
		hr = GetLicenseDataFromFile(szLicenseFile, szProductGuid, szContainer, pLicenseData, pcbLicenseData, &fileExists, NULL);
	}
	else
	{
		hr = GetLicenseDataFromRegistry(szProductGuid, szContainer, pLicenseData, pcbLicenseData, NULL);
	}

	McTRACE(_T("McLicenseVerify::GetLicenseData::End. hr: %d"), hr);
	return hr;
}

MCLICENSEVERIFY_API int GetLicenseData2(
	IN LPCTSTR szLicenseFile
	, IN LPCTSTR szProductGuid
	, IN LPCTSTR szContainer
	, OUT LPBYTE pLicenseData
	, IN OUT DWORD* pcbLicenseData
	, OUT DWORD* pExpirationDate
	)
{
	McTRACE(_T("McLicenseVerify::GetLicenseData2::Begin. szLicenseFile: %s, szProductGuid: %s"), szLicenseFile, szProductGuid);

	szContainer = _T("{688AB952-EE73-48ac-A5DD-31D364A099CC}");

	HRESULT hr;
	BOOL fileExists = FALSE;

	if(szLicenseFile != NULL)
	{
		hr = GetLicenseDataFromFile(szLicenseFile, szProductGuid, szContainer, pLicenseData, pcbLicenseData, &fileExists, pExpirationDate);
	}
	else
	{
		hr = GetLicenseDataFromRegistry(szProductGuid, szContainer, pLicenseData, pcbLicenseData, pExpirationDate);
	}

	McTRACE(_T("McLicenseVerify::GetLicenseData2::End. hr: %d"), hr);
	return hr;
}
