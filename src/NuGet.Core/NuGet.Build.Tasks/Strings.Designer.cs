﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuGet.Build.Tasks {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuGet.Build.Tasks.Strings", typeof(Strings).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ProjectReference &apos;{0}&apos; was resolved using &apos;{1}&apos; instead of the project target framework &apos;{2}&apos;. This project may not be fully compatible with your project..
        /// </summary>
        internal static string ImportsFallbackWarning {
            get {
                return ResourceManager.GetString("ImportsFallbackWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project &apos;{0}&apos; targets &apos;{2}&apos;. It cannot be referenced by a project that targets &apos;{1}&apos;..
        /// </summary>
        internal static string NoCompatibleTargetFramework {
            get {
                return ResourceManager.GetString("NoCompatibleTargetFramework", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find a project to restore!.
        /// </summary>
        internal static string NoProjectsProvidedToTask {
            get {
                return ResourceManager.GetString("NoProjectsProvidedToTask", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nothing to do. None of the projects specified contain packages to restore..
        /// </summary>
        internal static string NoProjectsToRestore {
            get {
                return ResourceManager.GetString("NoProjectsToRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Restore canceled!.
        /// </summary>
        internal static string RestoreCanceled {
            get {
                return ResourceManager.GetString("RestoreCanceled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The project fallback framework &apos;{0}&apos; is not a supported target framework..
        /// </summary>
        internal static string UnsupportedFallbackFramework {
            get {
                return ResourceManager.GetString("UnsupportedFallbackFramework", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The project target framework &apos;{0}&apos; is not a supported target framework..
        /// </summary>
        internal static string UnsupportedTargetFramework {
            get {
                return ResourceManager.GetString("UnsupportedTargetFramework", resourceCulture);
            }
        }
    }
}
