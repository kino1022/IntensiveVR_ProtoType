using R3;
using Sirenix.OdinInspector;

namespace Status {

    public interface IStatus {
        
        /// <summary>
        /// ステータスの現在の値
        /// </summary>
        ReadOnlyReactiveProperty<float> Current { get; }
        
        /// <summary>
        /// 初期化されたかどうか
        /// </summary>
        bool Initialized { get; }
        
        /// <summary>
        /// ステータスの初期化を行うメソッド
        /// </summary>
        /// <param name="context">初期化に利用するデータ</param>
        void Initialize (in StatusInitializeContext context);

        /// <summary>
        /// ステータスに値をセットする
        /// </summary>
        /// <param name="value"></param>
        void SetCurrent(float value);

        /// <summary>
        /// ステータスの値を増加させる
        /// </summary>
        /// <param name="value"></param>
        void Increase(float value);
        
        /// <summary>
        /// ステータスの値を減少させる
        /// </summary>
        /// <param name="value"></param>
        void Decrease(float value);
        
    }

    public readonly struct StatusInitializeContext {
        public readonly float Value;

        public StatusInitializeContext(float value) {
            Value = value;
        }
    }
    
    public abstract class StatusBehaviour : SerializedMonoBehaviour, IStatus {
        
        private ReactiveProperty<float> _current = new ReactiveProperty<float>(0f);

        private bool _initialized = false;
        
        public ReadOnlyReactiveProperty<float> Current => _current;
        
        public bool Initialized => _initialized;
        
        public void Initialize (in StatusInitializeContext context) {
            if (_initialized) {
                return;
            }
            OnPreInitialize(in context);
            _initialized = true;
            SetCurrent(context.Value);
            OnPostInitialize(in context);
        }
        
        public void SetCurrent(float next) {
            OnPreChangeCurrent(next);
            _current.Value = next;
            OnPostChangeCurrent();
        }
        
        public void Increase(float value) {
            SetCurrent(_current.Value + value);
        }

        public void Decrease(float value) {
            SetCurrent(_current.Value - value);
        }
        
        protected virtual void OnPreInitialize(in StatusInitializeContext context) { }
        
        protected virtual void OnPostInitialize(in StatusInitializeContext context) { }
        
        protected virtual void OnPreChangeCurrent(float next) { }
        
        protected virtual void OnPostChangeCurrent() { }
        
    }
}