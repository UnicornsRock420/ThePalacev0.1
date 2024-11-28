using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

namespace ThePalace.Core.Utility
{
    public static class ConfigManager
    {
        private static volatile ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

        private static DateTime _UpdateDate;
        private static UInt32 _kvTTL;

        private static void CheckTTL()
        {
            if (_kvTTL == 0 || DateTime.UtcNow.Subtract(_UpdateDate).Minutes > _kvTTL)
            {
                try
                {
                    _kvTTL = GetValue<uint>("AppCacheTTL", 3, true).Value;
                    _UpdateDate = DateTime.UtcNow;
                }
                catch
                {
                }
            }
        }

        public static string GetConnectionString(this string key, Type startingClass = null)
        {
            var value = (string)null;

            var actions = new List<Action>
            {
                () => {
                    value = ConfigurationManager.ConnectionStrings[key].ConnectionString;
                },
                () =>
                {
                    if (startingClass == null) return;

                    var assemblyName = startingClass.Assembly.GetName().Name;
                    var path = Path.Combine(Environment.CurrentDirectory, $"{assemblyName}.dll.config");

                    if (!File.Exists(path)) return;

                    using (var streamReader = new StreamReader(path))
                    {
                        var xml = streamReader.ReadToEnd();
                        var doc = new XmlDocument();
                        doc.LoadXml(xml);
                        var json = JsonConvert.SerializeXmlNode(doc);
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

                        for (var j = 0; j < jsonObject.configuration.connectionStrings.add.Count; j++)
                        {
                            if (jsonObject.configuration.connectionStrings.add[j]["@name"].ToString() == key)
                            {
                                value = jsonObject.configuration.connectionStrings.add[j]["@connectionString"].ToString();

                                break;
                            }
                        }
                    }
                },
                () =>
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "appSettings.json");

                    if (!File.Exists(path)) return;

                    using (var streamReader = new StreamReader(path))
                    {
                        var json = streamReader.ReadToEnd();
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

                        value = (string)jsonObject.ConnectionStrings[key];
                    }
                },
            };

            foreach (var action in actions)
            {
                try
                {
                    action();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }
                catch { }
            }

            return null;
        }

        public static string GetValue(this string key, string defaultValue = null, bool bypassCache = false, Type startingClass = null)
        {
            if (!bypassCache)
            {
                CheckTTL();

                if (_cache.ContainsKey(key))
                {
                    return (string)_cache[key];
                }
            }

            var value = (string)null;

            var actions = new List<Action>
            {
                () => {
                    value = ConfigurationManager.AppSettings[key];
                },
                () =>
                {
                    if (startingClass == null) return;

                    var assemblyName = startingClass.Assembly.GetName().Name;
                    var path = Path.Combine(Environment.CurrentDirectory, $"{assemblyName}.dll.config");

                    if (!File.Exists(path)) return;

                    using (var streamReader = new StreamReader(path))
                    {
                        var xml = streamReader.ReadToEnd();
                        var doc = new XmlDocument();
                        doc.LoadXml(xml);
                        var json = JsonConvert.SerializeXmlNode(doc);
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

                        for (var j = 0; j < jsonObject.configuration.appSettings.add.Count; j++)
                        {
                            if (jsonObject.configuration.appSettings.add[j]["@key"].ToString() == key)
                            {
                                value = jsonObject.configuration.appSettings.add[j]["@value"].ToString();

                                break;
                            }
                        }
                    }
                },
                () =>
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "appSettings.json");

                    if (!File.Exists(path)) return;

                    using (var streamReader = new StreamReader(path))
                    {
                        var json = streamReader.ReadToEnd();
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

                        value = (string)jsonObject[key];
                    }
                },
                //() =>
                //{
                //    using (var dbContext = DbConnection.For<ThePalaceEntities>())
                //    {
                //        value = dbContext.Config.AsNoTracking()
                //            .Where(c => c.Key == key)
                //            .Select(c => c.Value)
                //            .FirstOrDefault();
                //    }
                //},
            };

            foreach (var action in actions)
            {
                try
                {
                    action();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        lock (_cache)
                        {
                            _cache[key] = value;
                        }

                        return value;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return defaultValue;
        }

        public static T? GetValue<T>(this string key, T? defaultValue = null, bool bypassCache = false, Type startingClass = null) where T : struct
        {
            if (!bypassCache)
            {
                CheckTTL();

                if (_cache.ContainsKey(key))
                {
                    return (T)_cache[key];
                }
            }

            var value = (string)null;

            var actions = new List<Action>
            {
                () => {
                    value = ConfigurationManager.AppSettings[key];
                },
                () =>
                {
                    if (startingClass == null) return;

                    var assemblyName = startingClass.Assembly.GetName().Name;
                    var path = Path.Combine(Environment.CurrentDirectory, $"{assemblyName}.dll.config");

                    if (!File.Exists(path)) return;

                    using (var streamReader = new StreamReader(path))
                    {
                        var xml = streamReader.ReadToEnd();
                        var doc = new XmlDocument();
                        doc.LoadXml(xml);
                        var json = JsonConvert.SerializeXmlNode(doc);
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

                        for (var j = 0; j < jsonObject.configuration.appSettings.add.Count; j++)
                        {
                            if (jsonObject.configuration.appSettings.add[j]["@key"].ToString() == key)
                            {
                                value = jsonObject.configuration.appSettings.add[j]["@value"].ToString().TryParse<T>();

                                break;
                            }
                        }
                    }
                },
                () =>
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "appSettings.json");

                    if (!File.Exists(path)) return;

                    using (var streamReader = new StreamReader(path))
                    {
                        var json = streamReader.ReadToEnd();
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

                        value = jsonObject[key].TryParse<T>();
                    }
                },
                //() =>
                //{
                //    using (var dbContext = DbConnection.For<ThePalaceEntities>())
                //    {
                //        value = dbContext.Config.AsNoTracking()
                //            .Where(c => c.Key == key)
                //            .Select(c => c.Value)
                //            .FirstOrDefault();
                //    }
                //},
            };

            foreach (var action in actions)
            {
                try
                {
                    action();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        lock (_cache)
                        {
                            return (T)(_cache[key] = value.TryParse<T>());
                        }

                    }
                }
                catch { }
            }

            return defaultValue ?? default(T);
        }

        public static void SetValue(this string key, string value)
        {
            //using (var dbContext = DbConnection.For<ThePalaceEntities>())
            //{
            //    var cfg = dbContext.Config
            //        .Where(c => c.Key == key)
            //        .SingleOrDefault();

            //    if (cfg.Value != value)
            //    {
            //        cfg.Value = value;
            //    }

            //    dbContext.SaveChanges();
            //}
        }
    }
}
