using System.Text.Json;

namespace VoiceCountdown.Core.Settings;

/// <summary>
/// 設定を JSON ファイルに読み書きする。読めない場合は既定値を返す。
/// </summary>
public sealed class SettingsStore(string filePath)
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public string FilePath { get; } = filePath;

    /// <summary>%AppData%\VoiceCountdown\settings.json を使うストア。</summary>
    public static SettingsStore CreateDefault() => new(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "VoiceCountdown",
        "settings.json"));

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new AppSettings();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json, Options) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return new AppSettings();
        }
    }

    /// <summary>保存する。失敗しても例外は投げず false を返す。</summary>
    public bool Save(AppSettings settings)
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            // 書き込み途中で終了しても壊れないよう、一時ファイルに書いてから置き換える
            var temp = FilePath + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(settings, Options));
            File.Move(temp, FilePath, overwrite: true);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}
