// /********************************************************************
// ** Filename : EnumStringDefine.cs
// ** Author : quan
// ** Date : 16/5/23 下午4:22
// ** Summary : EnumStringDefine.cs
// ***********************************************************************/
using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class EnumStringDefine
    {
        public EnumStringDefine()
        {
        }
        public static string GetSexString(ESex sex)
        {
            if(sex == ESex.S_Female)
            {
                return "女";
            }
            else if( sex == ESex.S_Male)
            {
                return "男";
            }
            return "";
        }

//        public static string GetProjectCategoryString(EProjectCategory projectCategory)
//        {
//            if(projectCategory == EProjectCategory.PC_Relaxation)
//            {
//                return "休闲";
//            }
//            else if(projectCategory == EProjectCategory.PC_Puzzle)
//            {
//                return "解谜";
//            }
//            else if(projectCategory == EProjectCategory.PC_Challenge)
//            {
//                return "极限";
//            }
//            else
//            {
//                return "未知";
//            }
//        }
    }
}

