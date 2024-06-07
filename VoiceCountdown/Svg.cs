using Svg;

namespace VoiceCountdown
{
    internal static class Svg
    {
        public static Image? GetImage(Size size, Stream? stream)
        {
            if (size.Width <= 0) { return null; }
            if (size.Height <= 0) { return null; }
            if (stream is null) { return null; }

            Bitmap bmp = new(size.Width, size.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 外縁を描画
                int minSize = Math.Min(size.Width, size.Height);

                // SVGファイルの読み込み
                SvgDocument svgDocument = SvgDocument.Open<SvgDocument>(stream);
                // 移動とスケーリング変換を適用
                g.TranslateTransform((size.Width - minSize) / 2, (size.Height - minSize) / 2);
                // スケーリング変換を適用
                g.ScaleTransform(minSize / 128F, minSize / 128F);
                // 描画
                svgDocument.Draw(g);
            }
            return bmp;
        }
    }
}
