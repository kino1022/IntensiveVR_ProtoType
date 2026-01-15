using UnityEngine;

namespace Crash {
    /// <summary>
    /// 破壊可能なオブジェクトに破壊判定が衝突した際に生成される情報を格納するための構造体
    /// </summary>
    public readonly struct CrashCollisionContext {
        
        /// <summary>
        /// 衝突したオブジェクトの力の方向
        /// </summary>
        public readonly Vector3 hitDirection { get; }
        
        /// <summary>
        /// 衝突したオブジェクトの力の大きさ
        /// </summary>
        public readonly float hitForce { get; }
        
        /// <summary>
        /// 衝突した座標
        /// </summary>
        public readonly Vector3 hitPosition { get; }
        
        public CrashCollisionContext(Vector3 hitDirection, float hitForce, Vector3 hitPosition) {
            this.hitDirection = hitDirection;
            this.hitForce = hitForce;
            this.hitPosition = hitPosition;
        }
    }
}