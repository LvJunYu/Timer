//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine; 
using System;
using System.Collections;
using System.Collections.Generic;
namespace cn.sharesdk.unity3d
{
	/// <summary>
	/// Content type.
	/// </summary>
	public class ShareContent
	{	      		
		Hashtable shareParams = new Hashtable();
		Hashtable customizeShareParams = new Hashtable();

		/*iOS/Android*/
		public void SetTitle(String title) {
			shareParams["title"] = title;
		}

		/*iOS/Android*/
		public void SetText(String text) {
			shareParams["text"] = text;
		}

		/*iOS/Android*/
		public void SetUrl(String url) {
			shareParams["url"] = url;
		}

		/*iOS/Android - 本地图片路径*/
		public void SetImagePath(String imagePath) {
			#if UNITY_ANDROID
			shareParams["imagePath"] = imagePath;
			#elif UNITY_IPHONE
			shareParams["imageUrl"] = imagePath;
			#endif
		}

		/*iOS/Android - 网络图片路径*/
		public void SetImageUrl(String imageUrl) {
			shareParams["imageUrl"] = imageUrl;
		}

		/*iOS/Android - 分享类型*/
		public void SetShareType(int shareType) {
			#if UNITY_ANDROID
			if (shareType == 0) {
				shareType = 1;
			}
			#endif
			shareParams["shareType"] = shareType;
		}

		/*Android Only*/
		public void SetTitleUrl(String titleUrl) {
			shareParams["titleUrl"] = titleUrl;
		}

		/*iOS/Android*/
		public void SetComment(String comment) {
			shareParams["comment"] = comment;
		}

		/*Android Only*/
		public void SetSite(String site) {
			shareParams["site"] = site;
		}

		/*Android Only*/
		public void SetSiteUrl(String siteUrl) {
			shareParams["siteUrl"] = siteUrl;
		}

		/*Android Only*/
		public void SetAddress(String address) {
			shareParams["address"] = address;
		}

		/*iOS/Android*/
		public void SetFilePath(String filePath) {
			shareParams["filePath"] = filePath;
		}

		/*iOS/Android*/
		public void SetMusicUrl(String musicUrl) {
			shareParams["musicUrl"] = musicUrl;
		}

		/*iOS/Android - Sina/Tencent/Twitter/VKontakte*/
		public void SetLatitude(String latitude) {
			shareParams["latitude"] = latitude;
		}

		/*iOS/Android - Sina/Tencent/Twitter/VKontakte*/
		public void SetLongitude(String longitude) {
			shareParams["longitude"] = longitude;
		}
		
		/*iOS/Android - YouDaoNote*/
		public void SetSource(String source){
			#if UNITY_ANDROID
			shareParams["url"] = source;
			#elif UNITY_IPHONE
			shareParams ["source"] = source;
			#endif
		}
		
		/*iOS/Android - YouDaoNote*/
		public void SetAuthor(String author){
			#if UNITY_ANDROID
			shareParams["address"] = author;
			#elif UNITY_IPHONE
			shareParams ["author"] = author;
			#endif
		}
		
		/*iOS/Android - Flickr*/
		public void SetSafetyLevel(int safetyLevel){
			shareParams ["safetyLevel"] = safetyLevel;
		}
		
		/*iOS/Android - Flickr*/
		public void SetContentType(int contentType){
			shareParams ["contentType"] = contentType;
		}
		
		/*iOS/Android - Flickr*/
		public void SetHidden(int hidden){
			shareParams ["hidden"] = hidden;
		}
		
		/*iOS/Android - Flickr*/
		public void SetIsPublic(bool isPublic){
			shareParams ["isPublic"] = isPublic;
		}
		
		/*iOS/Android - Flickr*/
		public void SetIsFriend(bool isFriend){
			shareParams ["isFriend"] = isFriend;
		}
		
		/*iOS/Android - Flickr*/
		public void SetIsFamily(bool isFamily){
			shareParams ["isFamily"] = isFamily;
		}
		
