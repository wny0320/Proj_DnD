using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool emptyFlag; // ������ ������ �������
    public bool mainSlotFlag; // �������� ���� ���� ��ĭ, �� ������ ������ ����� ĭ����
    public Vector2Int itemDataPos = -Vector2Int.one; // �ش� �������� ������ ����� ĭ�� ������ ��� ����, �������� ���� �� �����Ƿ� �ʱⰪ�� ������ ����
}
