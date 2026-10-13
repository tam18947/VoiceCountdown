using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;
using VoiceCountdown.Desktop.Services;
using VoiceCountdown.Core.Countdown;
using VoiceCountdown.Core.Input;
using VoiceCountdown.Core.Settings;

namespace VoiceCountdown.Desktop.ViewModels;

/// <summary>残り時間の段階。文字色の切り替えに使う。</summary>
public enum TimeLevel
{
    Normal,
    Warning,
    Critical,
    Finished,
}

public sealed class MainViewModel : ObservableObject, IDisposable
{
    public const string DefaultDeviceLabel = "既定のデバイス";
    public const string DefaultFontLabel = "既定のフォント";
    private const string DefaultFontFamily = "Segoe UI Variable Display, Segoe UI, Yu Gothic UI";

    private const string PlayGlyph = "";
    private const string PauseGlyph = "";

    private static readonly TimeSpan WarningThreshold = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan CriticalThreshold = TimeSpan.FromSeconds(10);

    private readonly SettingsStore store;
    private readonly CountdownService countdown;
    private readonly DispatcherTimer timer;
    private readonly AudioPlayer audio = new();

    private bool isAlarmMode;
    private string durationText;
    private string alarmText;
    private string? inputError;
    private string displayText = "00:00";
    private string caption = "";
    private TimeLevel level = TimeLevel.Normal;
    private double progress;
    private bool isFinished;
    private TaskbarItemProgressState taskbarState = TaskbarItemProgressState.None;
    private IReadOnlyList<string> devices = [DefaultDeviceLabel];
    private string selectedDevice = DefaultDeviceLabel;
    private string selectedFont;
    private bool topmost;
    private bool isSettingsPaneOpen;
    private string? statusMessage;

    public MainViewModel(SettingsStore store)
    {
        this.store = store;
        var settings = store.Load();

        var items = CountdownConfig.DefaultItems;
        countdown = new CountdownService(items.Select(i => i.TriggerTime));
        countdown.TimeUpdated += OnTimeUpdated;
        countdown.Triggered += OnTriggered;
        countdown.Completed += OnCompleted;
        countdown.StateChanged += OnStateChanged;

        var enabledSeconds = settings.EnabledTriggerSeconds;
        Triggers = new ObservableCollection<TriggerItemViewModel>(items.Select(i => new TriggerItemViewModel(
            i,
            enabledSeconds is null ? i.EnabledByDefault : enabledSeconds.Contains((int)i.TriggerTime.TotalSeconds))));

        isAlarmMode = settings.IsAlarmMode;
        durationText = settings.DurationText;
        alarmText = settings.AlarmText;
        topmost = settings.Topmost;
        isSettingsPaneOpen = settings.IsSettingsPaneOpen;
        InitialWindowWidth = settings.WindowWidth;
        InitialWindowHeight = settings.WindowHeight;

        FontFamilies = [DefaultFontLabel, .. Fonts.SystemFontFamilies
            .Select(f => f.Source)
            .Distinct()
            .Order(StringComparer.Create(CultureInfo.CurrentCulture, ignoreCase: true))];
        selectedFont = settings.FontFamily is { } font && FontFamilies.Contains(font) ? font : DefaultFontLabel;

        selectedDevice = settings.DeviceName ?? DefaultDeviceLabel;
        RefreshDevices();

        timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        timer.Tick += (_, _) => countdown.Tick();

        StartPauseCommand = new RelayCommand(StartPause);
        ResetCommand = new RelayCommand(Reset, () => !IsIdle || IsFinished);
        PresetCommand = new RelayCommand(ApplyPreset, _ => IsIdle);

        ValidateInput();
        UpdateIdleDisplay();
    }

    // -----------------------------
    // 入力
    // -----------------------------

    public bool IsAlarmMode
    {
        get => isAlarmMode;
        set
        {
            if (!SetProperty(ref isAlarmMode, value)) return;

            OnPropertyChanged(nameof(IsDurationMode));
            OnPropertyChanged(nameof(InputText));
            OnPropertyChanged(nameof(InputHint));
            OnInputChanged();
        }
    }

    public bool IsDurationMode
    {
        get => !IsAlarmMode;
        set => IsAlarmMode = !value;
    }

    /// <summary>入力欄の文字列。期間と時刻で別々に覚えておく。</summary>
    public string InputText
    {
        get => IsAlarmMode ? alarmText : durationText;
        set
        {
            if (IsAlarmMode) alarmText = value;
            else durationText = value;

            OnPropertyChanged();
            OnInputChanged();
        }
    }

    public string InputHint => IsAlarmMode
        ? "例: 15:00、9:30:15（24 時間表記）"
        : "例: 5（分）、4:30、1:00:00";

    public string? InputError
    {
        get => inputError;
        private set => SetProperty(ref inputError, value);
    }

    // -----------------------------
    // 表示
    // -----------------------------

    public string DisplayText
    {
        get => displayText;
        private set => SetProperty(ref displayText, value);
    }

    /// <summary>残り時間の上に出す補足（時刻指定のときの「15:00 まで」など）。</summary>
    public string Caption
    {
        get => caption;
        private set => SetProperty(ref caption, value);
    }