		/*iOS/Android - VKontakte*/
		public void SetFriendsOnly(bool friendsOnly){
			#if UNITY_ANDROID
			shareParams["isFriend"] = friendsOnly;
			#elif UNITY_IPHONE
			shareParams ["friendsOnly"] = friendsOnly;
			#endif
		}
		
		/*iOS/Android - VKontakte*/
		public void SetGroupID(String groupID){
			shareParams ["groupID"] = groupID;
		}
		
		/*iOS/Android - WhatsApp*/
		public void SetAudioPath(String audioPath){
			#if UNITY_ANDROID
			shareParams["filePath"] = audioPath;
			#elif UNITY_IPHONE
			shareParams ["audioPath"] = audioPath;
			#endif
		}
		
		/*iOS/Android - WhatsApp*/
		public void SetVideoPath(String videoPath){
			#if UNITY_ANDROID
			shareParams["filePath"] = videoPath;
			#elif UNITY_IPHONE
			shareParams ["videoPath"] = videoPath;
			#endif
		}
		
		/*iOS/Android - YouDaoNote/YinXiang/Evernote*/
		public void SetNotebook(String notebook){
			shareParams ["notebook"] = notebook;
		}
		
		/*iOS/Android - Pocket/Flickr/YinXiang/Evernote*/
		public void SetTags(String tags){
			shareParams ["tags"] = tags;
		}

		/*iOS Only - Sina*/
		public void SetObjectID(String objectId) {
			shareParams["objectID"] = objectId;
		}

		/*iOS Only - Renren*/
		public void SetAlbumID(String albumId) {
			shareParams["AlbumID"] = albumId;
		}

		/*iOS Only - Wechat*/
		public void SetEmotionPath(String emotionPath){
			shareParams["emotionPath"] = emotionPath;
		}

		/*iOS Only - Wechat/Yixin*/
		public void SetExtInfoPath(String extInfoPath){
			shareParams["extInfoPath"] = extInfoPath;
		}

		/*iOS Only - Wechat*/ 
		public void SetSourceFileExtension(String sourceFileExtension){
			shareParams["sourceFileExtension"] = sourceFileExtension;
		}

		/*iOS Only - Wechat*/
		public void SetSourceFilePath(String sourceFilePath){
			shareParams["sourceFilePath"] = sourceFilePath;
		}

		/*iOS Only - QQ/Wechat/Yixin*/
		public void SetThumbImageUrl(String thumbImageUrl){
			shareParams["thumbImageUrl"] = thumbImageUrl;
		}

		/*iOS Only - Douban/LinkedIn*/
		public void SetUrlDescription(String urlDescription){
			shareParams["urlDescription"] = urlDescription;
		}

		/*iOS Only - Pinterest*/
		public void SetBoard(String SetBoard){
			shareParams["board"] = SetBoard;
		}

		/*iOS Only - WhatsApp/Instagram*/
		public void SetMenuX(float menuX){
			shareParams ["menuX"] = menuX;
		}

		/*iOS Only - WhatsApp/Instagram*/
		public void SetMenuY(float menuY){
			shareParams ["menuY"] = menuY;
		}

		/*iOS Only - LinkedIn*/
		public void SetVisibility(String visibility){
			shareParams ["visibility"] = visibility;
		}

		/*iOS Only - Tumblr*/
		public void SetBlogName(String blogName){
			shareParams ["blogName"] = blogName;
		}

		/*iOS Only - SMS/Mail*/
		public void SetRecipients(String recipients){
			shareParams ["recipients"] = recipients;
		}

		/*iOS Only - Mail*/
		public void SetCCRecipients(String ccRecipients){
			shareParams ["ccRecipients"] = ccRecipients;
		}

		/*iOS Only - Mail*/
		public void SetBCCRecipients(String bccRecipients){
			shareParams ["bccRecipients"] = bccRecipients;
		}

