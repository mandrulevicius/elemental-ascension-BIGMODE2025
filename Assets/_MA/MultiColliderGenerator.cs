using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MultiColliderGeneratorAdaptive : EditorWindow {
  [SerializeField] private int maxColliderCount = 10;
  [SerializeField] private float minColliderY = 2.0f; // Only consider triangles with centroid.y >= this value

  // DBSCAN parameters – you may want to adjust these
  [SerializeField] private float eps = 0.5f;   // maximum distance for neighbors (in world units)
  [SerializeField] private int minPts = 1;       // minimum points to form a cluster

  [MenuItem("Tools/Generate Adaptive Colliders")]
  public static void ShowWindow() {
    GetWindow<MultiColliderGeneratorAdaptive>("Adaptive Collider Generator");
  }

  void OnGUI() {
    GUILayout.Label("Adaptive Collider Generation Settings", EditorStyles.boldLabel);
    maxColliderCount = EditorGUILayout.IntField("Max Collider Count", maxColliderCount);
    minColliderY = EditorGUILayout.FloatField("Min Collider Y", minColliderY);
    eps = EditorGUILayout.FloatField("DBSCAN eps", eps);
    minPts = EditorGUILayout.IntField("DBSCAN minPts", minPts);

    if (GUILayout.Button("Generate Colliders for Selected Prefab")) {
      GameObject selected = Selection.activeGameObject;
      if (selected != null) {
        GenerateColliders(selected);
      } else {
        Debug.LogWarning("Please select a prefab with a MeshFilter.");
      }
    }
  }

  void GenerateColliders(GameObject parent) {
    MeshFilter mf = parent.GetComponentInChildren<MeshFilter>();
    if (mf == null || mf.sharedMesh == null) {
      Debug.LogWarning("No MeshFilter or mesh found on " + parent.name);
      return;
    }
    Mesh mesh = mf.sharedMesh;
    // Process the mesh into triangle data and filter by minColliderY.
    MeshProcessorAdaptive processor = new MeshProcessorAdaptive(mesh, mf.transform, minColliderY);
    List<TriangleData> triangles = processor.ExtractTriangles();
    // Cluster the triangle data adaptively using DBSCAN.
    List<List<TriangleData>> clusters = DBSCAN.Cluster(triangles, eps, minPts);
    
    // Convert each cluster into a Patch.
    List<Patch> patches = new List<Patch>();
    foreach (var cluster in clusters) {
      Patch patch = new Patch();
      foreach (var tri in cluster) {
        // Add all vertices from this triangle.
        patch.points.AddRange(tri.vertices);
        patch.AddNormal(tri.normal);
      }
      patch.ComputeLocalBounds();
      patches.Add(patch);
    }
    
    // Optionally, if we have more patches than needed, we can sort by area.
    patches.Sort((a, b) => b.totalArea.CompareTo(a.totalArea));
    int finalCount = Mathf.Min(maxColliderCount, patches.Count);
    
    // Create container for colliders.
    GameObject container = new GameObject("GeneratedColliders");
    container.transform.SetParent(parent.transform, false);
    ColliderGeneratorAdaptive colliderGen = new ColliderGeneratorAdaptive();
    for (int i = 0; i < finalCount; i++) {
      colliderGen.GenerateCollider(patches[i], container.transform, i);
    }
    Debug.Log("Generated " + finalCount + " adaptive colliders on " + parent.name);
  }
}

// ----------------------------------------------------------------------
// Data class for a single triangle.
public class TriangleData {
  public Vector3 centroid;
  public Vector3 normal;
  public Vector3[] vertices;
  public TriangleData(Vector3[] vertices) {
    this.vertices = vertices;
    centroid = (vertices[0] + vertices[1] + vertices[2]) / 3f;
    normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).normalized;
  }
}

// ----------------------------------------------------------------------
// Simplified DBSCAN algorithm for TriangleData.
public static class DBSCAN {
  public static List<List<TriangleData>> Cluster(List<TriangleData> points, float eps, int minPts) {
    List<List<TriangleData>> clusters = new List<List<TriangleData>>();
    Dictionary<TriangleData, bool> visited = new Dictionary<TriangleData, bool>();
    foreach (TriangleData pt in points) {
      visited[pt] = false;
    }
    foreach (TriangleData pt in points) {
      if (visited[pt])
        continue;
      visited[pt] = true;
      List<TriangleData> neighbors = RegionQuery(points, pt, eps);
      if (neighbors.Count < minPts) {
        // Mark as noise (ignored for now)
        continue;
      }
      List<TriangleData> cluster = new List<TriangleData>();
      ExpandCluster(points, pt, neighbors, cluster, eps, minPts, visited);
      clusters.Add(cluster);
    }
    return clusters;
  }

  static void ExpandCluster(List<TriangleData> points, TriangleData pt, List<TriangleData> neighbors,
                              List<TriangleData> cluster, float eps, int minPts, Dictionary<TriangleData, bool> visited) {
    cluster.Add(pt);
    for (int i = 0; i < neighbors.Count; i++) {
      TriangleData n = neighbors[i];
      if (!visited[n]) {
        visited[n] = true;
        List<TriangleData> nNeighbors = RegionQuery(points, n, eps);
        if (nNeighbors.Count >= minPts) {
          neighbors.AddRange(nNeighbors);
        }
      }
      if (!IsInCluster(cluster, n))
        cluster.Add(n);
    }
  }

  static bool IsInCluster(List<TriangleData> cluster, TriangleData pt) {
    return cluster.Contains(pt);
  }

