﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TMServer {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.9.0.0")]
    internal sealed partial class ServerConfig : global::System.Configuration.ApplicationSettingsBase {
        
        private static ServerConfig defaultInstance = ((ServerConfig)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ServerConfig())));
        
        public static ServerConfig Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6665")]
        public ushort AuthPort {
            get {
                return ((ushort)(this["AuthPort"]));
            }
            set {
                this["AuthPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6666")]
        public ushort ApiPort {
            get {
                return ((ushort)(this["ApiPort"]));
            }
            set {
                this["ApiPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6667")]
        public ushort LongPollPort {
            get {
                return ((ushort)(this["LongPollPort"]));
            }
            set {
                this["LongPollPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120")]
        public ushort LongPollLifeTimeSeconds {
            get {
                return ((ushort)(this["LongPollLifeTimeSeconds"]));
            }
            set {
                this["LongPollLifeTimeSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("48")]
        public ushort TokenLifetimeHours {
            get {
                return ((ushort)(this["TokenLifetimeHours"]));
            }
            set {
                this["TokenLifetimeHours"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("48")]
        public ushort RsaKeyLifetimeHours {
            get {
                return ((ushort)(this["RsaKeyLifetimeHours"]));
            }
            set {
                this["RsaKeyLifetimeHours"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NySzq6hatK")]
        public string PasswordSalt {
            get {
                return ((string)(this["PasswordSalt"]));
            }
            set {
                this["PasswordSalt"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.0.1")]
        public string Version {
            get {
                return ((string)(this["Version"]));
            }
            set {
                this["Version"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Host=localhost;Port=5432;Database=tmdb;Username=tmadmin;Password=1234;Include Err" +
            "or Detail=True;")]
        public string DbConnectionString {
            get {
                return ((string)(this["DbConnectionString"]));
            }
            set {
                this["DbConnectionString"] = value;
            }
        }
    }
}
