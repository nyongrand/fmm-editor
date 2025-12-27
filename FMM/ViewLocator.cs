using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FMM.ViewModels;

namespace FMM;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!;
        var candidates = new[]
        {
            name.Replace("ViewModel", "View", StringComparison.Ordinal),
            name.Replace("ViewModels", "Views", StringComparison.Ordinal)
                .Replace("ViewModel", "View", StringComparison.Ordinal),
            name.Replace(".ViewModels.", ".", StringComparison.Ordinal)
                .Replace("ViewModel", "View", StringComparison.Ordinal)
        };

        foreach (var candidate in candidates)
        {
            var type = Type.GetType(candidate);
            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}