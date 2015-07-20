#pragma once

#include <tchar.h>
#include <atlbase.h>
#include <atltime.h>
#include "atlcrypt.h" // http://www.codeplex.com/AtlServer

extern const LPCTSTR sProductsKeyName;
extern const LPCTSTR sLicensesKeyTemplate;
extern const LPCTSTR sSecretKey;

#define BUILD_LICENCE_FLAG(IsUnlimitedTime, IsDurationTime, CheckHdd) (DWORD)(((IsUnlimitedTime)?1:0)+(((IsDurationTime)?1:0)<<1)+ (((CheckHdd)?1:0)<<2))

#define E_LICENSE_EXPIRED MAKE_HRESULT(1,FACILITY_SECURITY,0xFF)
#define E_LICENSE_WRONG_PC MAKE_HRESULT(1,FACILITY_SECURITY,0xFE)

typedef struct _McActivatorClientPacket2
{
	DWORD version;
	GUID productGuid;
	GUID licenseGuid;
	DWORD hddSerialNumber;
}
McActivatorClientPacket2, *PMcActivatorClientPacket2;

typedef struct _McActivatorPacket
{
	DWORD	cbSize;				//	sizeof(_McActivatorPacket) struct
	DWORD	dwHeaderSize;		//	Header Size
	DWORD	dwHeaderOffset;		//	Header Pointer = Pointer + cbSize + dwHeaderOffset;
	DWORD	dwXmlSize;			//	XML Size
	DWORD	dwXmlOffset;		//	XML Pointer = Pointer + cbSize + dwXmlOffset;
} 
McActivatorPacket, *PMcActivatorPacket;

typedef struct _McCryptPacket
{
	DWORD	cbSize;				//	sizeof(_McCryptPacket) struct
	DWORD	dwDataSize;			//	Data Size
	DWORD	pDataOffset;		//	Real Data Pointer = Pointer + cbSize + pDataOffset;
	DWORD	dwSignDataSize;		//	Sign Data Size
	DWORD	pSignDataOffset;	//	Real Sign Data Pointer = Pointer + cbSize + pSignDataOffset;
} 
McCryptPacket, *PMcCryptPacket;

typedef struct _McLicencePacket
{
	DWORD	dwProtocolVersion;	//2
	DWORD	dwFlags;			//packet.dwFlags: 1 - Unlimited Time (1,N0), 2b - (ToTime - 0, Duration - 1), 3 bit - USEHDD
	GUID	ProductGUID;
	GUID	LicenseGUID;
	DWORD	dwVolumeSerialNumber;
	DWORD	dwTime;
	BYTE	LicenseFileMD5HashBuff[30];
	DWORD	dwLicenseFileMD5HashBuffSize;

} McLicencePacket, *PMcLicencePacket;

typedef struct _McClientPacket
{
	DWORD	dwProtocolVersion;	//2
	GUID	ProductGUID;
	GUID	LicenseGUID;
	DWORD	dwVolumeSerialNumber;
} McClientPacket, *PMcClientPacket;


HRESULT GetSysVolumeSerialNumber(OUT LPDWORD lpVolumeSerialNumber);

WORD Time2Word(const CTime &time);
CTime Word2Time(const WORD &value);

WORD McCoCRC16(const LPBYTE pBuffer, DWORD dwBufferSize);

HRESULT CreateDacl(OUT LPBYTE* ppData, OUT LPDWORD pcbData);
HRESULT CreateSecurityDescriptor(OUT PSECURITY_DESCRIPTOR* ppSd, OUT LPBYTE* ppDacl);
HRESULT McInitializeCryptProv(IN CCryptProv* pCriptProv, IN LPCTSTR szContainer);

HRESULT McGetFileMD5Hash(CCryptProv &hCriptProv, LPCTSTR File, BYTE* pBuffer, DWORD* pdwBufferSize);

HRESULT McCoStringToGuid(IN LPCTSTR strGuid,OUT GUID *pGuid);

LPTSTR McConcatStrings(LPCTSTR sz1, LPCTSTR sz2);
void McDelString(LPTSTR sz);
HKEY McRegOpenSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired);
HKEY McRegCreateSubKey(HKEY hKey, LPCTSTR szSubKey, REGSAM samDesired);
LPTSTR McRegGetString(HKEY hKey, LPCTSTR szSubKey, LPCTSTR szEntry, LPTSTR szDefault);

HRESULT S2GUID(IN LPCTSTR szGuid, OUT LPGUID guid);
HRESULT GUID2S(REFCLSID rclsid, LPTSTR szBuff, size_t buffSize, int* pcBuff);

#ifdef MCTRACE
void McTRACE(LPCTSTR pstrFormat, ...);
#else
void McTRACE(...);
#endif

#ifdef MC_SERVER_SIDE
HRESULT McCoCreateGuid(IN GUID ProductGuid, OUT GUID *pGuid);
HRESULT McCreateProductKey(IN const GUID *pGuid, IN OUT LPTSTR szProductKey, IN OUT int *pctProductKey);
HRESULT McCryptSignData(CCryptProv &hCriptProv,IN LPBYTE pbData, IN DWORD dwDataSize, OUT LPBYTE *ppbSignData, OUT DWORD *pdwSignDataSize);
HRESULT McCreateLicenseData(CCryptProv &hCriptProv, GUID ProductGUID, GUID LicenseGUID, DWORD dwFlags, DWORD dwVolumeSerialNumber, DWORD dwTime, LPCTSTR LicenseFile, OUT LPBYTE *ppbSignData, OUT DWORD *pdwSignDataSize);
HRESULT McCreateLicenseData(CCryptProv &hCriptProv, GUID ProductGUID, GUID LicenseGUID, DWORD dwFlags, DWORD dwVolumeSerialNumber, DWORD dwTime, BYTE* LicenseData, DWORD dwLicenseDataSize, OUT LPBYTE *ppbSignData, OUT DWORD *pdwSignDataSize);
HRESULT UnpackClientPacket(IN LPCTSTR szClientData, OUT PMcClientPacket pPacket);
#endif

#ifdef MC_CLIENT_SIDE
enum InstResult
{
 INST_OK,
 INST_WrongData,
 INST_NotInstalled
};

struct TrialInfo
{
 DWORD Version;
 WORD Random;
 WORD Date;
};

HRESULT McVerifySerialNumber(IN GUID ProductGuid, IN LPCTSTR szSerialNumber, OUT LPGUID pLicenseGuid);
HRESULT McVerifyLicenseData(CCryptProv &hCriptProv, GUID ProductGUID, LPCTSTR LicenseFile, HKEY keyParent,  LPCTSTR RegDatePath, IN LPBYTE pbSignData, IN DWORD dwSignDataSize, OUT DWORD* pExpirationDate);
HRESULT McVerifyLicenseData(CCryptProv &hCriptProv, IN LPBYTE pbSignData, IN DWORD dwSignDataSize, GUID *ProductGUID, GUID *LicenseGUID);
HRESULT McVerifyLicenseData(CCryptProv &hCriptProv, IN LPGUID pProductGUID, IN LPBYTE pHeader, IN DWORD cbHeader, IN LPBYTE pXml, IN DWORD cbXml, OUT DWORD* pExpirationDate);
LPTSTR GetSecretKey(LPCTSTR szLicenseGuid);
InstResult InstallDateGetCurrent(LPWORD pwDate, LPCTSTR szSecretValue);
InstResult InstallDateGetFromRegistry(LPWORD pwDate, LPCTSTR szSecretValue);
LPTSTR GetLicenseFileName(LPCTSTR szLicenseGuid);
#endif