using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cell Object for storing Data
public class Cell
{
    private int x;

	private int y;

	private int state;

	private bool discovered;

	private bool upwardMovement;

	private bool hasMoved;

	public Cell(int x, int y, int state){
		this.x = x;
		this.y = y;
		this.state = state;
		discovered = false;
		upwardMovement = false;
		hasMoved = false;
	}

	public int getX(){
		return x;
	}

	public int getY(){
		return y;
	}

	public int getState(){
		return state;
	}

	public void setState(int newStat){
		state = newStat;
	}

	public bool isDiscovered(){
		return discovered;
	}

	public void setDiscovered(bool d){
		discovered = d;
	}
	
	public bool isMovingUpward(){
		return upwardMovement;
	}

	public void setUpwardMovement(bool um){
		upwardMovement = um;
	}

	public bool getHasMoved(){
		return hasMoved;
	}

	public void setHasMoved(bool hm){
		hasMoved = hm;
	}
}
