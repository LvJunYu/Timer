//
//  JoyUnityAppController.mm
//  Unity-iPhone
//
//  Created by Quan Siwei on 16/6/22.
//
//
#import "JoyUnityAppController.h"
#import <Foundation/Foundation.h>
#import <ShareSDK/ShareSDK.h>
#import "../../../UmengPush/Plugins/iOS/UMessage.h"

IMPL_APP_CONTROLLER_SUBCLASS(JoyUnityAppController)

static JoyUnityAppController* _instance = nil;

@implementation JoyUnityAppController
- (id)init
{
    
    if( (self = [super init]) )
    {
        // due to clang issues with generating warning for overriding deprecated methods
        // we will simply assert if deprecated methods are present
        // NB: methods table is initied at load (before this call), so it is ok to check for override
        NSAssert(![self respondsToSelector:@selector(createUnityViewImpl)],
                 @"createUnityViewImpl is deprecated and will not be called. Override createUnityView"
                 );
        NSAssert(![self respondsToSelector:@selector(createViewHierarchyImpl)],
                 @"createViewHierarchyImpl is deprecated and will not be called. Override willStartWithViewController"
                 );
        NSAssert(![self respondsToSelector:@selector(createViewHierarchy)],
                 @"createViewHierarchy is deprecated and will not be implemented. Use createUI"
                 );
    }
    _instance = self;
    return self;
}

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    UMessageStartWithAppkeyForUnity(@"5779c90d67e58e60150010c6",launchOptions);
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}

- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo
{
    [super application:application didReceiveRemoteNotification:userInfo];
    UnitySendMessage("JoyNativeTool", "OnReceiveRemoteNotification", "");
}


#if !UNITY_TVOS
- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler
{
    [super application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:handler];
    UnitySendMessage("JoyNativeTool", "OnReceiveRemoteNotification", "");
}
#endif


- (void)refreshViewStatusBar
{
    for (int i=0; i<5; i++) {
        UnityViewControllerBase* vb = _viewControllerForOrientation[i];
        if(vb != nil)
        {
            [vb setNeedsStatusBarAppearanceUpdate];
        }
    }
}

- (NSString*)getTextFromClipboard
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    return pasteboard.string;
}

- (void)copyTextToClipboard:(NSString *)text
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = text;
}


+ (UnityAppController*)Instance
{
    return _instance;
}
@end

extern "C" void _setStatusBarShow(BOOL show)
{
    [UnityViewControllerBase setStatusBarHidden: !show];
    [[JoyUnityAppController Instance] refreshViewStatusBar];
}

extern "C" void _copyTextToClipboard(const char* str)
{
    NSString *text = [NSString stringWithUTF8String: str] ;
    [[JoyUnityAppController Instance] copyTextToClipboard:text];
}

extern "C" const char* _getTextFromClipboard()
{
    NSString *text = [[JoyUnityAppController Instance] getTextFromClipboard];
    if(text == nil)
    {
        return NULL;
    }
    const char* str = [text UTF8String];
    if(str == NULL)
    {
        return NULL;
    }
    char* res = (char*)malloc(strlen(str) + 1);
    strcpy(res, str);
    return res;
}
