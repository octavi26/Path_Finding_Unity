using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathTracking : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 move = new Vector2 (0f, 0f);
    public float moveSpeed = 6;

    public int index = 0;
    public int next = 1;

    public GameObject Grid;
    private float gridSizeX;
    private float gridSizeY;
    private int cellNumberX;
    private int cellNumberY;
    private float cellSizeX;
    private float cellSizeY;

    void Start(){
        rb = GetComponent<Rigidbody2D>();

        Grid = GameObject.Find("Grid");
        gridSizeX = Grid.GetComponent<Grid>().gridSizeX;
        gridSizeY = Grid.GetComponent<Grid>().gridSizeY;
        cellNumberX = Grid.GetComponent<Grid>().cellNumberX;
        cellNumberY = Grid.GetComponent<Grid>().cellNumberY;
        cellSizeX = gridSizeX / cellNumberX;
        cellSizeY = gridSizeY / cellNumberY;
    }

    public Vector2 Grid2Pos(int x, int y){
        Vector2 position = new Vector2 (x * cellSizeX + cellSizeX / 2 - 12f, y * cellSizeY + cellSizeY / 2 - 5f);
        return position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = Grid2Pos( Player.enemiesPath[index, next].x, Player.enemiesPath[index, next].y );
        move = target - transform.position;
        rb.velocity = move * moveSpeed;
    }
}
