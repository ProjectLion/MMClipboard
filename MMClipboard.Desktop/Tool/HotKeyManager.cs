/*🏷️----------------------------------------------------------------
 *📄 文件名：HotKeyManager.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-25 17:52:40
 *🏷️----------------------------------------------------------------*/


using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;


namespace MMClipboard.Tool;

public static class HotKeyManager
{
    public static void RegisterHotKey_ShowMainWindow(Key key, ModifierKeys modifierKeys)
    {
        HotkeyManager.Current.AddOrReplace("com.ht.mmclipboard.showMain", key, modifierKeys, ShowMainAction);
    }

    public static void RegisterHotKey_ShowPhraseWindow(Key key, ModifierKeys modifierKeys)
    {
        HotkeyManager.Current.AddOrReplace("com.ht.mmclipboard.showPhrase", key, modifierKeys, ShowPhraseAction);
    }

    private static void ShowMainAction(object sender, HotkeyEventArgs e)
    {
        if (!SharedInstance.Instance.isRecordingShortcutKey)
            SharedInstance.ShowMainWindow();
    }

    private static void ShowPhraseAction(object sender, HotkeyEventArgs e)
    {
        if (!SharedInstance.Instance.isRecordingShortcutKey)
            SharedInstance.ShowPhraseWindow();
    }
}