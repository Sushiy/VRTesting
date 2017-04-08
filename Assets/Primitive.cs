using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace primitive
{
    public class Primitive : MonoBehaviour {

        public static int PrimitiveCount { get { return s_iPrimitiveCount; } }
        private static int s_iPrimitiveCount = 0;

	    void Start () {
            s_iPrimitiveCount += 1;
	    }
	
	    void OnDestroy()
        {
            s_iPrimitiveCount -= 1;
        }
    }
}

