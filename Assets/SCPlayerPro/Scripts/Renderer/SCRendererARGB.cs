﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Rendering;

namespace Sttplay.MediaPlayer
{

    /// <summary>
    /// Renderer instance 
    /// The pixel format is ARGB, so 1 texture is needed
    /// format:(4x4)
    /// A R G B A R G B A R G B A R G B
    /// A R G B A R G B A R G B A R G B
    /// A R G B A R G B A R G B A R G B
    /// A R G B A R G B A R G B A R G B
    /// </summary>
    public class SCRendererARGB : SCRenderer
    {

        public override void InitRenderer(SCFrame frame)
        {
            SourceTextures = new Texture2D[1];
            RenderMaterial = new Material(Shader.Find("Sttplay/ARGB"));
            SourceTextures[0] = new Texture2D(frame.width, frame.height, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
            base.InitRenderer(frame);
        }

        public override void Renderer(SCFrame frame)
        {
            if (frame.format != (int)PixelFormat.ARGB)
            {
                Debug.LogError("The pixel format is different from the canvas type");
                return;
            }

            if (frame.linesize[0] != frame.width * 4)
                CopyImageData(frame);
            else
                this.frame = frame;

        }

        public override void Apply()
        {
            if (!isInit)
            {
                isInit = true;
                InitRenderer(frame);
                SetColorSpace(frame);
            }

            SourceTextures[0].LoadRawTextureData(frame.data[0], frame.width * frame.height * 4);

            SourceTextures[0].Apply();

            RenderMaterial.SetTexture("_Tex", SourceTextures[0]);
            base.Apply();
        }
    }
}