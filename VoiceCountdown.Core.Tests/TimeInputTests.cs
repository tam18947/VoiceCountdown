using VoiceCountdown.Core.Input;

namespace VoiceCountdown.Core.Tests;

public class TimeInputTests
{
    [Theory]
    [InlineData("5", "00:05:00")]
    [InlineData("90", "01:30:00")]
    [InlineData("4:30", "00:04:30")]
    [InlineData("0:30", "00:00:30")]
    [InlineData("1:00:00", "01:00:00")]
    [InlineData(" 10 : 00 ", "00:10:00")]
    [InlineData("５：００", "00:05:00")]
    [InlineData("99:59:59", "4.03:59:59")]
    public void 期間として解釈できる入力(string text, string expected)
    {
        Assert.True(TimeInput.TryParseDuration(text, out var value));
        Assert.Equal(TimeSpan.Parse(expected), value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("0")]
    [InlineData("0:00")]
    [InlineData("1:60")]
    [InlineData("1:60:00")]
    [InlineData("1:2:3:4")]
    [InlineData("abc")]
    [InlineData("-5")]
    [InlineData("1.5")]
    [InlineData("5:")]
    [InlineData("100:00:00")]
    public void 期間として解釈できない入力(string? text)
    {
        Assert.False(TimeInput.TryParseDuration(text, out _));
    }

    [Theory]
    [InlineData("15", "15:00:00")]
    [InlineData("15:30", "15:30:00")]
    [InlineData("9:05:10", "09:05:10")]
    [InlineData("0:00", "00:00:00")]
    [InlineData("１５：００", "15:00:00")]
    public void 時刻として解釈できる入力(string text, string expected)
    {
        Assert.True(TimeInput.TryParseTimeOfDay(text, out var value));
        Assert.Equal(TimeSpan.Parse(expected), value);
    }

    [Theory]
    [InlineData("24:00")]
    [InlineData("12:60")]
    [InlineData("12:00:60")]
    [InlineData("")]
    [InlineData("noon")]
    public void 時刻として解釈できない入力(string text)
    {
        Assert.False(TimeInput.TryParseTimeOfDay(text, out _));
    }

    [Theory]
    [InlineData("00:05:00", "5:00")]
    [InlineData("00:00:30", "0:30")]
    [InlineData("01:30:00", "1:30:00")]
    public void 期間は入力欄の形式で表示する(string value, string expected)
    {
        Assert.Equal(expected, TimeInput.FormatDuration(TimeSpan.Parse(value)));
    }

    [Theory]
    [InlineData("15:00:00", "15:00")]
    [InlineData("09:30:15", "9:30:15")]
    public void 時刻は入力欄の形式で表示する(string value, string expected)
    {
        Assert.Equal(expected, TimeInput.FormatTimeOfDay(TimeSpan.Parse(value)));
    }
}
