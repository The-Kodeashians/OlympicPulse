//
//  OP_Screen_Recorder.m
//  OP_Screen_Recorder
//
//  Created by Luke Hewitt on 6/10/2023.
//

#import "OP_Screen_Recorder.h"
#import <UIKit/UIKit.h>

@implementation OP_Screen_Recorder

- (void)startRecording {
    NSLog(@"Starting Recording, called in static library");
    if ([RPScreenRecorder sharedRecorder].isAvailable) {
        [RPScreenRecorder sharedRecorder].microphoneEnabled = YES;
        [[RPScreenRecorder sharedRecorder] startRecordingWithHandler:^(NSError * _Nullable error) {
            if (error) {
                NSLog(@"Failed to start recording: %@", [error localizedDescription]);
            }
        }];
    }
}

- (void)stopRecording {
    NSLog(@"Stopping Recording and bringing up preview, called in static library");
    [[RPScreenRecorder sharedRecorder] stopRecordingWithHandler:^(RPPreviewViewController * _Nullable previewViewController, NSError * _Nullable error) {
        if (previewViewController) {
            UIWindow *mainWindow = nil;
            for (UIWindowScene *windowScene in [UIApplication sharedApplication].connectedScenes) {
                if (windowScene.activationState == UISceneActivationStateForegroundActive) {
                    mainWindow = windowScene.windows.firstObject;
                    break;
                }
            }
            UIViewController *rootViewController = mainWindow.rootViewController;
            previewViewController.previewControllerDelegate = self;
            [rootViewController presentViewController:previewViewController animated:YES completion:nil];
        } else if (error) {
            NSLog(@"Failed to stop recording: %@", [error localizedDescription]);
        }
    }];
}

// Implement RPPreviewViewControllerDelegate method
- (void)previewControllerDidFinish:(RPPreviewViewController *)previewController {
    [previewController dismissViewControllerAnimated:YES completion:nil];
}

@end

// Extern C functions
#ifdef __cplusplus
extern "C" {
#endif

    void startRecording(void) {
        OP_Screen_Recorder* recorder = [[OP_Screen_Recorder alloc] init];
        [recorder startRecording];
    }

    void stopRecording(void) {
        OP_Screen_Recorder* recorder = [[OP_Screen_Recorder alloc] init];
        [recorder stopRecording];
    }

#ifdef __cplusplus
}
#endif
