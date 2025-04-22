using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName; //아이템 이름
    public string itemID; //아이템 데이터
}