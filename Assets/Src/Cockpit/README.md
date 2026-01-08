# VRコックピットカメラ投影システム

## 概要

このシステムは、UnityのVR向け実装でロボットを操縦するゲームにおいて、カメラ映像を隔離した座標に配置したコックピットを模した球体オブジェクトの内側に投影する機能を提供します。

## アーキテクチャ

このシステムは以下のコンポーネントで構成されています：

### 1. CockpitCameraProjection
- **役割**: カメラからの映像をRenderTextureに描画し、コックピット球体に投影
- **機能**:
  - RenderTextureの自動生成と管理
  - カメラ設定の調整（FOV、クリッピング距離など）
  - 投影テクスチャの解像度調整

### 2. InvertedSphereMesh
- **役割**: 内側から見える球体メッシュを生成
- **機能**:
  - プロシージャルに反転球体メッシュを生成
  - 法線を内側に向けることで、球体の内側から表面が見えるようにする
  - UVマッピングの適切な設定

### 3. CockpitManager
- **役割**: コックピットシステム全体の統合管理
- **機能**:
  - コックピットの隔離された座標への配置
  - VRカメラのコックピット内への配置
  - 投影カメラのロボットへのアタッチ
  - システムの有効/無効化

## セットアップ方法

### 1. 基本的なセットアップ

#### ステップ1: コックピット球体の作成
1. Hierarchyで右クリック → `Create Empty`で空のGameObjectを作成し、"CockpitSphere"と名付ける
2. `InvertedSphereMesh`コンポーネントを追加
3. `MeshRenderer`コンポーネントを追加（自動的に追加されていない場合）

#### ステップ2: 投影カメラの作成
1. Hierarchyで右クリック → `Camera`を作成し、"ProjectionCamera"と名付ける
2. このカメラをロボットの頭部（視点）に配置するか、親として設定
3. `CockpitCameraProjection`コンポーネントを追加

#### ステップ3: マテリアルの設定
1. Assets/Materials/Cockpitフォルダに新しいマテリアルを作成
2. Shaderを`Unlit/Texture`または`Universal Render Pipeline/Unlit`に設定
3. このマテリアルをCockpitSphereのMeshRendererに適用

#### ステップ4: コックピットマネージャーの設定
1. 空のGameObjectを作成し、"CockpitSystem"と名付ける
2. `CockpitManager`コンポーネントを追加
3. Inspectorで以下を設定:
   - **Projection Camera Object**: ProjectionCameraオブジェクトをドラッグ
   - **Cockpit Sphere Object**: CockpitSphereオブジェクトをドラッグ
   - **VR Camera**: XR OriginのMain Cameraをドラッグ
   - **Isolated Position**: コックピットを配置する隔離された座標（例: 1000, 1000, 1000）

#### ステップ5: コンポーネントの接続
1. ProjectionCameraの`CockpitCameraProjection`コンポーネントで:
   - **Cockpit Sphere Renderer**: CockpitSphereのMeshRendererをドラッグ

### 2. 詳細設定

#### CockpitCameraProjection設定
- **Field of View**: カメラの視野角（60-179度）
- **Near Clip Plane**: 最小描画距離
- **Far Clip Plane**: 最大描画距離
- **Cockpit Radius**: コックピット球体の半径

#### InvertedSphereMesh設定
- **Segments**: 球体の分割数（高いほど滑らか、デフォルト48）
- **Radius**: 球体の基本半径

## 動作原理

### 1. カメラ投影
```
ProjectionCamera (ロボット視点) → RenderTexture → CockpitSphere Material
```

1. ProjectionCameraがロボットの視点からシーンを描画
2. 描画結果がRenderTextureに保存される
3. RenderTextureがコックピット球体のマテリアルのテクスチャとして使用される

### 2. 座標の隔離
- コックピット球体は（1000, 1000, 1000）などの離れた座標に配置
- VRプレイヤー（XR Origin）もその座標に配置
- ProjectionCameraは実際のロボット位置に配置

