//通用方法 By Kyd Yu

#include "StdAfx.h"
//#include <afxinet.h>
#include "tinystr.h"
#include "tinyxml.h"
#include <map>
#include "common_function.h"

#include <WINSOCK2.H>

//#include <IPHlpApi.h>
//#pragma comment(lib, "IPHLPAPI.lib")

CString GetConfigValue(CString& szFileName, CString key)
{
	TiXmlDocument *myDocument=NULL;
	try
	{
		myDocument = new TiXmlDocument(szFileName);
		myDocument->LoadFile();
		if(myDocument->Error())
		{
			return "";
		}
		TiXmlElement* element = myDocument->FirstChildElement( (LPSTR) (LPCTSTR) key);
		CString str = element->FirstChild()->Value();
		delete myDocument;
		return str;
	}
	catch (...)
	{
		if(myDocument != NULL)
		{
			delete myDocument;
		}
		return "";
	}
}

/**
 * GetAdaptersAddresses()，适用于winxp以上，IPV4/6。
 */
/*
bool GAAVersion(CString& macOut)
{
	bool ret = false;

	ULONG ulOutBufLen = sizeof(IP_ADAPTER_INFO);
	PIP_ADAPTER_ADDRESSES paAddr = (IP_ADAPTER_ADDRESSES*) malloc(sizeof(IP_ADAPTER_ADDRESSES));
	if (paAddr == NULL)
		return false;

	//判断是否溢出
	if ( GetAdaptersAddresses(AF_UNSPEC, 0, NULL, paAddr, &ulOutBufLen) == ERROR_BUFFER_OVERFLOW )
	{
		free(paAddr);
		paAddr = (IP_ADAPTER_ADDRESSES*) malloc(ulOutBufLen);
		if (paAddr == NULL)
			return false;
	}

	//开始获得
	if ( GetAdaptersAddresses(AF_UNSPEC, 0, NULL, paAddr, &ulOutBufLen) == NO_ERROR )
	{
		for (PIP_ADAPTER_ADDRESSES paTp = paAddr; paTp != NULL; paTp = paTp->Next)
		{
			//cout << paTp->Description << endl;
		}

		for (PIP_ADAPTER_ADDRESSES paTemp = paAddr; paTemp != NULL; paTemp = paTemp->Next)
		{
			//MAC ADDR GOT 6 UNITS!
			if (paTemp->PhysicalAddressLength != 6)
				continue;
			//TRY GET IT.
			char acMac[32];
			sprintf(acMac, "%02X-%02X-%02X-%02X-%02X-%02X", 
				int (paTemp->PhysicalAddress[0]),
				int (paTemp->PhysicalAddress[1]),
				int (paTemp->PhysicalAddress[2]),
				int (paTemp->PhysicalAddress[3]),
				int (paTemp->PhysicalAddress[4]),
				int (paTemp->PhysicalAddress[5])
				);
			macOut = acMac;
			ret = true;
			break;
		}
	}

	free(paAddr);
	return ret;
}
*/

/**
 * GetAdaptersInfo()，适用于win2000以上，IPV4。
 */
/*
bool GAIVersion(CString& macOut)
{
	bool ret = false;

	ULONG ulOutBufLen = sizeof(IP_ADAPTER_INFO);
	PIP_ADAPTER_INFO paInfo = (IP_ADAPTER_INFO*) malloc(sizeof(IP_ADAPTER_INFO));
	if (paInfo == NULL)
		return false;

	//判断是否溢出
	if ( GetAdaptersInfo(paInfo, &ulOutBufLen) == ERROR_BUFFER_OVERFLOW )
	{
		free(paInfo);
		paInfo = (IP_ADAPTER_INFO*) malloc(ulOutBufLen);
		if (paInfo == NULL)
			return false;
	}

	//开始获得
	if ( GetAdaptersInfo(paInfo, &ulOutBufLen) == NO_ERROR )
	{
		for (PIP_ADAPTER_INFO paTp = paInfo; paTp != NULL; paTp = paTp->Next)
		{
			//cout << paTp->Description << endl;
		}

		for (PIP_ADAPTER_INFO paTemp = paInfo; paTemp != NULL; paTemp = paTemp->Next)
		{
			//ETHERNET?
			if (paTemp->Type != MIB_IF_TYPE_ETHERNET)
				continue;
			//MAC ADDR GOT 6 UNITS!
			if (paTemp->AddressLength != 6)
				continue;
			//TRY GET IT.
			char acMac[32];
			sprintf(acMac, "%02X-%02X-%02X-%02X-%02X-%02X", 
				int (paTemp->Address[0]),
				int (paTemp->Address[1]),
				int (paTemp->Address[2]),
				int (paTemp->Address[3]),
				int (paTemp->Address[4]),
				int (paTemp->Address[5])
				);
			macOut = acMac;
			ret = true;
			break;
		}
	}

	free(paInfo);
	return ret;
}

CString GetMyMacAddr()
{
	CString realMac = "";
	if (GAIVersion(realMac)) //1重
	{
		//char* nani = (LPSTR) (LPCSTR) realMac;
	}
	else if (GAAVersion(realMac)) //2重
	{
		//char* nani = (LPSTR) (LPCSTR) realMac;
	}
	else //失败
	{
		;
	}
	return realMac;
}
*/

