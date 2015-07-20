#include "StdAfx.h"
#include ".\mcactivation.h"

#include <objbase.h>
#include <atlenc.h>
#include <Sddl.h>

#pragma comment(lib,"ole32.lib")

#ifdef _MCWIN64
const LPCTSTR sProductsKeyName = _T("SOFTWARE\\Wow6432Node\\Mediachase\\Activator\\Products");
const LPCTSTR sLicensesKeyTemplate = _T("SOFTWARE\\Wow6432Node\\Mediachase\\Activator\\Licenses\\%s");
const LPCTSTR sSecretKey = _T("SOFTWARE\\Wow6432Node\\Classes\\CLSID\\{5131CD20-43A3-4920-B059-87E572A605BB}\\");
const LPCTSTR sWindowsCurrentVersion = _T("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion");
#endif
#ifdef _MCWIN32
const LPCTSTR sProductsKeyName = _T("Software\\Mediachase\\Activator\\Products");
const LPCTSTR sLicensesKeyTemplate = _T("SOFTWARE\\Mediachase\\Activator\\Licenses\\%s");
const LPCTSTR sSecretKey = _T("SOFTWARE\\Classes\\CLSID\\{5131CD20-43A3-4920-B059-87E572A605BB}\\");
const LPCTSTR sWindowsCurrentVersion = _T("SOFTWARE\\Microsoft\\Windows\\CurrentVersion");
#endif

#define _COMPARE_MASK (0xFFFF)
#define _SHAM_MASK (0x7)

BYTE g_PublicKey[] = 
{
	0x6, 0x2, 0x0, 0x0, 0x0, 0x24, 0x0, 0x0, 
	0x52, 0x53, 0x41, 0x31, 0x0, 0x2, 0x0, 0x0, 
	0x1, 0x0, 0x1, 0x0, 0x9B, 0xE5, 0x2B, 0x8D, 
	0xDE, 0xA3, 0x6, 0x58, 0x1E, 0x77, 0x38, 0xA2, 
	0x25, 0xB4, 0x3D, 0xD7, 0xF7, 0x83, 0x2D, 0xB9, 
	0xFC, 0x5C, 0x99, 0x71, 0x4D, 0xCA, 0x10, 0x65, 
	0x68, 0x21, 0xB3, 0xBE, 0xF, 0x50, 0x94, 0x69, 
	0xB0, 0xA, 0x6E, 0x1F, 0xD6, 0x59, 0x60, 0x3F, 
	0xD5, 0x64, 0x56, 0x68, 0xF2, 0x62, 0x6C, 0x2B, 
	0xFF, 0xB7, 0xD0, 0x6A, 0x5A, 0x88, 0xE, 0x81, 
	0xEC, 0xBC, 0x16, 0xBC
};

TCHAR g_Alphabet[32] = 
{
	_T('1'),_T('2'),_T('3'),_T('4'),_T('5'),_T('6'),_T('7'),_T('8'),
	_T('9'),_T('0'),_T('A'),_T('S'),_T('D'),_T('F'),_T('G'),_T('H'),
	_T('J'),_T('K'),_T('L'),_T('M'),_T('N'),_T('B'),_T('V'),_T('C'),
	_T('X'),_T('Z'),_T('Y'),_T('T'),_T('R'),_T('E'),_T('W'),_T('P')
};

DWORD Time2Dword(const CTime &time)
{
	DWORD y = time.GetYear();
	DWORD m = time.GetMonth();
	DWORD d = time.GetDay();
	McTRACE(_T("Time2Dword y m d: %d %d %d"), y, m, d);

	DWORD retVal = d | (m<<5) | (y<<9);
	McTRACE(_T("Time2Dword retVal: %d"), retVal);

	return retVal;
}

WORD Time2Word(const CTime &time)
{
	WORD y = (WORD)(time.GetYear() - 2004);
	WORD m = (WORD)time.GetMonth();
	WORD d = (WORD)time.GetDay();
	McTRACE(_T("Time2Word y m d: %d %d %d"), y, m, d);

	WORD retVal = d | ((m<<5)&0x1E0) | ((y<<9)&0x1E00);
	McTRACE(_T("Time2Word retVal: %d"), retVal);

	return retVal;
}

CTime Word2Time(const WORD &value)
{
	// Unpack Time [3/16/2004]
	WORD dwDay = value&0x1F;
	WORD dwMonth = (value&0x1E0)>>5;
	WORD dwYear = (value&0x1E00)>>9;

	return CTime(dwYear+2004, dwMonth, dwDay, 0, 0, 0);
}

// Help Function
/************************************************************************/
/*	GetSysVolumeSerialNumber - Return Volume Serial Number for a Sys Dir.
Parameters
[out] lpVolumeSerialNumber - Volume Serial Number.
Return Values
Zero if success else error code.
*/
/************************************************************************/
HRESULT GetSysVolumeSerialNumber(OUT LPDWORD lpVolumeSerialNumber)
{
	// Step 0. Test In Argument
	HRESULT hr = S_OK;

	if(IsBadWritePtr(lpVolumeSerialNumber, sizeof(DWORD)))
		hr = E_INVALIDARG;
	else
	{
		*lpVolumeSerialNumber = 0;

		// Step 1. GetSystemDirectory [5/22/2002]
		TCHAR strSysDir[1024];

		if(!GetSystemWindowsDirectory(strSysDir,1024))
			hr = HRESULT_FROM_WIN32(GetLastError());
		else
		{
			McTRACE(_T("McActivation::SystemWindowsDirectory=%s"), strSysDir);
			strSysDir[3] = _T('\0');

			// Step 2. GetSysVolumeSerialNumber [5/22/2002]
			if(!GetVolumeInformation(strSysDir, NULL, 0, lpVolumeSerialNumber, NULL, 0, NULL, 0))
				hr = HRESULT_FROM_WIN32(GetLastError());
			else
				McTRACE(_T("McActivation::HardwareID=%d"), *lpVolumeSerialNumber);
		}
	}

	return hr;
}

WORD	McCoCRC16(const LPBYTE pBuffer, DWORD dwBufferSize)
{
	DWORD	dwAcamulator	=	0;
	WORD	wAcamulator	=	0;

	LPWORD dMarker = (LPWORD)pBuffer;

	DWORD dwWordsCount = dwBufferSize >> 1;

	while(dwWordsCount--)
	{
		dwAcamulator += *dMarker ++;
	}

	if(dwBufferSize&1)
		dwAcamulator += *((LPBYTE)dMarker);

	dwAcamulator = WORD(dwAcamulator) + WORD((dwAcamulator >> 16) & 0xffff);

	wAcamulator = WORD(dwAcamulator) + WORD(dwAcamulator>> 16);

	return wAcamulator != 0xffff ? ~wAcamulator: wAcamulator;
}


