using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrowdControl : MonoBehaviour
{

		public Transform objToMoveTo;
		public Color spawnPointColor;
		public Vector3[] spawnPoints;

		public bool useRandomSpwan;
		public List<int> randomIndexUsed = new List<int> ();

		public List<PersonControl> personList = new List<PersonControl> ();


		public GameObject[] spawnPrefab;

		public int intialSpawnAmout = 5;
		public int spawnAmout = 5;
		public float tickTime = 1;
		public int spawnPerTick = 1;

		public float movementSpeed = 2;

		public bool useStraightMovement = false;

		public bool finishedSpawning = false;

		public Color[] randomColors;
		private Color personColor;

	
		// Use this for initialization
		void Awake ()
		{

				//randomIndexUsed = new List<int>();

				if (intialSpawnAmout != 0) {
						spawnPrefabs (intialSpawnAmout);
				}

				StartCoroutine (spawnOverTime (spawnAmout, spawnPerTick, tickTime));
		}
	
	
		// Update is called once per frame
		void Update ()
		{
	
		}


		private IEnumerator spawnOverTime (int spawnAmout, int spawnPerTick, float timeGap)
		{
				for (int i = 0; i < spawnAmout; i++) {
						yield return new WaitForSeconds (timeGap);
						spawnPrefabs (spawnPerTick);
				}

				finishedSpawning = true;
		}

	
		private void spawnPrefabs (int spawnAmout)
		{
				for (int i = 0; i < spawnAmout; i++) {
						Vector3 spawnPos = this.spawnPoints [getPredictatedRandom ()];
						//Debug.Log ("spawn " + i + " @ " + spawnPos);
						spawn (spawnPos);
				}
		}
	
	
		private int getPredictatedRandom ()
		{
				if (randomIndexUsed.Count >= this.spawnPoints.Length) {
						randomIndexUsed = new List<int> ();
				}
		
				int rand = -1;
				while (!this.randomIndexUsed.Contains(rand)) {
			
						rand = UnityEngine.Random.Range (0, this.spawnPoints.Length);
			
						if (!this.randomIndexUsed.Contains (rand)) {
								randomIndexUsed.Add (rand);
						}
				}
		
				return rand;
		}

		public void DestroyThat(GameObject temp){

		PersonControl PersonControltemp = temp.GetComponent<PersonControl> ();

		personList.Remove(PersonControltemp);
		}
	
		private void spawn (Vector3 pos)
		{
				//Instantiate (this.spawnPrefab, pos, Quaternion.identity);
				GameObject go = Instantiate (spawnPrefab[Random.Range (0,2)], pos, Quaternion.identity) as GameObject;
				PersonControl person = go.GetComponent<PersonControl> ();

				if (person != null) {
						person.objToMoveTo = objToMoveTo;
						person.movementSpeed = movementSpeed;
						person.crowdControl = this;
						SetRandomColor ();
						person.myColor = this.personColor;

						personList.Add (person);
				}

				CarControl car = go.GetComponent<CarControl> ();
		
				if (car != null) {
						car.objToMoveTo = objToMoveTo;
						car.movementSpeed = movementSpeed;
						car.crowdControl = this;
				}


/*				Vector3 direction = (objToMoveTo.position - this.transform.position).normalized;
				go.GetComponent<PersonControl> ().direction = new Vector3 (direction.x, 0, direction.z);
*/
		}


		public Vector3 GetRandomSpawnPos ()
		{
				return this.spawnPoints [UnityEngine.Random.Range (0, this.spawnPoints.Length)];
		}


		private void SetRandomColor ()
		{
				if (randomColors != null && randomColors.Length > 0) {
						int randomColor = UnityEngine.Random.Range (0, randomColors.Length);
						personColor = randomColors [randomColor];
				}

		}
	
}
