using Sttplay.MediaPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.InputSystem;

public class HK_SCPlayerCtrl : MonoBehaviour
{
    public string URL;
    public UnitySCPlayerPro SCPlayer = null;
    public InputActionReference JoyStitckSecondButton;
    public InputActionReference JoyStitckPrimaryButton;
    public InputActionReference JoyStitckTrigger;
    protected bool hasBeenTrigger = false;
    public InputActionAsset ActionAsset;
    public HK_RenderCompositionLayer hK_RenderCompositionLayer = null;

    //protected bool ReloadAndReplay = false;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
    private static AndroidJavaClass unityPlayer;
    private static AndroidJavaObject currentActivity;
    private static AndroidJavaObject context;
    private static System.IntPtr eglContext;
#endif

    // Start is called before the first frame update
    void Start()
    {
        SCPlayer.onFirstFrameRenderEvent.AddListener(FirstVideoFrameRender);
        Open();
        Play();
    }

    private void OnDestroy()
    {
        Close();
    }

    private void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaClass jclz = new AndroidJavaClass("com.sttplay.MediaPlayer.TimeUtility");
        long creationts = jclz.CallStatic<long>("GetCreationTime", GetBasePath());
        if (jclz.CallStatic<long>("GetPackageLastUpdateTime", currentActivity) > creationts)
        {
            if (System.IO.Directory.Exists(GetBasePath()))
                System.IO.Directory.Delete(GetBasePath(), true);
            AndroidJavaObject assetManager = currentActivity.Call<AndroidJavaObject>("getAssets");
            Debug.Log($"[_unity] persistentDataPath {Application.persistentDataPath}");
            new AndroidJavaClass("com.sttplay.MediaPlayer.FileUtility").CallStatic("CopyAssets", "Video", Application.persistentDataPath + "/", assetManager);
        }
        GL.IssuePluginEvent(XRendererEx.XRendererEx_GetUnityRenderEventFuncPointer(), 0xfff0);
        while (XRendererEx.XRendererEx_GetUnityContext(ref eglContext) < 0)
            Thread.Sleep(1);
        int jniVer = 0;
        System.IntPtr jvm = ISCNative.GetJavaVM(ref jniVer);
        XRendererEx.XRendererEx_SetJavaVM(jvm, jniVer);
#endif
    }

    protected void FirstVideoFrameRender(SCRenderer render)
    {
        Debug.Log($"video w({render.SyntheticTexture.width}) h({render.SyntheticTexture.height})"); 

    }
    protected string GetBasePath()
    {
        string _base = "";
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            _base = Application.streamingAssetsPath + "/Video";
        else if (Application.platform == RuntimePlatform.Android)
            _base = Application.persistentDataPath + "/Video";
        return _base;
    }
    public string GetFilePath()
    {
        string videoPath = "";

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            videoPath = GetBasePath() + "/" + URL + ".mp4";
        else if (Application.platform == RuntimePlatform.Android)
            videoPath = GetBasePath() + "/" + URL + ".mp4";
        return videoPath;
    }
    public void Open()
    {

        var videoUrl = GetFilePath();
        Debug.Log($"[_unity] {videoUrl}");

        SCPlayer.Open(MediaType.LocalFile, videoUrl);

        var render = SCPlayer.VideoRenderer;


    }

    public void Pause()
    {
        SCPlayer.Pause();
    }
    public void Close()
    {
        SCPlayer.Close();
    }

    public void Play()
    {
        SCPlayer.Play();
    }

    public void ReloadVideoAndPlay()
    {
        Close();
        Open();
        Play();
    }

    private void OnEnable()
    {
        if(ActionAsset)
        {
            ActionAsset.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!SCPlayer.IsPaused)
        {
            if(JoyStitckSecondButton.action.ReadValue<float>() > 0)
            {
                Pause();
            }
        }
        else if(SCPlayer.IsPaused)
        {
            if (JoyStitckPrimaryButton.action.ReadValue<float>() > 0)
            {
                Play();
            }
        }


        if (JoyStitckTrigger.action.ReadValue<float>() > 0)
        {
            if(hK_RenderCompositionLayer && !hasBeenTrigger)
            {
                hasBeenTrigger = true;
                if(hK_RenderCompositionLayer.isActiveAndEnabled)
                {
                    hK_RenderCompositionLayer.gameObject.SetActive(false);
                }
                else
                {
                    hK_RenderCompositionLayer.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            hasBeenTrigger = false;
        }

        //if(JoyStitckTrigger.action.ReadValue<bool>())
        //{
        //    if(!ReloadAndReplay)
        //    {
        //        ReloadVideoAndPlay();
        //        ReloadAndReplay = true;
        //    }
        //}
        //else
        //{
        //    if(ReloadAndReplay) ReloadAndReplay = false;
        //}


    }
}
