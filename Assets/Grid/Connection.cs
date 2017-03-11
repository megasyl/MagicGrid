using UnityEngine;
using System.Collections;

public class Connection {

	public Vector3 start;
	public Vector3 end;


	public bool open; //walkable or not
	public bool valid = true; //if it respect grid restriction

	public Connection(Vector3 _start,Vector3 _end,bool _open)
	{
		start = _start;
		end = _end;
		open = _open;

	}
}

public class Line {
	public Vector3 start;
	public Vector3 end;
	public Color color;

	public Line(Vector3 _start,Vector3 _end,Color _color)
	{
		start = _start;
		end = _end;
		color = _color;

	}
}