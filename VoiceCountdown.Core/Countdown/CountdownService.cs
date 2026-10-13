namespace VoiceCountdown.Core.Countdown;

/// <summary>
/// 残り時間の計算と、読み上げタイミングの判定を行う。
/// 時間の進行は <see cref="Tick"/> を定期的に呼び出すことで反映される。
/// </summary>
public class CountdownService
{
    private readonly TimeProvider timeProvider;
    private readonly TimeSpan[] triggerTimes;

    private TimeSpan duration;
    private TimeSpan elapsedBeforePause;
    private long runningSince;
    private TimeSpan previousRemaining;

    /// <summary>残り時間が更新されたとき。</summary>
    public event Action<TimeSpan>? TimeUpdated;

    /// <summary>読み上げタイミングを通過したとき。引数はトリガーのインデックス。</summary>
    public event Action<int>? Triggered;

    /// <summary>残り時間が 0 になったとき。</summary>
    public event Action? Completed;

    /// <summary>状態が変わったとき。</summary>
    public event Action<CountdownState>? StateChanged;

    public CountdownService(IEnumerable<TimeSpan> triggerTimes, TimeProvider? timeProvider = null)
    {
        this.triggerTimes = [.. triggerTimes];
        this.timeProvider = timeProvider ?? TimeProvider.System;
    }

    public CountdownState State { get; private set; } = CountdownState.Idle;

    /// <summary>最後に <see cref="Start"/> した時間。</summary>
    public TimeSpan Duration => duration;

    public TimeSpan Elapsed => State == CountdownState.Running
        ? elapsedBeforePause + timeProvider.GetElapsedTime(runningSince)
        : elapsedBeforePause;

    public TimeSpan Remaining => State == CountdownState.Idle
        ? TimeSpan.Zero
        : Max(duration - Elapsed, TimeSpan.Zero);

    /// <summary>
    /// 指定した時間でカウントダウンを開始する。
    /// 開始時点の残り時間と同じ読み上げタイミングは、最初の <see cref="Tick"/> で読み上げられる。
    /// </summary>
    public void Start(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration), "0 より大きい時間を指定してください。");

        this.duration = duration;
        elapsedBeforePause = TimeSpan.Zero;
        previousRemaining = duration;
        runningSince = timeProvider.GetTimestamp();
        SetState(CountdownState.Running);
        TimeUpdated?.Invoke(duration);
    }

    public void Pause()
    {
        if (State != CountdownState.Running) return;

        elapsedBeforePause += timeProvider.GetElapsedTime(runningSince);
        SetState(CountdownState.Paused);
    }

    public void Resume()
    {
        if (State != CountdownState.Paused) return;

        runningSince = timeProvider.GetTimestamp();
        SetState(CountdownState.Running);
    }

    public void Reset()
    {
        elapsedBeforePause = TimeSpan.Zero;
        SetState(CountdownState.Idle);
        TimeUpdated?.Invoke(TimeSpan.Zero);
    }

    public void Tick()
    {
        if (State != CountdownState.Running) return;

        var remaining = duration - Elapsed;

        // 前回の Tick から今回までの間に通過した読み上げタイミングのうち、最も残り時間の短いものを 1 つだけ通知する。
        // （スリープ復帰などで複数通過した場合に、まとめて再生されないようにするため）
        int hit = -1;
        for (int i = 0; i < triggerTimes.Length; i++)
        {
            var t = triggerTimes[i];
            if (remaining < t && t <= previousRemaining &&
                (hit < 0 || t < triggerTimes[hit]))
            {
                hit = i;
            }
        }
        previousRemaining = remaining;

        if (hit >= 0)
            Triggered?.Invoke(hit);

        if (remaining <= TimeSpan.Zero)
        {
            elapsedBeforePause = TimeSpan.Zero;
            SetState(CountdownState.Idle);
            TimeUpdated?.Invoke(TimeSpan.Zero);
            Completed?.Invoke();
            return;
        }

        TimeUpdated?.Invoke(remaining);
    }

    /// <summary>
    /// 現在時刻から、次に指定時刻（時:分:秒）になるまでの時間を返す。
    /// 指定時刻が現在時刻以前なら翌日の時刻とみなす。
    /// </summary>
    public static TimeSpan DurationUntil(TimeSpan targetTimeOfDay, DateTime now)
    {
        var ts = targetTimeOfDay - now.TimeOfDay;
        return ts > TimeSpan.Zero ? ts : ts + TimeSpan.FromDays(1);
    }

    /// <summary>
    /// 残り時間を表示用の文字列にする。秒未満は切り上げる（残り 9:59.9 は "10:00"）。
    /// </summary>
    public static string Format(TimeSpan remaining)
    {
        long seconds = Math.Max(0, (long)Math.Ceiling(remaining.TotalSeconds));
        var t = TimeSpan.FromSeconds(seconds);
        return t.TotalHours >= 1 ? $"{(int)t.TotalHours}:{t:mm\\:ss}" : t.ToString(@"mm\:ss");
    }

    private void SetState(CountdownState state)
    {
        if (State == state) return;
        State = state;
        StateChanged?.Invoke(state);
    }

    private static TimeSpan Max(TimeSpan a, TimeSpan b) => a > b ? a : b;
}
