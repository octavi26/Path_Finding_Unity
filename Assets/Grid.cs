using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public float gridSizeX = 24f; // Number of cells in the X axis
    public float gridSizeY = 10f; // Number of cells in the Y axis
    public int cellNumberX = 12; // X axis number of cells
    public int cellNumberY = 5; // Y axis number of cells
    public float cellSizeX = 2;
    public float cellSizeY = 2;
    public Material lineMaterial; // Material for the lines
    static public GameObject[,] cellGameObject = new GameObject[100, 100];

    void Start()
    {
        cellSizeX = gridSizeX / cellNumberX;
        cellSizeY = gridSizeY / cellNumberY;
        //DrawGrid();
        CreateGrid();
        transform.position = new Vector3 (-12f, -5f, 0f);
    }

    void Update(){
        CreateObstacle();
    }

    Vector2 Grid2Pos(int x, int y){
        Vector2 position = new Vector2 (x * cellSizeX + cellSizeX / 2 - 12f, y * cellSizeY + cellSizeY / 2 - 5f);
        return position;
    }

    void DrawGrid()
    {
        for( int x = 0; x <= cellNumberX; x++ ){
            CreateLine( 
                new Vector3(x * cellSizeX,        0f, 0f),
                new Vector3(x * cellSizeX, gridSizeY, 0f)
            );
        }

        for( int y = 0; y <= cellNumberY; y++ ){
            CreateLine(
                new Vector3(0f,        y * cellSizeY, 0f),
                new Vector3(gridSizeX, y * cellSizeY, 0f)
            );
        }
    }

    void CreateGrid(){
        for( int x = 0; x < cellNumberX; x++ )
            for( int y = 0; y < cellNumberY; y++ ){
                cellGameObject[x, y] = new GameObject("GridCell");
                cellGameObject[x, y].transform.position = new Vector3 (
                    x * cellSizeX + cellSizeX / 2,
                    y * cellSizeY + cellSizeY / 2,
                    0f
                );
                cellGameObject[x, y].transform.localScale = new Vector3 (
                    cellSizeX,
                    cellSizeY,
                    1f
                );
                cellGameObject[x, y].transform.parent = transform;
                cellGameObject[x, y].AddComponent<BoxCollider2D>();
            }
    }

    void CreateObstacle(){
        for( int x = 0; x < cellNumberX; x++ )
            for( int y = 0; y < cellNumberY; y++ ){
                Vector2 boundsMin = Grid2Pos(x, y) - new Vector2 (cellSizeX / 2, cellSizeY / 2);
                Vector2 boundsMax = Grid2Pos(x, y) + new Vector2 (cellSizeX / 2, cellSizeY / 2);
                Collider2D[] results = new Collider2D[10];
                int colliderCount = Physics2D.OverlapAreaNonAlloc(boundsMin, boundsMax, results);
                bool isTouchingTarget = false;
                for (int i = 0; i < colliderCount; i++)
                {
                    if (results[i].CompareTag("Obstacle"))
                    {
                        isTouchingTarget = true;
                        break;
                    }
                }
                if( isTouchingTarget ) Player.A[x, y] = 1;
                else Player.A[x, y] = 0;
                if( isTouchingTarget ) Debug.Log(x + " " + y + " (" + boundsMin.x + ", " + boundsMin.y + ") (" + boundsMax.x + ", " + boundsMax.y + ")");
            }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("GridLine");
        line.transform.parent = transform;
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = lineMaterial;
        lr.positionCount = 2;
        lr.useWorldSpace = false;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
