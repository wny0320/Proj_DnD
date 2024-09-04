using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Subsystems;

public class TestWindow : EditorWindow
{
    static TestWindow testWindow;
    [MenuItem("Window/testWindow")]
    static void Open()
    {
        if (testWindow == null)
        {
            testWindow = CreateInstance<TestWindow>();
        }
        testWindow.ShowUtility();
        //�� �ڵ带 ��ģ ���� GetWindow<Type>();
        //()�ȿ� type�� ������ ���� �����쿡 �߰� ���� ex) typeof(SceneView)

        //Show = �⺻ ����� �� ������
        //Showutility �� ������� ������ �ʰ� �׻� �տ� ǥ�õǴ� ������
    }
    // �˾��� �ν��Ͻ�ȭ

    private void OnGUI()
    {
        //ESC ������ �ݱ�
        if(Event.current.keyCode == KeyCode.Escape)
        {
            testWindow.Close();
        }
    }
}
public class Popup : PopupWindowContent
{
    public override void OnGUI(Rect rect)
    {
        EditorGUILayout.LabelField("Label");
    }
    public override void OnOpen()
    {
        //������ ȣ��
    }
    public override void OnClose()
    {
        //������ ȣ��
    }
    public override UnityEngine.Vector2 GetWindowSize()
    {
        //popup size
        return new UnityEngine.Vector2(300, 100);
    }
}
