/********************************************************************
** Filename : CheckTools
** Author : quansiwei
** Date : 16/4/11 下午12:34:05
** Summary : CheckTools
***********************************************************************/

using System;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

namespace SoyEngine
{
    public class CheckTools
    {
        public enum ECheckNickNameResult {
            None, Success, TooShort, TooLong, IllegalCharacter, Duplication
        }
        public static ECheckNickNameResult CheckNickName(String nickName) {
            if(string.IsNullOrEmpty(nickName)) {
                return ECheckNickNameResult.TooShort;
            }
            if(nickName.Length < SoyConstDefine.MinNickNameLength) {
                return ECheckNickNameResult.TooShort;
            }
            if(nickName.Length > SoyConstDefine.MaxNickNameLength) {
                return ECheckNickNameResult.TooLong;
            }
            //中文英文字母下划线减号

            if(!Regex.IsMatch(nickName, "^[\\w\\d\u4E00-\u9FFF_-]+$")) {
                return ECheckNickNameResult.IllegalCharacter;
            }
            if(nickName.StartsWith(SoyConstDefine.NickNamePrefix)) {
                return ECheckNickNameResult.Duplication;
            }
            return ECheckNickNameResult.Success;
        }
        
        public enum ECheckProfileResult {
            None, Success, TooLong, IllegalCharacter
        }
        public static ECheckProfileResult CheckProfile(String profile) {
            if(profile.Length > SoyConstDefine.MaxProfileLength) {
                return ECheckProfileResult.TooLong;
            }
            //中文英文字母下划线减号
            if(!Regex.IsMatch(profile, "^[\\w\\d\u4E00-\u9FFF_-]+$")) {
                return ECheckProfileResult.IllegalCharacter;
            }
            return ECheckProfileResult.Success;
        }
        
        public enum ECheckUserNameResult
        {
            None,
            Success,
            TooShort,
            TooLong,
            IllegalCharacter,
        }

        public static ECheckUserNameResult CheckUserName(string userName)
        {
            return ECheckUserNameResult.Success;
        }

        public static bool CheckPhoneNum(string phoneNum)
        {
            if (string.IsNullOrEmpty(phoneNum))
            {
                LogHelper.Debug("1");
                return false;
            }
            if (phoneNum.Length != 11)
            {
                LogHelper.Debug("2, "+phoneNum.Length);
                return false;
            }
            if (phoneNum[0] != '1')
            {
                LogHelper.Debug("3, " + phoneNum[0]);
                return false;
            }
            if(!phoneNum.All(char.IsNumber))
            {
                phoneNum.All(c =>
                {
                    LogHelper.Debug("c: {0}, isNumber: {1}", c, char.IsNumber(c));
                    return char.IsNumber(c);
                });
                return false;
            }
            return true;
        }

        public enum ECheckPasswordResult
        {
            None,
            Success,
            TooShort,
            TooLong,
            IllegalCharacter,
            TooSimple,
        }

        public static ECheckPasswordResult CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password)|| password.Length < SoyConstDefine.MinPasswordLength)
            {
                return ECheckPasswordResult.TooShort;
            }
            if (password.Length > SoyConstDefine.MaxPasswordLength)
            {
                return ECheckPasswordResult.TooLong;
            }
            return ECheckPasswordResult.Success;
        }

        public static bool CheckVerificationCode(string vCode)
        {
            if (string.IsNullOrEmpty(vCode))
            {
                return false;
            }
            if (vCode.Length != 6)
            {
                return false;
            }
            return true;
        }
    }
}