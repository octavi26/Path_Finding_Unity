using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the character

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public Material dotMaterial;

    public GameObject Grid;
    private float gridSizeX;
    private float gridSizeY;
    private int cellNumberX;
    private int cellNumberY;
    private float cellSizeX;
    private float cellSizeY;

    public struct GridPosition{
        public int x, y;
    }
    
    private Queue<GridPosition> Q = new Queue<GridPosition>();
    private int[] dy = {-1, 0, 0, 1, -1, -1, 1, 1};
    private int[] dx = {0, -1, 1, 0, -1, 1, -1, 1};
    public int[,] D = new int[100, 100];
    static public int[,] A = new int[100, 100];
    GridPosition lastPos, currentPos;
    bool trigger = false;

    public int numebrEnemies = 0;
    public GameObject[] enemies;
    static public GridPosition[,] enemiesPath = new GridPosition[100, 100];

    public float dotSize = 0.1f;
    public Color dotColor = Color.red;

    public GridPosition Pos2Grid( Vector2 pos ){
        GridPosition grid;
        pos.x += gridSizeX/2;
        pos.y += gridSizeY/2;
        grid.x = (int)(pos.x * cellNumberX / gridSizeX);
        grid.y = (int)(pos.y * cellNumberY / gridSizeY);
        return grid;
    }

    public Vector2 Grid2Pos(int x, int y){
        Vector2 position = new Vector2 (x * cellSizeX + cellSizeX / 2 - 12f, y * cellSizeY + cellSizeY / 2 - 5f);
        return position;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gridSizeX = Grid.GetComponent<Grid>().gridSizeX;
        gridSizeY = Grid.GetComponent<Grid>().gridSizeY;
        cellNumberX = Grid.GetComponent<Grid>().cellNumberX;
        cellNumberY = Grid.GetComponent<Grid>().cellNumberY;
        cellSizeX = gridSizeX / cellNumberX;
        cellSizeY = gridSizeY / cellNumberY;

        EnemyStart();
    }

    void EnemyStart(){
        for( int i=0; i<numebrEnemies; i++ )
            enemies[i].GetComponent<EnemyPathTracking>().index = i;
    }

    void Move(){
        // Get input from the player
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            // Create a movement vector
            moveInput = new Vector2(moveX, moveY).normalized;
    }
    
    bool InM(int x, int y){
        return !(x < 0 || x >= cellNumberX || y < 0 || y > cellNumberY);
    }

    void Lee(){
        for( int y = 0; y < cellNumberY; y++ )
            for( int x = 0; x < cellNumberX; x++ )
                D[x, y] = (int)2e9;

        Q.Enqueue(currentPos);
        D[currentPos.x, currentPos.y] = 0;
        while( Q.Count != 0 ){
            GridPosition current = Q.Dequeue();
            for( int k=0; k<8; k++ ){
                GridPosition next; next.x = current.x + dx[k]; next.y = current.y + dy[k];
                if( InM(next.x, next.y) && A[next.x, next.y] == 0 && D[current.x, current.y] + 1 < D[next.x, next.y] ){
                    D[next.x, next.y] = D[current.x, current.y] + 1;
                    Q.Enqueue(next);
                }
            }
        }
    }

    void RoadReconstruction(){
        for( int i=0; i<numebrEnemies; i++ ){
            if( D[Pos2Grid(enemies[i].transform.position).x, Pos2Grid(enemies[i].transform.position).y] == (int)2e9 ) continue;
            int nrCell = 0;
            GridPosition current = Pos2Grid(enemies[i].transform.position);
            enemiesPath[i, nrCell] = current;
            while( current.x != Pos2Grid(transform.position).x || current.y != Pos2Grid(transform.position).y ){
                for( int k=0; k < 8; k++ ){
                    GridPosition next; next.x = current.x + dx[k]; next.y = current.y + dy[k];
                    if( InM(next.x, next.y) && D[next.x, next.y] == D[current.x, current.y] - 1 ){
                        current = next;
                        break;
                    }
                }
                enemiesPath[i, ++nrCell] = current;
            }
            enemiesPath[i, ++nrCell] = Pos2Grid(transform.position);
        }
    }

    public void DrawLine(Vector3 startPoint, Vector3 endPoint, Color lineColor, float duration)
    {
        if( startPoint == endPoint ) return;
        GameObject lineObject = new GameObject("LineRendererObject");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        Destroy(lineObject, duration);
    }

    void DrawRoad(){
        for( int i=0; i<numebrEnemies; i++ ){
            if( D[Pos2Grid(enemies[i].transform.position).x, Pos2Grid(enemies[i].transform.position).y] == (int)2e9 ) continue;
            for( int j=1; j<=D[Pos2Grid(enemies[i].transform.position).x, Pos2Grid(enemies[i].transform.position).y]; j++ )
                DrawLine(
                    Grid2Pos(enemiesPath[i, j - 1].x, enemiesPath[i, j - 1].y),
                    Grid2Pos(enemiesPath[i, j].x,     enemiesPath[i, j].y),
                    Color.red,
                    0.02f
                );
        }
    }

    void Update()
    {
        Move();

        // Chec if the grid cell we are in has changed
        currentPos = Pos2Grid(transform.position);
        if( currentPos.x != lastPos.x || currentPos.y != lastPos.y ) trigger = true;
        else trigger = false;
        lastPos = currentPos;
    }

    void FixedUpdate()
    {
        Lee();
        RoadReconstruction();
        DrawRoad();

        // Move the character by setting the velocity
        rb.velocity = moveInput * moveSpeed;
    }
}
