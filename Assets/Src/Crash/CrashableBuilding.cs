using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Crash {
    /// <summary>
    /// 破壊可能な建物に対してアタッチするコンポーネント
    /// </summary>
    public class CrashableBuilding : SerializedMonoBehaviour, ICrashable {
        
        [SerializeField]
        private float _exploseRadius = 20.0f;
        
        [SerializeField]
        [LabelText("置換するPrefab")]
        private GameObject _replacePrefab;
        
        [SerializeField]
        [LabelText("生成したオブジェクトキャッシュ")]
        [ReadOnly]
        private GameObject _cachedGeneratedObject;

        /// <summary>
        /// 生成したprefabから取得したICrashableキャッシュ領域
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private List<ICrashable> _cachedCrashables = new List<ICrashable>(100);
        
        [SerializeField]
        [ReadOnly]
        private List<ICrashableBuildingPiece> _cachedBuildingPieces = new List<ICrashableBuildingPiece>(100);
        
        public void OnCrash(ref CrashCollisionContext context) {
            
            if (_replacePrefab is not null) {
                
                //破壊後Prefabの生成とキャッシュ処理
                _cachedGeneratedObject = Instantiate(
                    _replacePrefab,
                    transform.position,
                    transform.rotation
                );
                if (_cachedGeneratedObject is not null) {
                    //子のオブジェクトのICrashableの処理
                    _cachedCrashables.AddRange(_cachedGeneratedObject
                        .GetComponentsInChildren<ICrashable>());
                    if (_cachedCrashables.Count > 0) {
                        foreach (var crashable in _cachedCrashables) {
                            if (crashable is null) {
                                continue;
                            }
                            crashable.OnCrash(ref context);
                        }
                    }
                    _cachedCrashables.Clear();

                    //BuildingPieceの飛散処理
                    _cachedBuildingPieces.AddRange(_cachedGeneratedObject
                        .GetComponentsInChildren<ICrashableBuildingPiece>());
                    if (_cachedBuildingPieces.Count > 0) {
                        foreach (var piece in _cachedBuildingPieces) {
                            if (piece is null || piece.rigid is null) {
                                continue;
                            }
                            //BuildingPieceの飛散力付与
                            piece.rigid.AddExplosionForce(
                                context.HitForce,
                                context.HitPosition,
                                _exploseRadius
                                );
                        }
                    }
                    _cachedBuildingPieces.Clear();

                    Destroy(gameObject);
                }
            }
        }
    }
}