using Mewlist;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

class MassiveCloudsPass : CustomPass
{
    public MassiveClouds MassiveClouds;

    private Material     fullscreenPassMaterial;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        fullscreenPassMaterial = new Material(Shader.Find("Hidden/FullScreen/MassiveCloudsCustomPass"));
    }

    protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera camera, CullingResults cullingResult)
    {
        if (MassiveClouds == null) return;

        ResolveMSAAColorBuffer(cmd, camera);

        var format = RenderTextureFormat.ARGB64;
        var formatAlpha = RenderTextureFormat.DefaultHDR;

        RTHandle cameraColorBuffer;
        RTHandle cameraDepthBuffer;
        GetCameraBuffers(out cameraColorBuffer, out cameraDepthBuffer);

        MassiveClouds.BuildCommandBufferHDRP(cmd, camera, cameraColorBuffer,
            format, formatAlpha, fullscreenPassMaterial);
    }

    protected override void Cleanup()
    {
        if (Application.isPlaying)
            Object.Destroy(fullscreenPassMaterial);
        else
            Object.DestroyImmediate(fullscreenPassMaterial);
    }
}