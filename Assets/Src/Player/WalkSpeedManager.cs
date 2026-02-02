using Sirenix.OdinInspector;
using UnityEngine;

namespace Player {
    
    public interface IWalkSpeedManager : IWalkSpeedProvider{
        
        void SetSpeed (float value);
    }

    public interface IWalkSpeedProvider {
        float GetWalkSpeed();
    }

    
    public class WalkSpeedManager : SerializedMonoBehaviour, IWalkSpeedManager {
        
        [Title("Config")]
        
        [SerializeField, LabelText("初期歩行速度"), Range(0.0f, 100.0f)]
        private float _initialSpeed = 10.0f;

        [Title("RuntimeStatus")]
        
        [SerializeField] [LabelText("現在の歩行速度")] [ReadOnly]
        private float _currentSpeed = 0.0f;
        
        private void Awake() {
            SetSpeed(_initialSpeed);
        }
        public void SetSpeed (float value) {
            _currentSpeed = value;
        }
        public float GetWalkSpeed() {
            return _currentSpeed;
        }
    }
}