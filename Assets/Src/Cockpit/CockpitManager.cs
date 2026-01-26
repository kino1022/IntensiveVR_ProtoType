using Unity.XR.CoreUtils;
using UnityEngine;

namespace IntensiveVR.Cockpit
{
    /*
    public class CockpitManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CockpitPanoramaCamera projectionCameraObject;
        [SerializeField] private GameObject cockpitSphereObject;
        [SerializeField] private Camera vrCamera;
        
        [Header("Cockpit Position")]
        [SerializeField] private Vector3 isolatedPosition = new Vector3(1000f, 1000f, 1000f);
        [SerializeField] private bool placeVRCameraInCockpit = true;
        [SerializeField] private Transform xrOrigin;
        
        [Header("Physics Control")]
        [Tooltip("コックピット内では重力を無効化")]
        [SerializeField] private bool disableGravityInCockpit = true;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        
        private CockpitCameraProjection projectionSystem;
        private Transform cockpitTransform;
        private Vector3 initialXROriginPosition;
        
        // 物理コンポーネントのキャッシュ
        private CharacterController characterController;
        private Rigidbody rigidBody;
        private bool wasUsingGravity = false;
        private bool characterControllerWasEnabled = false;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            SetupCockpitPosition();
            
            if (placeVRCameraInCockpit)
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
                cockpitTransform = cockpitSphereObject. transform;
            }
            
            // XR Originを検索
            if (xrOrigin == null)
            {
                xrOrigin = FindXROrigin();
            }
            
            // 物理コンポーネントを取得
            if (xrOrigin != null)
            {
                initialXROriginPosition = xrOrigin.position;
                characterController = xrOrigin.GetComponent<CharacterController>();
                rigidBody = xrOrigin. GetComponent<Rigidbody>();
                
                if (characterController != null && debugMode)
                {
                    Debug.Log($"[CockpitManager] Found CharacterController on XR Origin");
                }
                
                if (rigidBody != null && debugMode)
                {
                    Debug.Log($"[CockpitManager] Found Rigidbody on XR Origin (useGravity: {rigidBody.useGravity})");
                }
            }
        }
        
        private void SetupCockpitPosition()
        {
            if (cockpitTransform != null)
            {
                cockpitTransform.position = isolatedPosition;
                
                if (debugMode)
                {
                    Debug.Log($"[CockpitManager] Cockpit sphere positioned at {isolatedPosition}");
                }
            }
        }
        
        /// <summary>
        /// VRカメラ（XR Origin）をコックピット内に配置
        /// </summary>
        private void PositionVRCameraInCockpit()
        {
            Transform origin = xrOrigin ??  FindXROrigin();
            
            if (origin == null)
            {
                Debug.LogError("[CockpitManager] XR Origin not found!");
                return;
            }
            
            if (cockpitTransform == null)
            {
                Debug.LogError("[CockpitManager] Cockpit transform is null!");
                return;
            }
            
            // 重力を無効化
            if (disableGravityInCockpit)
            {
                DisablePhysics();
            }
            
            // XR Origin全体を移動
            if (characterController != null)
            {
                // CharacterControllerの場合はMoveを使う
                characterController.enabled = false;
                origin.position = isolatedPosition;
                characterController.enabled = true;
            }
            else
            {
                // 通常のTransform移動
                origin.position = isolatedPosition;
            }
            
            if (debugMode)
            {
                Debug.Log($"[CockpitManager] XR Origin moved to {isolatedPosition}");
                
                if (vrCamera != null)
                {
                    Debug.Log($"[CockpitManager] VR Camera world position: {vrCamera.transform.position}");
                    float distance = Vector3.Distance(vrCamera. transform.position, isolatedPosition);
                    Debug.Log($"[CockpitManager] Distance from cockpit center: {distance}");
                }
            }
        }
        
        /// <summary>
        /// 物理演算を無効化
        /// </summary>
        private void DisablePhysics()
        {
            if (characterController != null)
            {
                characterControllerWasEnabled = characterController. enabled;
                characterController.enabled = false;
                
                if (debugMode)
                {
                    Debug.Log("[CockpitManager] CharacterController disabled");
                }
            }
            
            if (rigidBody != null)
            {
                wasUsingGravity = rigidBody.useGravity;
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
                
                if (debugMode)
                {
                    Debug.Log("[CockpitManager] Rigidbody gravity disabled and set to kinematic");
                }
            }
        }
        
        /// <summary>
        /// 物理演算を再有効化
        /// </summary>
        private void EnablePhysics()
        {
            if (characterController != null && characterControllerWasEnabled)
            {
                characterController.enabled = true;
                
                if (debugMode)
                {
                    Debug.Log("[CockpitManager] CharacterController re-enabled");
                }
            }
            
            if (rigidBody != null)
            {
                rigidBody.useGravity = wasUsingGravity;
                rigidBody.isKinematic = false;
                
                if (debugMode)
                {
                    Debug.Log("[CockpitManager] Rigidbody gravity restored");
                }
            }
        }
        
        /// <summary>
        /// XR Originを検索
        /// </summary>
        private Transform FindXROrigin()
        {
            if (xrOrigin != null)
            {
                return xrOrigin;
            }

            GameObject xrOriginObject = GameObject.Find("XR Origin")
                                        ?? GameObject.Find("XR Rig")
                                        ?? GameObject.Find("XROrigin")
                                        ?? FindAnyObjectByType<XROrigin>().gameObject;
            
            if (xrOriginObject != null)
            {
                xrOrigin = xrOriginObject.transform;
                
                if (debugMode)
                {
                    Debug.Log($"[CockpitManager] Found XR Origin:  {xrOrigin.name}");
                }
            }
            else if (vrCamera != null)
            {
                // VRカメラの親を遡って検索
                Transform current = vrCamera.transform. parent;
                while (current != null)
                {
                    if (current.name. Contains("XR") || current.name.Contains("Origin") || current.name.Contains("Rig"))
                    {
                        xrOrigin = current;
                        if (debugMode)
                        {
                            Debug.Log($"[CockpitManager] Found XR Origin by traversing:  {xrOrigin.name}");
                        }
                        break;
                    }
                    current = current.parent;
                }
            }
            
            return xrOrigin;
        }
        
        public void SetCockpitPosition(Vector3 position)
        {
            isolatedPosition = position;
            SetupCockpitPosition();
            
            if (placeVRCameraInCockpit)
            {
                PositionVRCameraInCockpit();
            }
        }
        
        public void AttachProjectionCameraToRobot(Transform robotHeadTransform)
        {
            if (projectionCameraObject != null && robotHeadTransform != null)
            {
                projectionCameraObject.transform.SetParent(robotHeadTransform);
                projectionCameraObject.transform.localPosition = Vector3.zero;
                projectionCameraObject.transform.localRotation = Quaternion.identity;
                
                if (debugMode)
                {
                    Debug.Log($"[CockpitManager] Projection camera attached to {robotHeadTransform.name}");
                }
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
                projectionCameraObject.gameObject.SetActive(active);
            }
            
            // 物理の制御
            if (disableGravityInCockpit)
            {
                if (active)
                {
                    DisablePhysics();
                }
                else
                {
                    EnablePhysics();
                }
            }
            
            if (debugMode)
            {
                Debug.Log($"[CockpitManager] Cockpit system set to {(active ? "active" : "inactive")}");
            }
        }
        
        /// <summary>
        /// XR Originを初期位置に戻す
        /// </summary>
        public void ResetVRCameraPosition()
        {
            if (xrOrigin != null)
            {
                if (disableGravityInCockpit)
                {
                    EnablePhysics();
                }
                
                xrOrigin. position = initialXROriginPosition;
                
                if (debugMode)
                {
                    Debug.Log($"[CockpitManager] XR Origin reset to {initialXROriginPosition}");
                }
            }
        }
        
        [ContextMenu("Debug Cockpit Status")]
        private void DebugCockpitStatus()
        {
            Debug.Log("=== Cockpit Manager Status ===");
            Debug. Log($"XR Origin: {(xrOrigin != null ? xrOrigin.name : "NULL")}");
            Debug.Log($"XR Origin Position: {xrOrigin?.position}");
            Debug.Log($"Cockpit Position: {isolatedPosition}");
            
            if (characterController != null)
            {
                Debug.Log($"CharacterController:  Enabled={characterController.enabled}");
            }
            
            if (rigidBody != null)
            {
                Debug.Log($"Rigidbody:  useGravity={rigidBody.useGravity}, isKinematic={rigidBody.isKinematic}");
            }
            
            if (vrCamera != null)
            {
                Debug.Log($"VR Camera Position: {vrCamera.transform.position}");
                float distance = Vector3.Distance(vrCamera.transform.position, isolatedPosition);
                Debug.Log($"Distance from cockpit center: {distance}");
            }
        }
        
        private void OnDestroy()
        {
            // クリーンアップ：物理を元に戻す
            if (disableGravityInCockpit)
            {
                EnablePhysics();
            }
        }
    }
    */
    public class CockpitManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CockpitPanoramaCamera projectionCameraObject;
        [SerializeField] private GameObject cockpitSphereObject;
        [SerializeField] private Camera vrCamera;
        
