using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{

		public AudioClip music;
		private bool startedMusic = false;

		// Use this for initialization
		void Start ()
		{
				DontDestroyOnLoad (this.gameObject);
		}

		private void playMusic ()
		{
				this.audio.clip = music;
				this.audio.Play ();

		}
	
		// Update is called once per frame
		void Update ()
		{

				/*if (!this.audio.isPlaying && !startedMusic) {
						StartCoroutine (startMusic ());
						startedMusic = true;
				}*/
	
		}


		private IEnumerator startMusic ()
		{
				yield return new WaitForSeconds (2);

				playMusic ();
				startedMusic = false;
		}

}
