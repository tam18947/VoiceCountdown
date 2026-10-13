using VoiceCountdown.Core.Countdown;

namespace VoiceCountdown.Core.Tests;

public class CountdownServiceTests
{
    private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(100);

    private readonly ManualTimeProvider time = new();
    private readonly List<int> triggered = [];
    private int completedCount;

    private CountdownService CreateService(params TimeSpan[] triggers)
    {
        var service = new CountdownService(triggers, time);
        service.Triggered += triggered.Add;
        service.Completed += () => completedCount++;
        return service;
    }

    /// <summary>100ms ごとに Tick を呼びながら時間を進める（UI のタイマーと同じ動き）。</summary>
    private void Run(CountdownService service, TimeSpan duration)
    {
        for (var t = TimeSpan.Zero; t < duration; t += TickInterval)
        {
            time.Advance(TickInterval);
            service.Tick();
        }
    }

    [Fact]
    public void 読み上げタイミングを通過したら一度だけ通知される()
    {
        var service = CreateService(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10));
        service.Start(TimeSpan.FromMinutes(1));

        Run(service, TimeSpan.FromSeconds(29.95));
        Assert.Empty(triggered);

        Run(service, TimeSpan.FromSeconds(0.2));
        Assert.Equal(new[] { 0 }, triggered);

        Run(service, TimeSpan.FromSeconds(20));
        Assert.Equal(new[] { 0, 1 }, triggered);
    }

    [Fact]
    public void 既定の全タイミングが順番に通知され最後に完了する()
    {
        var triggers = CountdownConfig.DefaultItems.Select(i => i.TriggerTime).ToArray();
        var service = CreateService(triggers);
        service.Start(TimeSpan.FromMinutes(15));

        Run(service, TimeSpan.FromMinutes(15) + TimeSpan.FromSeconds(1));

        Assert.Equal(Enumerable.Range(0, triggers.Length), triggered);
        Assert.Equal(1, completedCount);
        Assert.Equal(CountdownState.Idle, service.State);
    }

    [Fact]
    public void 開始時の残り時間より長いタイミングは通知されない()
    {
        var service = CreateService(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(1));
        service.Start(TimeSpan.FromMinutes(3));

        Run(service, TimeSpan.FromMinutes(3));

        Assert.Equal(new[] { 1 }, triggered);
    }

    [Fact]
    public void 開始時の残り時間と同じタイミングは最初のTickで通知される()
    {
        var service = CreateService(TimeSpan.FromMinutes(5));
        service.Start(TimeSpan.FromMinutes(5));

        Run(service, TickInterval);

        Assert.Equal(new[] { 0 }, triggered);
    }

    [Fact]
    public void 複数のタイミングを一度に通過したら最も残り時間の短いものだけ通知される()
    {
        var service = CreateService(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(3));
        service.Start(TimeSpan.FromMinutes(10));

        // スリープ復帰などで Tick が長時間呼ばれなかった場合
        time.Advance(TimeSpan.FromMinutes(7.5));
        service.Tick();

        Assert.Equal(new[] { 2 }, triggered);
    }

    [Fact]
    public void 一時停止中は時間が進まず再開後は続きから数える()
    {
        var service = CreateService();
        service.Start(TimeSpan.FromMinutes(1));

        Run(service, TimeSpan.FromSeconds(20));
        service.Pause();
        time.Advance(TimeSpan.FromMinutes(5));
        service.Tick();

        Assert.Equal(CountdownState.Paused, service.State);
        Assert.Equal(TimeSpan.FromSeconds(40), service.Remaining);

        service.Resume();
        Run(service, TimeSpan.FromSeconds(10));

        Assert.Equal(TimeSpan.FromSeconds(30), service.Remaining);
        Assert.Equal(0, completedCount);
    }

    [Fact]
    public void 一時停止中に通過したことにはならない()
    {
        var service = CreateService(TimeSpan.FromSeconds(30));
        service.Start(TimeSpan.FromMinutes(1));

        Run(service, TimeSpan.FromSeconds(20));
        service.Pause();
        time.Advance(TimeSpan.FromMinutes(1));
        service.Resume();
        Run(service, TimeSpan.FromSeconds(5));

        Assert.Empty(triggered);
    }

    [Fact]
    public void 残り時間が0になると完了して停止状態に戻る()
    {
        var service = CreateService();
        var states = new List<CountdownState>();
        service.StateChanged += states.Add;

        service.Start(TimeSpan.FromSeconds(3));
        Run(service, TimeSpan.FromSeconds(3.2));

        Assert.Equal(1, completedCount);
        Assert.Equal(new[] { CountdownState.Running, CountdownState.Idle }, states);

        // 完了後に Tick が呼ばれても何も起きない
        Run(service, TimeSpan.FromSeconds(1));
        Assert.Equal(1, completedCount);
    }

    [Fact]
    public void リセットすると停止状態になり残り時間0が通知される()
    {
        var service = CreateService();
        TimeSpan? last = null;
        service.TimeUpdated += ts => last = ts;

        service.Start(TimeSpan.FromMinutes(1));
        Run(service, TimeSpan.FromSeconds(5));
        service.Reset();

        Assert.Equal(CountdownState.Idle, service.State);
        Assert.Equal(TimeSpan.Zero, last);
        Assert.Equal(0, completedCount);
    }

    [Fact]
    public void 一時停止から再開しても完了は一度だけ()
    {
        var service = CreateService();
        service.Start(TimeSpan.FromSeconds(2));

        Run(service, TimeSpan.FromSeconds(1));
        service.Pause();
        service.Resume();
        Run(service, TimeSpan.FromSeconds(2));

        Assert.Equal(1, completedCount);
    }

    [Fact]
    public void 時間が0以下なら開始できない()
    {
        var service = CreateService();

        Assert.Throws<ArgumentOutOfRangeException>(() => service.Start(TimeSpan.Zero));
        Assert.Equal(CountdownState.Idle, service.State);
    }

    [Theory]
    [InlineData(600.0, "10:00")]
    [InlineData(599.9, "10:00")]
    [InlineData(599.0, "09:59")]
    [InlineData(0.05, "00:01")]
    [InlineData(0.0, "00:00")]
    [InlineData(-1.0, "00:00")]
    [InlineData(3600.0, "1:00:00")]
    [InlineData(3599.5, "1:00:00")]
    [InlineData(86399.0, "23:59:59")]
    public void 残り時間は秒未満を切り上げて表示する(double seconds, string expected)
    {
        Assert.Equal(expected, CountdownService.Format(TimeSpan.FromSeconds(seconds)));
    }

    [Theory]
    [InlineData("15:00:00", "14:00:00", "01:00:00")]
    [InlineData("15:00:00", "15:00:00", "1.00:00:00")]
    [InlineData("09:00:00", "23:30:00", "09:30:00")]
    public void 時刻指定は次にその時刻になるまでの時間になる(string target, string now, string expected)
    {
        var result = CountdownService.DurationUntil(TimeSpan.Parse(target), DateTime.Today + TimeSpan.Parse(now));

        Assert.Equal(TimeSpan.Parse(expected), result);
    }
}
