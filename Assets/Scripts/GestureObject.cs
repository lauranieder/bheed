using UnityEngine;
using System.Collections;

/*
 * Camera Controller IOS
 * Written by Laura Perrenoud
 * 
 * Based on fingerGesture*/

/*15 JUIN : Added tutoriel fonctionnality


 */

public class GestureObject : MonoBehaviour {

	public bool flatmode;
	public bool retinaDisplay = false;
	private int screenRes;
	private Vector2 DragPreviousPos;
	private Vector2 DragCurrentPos;

	//Zoom
    public float pinchZoomSensitivity = 2.0f;
	public float initialDistance = 10.0f;
	public float[] initialDistances;
    public float minDistance = 1.0f;
    public float maxDistance = 20.0f;
	
	public float distance = 20.0f;
	float idealDistance = 0;
	
	//Panning
    public float panningSensitivity = 1.0f;
	Vector3 idealPanOffset = Vector3.zero;
    public Vector3 panOffset = Vector3.zero;
	public bool panningLimited;
	public Vector3 initialPanOffset = new Vector3(10.4f,0.0f,10.0f);

	public float[] LimitsDisplayHeight;
	public Vector4[] Limits;
	public int limitIndex = 0;

	//Rotation
    public float yaw = 0;//gauche/droite
    public float pitch = 0;//haut/bas
    float idealYaw = 0;
    float idealPitch = 0;
	public float initialYaw;

    public float yawSensitivity = 80.0f;
    public float pitchSensitivity = 80.0f;
    public bool clampPitchAngle = true; 
    public float minPitch = -20;
    public float maxPitch = 80;

	//Smooth
	public bool smoothMotion = true;
	public float smoothZoomSpeed = 3.0f;
	public float smoothPanningSpeed = 3.0f;
	public float smoothOrbitSpeed = 3.0f;
	
	//Target
	public Vector3 initialTarget;
	public Vector3 target;
	public Vector3 IdealTarget;
	public float smoothChangeTargetSpeed = 3.0f;
	public Transform[] targets;
	
	//Draggable Object
	private GameObject touchedGameObject;

	//Boolean pour empêcher de confondre les interactions d'interface avec les touch des objets
	bool isRotating = false;
	bool isDragging = false;
	bool isZooming = false;
	bool isDraggingObject = false;

	//Boolean Enable Interaction

	public bool rotationEnabled = true;
	public bool zoomEnabled = true;
	public bool dragAroundEnabled = true;
	public bool dragObjectEnabled = true;
	public bool swipeUpEnabled = true;
	public bool swipeDownEnabled = true;

	public bool debug = true;

	public bool tutoriel = false;

	public bool alreadyRotated = false;
	public bool alreadyZoomed = false;
	public bool alreadyDraggedAround = false;
	public bool alreadyDraggedObject = false;
	public bool alreadySwipedUp = false;
	public bool alreadySwipedDown = false;

	public int LevelStepIndex;


	IEnumerator Intro(){

		//Quaternion rot = Quaternion.Euler(20,-224.62f,0);
		SetOffset(new Vector3(-4.07f,0,-1.27f), Quaternion.Euler(60,-224.62f,0), 30.0f);

		yield return new WaitForSeconds(3.0f);

		SetOffset(initialPanOffset, Quaternion.Euler(20,initialYaw,0), 5);

		yield return new WaitForSeconds(5.0f);

		SetSmoothValues(3.0f);
	}
	//---------------------------------------------------------------------------------------------------INITIALISATION--------------------------------------------------------------------------------------------------
	void Awake(){


		//Tutoriel Handler
		if(tutoriel){
			SetEnableInteraction(false);

			alreadyRotated = false;
			alreadyZoomed = false;
			alreadyDraggedAround = false;
			alreadyDraggedObject = false;
			alreadySwipedUp = false;
			alreadySwipedDown = false;
		
		}else{
			SetEnableInteraction(true);

			alreadyRotated = true;
			alreadyZoomed = true;
			alreadyDraggedAround = true;
			alreadyDraggedObject = true;
			alreadySwipedUp = true;
			alreadySwipedDown = true;
		}
	}
	
