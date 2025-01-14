﻿using Microsoft.CodeAnalysis;

namespace Ookii.CommandLine.Generator;

internal readonly struct ArgumentsClassAttributes
{
    private readonly AttributeData? _parseOptions;
    private readonly AttributeData? _description;
    private readonly AttributeData? _applicationFriendlyName;
    private readonly AttributeData? _command;
    private readonly AttributeData? _generatedParser;
    private readonly AttributeData? _parentCommand;
    private readonly List<AttributeData>? _classValidators;
    private readonly List<AttributeData>? _aliases;

    public ArgumentsClassAttributes(ITypeSymbol symbol, TypeHelper typeHelper)
    {
        // Exclude special types so we don't generate warnings for attributes on framework types.
        for (var current = symbol; current?.SpecialType == SpecialType.None; current = current.BaseType)
        {
            foreach (var attribute in current.GetAttributes())
            {
                var _ = attribute.CheckType(typeHelper.ParseOptionsAttribute, ref _parseOptions) ||
                    attribute.CheckType(typeHelper.DescriptionAttribute, ref _description) ||
                    attribute.CheckType(typeHelper.ApplicationFriendlyNameAttribute, ref _applicationFriendlyName) ||
                    attribute.CheckType(typeHelper.CommandAttribute, ref _command) ||
                    attribute.CheckType(typeHelper.ClassValidationAttribute, ref _classValidators) ||
                    attribute.CheckType(typeHelper.ParentCommandAttribute, ref _parentCommand) ||
                    attribute.CheckType(typeHelper.AliasAttribute, ref _aliases) ||
                    attribute.CheckType(typeHelper.GeneratedParserAttribute, ref _generatedParser);
            }
        }
    }

    public AttributeData? ParseOptions => _parseOptions;
    public AttributeData? Description => _description;
    public AttributeData? ApplicationFriendlyName => _applicationFriendlyName;
    public AttributeData? Command => _command;
    public AttributeData? GeneratedParser => _generatedParser;
    public AttributeData? ParentCommand => _parentCommand;
    public List<AttributeData>? ClassValidators => _classValidators;
    public List<AttributeData>? Aliases => _aliases;
}
