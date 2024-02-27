/*🏷️----------------------------------------------------------------
 *📄 文件名：ShortcutPhraseViewModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-03 11:24:29
 *🏷️----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using MMClipboard.Model;
using MMClipboard.Tool;
using MMClipboard.UserConfigs;
using MMClipboard.View;


namespace MMClipboard.ViewModel;

public class ShortcutPhraseViewModel : ObservableObject, IDisposable
{
    public List<ShortcutPhraseModel> phrases
    {
        get => _clips;
        private set => SetProperty(ref _clips, value);
    }
    private List<ShortcutPhraseModel> _clips;

    public WindowBackgroundModel backgroundModel { get; } = new();

    private ShortcutPhraseWindow m_window;

    /// <summary>
    /// init
    /// </summary>
    public ShortcutPhraseViewModel(ShortcutPhraseWindow window)
    {
        m_window = window;
        RefreshData();
    }

    /// <summary>
    /// 复制粘贴短语
    /// </summary>
    /// <param name="phrase"></param>
    public void CopyText(string phrase)
    {
        if (string.IsNullOrEmpty(phrase))
            return;
        CopyAndPasteHelper.CopyAndPasteText(phrase, () =>
        {
            if (UserConfig.Default.config.isCopiedClose)
                m_window.Close();
        });
    }

    /// <summary>
    /// 添加短语
    /// </summary>
    public void AddPhrase(ShortcutPhraseModel phraseModel)
    {
        if (DataBaseController.AddPhrase(phraseModel))
            RefreshData();
    }

    /// <summary>
    /// 删除短语
    /// </summary>
    /// <param name="model"></param>
    public void DeletePhrase(ShortcutPhraseModel model)
    {
        DataBaseController.DeletePhrase(model);
        RefreshData();
    }

    /// <summary>
    /// 获取所有短语
    /// </summary>
    public void RefreshData()
    {
        phrases = DataBaseController.GetAllPhrases();
    }

    public void Dispose()
    {
        backgroundModel.Dispose();
    }
}