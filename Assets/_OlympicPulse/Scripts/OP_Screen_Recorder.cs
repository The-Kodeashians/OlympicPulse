using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace _OlympicPulse.Scripts
{
    public class OP_Screen_Recorder : MonoBehaviour
    {
        // Flag to keep track of recording state
        private bool isRecording = false;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void startRecording();

        [DllImport("__Internal")]
        private static extern void stopRecording();
#endif

        // Method to start recording
        public void StartRecording()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (!isRecording)
            {
                startRecording();
                isRecording = true;
            }
#endif
        }

        // Method to stop recording nd show hte preview
        public void StopRecordingShowPreview()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (isRecording)
            {
                stopRecording();
                isRecording = false;
            }
#endif
        }
    }
}