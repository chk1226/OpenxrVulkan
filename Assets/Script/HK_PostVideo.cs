using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VIVE.OpenXR.CompositionLayer;

public class HK_PostVideo : MonoBehaviour
{
    protected RawImage planeImage = null;
    protected RectTransform rectTransform = null;
    protected CompositionLayer compositionLayer = null;
    protected RenderTexture videoRenderTexture = null;
    protected RenderTexture compositionRenderTexture = null;
    protected bool isReady = false;

    public bool UseCompositionLayer = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("StartCoroutine");
        StartCoroutine(Init(0.3f));
    }
    private IEnumerator Init(float waitTime)
    {
        while (true)
        {
            if (isReady) break; 

            if (!HK_VideoPlayer.VideoRenderTexture)
            {
                Debug.Log("WaitAndPrint " + Time.time);
                yield return new WaitForSeconds(waitTime);
            }
            else 
            {
                Debug.Log("VideoRenderTexture is ready" );

                this.videoRenderTexture = HK_VideoPlayer.VideoRenderTexture;
                this.compositionRenderTexture = new RenderTexture(videoRenderTexture.width / 2, videoRenderTexture.height, 16, RenderTextureFormat.ARGB32);

                if (!UseCompositionLayer)
                {
                    rectTransform = GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(compositionRenderTexture.width, compositionRenderTexture.height);

                    planeImage = GetComponent<RawImage>();
                    planeImage.texture = compositionRenderTexture;
                }
                else
                {
                    compositionLayer = GetComponent<CompositionLayer>();
                    //compositionLayer.texture = videoRenderTexture;
                    compositionLayer.texture = compositionRenderTexture;
                }
                isReady = true;
                break;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isReady)
        {
            Graphics.CopyTexture(videoRenderTexture, 0, 0, 0, 0, compositionRenderTexture.width , compositionRenderTexture.height, compositionRenderTexture, 0, 0, 0, 0);
        }

    }
}
