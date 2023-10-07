//
//  OP_Screen_Recorder.h
//  OP_Screen_Recorder
//
//  Created by Luke Hewitt on 6/10/2023.
//

#import <Foundation/Foundation.h>
#import <ReplayKit/ReplayKit.h>

@interface OP_Screen_Recorder : NSObject <RPPreviewViewControllerDelegate>

- (void)startRecording;

- (void)stopRecording;

@end
