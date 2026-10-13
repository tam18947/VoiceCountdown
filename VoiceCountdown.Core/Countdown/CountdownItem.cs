namespace VoiceCountdown.Core.Countdown;

/// <summary>
/// 読み上げタイミング 1 件分の定義。
/// </summary>
/// <param name="Label">一覧に表示する名前（例: "10分前"）。</param>
/// <param name="TriggerTime">残り時間がこの値を下回ったときに読み上げる。</param>
/// <param name="SoundKey">再生する音声のリソース名。</param>
/// <param name="EnabledByDefault">起動時に読み上げ対象としてチェックするか。</param>
public sealed record CountdownItem(string Label, TimeSpan TriggerTime, string SoundKey, bool EnabledByDefault = true);
