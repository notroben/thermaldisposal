using System;

public static class GameEvents
{
    public static Action OnFurnaceActivated;
    public static Action<bool> OnFurnaceDoorToggled;
    public static Action<string> OnTriggerGameOver;
}
