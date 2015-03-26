using UnityEngine;
using System.Collections;

public class Lines : MonoBehaviour
{
	public Material Mat;
	
	public int X;
	public int Z;
	
	void OnPostRender ()
	{
		if (Mat == null)
			return;
		
		GL.PushMatrix();
		Mat.SetPass(0);
		GL.Begin(GL.LINES);
		GL.Color(Color.white);
		for (int x = 0; x < X + 1; x++)
		{
			GL.Vertex(new Vector3((x-X/2)*.2f, 0, (-X/2)*.2f));
			GL.Vertex(new Vector3((x-X/2)*.2f, 0, (X/2)*.2f));		
		}
		for (int z = 0; z < Z + 1; z++)
		{
			GL.Vertex(new Vector3((-Z/2)*.2f, 0, (z-Z/2)*.2f));
			GL.Vertex(new Vector3((Z/2)*.2f, 0, (z-Z/2)*.2f));	
		}
		GL.End();
		GL.PopMatrix();
	}
}