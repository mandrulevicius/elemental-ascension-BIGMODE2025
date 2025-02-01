using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MultiColliderGenerator : EditorWindow {
  [MenuItem("Tools/Generate Multiple Colliders")]
  public static void ShowWindow() {
    GetWindow(typeof(MultiColliderGenerator));
  }

  void OnGUI() {
    if (GUILayout.Button("Generate Colliders for Selected Prefab")) {
      GameObject selected = Selection.activeGameObject;
      if (selected != null) {
        GenerateColliders(selected);
      } else {
        Debug.LogWarning("Please select a prefab with a MeshFilter.");
      }
    }
  }

  // Helper class for grouping triangles
  class TriangleGroup {
    public Vector3 averageNormal;
    public List<Vector3> points = new List<Vector3>();
    public float totalArea = 0f;
    public Vector3 centroidSum = Vector3.zero;
    public int count = 0;

    public Vector3 GetCentroid() {
      return (count > 0) ? centroidSum / count : Vector3.zero;
    }

    public void AddTriangle(Vector3[] triPoints, Vector3 normal, float area, Vector3 centroid) {
      totalArea += area;
      centroidSum += centroid;
      count++;
      // Simple running average for normal
      averageNormal = ((averageNormal * (count - 1)) + normal).normalized;
      points.AddRange(triPoints);
    }
  }

  static void GenerateColliders(GameObject obj) {
    MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
    if (mf == null) {
      Debug.LogWarning("No MeshFilter found on " + obj.name);
      return;
    }
    Mesh mesh = mf.sharedMesh;
    if (mesh == null) {
      Debug.LogWarning("MeshFilter has no mesh.");
      return;
    }

    Vector3[] vertices = mesh.vertices;
    int[] triangles = mesh.triangles;
    Bounds meshBounds = mesh.bounds;
    Vector3 meshCenter = meshBounds.center;

    List<TriangleGroup> groups = new List<TriangleGroup>();
    float normalThreshold = 0.95f;              // similarity of face normals
    float distanceThreshold = meshBounds.size.magnitude * 0.1f; // grouping spatially

    // Iterate through all triangles in the mesh
    for (int i = 0; i < triangles.Length; i += 3) {
      int i0 = triangles[i];
      int i1 = triangles[i + 1];
      int i2 = triangles[i + 2];

      Vector3 v0 = vertices[i0];
      Vector3 v1 = vertices[i1];
      Vector3 v2 = vertices[i2];

      Vector3 triNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
      Vector3 triCentroid = (v0 + v1 + v2) / 3f;
      float area = Vector3.Cross(v1 - v0, v2 - v0).magnitude * 0.5f;

      // Use extrusion (distance from mesh center along the normal) to filter minor faces.
      float extrusion = Vector3.Dot(triCentroid - meshCenter, triNormal);
      if (extrusion < distanceThreshold * 0.5f)
        continue;

      bool added = false;
      // Try to add triangle to an existing group
      foreach (var group in groups) {
        if (Vector3.Dot(group.averageNormal, triNormal) > normalThreshold) {
          Vector3 groupCentroid = group.GetCentroid();
          if (Vector3.Distance(groupCentroid, triCentroid) < distanceThreshold) {
            group.AddTriangle(new Vector3[] { v0, v1, v2 }, triNormal, area, triCentroid);
            added = true;
            break;
          }
        }
      }
      // No suitable group? Create a new one.
      if (!added) {
        TriangleGroup newGroup = new TriangleGroup();
        newGroup.averageNormal = triNormal;
        newGroup.AddTriangle(new Vector3[] { v0, v1, v2 }, triNormal, area, triCentroid);
        groups.Add(newGroup);
      }
    }

    // Sort groups by their total area (largest surfaces first)
    groups.Sort((a, b) => b.totalArea.CompareTo(a.totalArea));
    int maxGroups = Mathf.Min(10, groups.Count);

    // Create a thin box collider for each top group.
    for (int i = 0; i < maxGroups; i++) {
      CreateColliderForGroup(obj, groups[i], i);
    }

    Debug.Log("Generated " + maxGroups + " colliders on " + obj.name);
  }

  // Creates a thin BoxCollider approximating the group surface.
  static void CreateColliderForGroup(GameObject parent, TriangleGroup group, int index) {
    // Use the group's average normal as the Z axis.
    Vector3 axisZ = group.averageNormal;
    // Choose an arbitrary vector to generate an X axis.
    Vector3 arbitrary = Vector3.up;
    if (Mathf.Abs(Vector3.Dot(axisZ, arbitrary)) > 0.99f)
      arbitrary = Vector3.right;
    Vector3 axisX = Vector3.Cross(axisZ, arbitrary).normalized;
    Vector3 axisY = Vector3.Cross(axisZ, axisX);

    // Project all collected points into this coordinate frame.
    float minX = float.MaxValue, maxX = float.MinValue;
    float minY = float.MaxValue, maxY = float.MinValue;
    float sumZ = 0f;
    foreach (var pt in group.points) {
      float x = Vector3.Dot(pt, axisX);
      float y = Vector3.Dot(pt, axisY);
      float z = Vector3.Dot(pt, axisZ);
      if (x < minX) minX = x;
      if (x > maxX) maxX = x;
      if (y < minY) minY = y;
      if (y > maxY) maxY = y;
      sumZ += z;
    }
    float avgZ = sumZ / group.points.Count;

    // Compute the center and size of the projected bounding box.
    float centerX = (minX + maxX) * 0.5f;
    float centerY = (minY + maxY) * 0.5f;
    Vector3 localCenter = (axisX * centerX) + (axisY * centerY) + (axisZ * avgZ);
    Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f); // set thickness to a small value

    // Create a child GameObject for the collider.
    GameObject colliderObj = new GameObject("ColliderGroup_" + index);
    colliderObj.transform.parent = parent.transform;
    colliderObj.transform.localPosition = localCenter;
    // Align the collider to the computed axes.
    colliderObj.transform.localRotation = Quaternion.LookRotation(axisZ, axisY);

    BoxCollider box = colliderObj.AddComponent<BoxCollider>();
    box.center = Vector3.zero;
    box.size = size;

    // Set collider to layer 10.
    colliderObj.layer = 10;
  }
}