これにより：
- プレイヤーはコックピット内に閉じ込められる
- コックピット表面にロボットの視点が投影される
- 実際のゲーム世界とコックピットが物理的に分離される

### 3. 内側球体レンダリング
- 通常の球体は外側からしか見えない
- InvertedSphereMeshは法線を反転し、三角形の巻き順を逆にする
- これにより球体の内側から表面が見えるようになる

## 使用例

### ロボットへの投影カメラのアタッチ
```csharp
public class RobotController : MonoBehaviour
{
    [SerializeField] private CockpitManager cockpitManager;
    [SerializeField] private Transform robotHead;
    
    void Start()
    {
        cockpitManager.AttachProjectionCameraToRobot(robotHead);
    }
}
```

### コックピットの動的な有効化/無効化
```csharp
// コックピットモードを有効化
cockpitManager.SetCockpitActive(true);

// 通常モードに戻す
cockpitManager.SetCockpitActive(false);
```

### コックピット位置の変更
```csharp
// 異なる隔離座標に変更
cockpitManager.SetCockpitPosition(new Vector3(2000f, 2000f, 2000f));
```

## パフォーマンス最適化

### RenderTextureの解像度
デフォルトは2048x2048ですが、パフォーマンスに応じて調整可能：
```csharp
CockpitCameraProjection projection = GetComponent<CockpitCameraProjection>();
projection.SetRenderTextureResolution(1024, 1024); // 低解像度
projection.SetRenderTextureResolution(4096, 4096); // 高解像度
```

### 球体の分割数
InvertedSphereMeshのsegmentsを調整：
- 低スペック: 24-32
- 中スペック: 48（デフォルト）
- 高スペック: 64-96

### カリング最適化
- ProjectionCameraのCulling Maskを適切に設定
- 不要なレイヤーを描画しないようにする

## トラブルシューティング

### 球体の内側が見えない
- InvertedSphereMeshコンポーネントが正しくアタッチされているか確認
- MeshFilterにメッシュが設定されているか確認
- マテリアルのCull Modeが正しく設定されているか確認（通常はOff）

### 映像が投影されない
- CockpitCameraProjectionのCockpit Sphere Rendererが設定されているか確認
- ProjectionCameraがアクティブか確認
- RenderTextureが正しく生成されているか確認

### VRカメラが正しい位置にない
- CockpitManagerのVR Camera参照が正しいか確認
- XR Originが存在するか確認
- Isolated Positionが適切に設定されているか確認

### パフォーマンスの問題
- RenderTextureの解像度を下げる
- ProjectionCameraのCulling Maskを最適化
- 球体の分割数を減らす
- Anti-Aliasingを無効化または軽量化

## 技術的な考慮事項

### シェーダーの選択
- **Unlit/Texture**: 最もシンプルで軽量
- **URP/Unlit**: Universal Render Pipeline使用時
- **Custom Shader**: より高度なエフェクトが必要な場合

### VR最適化
- Single Pass Instanced Renderingを有効化
- Fixed Foveated Renderingの使用を検討
- テクスチャ圧縮の活用

### マルチプレイヤー対応
- 各プレイヤーに個別のCockpitSystemを用意
- 隔離座標をプレイヤーIDに基づいて動的に計算
- ネットワーク同期が必要なのはProjectionCameraの位置と回転のみ

## まとめ

このシステムは、VRでの没入感の高いロボット操縦体験を実現します。プレイヤーは実際にコックピット内にいるような感覚を得ながら、ロボットの視点から世界を見ることができます。

座標の隔離により、ゲームワールドとコックピット環境を完全に分離でき、複雑な空間管理を回避できます。

## 参考資料

- Unity XR Interaction Toolkit Documentation
- Unity RenderTexture Documentation
- Procedural Mesh Generation in Unity
