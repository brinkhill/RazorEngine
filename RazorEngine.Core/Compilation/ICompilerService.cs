﻿namespace RazorEngine.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the required contract for implementing a compiler service.
    /// </summary>
    public interface ICompilerService
    {
        #region Properties
        /// <summary>
        /// Gets or sets whether the compiler service is operating in debug mode.
        /// </summary>
        bool Debug { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Builds a type name for the specified template type and model type.
        /// </summary>
        /// <param name="templateType">The template type.</param>
        /// <param name="modelType">The model type.</param>
        /// <returns>The string type name (including namespace).</returns>
        string BuildTypeName(Type templateType, Type modelType);

        /// <summary>
        /// Compiles the type defined in the specified type context.
        /// </summary>
        /// <param name="context">The type context which defines the type to compile.</param>
        /// <returns>The compiled type.</returns>
        Tuple<Type, Assembly> CompileType(TypeContext context);

        string CompileTypeAndReturnSource(TypeContext context);

        /// <summary>
        /// Returns a set of assemblies that must be referenced by the compiled template.
        /// </summary>
        /// <returns>The set of assemblies.</returns>
        IEnumerable<string> IncludeAssemblies();
        #endregion
    }
}