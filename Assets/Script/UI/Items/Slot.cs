using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool emptyFlag; // 아이템 슬롯이 비었는지
    public bool mainSlotFlag; // 아이템의 제일 왼쪽 위칸, 즉 아이템 정보가 저장된 칸인지

    // 해당 아이템의 정보가 저장된 칸의 정보를 담고 있음
    // 음수값이 나올 수 없으므로 초기값은 음수로 지정, x좌표값이 실제론 y 좌표(Row)값
    public Vector2Int itemDataPos = -Vector2Int.one;
    // 해당 슬롯이 가지고 있는 아이템
    public Item slotItem;
}
