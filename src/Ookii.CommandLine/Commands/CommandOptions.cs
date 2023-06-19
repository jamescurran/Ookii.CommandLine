﻿using System;

namespace Ookii.CommandLine.Commands;

/// <summary>
/// Provides options for the <see cref="CommandManager"/> class.
/// </summary>
public class CommandOptions : ParseOptions
{
    /// <summary>
    /// Gets or sets a value that indicates whether the options follow POSIX conventions.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the options follow POSIX conventions; otherwise,
    /// <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   This property is provided as a convenient way to set a number of related properties
    ///   that together indicate the parser is using POSIX conventions. POSIX conventions in
    ///   this case means that parsing uses long/short mode, argument and command names are case
    ///   sensitive, and argument names, command names and value descriptions use dash case
    ///   (e.g. "argument-name").
    /// </para>
    /// <para>
    ///   Setting this property to <see langword="true"/> is equivalent to setting the
    ///   <see cref="ParseOptions.Mode"/> property to <see cref="ParsingMode.LongShort"/>, the
    ///   <see cref="ParseOptions.ArgumentNameComparison"/> property to <see cref="StringComparison.InvariantCulture"/>,
    ///   the <see cref="ParseOptions.ArgumentNameTransform"/> property to <see cref="NameTransform.DashCase"/>,
    ///   the <see cref="ParseOptions.ValueDescriptionTransform"/> property to <see cref="NameTransform.DashCase"/>,
    ///   the <see cref="CommandNameComparison"/> property to <see cref="StringComparison.InvariantCulture"/>,
    ///   and the <see cref="CommandNameTransform"/> property to <see cref="NameTransform.DashCase"/>.
    /// </para>
    /// <para>
    ///   This property will only return <see langword="true"/> if the above properties are the
    ///   indicated values, except that <see cref="ParseOptions.ArgumentNameComparison"/> and
    ///   <see cref="CommandNameComparison"/> can be any case-sensitive comparison. It will
    ///   return <see langword="false"/> for any other combination of values, not just the ones
    ///   indicated below.
    /// </para>
    /// <para>
    ///   Setting this property to <see langword="false"/> is equivalent to setting the
    ///   <see cref="ParseOptions.Mode"/> property to <see cref="ParsingMode.Default"/>, the
    ///   <see cref="ParseOptions.ArgumentNameComparison"/> property to <see cref="StringComparison.OrdinalIgnoreCase"/>,
    ///   the <see cref="ParseOptions.ArgumentNameTransform"/> property to <see cref="NameTransform.None"/>,
    ///   the <see cref="ParseOptions.ValueDescriptionTransform"/> property to <see cref="NameTransform.None"/>,
    ///   the <see cref="CommandNameComparison"/> property to <see cref="StringComparison.OrdinalIgnoreCase"/>,
    ///   and the <see cref="CommandNameTransform"/> property to <see cref="NameTransform.None"/>.
    /// </para>
    /// </remarks>
    public override bool IsPosix
    {
        get => base.IsPosix && CommandNameComparison.IsCaseSensitive() && CommandNameTransform == NameTransform.DashCase;
        set
        {
            base.IsPosix = value;
            if (value)
            {
                CommandNameComparison = StringComparison.InvariantCulture;
                CommandNameTransform = NameTransform.DashCase;
            }
            else
            {
                CommandNameComparison = StringComparison.OrdinalIgnoreCase;
                CommandNameTransform = NameTransform.None;
            }
        }
    }

    /// <summary>
    /// Gets or set the type of string comparison to use for argument names.
    /// </summary>
    /// <value>
    /// One of the values of the <see cref="StringComparison"/> enumeration. The default value
    /// is <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </value>
    public StringComparison CommandNameComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Gets or sets a value that indicates how names are created for commands that don't have
    /// an explicit name.
    /// </summary>
    /// <value>
    /// One of the values of the <see cref="NameTransform"/> enumeration. The default value
    /// is <see cref="NameTransform.None"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   If a command hasn't set an explicit name using the <see cref="CommandAttribute"/>
    ///   attribute, the name is derived from the type name of the command, applying the
    ///   specified transformation.
    /// </para>
    /// <para>
    ///   If this property is not <see cref="NameTransform.None"/>, the value specified by the
    ///   <see cref="StripCommandNameSuffix"/> property will be removed from the end of the
    ///   type name before applying the transformation.
    /// </para>
    /// <para>
    ///   This transformation is also used for the name of the automatic version command if
    ///   the <see cref="AutoVersionCommand"/> property is <see langword="true"/>.
    /// </para>
    /// <para>
    ///   This transformation is not used for commands that have an explicit name.
    /// </para>
    /// </remarks>
    public NameTransform CommandNameTransform { get; set; }