		/*iOS Only - Dropbox/Mail/SMS*/
		public void SetAttachmentPath(String attachmentPath){
			shareParams ["attachmentPath"] = attachmentPath;
		}

		/*iOS Only - Instapaper/Pinterest*/
		public void SetDesc(String desc){
			shareParams ["desc"] = desc;
		}

		/*iOS Only - Instapaper*/
		public void SetIsPrivateFromSource(bool isPrivateFromSource){
			shareParams ["isPrivateFromSource"] = isPrivateFromSource;
		}

		/*iOS Only - Instapaper*/
		public void SetResolveFinalUrl(bool resolveFinalUrl){
			shareParams ["resolveFinalUrl"] = resolveFinalUrl;
		}

		/*iOS Only - - Instapaper*/
		public void SetFolderId(int folderId){
			shareParams ["folderId"] = folderId;
		}

		/*iOS Only - Pocket*/
		public void SetTweetID(String tweetID){
			shareParams ["tweetID"] = tweetID;
		}

		/*iOS Only - Yixin*/
		public void SetToUserID(String toUserID){
			shareParams ["toUserID"] = toUserID;
		}

		/*iOS Only - Kakao*/
		public void SetPermission(String permission){
			shareParams ["permission"] = permission;
		}

		/*iOS Only - Kakao*/
		public void SetEnableShare(bool enableShare){
			shareParams ["enableShare"] = enableShare;
		}

		/*iOS Only - Kakao*/
		public void SetImageWidth(float imageWidth){
			shareParams ["imageWidth"] = imageWidth;
		}

		/*iOS Only - Kakao*/
		public void SetImageHeight(float imageHeight){
			shareParams ["imageHeight"] = imageHeight;
		}

		/*iOS Only - Kakao*/
		public void SetAppButtonTitle(String appButtonTitle){
			shareParams ["appButtonTitle"] = appButtonTitle;
		}

		/*iOS Only - Kakao*/
		public void SetAndroidExecParam(Hashtable androidExecParam){
			shareParams ["androidExecParam"] = androidExecParam;
		}

		/*iOS Only - Kakao*/
		public void SetAndroidMarkParam(String androidMarkParam){
			shareParams ["androidMarkParam"] = androidMarkParam;
		}

		/*iOS Only - Kakao*/
		public void SetIphoneExecParam(Hashtable iphoneExecParam){
			shareParams ["iphoneExecParam"] = iphoneExecParam;
		}

		/*iOS Only - Kakao*/
		public void SetIphoneMarkParam(String iphoneMarkParam){
			shareParams ["iphoneMarkParam"] = iphoneMarkParam;
		}

		/*iOS Only - Kakao*/
		public void SetIpadExecParam(Hashtable ipadExecParam){
			shareParams ["ipadExecParam"] = ipadExecParam;
		}

		/*iOS Only - Kakao*/
		public void SetIpadMarkParam(String ipadMarkParam){
			shareParams ["ipadMarkParam"] = ipadMarkParam;
		}


		//不同平台分享不同内容
		public void SetShareContentCustomize(PlatformType platform, ShareContent content) {
			customizeShareParams [(int)platform] = content.GetShareParamsStr();
		}

		public String GetShareParamsStr() {
			if (customizeShareParams.Count > 0) {
				shareParams["customizeShareParams"] = customizeShareParams;
			}
            String jsonStr = ShareSDKNS.MiniJSON.jsonEncode (shareParams);
			Debug.Log("ParseShareParams  ===>>> " + jsonStr );
			return jsonStr;
		}

		public Hashtable GetShareParams() {
			if (customizeShareParams.Count > 0) {
				shareParams["customizeShareParams"] = customizeShareParams;
			}
            String jsonStr = ShareSDKNS.MiniJSON.jsonEncode (shareParams);
			Debug.Log("ParseShareParams  ===>>> " + jsonStr );
			return shareParams;
		}
	}

}

