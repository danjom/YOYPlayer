using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using YOYPlayer.Model.Helpers;

namespace YOYPlayer.Model
{
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class CustomSettingsProvider : SettingsProvider
    {
        const string NAME = "name";
        const string SERIALIZE_AS = "serializeAs";
        const string CONFIG = "configuration";
        const string USER_SETTINGS = "userSettings";
        const string SETTING = "setting";

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(ApplicationName, config);
        }

        public override string ApplicationName
        {
            get => System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name;
            set { }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            //using (XmlTextWriter tw = new XmlTextWriter(FoldersHelper.PathToSettings, Encoding.Unicode))
            //{
            //    tw.WriteStartDocument();
            //    tw.WriteStartElement("settings");
            //    foreach (SettingsPropertyValue propertyValue in collection)
            //    {
            //        if (IsUserScoped(propertyValue.Property))
            //        {
            //            tw.WriteStartElement(propertyValue.Name);
            //            tw.WriteValue(propertyValue.SerializedValue ?? string.Empty);
            //            tw.WriteEndElement();
            //        }
            //    }
            //    tw.WriteEndElement();
            //    tw.WriteEndDocument();
            //}

            SaveValuesToFile(collection);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            foreach (SettingsProperty property in collection)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(property);
                value.IsDirty = false;
                values.Add(value);
            }

            if (!Directory.Exists(FoldersHelper.PathToAppData))
                Directory.CreateDirectory(FoldersHelper.PathToAppData);

            if (!File.Exists(FoldersHelper.PathToSettings))
                CreateEmptyConfig();

            //load the xml
            var configXml = XDocument.Load(FoldersHelper.PathToSettings);

            //get all of the <setting name="..." serializeAs="..."> elements.
            var settingElements = configXml.Element(CONFIG).Element(USER_SETTINGS).Element(typeof(Properties.Settings).FullName).Elements(SETTING);

            foreach (SettingsPropertyValue value in values)
            {
                if (IsUserScoped(value.Property))
                    try
                    {
                        value.SerializedValue = settingElements
                            .FirstOrDefault(x => x.Attribute("name").Value == value.Name)?.Value 
                            ?? string.Empty;

                        var convert = TypeDescriptor.GetConverter(value.Property.PropertyType);

                        value.PropertyValue = convert.ConvertFromString(value.SerializedValue.ToString());
                    }
                    catch (XmlException)
                    { /* ugly */ }
            }

            return values;
        }

        /// <summary>
        /// Creates an empty user.config file...looks like the one MS creates.  
        /// This could be overkill a simple key/value pairing would probably do.
        /// </summary>
        private void CreateEmptyConfig()
        {
            var doc = new XDocument();
            var declaration = new XDeclaration("1.0", "utf-8", "true");
            var config = new XElement(CONFIG);
            var userSettings = new XElement(USER_SETTINGS);
            var group = new XElement(typeof(Properties.Settings).FullName);
            userSettings.Add(group);
            config.Add(userSettings);
            doc.Add(config);
            doc.Declaration = declaration;
            doc.Save(FoldersHelper.PathToSettings);
        }

        /// <summary>
        /// Saves the in memory dictionary to the user config file
        /// </summary>
        private void SaveValuesToFile(SettingsPropertyValueCollection collection)
        {
            //load the current xml from the file.
            var import = XDocument.Load(FoldersHelper.PathToSettings);

            //get the settings group (e.g. <Company.Project.Desktop.Settings>)
            var settingsSection = import.Element(CONFIG).Element(USER_SETTINGS).Element(typeof(Properties.Settings).FullName);

            //iterate though the dictionary, either updating the value or adding the new setting.
            foreach (SettingsPropertyValue entry in collection)
            {
                var setting = settingsSection.Elements().FirstOrDefault(e => e.Attribute(NAME).Value == entry.Name);
                if (setting == null) //this can happen if a new setting is added via the .settings designer.
                {
                    var newSetting = new XElement(SETTING);
                    newSetting.Add(new XAttribute(NAME, entry.Name));
                    newSetting.Add(new XAttribute(SERIALIZE_AS, entry.Property.SerializeAs));

                    var convert = TypeDescriptor.GetConverter(entry.Property.PropertyType);

                    newSetting.Value = convert.ConvertToString(entry.PropertyValue);
                    settingsSection.Add(newSetting);
                }
                else //update the value if it exists.
                {
                    var convert = TypeDescriptor.GetConverter(entry.Property.PropertyType);

                    setting.Value = convert.ConvertToString(entry.PropertyValue);
                }
            }
            import.Save(FoldersHelper.PathToSettings);
        }

        private bool IsUserScoped(SettingsProperty property)
        {
            return property.Attributes.ContainsKey(typeof(UserScopedSettingAttribute));
        }
    }
}
