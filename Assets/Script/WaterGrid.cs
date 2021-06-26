using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/** Builds a 3D grid of cubes(3D cells) with default color white.
 *  Author: Jack Navarrette
 */
public class WaterGrid : MonoBehaviour
{
    // Cube prefab
    public GameObject Cell3 = null;
    public GameObject Parent = null;
    //Matrix to store instantiated cells
    public GameObject[,] MatrixofCells;
    // Matrix to store data transferred from GameSimulation.cs
    private Cell[,] dataMatrix;
    //Main Camera
    public Camera camera;

    public Color cInput;

    public Material whiteMaterial;

    public Material blueMaterial;

    public Material blackMaterial;

    private int column;

    private int rank;

    private float aspectRatio;
    private float previousAspectRatio;

    private GameSimulation sim;

    public void setColumn(int c)
    {
        column = c;
    }

    public void setRank(int r)
    {
        rank = r;
    }

    // Start is called before the first frame update
    void Start()
    {
        MatrixofCells = new GameObject[rank, column];
        sim = (GameSimulation) GameObject.Find("Game").GetComponent(typeof(GameSimulation));
        dataMatrix = sim.dataMatrix;
        for (int i = 0; i < rank; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Vector3 Position = new Vector3(i, j, 0);
                MatrixofCells[i, j] = Instantiate(Cell3, Position, Quaternion.identity, Parent.transform);
                if(dataMatrix[i,j].getState() == 1){
                    setCellColorBlue(MatrixofCells[i,j]);
                }
                else if(dataMatrix[i,j].getState() == -1){
                    setCellColorBlack(MatrixofCells[i,j]);
                }
                //setRandomColor(MatrixofCells[i, j]);
                //Debug.Log($"Cell x:{i}, y:{j}");
            }
        }
        updateData();
        //center camera on grid
        Vector3 pos = new Vector3((column - 1) / 2f, (rank - 1) / 2f, -10);
        camera.transform.position = pos;
        aspectRatio = camera.aspect;
        previousAspectRatio = aspectRatio;
        if (camera.orthographic)
        {
            setCameraSize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //dynamically adjust orthographic size of camera as aspect ratio changes
        if (camera.orthographic && camera.aspect != previousAspectRatio)
        {
            aspectRatio = camera.aspect;
            previousAspectRatio = aspectRatio;
            setCameraSize();
        }

        updateData();


    }
    //Method does not work with Universal Render Pipeline in current form
    public void setCellColorWhite(GameObject Cell)
    {
        Cell.GetComponent<Renderer>().material = whiteMaterial;
    }
    //Method does not work with Universal Render Pipeline in current form
    public void setCellColorBlue(GameObject Cell)
    {
        Cell.GetComponent<Renderer>().material = blueMaterial;
    }
    //Method does not work with Universal Render Pipeline in current form
    public void setCellColorBlack(GameObject Cell)
    {
        Cell.GetComponent<Renderer>().material = blackMaterial;
    }
    //Method does not work with Universal Render Pipeline in current form
    public void setRandomColor(GameObject Cell)
    {
        int rnd = Random.Range(0, 2);
        if (rnd == 1)
        {
            setCellColorBlue(Cell);
        }
        else
        {
            setCellColorWhite(Cell);
        }
    }

    void updateData(){
        for (int i = 0; i < rank; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // If the data has changed since the last frame, update the array and the cell colors
                dataMatrix[i, j] = sim.dataMatrix[i,j];
                if(dataMatrix[i,j].getState() == 1){
                    setCellColorBlue(MatrixofCells[i,j]);
                }
                else if(dataMatrix[i,j].getState() == 0){
                    setCellColorWhite(MatrixofCells[i,j]);
                }
                else if(dataMatrix[i,j].getState() == -1){
                    setCellColorBlack(MatrixofCells[i,j]);
                }
            }
        }
    }


    /// <summary>
    /// Computes the orthographic camera size based on
    /// the graphics being displayed
    /// </summary>
    private void setCameraSize()
    {
        //orthographic size as a function of width:
        //width/(2f*camera.aspect)

        //orthographic size as a function of height:
        //height/2f

        //Compute the orthographic size that fits the number of rows and the number of columns
        float rankBased = rank / 2f;
        float columnBased = column / (2f * camera.aspect);

        //use whichever one is bigger
        camera.orthographicSize = Mathf.Max(rankBased, columnBased);
    }
}
