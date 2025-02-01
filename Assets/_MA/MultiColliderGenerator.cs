using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HierarchicalColliderGenerator : EditorWindow {
    [SerializeField] private int maxColliderCount = 10;
    [SerializeField] private float minColliderY = 2.0f;
    [SerializeField] private int maxDepth = 4; // maximum quadtree depth

    [MenuItem("Tools/Generate Hierarchical Colliders")]
    public static void ShowWindow() {
        GetWindow<HierarchicalColliderGenerator>("Hierarchical Colliders");
    }

    void OnGUI() {
        GUILayout.Label("Hierarchical Collider Generation Settings", EditorStyles.boldLabel);
        maxColliderCount = EditorGUILayout.IntField("Max Collider Count", maxColliderCount);
        minColliderY = EditorGUILayout.FloatField("Min Collider Y", minColliderY);
        maxDepth = EditorGUILayout.IntField("Max Tree Depth", maxDepth);
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
        List<Vector3> filteredVertices = new List<Vector3>();
        foreach (Vector3 v in mesh.vertices) {
            Vector3 worldV = mf.transform.TransformPoint(v);
            if (worldV.y >= minColliderY)
                filteredVertices.Add(worldV);
        }
        if (filteredVertices.Count == 0) {
            Debug.LogWarning("No vertices above minColliderY found.");
            return;
        }

        // Build a quadtree from filtered vertices.
        Quadtree tree = new Quadtree(filteredVertices, maxDepth);
        List<OrientedBoundingBox> boxes = tree.GetBoundingBoxes();

        // If too many boxes, merge the smallest ones until count <= maxColliderCount.
        while (boxes.Count > maxColliderCount) {
            // For simplicity, merge the two smallest boxes.
            boxes.Sort((a, b) => a.Area.CompareTo(b.Area));
            OrientedBoundingBox merged = OrientedBoundingBox.Merge(boxes[0], boxes[1]);
            boxes.RemoveAt(0);
            boxes.RemoveAt(0);
            boxes.Add(merged);
        }

        GameObject container = new GameObject("GeneratedColliders");
        container.transform.SetParent(parent.transform, false);
        int index = 0;
        foreach (OrientedBoundingBox obb in boxes) {
            CreateBoxColliderFromOBB(obb, container.transform, index++);
        }
        Debug.Log("Generated " + boxes.Count + " colliders on " + parent.name);
    }

    void CreateBoxColliderFromOBB(OrientedBoundingBox obb, Transform parent, int index) {
        // Adjust the collider so that its top (local Z + half depth) is flush with the surface.
        float depth = obb.Size.z;
        float adjustedZ = obb.Center.z - depth * 0.5f;
        Vector3 localCenter = new Vector3(obb.Center.x, obb.Center.y, adjustedZ);

        GameObject colliderObj = new GameObject("Collider_" + index);
        colliderObj.transform.position = obb.Position; // world position computed from OBB
        colliderObj.transform.rotation = obb.Rotation; // rotation from OBB
        colliderObj.transform.SetParent(parent, true);

        BoxCollider box = colliderObj.AddComponent<BoxCollider>();
        box.size = obb.Size;
        // Shift the collider so that its top face is flush.
        box.center = new Vector3(0, 0, -depth * 0.5f);
        colliderObj.layer = 7;
    }
}

// ---- Helper Classes for Quadtree and OrientedBoundingBox ----

public class OrientedBoundingBox {
    public Vector3 Center;
    public Vector3 Size; // width, height, depth along local axes
    public Quaternion Rotation;
    public Vector3 Position; // world position

    public float Area { get { return Size.x * Size.y; } }

    public OrientedBoundingBox(Vector3 center, Vector3 size, Quaternion rotation, Vector3 position) {
        Center = center;
        Size = size;
        Rotation = rotation;
        Position = position;
    }

    public static OrientedBoundingBox Merge(OrientedBoundingBox a, OrientedBoundingBox b) {
        // A simple merge: combine the vertices of both OBBs and compute a new OBB.
        List<Vector3> points = new List<Vector3>();
        points.AddRange(a.GetCorners());
        points.AddRange(b.GetCorners());
        return OrientedBoundingBox.ComputeFromPoints(points);
    }

