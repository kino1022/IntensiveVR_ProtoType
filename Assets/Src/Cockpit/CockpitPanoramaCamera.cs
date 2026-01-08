using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace IntensiveVR.Cockpit
{
    /// <summary>
    /// 360度パノラマビューをコックピットに投影するカメラシステム
    /// </summary>
    public class CockpitPanoramaCamera : MonoBehaviour
    {
        [Header("Render Settings")]
        [Tooltip("コックピット球体のレンダラー")]
        [SerializeField] private Renderer cockpitSphereRenderer;
        
        [Tooltip("Cubemapの解像度")]
        [SerializeField] private int cubemapResolution = 2048;
        
        [Tooltip("出力するEquirectangularテクスチャの解像度")]
        [SerializeField] private Vector2Int equirectangularResolution = new Vector2Int(4096, 2048);
        
        [Header("Camera Settings")]
        [Tooltip("描画する最小距離")]
        [SerializeField] private float nearClipPlane = 0.1f;
        
        [Tooltip("描画する最大距離")]
        [SerializeField] private float farClipPlane = 1000f;
        
        [Tooltip("更新頻度（フレーム）")]
        [SerializeField] private int updateEveryNFrames = 1;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        
        private Camera panoramaCamera;
        private RenderTexture cubemapRT;
        private RenderTexture equirectangularRT;
        private Material equirectangularMaterial;
        private Material cockpitMaterial;
        private int frameCounter = 0;
        
        private void Awake()
        {
            InitializeCamera();
        }
        
        private void Start()
        {
            InitializeRenderTextures();
            InitializeMaterials();
        }
        
        private void InitializeCamera()
        {
            // カメラコンポーネントを取得または作成
            panoramaCamera = GetComponent<Camera>();
            if (panoramaCamera == null)
            {
                panoramaCamera = gameObject.AddComponent<Camera>();
            }
            
            // カメラの基本設定
            panoramaCamera.nearClipPlane = nearClipPlane;
            panoramaCamera.farClipPlane = farClipPlane;
            panoramaCamera.enabled = false; // 手動でレンダリング
            
            if (debugMode)
            {
                Debug.Log("[PanoramaCamera] Camera initialized");
            }
        }
        
        private void InitializeRenderTextures()
        {
            // Cubemap RenderTexture（6面）
            cubemapRT = new RenderTexture(cubemapResolution, cubemapResolution, 24)
            {
                dimension = TextureDimension.Cube,
                name = "PanoramaCubemap",
                hideFlags = HideFlags.HideAndDontSave
            };
            
            // Equirectangular RenderTexture（球体用）
            equirectangularRT = new RenderTexture(
                equirectangularResolution. x, 
                equirectangularResolution.y, 
                0, 
                RenderTextureFormat. ARGB32
            )
            {
                name = "PanoramaEquirectangular",
                filterMode = FilterMode. Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            
            if (debugMode)
            {
                Debug.Log($"[PanoramaCamera] RenderTextures created:  Cubemap={cubemapResolution}x{cubemapResolution}, Equirect={equirectangularResolution. x}x{equirectangularResolution.y}");
            }
        }
        
        private void InitializeMaterials()
        {
            // Cubemap→Equirectangular変換用シェーダー
            Shader convertShader = Shader.Find("Hidden/CubemapToEquirectangular");
            if (convertShader == null)
            {
                // フォールバック：シンプルな変換シェーダーを作成
                convertShader = CreateCubemapConvertShader();
            }
            
            equirectangularMaterial = new Material(convertShader);
            
            // コックピット球体のマテリアル
            if (cockpitSphereRenderer != null)
            {
                cockpitMaterial = cockpitSphereRenderer.material;
                cockpitMaterial.mainTexture = equirectangularRT;
                
                if (debugMode)
                {
                    Debug.Log("[PanoramaCamera] Materials initialized");
                }
            }
            else
            {
                Debug. LogError("[PanoramaCamera] Cockpit sphere renderer is null!");
            }
        }
        
        private void LateUpdate()
        {
            frameCounter++;
            
            if (frameCounter >= updateEveryNFrames)
            {
                RenderPanorama();
                frameCounter = 0;
            }
        }
        
        /// <summary>
        /// 360度パノラマをレンダリング
        /// </summary>
        private void RenderPanorama()
        {
            if (panoramaCamera == null || cubemapRT == null || equirectangularRT == null)
            {
                return;
            }
            
            // Step 1: Cubemapをレンダリング（6方向）
            panoramaCamera.RenderToCubemap(cubemapRT, 63); // 全6面をレンダリング
            
            // Step 2: Cubemap→Equirectangularに変換
            ConvertCubemapToEquirectangular();
        }
        
        /// <summary>
        /// CubemapをEquirectangular形式に変換
        /// </summary>
        private void ConvertCubemapToEquirectangular()
        {
            if (equirectangularMaterial == null) return;
            
            equirectangularMaterial.SetTexture("_Cubemap", cubemapRT);
            Graphics.Blit(null, equirectangularRT, equirectangularMaterial);
        }
        
        /// <summary>
        /// Cubemap→Equirectangular変換シェーダーを動的生成
        /// </summary>
        private Shader CreateCubemapConvertShader() {
            string shaderCode = @"
Shader ""Hidden/CubemapToEquirectangular""
{
    Properties
    {
        _Cubemap (""Cubemap"", CUBE) = """" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include ""UnityCG.cginc""
            
            samplerCUBE _Cubemap;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o. uv = v.uv;
                return o;
            }
            
            float3 EquirectangularToCubemap(float2 uv)
            {
                float theta = uv.y * UNITY_PI;
                float phi = uv.x * UNITY_PI * 2.0;
                
                float3 dir;
                dir.x = sin(theta) * cos(phi);
                dir.y = cos(theta);
                dir.z = sin(theta) * sin(phi);
                
                return dir;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = EquirectangularToCubemap(i.uv);
                return texCUBE(_Cubemap, dir);
            }
            ENDCG
        }
    }
}";
            return Shader.Find("Hidden/CubemapToEquirectangular") ??  
                   ShaderUtil.CreateShaderAsset(shaderCode);
        }
        
        /// <summary>
        /// レンダリング頻度を設定
        /// </summary>
        public void SetUpdateFrequency(int everyNFrames)
        {
            updateEveryNFrames = Mathf.Max(1, everyNFrames);
        }
        
        /// <summary>
        /// 解像度を変更
        /// </summary>
        public void SetResolution(int cubemapRes, Vector2Int equirectRes)
        {
            cubemapResolution = cubemapRes;
            equirectangularResolution = equirectRes;
            
            // RenderTextureを再作成
            if (cubemapRT != null) cubemapRT.Release();
            if (equirectangularRT != null) equirectangularRT.Release();
            
            InitializeRenderTextures();
            
            if (cockpitMaterial != null)
            {
                cockpitMaterial.mainTexture = equirectangularRT;
            }
        }
        
        private void OnDestroy()
        {
            if (cubemapRT != null)
            {
                cubemapRT.Release();
                Destroy(cubemapRT);
            }
            
            if (equirectangularRT != null)
            {
                equirectangularRT.Release();
                Destroy(equirectangularRT);
            }
            
            if (equirectangularMaterial != null)
            {
                Destroy(equirectangularMaterial);
            }
            
            if (cockpitMaterial != null)
            {
                Destroy(cockpitMaterial);
            }
        }
        
        [ContextMenu("Debug Status")]
        private void DebugStatus()
        {
            Debug.Log("=== Panorama Camera Status ===");
            Debug.Log($"Cubemap RT: {(cubemapRT != null ? $"{cubemapRT.width}x{cubemapRT.height}" : "NULL")}");
            Debug.Log($"Equirect RT: {(equirectangularRT != null ? $"{equirectangularRT.width}x{equirectangularRT.height}" : "NULL")}");
            Debug.Log($"Camera:  {(panoramaCamera != null ? "OK" : "NULL")}");
            Debug.Log($"Update Frequency: Every {updateEveryNFrames} frames");
        }
    }
}