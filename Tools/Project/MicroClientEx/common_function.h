//ͨ�÷��� By KydYu
#include "tinystr.h"
#include "tinyxml.h"
#include <wininet.h>
#include <vector>
using namespace std;
#pragma comment(lib,"wininet.lib")


void ReadXmlFile(CString& szFileName,vector<CString> &fileContainer,vector<CString> &md5Container);

//�������xml��key��Ӧ��ֵ
CString GetConfigValue(CString& szFileName, CString key);

//��ñ����ĺϷ�MAC��ַ
CString GetMyMacAddr();

//ʹ��GetAdaptersAddresses()��������winxp���ϣ�IPV4/6��
bool GAAVersion(CString& macOut);

//ʹ��GetAdaptersInfo()��������win2000���ϣ�IPV4��
bool GAIVersion(CString& macOut);

BOOL DelInetTempFiles();

float GetProgressNum(int lenghtTotal,int lengthSigle,int count);

//��ȡ�����ļ�ROOT-��Ϸ�����ַ
CString GetConfigFile(CString& szFileName);

//��ȡ�����ļ�URL-��Ϸҳ���ַ
CString GetUrlFromConfigFile(CString& szFileName);

//��ȡ�����ļ�INFO-ͳ��ҳ���ַ
CString GetInfoUrlFromConfigFile(CString& szFileName);

//��ȡ�������汾
static void ReadSerXml(CString& szFileName,CString key);