//获取配置文件ROOT-游戏程序地址
CString GetConfigFile(CString& szFileName)
{	
	try
	{
		TiXmlDocument *myDocument = new TiXmlDocument(szFileName);
		myDocument->LoadFile();
		if(myDocument->Error())
		{
			return "";
		}
		TiXmlElement * pRoot = myDocument->FirstChildElement();
		return (char*)pRoot->FirstChild()->Value();
	}
	catch (...)
	{
		return "";
	}
}

//获取配置文件URL-游戏页面地址
CString GetUrlFromConfigFile(CString& szFileName)
{
	try
	{
		TiXmlDocument *myDocument = new TiXmlDocument(szFileName);
		myDocument->LoadFile();
		if(myDocument->Error())
		{
			return "";
		}
		TiXmlElement * pRoot = myDocument->FirstChildElement();
		return (char*)pRoot->NextSibling()->FirstChild()->Value();
	}
	catch (...)
	{
		return "";
	}
}

//获取配置文件INFO-统计页面地址
CString GetInfoUrlFromConfigFile(CString& szFileName)
{
	try
	{
		TiXmlDocument *myDocument = new TiXmlDocument(szFileName);
		myDocument->LoadFile();
		if(myDocument->Error())
		{
			return "";
		}
		TiXmlElement * info = myDocument->FirstChildElement("Info");
		CString str = info->FirstChild()->Value();
		return str;
		//TiXmlElement * pRoot = myDocument->FirstChildElement();
		//TiXmlElement * pRoot1 = pRoot->NextSibling();
		//return (char*)pRoot1->NextSibling()->FirstChild()->Value();
		//TiXmlElement * pRoot = myDocument->LastChild();
		//return (char*) myDocument->LastChild()->ReadText();
	}
	catch (...)
	{
		return "";
	}
}

//删除HTTP缓存
BOOL DelInetTempFiles()
{
	BOOL bResult = FALSE;
	BOOL bDone = FALSE;
	LPINTERNET_CACHE_ENTRY_INFO lpCacheEntry = NULL;

	DWORD  dwTrySize, dwEntrySize = 4096; //内存大小
	HANDLE hCacheDir = NULL;
	DWORD  dwError = ERROR_INSUFFICIENT_BUFFER;

	do
	{
		switch (dwError)
		{
		case ERROR_INSUFFICIENT_BUFFER:
			delete [] lpCacheEntry;
			lpCacheEntry = (LPINTERNET_CACHE_ENTRY_INFO) new char[dwEntrySize];
			lpCacheEntry->dwStructSize = dwEntrySize;
			dwTrySize = dwEntrySize;
			BOOL bSuccess;

			if (hCacheDir == NULL)	
				bSuccess = (hCacheDir= FindFirstUrlCacheEntry(NULL, lpCacheEntry,&dwTrySize)) != NULL; //从Internet的高速缓存中找到特定入口
			else //查找下一个缓存入口
				bSuccess = FindNextUrlCacheEntry(hCacheDir, lpCacheEntry, &dwTrySize);

			if (bSuccess)
				dwError = ERROR_SUCCESS;
			else
			{
				dwError = GetLastError();
				dwEntrySize = dwTrySize; // use new size returned
			}
			break;	
		case ERROR_NO_MORE_ITEMS:  //没有可删除的项目
			bDone = TRUE;
			bResult = TRUE;
			break;			
		case ERROR_SUCCESS:	  	
			//不删除cookie
			if (!(lpCacheEntry->CacheEntryType & COOKIE_CACHE_ENTRY))
				DeleteUrlCacheEntry(lpCacheEntry->lpszSourceUrlName);  //删除Cache中指定的源文件名

			// 查找下一个缓存入口
			dwTrySize = dwEntrySize;
			if (FindNextUrlCacheEntry(hCacheDir, lpCacheEntry, &dwTrySize))
				dwError = ERROR_SUCCESS;
			else
			{
				dwError = GetLastError();
				dwEntrySize = dwTrySize; // use new size returned
			}
			break;			
		default:  
			bDone = TRUE;
			break;
		}

		if (bDone)
		{
			delete [] lpCacheEntry;

			if (hCacheDir)
				FindCloseUrlCache(hCacheDir);  //关闭指定的缓存枚举句柄
		}
	} while (!bDone);

	return bResult;
}


