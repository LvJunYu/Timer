#include "LogUtil.h"

#include "StdAfx.h"
#include <stdio.h>
#include <time.h>

#define MAX_SIZE 256

int WriteLog(const char* msg)
{
	char szExePath[MAX_SIZE];
	GetCurrentDirectory(MAX_SIZE,szExePath);
	strcat(szExePath,"\\syj.log");

	FILE* fp;
	fp = fopen(szExePath,"a+");
	if(!fp)
	{
		fprintf(stderr,"Open File Failed %s!",szExePath);
		return -1;
	}

	time_t mytime;
	struct tm* mytm;
	char szTime[MAX_SIZE];
	memset(szTime,'\0',MAX_SIZE);
	mytime = time(NULL);
	mytm = localtime(&mytime);
	sprintf(szTime,"%d-%d-%d %d:%d:%d",mytm->tm_year+1900,mytm->tm_mon+1,mytm->tm_mday,mytm->tm_hour,mytm->tm_min,mytm->tm_sec);

	char szBuf[MAX_SIZE*2];
	memset(szBuf,'\0',MAX_SIZE*2);
	sprintf(szBuf, "\n%s %s", szTime, msg);

	int ret = fwrite(szBuf, 1, strlen(szBuf), fp);
	if(ret < 0)
	{
		fprintf(stderr,"Write File Failed.\n");
		return -1;
	}

	fclose(fp);

	return 0;
}