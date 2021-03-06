﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BezierSurcuitUtitlity
{
    [Serializable]
    public class Path : IEnumerable
    {
#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        public bool isExpanded = false;
#endif

        [SerializeField]
        private List<BezierPoint> points = new List<BezierPoint>();

        [SerializeField]
        private bool isCyclic = false;

        public Path()
        {
            points.Add(new BezierPoint());
        }

        public Path(Vector2 startPosition)
        {
            points.Add(new BezierPoint(startPosition));
        }

        public bool IsCyclic
        {
            get
            {
                return isCyclic;
            }
            set
            {
                if (points.Count <= 1)
                {
                    isCyclic = false;
                    return;
                }
                isCyclic = value;
            }
        }


        public Vector2 PathPosition
        {
            get
            {
                return GetMediumPosition();
            }

            set
            {
                Vector2 currentMidPosition = GetMediumPosition();

                Vector2 deltaPosition = value - currentMidPosition;

                foreach (BezierPoint point in points)
                {
                    point.Anchor += deltaPosition;
                }
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
            if (points.Count <= 1)
            {
                return false;
            }

            bool removeResult = points.Remove(point);

            if (points.Count <= 1)
            {
                IsCyclic = false;
            }

            return removeResult;
        }

        public void RemovePointAt(int index)
        {
            if (points.Count <= 1)
            {
                return;
            }

            points.RemoveAt(index);

            if (points.Count <= 1)
            {
                IsCyclic = false;
            }
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

        private Vector2 GetMediumPosition()
        {
            Vector2 resultPosition = Vector2.zero;

            foreach (BezierPoint point in points)
            {
                resultPosition += point.Anchor / points.Count;
            }

            return resultPosition;
        }
}
}
