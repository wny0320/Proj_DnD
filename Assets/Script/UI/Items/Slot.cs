using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool emptyFlag; // ������ ������ �������
    public bool mainSlotFlag; // �������� ���� ���� ��ĭ, �� ������ ������ ����� ĭ����

    // �ش� �������� ������ ����� ĭ�� ������ ��� ����
    // �������� ���� �� �����Ƿ� �ʱⰪ�� ������ ����, x��ǥ���� ������ y ��ǥ(Row)��
    public Vector2Int itemDataPos = -Vector2Int.one;
    // �ش� ������ ������ �ִ� ������
    public Item slotItem;
}
