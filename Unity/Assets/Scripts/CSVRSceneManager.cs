using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cardboard = Google.XR.Cardboard.Api;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SpatialTracking;
using UnityEngine.Events;

public class CSVRSceneManager : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 15f)]
    [Tooltip("Delay of scene change.")]
    private float delayTime = 0f;

    [SerializeField]
    [Range(0f, 5f)]
    [Tooltip("Duration of fade-in and fade-out of curtain.")]
    private float transitionTime = 1.5f;

    [SerializeField]
    private string nextScene;

    [SerializeField]
    [Tooltip("Recenters coordinate system to current viewport direction on scene enter.")]
    private bool recenterOnStartup = false;

    [SerializeField]
    [Tooltip("Determines if VR should be turned on.")]
    private bool vrEnabled = true;

    [SerializeField]
    [Tooltip("Next scene will be loaded after delay time.")]
    private bool autoStartNextScene = false;

    [SerializeField]
    [Tooltip("Determines how the scene can react to VR input.")]
    private TrackedPoseDriver.TrackingType trackingType;

    [SerializeField]
    [Tooltip("Determines how the scene can react to VR input.")]
    private VRTargetPlatform vrTargetPlatform = VRTargetPlatform.mobile;

    [Header("Hooks")]

    [SerializeField]
    [Tooltip("Triggers events on start.")]
    private UnityEvent onStart;

    [SerializeField]
    [Tooltip("Activate gameobject on start.")]
    private GameObject[] activateOnStart;

    [SerializeField]
    [Tooltip("Deactivate gameobject on start.")]
    private GameObject[] deactivateOnStart;


    [Header("References")]

    [SerializeField]
    [Tooltip("Reference to the Camera for Mobile Platforms (Android, iOS)")]
    private GameObject mobileCamera;

    [SerializeField]
    [Tooltip("Reference to the Camera Rig for SteamVR Platform")]
    private GameObject steamVRCameraRig;

    // Curtain automatically derived in HandleVRTargetPlatform
    private Curtain curtain;


    private bool IsVRModeActive { get => XRGeneralSettings.Instance.Manager.isInitializationComplete; }

    public void Start()
    {
        HandleVRTargetPlatform(); // Derives curtain

        if (curtain != null)
        {
            curtain.FadeTransparent(transitionTime);
        }

        if (recenterOnStartup)
        {
            Cardboard.Recenter();
            Camera.main.transform.rotation.SetEulerAngles(0, 0, 0);
        }

        if (vrEnabled)
        {
            //Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //Screen.fullScreen = true;
            //Screen.orientation = ScreenOrientation.LandscapeLeft;

            if (!IsVRModeActive && vrTargetPlatform == VRTargetPlatform.mobile)
            {
                EnterVR();
            }

        } else
        {
            //Screen.sleepTimeout = SleepTimeout.SystemSetting;
            //Screen.orientation = ScreenOrientation.Portrait;
            //Cursor.lockState = CursorLockMode.None;

            //if (IsVRModeActive)
            //{
            //    ExitVR();
            //}

            //Screen.orientation = ScreenOrientation.Portrait;
        }

        // Checks if the device parameters are stored and scans them if not.
        // This is only required if the XR plugin is initialized on startup,
        // otherwise these API calls can be removed and just be used when the XR
        // plugin is started.
        if (!Cardboard.HasDeviceParams())
        {
            Cardboard.ScanDeviceParams();
        }

        onStart.Invoke();

        foreach (GameObject gameObject in activateOnStart)
        {
            gameObject.SetActive(true);
        }

        foreach (GameObject gameObject in deactivateOnStart)
        {
            gameObject.SetActive(false);
        }

        if (autoStartNextScene)
        {
            NextScene();
        }
    }

    public void OnValidate()
    {
        HandleVRTargetPlatform();
    }

    public void Update()
    {
        if (Cardboard.IsCloseButtonPressed)
        {
            ExitVR();
        }

        if (Cardboard.IsGearButtonPressed)
        {
            Cardboard.ScanDeviceParams();
        }
    }

    public void NextScene()
    {
        OpenScene(nextScene);
    }

    public void OpenScene(string sceneName)
    {
        StartCoroutine(SwitchScene(sceneName));
    }

    private IEnumerator SwitchScene(string sceneName)
    {
        yield return new WaitForSeconds(delayTime);

        if (curtain != null)
        {
            curtain.FadeBlack(transitionTime);
        }
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);

        yield return true;
    }

    public void OpenWebsite(string url)
    {
        Application.OpenURL(url);
    }

    public void EnterVR()
    {
        StartCoroutine(StartXR());
    }

    public void ExitVR()
    {
        StartCoroutine(StopXR());
    }


    /// <summary>
    /// Initializes and starts the Cardboard XR plugin.
    /// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
    /// </summary>
    ///
    /// <returns>
    /// Returns result value of <c>InitializeLoader</c> method from the XR General Settings Manager.
    /// </returns>
    private IEnumerator StartXR()
    {
        //Screen.fullScreen = true;

        if (Screen.orientation != ScreenOrientation.LandscapeLeft)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Camera.main.ResetAspect();

            // Wait for Screen rotation before initializing VR
            yield return new WaitForSeconds(.15f);
        }

        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed.");
        }
        else
        {
            Debug.Log("XR initialized.");

            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Debug.Log("XR started.");

            if (recenterOnStartup)
            {
                yield return new WaitForSeconds(transitionTime / 3);
                Cardboard.Recenter();
            }
        }
    }


    /// <summary>
    /// Stops and deinitializes the Cardboard XR plugin.
    /// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
    /// </summary>
    private IEnumerator StopXR()
    {
        Debug.Log("Stopping XR...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        Debug.Log("XR stopped.");

        Debug.Log("Deinitializing XR...");
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR deinitialized.");

        //yield return new WaitForSeconds(.3f);

        //Camera.main.fieldOfView = 60f;

        //Screen.fullScreen = false;

        Camera.main.ResetAspect();
        yield return null;

        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("TransitionTo2D");
    }

    private void HandleVRTargetPlatform()
    {
        if (vrEnabled)
        {
            if (mobileCamera == null || steamVRCameraRig == null)
            {
                throw new MissingComponentException("Mobile camera or SteamVR Camera Rig missing.");
            }

            switch (vrTargetPlatform)
            {
                case VRTargetPlatform.mobile:
                    mobileCamera.SetActive(true);
                    steamVRCameraRig.SetActive(false);
                    break;
                case VRTargetPlatform.steamVR:
                    mobileCamera.SetActive(false);
                    steamVRCameraRig.SetActive(true);

                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TrackedPoseDriver>().trackingType = trackingType;
                    break;
                default:
                    break;
            }

            GameObject curtainGao = GameObject.Find("Curtain");
            if (curtainGao != null)
            {
                curtain = curtainGao.GetComponent<Curtain>();
            }
        }

    }

    public enum VRTargetPlatform
    {
        mobile = 0,
        steamVR = 1
    }
}
