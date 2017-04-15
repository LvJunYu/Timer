#if  UNITY_IPHONE


using System.Runtime.InteropServices;
using AOT;

public class UMPushiOS
{


	//添加,删除Tag回调
	//response为绑定的tag集合,remain剩余可用的tag数,为-1时表示异常,error为获取失败时的信息
	public delegate void TagDelegate (string response,int remain,string error);
	//添加,删除alias回调
	//error为获取失败时的信息，response为成功返回的数据
	public delegate void AliasDelegate (string response,string error);



	/** 设置应用的日志输出的开关（默认关闭）
 	@param value 是否开启标志，注意App发布时请关闭日志输出
 	*/
	public static void setLogEnabled (bool value)
	{
#if !UNITY_EDITOR
		_setLogEnabled(value);
#endif
	}

	/** 设置是否允许SDK自动清空角标（默认开启）
 	@param value 是否开启角标清空
 	*/
	public static void setBadgeClear (bool value)
	{
#if !UNITY_EDITOR
		_setBadgeClear(value);
#endif
	}

	/** 设置是否允许SDK当应用在前台运行收到Push时弹出Alert框（默认开启）
	 @warning 建议不要关闭，
 	@param value 是否开启弹出框
 	*/
	public static void setAutoAlert (bool value)
	{
#if !UNITY_EDITOR
		_setAutoAlert(value);
#endif
	}


	/** 设置App的发布渠道（默认为:"App Store"）
	@param channel 渠道名称
 	*/
	public static void setChannel (string channel)
	{
#if !UNITY_EDITOR
		_setChannel(channel);
#endif
	}

	/** 解除RemoteNotification的注册（关闭消息推送，实际调用：[[UIApplication sharedApplication] unregisterForRemoteNotifications]）
 
 	*/
	public static void unregisterForRemoteNotifications ()
	{
#if !UNITY_EDITOR
		_unregisterForRemoteNotifications();
#endif
	}

    /** 绑定一个别名至设备（含账户，和平台类型）
    @warning 添加Alias的先决条件是已经成功获取到device_toke
    @param name 账户，例如email
    @param type 平台类型，参见UMPushAlias类的常量
    @param handler返回数据，error为获取失败时的信息，response为成功返回的数据
    */
    public static  void addAlias (string name, string type, AliasDelegate handler = null)
    {
#if !UNITY_EDITOR
tmp0 = handler;
_addAlias(name,type, AliasHandler0);
#endif

    }

    /** 绑定一个别名至设备（含账户，和平台类型）
    @warning 添加Alias的先决条件是已经成功获取到device_toke
    @param name 账户，例如email
    @param type 平台类型，参见UMPushAlias类的常量
    @param handler返回数据，error为获取失败时的信息，response为成功返回的数据
    */
    public static  void setAlias (string name, string type, AliasDelegate handler = null)
    {
#if !UNITY_EDITOR
tmp0 = handler;
_setAlias(name,type, AliasHandler0);
#endif

    }

	/** 删除一个设备的别名绑定
	 @warning 删除Alias的先决条件是已经成功获取到device_token
	 @param name 账户，例如email
	 @param type 平台类型，参见UMPushAlias类的常量
	 @param handler返回数据，error为获取失败时的信息，response为成功返回的数据
	 */
	public static  void removeAlias (string name, string type, AliasDelegate handler = null)
	{
#if !UNITY_EDITOR
		tmp1 = handler;
		_removeAlias(name,type,AliasHandler1);
#endif
	}



	/** 绑定一个或多个tag至设备，每台设备最多绑定64个tag，超过64个，绑定tag不再成功，可`removeTag`或者`removeAllTags`来精简空间
	 @warning 添加tag的先决条件是已经成功获取到device_token
	 @param tag tag标记,类型为string[]
	 @param handle response为绑定的tag集合,remain剩余可用的tag数,为-1时表示异常,error为获取失败时的信息
	 */


