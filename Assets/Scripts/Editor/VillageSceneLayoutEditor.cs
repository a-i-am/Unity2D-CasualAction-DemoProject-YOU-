using UnityEditor;
using UnityEngine;

public static class VillageSceneLayoutEditor
{
    [MenuItem("Tools/Village Layout/Apply Playable Layout")]
    private static void ApplyPlayableLayout()
    {
        SetTransform("Player", new Vector3(-5.28f, 6.69f, -16f), new Vector3(10f, 10f, 1f));
        Camera mainCamera = Camera.main;
        SetTransform("BG IMG_Viliage", new Vector3(mainCamera == null ? 0f : mainCamera.transform.position.x, 8.8f, 0f), Vector3.one);
        SetTransform("VillageWalkableGround", new Vector3(80f, 5f, 0f), Vector3.one);
        SetTransform("Bub house", new Vector3(0f, 12.4f, 0f), new Vector3(12f, 12f, 1f));
        SetTransform("Bub house 2", new Vector3(23f, 12.4f, 0f), new Vector3(12f, 12f, 1f));
        SetTransform("storage house", new Vector3(58f, 12.1f, 0f), new Vector3(12f, 12f, 1f));
        SetTransform("Shop", new Vector3(92f, 12.1f, 0f), new Vector3(12f, 12f, 1f));
        SetTransform("Bub house 2 (1)", new Vector3(132f, 15.7f, 0f), new Vector3(14f, 14f, 1f));
        SetTransform("Witchs House", new Vector3(176f, 18.2f, 0f), new Vector3(12f, 12f, 1f));
    }

    [MenuItem("Tools/Village Layout/Select Key Objects")]
    private static void SelectKeyObjects()
    {
        string[] names =
        {
            "Main Camera",
            "Player",
            "BG IMG_Viliage",
            "VillageWalkableGround",
            "VillageLeftWall",
            "VillageRightWall"
        };

        Selection.objects = System.Array.ConvertAll(names, GameObject.Find);
    }

    private static void SetTransform(string name, Vector3 position, Vector3 scale)
    {
        GameObject go = GameObject.Find(name);
        if (go == null) return;

        Undo.RecordObject(go.transform, "Apply Village Layout");
        go.transform.position = position;
        go.transform.localScale = scale;
        EditorUtility.SetDirty(go);
    }
}
