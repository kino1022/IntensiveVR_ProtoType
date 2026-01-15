using Sirenix.OdinInspector;
using UnityEngine;

namespace Crash {
    /// <summary>
    /// 破壊可能な建物に対してアタッチするコンポーネント
    /// </summary>
    public class CrashableBuilding : SerializedMonoBehaviour, ICrashable {
        
        [SerializeField]
        [LabelText("置換するPrefab")]
        private GameObject _replacePrefab;

        /// <summary>
        /// 生成したprefabから取得したICrashableキャッシュ領域
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private ICrashable[] _cachedCrashables = new ICrashable[100];
        
        public void OnCrash(ref CrashCollisionContext context) {
            if (_replacePrefab is not null) {
                var obj = Instantiate(
                    _replacePrefab,
                    transform.position,
                    transform.rotation
                );
                _cachedCrashables = new ICrashable[100];
                _cachedCrashables = obj.GetComponentsInChildren<ICrashable>();
                if (_cachedCrashables.Length > 0) {
                    foreach (var crashable in _cachedCrashables) {
                        crashable.OnCrash(ref context);
                    }
                }
            }
        }
    }
}