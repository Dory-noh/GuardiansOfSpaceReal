using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLaser : MonoBehaviour
{
    [SerializeField] private Transform FirePos;
    [SerializeField] private Transform target;
    [SerializeField] private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = FirePos.GetComponent<LineRenderer>();
        target = GameObject.FindWithTag("Player")?.transform;
    }

    public void Fire()
    {
        if (target == null) return;
        if(Physics.Raycast(FirePos.position, target.position - FirePos.position, out RaycastHit hitInfo, 100f, 1 << 7))
        {
            StartCoroutine(ShowLaser(hitInfo));
        }
    }

    IEnumerator ShowLaser(RaycastHit hitInfo)
    {
        lineRenderer.SetPosition(0, FirePos.position);
        lineRenderer.SetPosition(1, hitInfo.point+new Vector3(0f,-1f,-1f));
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(1f);

        lineRenderer.enabled = false;
    }
}
