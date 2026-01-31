using System;
using Controller.Hand;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Player {
    /// <summary>
    /// IHantTrackingModuleから取得したコントローラーの位置をもとにIKを制御するクラス
    /// </summary>
    public class HandIkController : SerializedMonoBehaviour {
        
        [Title("References")]
        
        [SerializeField]
        [LabelText("右手IKターゲット")]
        private Transform _rightHandIkTarget;
        
        [SerializeField]
        [LabelText("左手IKターゲット")]
        private Transform _leftHandIkTarget;
        
        [SerializeField]
        [LabelText("基準Transform")]
        private Transform _baseTransform;
        
        [Title("Config")]
        
        [SerializeField]
        [LabelText("IKの同期速度")]
        [Range(0.1f, 10.0f)]
        private float m_ikSmoothSpeed = 1.0f;

        [FormerlySerializedAs("m_bilateralRatio")] [SerializeField] [LabelText("バイラテラル補正")] [Range(0.1f, 10.0f)]
        private float m_positionBilateralRatio = 1.0f;
        
        [SerializeField]
        [LabelText("回転角バイラテラル補正")]
        [Range(0.1f, 10.0f)]
        private float m_rotationBilateralRatio = 1.0f;

        [TitleGroup("Injected Modules")]
        [OdinSerialize]
        [ReadOnly]
        private IRightHandTrackingModule _rightTracking;
        
        [OdinSerialize]
        [ReadOnly]
        private ILeftHandTrackingModule _leftTracking;

        private IObjectResolver _resolver;
        
        //以下キャッシュ領域
        private float _cachedSmoothSpeed = 0.0f;
        private Vector3 _cachedNextPosition = Vector3.zero;
        private Quaternion _cachedNextRotation = Quaternion.identity;
        private Quaternion _cachedAngledRotation = Quaternion.identity;

        [Inject]
        public void Construct(IObjectResolver resolver) {
            _resolver = resolver;
        }

        private void Start() {
            InjectionTrackingModules();
        }

        private void Update() {
            // トラッキングモジュールの注入確認
            if (_rightTracking is null || _leftTracking is null) {
                InjectionTrackingModules();
            }
        }

        private void LateUpdate() {
            UpdateIK();
        }

        /// <summary>
        /// トラッキングモジュールの注入処理
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void InjectionTrackingModules() {
            if (_resolver == null) {
                throw new ArgumentNullException($"{GetType().Name}: IObjectResolver is null");
            }
            _rightTracking ??= _resolver.Resolve<IRightHandTrackingModule>();
            _leftTracking ??= _resolver.Resolve<ILeftHandTrackingModule>();
        }

        
        private void UpdateIK()
        {
            if (_baseTransform is null)
            {
                return;
            }
            _cachedSmoothSpeed = Mathf.Clamp01(m_ikSmoothSpeed * Time.deltaTime);
            UpdateIkPosition(_rightTracking, _rightHandIkTarget, in _cachedSmoothSpeed);
            UpdateIkRotation(_rightTracking, _rightHandIkTarget, in _cachedSmoothSpeed);
            UpdateIkPosition(_leftTracking, _leftHandIkTarget, in _cachedSmoothSpeed);
            UpdateIkRotation(_leftTracking, _leftHandIkTarget, in _cachedSmoothSpeed);
        }

        /// <summary>
        /// IK対象の位置を更新する処理
        /// </summary>
        /// <param name="tacking"></param>
        /// <param name="target"></param>
        private void UpdateIkPosition(IHandTrackingModule tracking, Transform target, in float smooth) {
            if (tracking is null) {
                return;
            }

            if (target is null) {
                return;
            }

            _cachedNextPosition = _baseTransform.TransformPoint(tracking.CurrentPosition);
            target.position = Vector3.Lerp(
                target.position * m_positionBilateralRatio,
                _cachedNextPosition,
                smooth
            );
        }

        /// <summary>
        /// IK対象の回転を更新する処理
        /// </summary>
        /// <param name="tacking"></param>
        /// <param name="target"></param>
        private void UpdateIkRotation(IHandTrackingModule tracking, Transform target, in float smooth) {
            if (tracking is null) {
                return;
            }

            if (target is null) {
                return;
            }
            //シンバルロック回避のための分解処理
            tracking.CurrentRotation.ToAngleAxis(out var angle, out var axis);
            //倍率を掛けて回転を適用
            _cachedAngledRotation = Quaternion.AngleAxis(angle * m_rotationBilateralRatio, axis);
            _cachedNextRotation *= _baseTransform.rotation;
            target.rotation = Quaternion.Slerp(
                target.rotation,
                _cachedNextRotation,
                smooth
            );
        }
    }
}