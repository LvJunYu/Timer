//通用方法 By KydYu
#include "tinystr.h"
#include "tinyxml.h"
#include <wininet.h>
#include <vector>
using namespace std;
#pragma comment(lib,"wininet.lib")


void ReadXmlFile(CString& szFileName,vector<CString> &fileContainer,vector<CString> &md5Container);

//获得配置xml中key对应的值
CString GetConfigValue(CString& szFileName, CString key);

//获得本机的合法MAC地址
CString GetMyMacAddr();

//使用GetAdaptersAddresses()，适用于winxp以上，IPV4/6。
bool GAAVersion(CString& macOut);

//使用GetAdaptersInfo()，适用于win2000以上，IPV4。
bool GAIVersion(CString& macOut);

BOOL DelInetTempFiles();

float GetProgressNum(int lenghtTotal,int lengthSigle,int count);

//获取配置文件ROOT-游戏程序地址
CString GetConfigFile(CString& szFileName);

//获取配置文件URL-游戏页面地址
CString GetUrlFromConfigFile(CString& szFileName);

//获取配置文件INFO-统计页面地址
CString GetInfoUrlFromConfigFile(CString& szFileName);

//获取服务器版本
static void ReadSerXml(CString& szFileName,CString key);
