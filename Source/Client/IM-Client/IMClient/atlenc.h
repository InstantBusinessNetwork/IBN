// This is a part of the Active Template Library.
// Copyright (C) 1996-2001 Microsoft Corporation
// All rights reserved.
//
// This source code is only intended as a supplement to the
// Active Template Library Reference and related
// electronic documentation provided with the library.
// See these sources for detailed information regarding the
// Active Template Library product.

#ifndef __ATLENC_H__
#define __ATLENC_H__

#pragma once

//#include <atlbase.h>
#include <stdio.h>

namespace ATL {

//Not including CRLFs
//NOTE: For BASE64 and UUENCODE, this actually
//represents the amount of unencoded characters
//per line
#define ATLSMTP_MAX_QP_LINE_LENGTH       76
#define ATLSMTP_MAX_BASE64_LINE_LENGTH   57
#define ATLSMTP_MAX_UUENCODE_LINE_LENGTH 45


//=======================================================================
// Base64Encode/Base64Decode
// compliant with RFC 2045
//=======================================================================
//
#define ATL_BASE64_FLAG_NONE	0
#define ATL_BASE64_FLAG_NOPAD	1
#define ATL_BASE64_FLAG_NOCRLF  2

inline int Base64EncodeGetRequiredLength(int nSrcLen, DWORD dwFlags=ATL_BASE64_FLAG_NONE) throw()
{
	int nRet = nSrcLen*4/3;

	if ((dwFlags & ATL_BASE64_FLAG_NOPAD) == 0)
		nRet += nSrcLen % 3;

	int nCRLFs = nRet / 76 + 1;
	int nOnLastLine = nRet % 76;

	if (nOnLastLine)
	{
		if (nOnLastLine % 4)
			nRet += 4-(nOnLastLine % 4);
	}

	nCRLFs *= 2;

	if ((dwFlags & ATL_BASE64_FLAG_NOCRLF) == 0)
		nRet += nCRLFs;

	return nRet;
}

inline int Base64DecodeGetRequiredLength(int nSrcLen) throw()
{
	return nSrcLen;
}

inline BOOL Base64Encode(
	const BYTE *pbSrcData,
	int nSrcLen,
	LPTSTR szDest,
	int *pnDestLen,
	DWORD dwFlags=ATL_BASE64_FLAG_NONE) throw()
{
	static const char s_chBase64EncodingTable[64] = {
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q',
		'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g',	'h',
		'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y',
		'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };

	if (!pbSrcData || !szDest || !pnDestLen)
	{
		return FALSE;
	}

	ATLASSERT(*pnDestLen >= Base64EncodeGetRequiredLength(nSrcLen, dwFlags));

	int nWritten( 0 );
	int nLen1( (nSrcLen/3)*4 );
	int nLen2( nLen1/76 );
	int nLen3( 19 );

	for (int i=0; i<=nLen2; i++)
	{
		if (i==nLen2)
			nLen3 = (nLen1%76)/4;

		for (int j=0; j<nLen3; j++)
		{
			DWORD dwCurr(0);
			for (int n=0; n<3; n++)
			{
				dwCurr |= *pbSrcData++;
				dwCurr <<= 8;
			}
			for (int k=0; k<4; k++)
			{
				BYTE b = (BYTE)(dwCurr>>26);
				*szDest++ = s_chBase64EncodingTable[b];
				dwCurr <<= 6;
			}
		}
		nWritten+= nLen3*4;

		if ((dwFlags & ATL_BASE64_FLAG_NOCRLF)==0)
		{
			*szDest++ = '\r';
			*szDest++ = '\n';
			nWritten+= 2;
		}
	}

	if (nWritten && (dwFlags & ATL_BASE64_FLAG_NOCRLF)==0)
	{
		szDest-= 2;
		nWritten -= 2;
	}

	nLen2 = nSrcLen%3 ? nSrcLen%3 + 1 : 0;
	if (nLen2)
	{
		DWORD dwCurr(0);
		for (int n=0; n<3; n++)
		{
			if (n<(nSrcLen%3))
				dwCurr |= *pbSrcData++;
			dwCurr <<= 8;
		}
		for (int k=0; k<nLen2; k++)
		{
			BYTE b = (BYTE)(dwCurr>>26);
			*szDest++ = s_chBase64EncodingTable[b];
			dwCurr <<= 6;
		}
		nWritten+= nLen2;
		if ((dwFlags & ATL_BASE64_FLAG_NOPAD)==0)
		{
			nLen3 = nLen2 ? 4-nLen2 : 0;
			for (int j=0; j<nLen3; j++)
			{
				*szDest++ = '=';
			}
			nWritten+= nLen3;
		}
	}

	*pnDestLen = nWritten;
	return TRUE;
}

inline int DecodeBase64Char(unsigned int ch) throw()
{
	// returns -1 if the character is invalid
	// or should be skipped
	// otherwise, returns the 6-bit code for the character
	// from the encoding table
	if (ch >= 'A' && ch <= 'Z')
		return ch - 'A' + 0;	// 0 range starts at 'A'
	if (ch >= 'a' && ch <= 'z')
		return ch - 'a' + 26;	// 26 range starts at 'a'
	if (ch >= '0' && ch <= '9')
		return ch - '0' + 52;	// 52 range starts at '0'
	if (ch == '+')
		return 62;
	if (ch == '/')
		return 63;
	return -1;
}

inline BOOL Base64Decode(LPCSTR szSrc, int nSrcLen, BYTE *pbDest, int *pnDestLen) throw()
{
	// walk the source buffer
	// each four character sequence is converted to 3 bytes
	// CRLFs and =, and any characters not in the encoding table
	// are skiped

	if (!szSrc || !pbDest || !pnDestLen)
	{
		return FALSE;
	}

	LPCSTR szSrcEnd = szSrc + nSrcLen;
	int nWritten = 0;
	while (szSrc < szSrcEnd)
	{
		DWORD dwCurr = 0;
		int i;
		int nBits = 0;
		for (i=0; i<4; i++)
		{
			if (szSrc >= szSrcEnd)
				break;
			int nCh = DecodeBase64Char(*szSrc);
			szSrc++;
			if (nCh == -1)
			{
				// skip this char
				i--;
				continue;
			}
			dwCurr <<= 6;
			dwCurr |= nCh;
			nBits += 6;
		}
		// dwCurr has the 3 bytes to write to the output buffer
		// left to right
		dwCurr <<= 24-nBits;
		for (i=0; i<nBits/8; i++)
		{
			*pbDest = (BYTE) ((dwCurr & 0x00ff0000) >> 16);
			dwCurr <<= 8;
			pbDest++;
			nWritten++;
		}
	}

	*pnDestLen = nWritten;
	return TRUE;
}

TCHAR PROXY_AUTH_STRING_BEGIN[] = _T("Proxy-Authorization: Basic ");

int	ProxyAuthorizationStringGetRequiredLength(LPCTSTR strUserLogin, LPCTSTR strUserPassword)
{
	if(IsBadStringPtr(strUserLogin,1024)||
		IsBadStringPtr(strUserPassword,1024))
	{
		return -1;
	}
	
	int	HashStringLength  = _tcslen(strUserLogin) + _tcslen(strUserPassword) + 1;

	return 	_tcslen(PROXY_AUTH_STRING_BEGIN) + ATL::Base64EncodeGetRequiredLength(HashStringLength) + 3 /*\r\n\0*/;
}	

HRESULT	ProxyAuthorizationString(LPCTSTR strUserLogin, LPCTSTR strUserPassword, LPTSTR OutBuffer, int *pBufferSize)
{
	if(IsBadReadPtr(pBufferSize,sizeof(int))||
		IsBadStringPtr(strUserLogin,1024)||
		IsBadStringPtr(strUserPassword,1024)||
		IsBadWritePtr(OutBuffer,*pBufferSize))
	{
		return E_INVALIDARG;
	}
	
	int	HashStringLength  = _tcslen(strUserLogin) + _tcslen(strUserPassword) + 1;
	int	StringLength = 	ProxyAuthorizationStringGetRequiredLength(strUserLogin, strUserPassword);

	if(*pBufferSize<StringLength)
	{
		*pBufferSize =  StringLength;
		return E_OUTOFMEMORY;
	}
	
	BYTE	*pUnionString	=	new BYTE[HashStringLength];
	ZeroMemory(pUnionString,HashStringLength);

	memcpy(pUnionString, strUserLogin, _tcslen(strUserLogin));
	memcpy(pUnionString+_tcslen(strUserLogin), ":", _tcslen(strUserLogin));
	memcpy(pUnionString+_tcslen(strUserLogin)+1, strUserPassword, _tcslen(strUserPassword));
	
	_tcscpy(OutBuffer, PROXY_AUTH_STRING_BEGIN);
	int Base64BufferLen = *pBufferSize - _tcslen(PROXY_AUTH_STRING_BEGIN);

	BOOL bRetVal = ATL::Base64Encode(pUnionString, HashStringLength, OutBuffer+_tcslen(PROXY_AUTH_STRING_BEGIN), &Base64BufferLen);

	_tcscat (OutBuffer, _T("\r\n"));

	delete[] pUnionString;
	
	return bRetVal?S_OK:E_FAIL;
}

} // namespace ATL

#endif // __ATLENC_H__