	void Start () {

		target = IdealTarget = initialTarget;

		//Set initial value
		//target = IdealTarget = targets[0].position;
		distance = IdealDistance = initialDistance;
		yaw = IdealYaw = initialYaw;
		panOffset = IdealPanOffset = initialPanOffset;

		//FlatMode
		if (flatmode){	IdealPitch = maxPitch;}else{IdealPitch = minPitch;}

		// Make the rigid body not change rotation
        if( rigidbody ){
            rigidbody.freezeRotation = true;
		}
		//Smoothing adapted to screenRes
		if (retinaDisplay){
			screenRes = 2;	
		}else{
			screenRes = 1;
		}

		//SetSmoothValues(0.5f);
		//SetOffset(new Vector3(-4.07f,0,-1.27f), Quaternion.Euler(20,-224.62f,0), 30.0f);

		//StartCoroutine("Intro");

		//SetSmoothValues(3.0f);
	}

	void LateUpdate () {
		Apply();
	}
	
	//---------------------------------------------------------------------------------------------------APPLY--------------------------------------------------------------------------------------------------
	void Apply(){
	
		if(smoothMotion){
            distance = Mathf.Lerp( distance, IdealDistance, Time.deltaTime * smoothZoomSpeed );
			panOffset = Vector3.Lerp( panOffset, IdealPanOffset, Time.deltaTime * smoothPanningSpeed );
			yaw = Mathf.Lerp( yaw, IdealYaw, Time.deltaTime * smoothOrbitSpeed );
            pitch = Mathf.Lerp( pitch, IdealPitch, Time.deltaTime * smoothOrbitSpeed );
			target = Vector3.Lerp( target, IdealTarget, Time.deltaTime * smoothChangeTargetSpeed );
        }
        else{
            distance = IdealDistance;
			panOffset = IdealPanOffset;
			yaw = IdealYaw;
            pitch = IdealPitch;
			target = IdealTarget;   
        }

		transform.rotation = Quaternion.Euler(pitch, yaw, 0 );
		transform.position = (target + panOffset) - distance * transform.forward;
	}

	//__________________________________________________________________________________________________SETTER_________________________________________________________________________________________________