#ifdef MC_CLIENT_SIDE
HRESULT	McVerifySerialNumber(IN GUID ProductGuid, IN LPCTSTR szSerialNumber, OUT LPGUID pLicenseGuid)
{
	if(IsBadWritePtr(pLicenseGuid, sizeof(GUID)))
		return E_INVALIDARG;

	GUID hGuif = {0}, hEmptyGUID = {0};
	HRESULT hr  = McCoStringToGuid(szSerialNumber, &hGuif);

	if(hr==S_OK)
	{
		if(memcmp(&hGuif, &hEmptyGUID, sizeof(GUID)))
		{
			if(McCoCRC16((LPBYTE)&hGuif, sizeof(GUID)) == 0xFFFF)
			{
				if(hGuif.Data2 == McCoCRC16((LPBYTE)&ProductGuid, sizeof(GUID)))
					memcpy(pLicenseGuid, &hGuif, sizeof(GUID));
				else
					hr = E_FAIL;
			}
			else
				hr = E_FAIL;
		}
		else
			hr = E_FAIL;
	}

	return hr;
}
#endif

HRESULT	McCoStringToGuid(IN LPCTSTR	strGuid,OUT GUID *pGuid)
{
	if(IsBadStringPtr(strGuid,26))
		return E_INVALIDARG;

	if(IsBadWritePtr(pGuid,sizeof(GUID)))
		return E_INVALIDARG;

	HRESULT	hr	=	S_OK;

	GUID	tmpGUID	=	{0};

	LPBYTE	pBuffer	=	(LPBYTE)&tmpGUID;

	int BiteOffset		=	0;
	int WriteNextBites	=	5;
	int ByteOffset		=	0;

	DWORD	dwVal	=	0;

	for(int i=0;i<25;i++)
	{
		TCHAR	UnPackSim	=	strGuid[i];
		int SymID	=	-1;

		for(int j=0; j<sizeof(g_Alphabet)/sizeof(TCHAR); j++)
		{
			if(g_Alphabet[j]==UnPackSim)
			{
				SymID = j;
				break;
			}
		}

		if(SymID==-1)
			return E_PENDING;

		dwVal	=	SymID;

		int NowWriteByte	=	5;

		while(NowWriteByte)
		{
			*(pBuffer+ByteOffset)	|=	((BYTE)dwVal)<<(BiteOffset);

			dwVal			>>=	WriteNextBites;
			NowWriteByte	-=	WriteNextBites;

			if(NowWriteByte)
			{
				ByteOffset++;
				BiteOffset	=	0;
				WriteNextBites = NowWriteByte;
			}
			else
			{
				BiteOffset	+=	WriteNextBites;
				WriteNextBites	=	8	-	BiteOffset;
				if(WriteNextBites>5)
					WriteNextBites	=	5;
			}
		}
	}

	memcpy(pGuid,&tmpGUID,sizeof(GUID));

	return hr;
}


#ifdef MC_SERVER_SIDE
BYTE	g_key_PrivateBlob[]	= {0x7, 0x2, 0x0, 0x0, 0x0, 0x24, 0x0, 0x0, 0x52, 0x53, 
0x41, 0x32, 0x0, 0x2, 0x0, 0x0, 0x1, 0x0, 0x1, 0x0, 
0x9B, 0xE5, 0x2B, 0x8D, 0xDE, 0xA3, 0x6, 0x58, 0x1E, 
0x77, 0x38, 0xA2, 0x25, 0xB4, 0x3D, 0xD7, 0xF7, 0x83, 
0x2D, 0xB9, 0xFC, 0x5C, 0x99, 0x71, 0x4D, 0xCA, 0x10, 
0x65, 0x68, 0x21, 0xB3, 0xBE, 0xF, 0x50, 0x94, 0x69, 
0xB0, 0xA, 0x6E, 0x1F, 0xD6, 0x59, 0x60, 0x3F, 0xD5, 
0x64, 0x56, 0x68, 0xF2, 0x62, 0x6C, 0x2B, 0xFF, 0xB7, 
0xD0, 0x6A, 0x5A, 0x88, 0xE, 0x81, 0xEC, 0xBC, 0x16, 
0xBC, 0xDB, 0x13, 0x5A, 0x26, 0x64, 0x2F, 0x2D, 0xE3, 
0x35, 0x60, 0xA4, 0xE2, 0xEB, 0x52, 0x86, 0x6D, 0xBB, 
0xEC, 0x21, 0x64, 0xDD, 0x23, 0x21, 0xB, 0x9F, 0x15, 
0x18, 0xE7, 0xC1, 0xF9, 0x1A, 0xE7, 0x41, 0x1, 0x7B, 
0x84, 0x32, 0x95, 0xE9, 0x22, 0x92, 0xD9, 0x7B, 0xB0, 
0xDD, 0x47, 0x9B, 0x85, 0xED, 0x35, 0x21, 0xEF, 0x96, 
0x87, 0xEE, 0xE5, 0x23, 0x98, 0xA, 0xE4, 0xB7, 0x86, 
0x59, 0xD0, 0x25, 0xA6, 0x13, 0x6A, 0x9F, 0x2B, 0xA0, 
0x8D, 0xAC, 0xD8, 0xFD, 0x4B, 0x8C, 0xEE, 0xFD, 0x94, 
0x19, 0x1F, 0x8A, 0x8D, 0xB5, 0x45, 0xC, 0x1, 0x31, 
0xE1, 0x22, 0x99, 0x2C, 0x63, 0xB6, 0x69, 0x1, 0xA0, 
0xC5, 0x85, 0x77, 0x20, 0xF6, 0x6C, 0xBE, 0x53, 0xC6, 
0x1, 0xAD, 0x6B, 0xAF, 0x59, 0x4D, 0x92, 0x19, 0x42, 
0xAD, 0x8D, 0x3F, 0x97, 0xFF, 0x54, 0x94, 0x1, 0x40, 
0x94, 0xFB, 0x2D, 0x56, 0x11, 0x61, 0x79, 0xD3, 0x2B, 
0xDE, 0xC2, 0x3C, 0x74, 0xB2, 0x20, 0xCC, 0x2E, 0x4C, 
0xB, 0x21, 0x8C, 0x3A, 0x42, 0x70, 0xA1, 0x86, 0x15, 
0x7B, 0x7C, 0x71, 0xBC, 0xFA, 0xE8, 0xD5, 0x6B, 0x81, 
0xBC, 0x43, 0xBA, 0x2D, 0x50, 0x8, 0x48, 0x8D, 0xE4, 
0xB4, 0xBB, 0x4C, 0xE0, 0x4F, 0xA1, 0x97, 0xA6, 0xB4, 
0x28, 0x7F, 0x8B, 0xC9, 0xBD, 0x13, 0x7A, 0x49, 0xA1, 
0x3F, 0xD5, 0x23, 0x21, 0xFA, 0xC8, 0x89, 0x2A, 0xBB, 
0x7F, 0xBB, 0xB5, 0xC7, 0x73, 0x18, 0x12, 0x21, 0xA9, 
0xC1, 0x6, 0x63, 0x2A, 0x85, 0xC9, 0x17, 0x77, 0xF3, 
0x71, 0x7C, 0xD6, 0x2, 0xC, 0x5F, 0x57, 0x42, 0xA2};

