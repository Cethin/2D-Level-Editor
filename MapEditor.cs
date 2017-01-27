using UnityEngine;
using System.Collections;
using UnityEditor;

[InitializeOnLoad][ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
    #region Static Fields
    private static bool on = false;
    private static GameObject tile;
    private static int layer = 0;
    private static Transform tiles;

    public static bool On
    {
        get{ return on; }
        set{ on = value; }
    }

    public static GameObject Tile
    {
        get{ return tile; }
        set{ tile = value; }
    }

    public static int Layer
    {
        get{ return layer; }
        set{ layer = value; }
    }

    public static Transform Tiles
    {
        get{ return tiles; }
        set{ tiles = value; }
    }
    #endregion

    public static bool toggle() { return (on = !on); }

    void Update()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void setupTilesParent()
    {
        if (Tiles == null)
        {
            GameObject go;
            go = GameObject.FindGameObjectWithTag("Tiles Parent");
            if (go == null)
                go = new GameObject("Tiles");
            Tiles = go.transform;
            Tiles.tag = "Tiles Parent";
        }
    }

    static void OnSceneGUI(SceneView sv)
    {
        if(MapEditor.On)
        {
            if(Event.current.button == 0 && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
            {
                Vector3 mouse = Event.current.mousePosition;
                mouse.y = -mouse.y;
                Vector2 pos = Camera.current.ScreenToWorldPoint(mouse);
                createTileAt(mousePosCorrection(pos));
                Event.current.Use();
            }
        }
    }

    static Vector2 mousePosCorrection(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x);
        pos.y = Mathf.Floor(pos.y + Camera.current.orthographicSize * 2);
        return pos;
    }

    public static void createTileAt(Vector2 pos)
    {
        if(!World.Ready())
        {
            Debug.LogWarning("World not initialized! Please set a width, height, and depth in the inspector and click \"Set\".");
            return;
        }

        if(World.posInBounds(pos, Layer) && (Tile == null || !World.posIsThis(pos, Layer, Tile)))
        {
            if(Tiles == null)
                GameObject.FindObjectOfType<MapEditor>().setupTilesParent();

            if(Tile != null)
            {
                GameObject go = (GameObject)GameObject.Instantiate(Tile, new Vector3(pos.x, pos.y, Layer), Quaternion.identity);
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if(sr != null)
                {
                    sr.sortingOrder = layer * -World.SORTING_ORDER_MULT;
                }
                go.transform.parent = Tiles;
                World.set(pos, layer, go);
            }
            else
            {
                World.set(pos, layer, null);
            }
        }
    }
}

[CustomEditor(typeof(MapEditor))]
public class MapEditorEditor : Editor
{
    // Dimensions
    int x = (World.Width != 0)  ? World.Width   : 50;
    int y = (World.Height != 0) ? World.Height  : 50;
    int z = (World.Depth != 0)  ? World.Depth   : 3;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // World size fields and setup button
        x = EditorGUILayout.IntSlider("Width", x, 1, 500);
        y = EditorGUILayout.IntSlider("Height", y, 1, 500);
        z = EditorGUILayout.IntSlider("Depth", z, 1, 500);

        if(GUILayout.Button("Set"))
            World.setup(x, y, z);

        // Tile field
        MapEditor.Tile = (GameObject) EditorGUILayout.ObjectField("Tile", MapEditor.Tile, typeof(GameObject), false);
        // Layer field
        MapEditor.Layer = EditorGUILayout.IntSlider("Layer", MapEditor.Layer, 0, World.Depth-1);

        // Toggle on field
        MapEditor.On = GUILayout.Toggle(MapEditor.On, "ON");
    }
}
