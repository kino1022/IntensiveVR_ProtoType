
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class VRHandRig : MonoBehaviour
{
    [System.Serializable]
    public class HandRigSetup
    {
        public Transform controller;
        public TwoBoneIKConstraint ikConstraint;
        public Vector3 positionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero;
        [HideInInspector] public Transform mirror;
    }
    
    [Header("Hand Settings")]
    [SerializeField] private HandRigSetup leftHand;
    [SerializeField] private HandRigSetup rightHand;
    
    [Header("Space Settings")]
    [SerializeField] private Transform characterRoot;
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private RigBuilder rigBuilder;
    
    [Header("Initialization")]
    [SerializeField] private int waitFrames = 2; // XROrigin移動を待つフレーム数
    
    [Header("Tracking Mode")]
    [SerializeField] private TrackingMode trackingMode = TrackingMode.Direct;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private bool isInitialized = false;
    
    public enum TrackingMode
    {
        Direct,      // コントローラーの位置を直接使用
        Relative,    // XROriginからの相対位置
    }
    
    void Start()
    {
        StartCoroutine(InitializeAfterDelay());
    }

    private void RigBuild() {
        if (rigBuilder != null) {
            rigBuilder.Build();
        }
    }
    
    private IEnumerator InitializeAfterDelay()
    {
        for (int i = 0; i < waitFrames; i++)
        {
            yield return null;
        }

        leftHand.mirror = CreateMirror("LeftHandMirror");
        rightHand.mirror = CreateMirror("RightHandMirror");

        // ★修正: data全体を取得→変更→再設定
        var leftData = leftHand.ikConstraint.data;
        leftData.target = leftHand.mirror;
        leftHand.ikConstraint.data = leftData;

        var rightData = rightHand.ikConstraint.data;
        rightData.target = rightHand.mirror;
        rightHand.ikConstraint.data = rightData;

        isInitialized = true;
        
        RigBuild();
        
        if (showDebugInfo)
        {
            Debug.Log($"=== VR Hand Rig Initialized ===");
            Debug.Log($"XR Origin: {xrOrigin?.position}");
            Debug.Log($"Character Root: {characterRoot.position}");
            Debug.Log($"Left Controller: {leftHand.controller.position}");
            Debug.Log($"Tracking Mode: {trackingMode}");
        }
    }
    
    private Transform CreateMirror(string name)
    {
        GameObject mirror = new GameObject(name);
        Transform t = mirror.transform;
        t.SetParent(characterRoot);
        return t;
    }
    
    void LateUpdate()
    {
        if (!isInitialized) return;
        
        UpdateHand(leftHand);
        UpdateHand(rightHand);
    }
    
    private void UpdateHand(HandRigSetup hand)
    {
        if (hand.controller == null || hand.mirror == null) return;
        
        Vector3 targetPos;
        Quaternion targetRot;
        
        switch (trackingMode)
        {
            case TrackingMode.Direct:
                // コントローラーの位置を直接使用
                targetPos = hand.controller.position;
                targetRot = hand.controller.rotation;
                break;
                
            case TrackingMode.Relative:
                // XROriginからの相対位置
                Vector3 relativePos = hand.controller.position - xrOrigin.position;
                Quaternion relativeRot = Quaternion.Inverse(xrOrigin.rotation) * hand.controller.rotation;
                
                targetPos = characterRoot.position + relativePos;
                targetRot = characterRoot.rotation * relativeRot;
                break;
                
            default:
                targetPos = hand.controller.position;
                targetRot = hand.controller.rotation;
                break;
        }
        
        // オフセット適用
        if (hand.positionOffset != Vector3.zero)
        {
            targetPos += hand.controller.TransformDirection(hand.positionOffset);
        }
        
        if (hand.rotationOffset != Vector3.zero)
        {
            targetRot *= Quaternion.Euler(hand.rotationOffset);
        }
        
        // ミラーに反映
        hand.mirror.position = targetPos;
        hand.mirror.rotation = targetRot;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugInfo || !isInitialized) return;
        
        DrawHandGizmo(leftHand, Color.cyan);
        DrawHandGizmo(rightHand, Color.magenta);
    }
    
    private void DrawHandGizmo(HandRigSetup hand, Color color)
    {
        if (hand.controller == null || hand.mirror == null) return;
        
        // コントローラー
        Gizmos.color = color;
        Gizmos.DrawWireSphere(hand.controller.position, 0.05f);
        
        // ミラー
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(hand.mirror.position, 0.05f);
        
        // 接続線
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(hand.controller.position, hand.mirror.position);
    }
    
    // ランタイムで再初期化
    [ContextMenu("Reinitialize")]
    public void Reinitialize()
    {
        isInitialized = false;
        StartCoroutine(InitializeAfterDelay());
    }
}
