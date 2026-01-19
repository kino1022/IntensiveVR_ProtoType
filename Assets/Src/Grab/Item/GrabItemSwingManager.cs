using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grab.Item {

    public interface IGrabSwingManager : IGrabSwingSpeedProvider, IGrabSwingVelocityProvider {
        /// <summary>
        /// 現在スイング可能かどうか
        /// </summary>
        bool Swingable { get; }
        
        /// <summary>
        /// スイング可能かどうかを設定する
        /// </summary>
        /// <param name="swingable"></param>
        void SetSwingable (bool swingable);
    }
    
    public interface IGrabSwingSpeedProvider : IGrabOnSwingProvider {
        ReadOnlyReactiveProperty<float> SwingSpeed { get; }
    }
    
    public interface IGrabSwingVelocityProvider : IGrabOnSwingProvider {
        ReadOnlyReactiveProperty<Vector3> SwingVelocity { get; }
    }

    public interface IGrabOnSwingProvider {
        ReadOnlyReactiveProperty<bool> OnSwing { get; }
    }
    
    public class GrabItemSwingManager : SerializedMonoBehaviour, IGrabSwingManager {
        
        [Title("Reference")]
        
        [SerializeField]
        [LabelText("基準Transform")]
        private Transform _baseTransform;
        
        private ReactiveProperty<float> _swingSpeed = new ReactiveProperty<float>(0f);
        
        private ReactiveProperty<Vector3> _swingVelocity = new ReactiveProperty<Vector3>(Vector3.zero);
        
        private ReactiveProperty<bool> _onSwing = new ReactiveProperty<bool>(false);
        
        private bool _swingable = true;
        
        public ReadOnlyReactiveProperty<float> SwingSpeed => _swingSpeed;
        
        public ReadOnlyReactiveProperty<Vector3> SwingVelocity => _swingVelocity;

        public ReadOnlyReactiveProperty<bool> OnSwing => _onSwing;

        public bool Swingable => _swingable;
        
        private Vector3 _cachedPreviousPosition = Vector3.zero;

        private void Update() {
            
        }

        public void SetSwingable(bool swingable) {
            _swingable = swingable;
        }
    }
}