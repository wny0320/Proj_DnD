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
        //이 코드를 합친 것이 GetWindow<Type>();
        //()안에 type을 넣으면 원래 윈도우에 추가 가능 ex) typeof(SceneView)

        //Show = 기본 기능의 탭 윈도우
        //Showutility 탭 윈도우로 사용되지 않고 항상 앞에 표시되는 윈도우
    }
    // 팝업의 인스턴스화

    private void OnGUI()
    {
        //ESC 누를때 닫기
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
        //열릴때 호출
    }
    public override void OnClose()
    {
        //닫힐때 호출
    }
    public override UnityEngine.Vector2 GetWindowSize()
    {
        //popup size
        return new UnityEngine.Vector2(300, 100);
    }
}
