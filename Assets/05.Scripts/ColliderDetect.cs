using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.IsGameover) return;
        
        LivingEntity otherEntity = null;
        LivingEntity parentEntity = null;
        if (other.transform != null)
            otherEntity = (LivingEntity)other.GetComponent<LivingEntity>();
        if(transform.parent != null)
         parentEntity = gameObject.transform.parent.GetComponent<LivingEntity>();
        
        if (otherEntity is not null && parentEntity is not null)
        {
            Debug.Log($"{gameObject.name}이 {other.name} 을 공격하였음.");
            otherEntity.OnDamage(parentEntity.power);
        }
        else
        {
            Debug.Log($"{gameObject.transform.parent.name}-{gameObject.name}이 {other.name}을 공격하였다.");
            if (otherEntity is null) Debug.Log($"{other.name}의 LivingEntity를 찾을 수 없습니다.");
            else Debug.Log($"{gameObject.transform.parent.name}의 LivingEntity를 찾을 수 없습니다.");
        }
    }
}
