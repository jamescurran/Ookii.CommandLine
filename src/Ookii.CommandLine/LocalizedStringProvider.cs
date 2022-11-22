﻿using Ookii.CommandLine.Commands;
using Ookii.CommandLine.Properties;
using System;
using System.Globalization;
using System.Reflection;

namespace Ookii.CommandLine
{
    /// <summary>
    /// Provides custom localized strings for error messages and usage help.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Inherit from this class and override its members to provide customized or localized
    ///   strings. You can specify the implementation to use using <see cref="ParseOptions.StringProvider"/>.
    /// </para>
    /// <note>
    ///   For error messages, this only lets you customize error messages for the
    ///   <see cref="CommandLineArgumentException"/> class. Other exceptions thrown by this library,
    ///   such as for invalid argument definitions, constitute bugs and should not occur in a
    ///   correct program, and should therefore not be shown to the user.
    /// </note>
    /// </remarks>
    public partial class LocalizedStringProvider
    {
        /// <summary>
        /// Gets the name of the help argument created if the <see cref="ParseOptions.AutoHelpArgument"/>
        /// or <see cref="ParseOptionsAttribute.AutoHelpArgument"/> property is <see langword="true"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public virtual string AutomaticHelpName() => Resources.AutomaticHelpName;

        /// <summary>
        /// Gets the short name of the help argument created if the <see cref="ParseOptions.AutoHelpArgument"/>
        /// property is <see langword="true"/>, typically '?'.
        /// </summary>
        /// <returns>The string.</returns>
        /// <remarks>
        /// <para>
        ///   The argument will automatically have a short alias that is the lower case first
        ///   character of the value returned by <see cref="AutomaticHelpName"/>. If this character
        ///   is the same according to the argument name comparer, then no alias is added.
        /// </para>
        /// <para>
        ///   If <see cref="CommandLineParser.Mode"/> is not <see cref="ParsingMode.LongShort"/>,
        ///   the short name and the short alias will be used as a regular aliases instead.
        /// </para>
        /// </remarks>
        public virtual char AutomaticHelpShortName() => Resources.AutomaticHelpShortName[0];

        /// <summary>
        /// Gets the description of the help argument created if the <see cref="ParseOptions.AutoHelpArgument"/>
        /// property is <see langword="true"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public virtual string AutomaticHelpDescription() => Resources.AutomaticHelpDescription;

        /// <summary>
        /// Gets the name of the version argument created if the <see cref="ParseOptions.AutoVersionArgument"/>
        /// property is <see langword="true"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public virtual string AutomaticVersionName() => Resources.AutomaticVersionName;

        /// <summary>
        /// Gets the description of the version argument created if the <see cref="ParseOptions.AutoVersionArgument"/>
        /// property is <see langword="true"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public virtual string AutomaticVersionDescription() => Resources.AutomaticVersionDescription;

        /// <summary>
        /// Gets the name of the version command created if the <see cref="CommandOptions.AutoVersionCommand"/>
        /// property is <see langword="true"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public virtual string AutomaticVersionCommandName() => Resources.AutomaticVersionCommandName;

        /// <summary>
        /// Gets the description of the version command created if the <see cref="CommandOptions.AutoVersionCommand"/>
        /// property is <see langword="true"/>.
        /// </summary>
        /// <returns>The string.</returns>
        public virtual string AutomaticVersionCommandDescription() => Resources.AutomaticVersionDescription;

        /// <summary>
        /// Gets the name and version of the application, used by the automatic version argument
        /// and command.
        /// </summary>
        /// <param name="assembly">The assembly whose version to use.</param>
        /// <param name="friendlyName">
        /// The friendly name of the application; typically the value of the <see cref="CommandLineParser.ApplicationFriendlyName"/>
        /// property.
        /// </param>
        /// <returns>The string.</returns>
        /// <remarks>
        /// <para>
        ///   The base implementation uses the <see cref="AssemblyInformationalVersionAttribute"/>,
        ///   and will fall back to the assembly version if none is defined.
        /// </para>
        /// </remarks>
        public virtual string ApplicationNameAndVersion(Assembly assembly, string friendlyName)
        {
            var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = versionAttribute?.InformationalVersion ?? assembly.GetName().Version?.ToString() ?? string.Empty;
            return $"{friendlyName} {version}";
        }

        /// <summary>
        /// Gets the copyright information for the application, used by the automatic version
        /// argument and command.
        /// </summary>
        /// <param name="assembly">The assembly whose copyright information to use.</param>
        /// <returns>The string.</returns>
        /// <remarks>
        /// <para>
        ///   The base implementation returns the value of the <see cref="AssemblyCopyrightAttribute"/>,
        ///   or <see langword="null"/> if none is defined.
        /// </para>
        /// </remarks>
        public virtual string? ApplicationCopyright(Assembly assembly)
            => assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;

        private static string Format(string format, object? arg0)
            => string.Format(CultureInfo.CurrentCulture, format, arg0);

        private static string Format(string format, object? arg0, object? arg1)
            => string.Format(CultureInfo.CurrentCulture, format, arg0, arg1);

        private static string Format(string format, object? arg0, object? arg1, object? arg2)
            => string.Format(CultureInfo.CurrentCulture, format, arg0, arg1, arg2);
    }
}
