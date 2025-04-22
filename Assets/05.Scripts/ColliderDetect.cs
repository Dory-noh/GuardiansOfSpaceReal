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
            Debug.Log($"{gameObject.name}�� {other.name} �� �����Ͽ���.");
            otherEntity.OnDamage(parentEntity.power);
        }
        else
        {
            Debug.Log($"{gameObject.transform.parent.name}-{gameObject.name}�� {other.name}�� �����Ͽ���.");
            if (otherEntity is null) Debug.Log($"{other.name}�� LivingEntity�� ã�� �� �����ϴ�.");
            else Debug.Log($"{gameObject.transform.parent.name}�� LivingEntity�� ã�� �� �����ϴ�.");
        }
    }
}
