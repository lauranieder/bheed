using UnityEngine;
using System.Collections;
//needed for List
using System.Collections.Generic;

public class GestureManager : MonoBehaviour
{


		GameObject touchedGameObject;

		public LayerMask floor;
		public LayerMask people;

		public int rayLength = 100;

		bool Dragging = false;

		// Use this for initialization
		void Start ()
		{

		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}


		//Drag à 1 doigt + Drag d'objet
		void FingerGestures_OnDragBegin (Vector2 fingerPos, Vector2 startPos)
		{

				

				//RAYCAST
				Ray ray = Camera.main.ScreenPointToRay(fingerPos);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, rayLength, people)) {
					
						//Debug.DrawRay(ray.origin, ray.direction*30, Color.green);
						touchedGameObject = hit.collider.gameObject;//on récupère quel object a été touché
						if (touchedGameObject.tag == "People") {
						Debug.Log ("dragging people");
								Dragging = true;
								float dist = Vector3.Distance (hit.point, transform.position);
								Vector3 temp = camera.ScreenToWorldPoint (new Vector3 (fingerPos.x, fingerPos.y, dist - 0.1f));
								touchedGameObject.SendMessage ("CreatePath", temp, SendMessageOptions.RequireReceiver);
						}
				}
		}
		void FingerGestures_OnDragMove( Vector2 fingerPos, Vector2 delta ){
			if(Dragging){
				Ray ray = Camera.main.ScreenPointToRay(fingerPos);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, rayLength, floor)) {
					GameObject temporary = hit.collider.gameObject;
					if(temporary.tag == "Floor"){
						float dist = Vector3.Distance(hit.point, transform.position); 
						Vector3 temp = camera.ScreenToWorldPoint(new Vector3(fingerPos.x,fingerPos.y, dist-0.1f));
						//Debug.Log ("POSITION :   "+temp.y);
						//PathStorage.Add(temp);
						touchedGameObject.SendMessage("UpdatePath",temp, SendMessageOptions.RequireReceiver);
						Debug.Log ("updatePath");
					}else{
						float dist = Vector3.Distance(hit.point, transform.position); 
						Vector3 temp = camera.ScreenToWorldPoint(new Vector3(fingerPos.x,fingerPos.y, dist-0.1f));
						touchedGameObject.SendMessage ("SeekForAlternativePath",temp, SendMessageOptions.DontRequireReceiver);
					Debug.Log ("Missing point : "+temporary);
						
						
						
					}
					
				}		
			}else{
				
				//RAYCAST
				Ray ray = Camera.main.ScreenPointToRay(fingerPos);
				RaycastHit hit;
				
			if (Physics.Raycast (ray, out hit, rayLength, people)) {
					//Debug.DrawRay(ray.origin, ray.direction*30, Color.green);
					touchedGameObject = hit.collider.gameObject;//on récupère quel object a été touché
					if (touchedGameObject.tag =="People"){
						Dragging = true;
						float dist = Vector3.Distance(hit.point, transform.position);
						Vector3 temp = camera.ScreenToWorldPoint(new Vector3(fingerPos.x,fingerPos.y,dist));
						touchedGameObject.SendMessage("CreatePath",temp, SendMessageOptions.RequireReceiver);
						
					}
				}
				
				
			}
			
		}

		void FingerGestures_OnDragEnd (Vector2 fingerPos)
		{
				if (Dragging) {
						Dragging = false;
		
						touchedGameObject.SendMessage ("SetPath", SendMessageOptions.RequireReceiver);


				}
		}

	

		void OnEnable ()
		{	
				FingerGestures.OnDragBegin += FingerGestures_OnDragBegin;
				FingerGestures.OnDragMove += FingerGestures_OnDragMove;
				FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;

		}
	
		void OnDisable ()
		{
				FingerGestures.OnDragBegin -= FingerGestures_OnDragBegin;
				FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
				FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;

		}
}
