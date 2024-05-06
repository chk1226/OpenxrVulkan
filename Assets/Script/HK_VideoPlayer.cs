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
    public UnityEngine.Video.VideoPlayer VideoPlayer = null;
    public RenderTexture VideoRenderTexture = null;
    public string URL;

    protected RawImage rawImg = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!VideoRenderTexture) return;
        rawImg = gameObject.GetComponent<RawImage>();
        rawImg.texture = VideoRenderTexture;


        if (!VideoPlayer) return;
        // VideoPlayer automatically targets the camera backplane when it is added
        // to a camera object, no need to change videoPlayer.targetCamera.
        //videoPlayer = this.AddComponent<VideoPlayer>(); 

        // Play on awake defaults to true. Set it to false to avoid the url set
        // below to auto-start playback since we're in Start().
        VideoPlayer.playOnAwake = false;
        VideoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
        VideoPlayer.targetTexture = VideoRenderTexture;
        VideoPlayer.source = VideoSource.Url;
        VideoPlayer.url = HK_SCPlayerCtrl.GetFilePath(URL);

        // This will cause our Scene to be visible through the video being played.
        //videoPlayer.targetCameraAlpha = 0.7F;

        //videoPlayer.clip = VideoClipData;

        // Skip the first 100 frames.
        //videoPlayer.frame = 100;

        // Restart from beginning when done.
        VideoPlayer.isLooping = true;

        // Each time we reach the end, we slow down the playback by a factor of 10.
        //videoPlayer.loopPointReached += EndReached;

        // Start playback. This means the VideoPlayer may have to prepare (reserve
        // resources, pre-load a few frames, etc.). To better control the delays
        // associated with this preparation one can use videoPlayer.Prepare() along with
        // its prepareCompleted event.
        VideoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
        VideoPlayer.Prepare();
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
