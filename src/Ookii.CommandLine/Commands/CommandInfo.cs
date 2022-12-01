﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Ookii.CommandLine.Commands
{
    /// <summary>
    /// Provides information about a subcommand.
    /// </summary>
    /// <seealso cref="CommandManager"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="CommandAttribute"/>
    public struct CommandInfo
    {
        private readonly CommandManager _manager;
        private readonly string _name;
        private readonly Type _commandType;
        private readonly CommandAttribute _attribute;
        private string? _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInfo"/> structure.
        /// </summary>
        /// <param name="commandType">The type that implements the subcommand.</param>
        /// <param name="manager">
        ///   The <see cref="CommandManager"/> that is managing this command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="commandType"/> or <paramref name="manager"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="commandType"/> is not a command type.
        /// </exception>
        public CommandInfo(Type commandType, CommandManager manager)
            : this(commandType, GetCommandAttributeOrThrow(commandType), manager)
        {
        }

        private CommandInfo(string name, Type commandType, string description, CommandManager manager)
        {
            _manager = manager;
            _attribute = GetCommandAttribute(commandType)!;
            _name = name;
            _commandType = commandType;
            _description = description;
        }

        private CommandInfo(Type commandType, CommandAttribute attribute, CommandManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _name = GetName(attribute, commandType, manager.Options);
            _commandType = commandType;
            _description = null;
            _attribute = attribute;
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>
        /// The name of the command.
        /// </value>
        /// <remarks>
        /// <para>
        ///   The name is taken from the <see cref="CommandAttribute.CommandName"/> property. If
        ///   that property is <see langword="null"/>, the name is determined by taking the command
        ///   type's name, and applying the transformation specified by the <see cref="CommandOptions.CommandNameTransform"/>
        ///   property.
        /// </para>
        /// </remarks>
        public string Name => _name;

        /// <summary>
        /// Gets the type that implements the command.
        /// </summary>
        /// <value>
        /// The type that implements the command.
        /// </value>
        public Type CommandType => _commandType;

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        /// <value>
        /// The description of the command, determined using the <see cref="DescriptionAttribute"/>
        /// attribute.
        /// </value>
        public string? Description => _description ??= GetCommandDescription();

        /// <summary>
        /// Gets a value that indicates if the command uses custom parsing.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the command type implements the <see cref="ICommandWithCustomParsing"/>
        /// interface; otherwise, <see langword="false"/>.
        /// </value>
        public bool UseCustomArgumentParsing => _commandType.ImplementsInterface(typeof(ICommandWithCustomParsing));

        /// <summary>
        /// Gets or sets a value that indicates whether the command is hidden from the usage help.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the command is hidden from the usage help; otherwise,
        /// <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   A hidden command will not be included in the command list when usage help is
        ///   displayed, but can still be invoked from the command line.
        /// </para>
        /// </remarks>
        /// <seealso cref="CommandAttribute.IsHidden"/>
        public bool IsHidden => _attribute.IsHidden;

        /// <summary>
        /// Gets the alternative names of this command.
        /// </summary>
        /// <value>
        /// A list of aliases.
        /// </value>
        /// <remarks>
        /// <para>
        ///   Aliases for a command are specified by using the <see cref="AliasAttribute"/> on a
        ///   class implementing the <see cref="ICommand"/> interface.
        /// </para>
        /// </remarks>
        public IEnumerable<string> Aliases => _commandType.GetCustomAttributes<AliasAttribute>().Select(a => a.Alias);

        /// <summary>
        /// Creates an instance of the command type.
        /// </summary>
        /// <param name="args">The arguments to the command.</param>
        /// <param name="index">The index in <paramref name="args"/> at which to start parsing the arguments.</param>
        /// <returns>An instance of the <see cref="CommandType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="args"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> does not fall inside the bounds of <paramref name="args"/>.</exception>
        public ICommand CreateInstance(string[] args, int index)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (index < 0 || index > args.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (UseCustomArgumentParsing)
            {
                var command = (ICommandWithCustomParsing)Activator.CreateInstance(CommandType)!;
                command.Parse(args, index, _manager.Options);
                return command;
            }

            return (ICommand)CommandLineParser.ParseInternal(CommandType, args, index, _manager.Options)!;
        }

        /// <summary>
        /// Creates a <see cref="CommandLineParser"/> instance that can be used to instantiate
        /// </summary>
        /// <returns>
        /// A <see cref="CommandLineParser"/> instance for the <see cref="CommandType"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   The command uses the <see cref="ICommandWithCustomParsing"/> interface.
        /// </exception>
        /// <remarks>
        /// <para>
        ///   If the <see cref="UseCustomArgumentParsing"/> property is <see langword="true"/>, the
        ///   command cannot be created suing the <see cref="CommandLineParser"/> class, and you
        ///   must use the <see cref="CreateInstance"/> method.
        /// </para>
        /// </remarks>
        public CommandLineParser CreateParser()
        {
            if (UseCustomArgumentParsing)
            {
                throw new InvalidOperationException(Properties.Resources.NoParserForCustomParsingCommand);
            }

            return new CommandLineParser(CommandType, _manager.Options);
        }

        /// <summary>
        /// Checks whether the command's name or aliases match the specified name.
        /// </summary>
        /// <param name="name">The name to check for.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use for the comparisons, or <see langword="null"/>
        /// to use the default comparison, which is <see cref="StringComparer.OrdinalIgnoreCase"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="name"/> matches the <see cref="Name"/>
        /// property or any of the items in the <see cref="Aliases"/> property.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        public bool MatchesName(string name, IComparer<string>? comparer = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            comparer ??= StringComparer.OrdinalIgnoreCase;
            if (comparer.Compare(name, _name) == 0)
            {
                return true;
            }

            return Aliases.Any(alias => comparer.Compare(name, alias) == 0);
        }

        /// <summary>
        /// Creates an instance of the <see cref="CommandInfo"/> structure only if <paramref name="commandType"/>
        /// represents a command type.
        /// </summary>
        /// <param name="commandType">The type that implements the subcommand.</param>
        /// <param name="manager">
        ///   The <see cref="CommandManager"/> that is managing this command.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="commandType"/> or <paramref name="manager"/> is <see langword="null"/>.
        /// </exception>
        /// <returns>
        ///   A <see cref="CommandInfo"/> structure with information about the command, or
        ///   <see langword="null"/> if <paramref name="commandType"/> was not a command.
        /// </returns>
        public static CommandInfo? TryCreate(Type commandType, CommandManager manager)
        {
            var attribute = GetCommandAttribute(commandType);
            if (attribute == null)
            {
                return null;
            }

            return new CommandInfo(commandType, attribute, manager);
        }

        /// <summary>
        /// Returns a value indicating if the specified type is a subcommand.
        /// </summary>
        /// <param name="commandType">The type that implements the subcommand.</param>
        /// <returns>
        /// <see langword="true"/> if the type implements the <see cref="ICommand"/> interface and
        /// has the <see cref="CommandAttribute"/> applied; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="commandType"/> is <see langword="null"/>.
        /// </exception>
        public static bool IsCommand(Type commandType)
        {
            return GetCommandAttribute(commandType) != null;
        }

        internal static CommandInfo GetAutomaticVersionCommand(CommandManager manager)
        {
            var name = manager.Options.AutoVersionCommandName();
            var description = manager.Options.StringProvider.AutomaticVersionCommandDescription();
            return new CommandInfo(name, typeof(AutomaticVersionCommand), description, manager);
        }

        private static CommandAttribute? GetCommandAttribute(Type commandType)
        {
            if (commandType == null)
            {
                throw new ArgumentNullException(nameof(commandType));
            }

            if (commandType.IsAbstract || !commandType.ImplementsInterface(typeof(ICommand)))
            {
                return null;
            }

            return commandType.GetCustomAttribute<CommandAttribute>();
        }

        private static CommandAttribute GetCommandAttributeOrThrow(Type commandType)
        {
            return GetCommandAttribute(commandType) ??
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Properties.Resources.TypeIsNotCommandFormat, commandType.FullName));
        }

        private static string GetName(CommandAttribute attribute, Type commandType, CommandOptions? options)
        {
            return attribute.CommandName ??
                options?.CommandNameTransform.Apply(commandType.Name, options.StripCommandNameSuffix) ??
                commandType.Name;
        }

        private string? GetCommandDescription()
        {
            return _commandType.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }
    }
}