using VoiceCountdown.Core.Countdown;

namespace VoiceCountdown.Core.Tests;

public class CountdownConfigTests
{
    [Fact]
    public void 既定の読み上げタイミングは残り時間の長い順に重複なく並ぶ()
    {
        var times = CountdownConfig.DefaultItems.Select(i => i.TriggerTime).ToArray();

        Assert.Equal(times.OrderByDescending(t => t), times);
        Assert.Equal(times.Length, times.Distinct().Count());
    }

    [Fact]
    public void 起動時は2と1以外にチェックが付く()
    {
        var disabled = CountdownConfig.DefaultItems.Where(i => !i.EnabledByDefault).Select(i => i.Label);

        Assert.Equal(new[] { "2", "1" }, disabled);
    }
}
