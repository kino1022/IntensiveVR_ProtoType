using System;
using Grab.Item;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Grab {
    public class GrabHand : SerializedMonoBehaviour {

        [Title("Config")]

        [SerializeField]
        [LabelText("掴む入力アクション")]
        private InputActionReference _input;
        
        
        [Title("RuntimeStatus")]

        [SerializeField]
        [LabelText("掴んでいるアイテム")]
        [ReadOnly]
        private GrabableBehaviour _grabItem;
        
        [SerializeField]
        [LabelText("購読済みフラグ")]
        [ReadOnly]
        private bool _isSubscribed = false;
        
        [SerializeField]
        [LabelText("前フレームの座標")]
        [ReadOnly]
        private Vector3 _previousPosition = Vector3.zero;
        
        [SerializeField]
        [LabelText("現在の速度")]
        [ReadOnly]
        private Vector3 _currentVelocity = Vector3.zero;
        
        private GrabContext _grabContext = default;
        
        public GrabableBehaviour GrabItem => _grabItem;
        
        [Title("Dependencies")]
        
        [OdinSerialize]
        [LabelText("ターゲットプロバイダー")]
        private IGrabTargetProvider _targetProvider;

        private GrabTargetContext _cachedGrabContext = default;
        
        private ReleaseContext _cachedReleaseContext = default;

        private IObjectResolver _resolver;
        
        [Inject]
        public void Construct(IObjectResolver resolver) {
            _resolver = resolver ?? throw new ArgumentNullException();
        }

        private void Start() {
            if (!_isSubscribed) {
                RegisterInput();
            }
        }

        private void Update() {
            if (!_isSubscribed) {
                RegisterInput();
            }
            GetPositionCache(out _previousPosition);
            UpdateVelocityCache(out _currentVelocity);
        }

        private void FixedUpdate() {
            if (_grabItem is null) {
                _targetProvider.TryGetGrabTarget(out _cachedGrabContext);
            }
        }

        /// <summary>
        /// 実際に対象を握る際に呼ばれる処理
        /// </summary>
        private void Grab(GrabableBehaviour target) {
            if (target is null) {
                return;
            }
            CreateGrabContext(out _grabContext);
            target.Grab(_grabContext);
            _grabItem = target;
        }

        /// <summary>
        /// 実際に対象を話す際に呼ばれる処理
        /// </summary>
        /// <param name="target"></param>
        private void Release(GrabableBehaviour target) {
            if (target is null) {
                return;
            }
            CreateReleaseContext(out _cachedReleaseContext);
            target.Release(_cachedReleaseContext);
            _grabItem = null;
        }
        
        private void RegisterInput() {
            if (_input is null) {
                return;
            }

            _input
                .action
                .started += context => {
                    OnGrabInput(ref context);
                };

            _input
                .action
                .canceled += context => {
                    OnReleaseInput(ref context);
                };
            
            _isSubscribed = true;
        }

        private void GetPositionCache(out Vector3 pos) {
            pos = transform.position;
        }
        
        private void UpdateVelocityCache(out Vector3 velocity) {
            velocity = (transform.position - _previousPosition) / Time.fixedDeltaTime;
        }

        private void OnGrabInput(ref InputAction.CallbackContext context) {
            if (_cachedGrabContext.ExsistTarget is false) {
                return;
            }
            if (_cachedGrabContext.Target is null) {
                return;
            }
            if (_cachedGrabContext.Target.IsGrabable && _resolver is not null) {
                Grab(_cachedGrabContext.Target);
            }
        }

        private void OnReleaseInput(ref InputAction.CallbackContext context) {
            if (_grabItem is null) {
                return;
            }
            Release(_grabItem);
        }
        
        private void CreateGrabContext (out GrabContext context) {
            if (_resolver is null) {
                context = default;
                return;
            }
            context = new GrabContext(
                transform,
                gameObject,
                _resolver
            );
        }
        
        private void CreateReleaseContext (out ReleaseContext context) {
            context = new ReleaseContext(_currentVelocity);
        }
    }
}