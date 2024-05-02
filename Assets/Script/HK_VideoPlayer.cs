using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VIVE.OpenXR.CompositionLayer;

public class HK_VideoPlayer : MonoBehaviour
{
    protected UnityEngine.Video.VideoPlayer videoPlayer = null;
    static protected RenderTexture videoRenderTexture = null;

    static public RenderTexture VideoRenderTexture
    {
        get { return videoRenderTexture; }
    }

    public VideoClip VideoClipData = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!VideoClipData) return;

        Debug.Log($"video w({VideoClipData.width}) h({VideoClipData.height})");
        videoRenderTexture = new RenderTexture((int)VideoClipData.width, (int)VideoClipData.height, 16, RenderTextureFormat.ARGB32);


        // VideoPlayer automatically targets the camera backplane when it is added
        // to a camera object, no need to change videoPlayer.targetCamera.
        videoPlayer = this.AddComponent<VideoPlayer>(); 

        // Play on awake defaults to true. Set it to false to avoid the url set
        // below to auto-start playback since we're in Start().
        videoPlayer.playOnAwake = false;

        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoRenderTexture;

        // This will cause our Scene to be visible through the video being played.
        //videoPlayer.targetCameraAlpha = 0.7F;

        videoPlayer.clip = VideoClipData;

        // Skip the first 100 frames.
        //videoPlayer.frame = 100;

        // Restart from beginning when done.
        videoPlayer.isLooping = true;

        // Each time we reach the end, we slow down the playback by a factor of 10.
        //videoPlayer.loopPointReached += EndReached;

        // Start playback. This means the VideoPlayer may have to prepare (reserve
        // resources, pre-load a few frames, etc.). To better control the delays
        // associated with this preparation one can use videoPlayer.Prepare() along with
        // its prepareCompleted event.
        videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
        videoPlayer.Prepare();
    }

    protected void VideoPlayer_prepareCompleted(UnityEngine.Video.VideoPlayer source)
    {
        Debug.Log("prepare complete");
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
