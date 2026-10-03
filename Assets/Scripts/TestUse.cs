using Jing.Data;
using UnityEngine;
using VContainer;

public class TestUse : MonoBehaviour
{
    private Rect windowRect = new Rect(50, 50, 350, 120);
    private GUIStyle style;

    #region ::: Inject :::
    private Data_Uesr data_uesr;
    private IAIResume aiResume;


    [Inject]
    public void Construct(Data_Uesr data_uesr, IAIResume aiResume)
    {
        this.data_uesr = data_uesr;
        this.aiResume = aiResume;
    }

    #endregion
    private void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.button);
            style.fontSize = 40;
        }
        windowRect = GUI.Window(0, windowRect, DrawWindowContent, "Test Use");
    }

    private void DrawWindowContent(int windowID)
    {
        if (GUI.Button(new Rect(20, 35, 300, 50), "Re-fetch Resume Information"))
        {
            RefetchResume();
        }


        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    private void RefetchResume()
    {


    }
}

