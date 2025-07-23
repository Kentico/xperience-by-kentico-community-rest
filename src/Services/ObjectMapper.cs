using System.Data;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;

using CMS;
using CMS.DataEngine;

using Xperience.Community.Rest.Models.Requests;
using Xperience.Community.Rest.Services;

[assembly: RegisterImplementation(typeof(IObjectMapper), typeof(ObjectMapper))]
namespace Xperience.Community.Rest.Services
{
    public class ObjectMapper : IObjectMapper
    {
        public void MapFieldsFromRequest<TBody>(BaseInfo infoObject, TBody body) where TBody : IRequestBodyWithFields
        {
            foreach (var field in body.Fields)
            {
                if (!infoObject.ContainsColumn(field.Key))
                {
                    continue;
                }

                object? value = ConvertNodeValue(field.Value);
                infoObject.SetValue(field.Key, value);
            }
        }


        public dynamic MapToSimpleObject(BaseInfo infoObject)
        {
            var obj = new ExpandoObject();
            foreach (string column in infoObject.ColumnNames)
            {
                ((IDictionary<string, object?>)obj).Add(column, infoObject.GetValue(column));
            }

            return obj;
        }


        public dynamic MapToSimpleObject(DataRow row)
        {
            var obj = new ExpandoObject();
            var columns = row.Table.Columns.OfType<DataColumn>().Select(c => c.ColumnName);
            foreach (string column in columns)
            {
                object? value = row[column];
                if (value.GetType().Equals(typeof(DBNull)))
                {
                    value = null;
                }

                ((IDictionary<string, object?>)obj).Add(column, value);
            }

            return obj;
        }


        private static object? ConvertNodeValue(JsonNode? node)
        {
            if (node is null)
            {
                return null;
            }

            return node.AsValue().GetValueKind() switch
            {
                JsonValueKind.String => node.GetValue<string>(),
                JsonValueKind.Number => node.GetValue<int>(),
                JsonValueKind.True or JsonValueKind.False => node.GetValue<bool>(),
                JsonValueKind.Null => null,
                JsonValueKind.Undefined or JsonValueKind.Object or JsonValueKind.Array
                    => throw new InvalidOperationException($"Field cannot be converted to a value."),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