    public Vector3[] GetCorners() {
        Vector3 extents = Size * 0.5f;
        Vector3[] localCorners = new Vector3[8] {
            new Vector3(-extents.x, -extents.y, -extents.z),
            new Vector3(extents.x, -extents.y, -extents.z),
            new Vector3(-extents.x, extents.y, -extents.z),
            new Vector3(extents.x, extents.y, -extents.z),
            new Vector3(-extents.x, -extents.y, extents.z),
            new Vector3(extents.x, -extents.y, extents.z),
            new Vector3(-extents.x, extents.y, extents.z),
            new Vector3(extents.x, extents.y, extents.z)
        };
        Vector3[] worldCorners = new Vector3[8];
        for (int i = 0; i < 8; i++) {
            worldCorners[i] = Position + Rotation * localCorners[i];
        }
        return worldCorners;
    }

    public static OrientedBoundingBox ComputeFromPoints(List<Vector3> points) {
        // For simplicity, compute axis-aligned bounds and treat that as an OBB.
        Vector3 min = points[0], max = points[0];
        foreach (Vector3 p in points) {
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
        }
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;
        return new OrientedBoundingBox(center, size, Quaternion.identity, center);
    }
}

public class Quadtree {
    public Rect bounds;
    public List<Vector3> points;
    public int depth;
    public int maxDepth;
    public Quadtree[] children;

    public Quadtree(List<Vector3> points, int maxDepth) {
        // For a quadtree, we assume points lie roughly on a horizontal plane.
        // We'll compute bounds in the X-Y plane.
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (Vector3 p in points) {
            minX = Mathf.Min(minX, p.x);
            maxX = Mathf.Max(maxX, p.x);
            minY = Mathf.Min(minY, p.y);
            maxY = Mathf.Max(maxY, p.y);
        }
        bounds = new Rect(minX, minY, maxX - minX, maxY - minY);
        this.points = points;
        depth = 0;
        this.maxDepth = maxDepth;
        Subdivide();
    }

    void Subdivide() {
        if (depth >= maxDepth || points.Count < 2)
            return;
        children = new Quadtree[4];
        float halfWidth = bounds.width / 2f;
        float halfHeight = bounds.height / 2f;
        Rect[] quadrants = new Rect[4] {
            new Rect(bounds.x, bounds.y, halfWidth, halfHeight),
            new Rect(bounds.x + halfWidth, bounds.y, halfWidth, halfHeight),
            new Rect(bounds.x, bounds.y + halfHeight, halfWidth, halfHeight),
            new Rect(bounds.x + halfWidth, bounds.y + halfHeight, halfWidth, halfHeight)
        };
        for (int i = 0; i < 4; i++) {
            List<Vector3> quadPoints = new List<Vector3>();
            foreach (Vector3 p in points) {
                if (quadrants[i].Contains(new Vector2(p.x, p.y)))
                    quadPoints.Add(p);
            }
            children[i] = new Quadtree(quadPoints, maxDepth) { depth = this.depth + 1, bounds = quadrants[i] };
        }
    }

    public List<OrientedBoundingBox> GetBoundingBoxes() {
        List<OrientedBoundingBox> boxes = new List<OrientedBoundingBox>();
        if (children == null) {
            // Create an OBB from the points in this node.
            if (points.Count > 0) {
                Vector3 min = points[0], max = points[0];
                foreach (Vector3 p in points) {
                    min = Vector3.Min(min, p);
                    max = Vector3.Max(max, p);
                }
                Vector3 center = (min + max) * 0.5f;
                Vector3 size = max - min;
                // Assume no rotation (aligned with world axes)
                boxes.Add(new OrientedBoundingBox(center, size, Quaternion.identity, center));
            }
        } else {
            foreach (Quadtree child in children) {
                boxes.AddRange(child.GetBoundingBoxes());
            }
        }
        return boxes;
    }
}
