using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(CrowdControl))]
public class CrowdControlInspector : Editor
{

		CrowdControl myScript;

		public void OnEnable ()
		{
				myScript = target as CrowdControl;
		}

		[ExecuteInEditMode]
		public override void OnInspectorGUI ()
		{
				//base.OnInspectorGUI ();

				base.DrawDefaultInspector ();

				EditorGUILayout.LabelField ("Points: " + PointHolder.Instance.points);

		}

		void OnSceneGUI ()
		{
				drawPosition ();
		}



		public void drawPosition ()
		{
				Handles.color = this.myScript.spawnPointColor;

				for (int i = 0; i < myScript.spawnPoints.Length; i++) {
						//Debug.Log ("myScript.spawnPoints [i] " + myScript.spawnPoints [i]);
						Handles.DrawSolidDisc (myScript.spawnPoints [i], Vector3.up, 1f);

						//Handles.DrawLine()
				}
		}

}
