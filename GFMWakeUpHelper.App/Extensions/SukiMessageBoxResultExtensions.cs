using System;
using System.Threading.Tasks;
using SukiUI.MessageBox;

namespace GFMWakeUpHelper.App.Extensions;

public static class SukiMessageBoxResultExtensions
{
    /// <summary>
    /// 根据 MessageBox 结果执行对应的异步逻辑。
    /// </summary>
    /// <param name="result">用户选择结果。</param>
    /// <param name="onYes">当用户选择“是”时执行的逻辑。</param>
    /// <param name="onNo">当用户选择“否”时执行的逻辑。</param>
    /// <param name="onCancel">当用户选择“取消”时执行的逻辑。</param>
    /// <param name="onYesAsync">当用户选择“是”时异步执行的逻辑。</param>
    /// <param name="onNoAsync">当用户选择“否”时异步执行的逻辑。</param>
    /// <param name="onCancelAsync">当用户选择“取消”时异步执行的逻辑。</param>
    public static async Task HandleAsync(
        this SukiMessageBoxResult result,
        Action? onYes = null,
        Action? onNo = null,
        Action? onCancel = null,
        Func<Task>? onYesAsync = null,
        Func<Task>? onNoAsync = null,
        Func<Task>? onCancelAsync = null)
    {
        switch (result)
        {
            case SukiMessageBoxResult.Yes:
                if (onYes != null) onYes();
                else if (onYesAsync != null) await onYesAsync();
                break;
            case SukiMessageBoxResult.No:
                if (onNo != null) onNo();
                else if (onNoAsync != null) await onNoAsync();
                break;
            case SukiMessageBoxResult.Cancel:
                if (onCancel != null) onCancel();
                else if (onCancelAsync != null) await onCancelAsync();
                break;
        }
    }
}