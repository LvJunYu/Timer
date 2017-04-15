#import "UMessage.h"
#import "UMPushHandler.h"

@implementation UMPushHandler



- (void)didRegisterForRemoteNotificationsWithDeviceToken:(NSNotification*)notification
{
    [UMessage registerDeviceToken:(NSData*)notification.userInfo];
    
    
}

- (void)didReceiveRemoteNotification:(NSNotification*)notification
{
    [UMessage didReceiveRemoteNotification:(NSDictionary *)notification.userInfo];

}


@end

