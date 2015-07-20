// PasswordUtil.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <windows.h>
#include <Wincrypt.h>
#include <atlenc.h>
#include "McLicenseVerify\mcactivation.h"
#include "EventLog.h"

HRESULT PasswordUtil_Check(LPCWSTR password, LPCWSTR salt, LPCWSTR passwordHash, BOOL* pResult)
{
	if(IsBadStringPtrW(password,512))
		return E_INVALIDARG;
	if(IsBadStringPtrW(salt,512))
		return E_INVALIDARG;
	if(IsBadStringPtrW(passwordHash,512))
		return E_INVALIDARG;

	if(IsBadWritePtr(pResult,sizeof(BOOL*)))
		return E_INVALIDARG;

	*pResult = FALSE;

	HRESULT hr = S_OK;

	HCRYPTPROV hProv = NULL;
	HCRYPTHASH hHash = NULL;

	WCHAR saltAndPwd[1024] = L"";

	wcsncat_s(saltAndPwd, 1024, password, 512);
	wcsncat_s(saltAndPwd, 1024, L"$",1);
	wcsncat_s(saltAndPwd, 1024, salt, 511);

	DWORD dwFlags = CRYPT_SILENT|CRYPT_MACHINE_KEYSET;
	TCHAR szContainer[50] = _T("{BDECD56B-6D48-4add-9AEE-265D537408DF}");

	CString auditMessage;

	BOOL bCreated = FALSE;

	if(!CryptAcquireContext(&hProv, szContainer, MS_ENHANCED_PROV, PROV_RSA_FULL, dwFlags))
	{
		hr = HRESULT_FROM_WIN32(GetLastError());

		if(hr==0x80090016L) // Key Not Found
		{
			bCreated = TRUE;

			if(!CryptAcquireContext(&hProv, szContainer, MS_ENHANCED_PROV, PROV_RSA_FULL, CRYPT_NEWKEYSET|dwFlags))
			{
				hr = HRESULT_FROM_WIN32(GetLastError());

				if(hr == 0x8009000FL) // Key Exists
				{
					if(CryptAcquireContext(&hProv, szContainer, MS_ENHANCED_PROV, PROV_RSA_FULL, CRYPT_DELETEKEYSET|dwFlags))
					{
						hr = S_OK;

						if(!CryptAcquireContext(&hProv, szContainer, MS_ENHANCED_PROV, PROV_RSA_FULL, CRYPT_NEWKEYSET|dwFlags))
						{
							hr = HRESULT_FROM_WIN32(GetLastError());
						}
						else
						{
							hr = S_OK;
						}
					}
					else
					{
						hr = HRESULT_FROM_WIN32(GetLastError());
					}
				}
				else
				{
					hr = S_OK;
				}
			}
			else
			{
				hr = S_OK;
			}
		}
	}

	if(SUCCEEDED(hr))
	{
		if(bCreated)
		{
			// Set DACL for this container to allow full control for everyone and for local system.
			PSECURITY_DESCRIPTOR pSd = NULL;
			LPBYTE pbDacl = NULL;

			HRESULT hr2 = CreateSecurityDescriptor(&pSd, &pbDacl);
			if(SUCCEEDED(hr2))
			{
				CryptSetProvParam(hProv, PP_KEYSET_SEC_DESCR, reinterpret_cast<LPBYTE>(pSd), DACL_SECURITY_INFORMATION);
				delete pSd;
				delete[] pbDacl;
			}
		}

		if(CryptCreateHash(hProv, CALG_MD5, 0, 0, &hHash))
		{
			//auditMessage.Format(_T("Step 2-2. Error Code: 0x%X."), hr);
			//CEventLog::AddAppLog(auditMessage, FAILED_LOGIN, EVENTLOG_WARNING_TYPE);

			if(CryptHashData(hHash, (BYTE*)saltAndPwd, (DWORD)(wcslen(saltAndPwd))*2, 0))
			{
				//auditMessage.Format(_T("Step 2-3. Error Code: 0x%X."), hr);
				//CEventLog::AddAppLog(auditMessage, FAILED_LOGIN, EVENTLOG_WARNING_TYPE);

				BYTE szData[50] = {0};
				DWORD dwDataLen = 50;

				if(CryptGetHashParam(hHash, HP_HASHVAL, szData, &dwDataLen, 0))
				{
					/*auditMessage.Format(_T("Step 2-4. Error Code: 0x%X."), hr);
					CEventLog::AddAppLog(auditMessage, FAILED_LOGIN, EVENTLOG_WARNING_TYPE);*/

					CW2A ansiPasswordHash(passwordHash);

					int passwordHashLen = static_cast<int>(strlen(ansiPasswordHash));
					int nDestLen = Base64DecodeGetRequiredLength(passwordHashLen);

					CHeapPtr<BYTE> dataBuffer;
					if(dataBuffer.AllocateBytes(nDestLen))
					{
						//auditMessage.Format(_T("Step 2-5. Error Code: 0x%X."), hr);
						//CEventLog::AddAppLog(auditMessage, FAILED_LOGIN, EVENTLOG_WARNING_TYPE);

						if(Base64Decode(ansiPasswordHash, passwordHashLen, dataBuffer, &nDestLen))
						{
							size_t testHashLength = static_cast<size_t>(dwDataLen);
							size_t validHashLength = static_cast<size_t>(nDestLen);

							*pResult = (testHashLength == validHashLength && (memcmp(szData, dataBuffer, testHashLength) == 0));
						}
					}
					else
						hr = E_OUTOFMEMORY;
				}
				else
					hr = HRESULT_FROM_WIN32(GetLastError());
			}
			else
				hr = HRESULT_FROM_WIN32(GetLastError());

			CryptDestroyHash(hHash);
		}
		else
			hr = HRESULT_FROM_WIN32(GetLastError());

		CryptReleaseContext(hProv, 0);
	}
	//else
	//	hr = HRESULT_FROM_WIN32(GetLastError());

	//auditMessage.Format(_T("Step Final. Error Code: 0x%X."), hr);
	//CEventLog::AddAppLog(auditMessage, FAILED_LOGIN, EVENTLOG_WARNING_TYPE);

	return hr;
}

//void _tmain(int argc, _TCHAR* argv[])
//{
//	BOOL bResult = FALSE;
//	HRESULT hr= PasswordUtil_Check(L"password", L"RTEFnd8=", L"Ej/litBXv/DFzMXErBIVIw==", &bResult);
//
//	return;
//}
//
