
using UnityEngine;
using System.Runtime.InteropServices;



public class UMPushAlias  {

	public const string Sina   =   @"SINA_WEIBO";
	//腾讯微博
	public const string Tencent  = @"TENCENT_WEIBO";
	//QQ
	public const string QQ    =    @"QQ";
	//微信
	public const string WeiXin  =  @"WEIXIN";
	//百度
	public const string Baidu   =  @"BAIDU";
	//人人网
	public const string RenRen  =  @"RENREN";
	//开心网
	public const string Kaixin  =  @"KAIXIN";
	//豆瓣
	public const string Douban  =  @"DOUBAN";
	//facebook
	public const string Facebook = @"FACEBOOK";
	//twitter
	public const string Twitter  = @"TWITTER";
	//None
	public const string None  =    @"NONE";

}


#if  UNITY_ANDROID

 

public class UMPushAndroid  {

	

	public class Result
	{
		public int remain;
		public string status;
		public string errors;
		public string jsonString;
	}

	static AndroidJavaObject mPushAgent;
	static AndroidJavaObject tagMananger;
	static AndroidJavaObject context;

	static UMPushAndroid()
	{
#if UNITY_EDITOR
		return;
#else
		var PushAgent =new AndroidJavaClass ("com.umeng.message.PushAgent");
		context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		mPushAgent = PushAgent.CallStatic<AndroidJavaObject> ("getInstance",context);
		tagMananger = mPushAgent.Call<AndroidJavaObject> ("getTagManager");
#endif
	}


	static Result JavaObject2CSharpObject(AndroidJavaObject result)
	{
		var returnVal = new Result();

		returnVal.jsonString = result.Get<string>("jsonString");
		returnVal.remain = result.Get<int>("remain");
		returnVal.errors = result.Get<string>("errors");
		returnVal.status = result.Get<string>("status");

		return returnVal;
	}

	//统计应用启动
	static public void onAppStart ()
	{
#if UNITY_EDITOR
		return;
#else
		mPushAgent.Call ("onAppStart");
#endif
	}
	
	//开启推送
	static public void enable ()
	{
#if UNITY_EDITOR
		return;
#else
		mPushAgent.Call ("enable");
#endif
	}
	//关闭推送
	static public void disable ()
	{
#if UNITY_EDITOR
		return;
#else
		mPushAgent.Call ("disable");
#endif
	}
	//同步方法 请新建线程调用 防止阻塞UI
	//添加标签 
	static public Result addTags(string[] tags)
	{
#if UNITY_EDITOR
		return null;
#else
		var objlist = new object[1];
		objlist [0] = tags;
		return JavaObject2CSharpObject(tagMananger.Call<AndroidJavaObject> ("add",objlist));
#endif

	}


	//同步方法 请新建线程调用 防止阻塞UI
	//删除标签
	static public Result deleteTags(string[] tags)
	{
#if UNITY_EDITOR
		return null;
#else
		var objlist = new object[1];
		objlist [0] = tags;
		var result = tagMananger.Call<AndroidJavaObject>("delete",objlist);
		return JavaObject2CSharpObject (result);
#endif
	}
	//同步方法 请新建线程调用 防止阻塞UI
	//清除所有标签
	static public Result removeAllTags()
	{
#if UNITY_EDITOR
		return null;
#else
		var result = tagMananger.Call<AndroidJavaObject>("reset");
		return JavaObject2CSharpObject (result);
#endif
	}
	//同步方法 请新建线程调用 防止阻塞UI
	//获取服务器端的所有标签 		
	static public  string[] getTags()
	{
#if UNITY_EDITOR
		return null;
#else		
		var jo = tagMananger.Call<AndroidJavaObject>("list");
		var n = jo.Call<int> ("size");
		var list = new string[n];
		for (int i = 0; i < n; i++) {
			list [i] = jo.Call<string> ("get", i);
		}
		return list;
#endif
		
	}

	//同步方法 请新建线程调用 防止阻塞UI
	//设置用户id,type 为UMPushAlias类的常量
	public static bool addAlias(string alias, string type)
	{
#if UNITY_EDITOR
		return false;
#else
		return mPushAgent.Call<bool>("addAlias",alias,type);
#endif
	}
	//同步方法 请新建线程调用 防止阻塞UI
	//设置用户唯一id,type 为UMPushAlias类的常量
	public static bool addExclusiveAlias(string alias, string type)
	{
#if UNITY_EDITOR
		return false;
#else
		return mPushAgent.Call<bool>("addExclusiveAlias",alias,type);
#endif
	}
	//同步方法 请新建线程调用 防止阻塞UI
	//删除用户id,type 为UMPushAlias类的常量
	public static bool removeAlias(string alias, string type)
	{
#if UNITY_EDITOR
		return false;
#else
		return mPushAgent.Call<bool>("removeAlias",alias,type);
#endif
	}
	//设置通知免打扰模式
	public static void setNoDisturbMode(int startHour, int startMinute, int endHour, int endMinute)
	{
#if UNITY_EDITOR
		return ;
#else
		 mPushAgent.Call("setNoDisturbMode",startHour,startMinute,endHour,endMinute);
#endif
	}
	//获取设备的Device Token
	public static string getRegistrationId()
	{
#if UNITY_EDITOR
		return null;
#else
		var cls =new AndroidJavaClass ("com.umeng.message.UmengRegistrar");
		return cls.CallStatic<string> ("getRegistrationId",context);
#endif
	}
	//设置调试模式
	public static void setDebugMode(bool vaule)
	{
#if UNITY_EDITOR
		return;
#else
		mPushAgent.Call("setDebugMode", vaule);
#endif
	}

}
#endif
