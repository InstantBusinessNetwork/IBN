// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the MCLICENSEVERIFY_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// MCLICENSEVERIFY_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef NO_MCLICENSEVERIFY_API
#define MCLICENSEVERIFY_API
#else
#ifdef MCLICENSEVERIFY_EXPORTS
#define MCLICENSEVERIFY_API __declspec(dllexport)
#else
#define MCLICENSEVERIFY_API __declspec(dllimport)
#endif
#endif

/*
// This class is exported from the McLicenseVerify.dll
class MCLICENSEVERIFY_API CMcLicenseVerify {
public:
	CMcLicenseVerify(void);
	// TODO: add your methods here.
};

extern MCLICENSEVERIFY_API int nMcLicenseVerify;

MCLICENSEVERIFY_API int fnMcLicenseVerify(void);
*/

MCLICENSEVERIFY_API int GetLicenseDataSize(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	IN OUT DWORD* pcbLicenseData);

MCLICENSEVERIFY_API int GetLicenseData(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	IN LPCTSTR szContainer,
	OUT LPBYTE pLicenseData,
	IN OUT DWORD* pcbLicenseData);

MCLICENSEVERIFY_API int GetLicenseData2(
	IN LPCTSTR szLicenseFile,
	IN LPCTSTR szProductGuid,
	IN LPCTSTR szContainer,
	OUT LPBYTE pLicenseData,
	IN OUT DWORD* pcbLicenseData,
	OUT DWORD* pExpirationDate);
