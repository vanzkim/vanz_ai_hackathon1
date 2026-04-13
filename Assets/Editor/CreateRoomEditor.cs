using UnityEngine;
using UnityEditor;

public class CreateRoomEditor
{
    [MenuItem("Tools/Create Room")]
    public static void CreateRoom()
    {
        // 방 크기 설정
        float roomWidth = 10f;
        float roomDepth = 10f;
        float wallHeight = 3f;
        float wallThickness = 0.2f;

        // 부모 오브젝트 생성
        GameObject room = new GameObject("Room");
        Undo.RegisterCreatedObjectUndo(room, "Create Room");

        // 재질 생성
        Material wallMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        wallMat.color = new Color(0.85f, 0.85f, 0.85f);
        AssetDatabase.CreateAsset(wallMat, "Assets/Materials/WallMaterial.mat");

        Material floorMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floorMat.color = new Color(0.6f, 0.5f, 0.4f);
        AssetDatabase.CreateAsset(floorMat, "Assets/Materials/FloorMaterial.mat");

        AssetDatabase.SaveAssets();

        // 바닥
        CreateQuad("Floor", room.transform,
            new Vector3(0, 0, 0),
            new Vector3(roomWidth, wallThickness, roomDepth),
            floorMat);

        // 앞 벽 (North, +Z)
        CreateQuad("Wall_North", room.transform,
            new Vector3(0, wallHeight / 2f, roomDepth / 2f),
            new Vector3(roomWidth, wallHeight, wallThickness),
            wallMat);

        // 뒤 벽 (South, -Z)
        CreateQuad("Wall_South", room.transform,
            new Vector3(0, wallHeight / 2f, -roomDepth / 2f),
            new Vector3(roomWidth, wallHeight, wallThickness),
            wallMat);

        // 오른쪽 벽 (East, +X)
        CreateQuad("Wall_East", room.transform,
            new Vector3(roomWidth / 2f, wallHeight / 2f, 0),
            new Vector3(wallThickness, wallHeight, roomDepth),
            wallMat);

        // 왼쪽 벽 (West, -X)
        CreateQuad("Wall_West", room.transform,
            new Vector3(-roomWidth / 2f, wallHeight / 2f, 0),
            new Vector3(wallThickness, wallHeight, roomDepth),
            wallMat);

        // 씬에서 빨간 구체 찾아서 중앙으로 이동
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null && rend.sharedMaterial != null)
            {
                if (rend.sharedMaterial.color == Color.red || obj.name.ToLower().Contains("sphere"))
                {
                    float sphereRadius = 0.5f;
                    obj.transform.position = new Vector3(0, wallThickness + sphereRadius, 0);
                    Undo.RecordObject(obj.transform, "Move Sphere to Center");
                    Debug.Log($"[CreateRoom] '{obj.name}' 을 방 중앙으로 이동했습니다.");
                    break;
                }
            }
        }

        Selection.activeGameObject = room;
        SceneView.FrameLastActiveSceneView();
        Debug.Log("[CreateRoom] 방 생성 완료!");
    }

    private static GameObject CreateQuad(string name, Transform parent, Vector3 position, Vector3 scale, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = mat;
        Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
        return obj;
    }
}
