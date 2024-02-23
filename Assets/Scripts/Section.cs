using UnityEngine;
using System.Collections.Generic;

public class MeshSlicer : MonoBehaviour
{
    public GameObject sectionPlane;

    void Start()
    {
        SliceMesh(gameObject);
    }

    void SliceMesh(GameObject meshObject)
    {
        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found on object: " + meshObject.name);
            return;
        }

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;
        // Check if normals array is the same size as vertices array
        if (normals.Length != vertices.Length)
        {
            // If not, create a new normals array of the correct size
            normals = new Vector3[vertices.Length];
            for (int i = 0; i < normals.Length; i++)
            {
                // Assign a default normal direction (this may not be correct for your mesh)
                normals[i] = Vector3.up;
            }

            // Assign the new normals array to the mesh
            mesh.normals = normals;
        }

        Plane plane = new Plane(sectionPlane.transform.up, sectionPlane.transform.position);

        // Define lists to store sliced mesh data
        List<Vector3> frontVertices = new List<Vector3>();
        List<Vector3> backVertices = new List<Vector3>();
        List<int> frontTriangles = new List<int>();
        List<int> backTriangles = new List<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3[] triangleVertices = new Vector3[3];
            for (int j = 0; j < 3; j++)
            {
                int vertexIndex = triangles[i + j];
                triangleVertices[j] = vertices[vertexIndex];
            }

            bool[] side = new bool[3];
            for (int j = 0; j < 3; j++)
            {
                side[j] = plane.GetSide(triangleVertices[j]);
            }

            if (side[0] == side[1] && side[0] == side[2])
            {
                // Triangle is entirely on one side of the plane
                if (side[0])
                {
                    // Front side
                    frontVertices.AddRange(triangleVertices);
                    frontTriangles.Add(frontVertices.Count - 3);
                    frontTriangles.Add(frontVertices.Count - 2);
                    frontTriangles.Add(frontVertices.Count - 1);
                }
                else
                {
                    // Back side
                    backVertices.AddRange(triangleVertices);
                    backTriangles.Add(backVertices.Count - 3);
                    backTriangles.Add(backVertices.Count - 1);
                    backTriangles.Add(backVertices.Count - 2);
                }
            }
            else
            {
                // Triangle intersects the plane
                List<Vector3> intersectingVertices = new List<Vector3>();
                for (int j = 0; j < 3; j++)
                {
                    int nextIndex = (j + 1) % 3;
                    if (side[j] != side[nextIndex])
                    {
                        // Calculate intersection point
                        Vector3 point1 = triangleVertices[j];
                        Vector3 point2 = triangleVertices[nextIndex];
                        float distance1 = plane.GetDistanceToPoint(point1);
                        float distance2 = plane.GetDistanceToPoint(point2);
                        float t = distance1 / (distance1 - distance2);
                        Vector3 intersection = Vector3.Lerp(point1, point2, t);
                        intersectingVertices.Add(intersection);
                    }
                }

                if (intersectingVertices.Count == 2)
                {
                    // Add intersecting vertices to appropriate lists
                    if (side[0])
                    {
                        frontVertices.AddRange(intersectingVertices);
                        backVertices.AddRange(intersectingVertices);
                        frontVertices.AddRange(new Vector3[] { triangleVertices[0], intersectingVertices[0], intersectingVertices[1] });
                        backVertices.AddRange(new Vector3[] { triangleVertices[0], intersectingVertices[1], intersectingVertices[0] });
                    }
                    else
                    {
                        backVertices.AddRange(intersectingVertices);
                        frontVertices.AddRange(intersectingVertices);
                        backVertices.AddRange(new Vector3[] { triangleVertices[0], intersectingVertices[0], intersectingVertices[1] });
                        frontVertices.AddRange(new Vector3[] { triangleVertices[0], intersectingVertices[1], intersectingVertices[0] });
                    }
                }
            }
        }

        // Create new mesh for sliced parts
        Mesh frontMesh = new Mesh();
        frontMesh.vertices = frontVertices.ToArray();
        frontMesh.triangles = frontTriangles.ToArray();
        frontMesh.normals = normals;
        frontMesh.RecalculateBounds();
        frontMesh.RecalculateNormals();

        Mesh backMesh = new Mesh();
        backMesh.vertices = backVertices.ToArray();
        backMesh.triangles = backTriangles.ToArray();
        backMesh.normals = normals;
        backMesh.RecalculateBounds();
        backMesh.RecalculateNormals();

        // Create GameObjects to represent sliced parts
        GameObject frontObject = new GameObject("Front Slice");
        frontObject.AddComponent<MeshFilter>().mesh = frontMesh;
        frontObject.AddComponent<MeshRenderer>().material = meshObject.GetComponent<MeshRenderer>().material;
        frontObject.transform.position = meshObject.transform.position;
        frontObject.transform.rotation = meshObject.transform.rotation;

        GameObject backObject = new GameObject("Back Slice");
        backObject.AddComponent<MeshFilter>().mesh = backMesh;
        backObject.AddComponent<MeshRenderer>().material = meshObject.GetComponent<MeshRenderer>().material;
        backObject.transform.position = meshObject.transform.position;
        backObject.transform.rotation = meshObject.transform.rotation;
        backObject.transform.localScale = meshObject.transform.localScale;

        // Optionally, you can destroy or disable the original mesh object
        // Destroy(meshObject);
    }
}