	public static  void addTags (string[] tags, TagDelegate handler = null)
	{
		#if !UNITY_EDITOR
		tmpT0 = handler;
		_addTags (string.Join (",", tags), TagHandler0);

		#endif		
	}

	/** 删除设备中绑定的一个或多个tag
	 * @param tag tag标记,类型为string[]
	 @param handle response为绑定的tag集合,remain剩余可用的tag数,为-1时表示异常,error为获取失败时的信息

 	*/
	public static  void removeTags (string[] tags, TagDelegate handler = null)
	{
		#if !UNITY_EDITOR
		tmpT1 = handler;
		_removeTags (string.Join (",", tags), TagHandler1);
		#endif


	}

	/** 获取当前绑定设备上的所有tag(每台设备最多绑定64个tag)

	 */

	public static  void getTags (TagDelegate handler)
	{
		#if !UNITY_EDITOR
		tmpT2 = handler;
		_getTags (TagHandler2);
		#endif
	}

	/** 删除设备中所有绑定的tag,handle responseObject
	 @warning 删除tag的先决条件是已经成功获取到device_token，否则失败
	 @param handle response为绑定的tag集合,remain剩余可用的tag数,为-1时表示异常,error为获取失败时的信息
 	*/

	public static  void removeAllTags (TagDelegate handler = null)
	{
		#if !UNITY_EDITOR

		tmpT3 = handler;
		_removeAllTags (TagHandler3);
		#endif
	}




	static AliasDelegate tmp0=null, tmp1=null, tmp2=null;
	static TagDelegate tmpT0=null, tmpT1=null, tmpT2=null, tmpT3=null;

	[MonoPInvokeCallback (typeof(AliasDelegate))]
	static  void AliasHandler0 (string response, string error)
	{
		if (tmp0 != null) {
			tmp0 (response, error);
		}
	}

	[MonoPInvokeCallback (typeof(AliasDelegate))]
	static  void AliasHandler1 (string response, string error)
	{
		if (tmp1 != null) {
			tmp1 (response, error);
		}
	}

	[MonoPInvokeCallback (typeof(TagDelegate))]
	static void TagHandler0 (string response, int remain, string error)
	{
		if (tmpT0 != null) {
			tmpT0 (response, remain, error);
		}

	}

	[MonoPInvokeCallback (typeof(TagDelegate))]
	static void TagHandler1 (string response, int remain, string error)
	{
		if (tmpT1 != null) {
			tmpT1 (response, remain, error);
		}

	}

	[MonoPInvokeCallback (typeof(TagDelegate))]
	static void TagHandler2 (string response, int remain, string error)
	{
		if (tmpT2 != null) {
			tmpT2 (response, remain, error);
		}

	}

	[MonoPInvokeCallback (typeof(TagDelegate))]
	static void TagHandler3 (string response, int remain, string error)
	{
		if (tmpT3 != null) {
			tmpT3 (response, remain, error);
		}

	}



	[DllImport ("__Internal")]
	static extern private void _addTags (string tag, TagDelegate handler);

	[DllImport ("__Internal")]
	static extern private void _removeTags (string tag, TagDelegate handler);

	[DllImport ("__Internal")]
	static extern private void _getTags (TagDelegate handler);

	[DllImport ("__Internal")]
	static extern private void _removeAllTags (TagDelegate handler);



    [DllImport ("__Internal")]
    static extern private void _addAlias (string name, string type, AliasDelegate handler);

    [DllImport ("__Internal")]
    static extern private void _setAlias (string name, string type, AliasDelegate handler);

	[DllImport ("__Internal")]
	static extern private void _removeAlias (string name, string type, AliasDelegate handler);



	[DllImport ("__Internal")]
	static extern public void _setLogEnabled (bool value);

	[DllImport ("__Internal")]
	static extern public void _setBadgeClear (bool value);

	[DllImport ("__Internal")]
	static extern public void _setAutoAlert (bool value);

	[DllImport ("__Internal")]
	static extern public void _setChannel (string channel);

	[DllImport ("__Internal")]
	static extern public void _unregisterForRemoteNotifications ();






}
#endif
