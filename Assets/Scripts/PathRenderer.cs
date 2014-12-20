using UnityEngine;
using System.Collections;
//needed for List
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]

public class PathRenderer : MonoBehaviour
{
	private List<Vector3> PathStorage;
	private List<Vector3> AlternativePathStorage;

	public AudioClip tap;

	int movingIndex = 0;

	int lineRendererIndex = 0;


	public float startWidth = 0.1f;
	public float endWidth = 0.01f;

	public string[] layerNames;
	bool AlternativeRoadWasCreated = false;

	private LineRenderer _renderer;

	private PersonControl personMovement;

	public Material particle;

	private float offset = 0.1f;

	void Awake ()
	{
			personMovement = this.GetComponent<PersonControl> ();
	}

	void Start ()
	{

			PathStorage = new List<Vector3> ();
			AlternativePathStorage = new List<Vector3>();
			_renderer = GetComponent<LineRenderer> ();

			//_renderer.material = new Material (Shader.Find ("Particles/Additive"));
		_renderer.material = particle;
			_renderer.SetWidth (startWidth, endWidth);
			_renderer.useWorldSpace = true;

			//_renderer.enabled = false;

	}

	void Update ()
	{



	}

	private void playClip (AudioClip clip)
	{
		if (this.audio.clip != clip) {
			this.audio.clip = clip;
		}
		
		if (!this.audio.isPlaying) {
			this.audio.Play ();
		}
	}


	public void CreatePath (Vector3 _NewPathStep)
	{
		personMovement.StopMoving ();

		PathStorage.Clear ();
		//Vector3 pos = new Vector3 (_NewPathStep.x, this.transform.position.y, _NewPathStep.z);
		//PathStorage.Add(pos);
		lineRendererIndex = 0;
		_renderer.SetWidth (startWidth, endWidth);
		playClip (this.tap);

		this._renderer.SetColors (personMovement.myColor, personMovement.myColor);

	}



	public void UpdatePath (Vector3 _NewPathStep)
	{

			if(AlternativeRoadWasCreated){
				
				for(int i = 0; i<AlternativePathStorage.Count-1;i++){
					PathStorage.Add(AlternativePathStorage[i]);
					
				}
				AlternativePathStorage.Clear();
				AlternativeRoadWasCreated = false;
				
			Debug.Log ("linerenderer  : "+lineRendererIndex+"  count :  "+(PathStorage.Count-1));
				_renderer.SetVertexCount(PathStorage.Count);
				lineRendererIndex=0;
				
				for(int i = 0; i<PathStorage.Count-1;i++){
					
				_renderer.SetPosition(i, new Vector3( PathStorage[i].x, offset,PathStorage[i].z ));
					//_renderer.SetPosition(i, PathStorage[i]);
					lineRendererIndex++;
					
				}
				
				
			}
			_renderer.enabled = true;
			Vector3 pos = new Vector3 (_NewPathStep.x, this.transform.position.y, _NewPathStep.z);
			PathStorage.Add (pos);
			_renderer.SetVertexCount (PathStorage.Count);
			Vector3 offsetPos = new Vector3 (PathStorage [lineRendererIndex].x, offset, PathStorage [lineRendererIndex].z);
			_renderer.SetPosition (lineRendererIndex, offsetPos);
			lineRendererIndex++;

			/*for(int i = 0; i<PathStorage.Count; i++){
		_renderer.SetPosition(i, PathStorage[i]);

	}*/

	}

		public void SeekForAlternativePath(Vector3 _NewPathStep){
			if(PathStorage.Count>0){
				AlternativePathStorage.Clear();
				AlternativeRoadWasCreated = true;
				
				int walkableMask = WalkableMaskFromNames();
				
				// Query path from gameObject position to target transform position
				NavMeshPath path = new NavMeshPath();

				//NavMesh.CalculatePath(PathStorage[PathStorage.FindLastIndex], _NewPathStep, walkableMask, path);
				
				
				NavMesh.CalculatePath(PathStorage[PathStorage.Count-1], _NewPathStep, walkableMask, path);
				int pathElements = path.corners.Length;
				// Draw the path
				for(int i=1;i<pathElements;++i)
				{
					
					
					Debug.DrawLine(path.corners[i-1], path.corners[i], Color.green);
					Debug.Log ("GAP : "+path.corners[i-1] +"   "+ path.corners[i]);
					//Debug.Log(path.corners[i-1] +"   "+ path.corners[i]);
					AlternativePathStorage.Add(new Vector3(path.corners[i].x,transform.position.y, path.corners[i].z));
				}
				
				
			}
			
		}



	public void SetPath ()
	{

			_renderer.enabled = true;

			/*_renderer.SetVertexCount(PathStorage.Count);

	for(int i = 0; i<PathStorage.Count; i++){
		_renderer.SetPosition(i, PathStorage[i]);

	}*/


			StartCoroutine ("UpdateMovingWithVanished");

	}




	public void FlicLineRenderer ()
	{
			PathStorage.Reverse ();
			for (int i = 0; i<PathStorage.Count; i++) {
						_renderer.SetPosition(i, new Vector3( PathStorage[i].x, offset,PathStorage[i].z ));
					//_renderer.SetPosition (i, PathStorage [i]);

			}

			_renderer.SetWidth (endWidth, startWidth);
			//PathStorage.Reverse();
	}



	IEnumerator UpdateMovingWithVanished ()
	{
	
			
			FlicLineRenderer ();
	
			while (PathStorage.Count>0) {
					//Debug.Log(PathStorage[0]);
					_renderer.SetVertexCount (PathStorage.Count);
					Vector3 temp = new Vector3 (PathStorage [PathStorage.Count - 1].x, transform.position.y, PathStorage [PathStorage.Count - 1].z);
					transform.position = temp;
					PathStorage.RemoveAt (PathStorage.Count - 1);
					yield return new WaitForFixedUpdate ();
					//yield return new WaitForSeconds(0.1f);
			}

			/*while(PathStorage.Count>0){
		Debug.Log(PathStorage[0]);
		_renderer.SetVertexCount(PathStorage.Count);
		transform.position = PathStorage[0];
		PathStorage.RemoveAt(0);
		//yield return new WaitForFixedUpdate();
		yield return new WaitForSeconds(0.1f);
	}*/
	
	
			//Debug.Log ("Stopped moving");

			personMovement.StartCoroutine (personMovement.RestartMoving ());

			PathStorage.Clear ();
			_renderer.enabled = false;
			_renderer.SetVertexCount (0);

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

	






}