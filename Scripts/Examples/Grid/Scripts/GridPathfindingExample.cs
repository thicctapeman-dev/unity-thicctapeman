using UnityEngine;
using ThiccTapeman.Grid;
using ThiccTapeman.Utils;
using ThiccTapeman.Grid.Pathfinding;
using System.Collections.Generic;

public class GridPathfindingExample : MonoBehaviour
{
    ThiccTapeman.Grid.Grid grid;

    // -- Publics -- //

    public GameObject gameObject;
    public Material occupiedMaterial;
    public Material pathHighlightedMaterial;

    // Grid variables
    public int gridSizeX = 20;
    public int gridSizeY = 20;
    public Vector3 position;
    public Vector3 rotation;
    public float tileSizeX;
    public float tileSizeY;

    // Debug stuff
    public Transform projectionDebugObject;
    public bool projectionDebug = false;

    public bool Debug;

    // -- Privates -- //

    // Start is called before the first frame update
    private void Awake()
    {
        // This will create our initial grid and place a cube inside each
        grid = new ThiccTapeman.Grid.Grid(gridSizeX, gridSizeY, tileSizeX, tileSizeY, position, rotation, (grid, x, y) =>
        {
            GameObject obj = Instantiate(gameObject);

            // Just naming and moving the object accordingly
            obj.name = $"grid_object ({x},{y})";
            obj.transform.SetParent(transform);
            obj.transform.position = grid.GridToWorldPosition(x, y, out Vector3 rotation);
            obj.transform.rotation = Quaternion.Euler(rotation);

            bool occupied = false;
            if(Random.Range(0, 1) == 1) occupied = true;

            if(occupied) obj.transform.GetComponent<Renderer>().material = occupiedMaterial;

            return new GridObject(grid, x, y, obj.transform, occupied);
        }, Debug);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = MouseUtils.GetMouseWorldPosition();
            grid.WorldToGridPosition(mousePosition, out int x, out int y);
            List<GridPathfindingObject> path = GridPathfinding.AStar(grid, 0, 0, x, y, (grid, x, y) => {
                GridObject obj = (GridObject)grid.GetGridObject(x, y);
                return !obj.occupied;
            });

            if(path != null)
            {
                foreach(GridPathfindingObject obj in path)
                {
                    GridObject gridObject = (GridObject)obj;
                    gridObject.transform.GetComponent<Renderer>().material = pathHighlightedMaterial;
                }
            }
        }
    }

    private void OnValidate()
    {
        UpdateGrid();
    }

    private void UpdateGrid()
    {

        if (grid == null) return;

        // Will move the grid, set the new tilesize, and rotate it
        grid.SetGridPosition(position);
        grid.SetGridRotation(rotation);
        grid.SetGridTileSize(tileSizeX, tileSizeY);
    }

    // Creates a new class implementing the gridobject class
    public class GridObject : GridObjectAbstract, GridPathfindingObject
    {
        public Transform transform;
        public bool occupied;

        // Creates the base of the grid object, just assigning the transform and the base variables
        public GridObject(ThiccTapeman.Grid.Grid grid, int x, int y, Transform transform, bool occupied) : base(grid, x, y)
        {
            this.transform = transform;
            this.occupied = occupied;

            xPos = x;
            yPos = y;
        }

        // Will just move the object, this will need to be implemented by you.
        public override void Update()
        {
            
        }

        // This is used when debug is turned on, it will be displayed as worldtext's at the grids cordinates
        public override string ToString()
        {
            return $"({x},{y})";
        }

        public override void RelocatePositions()
        {
            if (transform == null) return;

            transform.position = grid.GridToWorldPosition(x, y, out Vector3 rotation);
            transform.rotation = Quaternion.Euler(rotation);
        }

        // This is needed for the pathfinding to work
        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; set; }
        public GridPathfindingObject cameFromNode { get; set; }
        public int xPos { get; set; }
        public int yPos { get; set; }

        // Any other function can be added down below and you can access it from inside the gridObject that is returned from the grid
        // . . .
    }
}