HRESULT	McCoCreateGuid(IN GUID ProductGuid, OUT GUID *pGuid)
{
	if(IsBadWritePtr(pGuid,sizeof(GUID)))
		return E_INVALIDARG;

	// Step 1. Create New GUID
	HRESULT hr =	CoCreateGuid(pGuid);

	if(hr==S_OK)
	{
		pGuid->Data2	=	McCoCRC16((LPBYTE)&ProductGuid,sizeof(GUID));
		pGuid->Data3	=	0;
		pGuid->Data4[7]	&=	_SHAM_MASK;
		// Step 2. Create Check Sum and Add Check Sum to GUID 
		pGuid->Data3	=	McCoCRC16((LPBYTE)pGuid,sizeof(GUID));

		//ASSERT(McCoCRC16((LPBYTE)pGuid,sizeof(GUID))==0xFFFF);
	}

	return hr;
}

HRESULT McCreateProductKey(
						   IN const GUID *pGuid
						   , IN OUT LPTSTR szProductKey
						   , IN OUT int *pctProductKey
						   )
{
	if(IsBadReadPtr(pGuid, sizeof(GUID)))
		return E_INVALIDARG;

	if(szProductKey != NULL && IsBadStringPtr(szProductKey, 26))
		return E_INVALIDARG;

	TCHAR tmpBuff[30];
	int dwBuffSize = 0;

	HRESULT hr = S_OK;

	int BitOffset     = 0;
	int ReadNextBits  = 5;
	int ByteOffset     = 0;
	int LastByteOffset = -1;


	LPBYTE pBuffer = reinterpret_cast<LPBYTE>(const_cast<LPGUID>(pGuid));

	DWORD dwVal = 0;

	while(dwBuffSize < 25)
	{
		if(LastByteOffset!=ByteOffset)
		{
			DWORD dwByteVal = *(pBuffer+ByteOffset);
			LastByteOffset=ByteOffset;
			dwVal += dwByteVal<<(5-ReadNextBits);
		}

		DWORD AlphabetId = dwVal&0x1F;

		tmpBuff[dwBuffSize] = g_Alphabet[AlphabetId];

		dwBuffSize++;
		BitOffset+=5;
		dwVal >>= 5;

		if(BitOffset>7)
		{
			BitOffset %= 8;
			if(BitOffset==0)
				ByteOffset++;
		}

		if(BitOffset>3)
		{
			ReadNextBits = 5-(8 - BitOffset);
			ByteOffset++;
		}
		else
			ReadNextBits = 5;
	}

	tmpBuff[dwBuffSize] = _T('\0');

	if(szProductKey != NULL)
		_tcsncpy_s(szProductKey, *pctProductKey, tmpBuff, 26);

	if(pctProductKey != NULL)
		*pctProductKey = dwBuffSize;

	return hr;
}

HRESULT McCryptSignData(CCryptProv &hCriptProv, IN LPBYTE pbData, IN DWORD dwDataSize, OUT LPBYTE *ppbSignData, OUT DWORD *pdwSignDataSize)
{
	if(IsBadReadPtr(pbData,dwDataSize))
		return E_INVALIDARG;

	HRESULT hr;
	CCryptKey hTmpKey;

	// Step 2. Load Private Key
	CCryptImportKey hImportPrivateKey;
	hr = hImportPrivateKey.Initialize(hCriptProv,g_key_PrivateBlob,sizeof(g_key_PrivateBlob),hTmpKey,0);
	if(hr==S_OK)
	{
		// Step 3. Create Hash Function
		CCryptMD5Hash hMD5Hash;
		hr = hMD5Hash.Initialize(hCriptProv,_T("Mediachase"));
		if(hr==S_OK)
		{
			// Step 4. Calculate Hash Size
			hr = hMD5Hash.AddData(pbData,dwDataSize);
			if(hr==S_OK)
			{
				CHeapPtr<BYTE> pSignData;
				DWORD dwSignDataSize = 0;

				// Step 5. Calculate Sign Size
				hr = hMD5Hash.Sign(0,&dwSignDataSize);
				if(hr==S_OK)
				{
					if(pSignData.AllocateBytes(dwSignDataSize))
					{
						// Step 6. Sign Data
						hr = hMD5Hash.Sign(pSignData,&dwSignDataSize);
						if(hr==S_OK)
						{
							DWORD dwFullSignDataSize	=	sizeof(McCryptPacket) + dwDataSize + dwSignDataSize;

							McCryptPacket	CryptPacket	=	{0};
							CryptPacket.cbSize			=	sizeof(McCryptPacket);
							CryptPacket.dwDataSize		=	dwDataSize;
							CryptPacket.pDataOffset		=	0;
							CryptPacket.dwSignDataSize	=	dwSignDataSize;
							CryptPacket.pSignDataOffset	=	dwDataSize;

							CHeapPtr<BYTE> pFullSignData;

							if(pFullSignData.AllocateBytes(dwFullSignDataSize))
							{
								memcpy(pFullSignData,&CryptPacket,sizeof(McCryptPacket));
								memcpy(pFullSignData+sizeof(McCryptPacket)+CryptPacket.pDataOffset, pbData, CryptPacket.dwDataSize);
								memcpy(pFullSignData+sizeof(McCryptPacket)+CryptPacket.pSignDataOffset, pSignData, CryptPacket.dwSignDataSize);

								if(pdwSignDataSize)
									*pdwSignDataSize = dwFullSignDataSize;

								if(ppbSignData)
									*ppbSignData = pFullSignData.Detach();
							}
							else
								hr = GetLastError();
						}
					}
					else
						hr = GetLastError();
				}
			}
		}
	}
	return hr;
}

HRESULT McCreateLicenseData(CCryptProv &hCriptProv, GUID ProductGUID, GUID LicenseGUID, DWORD dwFlags, DWORD dwVolumeSerialNumber, DWORD dwTime, LPCTSTR LicenseFile, OUT LPBYTE *ppbSignData, OUT DWORD *pdwSignDataSize)
{
	if(IsBadWritePtr(ppbSignData,sizeof(LPBYTE)))
		return E_INVALIDARG;

	if(IsBadWritePtr(pdwSignDataSize,sizeof(DWORD)))
		return E_INVALIDARG;

	McLicencePacket packet = {0};

	packet.dwProtocolVersion	=	2;
	packet.ProductGUID			= ProductGUID;
	packet.LicenseGUID			= LicenseGUID;
	packet.dwFlags				= dwFlags;
	packet.dwVolumeSerialNumber = dwVolumeSerialNumber;
	packet.dwTime				= dwTime;

	HRESULT hr;

	packet.dwLicenseFileMD5HashBuffSize = 30;
	hr = McGetFileMD5Hash(hCriptProv,LicenseFile,packet.LicenseFileMD5HashBuff,&packet.dwLicenseFileMD5HashBuffSize);

	if(hr==S_OK)
		hr = McCryptSignData(hCriptProv, (LPBYTE)&packet,sizeof(packet),ppbSignData, pdwSignDataSize);

	return hr;
}

