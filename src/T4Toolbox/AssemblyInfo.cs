// <copyright file="AssemblyInfo.cs" company="Oleg Sych">
//  Copyright © Oleg Sych. All Rights Reserved.
// </copyright>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle(T4Toolbox.AssemblyInfo.Name)]
[assembly: CLSCompliant(true)]

namespace T4Toolbox
{
    /// <summary>
    /// Defines constants for common assembly information.
    /// </summary>
    /// <remarks>
    /// This class is defined here instead of the CommonAssemblyInfo.cs because we need to use the
    /// <see cref="InternalsVisibleToAttribute"/> to enable access to internal members for testing.
    /// With internals visible to all projects in the solution, defining this class in every project
    /// generates compiler errors due to naming collisions.
    /// </remarks>
    internal abstract class AssemblyInfo
    {
        /// <summary>
        /// Name of the T4 Toolbox assembly.
        /// </summary>
        public const string Name = "T4Toolbox";

        /// <summary>
        /// Name of the product.
        /// </summary>
        public const string Product = "T4 Toolbox";

        /// <summary>
        /// Description of the product.
        /// </summary>
        public const string Description = "Extends code generation capabilities of Text Templates.";

#if SIGN_ASSEMBLY
        public const string PublicKey = ", PublicKey=002400000480000094000000060200000024000052534131000400000100010069e19ca99cac8ad7dca45d6bb7edcff99c2516b95b439451ba6e9acedc4acc6d539d8f3fa6d1071e56589910968391c33a8e0393cd54c4063347d931428f2e148b5d3e7d34aa92b0a825a0a580f6f86265efab9c41d3ddb378f8118fcddd1198442aff3227913e253c53888c77fdf572fcad92290f041b3050f2c6ae70407bbe";
        public const string PublicKeyToken = "d2bd2c2d7888b9f1";
#else
        public const string PublicKey = "";
        public const string PublicKeyToken = "";
#endif

        /// <summary>
        /// Version of the T4 Toolbox assembly.
        /// </summary>
        public const string Version = "17.0.0.0";
    }
}