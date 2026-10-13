namespace VoiceCountdown.Core.Tests;

/// <summary>
/// テスト用に時間を手動で進められる TimeProvider。
/// </summary>
internal sealed class ManualTimeProvider : TimeProvider
{
    private long timestamp;

    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    public override long GetTimestamp() => timestamp;

    public void Advance(TimeSpan delta) => timestamp += delta.Ticks;
}
