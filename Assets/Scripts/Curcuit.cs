using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierSurcuitUtitlity
{
    public class Curcuit : MonoBehaviour, IEnumerable
    {
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        public bool isExpanded = false;
#endif

        [SerializeField]
        private List<Path> paths = new List<Path>()
        {
            new Path()
        };

        public List<Path> Paths => new List<Path>(paths);

        public void AddPath(Path path)
        {
            paths.Add(path);
        }

        public void InsertPath(int index, Path path)
        {
            paths.Insert(index, path);
        }

        public void AddPathsRange(IEnumerable<Path> paths)
        {
            this.paths.AddRange(paths);
        }

        public bool RemovePath(Path path)
        {
            if (paths.Count <= 1)
            {
                return false;
            }

            return paths.Remove(path);
        }

        public void RemovePathAt(int index)
        {
            if (paths.Count <= 1)
            {
                return;
            }

            paths.RemoveAt(index);
        }

        public int Count
        {
            get
            {
                return paths.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return paths.GetEnumerator();
        }

        public Path this[int i]
        {
            get
            {
                return paths[i];
            }
            set
            {
                paths[i] = value;
            }
        }
    }
}
