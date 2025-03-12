using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    public List<GameObject> nodes;
    private List<AIPathSegment> segments;

    private void Awake() {
        SetSegments();
    }
    private void SetSegments() {
        segments = new List<AIPathSegment>();
        for (int i = 0; i < nodes.Count - 1; i++) {
            Vector3 a = nodes[i].transform.position;
            Vector3 b = nodes[i + 1].transform.position;
            AIPathSegment segment = new AIPathSegment(a, b);
            segments.Add(segment);
        }
    }

    public float GetParam(Vector3 position, float lastParam) {
        // Ближайший к позиции сегмент
        AIPathSegment currentSegment = null;
        float tmpParam = 0f;
        foreach (AIPathSegment segment in segments)
        {
            tmpParam += Vector3.Distance(segment.a, segment.b);
            if (lastParam <= tmpParam)
            {
                currentSegment = segment;
                break;
            }
        }
        if (currentSegment == null)
            return 0f;

        // Направление из позиции
        Vector3 currPos = position - currentSegment.a;
        Vector3 segmentDirection = currentSegment.b - currentSegment. a ;
        segmentDirection.Normalize() ;

        // Нахождение точки в сегменте с помощью проекции вектора
        Vector3 pointInSegment = Vector3.Project(currPos, segmentDirection);

        // Следующая позиция на линии маршрута
        float param = 0f;
        param = tmpParam - Vector3.Distance(currentSegment.a,
            currentSegment.b) ;
        param += pointInSegment.magnitude;
        return param;
    }

    public Vector3 GetPosition(float param) {
        // По текущему местоположению находим соответствующий сегмент
        AIPathSegment curSegment = null;
        float tmpParam = 0f;
        foreach (AIPathSegment segment in segments) {
            tmpParam += Vector3.Distance(segment.a, segment.b);
            if (param <= tmpParam) {
                curSegment = segment;
                break;
            }
        }
        if (curSegment == null)
            return Vector3.zero;
        
        // Преобразование параметра в позицию
        Vector3 segmentDir = (curSegment.b - curSegment.a).normalized;
        tmpParam -= Vector3.Distance(curSegment.a, curSegment.b);
        tmpParam = param - tmpParam;
        return curSegment.a + segmentDir * tmpParam;
    }

    private void OnDrawGizmos() {
        Color prevColor = Gizmos.color;
        Gizmos.color = Color.magenta;
        for (int i = 0; i < nodes.Count - 1; i++) {
            Vector3 src = nodes[i].transform.position;
            Vector3 dst = nodes[i + 1].transform.position;
            Vector3 dir = dst - src;
            Gizmos.DrawRay(src, dir);
        }
        Gizmos.color = prevColor;
    }
}
