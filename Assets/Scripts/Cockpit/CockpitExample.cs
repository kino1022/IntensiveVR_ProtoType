using UnityEngine;

namespace IntensiveVR.Cockpit.Examples
{
    /// <summary>
    /// コックピットシステムの使用例
    /// Example usage of the cockpit system
    /// </summary>
    public class CockpitExample : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CockpitManager cockpitManager;
        [SerializeField] private Transform robotHead;
        [SerializeField] private CockpitCameraProjection cockpitProjection;
        
        [Header("Settings")]
        [SerializeField] private bool enableCockpitOnStart = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.C;
        
        private bool isCockpitEnabled;
        
        private void Start()
        {
            if (cockpitManager != null && robotHead != null)
            {
                // ロボットの頭部に投影カメラをアタッチ
                // Attach projection camera to robot's head
                cockpitManager.AttachProjectionCameraToRobot(robotHead);
                
                if (enableCockpitOnStart)
                {
                    EnableCockpit();
                }
            }
        }
        
        private void Update()
        {
            // キーでコックピットモードの切り替え
            // Toggle cockpit mode with key
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleCockpit();
            }
        }
        
        /// <summary>
        /// コックピットモードを有効化
        /// Enable cockpit mode
        /// </summary>
        public void EnableCockpit()
        {
            if (cockpitManager != null)
            {
                cockpitManager.SetCockpitActive(true);
                isCockpitEnabled = true;
                Debug.Log("Cockpit mode enabled");
            }
        }
        
        /// <summary>
        /// コックピットモードを無効化
        /// Disable cockpit mode
        /// </summary>
        public void DisableCockpit()
        {
            if (cockpitManager != null)
            {
                cockpitManager.SetCockpitActive(false);
                isCockpitEnabled = false;
                Debug.Log("Cockpit mode disabled");
            }
        }
        
        /// <summary>
        /// コックピットモードを切り替え
        /// Toggle cockpit mode
        /// </summary>
        public void ToggleCockpit()
        {
            if (isCockpitEnabled)
            {
                DisableCockpit();
            }
            else
            {
                EnableCockpit();
            }
        }
        
        /// <summary>
        /// RenderTextureの解像度を変更（パフォーマンス調整用）
        /// Change RenderTexture resolution (for performance tuning)
        /// </summary>
        public void SetRenderQuality(RenderQuality quality)
        {
            if (cockpitProjection == null) return;
            
            switch (quality)
            {
                case RenderQuality.Low:
                    cockpitProjection.SetRenderTextureResolution(1024, 1024);
                    Debug.Log("Render quality set to Low (1024x1024)");
                    break;
                    
                case RenderQuality.Medium:
                    cockpitProjection.SetRenderTextureResolution(1536, 1536);
                    Debug.Log("Render quality set to Medium (1536x1536)");
                    break;
                    
                case RenderQuality.High:
                    cockpitProjection.SetRenderTextureResolution(2048, 2048);
                    Debug.Log("Render quality set to High (2048x2048)");
                    break;
                    
                case RenderQuality.VeryHigh:
                    cockpitProjection.SetRenderTextureResolution(4096, 4096);
                    Debug.Log("Render quality set to Very High (4096x4096)");
                    break;
            }
        }
        
        /// <summary>
        /// コックピットの位置を変更
        /// Change cockpit position
        /// </summary>
        public void MoveCockpit(Vector3 newPosition)
        {
            if (cockpitManager != null)
            {
                cockpitManager.SetCockpitPosition(newPosition);
                Debug.Log($"Cockpit moved to {newPosition}");
            }
        }
    }
    
    /// <summary>
    /// レンダリング品質の設定
    /// Render quality settings
    /// </summary>
    public enum RenderQuality
    {
        Low,      // 1024x1024 - モバイルVR推奨 / Recommended for mobile VR
        Medium,   // 1536x1536 - 低スペックPC VR / Low-spec PC VR
        High,     // 2048x2048 - 標準PC VR / Standard PC VR
        VeryHigh  // 4096x4096 - ハイエンドPC VR / High-end PC VR
    }
}
