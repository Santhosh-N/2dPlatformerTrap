using System;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    public float range = 8f;
    public LayerMask enemyLayer;
    [HideInInspector] public Transform currentTarget;
    [HideInInspector] public bool hasTarget = false;

    public Action OnTargetFound;

    public void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        Transform nearest = null;
        float nearestDist = Mathf.Infinity;


        if (hits.Length != 0)
        {
            if (!hasTarget) OnTargetFound?.Invoke();
            hasTarget = true;
            foreach (var hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = hit.transform;
                }
            }
        }
        else
        {
            hasTarget = false;
        }

        currentTarget = nearest;
    }
}