	public IEnumerator MoveCamera(float _distance, Vector3 _panOffset, float _yaw, float _duration){
		//SetEnableInteraction(false);
		float elapsedTime = 0;
		
		float _distanceStart = distance;
		Vector3 _panOffsetStart = panOffset;
		float _yawStart = yaw;
		
		while (elapsedTime < _duration){
			
			IdealDistance = Mathf.Lerp(_distanceStart, _distance, (elapsedTime / _duration));
			IdealPanOffset = Vector3.Lerp(_panOffsetStart, _panOffset, (elapsedTime / _duration));
			IdealYaw = Mathf.Lerp(_yawStart, _yaw, (elapsedTime / _duration));
			
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	public IEnumerator MoveCamera(float _distance, Vector3 _panOffset, float _yaw, float _pitch, float _duration){
		//SetEnableInteraction(false);
		float elapsedTime = 0;
		
		float _distanceStart = distance;
		Vector3 _panOffsetStart = panOffset;
		float _yawStart = yaw;
		float _pitchStart = pitch;

		_yaw = OffsetYaw(_yaw);
		
		while (elapsedTime < _duration){
			
			IdealDistance = Mathf.Lerp(_distanceStart, _distance, (elapsedTime / _duration));
			IdealPanOffset = Vector3.Lerp(_panOffsetStart, _panOffset, (elapsedTime / _duration));
			IdealYaw = Mathf.Lerp(_yawStart, _yaw, (elapsedTime / _duration));
			IdealPitch = Mathf.Lerp(_pitchStart, _pitch, (elapsedTime / _duration));
			
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	float OffsetYaw(float yawValue){
		float offset = yaw - yawValue;

		print("offset : "+offset);

		/*if(offset > 360){

			print("positif");
			yawValue = offset%360;
			
		}else if(offset< -360){
			print("negatif");
			yawValue = offset%360;
			
		}*/

		yawValue = offset%360;

		print("offset%360 : "+offset%360);
		print("yaw : "+yaw);
		print("yawValue : "+yawValue);

		yawValue = yaw-yawValue;
		print("yawValue : "+yawValue);

		return yawValue;
	}
	

	public void SetTarget(Transform _target, bool _reset){
		IdealTarget = _target.position;
		if(_reset){
			ResetOffset();
		}
	}

	public void SetTarget(int _target, bool _reset){
		IdealTarget = targets[_target].position;
		if(_reset){
			ResetOffset();
		}
	}

	//SetLimits
	public void SetLimits(int _limitIndex){ 
		Debug.Log("SetLimits");
		limitIndex = _limitIndex;

	}

	//Remet le offset au valeurs initiale
	public void ResetOffset(){
		IdealDistance = initialDistance;
		IdealPanOffset = initialPanOffset;
		//IdealYaw = initialYaw;

		//FlatMode Off
		IdealPitch = minPitch;
		flatmode = false;
	}

	//A éviter, pas très propre! On préfère l'IEnumerator MoveCamera
	//Change le offset de la camera
	public void SetOffset(Vector3 _position, Quaternion _rotation, float _distance){
		IdealDistance = _distance;
		IdealPanOffset = _position;
		IdealYaw = _rotation.eulerAngles.y;
		IdealPitch = _rotation.eulerAngles.x;


		//FlatMode Off
		//IdealPitch = minPitch;
		//flatmode = false;
		
	}

	//Change les valeurs de easing simultanément
	public void SetSmoothValues(float _value){
		smoothMotion = true;
		smoothZoomSpeed = _value;
		smoothPanningSpeed = _value;
		smoothOrbitSpeed = _value;

	}

	//Passe d'un mode Plat à un mode en Perspective
	void ToggleFlatmode(){
		if (flatmode){
			IdealPitch = minPitch;
		}else{	
			IdealPitch = maxPitch;
		}
		flatmode = !flatmode;
	}

	void ToggleFlatmode(bool _flat){



		if( _flat){
			Debug.Log("false");
			IdealPitch = minPitch;
			flatmode = false;
		}else{
			Debug.Log("true");
			IdealPitch = maxPitch;
			flatmode = true;
		}

	}

	//_____________________________________________________________________________GAME MANAGER CALLBACKS_______________________________________________________________________________________________

	public void HandleOnStepChange(int _LevelStepIndex){
		LevelStepIndex = _LevelStepIndex;
	
	}

	public void HandleOnSuccess(){
		SetEnableInteraction(false);
	
	}

	public void HandleOnPause(bool _paused){
		SetEnableInteraction(!_paused);
	}

	public void SetEnableInteraction(bool _enable){
		if(_enable){
			//Enable all interaction
			rotationEnabled = true;
			zoomEnabled = true;
			dragAroundEnabled = true;
			dragObjectEnabled = true;
			swipeUpEnabled = true;
			swipeDownEnabled = true;

			Debug.Log("Interaction Enabled");


		}else{
			//Disable all interaction
			rotationEnabled = false;
			zoomEnabled = false;
			dragAroundEnabled = false;
			dragObjectEnabled = false;
			swipeUpEnabled = false;
			swipeDownEnabled = false;

			Debug.Log("Interaction Disabled");
		}
	}

	//-------------------------------------------------------------------------------------------------------GESTURES--------------------------------------------------------------------------------------------
	//Rotation à 2 doigts
	void FingerGestures_OnRotationBegin( Vector2 fingerPos1, Vector2 fingerPos2 ){ if(rotationEnabled){isRotating=true;}}
	void FingerGestures_OnRotationMove( Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta ){



		if(rotationEnabled){
			if(targets.Length>0){
				IdealYaw += rotationAngleDelta * (yawSensitivity/screenRes) * 0.02f;
			}
		}
	}
	void FingerGestures_OnRotationEnd( Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle ){isRotating = false; if(rotationEnabled){alreadyRotated = true; }}
	
	//Zoom à 2 doigts
	void FingerGestures_OnPinchBegin( Vector2 fingerPos1, Vector2 fingerPos2 ){if(zoomEnabled){isZooming = true;}}
	void FingerGestures_OnPinchMove( Vector2 fingerPos1, Vector2 fingerPos2, float delta ){



		if(zoomEnabled){
			IdealDistance -= delta * (pinchZoomSensitivity/screenRes);
		}
	}
	void FingerGestures_OnPinchEnd( Vector2 fingerPos1, Vector2 fingerPos2 ){isZooming = false;if(zoomEnabled){alreadyZoomed = true; }}
	
	//Drag à 1 doigt + Drag d'objet
	void FingerGestures_OnDragBegin( Vector2 fingerPos, Vector2 startPos ){
		Ray ray = Camera.main.ScreenPointToRay(fingerPos);

		//print(fingerPos);

		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			Debug.DrawRay(ray.origin, ray.direction*30, Color.green);


			/*Debug.DrawRay(ray2.origin, ray2.direction*30, Color.blue);
			Debug.DrawRay(ray3.origin, ray3.direction*30, Color.blue);
			Debug.DrawRay(ray4.origin, ray4.direction*30, Color.blue);
			Debug.DrawRay(ray5.origin, ray5.direction*30, Color.blue);*/

			touchedGameObject = hit.collider.gameObject;//on récupère quel object a été touché
			
			if (touchedGameObject.tag =="Drag"){
				if(dragObjectEnabled){
					touchedGameObject.SendMessage("OnDragBegin",fingerPos,SendMessageOptions.DontRequireReceiver);//on lui dit qu'il a été touché
					isDraggingObject = true; //Drag d'objet
				}
			}else{
				if(dragAroundEnabled){
					touchedGameObject.SendMessage("DragImpossible", SendMessageOptions.DontRequireReceiver);
					isDragging = true; //Drag camera
				}
			}
		}else{
			int offsetRay = Screen.width/80;
			Ray ray2 = Camera.main.ScreenPointToRay(fingerPos + new Vector2(offsetRay,0));
			Ray ray3 = Camera.main.ScreenPointToRay(fingerPos + new Vector2(-offsetRay,0));
			Ray ray4 = Camera.main.ScreenPointToRay(fingerPos + new Vector2(0,offsetRay));
			Ray ray5 = Camera.main.ScreenPointToRay(fingerPos + new Vector2(0,-offsetRay));


			if (Physics.Raycast(ray2, out hit)) {
				touchedGameObject = hit.collider.gameObject;//on récupère quel object a été touché
				
				if (touchedGameObject.tag =="Drag"){
					if(dragObjectEnabled){
						touchedGameObject.SendMessage("OnDragBegin",fingerPos,SendMessageOptions.DontRequireReceiver);//on lui dit qu'il a été touché
						isDraggingObject = true; //Drag d'objet
					}
				}else{
					if(dragAroundEnabled){
						touchedGameObject.SendMessage("DragImpossible", SendMessageOptions.DontRequireReceiver);
						isDragging = true; //Drag camera
					}
				}




			}else if (Physics.Raycast(ray3, out hit)) {
				touchedGameObject = hit.collider.gameObject;//on récupère quel object a été touché
				
				if (touchedGameObject.tag =="Drag"){
					if(dragObjectEnabled){
						touchedGameObject.SendMessage("OnDragBegin",fingerPos,SendMessageOptions.DontRequireReceiver);//on lui dit qu'il a été touché
						isDraggingObject = true; //Drag d'objet
					}
				}else{
					if(dragAroundEnabled){
						touchedGameObject.SendMessage("DragImpossible", SendMessageOptions.DontRequireReceiver);
						isDragging = true; //Drag camera
					}
				}
				
				
				
				
			}else if (Physics.Raycast(ray4, out hit)) {
				touchedGameObject = hit.collider.gameObject;//on récupère quel object a été touché
				
				if (touchedGameObject.tag =="Drag"){
					if(dragObjectEnabled){
						touchedGameObject.SendMessage("OnDragBegin",fingerPos,SendMessageOptions.DontRequireReceiver);//on lui dit qu'il a été touché
						isDraggingObject = true; //Drag d'objet
					}
				}else{
					if(dragAroundEnabled){
						touchedGameObject.SendMessage("DragImpossible", SendMessageOptions.DontRequireReceiver);
						isDragging = true; //Drag camera
					}
				}
				
				
				
				
			}else if (Physics.Raycast(ray5, out hit)) {
				touchedGameObject = hit.collider.gameObject;//on récupère quel object a été touché
				
				if (touchedGameObject.tag =="Drag"){
					if(dragObjectEnabled){
						touchedGameObject.SendMessage("OnDragBegin",fingerPos,SendMessageOptions.DontRequireReceiver);//on lui dit qu'il a été touché
						isDraggingObject = true; //Drag d'objet
					}
				}else{
					if(dragAroundEnabled){
						touchedGameObject.SendMessage("DragImpossible", SendMessageOptions.DontRequireReceiver);
						isDragging = true; //Drag camera
					}
				}
				
				
				
				
			}else if(dragAroundEnabled){
				isDragging = true; //Drag camera
			}
		}
	}
	void FingerGestures_OnDragMove( Vector2 fingerPos, Vector2 delta ){




		///PANNING
		//print(fingerPos);
		if(isDragging) {
			if(dragAroundEnabled){
				Vector3 move = -0.02f * ((panningSensitivity*(distance/10))/screenRes) * ( Vector3.right * delta.x + Vector3.forward * delta.y );
				Vector3 moveGlobal = Quaternion.AngleAxis(yaw, Vector3.up) * move;
				IdealPanOffset += moveGlobal;
			}   
		}
		else if(isDraggingObject){
			if(dragObjectEnabled){
				touchedGameObject.SendMessage("OnDragMove",fingerPos, SendMessageOptions.DontRequireReceiver);
			}
		}
		int offsetRay = Screen.width/80;
		Ray ray = Camera.main.ScreenPointToRay(fingerPos);
		Ray ray2 = Camera.main.ScreenPointToRay(fingerPos+ new Vector2(offsetRay,0));
		Ray ray3 = Camera.main.ScreenPointToRay(fingerPos + new Vector2(-offsetRay,0));
		Ray ray4 = Camera.main.ScreenPointToRay(fingerPos + new Vector2(0,offsetRay));
		Ray ray5 = Camera.main.ScreenPointToRay(fingerPos + new Vector2(0,-offsetRay));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			Debug.DrawRay(ray.origin, ray.direction*60, Color.green);
			Debug.DrawRay(ray2.origin, ray2.direction*60, Color.blue);
			Debug.DrawRay(ray3.origin, ray3.direction*60, Color.blue);
			Debug.DrawRay(ray4.origin, ray4.direction*60, Color.blue);
			Debug.DrawRay(ray5.origin, ray5.direction*60, Color.blue);

		}
	}
	void FingerGestures_OnDragEnd( Vector2 fingerPos )
	{



		if(isDragging){
			isDragging = false;
			if(dragAroundEnabled){alreadyDraggedAround = true;}
		}else if(isDraggingObject){
			if(dragObjectEnabled){
				touchedGameObject.SendMessage("OnDragEnd",fingerPos,SendMessageOptions.DontRequireReceiver);
				alreadyDraggedObject = true;}
		}
	}
	
	//SWIPE a 2 doigts
	void FingerGestures_OnSwipe( Vector2 fingerPos1, FingerGestures.SwipeDirection swipeDirection, float Velocity ){



		if(isRotating == false && isZooming == false){
			if (swipeDirection == FingerGestures.SwipeDirection.Up){
				if(swipeUpEnabled){
					//3D
					ToggleFlatmode(true);
					alreadySwipedUp = true;
				}
				
			}else if (swipeDirection == FingerGestures.SwipeDirection.Down){
				if(swipeDownEnabled){
					ToggleFlatmode(false);
					alreadySwipedDown = true;
				}
			}
		}
	}
	//Plus utilisée pour l'instant
	void FingerGestures_OnTap( Vector2 fingerPos, int tapNumber ){  }
	
	//-----------------------------PAS DE MODIFICATION A FAIRE
	public float Distance
    {
        get { return distance; }
    } 

    public float IdealDistance
    {
        get { return idealDistance; }
        set { idealDistance = Mathf.Clamp( value, minDistance, maxDistance ); }
    }
	public Vector3 IdealPanOffset
    {
        get { return idealPanOffset; }
        
		set { if(panningLimited){

				float newX = Mathf.Clamp( value.x, Limits[limitIndex].y - target.x, Limits[limitIndex].x - target.x ); 
				float newZ = Mathf.Clamp( value.z, Limits[limitIndex].w - target.z, Limits[limitIndex].z - target.z );

				idealPanOffset = new Vector3(newX,value.y,newZ); 
			}else{
				idealPanOffset = value;
			}
		}
    }
	public float Yaw
    {
        get { return yaw; }
    } 

    public float IdealYaw
    {
        get { return idealYaw; }
        set { 

			//Si la valeur value est trop différente de ideal yaw

			/*float offset = idealYaw - value;
			if(offset > 360){
				 
				value = value-(offset%360)*360;

			}else if(offset<-360){
				value = value+(offset%360)*360;

			}*/

			idealYaw = value; 
		}
    }

    public float Pitch
    {
        get { return pitch; }
    } 

    public float IdealPitch
    {
        get { return idealPitch; }
        set { idealPitch = clampPitchAngle ? ClampAngle( value, minPitch, maxPitch ) : value; }
    }
	static float ClampAngle( float angle, float min, float max )
    {
        if( angle < -360 )
            angle += 360;
        
        if( angle > 360 )
            angle -= 360;

        return Mathf.Clamp( angle, min, max );
    }
	void OnEnable()
    {	
		FingerGestures.OnDragBegin += FingerGestures_OnDragBegin;
        FingerGestures.OnDragMove += FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;
		
		FingerGestures.OnPinchBegin += FingerGestures_OnPinchBegin; 
        FingerGestures.OnPinchMove += FingerGestures_OnPinchMove; 
		FingerGestures.OnPinchEnd += FingerGestures_OnPinchEnd; 
		
		FingerGestures.OnRotationBegin += FingerGestures_OnRotationBegin;
		FingerGestures.OnRotationMove += FingerGestures_OnRotationMove;//rotation 2 doigts
		FingerGestures.OnRotationEnd += FingerGestures_OnRotationEnd;

		FingerGestures.OnTap += FingerGestures_OnTap;

		FingerGestures.OnSwipe += FingerGestures_OnSwipe;
    }
        
    void OnDisable()
    {
		FingerGestures.OnDragBegin -= FingerGestures_OnDragBegin;
        FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;
		
		FingerGestures.OnPinchBegin -= FingerGestures_OnPinchBegin; 
        FingerGestures.OnPinchMove -= FingerGestures_OnPinchMove;
		FingerGestures.OnPinchEnd -= FingerGestures_OnPinchEnd; 
		
		FingerGestures.OnRotationBegin -= FingerGestures_OnRotationBegin;
		FingerGestures.OnRotationMove -= FingerGestures_OnRotationMove;
		FingerGestures.OnRotationEnd -= FingerGestures_OnRotationEnd;

		FingerGestures.OnTap -= FingerGestures_OnTap;

		FingerGestures.OnSwipe -= FingerGestures_OnSwipe;
    }

	//Gizmos Show Dragging & Panning Limits, si UseLimits = true 
	void OnDrawGizmos(){
		if(debug){
			
			Gizmos.color = Color.blue;

			for(int i=0; i<Limits.Length; i++){

				float height;
				if(LimitsDisplayHeight.Length>0){
					height = LimitsDisplayHeight[i];
				}else{
					height = 0;
				}
				Gizmos.color = Color.blue;

				if(i == LevelStepIndex){
					Gizmos.color = Color.red;
				}else if(i == LevelStepIndex+1){
					Gizmos.color = Color.green;
				}

				Vector3 _Corner1 = new Vector3(Limits[i].x, height, Limits[i].w);
				Vector3 _Corner2 = new Vector3(Limits[i].x, height, Limits[i].z);
				Vector3 _Corner3 = new Vector3(Limits[i].y, height, Limits[i].z);
				Vector3 _Corner4 = new Vector3(Limits[i].y, height, Limits[i].w);
				
				Gizmos.DrawLine(_Corner1, _Corner2);
				Gizmos.DrawLine(_Corner2, _Corner3);
				Gizmos.DrawLine(_Corner3, _Corner4);
				Gizmos.DrawLine(_Corner4, _Corner1);

			}
		}
	}

	// Whenever the object gets destroyed.
	void OnDestroy(){
		

		
	}
}
