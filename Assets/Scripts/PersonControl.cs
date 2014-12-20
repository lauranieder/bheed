using UnityEngine;
using System.Collections;

public class PersonControl : MonoBehaviour
{
		public Sprite leftSpr;
		public Sprite rightSpr;

		private SpriteRenderer mySprRender;
		private Animator myAnim;

		public Transform objToMoveTo;
		private Vector3 direction = Vector3.zero;
		public float movementSpeed = 2;

		public CrowdControl crowdControl;
		public int[] materialToColor;
		//public Color[] randomColors;

		public Color myColor;

		[Range(0, 2)]
		public float
				maxDirChangeTime = 1;

		[Range(0, 2)]
		public float
				fadeTime = 0.5f;

		[Range(0, 2)]
		public float
				moveCoolDown = 1f;

		public bool moveToObj = true;
		public bool gotToShop = false;

		void Awake ()
		{
/*				if (randomColors != null && randomColors.Length > 0) {
						int randomColor = UnityEngine.Random.Range (0, randomColors.Length);
						myColor = randomColors [randomColor];
						this.renderer.material.color = myColor;
				}
*/
				//mySprRender = this.GetComponent<SpriteRenderer> ();
				//mySprRender = this.GetComponentInChildren<SpriteRenderer> ();
				//myAnim = this.GetComponentInChildren<Animator> ();

				//mySprRender.sprite = this.leftSpr;
	
				StartCoroutine (Fade (0, 0));
		}

		void Start ()
		{
			foreach(int i in materialToColor){
				transform.GetChild(i).gameObject.renderer.material.color = myColor;

			}
				
				//mySprRender.color = myColor;

				StartCoroutine (changeDirection (objToMoveTo.position, false));
				//direction = GetMovingDirection (objToMoveTo.position);

				StartCoroutine (Fade (1, fadeTime));
		}
	
		void FixedUpdate ()
		{
				if (moveToObj) {
						this.rigidbody.MovePosition (this.rigidbody.position + transform.forward * movementSpeed * Time.deltaTime);
				}
		}

		private Vector3 GetMovingDirection (Vector3 positionTo)
		{
				Vector3 pos = (positionTo - this.transform.position).normalized;
				return new Vector3 (pos.x, 0, pos.z);
		}

		void OnTriggerEnter (Collider coll)
		{
			
				if (coll.tag.Equals ("Shop")) {
						
						Debug.Log ("check Color " + myColor + " collColor" + coll.gameObject.GetComponent<ShopControl> ().myColor);

						if (myColor.ToString ().Equals (coll.gameObject.GetComponent<ShopControl> ().myColor.ToString ())) {
								//Debug.Log ("hit shop");
								PointHolder.Instance.points++;
								StartCoroutine (foundShop ());
						}
				}

				if (coll.tag.Equals ("Movement") && coll.gameObject.transform == this.objToMoveTo) {
						//Debug.Log ("hit crossroad");
						StartCoroutine (changeDirection (Vector3.zero));
				}

				if (coll.tag.Equals ("Border")) {
						//Debug.Log ("hit border");
						PointHolder.Instance.points--;
						StartCoroutine (outsideTheBorder ());
				}

		}

		void OnBecameInvisible() {
			crowdControl.SendMessage("DestroyThat", this.gameObject);
		}

		private IEnumerator foundShop ()
		{
		Debug.Log ("foundshop");
		crowdControl.SendMessage("DestroyThat", this.gameObject);
				Debug.Log ("COROUTINE STARTS");
				gotToShop = true;
				moveToObj = false;
				gameObject.SetActive(false);
				

		yield return null;
				
						
				Destroy (this.gameObject);
		}
		
		private IEnumerator outsideTheBorder ()
		{
				yield return StartCoroutine (Fade (0, fadeTime));

				yield return new WaitForSeconds (0.5f);
				Destroy (this.gameObject);
		}
	
		private IEnumerator Fade (float to, float time)
		{
				//Color sprColor = this.mySprRender.color;
				float multiplier = 1;

				if (to < 0) {
						multiplier *= -1;
				}

/*				float step = multiplier * to / time;

				for (float i = 0; i < time; i+= Time.deltaTime) {
						sprColor.a += multiplier * Time.deltaTime;
						Debug.Log ("fading to " + sprColor);
						this.mySprRender.color = sprColor;
						yield return new WaitForEndOfFrame ();
				}
*/
/*				iTween.FadeTo (this.gameObject, iTween.Hash (iT.FadeTo.alpha, to,
		                            iT.FadeTo.time, time));
*/
				yield return new WaitForSeconds (time);

		}

		private IEnumerator changeDirection (Vector3 dir, bool random = true)
		{
			if(random){
				float wait = UnityEngine.Random.Range (0, maxDirChangeTime);
				yield return new WaitForSeconds (wait);

			}

			

				

				if (dir == Vector3.zero) {
						dir = this.crowdControl.GetRandomSpawnPos ();
				}

				//direction = GetMovingDirection (dir);
		transform.forward = GetMovingDirection (dir);

				float angle = Vector3.Angle (direction, Vector3.forward);
				//Debug.Log ("direction.z " + Mathf.RoundToInt (direction.z) + " direction.x " + Mathf.RoundToInt (direction.x) + " " + Vector3.forward + " angle " + (int)angle);


		//transform.localRotation = new Quaternion(new Vector
				/*if (Mathf.RoundToInt (direction.z) != 0) {
						//this.myAnim.SetBool ("right", true);
						//this.myAnim.SetBool ("left", false);
			        transform.Rotate (new Vector3(0,90,0));
				} else if (Mathf.RoundToInt (direction.x) != 0) {	
						//this.myAnim.SetBool ("right", false);
						//this.myAnim.SetBool ("left", true);
			        transform.Rotate (new Vector3(0,-90,0));
				}*/
		}

		void OnDestroy ()
		{
			if(crowdControl != null){
					crowdControl.personList.Remove(this);
			}
				//crowdControl.SendMessage("DestroyThat", this.gameObject);
		}

		public void StopMoving ()
		{
				this.moveToObj = false;
				this.animation.Stop();
		}

		public IEnumerator RestartMoving ()
		{
				if (!gotToShop) {
						this.animation.Play();
						//direction = GetMovingDirection (objToMoveTo.position);

						yield return StartCoroutine (changeDirection (objToMoveTo.position));

						yield return new WaitForSeconds (moveCoolDown);

						this.moveToObj = true;
				}
		}
	
}

