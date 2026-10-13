using VoiceCountdown.Core.Countdown;

namespace VoiceCountdown.Desktop.ViewModels;

/// <summary>読み上げタイミング一覧の 1 行。</summary>
public sealed class TriggerItemViewModel(CountdownItem item, bool isEnabled) : ObservableObject
{
    private bool isEnabled = isEnabled;
    private bool isLastTriggered;

    public CountdownItem Item { get; } = item;

    public string Label => Item.Label;

    public bool IsEnabled
    {
        get => isEnabled;
        set => SetProperty(ref isEnabled, value);
    }

    /// <summary>直前に通過したタイミングなら true（一覧で強調表示する）。</summary>
    public bool IsLastTriggered
    {
        get => isLastTriggered;
        set => SetProperty(ref isLastTriggered, value);
    }
}
