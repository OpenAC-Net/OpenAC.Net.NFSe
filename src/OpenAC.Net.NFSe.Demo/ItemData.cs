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

        var field = enumType.GetField(value.ToString(), BindingFlags.Static | BindingFlags.Public);
        if (field == null) Description = string.Empty;

        var attribute = field.GetAttribute<DescriptionAttribute>();
        Description = attribute == null ? value.ToString() : attribute.Description;
    }

    #endregion Constructors

    #region Properties

    public string Description { get; set; }

    public T Content { get; set; }

    #endregion Properties

    #region Methods

    public override string ToString()
    {
        return Description;
    }

    #endregion Methods
}