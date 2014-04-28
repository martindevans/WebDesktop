using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace TransparentWindow.Nancy
{
    public sealed class CustomJsonSerializer
        : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            FloatFormatHandling = FloatFormatHandling.String;
            FloatParseHandling = FloatParseHandling.Double;

            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
            TypeNameHandling = TypeNameHandling.Auto;

            Binder = new CustomSerializationBinder();
        }

        private class CustomSerializationBinder
            : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return Type.GetType(typeName + "," + assemblyName, true);
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                typeName = serializedType.FullName;
                assemblyName = serializedType.Assembly.FullName;
            }
        }
    }
}
