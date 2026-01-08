using System;
using UnityEngine;

namespace IntensiveVR.Cockpit
{
    /// <summary>
    /// VRコックピット用のカメラ投影システム
    /// カメラ映像を球体の内側に投影してコックピット環境を作成します
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CockpitCameraProjection : MonoBehaviour
    {
        [Header("Render Settings")]
        [Tooltip("カメラ映像を描画するレンダーテクスチャ")]
        [SerializeField] private RenderTexture renderTexture;
        
        [Tooltip("コックピット球体のレンダラー")]
        [SerializeField] private Renderer cockpitSphereRenderer;
        
        [Header("Camera Settings")]
        [Tooltip("カメラの視野角")]
        [Range(60f, 179f)]
        [SerializeField] private float fieldOfView = 90f;
        
        [Tooltip("描画する最小距離")]
        [SerializeField] private float nearClipPlane = 0.1f;
        
        [Tooltip("描画する最大距離")]
        [SerializeField] private float farClipPlane = 1000f;
        
        [Header("Cockpit Settings")]
        [Tooltip("コックピット球体の半径")]
        [SerializeField] private float cockpitRadius = 2f;
        
        [Tooltip("コックピットの位置オフセット")]
        [SerializeField] private Vector3 cockpitOffset = Vector3.zero;
        
        private Camera projectionCamera;
        private Material cockpitMaterial;
        
        private void Awake()
        {
            projectionCamera = GetComponent<Camera>();
            InitializeCamera();
            InitializeMaterial();
        }
        
        private void Start()
        {
            if (renderTexture == null)
            {
                CreateRenderTexture();
            }
            
            ApplyRenderTexture();
        }

        private void InitializeCamera()
        {
            if (projectionCamera != null)
            {
                projectionCamera.fieldOfView = fieldOfView;
                projectionCamera.nearClipPlane = nearClipPlane;
                projectionCamera.farClipPlane = farClipPlane;
            }
        }
        
        private void InitializeMaterial()
        {
            if (cockpitSphereRenderer != null)
            {
                // Use sharedMaterial to avoid creating instances
                // Material instance will be created only when needed
                cockpitMaterial = cockpitSphereRenderer.sharedMaterial;
            }
        }
        
        private void CreateRenderTexture()
        {
            renderTexture = new RenderTexture(2048, 2048, 24);
            renderTexture.name = "CockpitProjectionRT";
            renderTexture.antiAliasing = 4;
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.wrapMode = TextureWrapMode.Clamp;
        }
        
        private void ApplyRenderTexture()
        {
            if (projectionCamera != null && renderTexture != null)
            {
                projectionCamera.targetTexture = renderTexture;
            }
            
            if (cockpitMaterial != null && renderTexture != null)
            {
                cockpitMaterial.mainTexture = renderTexture;
            }
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && projectionCamera != null)
            {
                InitializeCamera();
            }
        }
        
        private void OnDestroy()
        {
            if (renderTexture != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(renderTexture);
                }
                else
                {
                    DestroyImmediate(renderTexture);
                }
            }
            
            // Clean up instanced material if created
            if (cockpitMaterial != null && Application.isPlaying)
            {
                Destroy(cockpitMaterial);
            }
        }
        
        /// <summary>
        /// コックピット球体の半径を設定
        /// </summary>
        public void SetCockpitRadius(float radius)
        {
            cockpitRadius = Mathf.Max(0.1f, radius);
            UpdateCockpitScale();
        }
        
        /// <summary>
        /// コックピット球体のスケールを更新
        /// </summary>
        private void UpdateCockpitScale()
        {
            if (cockpitSphereRenderer != null)
            {
                cockpitSphereRenderer.transform.localScale = Vector3.one * cockpitRadius * 2f;
            }
        }
        
        /// <summary>
        /// レンダーテクスチャの解像度を設定
        /// </summary>
        public void SetRenderTextureResolution(int width, int height)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                
                // 新しいRenderTextureを作成
                var oldRT = renderTexture;
                renderTexture = new RenderTexture(width, height, 24);
                renderTexture.name = "CockpitProjectionRT";
                renderTexture.antiAliasing = oldRT.antiAliasing;
                renderTexture.filterMode = oldRT.filterMode;
                renderTexture.wrapMode = oldRT.wrapMode;
                
                // 古いRenderTextureを破棄
                if (Application.isPlaying)
                {
                    Destroy(oldRT);
                }
                else
                {
                    DestroyImmediate(oldRT);
                }
                
                ApplyRenderTexture();
            }
        }
    }
}
