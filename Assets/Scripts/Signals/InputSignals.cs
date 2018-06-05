using UnityEngine;

public static class InputSignals
{
    public static event UiEnterHandler OnUiEnter;
    public delegate void UiEnterHandler();
    public static void DoUiEnter() { if (OnUiEnter != null) OnUiEnter(); }

    public static event UiExitHandler OnUiExit;
    public delegate void UiExitHandler();
    public static void DoUiExit() { if (OnUiExit != null) OnUiExit(); }

    public static event UiDragBeginHandler OnUiDragBegin;
    public delegate void UiDragBeginHandler();
    public static void DoUiDragBegin() { if (OnUiDragBegin != null) OnUiDragBegin(); }

    public static event UiDragEndHandler OnUiDragEnd;
    public delegate void UiDragEndHandler();
    public static void DoUiDragEnd() { if (OnUiDragEnd != null) OnUiDragEnd(); }
}