        [Header("Cockpit Position")]
        [SerializeField] private Vector3 isolatedPosition = new Vector3(1000f, 1000f, 1000f);
        [SerializeField] private bool placeVRCameraInCockpit = true;
        [SerializeField] private Transform xrOrigin;
        
        [Header("Physics Control")]
        [SerializeField] private bool disableGravityInCockpit = true;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true; // ✅ デフォルトでON
        
        private CockpitCameraProjection projectionSystem;
        private Transform cockpitTransform;
        private Vector3 initialXROriginPosition;
        
        private CharacterController characterController;
        private Rigidbody rigidBody;
        private bool wasUsingGravity = false;
        private bool characterControllerWasEnabled = false;
        
        private void Awake()
        {
            Debug.Log("=== [CockpitManager] Awake START ===");
            InitializeComponents();
            Debug.Log("=== [CockpitManager] Awake END ===");
        }
        
        private void Start()
        {
            Debug.Log("=== [CockpitManager] Start BEGIN ===");
            SetupCockpitPosition();
            
            if (placeVRCameraInCockpit)
            {
                PositionVRCameraInCockpit();
            }
            Debug.Log("=== [CockpitManager] Start END ===");
        }
        
        private void InitializeComponents()
        {
            Debug.Log($"[CockpitManager] InitializeComponents - projectionCameraObject: {projectionCameraObject != null}");
            
            if (projectionCameraObject != null)
            {
                Debug.Log($"[CockpitManager] ProjectionCameraObject found: {projectionCameraObject.gameObject.name}");
                Debug.Log($"  - Active: {projectionCameraObject.gameObject.activeSelf}");
                Debug.Log($"  - Position: {projectionCameraObject.transform.position}");
                Debug.Log($"  - Parent: {(projectionCameraObject.transform.parent ? projectionCameraObject.transform.parent.name : "None")}");
                
                projectionSystem = projectionCameraObject.GetComponent<CockpitCameraProjection>();
            }
            
            if (cockpitSphereObject != null)
            {
                cockpitTransform = cockpitSphereObject.transform;
            }
            
            if (xrOrigin == null)
            {
                xrOrigin = FindXROrigin();
            }
            
            if (xrOrigin != null)
            {
                initialXROriginPosition = xrOrigin.position;
                characterController = xrOrigin.GetComponent<CharacterController>();
                rigidBody = xrOrigin.GetComponent<Rigidbody>();
                
                if (characterController != null && debugMode)
                {
                    Debug.Log($"[CockpitManager] Found CharacterController on XR Origin");
                }
                
                if (rigidBody != null && debugMode)
                {
                    Debug.Log($"[CockpitManager] Found Rigidbody on XR Origin (useGravity: {rigidBody.useGravity})");
                }
            }
        }
        
