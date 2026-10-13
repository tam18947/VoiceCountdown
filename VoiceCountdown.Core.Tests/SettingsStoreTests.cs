using VoiceCountdown.Core.Settings;

namespace VoiceCountdown.Core.Tests;

public sealed class SettingsStoreTests : IDisposable
{
    private readonly string dir = Path.Combine(Path.GetTempPath(), "VoiceCountdownTests", Guid.NewGuid().ToString("N"));

    private string FilePath => Path.Combine(dir, "settings.json");

    public void Dispose()
    {
        if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
    }

    [Fact]
    public void 保存した設定を読み込める()
    {
        var store = new SettingsStore(FilePath);
        var settings = new AppSettings
        {
            IsAlarmMode = true,
            DurationText = "3:00",
            AlarmText = "9:30",
            EnabledTriggerSeconds = [600, 60, 10],
            DeviceName = "スピーカー",
            FontFamily = "Meiryo",
            Topmost = true,
            IsSettingsPaneOpen = false,
            WindowWidth = 800,
            WindowHeight = 500,
        };

        Assert.True(store.Save(settings));
        var loaded = store.Load();

        Assert.True(loaded.IsAlarmMode);
        Assert.Equal("3:00", loaded.DurationText);
        Assert.Equal("9:30", loaded.AlarmText);
        Assert.Equal(new[] { 600, 60, 10 }, loaded.EnabledTriggerSeconds);
        Assert.Equal("スピーカー", loaded.DeviceName);
        Assert.Equal("Meiryo", loaded.FontFamily);
        Assert.True(loaded.Topmost);
        Assert.False(loaded.IsSettingsPaneOpen);
        Assert.Equal(800, loaded.WindowWidth);
        Assert.Equal(500, loaded.WindowHeight);
    }

    [Fact]
    public void ファイルがなければ既定値になる()
    {
        var loaded = new SettingsStore(FilePath).Load();

        Assert.False(loaded.IsAlarmMode);
        Assert.Equal("5:00", loaded.DurationText);
        Assert.Null(loaded.EnabledTriggerSeconds);
        Assert.True(loaded.IsSettingsPaneOpen);
    }

    [Fact]
    public void 壊れたファイルなら既定値になる()
    {
        Directory.CreateDirectory(dir);
        File.WriteAllText(FilePath, "{ this is not json");

        var loaded = new SettingsStore(FilePath).Load();

        Assert.Equal("5:00", loaded.DurationText);
    }

    [Fact]
    public void 上書き保存できる()
    {
        var store = new SettingsStore(FilePath);

        store.Save(new AppSettings { DurationText = "1:00" });
        store.Save(new AppSettings { DurationText = "2:00" });

        Assert.Equal("2:00", store.Load().DurationText);
        Assert.False(File.Exists(FilePath + ".tmp"));
    }
}
