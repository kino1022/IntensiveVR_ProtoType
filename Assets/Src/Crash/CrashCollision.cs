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
        private GameObject _cachedCollidedObject = new();
        
        /// <summary>
        /// 衝突したオブジェクト情報キャッシュ領域
        /// </summary>
        private List<CrashCollisionInfo> _cachedCollisionInfo = new();

        /// <summary>
        /// 衝突オブジェクトのICrashableキャッシュ領域
        /// </summary>
        private ICrashable[] _cachedCrashables = new ICrashable[100];

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
            
            _cachedCollisionInfo.Add(info);

            //IChrashablesのキャッシュ処理
            _cachedCrashables = new ICrashable[100];
            _cachedCrashables = _cachedCollidedObject.GetComponents<ICrashable>();
            
            //キャッシュしたICrashableの処理呼び出し
            if (_cachedCrashables.Length > 0) {
                foreach (var crashable in _cachedCrashables) {
                    crashable.OnCrash(ref context);
                }
            }
        }
        
    }
}