        private void SetupCockpitPosition()
        {
            if (cockpitTransform != null)
            {
                cockpitTransform.position = isolatedPosition;
                
                if (debugMode)
                {
                    Debug.Log($"[CockpitManager] Cockpit sphere positioned at {isolatedPosition}");
                }
            }
        }
        
        private void PositionVRCameraInCockpit()
        {
            Transform origin = xrOrigin ?? FindXROrigin();
            
            if (origin == null)
            {
                Debug.LogError("[CockpitManager] XR Origin not found!");
                return;
            }
            
            if (cockpitTransform == null)
            {
                Debug.LogError("[CockpitManager] Cockpit transform is null!");
                return;
            }
            
            if (disableGravityInCockpit)
            {
                DisablePhysics();
            }
            
            origin.position = isolatedPosition;
            
            if (debugMode)
            {
                Debug.Log($"[CockpitManager] XR Origin moved to {isolatedPosition}");
                
                if (vrCamera != null)
                {
                    Debug.Log($"[CockpitManager] VR Camera world position: {vrCamera.transform.position}");
                    float distance = Vector3.Distance(vrCamera.transform.position, isolatedPosition);
                    Debug.Log($"[CockpitManager] Distance from cockpit center: {distance}");
                }
            }
        }
        
        private void DisablePhysics()
        {
            if (rigidBody != null)
            {
                wasUsingGravity = rigidBody.useGravity;
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
                
                if (debugMode)
                {
                    Debug.Log("[CockpitManager] Rigidbody gravity disabled and set to kinematic");
                }
            }
        }
        
