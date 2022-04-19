﻿
public class DijkstraAlgorithm
{
    private readonly Graph _visibilityGraph;
    private readonly KeyPoint _start;
    private readonly KeyPoint _destination;
    private readonly Dictionary<Point, float> _minDistanceToVertex = new();
    private readonly Dictionary<Point, bool> _visitedVertices = new();
    private readonly List<Point> _pathPoints = new();
    
    public DijkstraAlgorithm(Graph visibilityGraph, KeyPoint start, KeyPoint destination)
    {
        _visibilityGraph = visibilityGraph;
        _start = start;
        _destination = destination;
    }
    
    public List<Point> Evaluate()
    {
        Reset();
        EvaluatePathToEveryPoint();
        return RestorePathToTargetPoint();
    }

    private void Reset()
    {
        _visitedVertices.Clear();
        _minDistanceToVertex.Clear();

        foreach (Point graphVertex in _visibilityGraph.Vertices)
        {
            _minDistanceToVertex.Add(graphVertex, float.MaxValue);
            _visitedVertices.Add(graphVertex, false);
        }
        
        _minDistanceToVertex[_start.Point] = 0;
    }
    
    private void EvaluatePathToEveryPoint()
    {
        for (Point? targetPoint = GetTargetPoint(); targetPoint != null; targetPoint = GetTargetPoint())
        {
            TrySetNewMinPath(targetPoint.Value);

            _visitedVertices[targetPoint.Value] = true;
        }
    }
    
    private Point? GetTargetPoint()
    {
        Point? targetPoint = null;
        var min = float.MaxValue;

        foreach (Point point in _visibilityGraph.Vertices)
        {
            if (_visitedVertices[point] == false && _minDistanceToVertex[point] < min)
            {
                min = _minDistanceToVertex[point];
                targetPoint = point;
            }
        }

        return targetPoint;
    }

    private void TrySetNewMinPath(Point targetPoint)
    {
        foreach (Edge edge in _visibilityGraph.Edges)
        {
            if (IsEquals(in edge.LeftPoint, in targetPoint) || IsEquals(in edge.RightPoint, in targetPoint))
            {
                Point newPathPoint = IsEquals(in edge.LeftPoint, in targetPoint) ? edge.RightPoint : edge.LeftPoint;
                float distanceToNewVertex = _minDistanceToVertex[targetPoint] + edge.Magnitude;

                if (distanceToNewVertex < _minDistanceToVertex[newPathPoint])
                {
                    _minDistanceToVertex[newPathPoint] = distanceToNewVertex;
                }
            }
        }
    }
    
    private List<Point> RestorePathToTargetPoint()
    {
        _pathPoints.Clear();
        _pathPoints.Add(_destination.Point);

        for (Point currentPoint = _destination.Point; IsEquals(in currentPoint, _start.Point) == false;)
        {
            foreach (Edge edge in _visibilityGraph.Edges)
            {
                if (IsEquals(in edge.LeftPoint, in currentPoint) || IsEquals(in edge.RightPoint, in currentPoint))
                {
                    Point newPathPoint = IsEquals(in edge.LeftPoint, in currentPoint) ? edge.RightPoint : edge.LeftPoint;
                    float distanceToNewVertex = _minDistanceToVertex[currentPoint] - edge.Magnitude;
        
                    if (_minDistanceToVertex[newPathPoint] - distanceToNewVertex < 0.0001f)
                    {
                        _pathPoints.Add(newPathPoint);
                        currentPoint = newPathPoint;
                        _minDistanceToVertex[currentPoint] = distanceToNewVertex;
                        break;
                    }
                }
            }
        }

        _pathPoints.Reverse();
        return _pathPoints.GetRange(1, _pathPoints.Count - 2);
    }

    private bool IsEquals(in Point point1, in Point point2)
    {
        return Math.Abs(point1.X - point2.X) < float.Epsilon && Math.Abs(point1.Y - point2.Y) < float.Epsilon;
    }
}