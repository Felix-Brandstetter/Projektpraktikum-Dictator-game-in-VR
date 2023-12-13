using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeviceAudio : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onAudioMuted;

    [SerializeField]
    private UnityEvent onAudioUnmuted;

    void Start()
    {
        AudioSettings.Mobile.StartAudioOutput();

        HandleMuteState(AudioSettings.Mobile.muteState);

        AudioSettings.Mobile.OnMuteStateChanged += HandleMuteState;
    }

    public void HandleMuteState(bool muteState)
    {
        if (muteState)
        {
            onAudioMuted?.Invoke();
        } else
        {
            onAudioUnmuted.Invoke();
        }
    }



}
