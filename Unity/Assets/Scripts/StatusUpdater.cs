using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class StatusUpdater : MonoBehaviour
{
    [SerializeField]
    private string serverUri;

    private float fps = 30f;

    // Start is called before the first frame update
    void Start()
    {
        PostUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate fps
        float newFPS = 1.0f / Time.deltaTime;
        fps = Mathf.Lerp(fps, newFPS, 0.0005f);
    }

    void PostUpdate()
    {
        StartCoroutine(PostUpdateCoroutine());
    }

    private IEnumerator PostUpdateCoroutine()
    {
        Status status = CurrentStatus();

        byte[] payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(status));

        var uwr = new UnityWebRequest(serverUri + "/state/" + status.DeviceName, "POST");
        uwr.SetRequestHeader("Content-Type", "application/json");

        uwr.uploadHandler = new UploadHandlerRaw(payload);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Connection error.");
        }

        yield return new WaitForSeconds(1f);
        PostUpdate();
    }

    private Status CurrentStatus()
    {
        // Create status object
        Status status = new Status();

        status.DeviceName = SystemInfo.deviceName;
        status.DeviceModel = SystemInfo.deviceModel;
        status.DeviceType = SystemInfo.deviceType.ToString();
        status.BatteryLevel = SystemInfo.batteryLevel * 100;
        status.BatteryStatus = SystemInfo.batteryStatus.ToString();
        status.CurrentScene = SceneManager.GetActiveScene().name;
        status.IsMuted = AudioSettings.Mobile.muteState;
        status.RunTime = Time.realtimeSinceStartup;
        status.fps = fps;

        return status;
    }
}
