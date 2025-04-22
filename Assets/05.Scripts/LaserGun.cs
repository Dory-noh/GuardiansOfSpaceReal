using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform FirePos;
    //public GameObject bulletPrefab;
    private float power = 15f;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        FirePos = transform.GetChild(3).transform;
    }

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirePos.position, transform.forward, out hit, 100f))
        {
            Debug.Log($"{hit.transform.gameObject.name} 감지 성공");
            if (hit.transform.CompareTag("Enemy"))
            {
                
                LivingEntity enemy = hit.transform.gameObject.transform.GetComponent<LivingEntity>();

                if(enemy != null)
                {
                    Debug.Log($"{hit.transform. name} 공격 성공");
                    enemy.OnDamage(power);
                }
                else
                {
                    Debug.Log($"총알 공격 실패");
                }
                
            }
            StartCoroutine(ShowLaser(hit.point));
        }
    }

    IEnumerator ShowLaser(Vector3 hitPoint)
    {
        lineRenderer.SetPosition(0, FirePos.position);
        lineRenderer.SetPosition(1, hitPoint);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(1f);

        lineRenderer.enabled = false;
    }
}
