
using UnityEngine;
using System.Collections;

public class UMPushExample : MonoBehaviour {

	// Use this for initialization
	void Start () {




		#if  UNITY_IPHONE


		//Push iOS Tag 和  alias使用例子

		string[] tagList= {"tag1","tag2","tag3","tag4"};
		UMPushiOS.setChannel("App Store");



		UMPushiOS.AliasDelegate aliascallback= delegate(string response,string error){
			
			Debug.Log("response:\n"+response+" result:\n"+ error!=null?"OK":"Failed!");
		};
		



		UMPushiOS.addAlias ("Alias1",UMPushAlias.Baidu,aliascallback);
		                 

		UMPushiOS.removeAlias("Alias2",UMPushAlias.Baidu,aliascallback);





		UMPushiOS.TagDelegate tagcallback = delegate(string response,int remain,string error){

			Debug.Log("response:\n"+response+"remain:\n"+remain+" result\n:"+error!=null?"OK":"Failed!");
		};

		UMPushiOS.addTags (tagList,tagcallback);





		UMPushiOS.getTags(tagcallback);
		              
		UMPushiOS.removeTags (tagList,tagcallback);


		UMPushiOS.removeAllTags(tagcallback);


		#endif

		#if  UNITY_ANDROID



		UMPushAndroid.enable();
		UMPushAndroid.onAppStart();
		Debug.Log("Device Token:"+UMPushAndroid.getRegistrationId());

		//Push iOS Tag 和  alias 使用例子

		string[] tagList= {"tag1","tag2","tag3","tag4"};
		Debug.Log(UMPushAndroid.addTags (tagList).jsonString);
		Debug.Log(UMPushAndroid.getTags().GetLength(0));
		Debug.Log(UMPushAndroid.deleteTags(tagList).jsonString);

		UMPushAndroid.addAlias("Alias1",UMPushAlias.Baidu);
		UMPushAndroid.removeAlias("Alias1",UMPushAlias.Baidu);






		#endif
	
	}

	void HandleAliasDelegate (string response, string error)
	{
		
	}



}
