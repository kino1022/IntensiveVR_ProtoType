using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.Action {
    public class BeamActio : ReferenceBasedActionBehaviour<float> {

        [Title("Config")]
        [SerializeField]
        [LabelText("Biimの始点")]
        private Transform _beamOrigin;
        
        [SerializeField]
        [LabelText("Beamの長さ")]
        [Range(0, 100)]
        private float _beamLength = 10.0f;
        
        [SerializeField]
        [LabelText("Beamの太さ")]
        [Range(0.1f, 100.0f)]
        private float _beamThickness = 1.0f;

        [SerializeField]
        private LineRenderer _renderer;

        public override void StartAction(ref ActionContext<float> context) {
            
        }
        
        public override void PerformAction(ref ActionContext<float> context) {
            if (_renderer == null) {
                return;
            }
            _renderer.SetPosition(1, _beamOrigin.position + _beamOrigin.forward * _beamLength);
        }
        
        public override void CancelAction(ref ActionContext<float> context) {
            
        }
    }
}