        private void EnablePhysics()
        {
            if (rigidBody != null)
            {
                rigidBody.useGravity = wasUsingGravity;
                rigidBody.isKinematic = false;
                
                if (debugMode)
                {
                    Debug.Log("[CockpitManager] Rigidbody gravity restored");
                }
            }
        }
        
        private Transform FindXROrigin()
        {
            if (xrOrigin != null)
            {
                return xrOrigin;
            }

            GameObject xrOriginObject = GameObject.Find("XR Origin")
                                        ?? GameObject.Find("XR Rig")
                                        ?? GameObject.Find("XROrigin");
            
            if (xrOriginObject == null)
            {
                var xrOriginComponent = FindAnyObjectByType<XROrigin>();
                if (xrOriginComponent != null)
                {
                    xrOriginObject = xrOriginComponent.gameObject;
                }
            }
            
            if (xrOriginObject != null)
            {
                xrOrigin = xrOriginObject.transform;
                
                if (debugMode)
                {
                    Debug.Log($"[CockpitManager] Found XR Origin: {xrOrigin.name}");
                }
            }
            else if (vrCamera != null)
            {
                Transform current = vrCamera.transform.parent;
                while (current != null)
                {
                    if (current.name.Contains("XR") || current.name.Contains("Origin") || current.name.Contains("Rig"))
                    {
                        xrOrigin = current;
                        if (debugMode)
                        {
                            Debug.Log($"[CockpitManager] Found XR Origin by traversing: {xrOrigin.name}");
                        }
                        break;
                    }
                    current = current.parent;
                }
            }
            
            return xrOrigin;
        }
        
        public void SetCockpitPosition(Vector3 position)
        {
            isolatedPosition = position;
            SetupCockpitPosition();
            
            if (placeVRCameraInCockpit)
            {
                PositionVRCameraInCockpit();
            }
        }
        
        public void AttachProjectionCameraToRobot(Transform robotHeadTransform)
        {
            Debug.Log("=== [CockpitManager] AttachProjectionCameraToRobot START ===");
            Debug.Log($"  projectionCameraObject: {projectionCameraObject != null}");
            Debug.Log($"  robotHeadTransform: {robotHeadTransform != null}");
            
            if (projectionCameraObject != null && robotHeadTransform != null)
            {
                Debug.Log($"  BEFORE Attach:");
                Debug.Log($"    - Camera GameObject: {projectionCameraObject.gameObject.name}");
                Debug.Log($"    - Camera Active: {projectionCameraObject.gameObject.activeSelf}");
                Debug.Log($"    - Camera ActiveInHierarchy: {projectionCameraObject.gameObject.activeInHierarchy}");
                Debug.Log($"    - Camera Position: {projectionCameraObject.transform.position}");
                Debug.Log($"    - Camera Parent: {(projectionCameraObject.transform.parent ? projectionCameraObject.transform.parent.name : "None")}");
                Debug.Log($"    - Robot Head: {robotHeadTransform.name}");
                Debug.Log($"    - Robot Head Active: {robotHeadTransform.gameObject.activeSelf}");
                Debug.Log($"    - Robot Head ActiveInHierarchy: {robotHeadTransform.gameObject.activeInHierarchy}");
                
                projectionCameraObject.transform.SetParent(robotHeadTransform);
                projectionCameraObject.transform.localPosition = Vector3.zero;
                projectionCameraObject.transform.localRotation = Quaternion.identity;
                
                Debug.Log($"  AFTER Attach:");
                Debug.Log($"    - Camera Active: {projectionCameraObject.gameObject.activeSelf}");
                Debug.Log($"    - Camera ActiveInHierarchy: {projectionCameraObject.gameObject.activeInHierarchy}");
                Debug.Log($"    - Camera World Position: {projectionCameraObject.transform.position}");
                Debug.Log($"    - Camera Parent: {projectionCameraObject.transform.parent.name}");
            }
            else
            {
                Debug.LogError($"[CockpitManager] Cannot attach camera - projectionCameraObject or robotHeadTransform is null!");
            }
            
            Debug.Log("=== [CockpitManager] AttachProjectionCameraToRobot END ===");
        }
        
