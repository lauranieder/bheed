using UnityEngine;

public class NavMeshCalculatePath : MonoBehaviour
{
	public Transform target;
	public string[] layerNames;

	private NavMeshAgent agent;

	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
	}
	
	private int WalkableMaskFromNames()
	{
		if(layerNames.Length == 0)
			return -1; // All layers if no names are specified
		
		int navMeshLayerMask = 0;
		foreach(string layerName in layerNames)
		{
			int layer = NavMesh.GetNavMeshLayerFromName(layerName);
			if(layer >= 0)
				navMeshLayerMask |= (1<<layer);
		}
		return navMeshLayerMask;
	}
	
	void Update()
	{

		if(target==null)
			return;
		
		int walkableMask = WalkableMaskFromNames();
		
		// Query path from gameObject position to target transform position
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(transform.position, target.position, walkableMask, path);
		int pathElements = path.corners.Length;
		
		// Draw the path
		for(int i=1;i<pathElements;++i)
		{
			Debug.DrawLine(path.corners[i-1], path.corners[i], Color.green);
			//Debug.Log ("GAP");
			//Debug.Log(path.corners[i-1] +"   "+ path.corners[i]);
		}

		agent.SetDestination(target.position);
	}



}