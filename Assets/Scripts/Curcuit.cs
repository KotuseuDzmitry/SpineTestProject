using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurcuitUtitlity
{
    public class Curcuit : MonoBehaviour
    {
        [SerializeField]
        private Path path = new Path();

        public Path Path
        {
            get
            {
                return path;
            }
        }
    }
}
