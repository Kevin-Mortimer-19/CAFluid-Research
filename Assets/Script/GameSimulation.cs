using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSimulation : MonoBehaviour
{
    public Cell[,] dataMatrix = null;

    public GameObject grid = null;

    public int column;

    public int rank;

    // Defines whether movement should be random or predictable
    public bool randomMovement;

    // Defines whether cells can move diagonally, or just in the four cardinal directions
    public bool diagonalMovement;

    private WaterGrid wg;

    private int generation;

    private int timer;

    // Defines whether or not the simulation is currently running.
    public bool running;

    public bool pressure;

    // Start is called before the first frame update
    void Start()
    {
        dataMatrix = new Cell[rank, column];
        wg = grid.GetComponent<WaterGrid>();
        wg.setColumn(column);
        wg.setRank(rank);
        running = false;
        generation = 0;
        // Randomly assigns a value to each cell in the grid; -1 is a solid wall, 0 is an empty cell, 1 is a full cell
        for (int i = 0; i < rank; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // If the cell is on the edge of the grid, it is a wall
                if(i == 0 || j == 0 || i == rank-1 || j == column-1){
                    dataMatrix [i, j] = new Cell(i,j, -1);
                }
                else {
                    dataMatrix [i, j] = new Cell(i,j, 0);
                }
            }
        }
    }

    // Randomly fills all empty squares with water
    void fillRandom(){
        for (int i = 0; i < rank; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if(dataMatrix [i, j].getState() != -1){
                    int rnd = Random.Range(0, 2);
                    dataMatrix [i, j].setState(rnd);
                }
            }
        } 
    }

    // Randomly fills all empty squares with water
    void fillRandom2(){
        for (int i = 0; i < rank; i++)
        {
            for (int j = column/2; j < column; j++)
            {
                if(dataMatrix [i, j].getState() != -1){
                    int rnd = Random.Range(0, 2);
                    dataMatrix [i, j].setState(rnd);
                }
            }
        }
    }

    // Creates an X shape
    void WallPosition1(){
        for (int i = 1; i < rank-1; i++)
        {
            for (int j = 1; j < column-1; j++)
            {
                if(i == j || (rank - i) == j || i+1 == j || (rank - i+1) == j || i-1 == j || (rank - i-1) == j){
                    dataMatrix [i, j].setState(-1);
                }
                else {
                    dataMatrix [i, j].setState(0);
                }
            }
        } 
    }

    // Creates a horizontal line across the middle, with a whole at the center
    void WallPosition2(){
        for (int i = 1; i < rank-1; i++)
        {
            for (int j = 1; j < column-1; j++)
            {
                dataMatrix [i, j].setState(0);
            }
        }
        for (int i = 1; i < (rank/2)-1; i++)
        {
            dataMatrix [i, column/2].setState(-1);
        }
        for (int i = (rank/2)+1; i < rank-1; i++)
        {
            dataMatrix [i, column/2].setState(-1);
        }
    }
    
    // Creates a horizontal line across the middle, with a whole at the center
    void WallPosition3(){
        for (int i = 1; i < rank-1; i++)
        {
            for (int j = 1; j < column-1; j++)
            {
                dataMatrix [i, j].setState(0);
            }
        }
        for (int i = 1; i < (rank/2)-1; i++)
        {
            dataMatrix [i, column/2].setState(-1);
        }
        for (int i = (rank/2)+1; i < rank-1; i++)
        {
            dataMatrix [i, column/2].setState(-1);
        }
        for(int j = (column/2); j > 5; j--){
            dataMatrix [(rank/2)-3, j].setState(-1);
            dataMatrix [(rank/2)+3, j].setState(-1);
        }
        for(int j = 1; j < (column/2)-5; j++){
            dataMatrix [(rank/2)-8, j].setState(-1);
            dataMatrix [(rank/2)+8, j].setState(-1);
        }
    }



    void generate()
    {
        // Looping but skipping the edge cells
        for (int x = 1; x < rank-1; x++) {
            for (int y = 1; y < column-1; y++) {
                // Only update the cells that are currently "filled" with water
                if(dataMatrix[x, y].getState() == 1){

                    // Storage variables for relevant states; d = down, l = left, r = right
                    Cell c = dataMatrix[x, y];
                    int u = dataMatrix[x, y+1].getState();
                    int d = dataMatrix[x, y-1].getState();
                    int dl = dataMatrix[x-1, y-1].getState();
                    int dr = dataMatrix[x+1, y-1].getState();
                    int l = dataMatrix[x-1, y].getState();
                    int r = dataMatrix[x+1, y].getState();
                    
                    // If the cell has already been moved by pressureGenerate(), don't move it back down
                    if(c.getHasMoved()){
                        continue;
                    }
                    // If random movement is enabled, and all three lower cells are empty, the destination is determined randomly
                    else if(diagonalMovement && randomMovement && (dl == 0 && dr == 0 && d == 0) ){
                        int rnd = Random.Range(0,3);
                        if(rnd == 0){
                            moveDownLeft(x, y);
                        }
                        else if (rnd == 1){
                            moveDownRight(x, y);
                        }
                        else {
                            moveDown(x, y);
                        }
                    }
                    // If random movement is enabled, and only two lower cells are open, the destination is determined randomly
                    else if(diagonalMovement && randomMovement && (dl == 0 && d == 0) ){
                        if(Random.Range(0, 2) == 0){
                            moveDownLeft(x, y);
                        }
                        else {
                            moveDown(x, y);
                        }
                    }
                    // If random movement is enabled, and only two lower cells are open, the destination is determined randomly
                    else if(diagonalMovement && randomMovement && (dr == 0 && d == 0) ){
                        if(Random.Range(0, 2) == 0){
                            moveDownRight(x, y);
                        }
                        else {
                            moveDown(x, y);
                        }
                    }
                    // If random movement is enabled, and only two lower cells are open, the destination is determined randomly
                    else if(diagonalMovement && randomMovement && (dl == 0 && dr == 0) ){
                        if(Random.Range(0, 2) == 0){
                            moveDownLeft(x, y);
                        }
                        else {
                            moveDownRight(x, y);
                        }
                    }
                    // If the cell immediately below is empty, the current cell moves down
                    else if(d == 0){
                        moveDown(x, y);
                    }
                    // If the cell diagonally down and left is empty, the current cell moves there (only if diagonalMovement is enabled)
                    else if(dl == 0 && diagonalMovement){
                        moveDownLeft(x, y);
                    }
                    // If the cell diagonally down and right is empty, the current cell moves there (only if diagonalMovement is enabled)
                    else if(dr == 0 && diagonalMovement){
                        moveDownRight(x, y);
                    }
                    // If random movement is enabled, and positions to the left and right are both open, the destination is determined randomly
                    else if(randomMovement && (l == 0 && r == 0) ){
                        if(Random.Range(0, 2) == 0){
                            moveLeft(x, y);
                        }
                        else{
                            moveRight(x, y);
                        }
                    }
                    // If the cell to the left is empty, the current cell moves left
                    else if(l == 0){
                        moveLeft(x, y);
                    }
                    // If the cell to the right is empty, the current cell moves right
                    else if(r == 0){
                        moveRight(x, y);
                    }
                }
            }
        }
        // Increment the generation counter.
        generation++;
    }

    // Alternate generate(), exclusively for cells being pushed upwards by pressure
    void pressureGenerate(){
        for (int x = rank-2; x > 0; x--) {
            for (int y = column-2; y > 0; y--) {
                // Storage variables for relevant states; u = up, l = left, r = right
                Cell c = dataMatrix[x, y];
                int u = dataMatrix[x, y+1].getState();
                int ul = dataMatrix[x-1, y+1].getState();
                int ur = dataMatrix[x+1, y+1].getState();

                // If random movement is enabled, and all three upper cells are empty, the destination is determined randomly
                if(c.isMovingUpward()){
                    if(diagonalMovement && randomMovement && (ul == 0 && ur == 0 && u == 0) ){
                        int rnd = Random.Range(0,3);
                        if(rnd == 0){
                            moveUpLeft(x, y);
                        }
                        else if (rnd == 1){
                            moveUpRight(x, y);
                        }
                        else {
                            moveUp(x, y);
                        }
                    }
                    // If random movement is enabled, and only two upper cells are open, the destination is determined randomly
                    else if(diagonalMovement && randomMovement && (ul == 0 && u == 0) ){
                        if(Random.Range(0, 2) == 0){
                            moveUpLeft(x, y);
                        }
                        else {
                            moveUp(x, y);
                        }
                    }
                    // If random movement is enabled, and only two upper cells are open, the destination is determined randomly
                    else if(diagonalMovement && randomMovement && (ur == 0 && u == 0) ){
                        if(Random.Range(0, 2) == 0){
                            moveUpRight(x, y);
                        }
                        else {
                            moveUp(x, y);
                        }
                    }
                    // If random movement is enabled, and only two upper cells are open, the destination is determined randomly
                    else if(diagonalMovement && randomMovement && (ul == 0 && ur == 0) ){
                        if(Random.Range(0, 2) == 0){
                            moveUpLeft(x, y);
                        }
                        else {
                            moveUpRight(x, y);
                        }
                    } 
                    else if(u == 0){
                        moveUp(x, y);
                        dataMatrix[x, y+1].setHasMoved(true);
                    }
                }
                dataMatrix[x, y].setUpwardMovement(false);
            }
        }
    }

    // Given a set of coordinates, each of the following functions moves the cell at that position in a given direction

    void moveUp(int x, int y){
        flipState(x, y);
        flipState(x, y+1);
    }

    void moveUpLeft(int x, int y){
        flipState(x, y);
        flipState(x-1, y+1);
    }

    void moveUpRight(int x, int y){
        flipState(x, y);
        flipState(x+1, y+1);
    }

    void moveDown(int x, int y){
        flipState(x, y);
        flipState(x, y-1);
    }

    void moveDownLeft(int x, int y){
        flipState(x, y);
        flipState(x-1, y-1);
    }

    void moveDownRight(int x, int y){
        flipState(x, y);
        flipState(x+1, y-1);
    }

    void moveLeft(int x, int y){
        flipState(x, y);
        flipState(x-1, y);
    }

    void moveRight(int x, int y){
        flipState(x, y);
        flipState(x+1, y);
    }

    // FLips the state of a cell at the given coordinates; 0 signals an empty cell, and 1 signals a full cell

    void flipState(int r, int c){
        if(dataMatrix[r,c].getState() == 0){
            dataMatrix[r,c].setState(1);
        }
        else if(dataMatrix[r,c].getState() == 1){
            dataMatrix[r,c].setState(0);
        }
    }

    // Loop through each cell, and if it hasn't been seen yet, calculate the pressure for it and all contiguous cells
    void managePressure(){
        for (int x = 1; x < rank-1; x++) {
            for (int y = 1; y < column-1; y++) {
                if(!dataMatrix[x, y].isDiscovered()){
                    calculatePressure(dataMatrix[x, y]);
                }
            }
        }
    }

    // Reset necessary values after pressure calculations are complete
    void resetPressure(){
        for (int x = 1; x < rank-1; x++) {
            for (int y = 1; y < column-1; y++) {
                if(dataMatrix[x, y].getState() == 1 && dataMatrix[x, y].isDiscovered()){
                    dataMatrix[x, y].setDiscovered(false);
                    dataMatrix[x, y].setHasMoved(false);
                    dataMatrix[x, y].setUpwardMovement(false);
                }
            }
        }
    }


    // Breadth first search function which, when given a root cell, creates a list of all connected cells, then determines which need to move upwards
    void calculatePressure(Cell root){
        List<Cell> cells = new List<Cell>();
        root.setDiscovered(true);
        cells.Add(root);
        Queue<Cell> searchQueue = new Queue<Cell>();
        searchQueue.Enqueue(root);
        int max = root.getY();
        int min = root.getY();
        bool touchingGround = false;

        // Create List of every adjacent water cell
        while(searchQueue.Count != 0){
            Cell current = searchQueue.Dequeue();
            int x = current.getX();
            int y = current.getY();
            if(y > max){
                max = y;
            }
            else if(y < min){
                min = y;
            }
            // Get a reference to each cell adjacent to root
            Cell r = dataMatrix[x+1, y];
            Cell l = dataMatrix[x-1, y];
            Cell u = dataMatrix[x, y+1];
            Cell d = dataMatrix[x, y-1];

            // When an adjacent water cell is found, it is marked as discovered and added to the queue and the list
            if(r.getState() == 1 && !r.isDiscovered()){ 
                searchQueue.Enqueue(r);
                dataMatrix[r.getX(), r.getY()].setDiscovered(true);
                cells.Add(r);
            }
            if(l.getState() == 1 && !l.isDiscovered()){
                searchQueue.Enqueue(l);
                dataMatrix[l.getX(), l.getY()].setDiscovered(true);
                cells.Add(l);
            }
            if(u.getState() == 1 && !u.isDiscovered()){
                searchQueue.Enqueue(u);
                dataMatrix[u.getX(), u.getY()].setDiscovered(true);
                cells.Add(u);
            }
            if(d.getState() == 1 && !d.isDiscovered()){
                searchQueue.Enqueue(d);
                dataMatrix[d.getX(), d.getY()].setDiscovered(true);
                cells.Add(d);
            }
            if(d.getState() == -1){
                touchingGround = true;
            }
        }

        // Creates an area at the top 1/8 of any given region with no upward movement
        int range = max - min;
        int pressure_limit = max - (range/6);
        if(range > 10 && touchingGround){
        List<Cell>.Enumerator e = cells.GetEnumerator();
        // If any cell in list needs to be pushed up and has an open space, move cell up
        while(e.MoveNext()){
            Cell current2 = e.Current;
            int x = current2.getX();
            int y = current2.getY();

            if(current2.getY() < pressure_limit){
                dataMatrix[x, y].setUpwardMovement(true);
            }
        }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space")){
            if(running){
                running = false;
            }
            else {
                running = true;
            }
        }

        if(Input.GetKeyDown("r")){
            fillRandom();
        }

        if(Input.GetKeyDown("t")){
            fillRandom2();
        }
        
        if(Input.GetKeyDown("1")){
            WallPosition1();
        }
        
        if(Input.GetKeyDown("2")){
            WallPosition2();
        }
        
        if(Input.GetKeyDown("3")){
            WallPosition3();
        }


        if(running){
            timer++;
            if(timer == 1){
                generate();
            }
            else if(timer == 2){
                if(pressure){
                    resetPressure();
                    managePressure();
                    pressureGenerate();
                }
                timer = 0;
            }
        }
    }
}