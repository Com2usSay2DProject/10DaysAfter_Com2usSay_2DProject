using UnityEngine;

public class TileNode // 수민
{
    private Vector3 offset = new Vector3(); // 오브젝트 올려놨을때 이쁜 위치
    public Vector3 WorldPositon; // 해당 칸의 센터 위치

    public int X, Y;
    public bool IsWalkable;

    public int gCost, hCost;
    public TileNode parent;


    public int fCost => gCost + hCost;

    public TileNode(int x, int y, bool isWalkable, Vector3 worldPosition)
    {
        X = x;
        Y = y;
        IsWalkable = isWalkable;
        WorldPositon = worldPosition + offset;
    }
}