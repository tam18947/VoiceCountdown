namespace VoiceCountdown.Core.Countdown;

public static class CountdownConfig
{
    /// <summary>
    /// 既定の読み上げタイミング。残り時間の長い順に並べる。
    /// 「2」「1」は起動時はチェックを外しておく（従来どおり）。
    /// </summary>
    public static readonly IReadOnlyList<CountdownItem> DefaultItems =
    [
        new("10分前", TimeSpan.FromMinutes(10), "timer_10punmae_01"),
        new("5分前",  TimeSpan.FromMinutes(5),  "timer_5funmae_01"),
        new("4分前",  TimeSpan.FromMinutes(4),  "timer_4punmae_01"),
        new("3分前",  TimeSpan.FromMinutes(3),  "timer_3punmae_01"),
        new("2分前",  TimeSpan.FromMinutes(2),  "timer_2funmae_01"),
        new("1分前",  TimeSpan.FromMinutes(1),  "timer_1punmae_01"),
        new("30秒前", TimeSpan.FromSeconds(30), "30byoumae"),
        new("10秒前", TimeSpan.FromSeconds(10), "10byoumae"),
        new("5",      TimeSpan.FromSeconds(5),  "num005_01"),
        new("4",      TimeSpan.FromSeconds(4),  "num004_01"),
        new("3",      TimeSpan.FromSeconds(3),  "num003_01"),
        new("2",      TimeSpan.FromSeconds(2),  "num002_01", EnabledByDefault: false),
        new("1",      TimeSpan.FromSeconds(1),  "num001_01", EnabledByDefault: false),
    ];
}
