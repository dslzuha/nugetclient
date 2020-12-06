﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuGet.Protocol.VisualStudio {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Strings() {
        }
        
        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuGet.Protocol.VisualStudio.Strings", typeof(Strings).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Install failed. Rolling back....
        /// </summary>
        public static string ActionExecutor_RollingBack {
            get {
                return ResourceManager.GetString("ActionExecutor_RollingBack", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Unrecognized Package Action &apos;{0}&apos;..
        /// </summary>
        public static string ActionResolver_UnsupportedAction {
            get {
                return ResourceManager.GetString("ActionResolver_UnsupportedAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Unsupported Dependency Behavior &apos;{0}&apos;..
        /// </summary>
        public static string ActionResolver_UnsupportedDependencyBehavior {
            get {
                return ResourceManager.GetString("ActionResolver_UnsupportedDependencyBehavior", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The download URL for {0} &apos;{1}&apos; is invalid..
        /// </summary>
        public static string DownloadActionHandler_InvalidDownloadUrl {
            get {
                return ResourceManager.GetString("DownloadActionHandler_InvalidDownloadUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to No download URL could be found for {0}..
        /// </summary>
        public static string DownloadActionHandler_NoDownloadUrl {
            get {
                return ResourceManager.GetString("DownloadActionHandler_NoDownloadUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Cannot create a NuGet Repository from the Aggregate Source.
        /// </summary>
        public static string NuGetRepository_CannotCreateAggregateRepo {
            get {
                return ResourceManager.GetString("NuGetRepository_CannotCreateAggregateRepo", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The {0} service is not supported by this object..
        /// </summary>
        public static string NuGetServiceProvider_ServiceNotSupported {
            get {
                return ResourceManager.GetString("NuGetServiceProvider_ServiceNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Unrecognized Package Action &apos;{0}&apos;..
        /// </summary>
        public static string PackageActionDescriptionWrapper_UnrecognizedAction {
            get {
                return ResourceManager.GetString("PackageActionDescriptionWrapper_UnrecognizedAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The project &apos;{0}&apos; is not one of the projects targetted by this object..
        /// </summary>
        public static string ProjectInstallationTarget_ProjectIsNotTargetted {
            get {
                return ResourceManager.GetString("ProjectInstallationTarget_ProjectIsNotTargetted", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Service index document is missing the &apos;resources&apos; property..
        /// </summary>
        public static string Protocol_IndexMissingResourcesNode {
            get {
                return ResourceManager.GetString("Protocol_IndexMissingResourcesNode", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The source does not have a Registration Base Url defined!.
        /// </summary>
        public static string Protocol_MissingRegistrationBase {
            get {
                return ResourceManager.GetString("Protocol_MissingRegistrationBase", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The source does not have a Search service!.
        /// </summary>
        public static string Protocol_MissingSearchService {
            get {
                return ResourceManager.GetString("Protocol_MissingSearchService", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The path &apos;{0}&apos; for the selected source could not be resolved..
        /// </summary>
        public static string Protocol_Search_LocalSourceNotFound {
            get {
                return ResourceManager.GetString("Protocol_Search_LocalSourceNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The &apos;{0}&apos; installation feature was required by a package but is not supported on the current host..
        /// </summary>
        public static string RequiredFeatureUnsupportedException_DefaultMessageWithFeature {
            get {
                return ResourceManager.GetString("RequiredFeatureUnsupportedException_DefaultMessageWithFeature", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The installation host does not support a feature required by this package..
        /// </summary>
        public static string RequiredFeatureUnsupportedException_DefaultMessageWithoutFeature {
            get {
                return ResourceManager.GetString("RequiredFeatureUnsupportedException_DefaultMessageWithoutFeature", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to API v2 (legacy).
        /// </summary>
        public static string v2sourceDescription {
            get {
                return ResourceManager.GetString("v2sourceDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to API v3.
        /// </summary>
        public static string v3sourceDescription {
            get {
                return ResourceManager.GetString("v3sourceDescription", resourceCulture);
            }
        }
    }
}
