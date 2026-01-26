using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace IntensiveVR.Cockpit
{
    /// <summary>
    /// VRコックピット用のカメラ投影システム
    /// カメラ映像を球体の内側に投影してコックピット環境を作成します
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CockpitCameraProjection : MonoBehaviour {
        /*
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
    */
        public class CockpitPanoramaCamera : MonoBehaviour {
            [Header("Render Settings")]
            [SerializeField]
            private Renderer cockpitSphereRenderer;

            [SerializeField]
            private int cubemapResolution = 2048;

            [SerializeField]
            private Vector2Int equirectangularResolution = new Vector2Int(4096, 2048);

            [Header("Camera Settings")]
            [SerializeField]
            private float nearClipPlane = 0.1f;

            [SerializeField]
            private float farClipPlane = 1000f;

            [SerializeField]
            private int updateEveryNFrames = 1;

            [Header("Debug")]
            [SerializeField]
            private bool debugMode = true; // ✅ デフォルトでON

            [SerializeField]
            private bool showDetailedLogs = true; // ✅ 追加

            private Camera panoramaCamera;
            private RenderTexture cubemapRT;
            private RenderTexture equirectangularRT;
            private Material equirectangularMaterial;
            private Material cockpitMaterial;
            private int frameCounter = 0;
            private int renderCount = 0; // ✅ レンダリング回数カウント

            private void Awake() {
                Debug.Log("=== [PanoramaCamera] Awake START ===");
                InitializeCamera();
                Debug.Log("=== [PanoramaCamera] Awake END ===");
            }

            private void Start() {
                Debug.Log("=== [PanoramaCamera] Start BEGIN ===");
                InitializeRenderTextures();
                InitializeMaterials();
                LogSetupStatus();
                Debug.Log("=== [PanoramaCamera] Start END ===");
            }

            private void InitializeCamera() {
                panoramaCamera = GetComponent<Camera>();
                if (panoramaCamera == null) {
                    Debug.Log("[PanoramaCamera] Camera component not found, adding...");
                    panoramaCamera = gameObject.AddComponent<Camera>();
                }

                panoramaCamera.nearClipPlane = nearClipPlane;
                panoramaCamera.farClipPlane = farClipPlane;
                panoramaCamera.enabled = false; // 手動でレンダリング（これは正常）

                // ✅ Editor/HMD無しでも動作するよう設定
#if UNITY_EDITOR
                panoramaCamera.allowMSAA = false;
                panoramaCamera.allowHDR = false;
#endif

                // ✅ AudioListenerを削除
                var audioListener = GetComponent<AudioListener>();
                if (audioListener != null) {
                    Destroy(audioListener);
                    Debug.Log("[PanoramaCamera] Removed AudioListener");
                }

                Debug.Log(
                    $"[PanoramaCamera] Camera initialized (enabled={panoramaCamera.enabled} - THIS IS INTENTIONAL)");
            }

            private void InitializeRenderTextures() {
                // Cubemap RenderTexture
                cubemapRT = new RenderTexture(cubemapResolution, cubemapResolution, 24) {
                    dimension = TextureDimension.Cube,
                    name = "PanoramaCubemap",
                    hideFlags = HideFlags.HideAndDontSave
                };
                cubemapRT.Create(); // ✅ 明示的に作成

                // Equirectangular RenderTexture
                equirectangularRT = new RenderTexture(
                    equirectangularResolution.x,
                    equirectangularResolution.y,
                    0,
                    RenderTextureFormat.ARGB32) {
                    name = "PanoramaEquirectangular",
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };
                equirectangularRT.Create(); // ✅ 明示的に作成

                Debug.Log(
                    $"[PanoramaCamera] RenderTextures created: Cubemap={cubemapResolution}x{cubemapResolution}, Equirect={equirectangularResolution.x}x{equirectangularResolution.y}");
            }

            private void InitializeMaterials() {
                // Cubemap→Equirectangular変換用シェーダー
                Shader convertShader = Shader.Find("Hidden/CubemapToEquirectangular");
                if (convertShader == null) {
                    Debug.LogError("[PanoramaCamera] Shader 'Hidden/CubemapToEquirectangular' not found!");
                    return;
                }

                equirectangularMaterial = new Material(convertShader);
                Debug.Log("[PanoramaCamera] Equirectangular material created");

                // コックピット球体のマテリアル
                if (cockpitSphereRenderer != null) {
                    cockpitMaterial = cockpitSphereRenderer.material; // インスタンスを作成
                    cockpitMaterial.mainTexture = equirectangularRT;
                    Debug.Log($"[PanoramaCamera] Cockpit material assigned to: {cockpitSphereRenderer.name}");
                }
                else {
                    Debug.LogError("[PanoramaCamera] Cockpit Sphere Renderer is NULL! Assign it in Inspector!");
                }
            }

            private void LateUpdate() {
                frameCounter++;

                if (frameCounter >= updateEveryNFrames) {
                    RenderPanorama();
                    frameCounter = 0;
                }
            }

            private void RenderPanorama() {
                if (panoramaCamera == null || cubemapRT == null || equirectangularRT == null) {
                    if (showDetailedLogs) {
                        Debug.LogError(
                            $"[PanoramaCamera] Cannot render: camera={panoramaCamera != null}, cubemapRT={cubemapRT != null}, equirectRT={equirectangularRT != null}");
                    }

                    return;
                }

                // Step 1: Cubemapをレンダリング
                panoramaCamera.RenderToCubemap(cubemapRT, 63);

                // Step 2: Cubemap→Equirectangularに変換
                ConvertCubemapToEquirectangular();

                renderCount++;

                // ✅ 10フレームごとにログ出力
                if (showDetailedLogs && renderCount % 10 == 0) {
                    Debug.Log($"[PanoramaCamera] Rendered {renderCount} times - Position: {transform.position}");
                }
            }

            private void ConvertCubemapToEquirectangular() {
                if (equirectangularMaterial == null) {
                    Debug.LogError("[PanoramaCamera] Equirectangular material is null!");
                    return;
                }

                equirectangularMaterial.SetTexture("_Cubemap", cubemapRT);
                Graphics.Blit(null, equirectangularRT, equirectangularMaterial);
            }

            private void LogSetupStatus() {
                Debug.Log("========================================");
                Debug.Log("[PanoramaCamera] Setup Status:");
                Debug.Log($"  Camera: {panoramaCamera != null} (enabled={panoramaCamera?.enabled} - should be FALSE)");
                Debug.Log($"  GameObject active: {gameObject.activeSelf}");
                Debug.Log($"  Cubemap RT: {cubemapRT != null} ({cubemapRT?.width}x{cubemapRT?.height})");
                Debug.Log(
                    $"  Equirect RT: {equirectangularRT != null} ({equirectangularRT?.width}x{equirectangularRT?.height})");
                Debug.Log($"  Cockpit Sphere Renderer: {cockpitSphereRenderer != null}");
                Debug.Log($"  Cockpit Material assigned: {cockpitMaterial != null}");
                Debug.Log($"  Update frequency: Every {updateEveryNFrames} frame(s)");
                Debug.Log("========================================");
            }

            [ContextMenu("Force Render Now")]
            private void ForceRenderNow() {
                Debug.Log("[PanoramaCamera] Force rendering...");
                RenderPanorama();
                Debug.Log("[PanoramaCamera] Force render complete");
            }

            [ContextMenu("Log Current Status")]
            private void LogCurrentStatus() {
                LogSetupStatus();
                Debug.Log($"  Total renders: {renderCount}");
                Debug.Log($"  Current position: {transform.position}");
                Debug.Log($"  Current rotation: {transform.rotation.eulerAngles}");
            }

            private void OnDestroy() {
                if (cubemapRT != null) {
                    cubemapRT.Release();
                    Destroy(cubemapRT);
                }

                if (equirectangularRT != null) {
                    equirectangularRT.Release();
                    Destroy(equirectangularRT);
                }

                if (equirectangularMaterial != null) {
                    Destroy(equirectangularMaterial);
                }

                if (cockpitMaterial != null) {
                    Destroy(cockpitMaterial);
                }
            }
        }
    }
}
