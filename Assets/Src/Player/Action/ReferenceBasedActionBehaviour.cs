using RinaInput.Controller.Module;
using RinaInput.Signal;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.Action {
    public abstract class ReferenceBasedActionBehaviour<T> : ActionBehaviour<T> where T : struct {
        
        [OdinSerialize]
        [LabelText("入力モジュール")]
        protected InputActionReference Input;
        
        [FormerlySerializedAs("PreviousDuration")]
        [Title("Runtime Status")]
        
        [SerializeField]
        [LabelText("購読済みフラグ")]
        [ReadOnly]
        protected bool _isSubscribed = false;

        [SerializeField]
        [LabelText("入力中フラグ")]
        [ReadOnly]
        protected bool _isInputting = false;
        
        protected ActionContext<T> CachedContext;

        private void Start() {
            if (!_isSubscribed) {
                RegisterInputModule();
            }
            OnStart();
        }

        private void Update() {
            if (!_isSubscribed) {
                RegisterInputModule();
            }
            if (_isInputting) {
                PerformAction(ref CachedContext);
            }
            OnTick();
        }

        private void RegisterInputModule() {
            if (Input == null) {
                return;
            }

            Input
                .action
                .started += ctx => {
                    CreateContext(ref ctx, out CachedContext);
                    OnStart();
                };

            Input
                .action
                .performed += ctx => {
                    CreateContext(ref ctx, out CachedContext);
                    OnTick();
                };

            Input
                .action
                .canceled += ctx => {
                    CreateContext(ref ctx, out CachedContext);
                    OnStart();
                };
            
            _isSubscribed = true;
        }

        private void CreateContext(ref InputAction.CallbackContext con, out ActionContext<T> context) {
            context = new ActionContext<T>(con.ReadValue<T>());
        }
        
        protected virtual void OnStart () {}
        
        protected virtual void OnTick () {}
    }
}