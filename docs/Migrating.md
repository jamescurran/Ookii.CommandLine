# Migrating from Ookii.CommandLine 2.x / 3.x

Ookii.CommandLine 4.0 and later have a number of breaking changes from version 3.x and earlier
versions. This article explains what you need to know to migrate your code to the new version.

Although there are quite a few changes, it's likely your application will not require many
modifications unless you used subcommands, heavily customized the usage help format, or used
custom argument value conversion.

## .Net Framework support

As of version 3.0, .Net Framework 2.0 is no longer supported. You can still target .Net Framework
4.6.1 and later using the .Net Standard 2.0 assembly. If you need to support an older version of
.Net, please continue to use [version 2.4](https://github.com/SvenGroot/ookii.commandline/releases/tag/v2.4).

## Breaking API changes from version 3.0

- It's strongly recommended to apply the [`GeneratedParserAttribute`][] to your arguments classes
  unless you cannot meet the requirements for [source generation](SourceGeneration.md).
- The `CommandLineArgumentAttribute.ValueDescription` property has been replaced by the
  [`ValueDescriptionAttribute`][] attribute. This new attribute is not sealed, enabling derived
  attributes e.g. to load a value description from a localized resource.
- Converting argument values from a string to their final type is no longer done using the
  [`TypeConverter`][] class, but instead using a custom [`ArgumentConverter`][] class. Custom
  converters must be specified using the [`ArgumentConverterAttribute`][] instead of the
  [`TypeConverterAttribute`][].
  - If you have existing conversions that depend on a [`TypeConverter`][], use the
    [`WrappedTypeConverter<T>`][] and [`WrappedDefaultTypeConverter<T>`][] as a convenient way to
    keep using that conversion.
  - The [`KeyValuePairConverter<TKey, TValue>`][] class has moved into the
    [`Ookii.CommandLine.Conversion`][] namespace.
  - The [`KeyValueSeparatorAttribute`][] has moved into the [`Ookii.CommandLine.Conversion`][]
    namespace.
  - The `KeyTypeConverterAttribute` and `ValueTypeConverterAttribute` were renamed to
    [`KeyConverterAttribute`][] and [`ValueConverterAttribute`][] respectively
- Constructor parameters can no longer be used to define command line arguments. Instead, all
  arguments must be defined using properties.
- `ParseOptions.ArgumentNameComparer` and `CommandOptions.CommandNameComparer` have been replaced by
  [`ArgumentNameComparison`][ArgumentNameComparison_1] and [`CommandNameComparison`][] respectively,
  both now taking a [`StringComparison`][] value instead of an [`IComparer<string>`][].
- Overloads of the [`CommandLineParser.Parse()`][CommandLineParser.Parse()_2], [`CommandLineParser.ParseWithErrorHandling()`][],
  [`CommandLineParser<T>.Parse()`][], [`CommandLineParser<T>.ParseWithErrorHandling()`][],
  [`CommandManager.CreateCommand()`][] and [`CommandManager.RunCommand()`][] methods that took an index have
  been replaced by overloads that take a [`ReadOnlyMemory<string>`][].
- The [`CommandInfo`][] type is now a class instead of a structure.
- The [`ICommandWithCustomParsing.Parse()`][] method signature has changed to use a
  [`ReadOnlyMemory<string>`][] structure for the arguments and to receive a reference to the calling
  [`CommandManager`][] instance.
- The [`CommandLineArgumentAttribute.CancelParsing`][] property now takes a [`CancelMode`][]
  enumeration rather than a boolean.
- The [`ArgumentParsedEventArgs`][] class was changed to use the [`CancelMode`][] enumeration.
- Canceling parsing using the [`ArgumentParsed`][] event no longer automatically sets the [`HelpRequested`][]
  property; instead, you must set it manually in the event handler if desired.
- The `ParseOptionsAttribute.NameValueSeparator` property was replaced with
  [`ParseOptionsAttribute.NameValueSeparators`][].
- The `ParseOptions.NameValueSeparator` property was replaced with
  [`ParseOptions.NameValueSeparators`][].
- Properties that previously returned a [`ReadOnlyCollection<T>`][] now return an
  [`ImmutableArray<T>`][].
- The `CommandLineArgument.MultiValueSeparator` and `CommandLineArgument.AllowMultiValueWhiteSpaceSeparator`
  properties have been moved into the [`CommandLineArgument.MultiValueInfo`][] property.
- The `CommandLineArgument.AllowsDuplicateDictionaryKeys` and `CommandLineArgument.KeyValueSeparator`
  properties have been moved into the [`CommandLineArgument.DictionaryInfo`][] property.
- The `CommandLineArgument.IsDictionary` and `CommandLineArgument.IsMultiValue` properties have been
  removed; instead, check [`CommandLineArgument.DictionaryInfo`][] or [`CommandLineArgument.MultiValueInfo`][]
  for null values, or use the [`CommandLineArgument.Kind`][] property.
- [`TextFormat`][] is now a structure with strongly-typed values for VT sequences, and that structure is
  used by the [`UsageWriter`][] class for the various color formatting options.

## Breaking behavior changes from version 3.0

- By default, both `:` and `=` are accepted as argument name/value separators.
- The default value of [`ParseOptions.ShowUsageOnError`][] has changed to [`UsageHelpRequest.SyntaxOnly`][].
- [Automatic prefix aliases](DefiningArguments.md#automatic-prefix-aliases) are enabled by default
  for both argument names and [command names](Subcommands.md#command-aliases).
- The [`CommandManager`][], when using an assembly that is not the calling assembly, will only use
  public command classes, where before it would also use internal ones. This is to better respect
  access modifiers, and to make sure generated and reflection-based command managers behave the
  same.

## Breaking API changes from version 2.4

- It's strongly recommended to switch to the static [`CommandLineParser.Parse<T>()`][] method, if you
  were not already using it from version 2.4.
- If you do need to manually handle errors, be aware of the following changes:
  - If the instance [`CommandLineParser.Parse()`][CommandLineParser.Parse()_2] method returns null, you should only show usage help
    if the [`CommandLineParser.HelpRequested`][] property is true.
  - Version 3.0 adds automatic "-Help" and "-Version" properties, which means the [`Parse()`][Parse()_6] method
    can return null even if it previously wouldn't.
  - Recommended: use the [`CommandLineParser<T>`][] class to get strongly typed instance [`Parse()`][Parse()_5]
    methods.
  - The [`CommandLineParser`][] constructor now takes a [`ParseOptions`][] instance.
  - Several properties of the [`CommandLineParser`][] class that could be used to change parsing behavior
    are now read-only and can only be changed through [`ParseOptions`][].
  - See [manual parsing and error handling](ParsingArguments.md#manual-parsing-and-error-handling)
    for an updated example.
- The `WriteUsageOptions` class has been replaced with [`UsageWriter`][].
- Usage options that previously were formatting strings now require creating a class that derives
  from [`UsageWriter`][] and overrides some of its methods. You have much more control over the
  formatting this way. See [customizing the usage help](UsageHelp.md#customizing-the-usage-help).
- The `CommandLineParser.WriteUsageToConsole()` method no longer exists; instead, the
  [`CommandLineParser.WriteUsage()`][] method will write to the console when invoked using a
  [`UsageWriter`][] with no explicitly set output.
- The subcommand (formerly called "shell command") API has had substantial changes.
  - Subcommand related functionality has moved into the [`Ookii.CommandLine.Commands`][] namespace.
  - The `ShellCommand` class as a base class for command classes has been replaced with the
    [`ICommand`][] interface.
  - The `ShellCommand.ExitCode` property has been replaced with the return value of the
    [`ICommand.Run()`][] method.
  - The `ShellCommand` class's static methods have been replaced with the [`CommandManager`][] class.
  - The `ShellCommandAttribute` class has been renamed to [`CommandAttribute`][].
  - The `ShellCommandAttribute.CustomParsing` property has been replaced with the
    [`ICommandWithCustomParsing`][] interface.
    - Commands with custom parsing no longer need a special constructor, and must implement the
      [`ICommandWithCustomParsing.Parse()`][] method instead.
  - The `CreateShellCommandOptions` class has been renamed to [`CommandOptions`][].
  - Usage related options have been moved into the [`UsageWriter`][] class.
  - Recommended: use [`IAsyncCommand`][] or [`AsyncCommandBase`][], along with
    [`CommandManager.RunCommandAsync()`][], if your commands need to run asynchronous code.
- A number of explicit method overloads have been removed in favor of optional parameters.
- The [`CommandLineArgument.ElementType`][] property now returns the underlying type for arguments
  using the [`Nullable<T>`][] type.

## Breaking behavior changes from version 2.4

- Argument type conversion now defaults to [`CultureInfo.InvariantCulture`][], instead of
  [`CurrentCulture`][]. This change was made to ensure a consistent parsing experience regardless of the
  user's regional settings. Only change it if you have a good reason.
- [`CommandLineParser`][] automatically adds `-Help` and `-Version` arguments by default. If you had
  arguments with these names, this will not affect you. The `-Version` argument is not added for
  subcommands.
- [`CommandManager`][] automatically adds a `version` command by default. If you had a command with
  this name, this will not affect you.
- The usage help now includes aliases and default values by default.
- The default format for showing aliases in the usage help has changed.
- Usage help uses color output by default (where supported).
- The [`LineWrappingTextWriter`][] class does not count virtual terminal sequences as part of the
  line length by default.

[`ArgumentConverter`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_ArgumentConverter.htm
[`ArgumentConverterAttribute`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_ArgumentConverterAttribute.htm
[`ArgumentParsed`]: https://www.ookii.org/docs/commandline-4.0/html/E_Ookii_CommandLine_CommandLineParser_ArgumentParsed.htm
[`ArgumentParsedEventArgs`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_ArgumentParsedEventArgs.htm
[`AsyncCommandBase`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_AsyncCommandBase.htm
[`CancelMode`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_CancelMode.htm
[`CommandAttribute`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_CommandAttribute.htm
[`CommandInfo`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_CommandInfo.htm
[`CommandLineArgument.DictionaryInfo`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_CommandLineArgument_DictionaryInfo.htm
[`CommandLineArgument.ElementType`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_CommandLineArgument_ElementType.htm
[`CommandLineArgument.Kind`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_CommandLineArgument_Kind.htm
[`CommandLineArgument.MultiValueInfo`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_CommandLineArgument_MultiValueInfo.htm
[`CommandLineArgumentAttribute.CancelParsing`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_CommandLineArgumentAttribute_CancelParsing.htm
[`CommandLineParser.HelpRequested`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_CommandLineParser_HelpRequested.htm
[`CommandLineParser.Parse<T>()`]: https://www.ookii.org/docs/commandline-4.0/html/M_Ookii_CommandLine_CommandLineParser_Parse__1.htm
[`CommandLineParser.ParseWithErrorHandling()`]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_CommandLineParser_ParseWithErrorHandling.htm
[`CommandLineParser.WriteUsage()`]: https://www.ookii.org/docs/commandline-4.0/html/M_Ookii_CommandLine_CommandLineParser_WriteUsage.htm
[`CommandLineParser`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_CommandLineParser.htm
[`CommandLineParser<T>.Parse()`]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_CommandLineParser_1_Parse.htm
[`CommandLineParser<T>.ParseWithErrorHandling()`]: https://www.ookii.org/docs/commandline-4.0/html/M_Ookii_CommandLine_CommandLineParser_1_ParseWithErrorHandling.htm
[`CommandLineParser<T>`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_CommandLineParser_1.htm
[`CommandManager.CreateCommand()`]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_Commands_CommandManager_CreateCommand.htm
[`CommandManager.RunCommand()`]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_Commands_CommandManager_RunCommand.htm
[`CommandManager.RunCommandAsync()`]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_Commands_CommandManager_RunCommandAsync.htm
[`CommandManager`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_CommandManager.htm
[`CommandNameComparison`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_Commands_CommandOptions_CommandNameComparison.htm
[`CommandOptions`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_CommandOptions.htm
[`CultureInfo.InvariantCulture`]: https://learn.microsoft.com/dotnet/api/system.globalization.cultureinfo.invariantculture
[`CurrentCulture`]: https://learn.microsoft.com/dotnet/api/system.globalization.cultureinfo.currentculture
[`GeneratedParserAttribute`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_GeneratedParserAttribute.htm
[`HelpRequested`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_CommandLineParser_HelpRequested.htm
[`IAsyncCommand`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_IAsyncCommand.htm
[`ICommand.Run()`]: https://www.ookii.org/docs/commandline-4.0/html/M_Ookii_CommandLine_Commands_ICommand_Run.htm
[`ICommand`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_ICommand.htm
[`ICommandWithCustomParsing.Parse()`]: https://www.ookii.org/docs/commandline-4.0/html/M_Ookii_CommandLine_Commands_ICommandWithCustomParsing_Parse.htm
[`ICommandWithCustomParsing`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Commands_ICommandWithCustomParsing.htm
[`IComparer<string>`]: https://learn.microsoft.com/dotnet/api/system.collections.generic.icomparer-1
[`ImmutableArray<T>`]: https://learn.microsoft.com/dotnet/api/system.collections.immutable.immutablearray-1
[`KeyConverterAttribute`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_KeyConverterAttribute.htm
[`KeyValuePairConverter<TKey, TValue>`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_KeyValuePairConverter_2.htm
[`KeyValueSeparatorAttribute`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_KeyValueSeparatorAttribute.htm
[`LineWrappingTextWriter`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_LineWrappingTextWriter.htm
[`Nullable<T>`]: https://learn.microsoft.com/dotnet/api/system.nullable-1
[`Ookii.CommandLine.Commands`]: https://www.ookii.org/docs/commandline-4.0/html/N_Ookii_CommandLine_Commands.htm
[`Ookii.CommandLine.Conversion`]: https://www.ookii.org/docs/commandline-4.0/html/N_Ookii_CommandLine_Conversion.htm
[`ParseOptions.NameValueSeparators`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_ParseOptions_NameValueSeparators.htm
[`ParseOptions.ShowUsageOnError`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_ParseOptions_ShowUsageOnError.htm
[`ParseOptions`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_ParseOptions.htm
[`ParseOptionsAttribute.NameValueSeparators`]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_ParseOptionsAttribute_NameValueSeparators.htm
[`ReadOnlyCollection<T>`]: https://learn.microsoft.com/dotnet/api/system.collections.objectmodel.readonlycollection-1
[`ReadOnlyMemory<string>`]: https://learn.microsoft.com/dotnet/api/system.readonlymemory-1
[`StringComparison`]: https://learn.microsoft.com/dotnet/api/system.stringcomparison
[`TextFormat`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Terminal_TextFormat.htm
[`TypeConverter`]: https://learn.microsoft.com/dotnet/api/system.componentmodel.typeconverter
[`TypeConverterAttribute`]: https://learn.microsoft.com/dotnet/api/system.componentmodel.typeconverterattribute
[`UsageHelpRequest.SyntaxOnly`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_UsageHelpRequest.htm
[`UsageWriter`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_UsageWriter.htm
[`ValueConverterAttribute`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_ValueConverterAttribute.htm
[`ValueDescriptionAttribute`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_ValueDescriptionAttribute.htm
[`WrappedTypeConverter<T>`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_WrappedTypeConverter_1.htm
[`WrappedDefaultTypeConverter<T>`]: https://www.ookii.org/docs/commandline-4.0/html/T_Ookii_CommandLine_Conversion_WrappedDefaultTypeConverter_1.htm
[ArgumentNameComparison_1]: https://www.ookii.org/docs/commandline-4.0/html/P_Ookii_CommandLine_ParseOptions_ArgumentNameComparison.htm
[CommandLineParser.Parse()_2]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_CommandLineParser_Parse.htm
[Parse()_5]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_CommandLineParser_1_Parse.htm
[Parse()_6]: https://www.ookii.org/docs/commandline-4.0/html/Overload_Ookii_CommandLine_CommandLineParser_Parse.htm
