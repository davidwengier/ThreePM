﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace starH45.net.mp3.utilities.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("starH45.net.mp3.utilities.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to &lt;html&gt;
        ///&lt;head&gt;
        ///    &lt;title&gt;{Title} - {Artist} - starH45.net.mp3&lt;/title&gt;
        ///    &lt;style type=&quot;text/css&quot;&gt;
        ///	table
        ///	   { margin-top:0px;
        ///	     padding-top:0px;
        ///	     border-style:solid;
        ///	     border-color:#DDDDDD;
        ///	     border-left-width:1px;
        ///	     border-bottom-width:1px;
        ///	     border-top-width:1px;
        ///	     border-right-width:1px; }
        ///	table.noborder
        ///	   { border-width:0px; }
        ///	td
        ///	   { color:#FFFFEC; 
        ///	     vertical-align:top;
        ///	     margin:0px; 
        ///	     padding:0px;
        ///	     border:0px;
        ///	     font-family [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string HttpServerTemplate {
            get {
                return ResourceManager.GetString("HttpServerTemplate", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap LoadingAlbumArt {
            get {
                object obj = ResourceManager.GetObject("LoadingAlbumArt", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap NoAlbumArt {
            get {
                object obj = ResourceManager.GetObject("NoAlbumArt", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
