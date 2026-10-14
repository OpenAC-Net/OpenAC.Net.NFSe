using System;
using System.ComponentModel;
using System.Reflection;
using OpenAC.Net.Core;
using OpenAC.Net.Core.Extensions;

namespace OpenAC.Net.NFSe.Demo;

public sealed class ItemData<T>
{
    #region Constructors

    public ItemData()
    {
    }

    public ItemData(string description, T content)
    {
        Description = description;
        Content = content;
    }

    public ItemData(T value)
    {
        Content = value;

        if (!(value is Enum)) return;

        var enumType = typeof(T);
        Guard.Against(!enumType.IsEnum, "O tipo de parametro T precisa ser um enum.");
        Guard.Against(!Enum.IsDefined(enumType, value), $"{enumType} o valor {value} não esta definido no enum.");

        var field = value != null ? enumType.GetField(value.ToString()!, BindingFlags.Static | BindingFlags.Public) : null;
        if (field == null)
        {
            Description = value?.ToString() ?? string.Empty;
            return;
        }

        var attribute = field.GetAttribute<DescriptionAttribute>();
        Description = attribute?.Description ?? value?.ToString() ?? string.Empty;
    }

    #endregion Constructors

    #region Properties

    public string Description { get; set; } = string.Empty;

    public T Content { get; set; } = default!;

    #endregion Properties

    #region Methods

    public override string ToString()
    {
        return Description;
    }

    #endregion Methods
}