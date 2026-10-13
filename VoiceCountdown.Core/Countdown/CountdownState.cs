namespace VoiceCountdown.Core.Countdown;

public enum CountdownState
{
    /// <summary>停止中（未開始・リセット後・終了後）。</summary>
    Idle,
    /// <summary>カウントダウン中。</summary>
    Running,
    /// <summary>一時停止中。</summary>
    Paused,
}
