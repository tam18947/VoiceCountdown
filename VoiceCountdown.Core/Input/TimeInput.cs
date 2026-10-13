using System.Globalization;
using System.Text;

namespace VoiceCountdown.Core.Input;

/// <summary>
/// タイマーの時間入力（テキスト）の解析と整形。全角の数字やコロンも受け付ける。
/// </summary>
public static class TimeInput
{
    /// <summary>期間として入力できる最大値。</summary>
    public static readonly TimeSpan MaxDuration = new(99, 59, 59);

    /// <summary>
    /// 期間を解析する。
    /// "5" → 5 分、"4:30" → 4 分 30 秒、"1:00:00" → 1 時間。0 より大きい値のみ有効。
    /// </summary>
    public static bool TryParseDuration(string? text, out TimeSpan value)
    {
        value = TimeSpan.Zero;
        if (!TrySplit(text, out var parts)) return false;

        TimeSpan result;
        switch (parts.Length)
        {
            case 1:
                if (parts[0] > MaxDuration.TotalMinutes) return false;
                result = TimeSpan.FromMinutes(parts[0]);
                break;
            case 2:
                if (parts[1] >= 60) return false;
                result = new TimeSpan(0, parts[0], parts[1]);
                break;
            case 3:
                if (parts[1] >= 60 || parts[2] >= 60) return false;
                result = new TimeSpan(parts[0], parts[1], parts[2]);
                break;
            default:
                return false;
        }

        if (result <= TimeSpan.Zero || result > MaxDuration) return false;

        value = result;
        return true;
    }

    /// <summary>
    /// 時刻を解析する。"15" → 15:00:00、"15:30" → 15:30:00、"9:30:15" → 9:30:15。
    /// </summary>
    public static bool TryParseTimeOfDay(string? text, out TimeSpan value)
    {
        value = TimeSpan.Zero;
        if (!TrySplit(text, out var parts)) return false;

        int h = parts[0];
        int m = parts.Length > 1 ? parts[1] : 0;
        int s = parts.Length > 2 ? parts[2] : 0;

        if (parts.Length > 3 || h >= 24 || m >= 60 || s >= 60) return false;

        value = new TimeSpan(h, m, s);
        return true;
    }

    /// <summary>期間を入力欄に表示する形にする（"5:00"、"1:30:00"）。</summary>
    public static string FormatDuration(TimeSpan value) =>
        value.TotalHours >= 1
            ? $"{(int)value.TotalHours}:{value:mm\\:ss}"
            : $"{(int)value.TotalMinutes}:{value:ss}";

    /// <summary>時刻を入力欄に表示する形にする（"15:00"、"9:30:15"）。</summary>
    public static string FormatTimeOfDay(TimeSpan value) =>
        value.Seconds == 0
            ? $"{value.Hours}:{value:mm}"
            : $"{value.Hours}:{value:mm\\:ss}";

    private static bool TrySplit(string? text, out int[] parts)
    {
        parts = [];
        if (string.IsNullOrWhiteSpace(text)) return false;

        // 全角数字・全角コロンを半角にそろえる
        var normalized = text.Normalize(NormalizationForm.FormKC).Trim();

        var tokens = normalized.Split(':');
        var result = new int[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i].Trim();
            if (token.Length == 0 || !token.All(char.IsAsciiDigit)) return false;
            if (!int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out result[i])) return false;
        }

        parts = result;
        return true;
    }
}
