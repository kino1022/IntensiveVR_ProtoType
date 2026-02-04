using UnityEngine;

namespace Cam {
    
    public interface ICameraIdentify {
        Camera IdentifiedCamera { get; }
    }
    
    /// <summary>
    /// カメラごとに識別するためのカメラのラッパークラス
    /// </summary>
    public class CameraIdentify : ICameraIdentify {
        private Camera _cam;
        
        public Camera IdentifiedCamera => _cam;

        public CameraIdentify(Camera cam) {
            _cam = cam;
        }
    }
    
    public interface IXROriginIdentifiedCamera : ICameraIdentify {}

    public class XROriginIdentifiedCamera : CameraIdentify, IXROriginIdentifiedCamera {
        public XROriginIdentifiedCamera(Camera cam) : base(cam) {}
    }
    
    public interface IMainIdentifiedCamera : ICameraIdentify {}

    public class MainIdentifiedCamera : CameraIdentify, IMainIdentifiedCamera {
        public MainIdentifiedCamera(Camera cam) : base (cam) {}
    }
}