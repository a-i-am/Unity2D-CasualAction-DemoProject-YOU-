using UnityEditor;
using UnityEngine;

public class VillageLayoutWindow : EditorWindow
{
    private float centerX = 80f;
    private float groundY = 5f;
    private float width = 270f;
    private float wallHeight = 30f;
    private float cameraY = 11.5f;
    private float cameraHeight = 35f;
    private float parallaxSpeed = 0.03f;

    [MenuItem("Tools/Village Layout/Open Layout Tool")]
    private static void Open()
    {
        GetWindow<VillageLayoutWindow>("Village Layout");
    }

    private void OnGUI()
    {
        centerX = EditorGUILayout.FloatField("Center X", centerX);
        groundY = EditorGUILayout.FloatField("Ground Y", groundY);
        width = EditorGUILayout.FloatField("Width", width);
        wallHeight = EditorGUILayout.FloatField("Wall Height", wallHeight);
        cameraY = EditorGUILayout.FloatField("Camera Bounds Y", cameraY);
        cameraHeight = EditorGUILayout.FloatField("Camera Bounds Height", cameraHeight);
        parallaxSpeed = EditorGUILayout.Slider("Parallax Speed", parallaxSpeed, 0.01f, 0.12f);

        if (GUILayout.Button("Apply Ground, Walls, Camera Bounds, Parallax"))
            Apply();
    }

    private void Apply()
    {
        float left = centerX - width * 0.5f;
        float right = centerX + width * 0.5f;

        SetTransform("VillageWalkableGround", new Vector3(centerX, groundY, 0f));
        SetBox("VillageWalkableGround", new Vector2(width, 2f));
        SetTransform("VillageLeftWall", new Vector3(left - 0.5f, groundY + wallHeight * 0.5f, 0f));
        SetTransform("VillageRightWall", new Vector3(right + 0.5f, groundY + wallHeight * 0.5f, 0f));
        SetBox("VillageLeftWall", new Vector2(2f, wallHeight));
        SetBox("VillageRightWall", new Vector2(2f, wallHeight));
        Camera mainCamera = Camera.main;
        float backgroundX = mainCamera == null ? 0f : mainCamera.transform.position.x;
        SetTransform("BG IMG_Viliage", new Vector3(backgroundX, 8.8f, 0f));
        SetScale("BG IMG_Viliage", Vector3.one);
        SetParallaxSpeed("BG IMG_Viliage", parallaxSpeed);
        SetCameraBounds(new Vector2(centerX, cameraY), new Vector2(width, cameraHeight));
    }

    private static void SetTransform(string name, Vector3 position)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;
        Undo.RecordObject(go.transform, "Apply Village Layout");
        go.transform.position = position;
        EditorUtility.SetDirty(go);
    }

    private static void SetBox(string name, Vector2 size)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;
        BoxCollider2D box = go.GetComponent<BoxCollider2D>();
        if (box == null) return;
        Undo.RecordObject(box, "Apply Village Layout");
        box.size = size;
        EditorUtility.SetDirty(box);
    }

    private static void SetScale(string name, Vector3 scale)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;
        Undo.RecordObject(go.transform, "Apply Village Layout");
        go.transform.localScale = scale;
        EditorUtility.SetDirty(go);
    }

    private static void SetParallaxSpeed(string name, float speed)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;
        Parallax parallax = go.GetComponent<Parallax>();
        if (parallax == null) return;
        SerializedObject serialized = new SerializedObject(parallax);
        serialized.FindProperty("parallaxSpeed").floatValue = speed;
        serialized.ApplyModifiedProperties();
    }

    private static void SetCameraBounds(Vector2 center, Vector2 size)
    {
        GameObject go = GameObject.Find("StageCamBounds");
        if (go == null) return;
        PolygonCollider2D polygon = go.GetComponent<PolygonCollider2D>();
        if (polygon == null) return;

        Undo.RecordObject(go.transform, "Apply Village Layout");
        Undo.RecordObject(polygon, "Apply Village Layout");
        go.transform.position = center;
        polygon.enabled = true;
        polygon.isTrigger = true;
        polygon.points = new[]
        {
            new Vector2(-size.x * 0.5f, -size.y * 0.5f),
            new Vector2(size.x * 0.5f, -size.y * 0.5f),
            new Vector2(size.x * 0.5f, size.y * 0.5f),
            new Vector2(-size.x * 0.5f, size.y * 0.5f)
        };
        EditorUtility.SetDirty(go);
        EditorUtility.SetDirty(polygon);
    }
}
