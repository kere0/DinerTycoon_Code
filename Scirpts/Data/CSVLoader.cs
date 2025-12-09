using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
public class CSVLoader
{
    public Dictionary<string, object> LoadCSV<T>(TextAsset ts) where T : InterfaceID, new()
    {
        TextAsset textAsset = ts;
        if (textAsset == null)
        {
            Debug.Log($"{textAsset.name} is null");
        }
        StringReader reader = new StringReader(textAsset.text);
        string[] headers = reader.ReadLine().Split(',');
        
        Dictionary<string, object> dict = new Dictionary<string, object>();
        Debug.Log(headers.Length);
        while (reader.Peek() > 0)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(',');
            T data = new T();
            for (int i = 0; i < headers.Length; i++)
            {
                string title = headers[i];
                string value = values[i];
                
                FieldInfo field = typeof(T).GetField(title);
                if (field != null)
                {
                    object converted;
                    if (field.FieldType.IsEnum == true)
                    {
                        converted = Enum.Parse(field.FieldType, value);
                    }
                    else
                    {
                        converted = Convert.ChangeType(value, field.FieldType);
                    }
                    field.SetValue(data, converted);
                }
            }
            Debug.Log(data.ID);
            dict.Add(data.ID, data);
        }
        return dict;
    }
}
