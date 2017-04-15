//
//  JoyUnityAppController.h
//  Unity-iPhone
//
//  Created by Quan Siwei on 16/6/22.
//
//
#pragma once
#include "Classes/UnityAppController.h"
#include "Classes/UI/UnityViewControllerBase.h"

@interface JoyUnityAppController : UnityAppController
{
}

- (void)refreshViewStatusBar;
- (void)copyTextToClipboard:(NSString*)text;
- (NSString*)getTextFromClipboard;
+ (JoyUnityAppController*)Instance;

@end
