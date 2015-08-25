#include "stdafx.h"
#include "cdib.h"
#include "windowsx.h"

CDib::CDib(IStream *pStream)
{
	m_fileName[0]       = '\0';
    m_pRGB              = NULL;
    m_pData             = NULL;
	m_pBitmapFileHeader = NULL;
	m_pBitmapInfoHeader = NULL;
	m_pBitmapInfo       = NULL;
	LoadStream(pStream);
}

CDib::CDib(LPCTSTR dibFileName)
{
	m_fileName[0]       = '\0';
    m_pRGB              = NULL;
    m_pData             = NULL;
	m_pBitmapFileHeader = NULL;
	m_pBitmapInfoHeader = NULL;
	m_pBitmapInfo       = NULL;
    _tcscpy(m_fileName, dibFileName);
    LoadFile();
}

CDib::~CDib()
{
    GlobalFreePtr(m_pBitmapInfo);
}

void CDib::LoadStream(IStream *pStr)
{
	BITMAPFILEHEADER bitmapFileHeader;
	ULONG ReadSize = 0;
	HRESULT hr = S_OK;

    hr = pStr->Read((void*)&bitmapFileHeader,sizeof(BITMAPFILEHEADER),&ReadSize);
    if (SUCCEEDED(hr)&&(bitmapFileHeader.bfType == 0x4d42))
    {
		DWORD fileLength = bitmapFileHeader.bfSize;
		DWORD size = fileLength - sizeof(BITMAPFILEHEADER);
        BYTE* pDib = (BYTE*)GlobalAllocPtr(GMEM_MOVEABLE, size);
		hr = pStr->Read((void*)pDib, size,&ReadSize);
		if(SUCCEEDED(hr))
		{
			m_pBitmapInfo = (BITMAPINFO*)pDib;
			m_pBitmapInfoHeader = (BITMAPINFOHEADER*) pDib;
			m_pRGB = (RGBQUAD*)(pDib + m_pBitmapInfoHeader->biSize);
			int m_numberOfColors = GetNumberOfColors();
			if (m_pBitmapInfoHeader->biClrUsed == 0)
				m_pBitmapInfoHeader->biClrUsed = m_numberOfColors;
			DWORD colorTableSize = m_numberOfColors * sizeof(RGBQUAD);
			m_pData = pDib + m_pBitmapInfoHeader->biSize + colorTableSize;
			if (m_pRGB == (RGBQUAD*)m_pData) // No color table
				m_pRGB = NULL;
			m_pBitmapInfoHeader->biSizeImage = GetSize();
			m_valid = TRUE;
			return ;
		}		
    }    
    m_valid = FALSE;
    #ifdef _DEVELOVER_VERSION_L1
      AfxMessageBox(_T("This isn't a bitmap file in Stream!"));
    #endif
}

void CDib::LoadFile()
{
    CFile dibFile(m_fileName, CFile::modeRead);
    
    BITMAPFILEHEADER bitmapFileHeader;
    dibFile.Read((void*)&bitmapFileHeader,
        sizeof(BITMAPFILEHEADER));

    if (bitmapFileHeader.bfType == 0x4d42)
    {
        DWORD fileLength = dibFile.GetLength();    
        DWORD size = fileLength -
			sizeof(BITMAPFILEHEADER);
        BYTE* pDib =
            (BYTE*)GlobalAllocPtr(GMEM_MOVEABLE, size);
        dibFile.Read((void*)pDib, size);
        dibFile.Close();

        m_pBitmapInfo = (BITMAPINFO*) pDib;
        m_pBitmapInfoHeader = (BITMAPINFOHEADER*) pDib;
        m_pRGB = (RGBQUAD*)(pDib +
			m_pBitmapInfoHeader->biSize);
        int m_numberOfColors = GetNumberOfColors();
        if (m_pBitmapInfoHeader->biClrUsed == 0)
            m_pBitmapInfoHeader->biClrUsed =
			    m_numberOfColors;
        DWORD colorTableSize = m_numberOfColors *
            sizeof(RGBQUAD);
        m_pData = pDib + m_pBitmapInfoHeader->biSize
            + colorTableSize;
		if (m_pRGB == (RGBQUAD*)m_pData) // No color table
			m_pRGB = NULL;
        m_pBitmapInfoHeader->biSizeImage = GetSize();
		m_valid = TRUE;
    }    
    else
    {
        m_valid = FALSE;
        #ifdef _DEVELOVER_VERSION_L1
		    AfxMessageBox(_T("This isn't a bitmap file in Stream!"));
        #endif
    }
}

BOOL CDib::IsValid()
{
    return m_valid;
}
        
LPTSTR CDib::GetFileName()
{
    return m_fileName;
}
        
UINT CDib::GetWidth()
{
    return (UINT) m_pBitmapInfoHeader->biWidth;
}
        
UINT CDib::GetHeight()
{
    return (UINT) m_pBitmapInfoHeader->biHeight;
}
        
DWORD CDib::GetSize()
{
    if (m_pBitmapInfoHeader->biSizeImage != 0)
        return m_pBitmapInfoHeader->biSizeImage;
	else
    {
        DWORD height = (DWORD) GetHeight();
        DWORD width = (DWORD) GetWidth();
        return height * width;
    }
}

UINT CDib::GetNumberOfColors()
{
	int numberOfColors;

    if ((m_pBitmapInfoHeader->biClrUsed == 0) &&
          (m_pBitmapInfoHeader->biBitCount < 9))
	{
		switch (m_pBitmapInfoHeader->biBitCount)
		{
		    case 1: numberOfColors = 2; break;
		    case 4: numberOfColors = 16; break;
		    case 8: numberOfColors = 256;
		}
	}
    else
		numberOfColors = (int) m_pBitmapInfoHeader->biClrUsed;

    return numberOfColors;
}
    
BYTE* CDib::GetData()
{
    return m_pData;
}

RGBQUAD* CDib::GetRGB()
{
    return m_pRGB;
}

BITMAPINFO* CDib::GetInfo()
{
    return m_pBitmapInfo;
}


HBITMAP CDib::GetHBITMAP(HDC hdc)
{
	if(m_valid)
		return  CreateDIBitmap(hdc,m_pBitmapInfoHeader,
		        CBM_INIT,(VOID *)m_pData,m_pBitmapInfo, DIB_RGB_COLORS);
	else
		return NULL;
}

