using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Triangle : MonoBehaviour
{

    public Vector3[] _vertices;
    public int[] _triangles;

    private void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
    }
}
