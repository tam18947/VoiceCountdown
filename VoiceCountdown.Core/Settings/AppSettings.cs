namespace VoiceCountdown.Core.Settings;

/// <summary>
/// 次回起動時に復元する設定。
/// </summary>
public sealed class AppSettings
{
    /// <summary>true なら時刻指定、false なら期間指定。</summary>
    public bool IsAlarmMode { get; set; }

    /// <summary>期間の入力値（例: "5:00"）。</summary>
    public string DurationText { get; set; } = "5:00";

    /// <summary>時刻の入力値（例: "15:00"）。</summary>
    public string AlarmText { get; set; } = "15:00";

    /// <summary>読み上げを有効にしているタイミング（秒）。null なら既定値を使う。</summary>
    public List<int>? EnabledTriggerSeconds { get; set; }

    /// <summary>出力デバイス名。null なら既定のデバイス。</summary>
    public string? DeviceName { get; set; }

    /// <summary>残り時間の表示フォント。null なら既定のフォント。</summary>
    public string? FontFamily { get; set; }

    /// <summary>常に手前に表示するか。</summary>
    public bool Topmost { get; set; }

    /// <summary>設定パネルを開いているか。</summary>
    public bool IsSettingsPaneOpen { get; set; } = true;

    public double? WindowWidth { get; set; }

    public double? WindowHeight { get; set; }
}
