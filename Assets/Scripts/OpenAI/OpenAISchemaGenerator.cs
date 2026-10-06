using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OpenAISchemaGenerator
{
    public JObject GenerateJsonSchema<T>()
    {
        var schema = GenerateSchemaForType(typeof(T));
        var root = new Dictionary<string, object>
        {
            { "name", typeof(T).Name.ToLower() + "_schema" },
            { "strict", true },
            { "schema", schema }
        };

        return JObject.FromObject(root);
    }

    private Dictionary<string, object> GenerateSchemaForType(Type type)
    {
        var properties = new Dictionary<string, object>();
        var requiredList = new List<string>();

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            requiredList.Add(field.Name);
            properties.Add(field.Name, GetFieldSchema(field.FieldType));
        }

        return new Dictionary<string, object>
        {
            { "type", "object" },
            { "properties", properties },
            { "required", requiredList },
            { "additionalProperties", false } // OpenAI strict 模式必填
        };
    }

    private Dictionary<string, object> GetFieldSchema(Type memberType)
    {
        if (memberType == typeof(string))
        {
            return new Dictionary<string, object> { { "type", "string" } };
        }
        else if (memberType == typeof(int) || memberType == typeof(long))
        {
            return new Dictionary<string, object> { { "type", "integer" } };
        }
        else if (memberType == typeof(float) || memberType == typeof(double))
        {
            return new Dictionary<string, object> { { "type", "number" } };
        }
        else if (memberType == typeof(bool))
        {
            return new Dictionary<string, object> { { "type", "boolean" } };
        }
        else if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var itemType = memberType.GetGenericArguments()[0];
            return new Dictionary<string, object>
            {
                { "type", "array" },
                { "items", GetFieldSchema(itemType) }
            };
        }
        else
        {
            return GenerateSchemaForType(memberType);
        }
    }
}