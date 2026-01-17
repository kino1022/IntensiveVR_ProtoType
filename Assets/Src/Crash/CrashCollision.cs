using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

        /// <summary>
        /// 衝突したオブジェクトキャッシュ領域
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private GameObject _cachedCollidedObject = new();

        /// <summary>
        /// 衝突したオブジェクト情報キャッシュ領域
        /// </summary>
        [SerializeField]
        [TableList]
        [ReadOnly]
        private CrashCollisionInfo[] _cachedCollisionInfo = new CrashCollisionInfo[50];

        /// <summary>
        /// 衝突オブジェクトのICrashableキャッシュ領域
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private List<ICrashable> _cachedCrashables = new List<ICrashable>(50);

        public void OnCollisionEnter(Collision collision) {
            
            _cachedCollidedObject = collision.gameObject;

            //衝突コンテキストの生成処理
            var context = new CrashCollisionContext(
                hitDirection: collision.relativeVelocity.normalized,
                hitForce: collision.relativeVelocity.magnitude,
                hitPosition: collision.GetContact(0).point
            );

            //衝突情報の生成処理
            var info = new CrashCollisionInfo(
                collisionTime: TimeSpan.FromSeconds(Time.time),
                collidedObject: _cachedCollidedObject,
                context: context
            );

            //IChrashablesのキャッシュ処理
            _cachedCrashables.AddRange(_cachedCollidedObject.GetComponents<ICrashable>());
            
            //キャッシュしたICrashableの処理呼び出し
            if (_cachedCrashables.Count > 0) {
                foreach (var crashable in _cachedCrashables) {
                    crashable.OnCrash(ref context);
                }
            }
            
            _cachedCrashables.Clear();
        }
    }
}