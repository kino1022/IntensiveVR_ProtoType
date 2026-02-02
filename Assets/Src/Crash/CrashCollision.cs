using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private List<ICrashable> _tempCrashables = new List<ICrashable>(50);
        
        private CrashCollisionContext _cachedCollisionContext;

        public void OnCollisionEnter(Collision other) {
            Debug.Log($"[CrashCollision] OnCollisionEnter: {other.gameObject.name}");
            
            _cachedCollidedObject = other.gameObject;

            //衝突コンテキストの生成処理
            _cachedCollisionContext = new CrashCollisionContext(
                hitDirection: other.relativeVelocity.normalized,
                hitForce: other.relativeVelocity.magnitude,
                hitPosition: other.GetContact(0).point
            );

            //衝突情報の生成処理
            _cachedCollisionInfo = new CrashCollisionInfo(
                collisionTime: TimeSpan.FromSeconds(Time.time),
                collidedObject: _cachedCollidedObject,
                context: _cachedCollisionContext
            );

            if (_cachedCollidedObject is null) {
                return;
            }
            _tempCrashables = _cachedCollidedObject.GetComponentsInChildren<ICrashable>().ToList();
            if (_tempCrashables.Count == 0) {
                return;
            }
            //IChrashablesのキャッシュ処理
            _cachedCrashables.AddRange(_tempCrashables);
            
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