using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SurcuitUtitlity
{
    [Serializable]
    public class Path : IEnumerable
    {
        [SerializeField]
        private List<BezierPoint> points = new List<BezierPoint>();

        private bool isCyclic = false;

        public Path()
        {
            points.Add(new BezierPoint());
        }

        public bool IsCyclic
        {
            get
            {
                return isCyclic;
            }
            set
            {
                isCyclic = value;
            }
        }

        public void AddPoint(BezierPoint point)
        {
            points.Add(point);
        }

        public void InsertPoint(int index, BezierPoint point)
        {
            points.Insert(index, point);
        }

        public void AddPointsRange(IEnumerable<BezierPoint> points)
        {
            this.points.AddRange(points);
        }

        public bool RemovePoint(BezierPoint point)
        {
            return points.Remove(point);
        }

        public void RemovePointAt(int index)
        {
            points.RemoveAt(index);
        }

        public int Count
        {
            get
            {
                return points.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return points.GetEnumerator();
        }

        public BezierPoint this[int i]
        {
            get
            {
                return points[i];
            }
            set
            {
                points[i] = value;
            }
        }
}
}
