using System;
using UnityEngine;

namespace Crash {
    /// <summary>
    /// 破壊可能なオブジェクトに破壊判定が衝突した際に生成される情報を格納するための構造体
    /// </summary>
    [Serializable]
    public readonly struct CrashCollisionContext {

        /// <summary>
        /// 衝突したオブジェクトの力の方向
        /// </summary>
        public readonly Vector3 HitDirection;

        /// <summary>
        /// 衝突したオブジェクトの力の大きさ
        /// </summary>
        public readonly float HitForce;

        /// <summary>
        /// 衝突した座標
        /// </summary>
        public readonly Vector3 HitPosition;
        
        public CrashCollisionContext(Vector3 hitDirection, float hitForce, Vector3 hitPosition) {
            this.HitDirection = hitDirection;
            this.HitForce = hitForce;
            this.HitPosition = hitPosition;
        }
    }
}