using System.Collections.Generic;
using System.Threading.Tasks;
using GFMWakeUpHelper.Data.Entities;
using SukiUI.Controls;
using SukiUI.MessageBox;

namespace GFMWakeUpHelper.App.Dialogs.AskSameSongDialog;

public static class ShowAskSameSongDialog
{
    public static async Task<object?> ShowAskSameSongMessageBox(IEnumerable<Song> allSongs)
    {
        var dataContext = new AskSameSongDialogViewModel(allSongs);

        var result = await SukiMessageBox.ShowDialog(new SukiMessageBoxHost
            {
                IconPreset = SukiMessageBoxIcons.Question,
                Header = "合并选项",
                Content = new AskSameSongDialog
                {
                    DataContext = dataContext
                },
                ActionButtonsPreset = SukiMessageBoxButtons.YesNoCancel
            },
            new SukiMessageBoxOptions
            {
                IsTitleBarVisible = false,
                CanResize = false
            });


        return result;
    }
}