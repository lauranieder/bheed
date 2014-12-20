using UnityEngine;
using System.Collections;

public class PointHolder
{

		public int points;

		private static PointHolder instance = null;

		public static PointHolder Instance {
				get {
						if (instance == null) {
								instance = new PointHolder ();
						}
						return instance; 
				}
		}


}

