using System;
using UniRx;
public static class GameSettings
{
    public static float GridUnitSize = 1f;
    public static ReactiveProperty<float> GlobalTime = new ReactiveProperty<float>(1);

    public static float ProgressGlobalTimeBySpeed(float speed)
    {
        GlobalTime.Value += 1 / speed;
        return GlobalTime.Value;
    }
}