HRESULT	McCreateLicenseData(CCryptProv	&hCriptProv, GUID	ProductGUID, GUID LicenseGUID, DWORD dwFlags, DWORD	dwVolumeSerialNumber, DWORD dwTime, BYTE* LicenseData, DWORD dwLicenseDataSize, OUT LPBYTE *ppbSignData, OUT DWORD *pdwSignDataSize)
{
	if(IsBadReadPtr(LicenseData,dwLicenseDataSize))
		return E_INVALIDARG;

	if(IsBadWritePtr(ppbSignData,sizeof(LPBYTE)))
		return E_INVALIDARG;

	if(IsBadWritePtr(pdwSignDataSize,sizeof(DWORD)))
		return E_INVALIDARG;

	McLicencePacket	packet	=	{0};

	packet.dwProtocolVersion	=	2;
	packet.ProductGUID			= ProductGUID;
	packet.LicenseGUID			= LicenseGUID;	
	packet.dwFlags				= dwFlags;
	packet.dwVolumeSerialNumber = dwVolumeSerialNumber;
	packet.dwTime				= dwTime;

	HRESULT hr	=	S_OK;

	if(hr==S_OK)
	{
		CCryptMD5Hash	hMD5Hash;
		hr = hMD5Hash.Initialize(hCriptProv,_T("Mediachase"));
		if(hr==S_OK)
		{
			hMD5Hash.AddData(LicenseData,dwLicenseDataSize);

			packet.dwLicenseFileMD5HashBuffSize = 30;
			hr = hMD5Hash.GetValue(packet.LicenseFileMD5HashBuff,&packet.dwLicenseFileMD5HashBuffSize);

			if(hr==S_OK)
				hr = McCryptSignData(hCriptProv, (LPBYTE)&packet,sizeof(packet),ppbSignData, pdwSignDataSize);
		}
	}

	return hr;
}

HRESULT UnpackClientPacket(IN LPCTSTR szClientData, OUT PMcClientPacket pPacket)
{
	HRESULT hr = S_OK;

	int nSrcLen = static_cast<int>(_tcslen(szClientData));
	int nDestLen = Base64DecodeGetRequiredLength(nSrcLen);
	CHeapPtr<BYTE> ClientDataBuffer;
	if(!ClientDataBuffer.AllocateBytes(nDestLen))
		hr = E_OUTOFMEMORY;
	else
	{
		if(!Base64Decode(szClientData, nSrcLen, ClientDataBuffer, &nDestLen))
			hr = E_INVALIDARG;
		else
		{
			if(nDestLen < sizeof(McClientPacket))
				hr = E_INVALIDARG;
			else
			{
				memcpy(pPacket, ClientDataBuffer, sizeof(McClientPacket));
			}
		}
	}
	return hr;
}
#endif

