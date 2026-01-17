using Sirenix.OdinInspector;
using UnityEngine;

namespace Crash {

    public interface ICrashableBuildingPiece {
        
        Rigidbody rigid { get; }
    }
    
    /// <summary>
    /// 破壊可能な建物の破片に対してアタッチするコンポーネント
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CrashableBuildingPiece : SerializedMonoBehaviour, ICrashableBuildingPiece {
        
        public Rigidbody rigid => GetComponent(typeof(Rigidbody)) as Rigidbody;
        
    }
}