using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class RandomLineRender : MonoBehaviour
{
	
		public Material[] randomMaterial;
		
	
		// Use this for initialization
		void Awake ()
		{
				int randomMat = UnityEngine.Random.Range (0, randomMaterial.Length);
				this.GetComponent<LineRenderer> ().material = randomMaterial [randomMat];
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}



}
