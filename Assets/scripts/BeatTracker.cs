using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;

public class BeatTracker : MonoBehaviour
{
    public EventReference musicEvent;
    private EventInstance musicInstance;

    FMOD.Studio.EVENT_CALLBACK markerCallback;

    [StructLayout(LayoutKind.Sequential)]
    public struct TimelineMarkerProperties
    {
        public IntPtr name;
        public int position;
    }

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);

        markerCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        musicInstance.setCallback(markerCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicInstance.start();
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            TimelineMarkerProperties marker = (TimelineMarkerProperties)Marshal.PtrToStructure(parameterPtr, typeof(TimelineMarkerProperties));
            string markerName = Marshal.PtrToStringAnsi(marker.name);

            if (markerName == "Beat")
            {
                Debug.Log("Beat hit at position: " + marker.position);
            }
        }

        return FMOD.RESULT.OK;
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }
}
