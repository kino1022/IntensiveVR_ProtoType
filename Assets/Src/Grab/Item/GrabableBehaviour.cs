using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Grab.Item {

    /// <summary>
    /// つかめるオブジェクトに対して約束するインターフェース
    /// </summary>
    public interface IGrabable {
        
        /// <summary>
        /// 現在つかめるかどうか
        /// </summary>
        bool IsGrabable { get; }
        
        /// <summary>
        /// 現在掴まれているかどうか
        /// </summary>
        bool IsGrabbed { get;  }

        /// <summary>
        /// 掴む際の処理
        /// </summary>
        /// <param name="context">掴んだ際の申し送り</param>
        void Grab(GrabContext context);
        
        /// <summary>
        /// 離す際の処理
        /// </summary>
        void Release();
        
        /// <summary>
        /// つかめるかどうかを設定する
        /// </summary>
        /// <param name="isGrabable"></param>
        void SetGrabable (bool isGrabable);
    }

    public interface IOnGrabCallback {
        void OnGrabCallback (GrabableBehaviour grabable);
    }

    public interface IOnReleaseCallback {
        void OnReleaseCallback (GrabableBehaviour grabable);
    }

    /// <summary>
    /// 掴んだ際に掴まれたオブジェクトに対して送られる情報を詰め込んだ構造体
    /// </summary>
    public readonly struct GrabContext {
        
        public readonly Transform Grabber;
        public readonly GameObject Owner;
        public readonly IObjectResolver Resolver;

        public GrabContext(Transform grabber, GameObject owner, IObjectResolver resolver) {
            Grabber = grabber;
            Owner = owner;
            Resolver = resolver;
        }
    }
    
    /// <summary>
    /// 掴むことのできるオブジェクトに対してアタッチするコンポーネント
    /// </summary>
    public abstract class GrabableBehaviour : SerializedMonoBehaviour, IGrabable　{
        
        [SerializeField]
        [LabelText("現在つかめるかどうか")]
        protected bool _isGrabable = false;
        
        [SerializeField]
        [LabelText("現在掴まれているかどうか")]
        protected bool _isGrabbed = false;
        
        [SerializeField]
        [LabelText("つかみ情報")]
        protected GrabContext _cachedGrabContext;
        
        [Title("Config")]
        [SerializeField]
        [LabelText("掴まれている際の追従速度")]
        private float _locomotionSmoothSpeed = 1.0f;
        
        [SerializeField]
        [LabelText("掴まれている際に再度掴むのを許可するか")]
        private bool _enableReGrab = false;

        [SerializeField]
        [LabelText("追従するか")]
        private bool _enableFollow = true;

        public bool IsGrabable => _isGrabable;
        
        public bool IsGrabbed => _isGrabbed;

        public GrabContext GrabContext => _cachedGrabContext;
        
        //以下キャッシュ領域
        private float _cachedSmoothSpeed = 0.0f;
        private Vector3 _cachedNextPosition = Vector3.zero;
        private Quaternion _cachedNextRotation = Quaternion.identity;
        private List<IOnGrabCallback> _onGrabCallbacks = new List<IOnGrabCallback>(10);
        private List<IOnReleaseCallback> _onReleaseCallbacks = new List<IOnReleaseCallback>(10);

        private void Start() {
            CacheOnGrabCallbacks();
            CacheOnReleaseCallbacks();
        }

        private void Update() {
            UpdateTransformOnGrab();
        }
        
        public void Grab(GrabContext context) {
            OnPreGrab(ref context);
            _isGrabbed = true;
            //再度つかみを許可しないのであればつかみ可能フラグを折る
            if (_enableReGrab is false) {
                SetGrabable(false);
            }
            _cachedGrabContext = context;
            InvokeOnGrabCallback();
            OnPostGrab();
        }

        public void Release() {
            OnPreRelease();
            _isGrabbed = false;
            _isGrabable = true;
            _cachedGrabContext = default;
            InvokeOnReleaseCallback();
            OnPostRelease();
        }

        public void SetGrabable(bool isGrabable) {
            OnPreSetGrabable(ref isGrabable);
            _isGrabable = isGrabable;
            OnPostSetGrabable();
        }
        
        /// <summary>
        /// 掴んでいるTransformをもとに座標を更新する処理
        /// </summary>
        private void UpdateTransformOnGrab() {
            if (_isGrabbed is false || _enableFollow is false) {
                return;
            }
            if (_cachedGrabContext.Grabber is null) {
                return;
            }
            OnPreUpdateTransform();
            CalculateSmoothSpeed(out _cachedSmoothSpeed);
            CalculateNextPosition(out _cachedNextPosition, ref _cachedSmoothSpeed);
            CalculateNextRotation(out _cachedNextRotation, ref _cachedSmoothSpeed);
            transform.position = _cachedNextPosition;
            transform.rotation = _cachedNextRotation;
            OnPostUpdateTransform();
        }
        
        private void CalculateSmoothSpeed(out float smooth) {
            smooth = Mathf.Clamp01(Time.deltaTime * _locomotionSmoothSpeed);
        }

        private void CalculateNextPosition(out Vector3 nextPos, ref float smooth) {
            nextPos = Vector3.Lerp(
                transform.position ,
                _cachedGrabContext.Grabber.position, 
                smooth
                );
        }
        
        private void CalculateNextRotation(out Quaternion nextRot, ref float smooth) {
            nextRot = Quaternion.Slerp(
                transform.rotation,
                _cachedGrabContext.Grabber.rotation,
                smooth
                );
        }
        
        private void CacheOnGrabCallbacks() {
            _onGrabCallbacks.Clear();
            _onGrabCallbacks.AddRange(gameObject.GetComponents<IOnGrabCallback>());
        }

        private void CacheOnReleaseCallbacks() {
            _onReleaseCallbacks.Clear();
            _onReleaseCallbacks.AddRange(gameObject.GetComponents<IOnReleaseCallback>());
        }
        
        private void InvokeOnGrabCallback() {
            if (_onGrabCallbacks.Count > 0) {
                foreach (var callback in _onGrabCallbacks) {
                    if (callback is null) {
                        continue;
                    }
                    callback.OnGrabCallback(this);
                }
            }
        }

        private void InvokeOnReleaseCallback() {
            if (_onReleaseCallbacks.Count > 0) {
                foreach (var callback in _onReleaseCallbacks) {
                    if (callback is null) {
                        continue;
                    }
                    callback.OnReleaseCallback(this);
                }
            }
        }
        
        /// <summary>
        /// 掴まれた際に最初に呼ばれる処理
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnPreGrab(ref GrabContext context) {}
        
        /// <summary>
        /// 掴まれた際に最後に呼ばれる処理
        /// </summary>
        protected virtual void OnPostGrab() {}
        
        /// <summary>
        /// 離された際に最初に呼ばれる処理
        /// </summary>
        protected virtual void OnPreRelease() {}
        
        /// <summary>
        /// 離された際に最後に呼ばれる処理
        /// </summary>
        protected virtual void OnPostRelease() {}
        
        /// <summary>
        /// IsGrabableを設定する前に呼ばれる処理
        /// </summary>
        /// <param name="isGrabable"></param>
        protected virtual void OnPreSetGrabable(ref bool isGrabable) {}
        
        /// <summary>
        /// IsGrabableを設定した後に呼ばれる処理
        /// </summary>
        protected virtual void OnPostSetGrabable() {}
        
        /// <summary>
        /// 座標更新前に呼ばれる処理
        /// </summary>
        protected virtual void OnPreUpdateTransform() {}
        
        /// <summary>
        /// 座標更新後に呼ばれる処理
        /// </summary>
        protected virtual void OnPostUpdateTransform() {}
    }
}