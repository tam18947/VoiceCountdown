using Svg;
using VoiceCountdown.Properties;

namespace VoiceCountdown;

internal enum ButtonIcon
{
    Play,
    Pause,
    Stop,
}

/// <summary>
/// ボタン用の SVG アイコンを指定サイズの画像にする。SVG の解析結果はキャッシュする。
/// </summary>
internal static class SvgIcons
{
    // SVG の viewBox は 0 0 128 128
    private const float ViewBoxSize = 128f;

    private static readonly Dictionary<ButtonIcon, SvgDocument> cache = [];

    /// <summary>
    /// 縦横比を保ったまま中央に描画した画像を返す。サイズが 0 以下なら null。
    /// 返した画像の破棄は呼び出し側の責任。
    /// </summary>
    public static Bitmap? Render(ButtonIcon icon, Size size)
    {
        if (size.Width <= 0 || size.Height <= 0) return null;

        var doc = GetDocument(icon);
        var bmp = new Bitmap(size.Width, size.Height);

        using var g = Graphics.FromImage(bmp);
        float side = Math.Min(size.Width, size.Height);
        g.TranslateTransform((size.Width - side) / 2f, (size.Height - side) / 2f);
        g.ScaleTransform(side / ViewBoxSize, side / ViewBoxSize);
        doc.Draw(g);

        return bmp;
    }

    private static SvgDocument GetDocument(ButtonIcon icon)
    {
        if (cache.TryGetValue(icon, out var doc)) return doc;

        byte[] bytes = icon switch
        {
            ButtonIcon.Play => Resources.Play,
            ButtonIcon.Pause => Resources.Pause,
            ButtonIcon.Stop => Resources.Stop,
            _ => throw new ArgumentOutOfRangeException(nameof(icon)),
        };

        using var stream = new MemoryStream(bytes);
        doc = SvgDocument.Open<SvgDocument>(stream);
        cache[icon] = doc;
        return doc;
    }
}
