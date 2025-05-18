using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Save.Surrogates;
using UnityEngine;

public class Storage
{
    private string directory;
    private BinaryFormatter formatter;

    public Storage()
    {
        directory = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        InitBinaryFormatter();
    }

    private void InitBinaryFormatter()
    {
        formatter = new BinaryFormatter();
        var selector = new SurrogateSelector();
        var v3Surrogate = new Vector3SerializationSurrogate();
        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);
        formatter.SurrogateSelector = selector;
    }

    public object Load(string fileName, object saveDataByDefault)
    {
        string filePath = directory + "/" + fileName;
        if (!File.Exists(filePath))
        {
            if (saveDataByDefault != null)
            {
                Save(fileName, saveDataByDefault);
            }
            return saveDataByDefault;
        }

        var file = File.Open(filePath, FileMode.Open);
        if (file.Length == 0)
        {
            file.Close();
            return saveDataByDefault;
        }

        var savedData = formatter.Deserialize(file);
        file.Close();
        return savedData;
    }

    public void Save(string fileName, object saveData)
    {
        string filePath = directory + "/" + fileName;
        Debug.Log($"Saving data to: {filePath}");
        var file = File.Create(filePath);
        formatter.Serialize(file, saveData);
        file.Close();
    }
}