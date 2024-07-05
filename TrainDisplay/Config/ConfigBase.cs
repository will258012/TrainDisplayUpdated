using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TrainDisplay.Utils;
namespace TrainDisplay.Config
{
    public abstract class ConfigBase<T> where T : ConfigBase<T>, new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Load();
                }

                return instance;
            }
        }
        private static T Load()
        {
            if (instance == null)
            {
                var configPath = GetConfigPath();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                try
                {
                    if (File.Exists(configPath))
                    {
                        using (StreamReader streamReader = new StreamReader(configPath))
                        {
                            instance = xmlSerializer.Deserialize(streamReader) as T;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }
            return instance ?? (instance = new T());
        }

        public static void Save()
        {
            if (instance == null) return;

            var configPath = GetConfigPath();

            var xmlSerializer = new XmlSerializer(typeof(T));
            var noNamespaces = new XmlSerializerNamespaces();
            noNamespaces.Add("", "");
            try
            {
                using (var streamWriter = new StreamWriter(configPath))
                {
                    xmlSerializer.Serialize(streamWriter, instance, noNamespaces);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private static string GetConfigPath()
        {
            if (typeof(T).GetCustomAttributes(typeof(ConfigPathAttribute), true)
                .FirstOrDefault() is ConfigPathAttribute configPathAttribute)
            {
                return configPathAttribute.Value;
            }
            else
            {
                Log.Error("ConfigPath attribute missing in " + typeof(T).Name);
                return typeof(T).Name + ".xml";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigPathAttribute : Attribute
    {
        public ConfigPathAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}