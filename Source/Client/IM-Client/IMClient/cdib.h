#ifndef __CDIB_H
#define __CDIB_H

class CDib : public CObject
{
protected:
	TCHAR m_fileName[256];
    RGBQUAD* m_pRGB;
    BYTE* m_pData;
	BOOL m_valid;
    BITMAPFILEHEADER* m_pBitmapFileHeader;
    BITMAPINFOHEADER* m_pBitmapInfoHeader;
    BITMAPINFO* m_pBitmapInfo;
    
public:
	HBITMAP GetHBITMAP(HDC hdc);
	CDib(IStream *pStream);
    CDib(LPCTSTR dibFileName);
    ~CDib();

    LPTSTR GetFileName();
    BOOL IsValid();
    DWORD GetSize();
    UINT GetWidth();
    UINT GetHeight();
    UINT GetNumberOfColors();
    RGBQUAD* GetRGB();
    BYTE* GetData();
    BITMAPINFO* GetInfo();

protected:
	void LoadStream(IStream *pStr);
    void LoadFile();
};

#endif
