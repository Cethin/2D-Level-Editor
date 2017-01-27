using UnityEngine;
using UnityEditor;
using System.Collections;

public static class World
{
    private static GameObject[][][] map;
    private static int width, height, depth;
    public const int SORTING_ORDER_MULT = 10;

    public static int Width
    {
        get { return width; }
    }

    public static int Height
    {
        get { return height; }
    }

    public static int Depth
    {
        get { return depth; }
    }

    public static bool Ready()
    {
        return map != null;
    }

    public static void setup(int w, int h, int d)
    {
        width = w;
        height = h;
        depth = d;

        instantiateMap();
        setupWithGOs();
    }

    private static void instantiateMap()
    {
        map = new GameObject[width][][];
        for(int i = 0; i < width; i++)
        {
            map[i] = new GameObject[height][];
            for(int j = 0; j < height; j++)
            {
                map[i][j] = new GameObject[depth];
            }
        }
    }

    public static void giveMap(GameObject[][][] newMap) { map = newMap; }

    public static bool set(Vector3 pos, GameObject go) { return set(pos.x, pos.y, pos.z, go); }

    public static bool set(Vector2 pos, float z, GameObject go) { return set(pos.x, pos.y, z, go); }

    public static bool set(Vector2 pos, int z, GameObject go) { return set((int)pos.x, (int)pos.y, z, go); }

    public static bool set(float x, float y, float z, GameObject go) { return set((int)x, (int)y, (int)z, go); }

    public static bool set(int x, int y, int z, GameObject go)
    {
        if(!posInBounds(x, y, z))
            return false;
        GameObject.DestroyImmediate(map[x][y][z]);

        map[x][y][z] = go;
        return true;
    }

    public static bool posIsEmpty(Vector3 pos) { return posIsEmpty(pos.x, pos.y, pos.z); }

    public static bool posIsEmpty(Vector2 pos, float z) { return posIsEmpty(pos.x, pos.y, z); }

    public static bool posIsEmpty(Vector2 pos, int z) { return posIsEmpty((int)pos.x, (int)pos.y, z); }

    public static bool posIsEmpty(float x, float y, float z){ return posIsEmpty((int)x, (int)y, (int)z); }

    public static bool posIsEmpty(int x, int y, int z)
    {
        if(!posInBounds(x, y, z))
            return false;
        return map[x][y][z] == null;
    }

    public static bool posIsThis(Vector3 pos, GameObject go) { return posIsThis(pos.x, pos.y, pos.z, go); }

    public static bool posIsThis(Vector2 pos, float z, GameObject go) { return posIsThis(pos.x, pos.y, z, go); }

    public static bool posIsThis(Vector2 pos, int z, GameObject go) { return posIsThis((int)pos.x, (int)pos.y, z, go); }

    public static bool posIsThis(float x, float y, float z, GameObject go) { return posIsThis((int)x, (int)y, (int)z, go); }

    public static bool posIsThis(int x, int y, int z, GameObject go)
    {
        if(!posInBounds(x, y, z))
            return false;
        return PrefabUtility.GetPrefabObject(map[x][y][z]) == PrefabUtility.GetPrefabObject(go);
    }
    public static bool posInBounds(Vector3 pos) { return posInBounds(pos.x, pos.y, pos.z); }

    public static bool posInBounds(Vector2 pos, float z) { return posInBounds(pos.x, pos.y, z); }

    public static bool posInBounds(Vector2 pos, int z) { return posInBounds((int)pos.x, (int)pos.y, z); }

    public static bool posInBounds(float x, float y, float z) { return posInBounds((int)x, (int)y, (int)z); }

    public static bool posInBounds(int x, int y, int z)
    {
        if(x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth)
        {
            Debug.LogWarning("Pos out of bounds!");
            return false;
        }
        return true;
    }



    public static GameObject get(Vector3 pos) { return get(pos.x, pos.y, pos.z); }

    public static GameObject get(Vector2 pos, float z) { return get(pos.x, pos.y, z); }

    public static GameObject get(Vector2 pos, int z) { return get((int)pos.x, (int)pos.y, z); }

    public static GameObject get(float x, float y, float z) { return get((int)x, (int)y, (int)z); }

    public static GameObject get(int x, int y, int z)
    {
        if(!posInBounds(x, y, z))
        {
            return null;
        }
        return map[x][y][z];
    }

    private static void setupWithGOs()
    {
        GameObject tileParent = GameObject.FindGameObjectWithTag("Tiles Parent");
        if(tileParent != null)
        {
            for(int i = 0; i < tileParent.transform.childCount; i++)
            {
                GameObject go = tileParent.transform.GetChild(i).gameObject;
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if(sr != null)
                {
                    if(!set(go.transform.position.x, go.transform.position.y, sr.sortingOrder / SORTING_ORDER_MULT, go))
                        GameObject.DestroyImmediate(go);
                }
            }
        }
    }
}