    public TimeLevel Level
    {
        get => level;
        private set => SetProperty(ref level, value);
    }

    /// <summary>残り時間の割合（1 → 0）。</summary>
    public double Progress
    {
        get => progress;
        private set => SetProperty(ref progress, value);
    }

    /// <summary>時間切れになってからリセットまたは再開始するまで true。</summary>
    public bool IsFinished
    {
        get => isFinished;
        private set => SetProperty(ref isFinished, value);
    }

    public TaskbarItemProgressState TaskbarState
    {
        get => taskbarState;
        private set => SetProperty(ref taskbarState, value);
    }

    public bool IsIdle => countdown.State == CountdownState.Idle;

    public string StartPauseGlyph => countdown.State == CountdownState.Running ? PauseGlyph : PlayGlyph;

    public string StartPauseLabel => countdown.State switch
    {
        CountdownState.Running => "一時停止 (Space)",
        CountdownState.Paused => "再開 (Space)",
        _ => "開始 (Space)",
    };

    public string? StatusMessage
    {
        get => statusMessage;
        private set => SetProperty(ref statusMessage, value);
    }

    // -----------------------------
    // 設定
    // -----------------------------

    public ObservableCollection<TriggerItemViewModel> Triggers { get; }

    public IReadOnlyList<string> Devices
    {
        get => devices;
        private set => SetProperty(ref devices, value);
    }

    public string SelectedDevice
    {
        get => selectedDevice;
        // 一覧の作り直し中に ComboBox から null が来ることがあるので無視する
        set { if (value is not null) SetProperty(ref selectedDevice, value); }
    }

    public IReadOnlyList<string> FontFamilies { get; }

    public string SelectedFont
    {
        get => selectedFont;
        set
        {
            if (value is null || !SetProperty(ref selectedFont, value)) return;
            OnPropertyChanged(nameof(DisplayFontFamily));
        }
    }

    /// <summary>残り時間の表示に使うフォント。</summary>
    public string DisplayFontFamily => SelectedFont == DefaultFontLabel ? DefaultFontFamily : SelectedFont;

    public bool Topmost
    {
        get => topmost;
        set => SetProperty(ref topmost, value);
    }

    public bool IsSettingsPaneOpen
    {
        get => isSettingsPaneOpen;
        set => SetProperty(ref isSettingsPaneOpen, value);
    }

    public double? InitialWindowWidth { get; }

    public double? InitialWindowHeight { get; }

    // -----------------------------
    // コマンド
    // -----------------------------

    public ICommand StartPauseCommand { get; }

    public ICommand ResetCommand { get; }

    /// <summary>引数は分（"5" など）。</summary>
    public ICommand PresetCommand { get; }

    private void StartPause()
    {
        switch (countdown.State)
        {
            case CountdownState.Idle:
                Start();
                break;
            case CountdownState.Running:
                audio.Stop();
                countdown.Pause();
                break;
            case CountdownState.Paused:
                countdown.Resume();
                break;
        }
    }

    private void Start()
    {
        if (!TryGetStartDuration(out var duration))
        {
            ValidateInput();
            return;
        }

        StatusMessage = null;
        IsFinished = false;
        ClearHighlight();
        Caption = IsAlarmMode && TimeInput.TryParseTimeOfDay(alarmText, out var target)
            ? $"{TimeInput.FormatTimeOfDay(target)} まで"
            : "";

        countdown.Start(duration);
    }

    private void Reset()
    {
        audio.Stop();
        IsFinished = false;
        ClearHighlight();
        countdown.Reset();

        // 時間切れ後（すでに停止状態）のリセットでは StateChanged が来ないので、ここで表示を戻す
        ShowIdle();
    }

    private void ApplyPreset(object? parameter)
    {
        if (!IsIdle || !int.TryParse(parameter as string, out var minutes)) return;

        IsAlarmMode = false;
        InputText = TimeInput.FormatDuration(TimeSpan.FromMinutes(minutes));
    }

    // -----------------------------
    // カウントダウンのイベント
    // -----------------------------

    private void OnTimeUpdated(TimeSpan remaining)
    {
        // 停止状態の表示は ShowIdle / OnCompleted で行う
        if (countdown.State == CountdownState.Idle) return;

        DisplayText = CountdownService.Format(remaining);
        Level = remaining <= CriticalThreshold ? TimeLevel.Critical
            : remaining <= WarningThreshold ? TimeLevel.Warning
            : TimeLevel.Normal;

        var total = countdown.Duration.TotalSeconds;
        Progress = total > 0 ? Math.Clamp(remaining.TotalSeconds / total, 0, 1) : 0;
    }

    private void OnTriggered(int index)
    {
        ClearHighlight();
        var trigger = Triggers[index];
        trigger.IsLastTriggered = true;

        if (!trigger.IsEnabled) return;

        try
        {
            audio.Play(trigger.Item.SoundKey, GetDeviceNumber());
            StatusMessage = null;
        }
        catch (Exception ex)
        {
            // 出力デバイスが外された場合など。カウントダウンは止めない
            audio.Stop();
            StatusMessage = $"音声を再生できませんでした: {ex.Message}";
        }
    }

