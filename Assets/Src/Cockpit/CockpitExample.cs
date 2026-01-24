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
        [SerializeField] private CockpitPanoramaCamera cockpitProjection;
        
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
