using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using VoiceCountdown.Desktop.ViewModels;
using VoiceCountdown.Core.Settings;

namespace VoiceCountdown.Desktop;

public partial class MainWindow : Window
{
    private const string FullScreenGlyph = "";
    private const string BackToWindowGlyph = "";

    private readonly MainViewModel viewModel;

    // 全画面表示にする前のウィンドウの状態
    private bool isFullScreen;
    private WindowState previousState;
    private WindowStyle previousStyle;
    private ResizeMode previousResizeMode;

    public MainWindow()
    {
        InitializeComponent();

        viewModel = new MainViewModel(SettingsStore.CreateDefault());
        DataContext = viewModel;

        if (viewModel.InitialWindowWidth is double width && width >= MinWidth) Width = width;
        if (viewModel.InitialWindowHeight is double height && height >= MinHeight) Height = height;
    }

    // -----------------------------
    // キーボード操作
    // -----------------------------

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled) return;

        switch (e.Key)
        {
            case Key.F11:
                ToggleFullScreen();
                e.Handled = true;
                break;

            case Key.Escape:
                if (isFullScreen) ToggleFullScreen();
                else Execute(viewModel.ResetCommand);
                e.Handled = true;
                break;

            // 入力欄でのスペースは開始操作にしない
            case Key.Space when Keyboard.FocusedElement is not TextBox:
            case Key.Enter:
                Execute(viewModel.StartPauseCommand);
                e.Handled = true;
                break;
        }
    }

    private static void Execute(ICommand command)
    {
        if (command.CanExecute(null)) command.Execute(null);
    }

    // -----------------------------
    // 全画面表示
    // -----------------------------

    private void FullScreen_Click(object sender, RoutedEventArgs e) => ToggleFullScreen();

    private void ToggleFullScreen()
    {
        if (!isFullScreen)
        {
            previousState = WindowState;
            previousStyle = WindowStyle;
            previousResizeMode = ResizeMode;

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            // 最大化中に枠を消してもタスクバーが隠れないため、いったん通常表示に戻す
            WindowState = WindowState.Normal;
            WindowState = WindowState.Maximized;

            FullScreenButton.Content = BackToWindowGlyph;
            FullScreenButton.ToolTip = "全画面表示を解除 (F11 / Esc)";
        }
        else
        {
            WindowStyle = previousStyle;
            ResizeMode = previousResizeMode;
            WindowState = previousState;

            FullScreenButton.Content = FullScreenGlyph;
            FullScreenButton.ToolTip = "全画面表示 (F11)";
        }

        isFullScreen = !isFullScreen;
    }

    // -----------------------------
    // その他
    // -----------------------------

    private void Devices_DropDownOpened(object? sender, EventArgs e) => viewModel.RefreshDevices();

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        if (e.Cancel) return;

        // 最大化・全画面のときは通常表示のサイズを保存する
        var bounds = WindowState == WindowState.Normal && !isFullScreen
            ? new Size(ActualWidth, ActualHeight)
            : RestoreBounds.Size;

        // 一度も最大化していない場合など、RestoreBounds が空のことがある
        static double? Valid(double v) => double.IsFinite(v) && v > 0 ? v : null;
        viewModel.SaveSettings(Valid(bounds.Width), Valid(bounds.Height));
        viewModel.Dispose();
    }
}
