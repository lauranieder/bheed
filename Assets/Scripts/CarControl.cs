using UnityEngine;
using System.Collections;

public class CarControl : MonoBehaviour
{
		public Transform objToMoveTo;
		private Vector3 direction = Vector3.zero;
		public float movementSpeed = 2;
	
		public CrowdControl crowdControl;
	
	
		public Color[] randomColors;	
		private Color myColor;

	
	
		void Awake ()
		{
				if (randomColors != null && randomColors.Length != 0) {
						int randomColor = UnityEngine.Random.Range (0, randomColors.Length);
						myColor = randomColors [randomColor];
						this.renderer.material.color = myColor;
				}
		}
	
		void Start ()
		{
				direction = GetMovingDirection (objToMoveTo.position);
		}
	
		void FixedUpdate ()
		{
				this.rigidbody.MovePosition (this.rigidbody.position + direction * movementSpeed * Time.deltaTime);
		}
	
		private Vector3 GetMovingDirection (Vector3 positionTo)
		{
				Vector3 pos = (positionTo - this.transform.position).normalized;
				return new Vector3 (pos.x, 0, pos.z);
		}
	
		//private void 
		
		
}

