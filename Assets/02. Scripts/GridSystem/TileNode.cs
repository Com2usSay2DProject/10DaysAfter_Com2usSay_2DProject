using UnityEngine;

public class TileNode // 수민
{
    private Vector3 offset = new Vector3(); // 오브젝트 올려놨을때 이쁜 위치
    public Vector3 WorldPositon; // 해당 칸의 센터 위치

    public int x, y;
    public bool isWalkable;

    public int gCost, hCost;
    public TileNode parent;


    public int fCost => gCost + hCost;

    public TileNode(int x, int y, bool isWalkable, Vector3 worldPosition)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.WorldPositon = worldPosition + offset;
    }
}