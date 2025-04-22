using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveOBJ : MonoBehaviour, IItem
{
    public ItemData itemData; // Inspector에서 할당

    public void AddItem(ItemData item)
    {
        UIManager.Instance.UpdateQuest(item); //퀘스트 UI업데이트
        if (int.Parse(item.itemID) < 5)
        {
            UIManager.Instance.UpdateItemIcons(item); // UI 업데이트
            gameObject.SetActive(false); //아이템 획득 후 아이템 비 활성화
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
