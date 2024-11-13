using System;
using System.Collections.Generic;
using System.ComponentModel;
using ConfigurationHub.Enums;

namespace ConfigurationHub.Helpers
{
    public static class TypeConversionHelper
    {
        public static List<Type> AcceptedTypes => new()
        {
            typeof(int),
            typeof(string),
            typeof(double),
            typeof(bool)
        };

        public static Type GetTypeFromEnum(ConfigurationValueTypes typeEnum)
        {
            switch (typeEnum)
            {
                case ConfigurationValueTypes.Int:
                    return typeof(int);
                case ConfigurationValueTypes.String:
                    return typeof(string);
                case ConfigurationValueTypes.Double:
                    return typeof(double);
                case ConfigurationValueTypes.Boolean:
                    return typeof(bool);
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeEnum), typeEnum, null);
            }
        }

        public static ConfigurationValueTypes GetEnumFromType(Type type)
        {
            if (type == typeof(int))
            {
                return ConfigurationValueTypes.Int;
            }

            if (type == typeof(string))
            {
                return ConfigurationValueTypes.String;
            }

            if (type == typeof(double))
            {
                return ConfigurationValueTypes.Double;
            }

            if (type == typeof(bool))
            {
                return ConfigurationValueTypes.Boolean;
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        public static bool IsConversionPossible(string input, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return true;
            }

            var converter = TypeDescriptor.GetConverter(targetType);

            if (converter.IsValid(input))
            {
                return true;
            }

            return false;
        }
    }
}