HRESULT McGetFileMD5Hash(CCryptProv	&hCriptProv, LPCTSTR File, BYTE* pBuffer, DWORD* pdwBufferSize)
{
	HRESULT hr;

	HANDLE hFile = CreateFile(File, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	if(hFile == INVALID_HANDLE_VALUE)
	{
		hr = AtlHresultFromLastError();
	}
	else
	{
		CCryptMD5Hash hMD5Hash;
		hr = hMD5Hash.Initialize(hCriptProv, _T("Mediachase"));

		if(hr == S_OK)
		{
			const DWORD dwBufferSize = 1024 * 64;
			BYTE* tmpBuffer = new BYTE[dwBufferSize];
			if(tmpBuffer == NULL)
			{
				hr = E_OUTOFMEMORY;
			}
			else
			{
				DWORD dwRealRead = dwBufferSize;
				while(dwRealRead == dwBufferSize)
				{
					dwRealRead = 0;

					if(ReadFile(hFile, tmpBuffer, dwBufferSize, &dwRealRead, NULL))
					{
						if(dwRealRead > 0)
						{
							hMD5Hash.AddData(tmpBuffer, dwRealRead);
						}
					}
					else
					{
						hr = AtlHresultFromLastError();
						break;
					}
				}

				if(hr == S_OK)
					hr = hMD5Hash.GetValue(pBuffer, pdwBufferSize);

				delete[] tmpBuffer;
			}
		}

		CloseHandle(hFile);
	}

	return hr;
}

#ifdef MC_CLIENT_SIDE
HRESULT GetBufferMd5Hash(IN CCryptProv &hCriptProv, IN LPBYTE pData, IN DWORD cbData, IN CCryptMD5Hash* pHash)
{
	HRESULT hr = E_INVALIDARG;
	if(pHash != NULL)
	{
		hr = pHash->Initialize(hCriptProv, _T("Mediachase"));
		if(SUCCEEDED(hr))
			hr = pHash->AddData(pData, cbData);
	}
	return hr;
}

HRESULT McVerifyLicenseData(CCryptProv &hCriptProv, IN LPGUID pProductGUID, IN LPBYTE pHeader, IN DWORD cbHeader, IN LPBYTE pXml, IN DWORD cbXml, OUT DWORD* pExpirationDate)
{
	if(pExpirationDate != NULL && IsBadWritePtr(pExpirationDate, sizeof(DWORD)))
		return E_INVALIDARG;

	HRESULT hr;
	CCryptKey hTmpKey;
	CCryptImportKey hImportPublicKey;

	if(pExpirationDate != NULL)
		*pExpirationDate = 0; // Unknown

	// Load public key.
	hr = hImportPublicKey.Initialize(hCriptProv, g_PublicKey, sizeof(g_PublicKey), hTmpKey, 0);
	if(SUCCEEDED(hr))
	{
		if(cbHeader >= sizeof(PMcCryptPacket) && !IsBadReadPtr(pHeader, cbHeader))
		{
			PMcCryptPacket pCryptPacket = reinterpret_cast<PMcCryptPacket>(pHeader);

			LPBYTE pbVerData = pHeader + pCryptPacket->cbSize + pCryptPacket->pDataOffset;
			LPBYTE pbVerSignData = pHeader + pCryptPacket->cbSize + pCryptPacket->pSignDataOffset;

			if(!IsBadReadPtr(pbVerData, pCryptPacket->dwDataSize)
				&& !IsBadReadPtr(pbVerSignData, pCryptPacket->dwSignDataSize))
			{
				CCryptMD5Hash hMD5Hash;
				hr = GetBufferMd5Hash(hCriptProv, pbVerData, pCryptPacket->dwDataSize, &hMD5Hash);
				if(SUCCEEDED(hr))
				{
					hr = hMD5Hash.VerifySignature(pbVerSignData, pCryptPacket->dwSignDataSize, hImportPublicKey);
					if(SUCCEEDED(hr))
					{
						CCryptMD5Hash hXmlHash;
						hr = GetBufferMd5Hash(hCriptProv, pXml, cbXml, &hXmlHash);
						if(SUCCEEDED(hr))
						{
							CHeapPtr<BYTE> pHashData;
							DWORD dwHashDataSize = 30;
							pHashData.AllocateBytes(dwHashDataSize);
							hr = hXmlHash.GetValue(pHashData, &dwHashDataSize);
							if(SUCCEEDED(hr))
							{
								hr = E_FAIL;
								if(pCryptPacket->dwDataSize >= sizeof(McLicencePacket))
								{
									McLicencePacket packet = {0};
									memcpy(&packet, pbVerData, sizeof(McLicencePacket));

									if(packet.dwProtocolVersion == 2
										&& packet.dwLicenseFileMD5HashBuffSize == dwHashDataSize
										&& memcmp(pHashData, packet.LicenseFileMD5HashBuff, packet.dwLicenseFileMD5HashBuffSize) == 0
										&& memcmp((const VOID*)pProductGUID, (const VOID*)&packet.ProductGUID, sizeof(GUID)) == 0)
									{
										//packet.dwFlags: 1b - Unlimited Time (1,0), 2b - (ToTime - 0, Duration - 1), 3 bit - USEHDD
										BOOL bInfinite = ((packet.dwFlags & 1) != 0);
										BOOL bDuration = ((packet.dwFlags & 2) != 0);
										BOOL bCheckHdd = ((packet.dwFlags & 4) != 0);

										if(!bCheckHdd && (bInfinite || !bDuration))
										{
											if(bInfinite)
											{
												hr = S_OK;
												if(pExpirationDate != NULL)
													*pExpirationDate = (DWORD)-1; // Infinite
											}
											else
											{
												CTime finishTime = Word2Time((WORD)packet.dwTime);
												if(pExpirationDate != NULL)
													*pExpirationDate = Time2Dword(finishTime);

												CTime currentTime = CTime::GetCurrentTime();
												if(currentTime < finishTime)
													hr = S_OK;
												else
													hr = E_LICENSE_EXPIRED;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else 
				hr = E_INVALIDARG;
		}
		else 
			hr = E_INVALIDARG;
	}
	return hr;
}

HRESULT McVerifyLicenseData(CCryptProv &hCriptProv, IN LPBYTE pbSignData, IN DWORD dwSignDataSize, GUID	*pProductGUID, GUID	*pLicenseGUID)
{
	if(IsBadReadPtr(pbSignData,dwSignDataSize))
		return E_INVALIDARG;

	if(IsBadWritePtr(pProductGUID,sizeof(GUID)))
		return E_INVALIDARG;

	if(IsBadWritePtr(pLicenseGUID,sizeof(GUID)))
		return E_INVALIDARG;

	HRESULT hr = S_OK;

	if(hr==S_OK)
	{
		CCryptKey	hTmpKey;
		// Step 2. Load Public Key
		CCryptImportKey	hImportPublicKey;
		hr = hImportPublicKey.Initialize(hCriptProv, g_PublicKey, sizeof(g_PublicKey), hTmpKey,0);
		if(hr==S_OK)
		{
			// Step 3. Create Hash Function
			CCryptMD5Hash	hMD5Hash;
			hr = hMD5Hash.Initialize(hCriptProv,_T("Mediachase"));
			if(hr==S_OK)
			{
				PMcCryptPacket	pCryptPacket	=	(PMcCryptPacket)pbSignData;

				LPBYTE			pbVerData		=	pbSignData+pCryptPacket->cbSize+pCryptPacket->pDataOffset;
				LPBYTE			pbVerSignData	=	pbSignData+pCryptPacket->cbSize+pCryptPacket->pSignDataOffset;

				// I can read data
				// I can read sign data
				if(!IsBadReadPtr(pbVerData,pCryptPacket->dwDataSize)&&
					!IsBadReadPtr(pbVerSignData,pCryptPacket->dwSignDataSize))
				{
					// Step 4. Calculate Hash Size
					hr = hMD5Hash.AddData(pbVerData,pCryptPacket->dwDataSize);
					if(hr==S_OK)
					{
						hr = hMD5Hash.VerifySignature(pbVerSignData,pCryptPacket->dwSignDataSize,hImportPublicKey);
						if(hr==S_OK)
						{
							McLicencePacket	packet	=	{0};
							memcpy(&packet,pbVerData,sizeof(McLicencePacket));

							if(packet.dwProtocolVersion==2)
							{
								memcpy(pProductGUID,&packet.ProductGUID,sizeof(GUID));
								memcpy(pLicenseGUID,&packet.LicenseGUID,sizeof(GUID));
							}
							else
								hr = E_FAIL;
						}
					}
				}
				else 
					hr = E_INVALIDARG;
			}
		}
	}
	return hr;
}


HRESULT McVerifyLicenseData(CCryptProv &hCriptProv, GUID ProductGUID, LPCTSTR LicenseFile, HKEY keyParent, LPCTSTR RegDatePath, IN LPBYTE pbSignData, IN DWORD dwSignDataSize, OUT DWORD* pExpirationDate)
{
	if(IsBadReadPtr(pbSignData, dwSignDataSize))
		return E_INVALIDARG;
	if(pExpirationDate != NULL && IsBadWritePtr(pExpirationDate, sizeof(DWORD)))
		return E_INVALIDARG;

	HRESULT hr;
	CCryptKey hTmpKey;

	if(pExpirationDate != NULL)
		*pExpirationDate = 0; // Unknown

	// Step 2. Load Public Key
	CCryptImportKey hImportPublicKey;
	McTRACE(_T("McActivation::McVerifyLicenseData::hImportPublicKey.Initialize"));
	hr = hImportPublicKey.Initialize(hCriptProv, g_PublicKey, sizeof(g_PublicKey), hTmpKey, 0);
	if(hr == S_OK)
	{
		// Step 3. Create Hash Function
		CCryptMD5Hash hMD5Hash;
		McTRACE(_T("McActivation::McVerifyLicenseData::hMD5Hash.Initialize"));
		hr = hMD5Hash.Initialize(hCriptProv, _T("Mediachase"));
		if(hr == S_OK)
		{
			PMcCryptPacket pCryptPacket = (PMcCryptPacket)pbSignData;

			LPBYTE pbVerData = pbSignData + pCryptPacket->cbSize + pCryptPacket->pDataOffset;
			LPBYTE pbVerSignData = pbSignData + pCryptPacket->cbSize + pCryptPacket->pSignDataOffset;

			// I can read data
			// I can read sign data
			if(!IsBadReadPtr(pbVerData,pCryptPacket->dwDataSize)&&
				!IsBadReadPtr(pbVerSignData, pCryptPacket->dwSignDataSize))
			{
				// Step 4. Calculate Hash Size
				McTRACE(_T("McActivation::McVerifyLicenseData::hMD5Hash.AddData"));
				hr = hMD5Hash.AddData(pbVerData, pCryptPacket->dwDataSize);
				if(hr == S_OK)
				{
					McTRACE(_T("McActivation::McVerifyLicenseData::hMD5Hash.VerifySignature"));
					hr = hMD5Hash.VerifySignature(pbVerSignData, pCryptPacket->dwSignDataSize, hImportPublicKey);
					if(hr == S_OK)
					{
						CHeapPtr<BYTE> pHashData;
						pHashData.AllocateBytes(30);
						DWORD dwHashDataSize = 30;

						McTRACE(_T("McActivation::McVerifyLicenseData::McGetFileMD5Hash LicenseFile=%s"), LicenseFile);
						hr = McGetFileMD5Hash(hCriptProv,LicenseFile,pHashData,&dwHashDataSize);
						if(hr == S_OK)
						{
							hr = E_FAIL;

							McTRACE(_T("McActivation::McVerifyLicenseData::pCryptPacket->dwDataSize=%d, sizeof(McLicencePacket)=%d"), pCryptPacket->dwDataSize, sizeof(McLicencePacket));
							if(pCryptPacket->dwDataSize>=sizeof(McLicencePacket))
							{
								McLicencePacket packet = {0};

								memcpy(&packet,pbVerData,sizeof(McLicencePacket));

								McTRACE(_T("McActivation::McVerifyLicenseData::packet.dwProtocolVersion=%d, packet.dwLicenseFileMD5HashBuffSize=%d, dwHashDataSize=%d"), packet.dwProtocolVersion, packet.dwLicenseFileMD5HashBuffSize, dwHashDataSize);
								if(packet.dwProtocolVersion == 2
									&& packet.dwLicenseFileMD5HashBuffSize == dwHashDataSize
									&& memcmp(pHashData,packet.LicenseFileMD5HashBuff,packet.dwLicenseFileMD5HashBuffSize) == 0
									&& memcmp((const VOID*)&ProductGUID,(const VOID*)&packet.ProductGUID,sizeof(GUID)) == 0
									)
								{
									//packet.dwFlags: 1b - Unlimited Time (1,0), 2b - (ToTime - 0, Duration - 1), 3 bit - USEHDD
									BOOL bInfinite = ((packet.dwFlags & 1) != 0);
									BOOL bDuration = ((packet.dwFlags & 2) != 0);
									BOOL bCheckHdd = ((packet.dwFlags & 4) != 0);

									DWORD dwSerialNumber = 0;
									HRESULT hr2 = GetSysVolumeSerialNumber(&dwSerialNumber);
									McTRACE(_T("McActivation::McVerifyLicenseData::GetSysVolumeSerialNumber::result=%d"), hr2);
									McTRACE(_T("McActivation::McVerifyLicenseData::1=%d, 2=%d, 3=%d"), packet.dwFlags, packet.dwVolumeSerialNumber, dwSerialNumber);

									// Check HDD [6/29/2004]
									if(bCheckHdd && packet.dwVolumeSerialNumber != dwSerialNumber)
									{
										hr = E_LICENSE_WRONG_PC;
									}
									else
									{
										// Check Time [6/29/2004]
										if(bInfinite)
										{
											hr = S_OK;
											if(pExpirationDate != NULL)
												*pExpirationDate = (DWORD)-1; // Infinite
										}
										else 
										{
											CRegKey regKey;

											DWORD dwRegTime = (DWORD)rand();
											TrialInfo trialInfo = {0};

											McTRACE(_T("McActivation::McVerifyLicenseData::regKey.Open"));
											if((hr = HRESULT_FROM_WIN32(regKey.Open(keyParent, RegDatePath, KEY_READ))) == 0)
											{
												ULONG lBytes = 0;
												McTRACE(_T("McActivation::McVerifyLicenseData::regKey.QueryBinaryValue"));
												if((hr = HRESULT_FROM_WIN32(regKey.QueryBinaryValue(NULL, NULL, &lBytes))) == 0)
												{
													CHeapPtr<BYTE> pTrial64Buffer;
													pTrial64Buffer.AllocateBytes(lBytes);

													McTRACE(_T("McActivation::McVerifyLicenseData::regKey.QueryBinaryValue"));
													if((hr = HRESULT_FROM_WIN32(regKey.QueryBinaryValue(NULL, pTrial64Buffer, &lBytes))) == 0)
													{
														int TrialBufferLen = Base64DecodeGetRequiredLength(lBytes);
														CHeapPtr<BYTE> pTrialBuffer;
														pTrialBuffer.AllocateBytes(TrialBufferLen);

														McTRACE(_T("McActivation::McVerifyLicenseData::Base64Decode"));
														if(Base64Decode((LPCSTR)(LPBYTE)pTrial64Buffer, lBytes, pTrialBuffer, &TrialBufferLen))
														{
															McTRACE(_T("McActivation::McVerifyLicenseData::1=%d, 2=%d"), TrialBufferLen, sizeof(TrialInfo));
															if(TrialBufferLen>=sizeof(TrialInfo))
															{
																memcpy(&trialInfo, pTrialBuffer, sizeof(TrialInfo));
																dwRegTime = trialInfo.Date ^ trialInfo.Random;
															}
															else
																hr = E_FAIL;
														}
														else
															hr = E_FAIL;
													}
												}


												WIN32_FILE_ATTRIBUTE_DATA attr = {0};

												McTRACE(_T("McActivation::McVerifyLicenseData::GetFileAttributesEx"));
												if(SUCCEEDED(hr) && GetFileAttributesEx(LicenseFile, GetFileExInfoStandard, (LPVOID)&attr))
												{
													SYSTEMTIME sysCreationTime = {0};
													//SYSTEMTIME sysTodayTime = {0};

													FileTimeToSystemTime(&attr.ftCreationTime, &sysCreationTime);

													McTRACE(_T("McActivation::McVerifyLicenseData::GetFileAttributesEx"));
													if((dwRegTime & 1023) == sysCreationTime.wMilliseconds)
													{
														hr = E_LICENSE_EXPIRED;

														// Unpack Time [3/16/2004]
														CTime currentTime = CTime::GetCurrentTime();
														currentTime = CTime(currentTime.GetYear(), currentTime.GetMonth(), currentTime.GetDay(), 12, 0, 0);

														CTime installTime = Word2Time(static_cast<WORD>(dwRegTime));
#ifdef LOG
														_tprintf(_T("1: %d.\n"), dwRegTime);
														_tprintf(_T("2: %s.\n"), currentTime.Format(_T("%Y-%m-%d %H:%M:%S")));
														_tprintf(_T("3: %s.\n"), installTime.Format(_T("%Y-%m-%d %H:%M:%S")));
#endif
														CTime finishTime;
														if(bDuration)
														{
															finishTime = installTime + CTimeSpan(packet.dwTime, 0, 0, 0);
														}
														else //ToTime
														{
															finishTime = Word2Time((WORD)packet.dwTime);
														}
														if(pExpirationDate != NULL)
															*pExpirationDate = Time2Dword(finishTime);
#ifdef LOG
														_tprintf(_T("4: %d.\n"), packet.dwFlags);
														_tprintf(_T("5: %d.\n"), packet.dwTime);
														_tprintf(_T("6: %s.\n"), finishTime.Format(_T("%Y-%m-%d %H:%M:%S")));
#endif
														if(currentTime < finishTime)
														{
															hr = S_OK;
														}
													}
												}
												else
												{
													hr = AtlHresultFromLastError();
												}
											}

										}
									}
								}
							}
						}
					}
				}
			}
			else 
				hr =  E_INVALIDARG;
		}
	}
	return hr;
}

LPTSTR GetSecretKey(LPCTSTR szLicenseGuid)
{
	size_t size1 = _tcslen(sSecretKey);
	size_t size2 = _tcslen(szLicenseGuid);
	size_t size = size1 + size2;
	LPTSTR sz = new TCHAR[size+1];
	if(sz == NULL)
		SetLastError(ERROR_OUTOFMEMORY);
	else
	{
		_tcscpy_s(sz, size+1, sSecretKey);
		_tcscat_s(sz, size+1, szLicenseGuid);
		for(size_t i=0; i<size2; i++)
			sz[size1+i] = szLicenseGuid[size2 - i - 1];
		sz[size] = _T('\0');
	}
	return sz;
}

InstResult InstallDateGetCurrent(LPWORD pwDate, LPCTSTR szSecretKey)
{
	InstResult ret = InstallDateGetFromRegistry(pwDate, szSecretKey);
#if defined(INTERNAL)
	ret = INST_NotInstalled;
#endif
	if(ret == INST_NotInstalled)
	{
		/*		SYSTEMTIME t;
		GetSystemTime(&t);
		*pwDate = t.wDay;
		*pwDate |= (t.wMonth << 5);
		*pwDate |= ((t.wYear - 2004) << 9);
		*/
		CTime t = CTime::GetCurrentTime();
		//*pwDate = t.GetDay();
		//*pwDate |= (t.GetMonth() << 5);
		//*pwDate |= ((t.GetYear() - 2004) << 9);
		*pwDate = Time2Word(t);
	}

	return ret;
}

InstResult InstallDateGetFromRegistry(LPWORD pwDate, LPCTSTR szSecretKey)
{
	InstResult ret = INST_NotInstalled;
	*pwDate = 0;

	LONG res;
	HKEY hKey;
	DWORD cbData;
	DWORD type;

	res = RegOpenKeyEx(HKEY_LOCAL_MACHINE, szSecretKey, 0, KEY_QUERY_VALUE, &hKey);
	if(ERROR_SUCCESS == res)
	{
		res = RegQueryValueEx(hKey, NULL, 0, &type, NULL, &cbData);
		if(res == ERROR_SUCCESS && type == REG_BINARY)
		{
			ret = INST_WrongData;
			LPBYTE pData = new BYTE[cbData];
			if(pData != NULL)
			{
				SecureZeroMemory(pData, cbData);
				res = RegQueryValueEx(hKey, NULL, 0, NULL, pData, &cbData);
				if(res == ERROR_SUCCESS)
				{
					int nSrcLen = static_cast<int>(cbData);
					int nDestLen = Base64DecodeGetRequiredLength(nSrcLen);
					BYTE* pDest = new BYTE[nDestLen];
					if(pDest != NULL)
					{
						if(Base64Decode(reinterpret_cast<LPCSTR>(pData), nSrcLen, pDest, &nDestLen))
						{
							if(sizeof(TrialInfo) == static_cast<size_t>(nDestLen))
							{
								TrialInfo *pInfo = reinterpret_cast<TrialInfo*>(pDest);
								if(pInfo->Version == 2)
								{
									*pwDate = pInfo->Date ^ pInfo->Random;
									ret = INST_OK;
								}
							}
						}
						delete[] pDest;
					}
				}
				delete[] pData;
			}
		}
		RegCloseKey(hKey);
	}
	return ret;
}

LPTSTR GetLicenseFileName(LPCTSTR szLicenseGuid)
{
	LPTSTR ret = NULL;

	LPTSTR szPath = McRegGetString(HKEY_LOCAL_MACHINE, sWindowsCurrentVersion, _T("CommonFilesDir"), NULL);
	if(szPath != NULL)
	{
		LPCTSTR szSubPath = _T("\\Mediachase\\License\\");
		LPCTSTR szExtension = _T(".xml");
		size_t size = 1 + _tcslen(szPath) + _tcslen(szSubPath) + _tcslen(szLicenseGuid) + _tcslen(szExtension);
		ret = new TCHAR[size];
		if(ret == NULL)
			SetLastError(ERROR_OUTOFMEMORY);
		else
		{
			_tcscpy_s(ret, size, szPath);
			_tcscat_s(ret, size, szSubPath);
			_tcscat_s(ret, size, szLicenseGuid);
			_tcscat_s(ret, size, szExtension);
		}
		McDelString(szPath);
	}
	return ret;
}
#endif


HRESULT CreateDacl(OUT LPBYTE* ppData, OUT LPDWORD pcbData)
{
	*ppData = NULL;
	*pcbData = 0;

	HRESULT hr = S_OK;
#define NUM_OF_ACES 2
	LPCTSTR sids[] = {TEXT("S-1-1-0"), TEXT("S-1-5-18")}; // Everyone, Local System
	PSID pSids[NUM_OF_ACES];

	size_t cbAcl = sizeof(ACL) + ((sizeof(ACCESS_ALLOWED_ACE) - sizeof(DWORD)) * NUM_OF_ACES);
	for(int i=0; i<NUM_OF_ACES; i++)
	{
		if(ConvertStringSidToSid(sids[i], &(pSids[i])))
			cbAcl += GetLengthSid(pSids[i]);
		else
		{
			hr = HRESULT_FROM_WIN32(GetLastError());
			break;
		}
	}

	if(SUCCEEDED(hr))
	{
		PACL pAcl = reinterpret_cast<PACL>(new BYTE[cbAcl]);
		if(pAcl != NULL)
		{
			if(InitializeAcl(pAcl, static_cast<DWORD>(cbAcl), ACL_REVISION))
			{
				for(int i=0; i<NUM_OF_ACES; i++)
				{
					if(!AddAccessAllowedAceEx(pAcl, ACL_REVISION, CONTAINER_INHERIT_ACE | OBJECT_INHERIT_ACE, 0x700F01FF, pSids[i]))
					{
						hr = HRESULT_FROM_WIN32(GetLastError());
						break;
					}
				}
				if(SUCCEEDED(hr))
				{
					*ppData = reinterpret_cast<LPBYTE>(pAcl);
					*pcbData = static_cast<DWORD>(cbAcl);
				}
			}
			else
				hr = HRESULT_FROM_WIN32(GetLastError());

			if(FAILED(hr))
				delete[] reinterpret_cast<PBYTE>(pAcl);
		}
		else
			hr = E_OUTOFMEMORY;
	}
	return hr;
}

HRESULT CreateSecurityDescriptor(OUT PSECURITY_DESCRIPTOR* ppSd, OUT LPBYTE* ppDacl)
{
	*ppSd = NULL;
	*ppDacl = NULL;

	HRESULT hr;
	PSECURITY_DESCRIPTOR pSd = new SECURITY_DESCRIPTOR;
	if(pSd != NULL)
	{
		if(InitializeSecurityDescriptor(pSd, SECURITY_DESCRIPTOR_REVISION))
		{
			LPBYTE pbDacl = NULL;
			DWORD cbDacl = 0;

			hr = CreateDacl(&pbDacl, &cbDacl);
			if(SUCCEEDED(hr))
			{
				if(SetSecurityDescriptorDacl(pSd, TRUE, reinterpret_cast<PACL>(pbDacl), FALSE))
				{
					*ppSd = pSd;
					*ppDacl = pbDacl;
				}
				else
				{
					hr = HRESULT_FROM_WIN32(GetLastError());
					delete[] pbDacl;
				}
			}
		}
		else
			hr = HRESULT_FROM_WIN32(GetLastError());
	}
	else
		hr = E_OUTOFMEMORY;
	return hr;
}

HRESULT McInitializeCryptProv(IN CCryptProv* pCriptProv, IN LPCTSTR szContainer)
{
	HRESULT hr;

	DWORD dwProviderType = PROV_RSA_FULL;
	LPCTSTR szProvider = MS_DEF_PROV;
	DWORD dwFlags = CRYPT_SILENT|CRYPT_MACHINE_KEYSET;
	BOOL bCreated = FALSE;

	hr = pCriptProv->Initialize(dwProviderType, szContainer, szProvider, dwFlags);
	if(hr == NTE_BAD_KEYSET)
	{
		hr = pCriptProv->Initialize(dwProviderType, szContainer, szProvider, dwFlags|CRYPT_NEWKEYSET);
		if(SUCCEEDED(hr))
			bCreated = TRUE;
		else if(hr == NTE_EXISTS)
		{
			hr = pCriptProv->DeleteKeySet(dwProviderType, szContainer, szProvider, dwFlags);
			if(SUCCEEDED(hr))
			{
				hr = pCriptProv->Initialize(dwProviderType, szContainer, szProvider, dwFlags|CRYPT_NEWKEYSET);
				if(SUCCEEDED(hr))
					bCreated = TRUE;
			}
		}
	}

	HRESULT hr2;
	if(bCreated)
	{
		// Set DACL for this container to allow full control for everyone and for local system.
		PSECURITY_DESCRIPTOR pSd = NULL;
		LPBYTE pbDacl = NULL;

		hr2 = CreateSecurityDescriptor(&pSd, &pbDacl);
		if(SUCCEEDED(hr2))
		{
			hr2 = pCriptProv->SetParam(PP_KEYSET_SEC_DESCR, reinterpret_cast<LPBYTE>(pSd), DACL_SECURITY_INFORMATION);
			delete pSd;
			delete[] pbDacl;
		}
	}
	return hr;
}

LPTSTR McConcatStrings(LPCTSTR sz1, LPCTSTR sz2)
{
	TCHAR* szResult = NULL;

	size_t n = _tcslen(sz1) + _tcslen(sz2);
	if(n > 0)
	{
		szResult = new TCHAR[n+1];
		if(szResult != NULL)
			_stprintf_s(szResult, n+1, TEXT("%s%s"), sz1, sz2);
	}
	return szResult;
}

void McDelString(LPTSTR sz)
{
	if(sz != NULL)
		delete[] sz;
}

HKEY McRegOpenSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired)
{
	HKEY hSubKey = NULL;
	LONG err = RegOpenKeyEx(hKey, szSubKey, 0, samDesired, &hSubKey);
	SetLastError(err);
	return hSubKey;
}

HKEY McRegCreateSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired)
{
	DWORD dw;
	HKEY hSubKey = NULL;
	LONG err = RegCreateKeyEx(hKey, szSubKey, 0, REG_NONE, REG_OPTION_NON_VOLATILE, samDesired, NULL, &hSubKey, &dw);
	SetLastError(err);
	return hSubKey;
}

LPTSTR McRegGetString(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, LPTSTR szDefault)
{
	McTRACE(_T("McActivation::McRegGetString::Begin. hKey: %d, szSubKey: %s, szEntry: %s"), hKey, szSubKey, szEntry);

	LPTSTR ret = szDefault;

	HKEY hSecKey = hKey;
	if(szSubKey != NULL && lstrlen(szSubKey) > 0)
	{
		hSecKey = McRegOpenSubKey(hKey, szSubKey, KEY_READ);
		if(hSecKey != NULL)
		{
			DWORD dwType, dwCount;
			LONG lResult = RegQueryValueEx(hSecKey, (LPTSTR)szEntry, NULL, &dwType, NULL, &dwCount);
			if(lResult != ERROR_SUCCESS)
				SetLastError(lResult);
			else if(dwType != REG_SZ)
				SetLastError(ERROR_BAD_ARGUMENTS);
			else
			{
				LPTSTR Value = new TCHAR[dwCount/sizeof(TCHAR)];
				if(Value == NULL)
					SetLastError(ERROR_OUTOFMEMORY);
				else
				{
					lResult = RegQueryValueEx(hSecKey, (LPTSTR)szEntry, NULL, &dwType, (LPBYTE)Value, &dwCount);
					if(lResult != ERROR_SUCCESS)
					{
						McDelString(Value);
						SetLastError(lResult);
					}
					else
						ret = Value;
				}
			}
			if(hSecKey != hKey)
				RegCloseKey(hSecKey);
		}
	}

	McTRACE(_T("McActivation::McRegGetString::End. ret: %s"), ret);
	return ret;
}

HRESULT S2GUID(IN LPCTSTR szGuid, OUT LPGUID pGuid)
{
	return CLSIDFromString(CT2OLE(szGuid), pGuid);
}

HRESULT GUID2S(REFCLSID rclsid, LPTSTR szBuff, size_t buffSize, int* pcBuff)
{
	HRESULT hr;
	LPOLESTR szGuid = NULL;

	hr = StringFromCLSID(rclsid, &szGuid);
	if(SUCCEEDED(hr) && szGuid != NULL)
	{
		int n = static_cast<int>(wcslen(szGuid));
		if(*pcBuff < n)
			hr = HRESULT_FROM_WIN32(ERROR_MORE_DATA);
		else
		{
			_tcsncpy_s(szBuff, buffSize, COLE2CT(szGuid), *pcBuff);
		}
		*pcBuff = n;

		CoTaskMemFree(szGuid);
	}
	return hr;
}

#ifdef MCTRACE
void McTRACE(LPCTSTR pstrFormat, ...)
{
	CString str;

	// format and write the data you were given
	va_list args;
	va_start(args, pstrFormat);

	str.FormatV(pstrFormat, args);
	va_end(args);

	OutputDebugString(str);
}
#else
void McTRACE(...)
{
}
#endif
