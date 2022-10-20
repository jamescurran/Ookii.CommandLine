﻿// Copyright (c) Sven Groot (Ookii.org)
using System;

namespace Ookii.CommandLine
{
    /// <summary>
    /// Indicates a property of a class defines a command line argument.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CommandLineArgumentAttribute : Attribute
    {
        private readonly string? _argumentName;
        private bool _short;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentAttribute"/> class using the property name as the argument name.
        /// </summary>
        public CommandLineArgumentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentAttribute"/> class using the specified argument name.
        /// </summary>
        /// <param name="argumentName">The name of the argument, or <see langword="null"/> to indicate the property name should be used.</param>
        public CommandLineArgumentAttribute(string? argumentName)
        {
            Position = -1;
            _argumentName = argumentName;
        }

        /// <summary>
        /// Gets the name of the argument's command switch.
        /// </summary>
        /// <value>
        /// The name of the command switch used to set the argument, or <see langword="null"/> if the property name should be used.
        /// </value>
        public string? ArgumentName
        {
            get { return _argumentName; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the argument has a long name.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the argument has a long name; otherwise, <see langword="false"/>.
        /// The default value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <note>
        ///   This property is ignored if <see cref="CommandLineParser.Mode"/> is not
        ///   <see cref="ParsingMode.LongShort"/>.
        /// </note>
        /// </remarks>
        public bool Long { get; set; } = true;
        
        /// <summary>
        /// Gets or sets a value that indicates whether the argument has a short name.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the argument has a short name; otherwise, <see langword="false"/>.
        /// The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <note>
        ///   This property is ignored if <see cref="CommandLineParser.Mode"/> is not
        ///   <see cref="ParsingMode.LongShort"/>.
        /// </note>
        /// <para>
        ///   If <see cref="ShortName"/> is not set but this property is set to <see langword="true"/>,
        ///   the short name will be derived using the first character of the long name.
        /// </para>
        /// </remarks>
        public bool Short
        {
            get => _short || ShortName != '\0';
            set => _short = value;
        }

        /// <summary>
        /// Gets or sets the argument's short name.
        /// </summary>
        /// <value>The short name, or a null character ('\0') if the argument has no short name.</value>
        /// <remarks>
        /// <note>
        ///   This property is ignored if <see cref="CommandLineParser.Mode"/> is not
        ///   <see cref="ParsingMode.LongShort"/>.
        /// </note>
        /// <para>
        ///   Setting this property implies <see cref="Short"/> is <see langword="true"/>.
        /// </para>
        /// </remarks>
        public char ShortName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the named argument is required.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the named argument must be supplied on the command line; otherwise, <see langword="false"/>.
        ///   The default value is <see langword="false"/>.
        /// </value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the position of a positional argument.
        /// </summary>
        /// <value>
        /// The position of the argument, or a negative value if the argument can only be specified by name. The default value is -1.
        /// </value>
        /// <remarks>
        /// <para>
        ///   The <see cref="Position"/> property specifies the relative position of the positional arguments created by properties. If
        ///   you skip any numbers, they will be ignored; if you have only two positional arguments with positions set to
        ///   4 and 7, they will be the first and second positional arguments, not the 4th and 7th. It is an error
        ///   to use the same number more than once.
        /// </para>
        /// <para>
        ///   If you have arguments defined by the type's constructor parameters, positional arguments defined by properties will
        ///   always come after them; for example, if you have two contructor parameter arguments and one property positional argument with
        ///   position 0, then that argument will actually be the third positional argument.
        /// </para>
        /// <para>
        ///   The <see cref="CommandLineArgument.Position"/> property will be set to reflect the actual position of the argument,
        ///   which may not match the value of the <see cref="Position"/> property.
        /// </para>
        /// </remarks>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the default value to be assigned to the property if the argument is not supplied on the command line.
        /// </summary>
        /// <value>
        /// The default value for the argument. The default value is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   The <see cref="DefaultValue"/> property will not be used if the <see cref="IsRequired"/> property is <see langword="true"/>,
        ///   or if the argument is a multi-value or dictionary argument.
        /// </para>
        /// <para>
        ///   By default, the command line usage help generated by <see cref="CommandLineParser.WriteUsage(System.IO.TextWriter,int,WriteUsageOptions)"/>
        ///   does not include the default value. Either manually add it to the description, or set the <see cref="WriteUsageOptions.IncludeDefaultValueInDescription"/>
        ///   property to <see langword="true"/>.
        /// </para>
        /// </remarks>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the description of the property's value to use when printing usage information.
        /// </summary>
        /// <value>
        /// The description of the value, or <see langword="null"/> to indicate that the property's type name should be used.
        /// </value>
        /// <remarks>
        /// <para>
        ///   The value description is a short (typically one word) description that indicates the type of value that
        ///   the user should supply. By default the type of the property is used. If the type is an array type, the
        ///   array's element type is used. If the type is a nullable type, the nullable type's underlying type is used.
        /// </para>
        /// <para>
        ///   The value description is used when printing usage. For example, the usage for an argument named Sample with
        ///   a value description of String would look like "-Sample &lt;String&gt;".
        /// </para>
        /// <note>
        ///   This is not the long description used to describe the purpose of the argument. That should be specified
        ///   using the <see cref="System.ComponentModel.DescriptionAttribute"/> attribute.
        /// </note>
        /// </remarks>
        public string? ValueDescription { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether argument parsing should be cancelled if
        /// this argument is encountered.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if argument parsing should be cancelled after this argument;
        /// otherwise, <see langword="false"/>. The default value is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// <para>
        ///   If this property is <see langword="true"/>, the <see cref="CommandLineParser"/> will
        ///   stop parsing the command line arguments after seeing this argument, and return
        ///   <see langword="null"/> from the <see cref="CommandLineParser.Parse(string[], int)"/> method
        ///   or one of its overloads. Since no instance of the arguments type is returned, it's
        ///   not possible to determine argument values, or which argument caused the cancellation,
        ///   except by inspecting the <see cref="CommandLineParser.Arguments"/> property.
        /// </para>
        /// <para>
        ///   This property is most commonly useful to implement a "-Help" or "-?" style switch
        ///   argument, where the presence of that argument causes usage help to be printed and
        ///   the program to exit, regardless of whether the rest of the command line is valid
        ///   or not.
        /// </para>
        /// <para>
        ///   The <see cref="CommandLineParser.Parse{T}(string[], ParseOptions?)"/> static helper method will print
        ///   usage information if parsing was cancelled through this method.
        /// </para>
        /// <para>
        ///   Cancelling parsing in this way is identical to handling the <see cref="CommandLineParser.ArgumentParsed"/>
        ///   event and setting <see cref="System.ComponentModel.CancelEventArgs.Cancel"/> to
        ///   <see langword="true" />.
        /// </para>
        /// <para>
        ///   It's possible to prevent cancellation when an argument has this property set by
        ///   handling the <see cref="CommandLineParser.ArgumentParsed"/> event and setting the
        ///   <see cref="ArgumentParsedEventArgs.OverrideCancelParsing"/> property to 
        ///   <see langword="true"/>.
        /// </para>
        /// </remarks>
        public bool CancelParsing { get; set; }
    }
}
