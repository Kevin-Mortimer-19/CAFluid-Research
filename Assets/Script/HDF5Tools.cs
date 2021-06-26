using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HDF.PInvoke;

public static class HDF5Tools
{

    /// <summary>
    /// Writes a 1D integer dataset to and HDF5 file. Must include the 2D height, width, and iteration of the simulation
    /// </summary>
    /// <param name="data">1D interger dataset to write to HDF5 file</param>
    /// <param name="height">Number of rows in the 2D data</param>
    /// <param name="width">Number of columns in the 2D data</param>
    /// <param name="iteration">Iteration of the simulation to be written</param>
    /// <param name="fileName">File name, stored directly in Assets</param>
    public unsafe static void writeToFile(int[] data, int height, int width, int iteration, string fileName)
    {

        fixed (int* pointer = data)
        {
            //setup
            System.IntPtr dataPointer = new System.IntPtr((void*)pointer);
            int status;
            string filePath = "Assets/" + fileName;
            long fileID = H5F.create(filePath, H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT);
            ulong[] dimsf = { (ulong)data.Length };
            long dataspace = H5S.create_simple(1, dimsf, null);


            writeSingleDataSet(fileID, dataspace, fileName, height, width, iteration, dataPointer);


            status = H5S.close(dataspace);
            if (status != 0)
            {
                Debug.LogError("HDF5 Dataspace Close Failed - Status: " + status);
            }

            status = H5F.close(fileID);
            if (status != 0)
            {
                Debug.LogError("HDF5 File Close: " + fileName + " Failed. - Status: " + status);
            }

        }
    }

    /// <summary>
    /// Writes 2D integer data to HDF5 file. Must include the iteration being written.
    /// This method converts the data to 1D and calls the first overload.
    /// </summary>
    /// <param name="data2D">2D array of integer data</param>
    /// <param name="iteration">Iterations of the simulation to be written</param>
    /// <param name="fileName">File name, stored directly in Assets</param>
    public static void writeToFile(int[][] data2D, int iteration, string fileName)
    {
        int height = data2D.Length;
        int width = data2D[0].Length;
        Debug.Log("Data Height: " + height + "\nData Width: " + width);
        //convert 2D data to 1D data
        int[] data1D = new int[height * width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                data1D[j + i * width] = data2D[i][j];
            }
        }

        writeToFile(data1D, height, width, iteration, fileName);

    }

    /// <summary>
    /// Writes a single data set to the specified dataspace
    /// </summary>
    /// <param name="fileID">HDF5 file identifier</param>
    /// <param name="dataspace">HDF5 dataspace identifier</param>
    /// <param name="fileName">File name, stored directly in Assets</param>
    /// <param name="height">Number of rows in the 2D data</param>
    /// <param name="width">Number of columns in the 2D data</param>
    /// <param name="iteration">Iteration of the simulation to be written</param>
    /// <param name="dataPointer">System.IntPtr to the 1D data array</param>
    private unsafe static void writeSingleDataSet(long fileID, long dataspace, string fileName, int height, int width, int iteration, System.IntPtr dataPointer)
    {
        int status = 0;
        long dataset = H5D.create(fileID, "Fluid Simulation", H5T.NATIVE_INT, dataspace, H5P.DEFAULT, H5P.DEFAULT, H5P.DEFAULT);

        //writing

        //write 1D dataset
        status = H5D.write(dataset, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, dataPointer);
        if (status != 0)
        {
            Debug.LogError("HDF5 Write to " + fileName + " Failed - Status: " + status);
        }


        //write the three attributes
        writeIntAttribute(dataset, height, "Rows");
        writeIntAttribute(dataset, width, "Cols");
        writeIntAttribute(dataset, iteration, "Iteration");


        //close all

        status = H5D.close(dataset);
        if (status != 0)
        {
            Debug.LogError("HDF5 Dataset Close Failed - Status: " + status);
        }
    }

    /// <summary>
    /// Writes single integer attribute to the specified dataset
    /// </summary>
    /// <param name="dataset">HDF5 dataset identifier</param>
    /// <param name="data">Integer data to write to attribute</param>
    /// <param name="name">Name of the attribute</param>
    private unsafe static void writeIntAttribute(long dataset, int data, string name)
    {
        int status = 0;
        System.IntPtr dataPointer = new System.IntPtr(&data);
        /*
        * Create scalar attribute.
        */
        long aID = H5S.create(H5S.class_t.SCALAR);
        long attr = H5A.create(dataset, name, H5T.NATIVE_INT, aID, H5P.DEFAULT, H5P.DEFAULT);

        /*
        * Write scalar attribute.
        */
        status = H5A.write(attr, H5T.NATIVE_INT, dataPointer);
        if (status != 0)
        {
            Debug.LogError("HDF5 Write Attribute: " + name + " Failed. - Status: " + status);
        }

        /*
        * Close attribute dataspace.
        */
        status = H5S.close(aID);
        if (status != 0)
        {
            Debug.LogError("HDF5 Close Attribute Dataspace: " + name + " Failed. - Status: " + status);
        }

        /*
        * Close attribute.
        */
        status = H5A.close(attr);
        if (status != 0)
        {
            Debug.LogError("HDF5 Close Attribute: " + name + " Failed. - Status: " + status);
        }
    }
}
