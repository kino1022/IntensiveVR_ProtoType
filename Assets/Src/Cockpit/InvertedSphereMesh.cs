using UnityEngine;

namespace IntensiveVR. Cockpit
{
    /// <summary>
    /// 球体メッシュを反転させて内側から見えるようにするユーティリティ
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class InvertedSphereMesh : MonoBehaviour
    {
        [Tooltip("球体の分割数（高いほど滑らか��")]
        [SerializeField] private int segments = 48;
        
        [Tooltip("球体の半径")]
        [SerializeField] private float radius = 1f;
        
        [Tooltip("UVを水平方向に反転")]
        [SerializeField] private bool flipUVHorizontal = true;
        
        [Tooltip("UVを垂直方向に反転")]
        [SerializeField] private bool flipUVVertical = false;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private int lastSegments;
        private float lastRadius;
        private bool lastFlipUVHorizontal;
        private bool lastFlipUVVertical;
        
        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        
        private void Start()
        {
            GenerateInvertedSphereMesh();
            lastSegments = segments;
            lastRadius = radius;
            lastFlipUVHorizontal = flipUVHorizontal;
            lastFlipUVVertical = flipUVVertical;
        }
        
        /// <summary>
        /// 内側を向いた球体メッシュを生成
        /// </summary>
        public void GenerateInvertedSphereMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Inverted Sphere";
            
            int vertexCount = (segments + 1) * (segments + 1);
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            
            // 頂点、法線、UVを生成
            int index = 0;
            for (int lat = 0; lat <= segments; lat++)
            {
                float theta = lat * Mathf.PI / segments;
                float sinTheta = Mathf. Sin(theta);
                float cosTheta = Mathf. Cos(theta);
                
                for (int lon = 0; lon <= segments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / segments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);
                    
                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;
                    
                    vertices[index] = new Vector3(x, y, z) * radius;
                    
                    // 法線を内側に向ける（通常の球体と逆）
                    normals[index] = new Vector3(-x, -y, -z);
                    
                    // UV座標の計算（内側から見るための調整）
                    float u = (float)lon / segments;
                    float v = (float)lat / segments;
                    
                    // 水平方向の反転（左右を正しく表示）
                    if (flipUVHorizontal)
                    {
                        u = 1.0f - u;
                    }
                    
                    // 垂直方向の反転（必要に応じて）
                    if (flipUVVertical)
                    {
                        v = 1.0f - v;
                    }
                    
                    uvs[index] = new Vector2(u, v);
                    
                    index++;
                }
            }
            
            // 三角形インデックスを生成（裏面を表にする）
            int triangleCount = segments * segments * 6;
            int[] triangles = new int[triangleCount];
            int triIndex = 0;
            
            for (int lat = 0; lat < segments; lat++)
            {
                for (int lon = 0; lon < segments; lon++)
                {
                    int current = lat * (segments + 1) + lon;
                    int next = current + segments + 1;
                    
                    // 三角形の巻き順を反転（内側から見えるように）
                    triangles[triIndex++] = current;
                    triangles[triIndex++] = next;
                    triangles[triIndex++] = current + 1;
                    
                    triangles[triIndex++] = current + 1;
                    triangles[triIndex++] = next;
                    triangles[triIndex++] = next + 1;
                }
            }
            
            mesh. vertices = vertices;
            mesh. normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            
            mesh. RecalculateBounds();
            
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
                
                if (debugMode)
                {
                    Debug.Log($"[InvertedSphereMesh] Generated mesh:");
                    Debug.Log($"  Vertices: {vertices.Length}");
                    Debug.Log($"  Triangles: {triangles.Length / 3}");
                    Debug.Log($"  Radius: {radius}");
                    Debug.Log($"  Segments: {segments}");
                    Debug.Log($"  UV Flip: H={flipUVHorizontal}, V={flipUVVertical}");
                }
            }
            else
            {
                Debug.LogError("[InvertedSphereMesh] MeshFilter is null!");
            }
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && meshFilter != null)
            {
                bool needsRegeneration = segments != lastSegments 
                                      || !Mathf.Approximately(radius, lastRadius)
                                      || flipUVHorizontal != lastFlipUVHorizontal
                                      || flipUVVertical != lastFlipUVVertical;
                
                if (needsRegeneration)
                {
                    GenerateInvertedSphereMesh();
                    lastSegments = segments;
                    lastRadius = radius;
                    lastFlipUVHorizontal = flipUVHorizontal;
                    lastFlipUVVertical = flipUVVertical;
                }
            }
        }
        
        [ContextMenu("Regenerate Mesh")]
        public void RegenerateMesh()
        {
            GenerateInvertedSphereMesh();
        }
        
        [ContextMenu("Debug Mesh Info")]
        private void DebugMeshInfo()
        {
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Debug.Log($"=== Inverted Sphere Mesh Info ===");
                Debug.Log($"Vertex Count: {mesh. vertexCount}");
                Debug.Log($"Triangle Count: {mesh.triangles.Length / 3}");
                Debug.Log($"Bounds: {mesh. bounds}");
                
                // 最初の数頂点のUVを表示
                if (mesh.uv.Length >= 3)
                {
                    Debug.Log($"First 3 UV coordinates:");
                    Debug.Log($"  UV[0]:  {mesh.uv[0]}");
                    Debug.Log($"  UV[1]: {mesh.uv[1]}");
                    Debug.Log($"  UV[2]: {mesh.uv[2]}");
                }
                
                // 最初の三角形を表示
                if (mesh.triangles.Length >= 3)
                {
                    Debug.Log($"First Triangle:");
                    Debug.Log($"  Indices: [{mesh.triangles[0]}, {mesh.triangles[1]}, {mesh.triangles[2]}]");
                    Debug.Log($"  V0: {mesh.vertices[mesh.triangles[0]]}");
                    Debug.Log($"  V1: {mesh.vertices[mesh.triangles[1]]}");
                    Debug.Log($"  V2: {mesh.vertices[mesh.triangles[2]]}");
                }
            }
            else
            {
                Debug. LogWarning("No mesh found!");
            }
        }
    }
}