  static List<TriangleData> RegionQuery(List<TriangleData> points, TriangleData pt, float eps) {
    List<TriangleData> neighbors = new List<TriangleData>();
    foreach (TriangleData other in points) {
      if (Vector3.Distance(pt.centroid, other.centroid) <= eps) {
        neighbors.Add(other);
      }
    }
    return neighbors;
  }
}

// ----------------------------------------------------------------------
// MeshProcessorAdaptive extracts triangle data from the mesh.
public class MeshProcessorAdaptive {
  Mesh mesh;
  Transform meshTransform;
  float minColliderY;
  public MeshProcessorAdaptive(Mesh mesh, Transform transform, float minColliderY) {
    this.mesh = mesh;
    this.meshTransform = transform;
    this.minColliderY = minColliderY;
  }
  public List<TriangleData> ExtractTriangles() {
    List<TriangleData> tris = new List<TriangleData>();
    Vector3[] vertices = mesh.vertices;
    int[] indices = mesh.triangles;
    for (int i = 0; i < indices.Length; i += 3) {
      Vector3 v0 = meshTransform.TransformPoint(vertices[indices[i]]);
      Vector3 v1 = meshTransform.TransformPoint(vertices[indices[i + 1]]);
      Vector3 v2 = meshTransform.TransformPoint(vertices[indices[i + 2]]);
      Vector3 centroid = (v0 + v1 + v2) / 3f;
      if (centroid.y < minColliderY)
        continue;
      TriangleData tri = new TriangleData(new Vector3[] { v0, v1, v2 });
      tris.Add(tri);
    }
    return tris;
  }
}

// ----------------------------------------------------------------------
// Patch represents a cluster (surface patch) and computes its bounds.
public class Patch {
  public List<Vector3> points = new List<Vector3>();
  public Vector3 averageNormal = Vector3.zero;
  public Vector3 centroid = Vector3.zero;
  public int count = 0;
  public Rect bounds2D;   // in the local plane
  public float minZ, maxZ; // extents along the local Z axis
  public float totalArea;
  public Vector3 axisX, axisY, axisZ;

  // Add a normal from a triangle (we average them)
  public void AddNormal(Vector3 normal) {
    if (count == 0)
      averageNormal = normal;
    else
      averageNormal = ((averageNormal * count) + normal) / (count + 1);
    averageNormal.Normalize();
    count++;
  }
  
  // Compute the local coordinate system and bounds.
  public void ComputeLocalBounds() {
    axisZ = averageNormal;
    Vector3 arbitrary = Vector3.up;
    if (Mathf.Abs(Vector3.Dot(axisZ, arbitrary)) > 0.99f)
      arbitrary = Vector3.right;
    axisX = Vector3.Cross(axisZ, arbitrary).normalized;
    axisY = Vector3.Cross(axisZ, axisX);
    float minX = float.MaxValue, maxX = float.MinValue;
    float minY = float.MaxValue, maxY = float.MinValue;
    minZ = float.MaxValue; maxZ = float.MinValue;
    foreach (Vector3 p in points) {
      float x = Vector3.Dot(p, axisX);
      float y = Vector3.Dot(p, axisY);
      float z = Vector3.Dot(p, axisZ);
      if (x < minX) minX = x;
      if (x > maxX) maxX = x;
      if (y < minY) minY = y;
      if (y > maxY) maxY = y;
      if (z < minZ) minZ = z;
      if (z > maxZ) maxZ = z;
    }
    bounds2D = new Rect(minX, minY, maxX - minX, maxY - minY);
    totalArea = (maxX - minX) * (maxY - minY);
    // Also compute the overall centroid.
    Vector3 sum = Vector3.zero;
    foreach (Vector3 p in points) { sum += p; }
    centroid = (points.Count > 0) ? sum / points.Count : Vector3.zero;
  }
}

// ----------------------------------------------------------------------
// ColliderGeneratorAdaptive creates BoxColliders from patches.
public class ColliderGeneratorAdaptive {
  private const float thinMultiplier = 0.8f;
  private const float bottomPadding = 0.1f;
  
  public void GenerateCollider(Patch patch, Transform parent, int index) {
    // The top of the surface is at patch.maxZ.
    float topSide = patch.maxZ;
    float bottomSide = patch.minZ - bottomPadding;
    float computedThickness = topSide - bottomSide;
    float finalThickness = computedThickness * thinMultiplier;
    // Set local Z center so that (localCenter.z + finalThickness/2) == topSide.
    float localCenterZ = topSide - finalThickness * 0.5f;
    float width = patch.bounds2D.width;
    float height = patch.bounds2D.height;
    float centerX = patch.bounds2D.x + width * 0.5f;
    float centerY = patch.bounds2D.y + height * 0.5f;
    Vector3 localCenter = new Vector3(centerX, centerY, localCenterZ);
    // Convert local center to world space.
    Vector3 globalCenter = patch.axisX * localCenter.x + patch.axisY * localCenter.y + patch.axisZ * localCenter.z;
    
    GameObject colliderObj = new GameObject("ColliderPatch_" + index);
    colliderObj.transform.position = globalCenter;
    colliderObj.transform.rotation = Quaternion.LookRotation(patch.axisZ, patch.axisY);
    colliderObj.transform.SetParent(parent, true);
    
    BoxCollider box = colliderObj.AddComponent<BoxCollider>();
    box.size = new Vector3(width, height, finalThickness);
    // Shift the box's local center so its top face is flush with the mesh surface.
    box.center = new Vector3(0, 0, -finalThickness * 0.5f);
    
    colliderObj.layer = 7; // Assign to layer "Terrain" (index 7)
  }
}
