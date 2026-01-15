using Sirenix.OdinInspector;
using UnityEngine;

namespace Crash {
    /// <summary>
    /// 破壊可能な建物の破片に対してアタッチするコンポーネント
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CrashableBuildingPiece : SerializedMonoBehaviour, ICrashable {

        [SerializeField]
        [LabelText("方向オフセット")]
        private Vector3 _directionOffset = Vector3.zero;
        
        [SerializeField]
        [LabelText("力のオフセット")]
        private float _forceOffset = 0f;
        
        public void OnCrash(ref CrashCollisionContext context) {
            
        }
    }
}