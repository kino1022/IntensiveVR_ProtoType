using UnityEngine;

namespace IntensiveVR.Cockpit
{
    /// <summary>
    /// 球体メッシュを反転させて内側から見えるようにするユーティリティ
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class InvertedSphereMesh : MonoBehaviour
    {
        [Tooltip("球体の分割数（高いほど滑らか）")]
        [SerializeField] private int segments = 48;
        
        [Tooltip("球体の半径")]
        [SerializeField] private float radius = 1f;
        
        private MeshFilter meshFilter;
        private int lastSegments;
        private float lastRadius;
        
        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        
        private void Start()
        {
            GenerateInvertedSphereMesh();
            lastSegments = segments;
            lastRadius = radius;
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
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);
                
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
                    uvs[index] = new Vector2((float)lon / segments, (float)lat / segments);
                    
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
                    triangles[triIndex++] = current + 1;
                    triangles[triIndex++] = next;
                    
                    triangles[triIndex++] = current + 1;
                    triangles[triIndex++] = next + 1;
                    triangles[triIndex++] = next;
                }
            }
            
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            
            mesh.RecalculateBounds();
            
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
            }
        }
        
        private void OnValidate()
        {
            // Only regenerate if properties actually changed
            if (Application.isPlaying && meshFilter != null)
            {
                if (segments != lastSegments || !Mathf.Approximately(radius, lastRadius))
                {
                    GenerateInvertedSphereMesh();
                    lastSegments = segments;
                    lastRadius = radius;
                }
            }
        }
    }
}
