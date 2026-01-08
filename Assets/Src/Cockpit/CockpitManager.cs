using UnityEngine;

namespace IntensiveVR.Cockpit
{
    /// <summary>
    /// VRロボット操縦用コックピットシステムのマネージャー
    /// カメラ投影とコックピット環境を統合管理します
    /// </summary>
    public class CockpitManager : MonoBehaviour
    {
        [Header("Components")]
        [Tooltip("投影カメラオブジェクト")]
        [SerializeField] private GameObject projectionCameraObject;
        
        [Tooltip("コックピット球体オブジェクト")]
        [SerializeField] private GameObject cockpitSphereObject;
        
        [Tooltip("VRカメラ（プレイヤーの頭）")]
        [SerializeField] private Camera vrCamera;
        
        [Header("Cockpit Position")]
        [Tooltip("コックピットの隔離された座標")]
        [SerializeField] private Vector3 isolatedPosition = new Vector3(1000f, 1000f, 1000f);
        
        [Tooltip("VRカメラをコックピット内に配置")]
        [SerializeField] private bool placeVRCameraInCockpit = true;
        
        [Tooltip("XR Origin（自動検索される場合は空でも可）")]
        [SerializeField] private Transform xrOrigin;
        
        private CockpitCameraProjection projectionSystem;
        private Transform cockpitTransform;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            SetupCockpitPosition();
            
            if (placeVRCameraInCockpit && vrCamera != null)
            {
                PositionVRCameraInCockpit();
            }
        }
        
        private void InitializeComponents()
        {
            if (projectionCameraObject != null)
            {
                projectionSystem = projectionCameraObject.GetComponent<CockpitCameraProjection>();
            }
            
            if (cockpitSphereObject != null)
            {
                cockpitTransform = cockpitSphereObject.transform;
            }
        }
        
        /// <summary>
        /// コックピットを隔離された座標に配置
        /// </summary>
        private void SetupCockpitPosition()
        {
            if (cockpitTransform != null)
            {
                cockpitTransform.position = isolatedPosition;
            }
            
            if (projectionCameraObject != null)
            {
                // 投影カメラは元の位置に残す（ロボットの視点）
                // またはロボットの頭部にアタッチすることを想定
            }
        }
        
        /// <summary>
        /// VRカメラをコックピット内に配置
        /// </summary>
        private void PositionVRCameraInCockpit()
        {
            if (vrCamera != null && cockpitTransform != null)
            {
                Transform vrCameraTransform = vrCamera.transform;
                
                // VRカメラの親をコックピットに設定するか、
                // VRカメラをコックピットの中心に配置
                // （XR Originの扱いによって調整が必要）
                
                // 注: XR Originを使用している場合は、Origin全体を移動する必要がある
                Transform xrOrigin = FindXROrigin();
                if (xrOrigin != null)
                {
                    xrOrigin.position = isolatedPosition;
                }
            }
        }
        
        /// <summary>
        /// XR Originを検索（キャッシュを優先）
        /// </summary>
        private Transform FindXROrigin()
        {
            // Already cached
            if (xrOrigin != null)
            {
                return xrOrigin;
            }
            
            // Try to find and cache
            GameObject xrOriginObject = GameObject.Find("XR Origin") ?? GameObject.Find("XR Rig");
            if (xrOriginObject != null)
            {
                xrOrigin = xrOriginObject.transform;
            }
            
            return xrOrigin;
        }
        
        /// <summary>
        /// コックピットの位置を変更
        /// </summary>
        public void SetCockpitPosition(Vector3 position)
        {
            isolatedPosition = position;
            SetupCockpitPosition();
            
            if (placeVRCameraInCockpit)
            {
                PositionVRCameraInCockpit();
            }
        }
        
        /// <summary>
        /// 投影カメラをロボットのトランスフォームに追従させる
        /// </summary>
        public void AttachProjectionCameraToRobot(Transform robotHeadTransform)
        {
            if (projectionCameraObject != null && robotHeadTransform != null)
            {
                projectionCameraObject.transform.SetParent(robotHeadTransform);
                projectionCameraObject.transform.localPosition = Vector3.zero;
                projectionCameraObject.transform.localRotation = Quaternion.identity;
            }
        }
        
        /// <summary>
        /// コックピットシステムを有効/無効化
        /// </summary>
        public void SetCockpitActive(bool active)
        {
            if (cockpitSphereObject != null)
            {
                cockpitSphereObject.SetActive(active);
            }
            
            if (projectionCameraObject != null)
            {
                projectionCameraObject.SetActive(active);
            }
        }
    }
}
