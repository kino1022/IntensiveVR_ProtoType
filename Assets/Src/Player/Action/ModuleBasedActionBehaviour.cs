using R3;
using RinaInput.Controller.Module;
using RinaInput.Signal;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player.Action {
    public abstract class ModuleBasedActionBehaviour<T> : ActionBehaviour<T> where T : struct {

        protected IInputModule<T> InputModule;
        
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
            if (InputModule == null) {
                return;
            }

            InputModule
                .Stream
                .Subscribe(x => {
                    if (x.Phase is InputActionPhase.Started) {
                        _isInputting = true;
                        CreateContext(ref x, out CachedContext);
                        StartAction(ref CachedContext);
                    }
                    else if (x.Phase is InputActionPhase.Canceled) {
                        _isInputting = false;
                        CreateContext(ref x, out CachedContext);
                        CancelAction(ref CachedContext);
                    }
                })
                .AddTo(this);
            _isSubscribed = true;
        }

        private void CreateContext(ref InputSignal<T> signal, out ActionContext<T> context) {
            context = new ActionContext<T>(signal.Value);
        }
        
        protected virtual void OnStart () {}
        
        protected virtual void OnTick () {}
    }
}