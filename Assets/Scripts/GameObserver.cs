using UnityEngine;
using System.Collections;

public class GameObserver : MonoBehaviour
{

		public AudioClip success;
		public AudioClip fail;

		private CrowdControl personSpawner;
		private CrowdControl personSpawner2;

		
		

		public TextMesh pointText;


		// Use this for initialization
		void Awake ()
		{	
				PointHolder.Instance.points = 0;
				//personSpawner = GameObject.FindWithTag ("PersonSpawner").GetComponent<CrowdControl> ();
			GameObject[] temp = GameObject.FindGameObjectsWithTag("PersonSpawner");
		personSpawner = temp[0].GetComponent<CrowdControl> ();
		personSpawner2 = temp[1].GetComponent<CrowdControl> ();
		}
	
		void FixedUpdate ()
	{if (pointText != null) {
			if(PointHolder.Instance.points <= 0){
				pointText.text = "" + "0"+"/"+(personSpawner.spawnAmout);
				
			}else{
				
				pointText.text = "" + PointHolder.Instance.points+"/"+(personSpawner.spawnAmout);
			}
		}
			
			if (personSpawner.finishedSpawning && personSpawner2.finishedSpawning) {
						if (checkForEnd ()) {
				if (PointHolder.Instance.points >= personSpawner.spawnAmout) {
					pointText.text = "GREAT";
										playClip (this.success);
										Debug.Log ("GREAT");

										StartCoroutine("loadNextLvl");
										
								} else {
					pointText.text = "FAIL";
					Debug.Log ("FAIL");
										playClip (this.fail);

										StartCoroutine("reloadThisLevel");
								}
						}
			}

				
		}


/*		void OnGUI ()
		{
				GUI.Label (new Rect (Screen.height, 100, 200, 200), "Points " + PointHolder.Instance.points);
		}
*/

		private bool checkForEnd ()
		{
		Debug.Log ("count : "+personSpawner.personList.Count+"  count2 : "+personSpawner2.personList.Count);
			if(personSpawner.personList.Count <= 0 && personSpawner2.personList.Count <= 0){
				Debug.Log ("checkForEndtrue");
				return true;
			}else{
				Debug.Log ("checkForEndfalse");
				return false;
			}
			//return (personSpawner.personList.Count <= 0);
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

		private IEnumerator loadNextLvl ()
		{
			pointText.text = "GREAT";
				yield return new WaitForSeconds (4.0f);
				
				Application.LoadLevel(Application.loadedLevel + 1);

		}

		private IEnumerator reloadThisLevel()
		{
			pointText.text = "FAIL";
			yield return new WaitForSeconds (4.0f);
			
			Application.LoadLevel(Application.loadedLevel);
			
		}

}

