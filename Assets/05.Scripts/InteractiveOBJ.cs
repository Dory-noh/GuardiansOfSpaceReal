using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveOBJ : MonoBehaviour, IItem
{
    public ItemData itemData; // Inspector���� �Ҵ�

    public void AddItem(ItemData item)
    {
        UIManager.Instance.UpdateQuest(item); //����Ʈ UI������Ʈ
        if (int.Parse(item.itemID) < 5)
        {
            UIManager.Instance.UpdateItemIcons(item); // UI ������Ʈ
            gameObject.SetActive(false); //������ ȹ�� �� ������ �� Ȱ��ȭ
        }
    }


    public void Use(GameObject target)
    {
        AddItem(itemData);
}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ToggleHelpUI(0, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            UIManager.Instance.ToggleHelpUI(0, false);
    }

    private void OnDisable()
    {
        UIManager.Instance.ToggleHelpUI(0, false);
    }
}
