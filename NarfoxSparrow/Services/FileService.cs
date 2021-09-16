using Newtonsoft.Json;
using System;
using System.IO;

namespace NarfoxSparrow.Services
{
    public class FileService
    {
        static FileService instance;

        private FileService() { }

        public static FileService Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new FileService();
                }
                return instance;
            }
        }

        public T LoadFile<T>(string path)
        {
            var filetext = LoadText(path);
            var inflated = JsonConvert.DeserializeObject<T>(filetext);
            LogService.Instance.Info($"Loaded file: {path}");
            return inflated;
        }

        public void SaveFile(object model, string path)
        {
            var json = JsonConvert.SerializeObject(model);
            SaveText(path, json);
            LogService.Instance.Info($"Saved file: {path}");
        }

        string LoadText(string path)
        {
            string text;
            try
            {
                text = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                var msg = string.Format("Failed to load from {0}, error: {1}", path, e);
                LogService.Instance.Error(msg);
                text = "";
            }
            return text;
        }

        bool SaveText(string path, string text)
        {
            bool success = false;
            try
            {
                File.WriteAllText(path, text);
                success = true;
            }
            catch (Exception e)
            {
                var msg = string.Format("Failed to save to {0}, error: {1}", path, e);
                LogService.Instance.Error(msg);
            }

            return success;
        }
    }
}
