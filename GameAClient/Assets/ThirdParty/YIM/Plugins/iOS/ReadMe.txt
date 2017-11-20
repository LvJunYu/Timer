在File->Build Settings...的Platform列表中选择iOS；
点击Build Settings->Build，选择输出iOS工程的路径，输入工程名字，导出iOS工程；
在Xcode中打开上一步输出的iOS工程，在工程配置中Build Phases->Link Binary With Libraries下拉菜单中添加下面几个库文件：
libc++.1.tbd 
libsqlite3.0.tbd 
libresolv.9.tbd 
SystemConfiguration.framework
libYouMeCommon.a l
ibyim.a 
libz.tbd 
CoreTelephony.framework 
iflyMSC.framework 
//如果是带语音转文字的sdk需要添加 AVFoundation.framework 
AudioToolbox.framework 
AddressBook.framework 
CoreLocation.framework
Contacts.framework

为iOS10添加录音权限 
iOS 10 使用录音权限，需要在info新加Privacy - Microphone Usage Description键，值为字符串，比如“语音聊天需要录音权限”。
首次录音时会向用户申请权限。配置方式如图(选择Private-Microphone Usage Description)。 
参考https://youme.im/doc/IMSDKUnityC.php#导入SDK