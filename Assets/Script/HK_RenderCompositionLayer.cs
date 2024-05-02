using Sttplay.MediaPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VIVE.OpenXR.CompositionLayer;



public class HK_RenderCompositionLayer : SCRenderTarget
{
    /// <summary>
    /// renderer target
    /// </summary>
    protected CompositionLayer compositionLayer = null;


    private void Start()
    {
    }

    protected void CreateCompositionLayer(Texture texture)
    {
        if (compositionLayer) return;

        compositionLayer = gameObject.AddComponent<CompositionLayer>();
        compositionLayer.layerType = CompositionLayer.LayerType.Overlay;
        compositionLayer.layerShape = CompositionLayer.LayerShape.Quad;
        compositionLayer.layerVisibility = CompositionLayer.Visibility.Both;
        compositionLayer.isDynamicLayer = true;
        compositionLayer.texture = texture;
    }
    private void Awake()
    {
        //if (!compositionLayer)
        //{
        //    CreateCompositionLayer(defaultTexture);
        //}

        //compositionLayer.texture = defaultTexture;
    }

    protected override void OnRendererChanged()
    {
        if (player.OpenSuccessed)
        {
            if(player.VideoRenderer.SCRenderer.SyntheticTexture)
            {
                CreateCompositionLayer(player.VideoRenderer.SCRenderer.SyntheticTexture);
            }

        }
    }

    private void OnEnable()
    {
        OnRendererChanged();
    }
}
