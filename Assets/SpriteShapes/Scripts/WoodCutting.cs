using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using ClipperLib;

public class WoodCutting : MonoBehaviour
{
    [SerializeField] SpriteShapeController wood;
    [SerializeField] Transform woodCutterTransform;

    [SerializeField] GameObject emptyShape;

    [SerializeField] SpriteShapeController blueSquare;
    [SerializeField] SpriteShapeController yellowSquare;

    [ContextMenu("CutSomeTrees")]
    public void CutSomeTrees()
    {
        int pointCount = wood.spline.GetPointCount();

        int closestIndex = 0;
        int secondClosestIndex;

        Vector3 aPosition;
        Vector3 bPosition;

        bool isPrevious = false;

        float smallestDistance = Vector3.Distance(woodCutterTransform.position, wood.spline.GetPosition(0));

        for (int i = 1; i < pointCount; i++)
        {            
            float distance = Vector3.Distance(woodCutterTransform.position, wood.spline.GetPosition(i));
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestIndex = i;
            }
        }

        int nextIndex;
        int previousIndex;

        if (closestIndex == pointCount - 1)
        {
            nextIndex = 0;            
        }
        else
        {
            nextIndex = closestIndex + 1;
        }

        if(closestIndex == 0)
        {
            previousIndex = pointCount - 1;
        }
        else
        {
            previousIndex = closestIndex - 1;
        }

        float nextDistance = Vector3.Distance(woodCutterTransform.position, wood.spline.GetPosition(nextIndex));
        float previousDistance = Vector3.Distance(woodCutterTransform.position, wood.spline.GetPosition(previousIndex));

        if(nextDistance<=previousDistance)
        {
            secondClosestIndex = nextIndex;
            aPosition = wood.spline.GetPosition(closestIndex);
            bPosition = wood.spline.GetPosition(secondClosestIndex);
        }
        else
        {
            // треба зробити свій тип даних, який буде зберігати індекс разом із позицією

            isPrevious = true;
            secondClosestIndex = previousIndex;
            bPosition = wood.spline.GetPosition(closestIndex);
            aPosition = wood.spline.GetPosition(secondClosestIndex);
        }


        Vector2 pointPos = FindNearestPointOnLine(aPosition, bPosition, woodCutterTransform.position);
        
        if(isPrevious)
        {
            wood.spline.InsertPointAt(GetNextIndex(secondClosestIndex,pointCount-1), pointPos);
        }
        else
        {
            wood.spline.InsertPointAt(GetNextIndex(closestIndex,pointCount-1), pointPos);
        }

        





    }

    public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        //Get heading
        Vector2 heading = end - origin;
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }
    
    private int GetNextIndex(int index, int maxIndex)
    {
        return index == maxIndex ? 0 : index + 1;
    }

    [ContextMenu("ListPositions")]
    public void ListPositions()
    {
        foreach(ShapeControlPoint shapeControlPoint in blueSquare.spline.ControlPoints)
        {
            Debug.Log(shapeControlPoint.position);
        }
    }

    [ContextMenu("TestClipper")]
    public void TestClipper()
    {
        List<ShapeControlPoint> bluePoints = blueSquare.spline.ControlPoints;
        List<ShapeControlPoint> yellowPoints = yellowSquare.spline.ControlPoints;

        List<IntPoint> blueIntPoints = VectorsToPoints(bluePoints, blueSquare.transform.position);
        List<IntPoint> yellowIntPoints = VectorsToPoints(yellowPoints, yellowSquare.transform.position);

        Clipper c = new Clipper();
        c.AddPath(blueIntPoints, PolyType.ptSubject, true);
        c.AddPath(yellowIntPoints, PolyType.ptClip, true);

        List<List<IntPoint>> solution = new List<List<IntPoint>>();

        c.Execute(ClipType.ctIntersection, solution);

        DrawShapeIntersection(solution[0]);

        for (int i = 0; i < solution[0].Count; i++)
        {
            Debug.Log($"X: {solution[0][i].X}, Y: {solution[0][i].Y}");
        }
    }

    private List<IntPoint> VectorsToPoints(List<ShapeControlPoint> points, Vector3 goPosition)
    {
        List<IntPoint> intPoints = new List<IntPoint>();

        foreach(var point in points)
        {
            intPoints.Add(new IntPoint(point.position.x + goPosition.x, point.position.y + goPosition.y));
        }

        return intPoints;
    }

    private void DrawShapeIntersection(List<IntPoint> intPoints)
    {
        SpriteShapeController controller = Instantiate(emptyShape).GetComponent<SpriteShapeController>();

        int index = 0;

        for (int i = intPoints.Count-1; i >= 0; i--)
        {
            if(i>intPoints.Count-3)
            {
                controller.spline.SetPosition(index, IntPointToVector3(intPoints[i]));
            }
            else
            {
                controller.spline.InsertPointAt(index, IntPointToVector3(intPoints[i]));
            }
            index++;
        }
    }

    private Vector3 IntPointToVector3(IntPoint point)
    {
        float x = point.X;
        float y = point.Y;

        return new Vector3(x, y, 0f);
    }
}