        public void SetCockpitActive(bool active)
        {
            Debug.Log($"=== [CockpitManager] SetCockpitActive({active}) START ===");
            
            if (cockpitSphereObject != null)
            {
                Debug.Log($"  Setting cockpitSphereObject to {active}");
                cockpitSphereObject.SetActive(active);
            }
            
            if (projectionCameraObject != null)
            {
                Debug.Log($"  BEFORE setting camera active:");
                Debug.Log($"    - Camera Active: {projectionCameraObject.gameObject.activeSelf}");
                Debug.Log($"    - Camera ActiveInHierarchy: {projectionCameraObject.gameObject.activeInHierarchy}");
                Debug.Log($"    - Camera Parent: {(projectionCameraObject.transform.parent ? projectionCameraObject.transform.parent.name : "None")}");
                
                if (projectionCameraObject.transform.parent != null)
                {
                    Debug.Log($"    - Parent Active: {projectionCameraObject.transform.parent.gameObject.activeSelf}");
                    Debug.Log($"    - Parent ActiveInHierarchy: {projectionCameraObject.transform.parent.gameObject.activeInHierarchy}");
                }
                
                projectionCameraObject.gameObject.SetActive(active);
                
                Debug.Log($"  AFTER setting camera to {active}:");
                Debug.Log($"    - Camera Active: {projectionCameraObject.gameObject.activeSelf}");
                Debug.Log($"    - Camera ActiveInHierarchy: {projectionCameraObject.gameObject.activeInHierarchy}");
            }
            
            if (disableGravityInCockpit)
            {
                if (active)
                {
                    DisablePhysics();
                }
                else
                {
                    EnablePhysics();
                }
            }
            
            Debug.Log($"=== [CockpitManager] SetCockpitActive({active}) END ===");
        }
        
        public void ResetVRCameraPosition()
        {
            if (xrOrigin != null)
            {
                if (disableGravityInCockpit)
                {
                    EnablePhysics();
                }
                
                xrOrigin.position = initialXROriginPosition;
                
                if (debugMode)
                {
                    Debug.Log($"[CockpitManager] XR Origin reset to {initialXROriginPosition}");
                }
            }
        }
        
        [ContextMenu("Debug Cockpit Status")]
        private void DebugCockpitStatus()
        {
            Debug.Log("=== Cockpit Manager Status ===");
            Debug.Log($"ProjectionCameraObject: {(projectionCameraObject != null ? projectionCameraObject.gameObject.name : "NULL")}");
            if (projectionCameraObject != null)
            {
                Debug.Log($"  - Active: {projectionCameraObject.gameObject.activeSelf}");
                Debug.Log($"  - ActiveInHierarchy: {projectionCameraObject.gameObject.activeInHierarchy}");
                Debug.Log($"  - Position: {projectionCameraObject.transform.position}");
                Debug.Log($"  - Parent: {(projectionCameraObject.transform.parent ? projectionCameraObject.transform.parent.name : "None")}");
            }
            Debug.Log($"XR Origin: {(xrOrigin != null ? xrOrigin.name : "NULL")}");
            Debug.Log($"XR Origin Position: {xrOrigin?.position}");
            Debug.Log($"Cockpit Position: {isolatedPosition}");
        }
    }
}