float GetProgressNum(int lenghtTotal,int lengthSigle,int count)
{
	return (1.0*lengthSigle*count/lenghtTotal);
}


void CreateDirs(CString& path)
{
	CString temp = path;
	for(int i = 0 ; i < temp.GetLength();++i)
	{
		if(temp[i] == '\\')
		{
			//创建文件目录
			CreateDirectory(temp.Left(i),NULL);
		}	
	}
}

void DeleteDirs(CString& path)
{
    //文件句柄  
    long hFile = 0;  
    //文件信息  
    struct _finddata_t fileinfo;
	CString filePattern = path + "*";
    if((hFile = _findfirst((LPTSTR)(LPCTSTR)filePattern, &fileinfo)) !=  -1)  
    {
        do
        {
            //如果是目录,迭代之
            //如果不是,加入列表
            if((fileinfo.attrib &  _A_SUBDIR))  
            {
				CString subPath = path + fileinfo.name + "\\";
                if(strcmp(fileinfo.name,".") != 0 && strcmp(fileinfo.name,"..") != 0)
				{
                    DeleteDirs(subPath);
				}
            }
            else
            {
				CString fileFullName = path + fileinfo.name;
				DeleteFile(fileFullName);
            }
        }while(_findnext(hFile, &fileinfo)  == 0);
        _findclose(hFile);
		RemoveDirectory(path);
    }
}

void GetFiles(CString& path, vector<CString>& files)
{
    //文件句柄  
    long hFile = 0;
    //文件信息
    struct _finddata_t fileinfo;
	CString filePattern = path + "*";
    if((hFile = _findfirst((LPTSTR)(LPCTSTR)filePattern, &fileinfo)) !=  -1)  
    {
        do  
        {
            //如果是目录,迭代之
            //如果不是,加入列表
            if((fileinfo.attrib &  _A_SUBDIR))  
            {
				CString subPath = path + fileinfo.name + "\\";
                if(strcmp(fileinfo.name,".") != 0 && strcmp(fileinfo.name,"..") != 0)
				{
                    GetFiles(subPath, files); 
				}
            }
            else  
            {
				CString fileFullName = path + fileinfo.name;
                files.push_back(fileFullName);  
            }  
        }while(_findnext(hFile, &fileinfo)  == 0);
        _findclose(hFile);
    }
}

int split(const string& str, vector<string>& ret_, string sep = ",")
{
	if (str.empty())
	{
		return 0;
	}

	std::string tmp;
	std::string::size_type pos_begin = str.find_first_not_of(sep);
	std::string::size_type comma_pos = 0;

	while (pos_begin != std::string::npos)
	{
		comma_pos = str.find(sep, pos_begin);
		if (comma_pos != std::string::npos)
		{
			tmp = str.substr(pos_begin, comma_pos - pos_begin);
			pos_begin = comma_pos + sep.length();
		}
		else
		{
			tmp = str.substr(pos_begin);
			pos_begin = comma_pos;
		}

		if (!tmp.empty())
		{
			ret_.push_back(tmp);
			//tmp.clear();
		}
	}
	return 0;
}

