﻿#if NET7_0_OR_GREATER

using System;

namespace Ookii.CommandLine;

/// <summary>
/// Defines a mechanism to parse command line arguments into a type.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
/// <remarks>
/// <note>
///   This type is only available when using .Net 7 or later.
/// </note>
/// <para>
///   This interface is automatically implemented on a class (on .Net 7 and later only) when the
///   <see cref="GeneratedParserAttribute"/> is used. Classes without that attribute must parse
///   arguments using the <see cref="CommandLineParser.Parse{T}(Ookii.CommandLine.ParseOptions?)"/>
///   method, or create the parser directly by using the <see cref="CommandLineParser{T}.CommandLineParser(Ookii.CommandLine.ParseOptions?)"/>
///   constructor directly; these classes do not support this interface unless it is manually
///   implemented.
/// </para>
/// </remarks>
public interface IParser<TSelf> : IParserProvider<TSelf>
    where TSelf : class, IParser<TSelf>
{
    /// <summary>
    /// Parses the arguments returned by the <see cref="Environment.GetCommandLineArgs"/>
    /// method using the type <typeparamref name="TSelf"/>.
    /// </summary>
    /// <param name="options">
    ///   The options that control parsing behavior and usage help formatting. If
    ///   <see langword="null" />, the default options are used.
    /// </param>
    /// <returns>
    ///   An instance of the type <typeparamref name="TSelf"/>, or <see langword="null"/> if an
    ///   error occurred, or argument parsing was canceled by the <see cref="CommandLineArgumentAttribute.CancelParsing"/>
    ///   property or a method argument that returned <see langword="false"/>.
    /// </returns>
    /// <seealso cref="CommandLineParser.Parse{T}(ParseOptions?)"/>
    public static abstract TSelf? Parse(ParseOptions? options = null);

    /// <summary>
    /// Parses the specified command line arguments using the type <typeparamref name="TSelf"/>.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <param name="options">
    ///   The options that control parsing behavior and usage help formatting. If
    ///   <see langword="null" />, the default options are used.
    /// </param>
    /// <returns>
    ///   An instance of the type <typeparamref name="TSelf"/>, or <see langword="null"/> if an
    ///   error occurred, or argument parsing was canceled by the <see cref="CommandLineArgumentAttribute.CancelParsing"/>
    ///   property or a method argument that returned <see langword="false"/>.
    /// </returns>
    /// <seealso cref="CommandLineParser.Parse{T}(string[], ParseOptions?)"/>
    public static abstract TSelf? Parse(string[] args, ParseOptions? options = null);

    /// <summary>
    /// Parses the specified command line arguments, starting at the specified index, using the
    /// type <typeparamref name="TSelf"/>.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <param name="index">The index of the first argument to parse.</param>
    /// <param name="options">
    ///   The options that control parsing behavior and usage help formatting. If
    ///   <see langword="null" />, the default options are used.
    /// </param>
    /// <returns>
    ///   An instance of the type <typeparamref name="TSelf"/>, or <see langword="null"/> if an
    ///   error occurred, or argument parsing was canceled by the <see cref="CommandLineArgumentAttribute.CancelParsing"/>
    ///   property or a method argument that returned <see langword="false"/>.
    /// </returns>
    /// <seealso cref="CommandLineParser.Parse{T}(string[], ParseOptions?)"/>
    public static abstract TSelf? Parse(string[] args, int index, ParseOptions? options = null);
}

#endif
