using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Crash {

    /// <summary>
    /// 衝突したオブジェクトの情報
    /// </summary>
    public struct CrashCollisionInfo {
        
        /// <summary>
        /// 衝突した時間
        /// </summary>
        public TimeSpan CollisionTime { get; }
        
        /// <summary>
        /// 衝突したオブジェクト
        /// </summary>
        public GameObject CollidedObject { get; }
        
        /// <summary>
        /// 判定時に生成されたコンテキスト
        /// </summary>
        public CrashCollisionContext Context { get; }
        
        public CrashCollisionInfo(TimeSpan collisionTime, GameObject collidedObject, CrashCollisionContext context) {
            CollisionTime = collisionTime;
            CollidedObject = collidedObject;
            Context = context;
        }
        
    }
    
    public class CrashCollision : SerializedMonoBehaviour {
        
        [Title("Runtime Status")]

        [SerializeField]
        [LabelText("衝突したオブジェクト")]
        [ReadOnly]
        private GameObject _cachedCollidedObject;

        private CrashCollisionInfo _cachedCollisionInfo; 

        [OdinSerialize]
        [LabelText("衝突処理オブジェクトキャッシュ")]
        [ReadOnly]
        private List<ICrashable> _cachedCrashables = new List<ICrashable>(50);
        
        private CrashCollisionContext _cachedCollisionContext;

        public void OnCollisionEnter(Collision collision) {
            
            _cachedCollidedObject = collision.gameObject;

            //衝突コンテキストの生成処理
            _cachedCollisionContext = new CrashCollisionContext(
                hitDirection: collision.relativeVelocity.normalized,
                hitForce: collision.relativeVelocity.magnitude,
                hitPosition: collision.GetContact(0).point
            );

            //衝突情報の生成処理
            _cachedCollisionInfo = new CrashCollisionInfo(
                collisionTime: TimeSpan.FromSeconds(Time.time),
                collidedObject: _cachedCollidedObject,
                context: _cachedCollisionContext
            );


            //IChrashablesのキャッシュ処理
            _cachedCrashables.AddRange(_cachedCollidedObject.GetComponents<ICrashable>());
            
            //キャッシュしたICrashableの処理呼び出し
            if (_cachedCrashables.Count > 0) {
                foreach (var crashable in _cachedCrashables) {
                    crashable.OnCrash(ref _cachedCollisionContext);
                }
            }
            
            _cachedCrashables.Clear();
        }
    }
}