    /// <summary>
    /// Gets or sets a value that will be removed from the end of a command name during name
    /// transformation.
    /// </summary>
    /// <value>
    /// The suffix to remove, or <see langword="null"/> to not remove any suffix. The default
    /// value is "Command".
    /// </value>
    /// <remarks>
    /// <para>
    ///   This property is only used if the <see cref="CommandNameTransform"/> property is not 
    ///   <see cref="NameTransform.None"/>, and is never used for commands with an explicit
    ///   name.
    /// </para>
    /// <para>
    ///   For example, if you have a subcommand class named "CreateFileCommand" and you use
    ///   <see cref="NameTransform.DashCase"/> and the default value of "Command" for this
    ///   property, the name of the command will be "create-file" without having to explicitly
    ///   specify it.
    /// </para>
    /// <para>
    ///   The suffix is case sensitive.
    /// </para>
    /// </remarks>
    public string? StripCommandNameSuffix { get; set; } = "Command";

    /// <summary>
    /// Gets or sets a function that filters which commands to include.
    /// </summary>
    /// <value>
    /// A function that filters the commands, or <see langword="null"/> to use no filter. The
    /// default value is <see langword="null"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   Use this to only use a subset of the commands defined in the assembly or assemblies.
    ///   The remaining commands will not be possible to invoke by the user.
    /// </para>
    /// <note>
    ///   The filter is not invoked for the automatic version command. Set the <see cref="AutoVersionCommand"/>
    ///   property to <see langword="false"/> if you wish to exclude that command.
    /// </note>
    /// </remarks>
    public Func<CommandInfo, bool>? CommandFilter { get; set; }

    /// <summary>
    /// Gets or sets the parent command to filter commands by.
    /// </summary>
    /// <value>
    /// The <see cref="Type"/> of a command whose children should be used by the <see cref="CommandManager"/>
    /// class, or <see langword="null"/> to use commands without a parent.
    /// </value>
    /// <remarks>
    /// <para>
    ///   The <see cref="CommandManager"/> class will only consider commands whose parent, as
    ///   set using the <see cref="ParentCommandAttribute"/> attribute, matches this type. If
    ///   this property is <see langword="null"/>, only commands that do not have a the
    ///   <see cref="ParentCommandAttribute"/> attribute are considered.
    /// </para>
    /// <para>
    ///   All other commands are filtered out and will not be returned, created, or executed
    ///   by the command manager.
    /// </para>
    /// </remarks>
    public Type? ParentCommand { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether a version command should automatically be
    /// created.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically create a version command; otherwise,
    /// <see langword="false"/>. The default is <see langword="true"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   If this property is true, a command named "version" will be automatically added to
    ///   the list of available commands, unless a command with that name already exists.
    ///   When invoked, the command will show version information for the application, based
    ///   on the entry point assembly.
    /// </para>
    /// </remarks>
    public bool AutoVersionCommand { get; set; } = true;

    /// <summary>
    /// Gets or sets a value that indicates whether unique prefixes of a command name are
    /// automatically used as aliases.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> to automatically use unique prefixes of a command as aliases
    ///   for that argument; otherwise <see langword="false"/>. The default value is
    ///   <see langword="true"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   If this property is <see langword="true"/>, the <see cref="CommandManager"/> class
    ///   will consider any prefix that uniquely identifies a command by its name or one of its
    ///   explicit aliases as an alias for that argument. For example, given two commands "read"
    ///   and "record", "rea" would be an alias for "read", and "rec" an alias for
    ///   "record" (as well as "reco" and "recor"). Both "r" and "re" would not be an alias
    ///   because they don't uniquely identify a single command.
    /// </para>
    /// </remarks>
    public bool AutoCommandPrefixAliases { get; set; } = true;

    internal string AutoVersionCommandName()
    {
        return CommandNameTransform.Apply(StringProvider.AutomaticVersionCommandName());
    }
}