    private void OnCompleted()
    {
        IsFinished = true;
        DisplayText = CountdownService.Format(TimeSpan.Zero);
        Level = TimeLevel.Finished;
        Progress = 1;
        TaskbarState = TaskbarItemProgressState.Error;
    }

    private void OnStateChanged(CountdownState state)
    {
        timer.IsEnabled = state == CountdownState.Running;

        TaskbarState = state switch
        {
            CountdownState.Running => TaskbarItemProgressState.Normal,
            CountdownState.Paused => TaskbarItemProgressState.Paused,
            _ => TaskbarState,
        };

        OnPropertyChanged(nameof(IsIdle));
        OnPropertyChanged(nameof(StartPauseGlyph));
        OnPropertyChanged(nameof(StartPauseLabel));
        CommandManager.InvalidateRequerySuggested();
    }

    // -----------------------------
    // 停止中の表示
    // -----------------------------

    private void ShowIdle()
    {
        Level = TimeLevel.Normal;
        Progress = 0;
        TaskbarState = TaskbarItemProgressState.None;
        UpdateIdleDisplay();
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>停止中は、入力中の時間（時刻指定なら目標時刻）を大きく表示しておく。</summary>
    private void UpdateIdleDisplay()
    {
        if (!IsIdle || IsFinished) return;

        if (IsAlarmMode)
        {
            var ok = TimeInput.TryParseTimeOfDay(alarmText, out var target);
            DisplayText = ok ? TimeInput.FormatTimeOfDay(target) : "--:--";
            Caption = ok ? "この時刻までカウントダウン" : "";
        }
        else
        {
            DisplayText = TimeInput.TryParseDuration(durationText, out var duration)
                ? CountdownService.Format(duration)
                : "--:--";
            Caption = "";
        }
    }

    /// <summary>入力が変わったとき。時間切れの表示中なら待機表示に戻す。</summary>
    private void OnInputChanged()
    {
        ValidateInput();
        if (!IsIdle) return;

        if (IsFinished)
        {
            IsFinished = false;
            ClearHighlight();
            ShowIdle();
        }
        else
        {
            UpdateIdleDisplay();
        }
    }

    private void ValidateInput()
    {
        InputError = IsAlarmMode
            ? TimeInput.TryParseTimeOfDay(alarmText, out _) ? null : "時刻を「15:00」のように入力してください。"
            : TimeInput.TryParseDuration(durationText, out _) ? null : "時間を「5」「4:30」「1:00:00」のように入力してください。";
    }

    private bool TryGetStartDuration(out TimeSpan duration)
    {
        if (IsAlarmMode)
        {
            var ok = TimeInput.TryParseTimeOfDay(alarmText, out var target);
            duration = ok ? CountdownService.DurationUntil(target, DateTime.Now) : TimeSpan.Zero;
            return ok;
        }

        return TimeInput.TryParseDuration(durationText, out duration);
    }

    private void ClearHighlight()
    {
        foreach (var t in Triggers)
            t.IsLastTriggered = false;
    }

    // -----------------------------
    // 出力デバイス
    // -----------------------------

    /// <summary>デバイス一覧を取り直す。選択中のデバイスがなくなっていれば既定に戻す。</summary>
    public void RefreshDevices()
    {
        IReadOnlyList<string> list = [DefaultDeviceLabel, .. AudioPlayer.GetDeviceNames()];
        var current = list.Contains(selectedDevice) ? selectedDevice : DefaultDeviceLabel;

        if (!list.SequenceEqual(Devices))
            Devices = list;

        selectedDevice = current;
        OnPropertyChanged(nameof(SelectedDevice));
    }

    private int GetDeviceNumber()
    {
        if (SelectedDevice == DefaultDeviceLabel) return AudioPlayer.DefaultDevice;

        var names = AudioPlayer.GetDeviceNames();
        for (int i = 0; i < names.Count; i++)
        {
            if (names[i] == SelectedDevice) return i;
        }

        // 選んでいたデバイスが外された
        RefreshDevices();
        return AudioPlayer.DefaultDevice;
    }

    // -----------------------------
    // 保存・終了
    // -----------------------------

    public void SaveSettings(double? windowWidth, double? windowHeight)
    {
        store.Save(new AppSettings
        {
            IsAlarmMode = IsAlarmMode,
            DurationText = durationText,
            AlarmText = alarmText,
            EnabledTriggerSeconds = Triggers
                .Where(t => t.IsEnabled)
                .Select(t => (int)t.Item.TriggerTime.TotalSeconds)
                .ToList(),
            DeviceName = SelectedDevice == DefaultDeviceLabel ? null : SelectedDevice,
            FontFamily = SelectedFont == DefaultFontLabel ? null : SelectedFont,
            Topmost = Topmost,
            IsSettingsPaneOpen = IsSettingsPaneOpen,
            WindowWidth = windowWidth,
            WindowHeight = windowHeight,
        });
    }

    public void Dispose()
    {
        timer.Stop();
        audio.Dispose();
    }
}
