/*🏷️----------------------------------------------------------------
 *📄 文件名：ShortcutPhraseModel.cs
 *🏷️
 *👨🏽‍💻 创建者：Ht
 *⏱️ 创建时间：2024-02-03 15:35:29
 *🏷️----------------------------------------------------------------*/


using CommunityToolkit.Mvvm.ComponentModel;
using FreeSql.DataAnnotations;
using MMClipboard.Tool;


namespace MMClipboard.Model;

[Table(Name = "phrase")]
public class ShortcutPhraseModel : ObservableObject
{
    [Column(IsIdentity = true)] public int id { get; set; }

    /// <summary>
    /// 短语标题，默认为内容
    /// </summary>
    public string title { get; set; } = "";

    /// <summary>
    /// 短语内容
    /// </summary>
    [Column(StringLength = 1000)]
    public string phrase { get; set; } = "";

    /// <summary>
    /// 标签颜色 默认红色
    /// </summary>
    public string tagColor { get; set; } = "FF0000";

    /// <summary>
    /// 标签名称
    /// </summary>
    public string tagName { get; set; } = "默认分组";

    private ShortcutPhraseModel()
    { }

    public static ShortcutPhraseModel Create(string phrase, string tagName = null, string title = null, string tagColor = null)
    {
        var res = new ShortcutPhraseModel()
        {
            phrase = phrase,
            title = title ?? phrase
        };

        if (!tagName.Ht_IsEmpty())
            res.tagName = tagName;

        if (tagColor is not null)
        {
            res.tagColor = tagColor;
        }
        else
        {
            var colorStr = DataBaseController.GetPhraseTagColorWithTag(tagName);
            res.tagColor = colorStr ?? "FF0000";
        }
        return res;
    }
}