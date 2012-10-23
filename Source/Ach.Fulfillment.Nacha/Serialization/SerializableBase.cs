namespace Ach.Fulfillment.Nacha.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    using Ach.Fulfillment.Nacha.Attribute;
    using Ach.Fulfillment.Nacha.Enumeration;
    using Ach.Fulfillment.Nacha.Exception;
    using Ach.Fulfillment.Shared.Reflection;

    [Serializable]
    public abstract class SerializableBase : ISerializable
    {
        private static ReflectionQuery<RecordAttribute> reflectionRecordQuery = new ReflectionQuery<RecordAttribute>();
        private static ReflectionQuery<FieldAttribute> reflectionFieldQuery = new ReflectionQuery<FieldAttribute>();

        public virtual string Serialize(bool audit = false)
        {
            var message = new StringBuilder();

            foreach (var entry in reflectionRecordQuery.Reflect(this.GetType()).ReflectableMembers)
            {
                var value = entry.Value.GetValue(this);

                if (value == null)
                {
                    continue;
                }

                if (value is IList)
                {
                    foreach (SerializableBase item in (IList)value)
                    {
                        if (item == null)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(entry.Value.ReflectAttribute.Prefix))
                        {
                            message.Append(entry.Value.ReflectAttribute.Prefix);
                        }

                        message.Append(item.Serialize());

                        if (!string.IsNullOrEmpty(entry.Value.ReflectAttribute.Postfix))
                        {
                            message.Append(entry.Value.ReflectAttribute.Postfix);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(entry.Value.ReflectAttribute.Prefix))
                    {
                        message.Append(entry.Value.ReflectAttribute.Prefix);
                    }

                    message.Append(((SerializableBase)value).Serialize());

                    if (!string.IsNullOrEmpty(entry.Value.ReflectAttribute.Postfix))
                    {
                        message.Append(entry.Value.ReflectAttribute.Postfix);
                    }
                }
            }

            foreach (var entry in reflectionFieldQuery.Reflect(this.GetType()).ReflectableMembers)
            {
                var attribute = entry.Value.ReflectAttribute;
                var value = entry.Value.GetValue(this);

                if (value == null && attribute.IsRequired)
                {
                    throw new FieldNullException(entry.Value.MemberName, "Field is marked as required.");
                }

                var valueType = entry.Value.Type;

                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    valueType = Nullable.GetUnderlyingType(valueType);
                }

                var valueString = string.Empty;

                if (valueType == typeof(string))
                {
                    valueString = (string)value ?? string.Empty;
                }
                else if (valueType == typeof(int) || valueType == typeof(long))
                {
                    var lvalue = value == null ? 0 : (long)value;
                    if (attribute.Scale != 0)
                    {
                        lvalue *= attribute.Scale;
                    }

                    valueString = lvalue.ToString(CultureInfo.InvariantCulture);
                }
                else if (valueType == typeof(decimal) || valueType == typeof(double) || valueType == typeof(float))
                {
                    var dvalue = value == null ? 0m : (decimal)value;

                    if (attribute.Scale != 0)
                    {
                        dvalue *= attribute.Scale;
                    }

                    dvalue = Math.Round(dvalue, attribute.Precision);
                    valueString = dvalue.ToString(CultureInfo.InvariantCulture);
                }
                else if (valueType == typeof(DateTime))
                {
                    // if (value == null)
                    // {
                    //    valueString = string.Empty;
                    // }
                    if (!string.IsNullOrWhiteSpace(attribute.FormattingString))
                    {
                        Debug.Assert(value != null, "value != null");
                        valueString = ((DateTime)value).ToString(attribute.FormattingString);
                    }
                    else
                    {
                        Debug.Assert(value != null, "value != null");
                        valueString = ((DateTime)value).ToString(CultureInfo.InvariantCulture);
                    }
                }
                else if (valueType.IsEnum)
                {
                    if (value == null)
                    {
                        valueString = string.Empty;
                    }

                    var @enum = (Enum)value;
                    if (@enum != null)
                    {
                        valueString = @enum.ToString(attribute.DataType == DataType.Numeric ? "D" : "G");
                    }
                }
                else
                {
                    throw new NotImplementedException(string.Format("Serialiation for type \"{0}\" is not supported at this time", valueType));
                }

                if (valueString.Length > attribute.Length)
                {
                    throw new FieldOutOfRangeException(
                        entry.Value.MemberName,
                        valueString,
                        string.Format("Data length exceeds defined length of {0}", attribute.Length));
                }

                switch (attribute.PaddingType)
                {
                    case PaddingType.Default:
                        valueString = attribute.DataType == DataType.Alphanumeric ? valueString.PadRight(attribute.Length, ' ') : valueString.PadLeft(attribute.Length, '0');

                        break;
                    case PaddingType.SpacePadRight:
                        valueString = valueString.PadRight(attribute.Length, ' ');
                        break;
                    case PaddingType.SpacePadLeft:
                        valueString = valueString.PadLeft(attribute.Length, ' ');
                        break;
                    case PaddingType.ZeroPadRight:
                        valueString = valueString.PadRight(attribute.Length, '0');
                        break;
                    case PaddingType.ZeroPadLeft:
                        valueString = valueString.PadLeft(attribute.Length, '0');
                        break;
                    case PaddingType.CustomPadRight:
                        valueString = valueString.PadRight(attribute.Length, attribute.PaddingCharacter);
                        break;
                    case PaddingType.CustomPadLeft:
                        valueString = valueString.PadLeft(attribute.Length, attribute.PaddingCharacter);
                        break;
                    default:
                        valueString = attribute.DataType == DataType.Alphanumeric ?
                            valueString.PadRight(attribute.Length, ' ') :
                            valueString.PadLeft(attribute.Length, '0');

                        break;
                }

                if (audit && attribute.MaskOnAudit)
                {
                    valueString = new string('X', valueString.Length);
                }

                message.Append(valueString);
            }

            return message.ToString();
        }

        public virtual void Deserialize(string message)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(message), false))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    this.Deserialize(streamReader);
                }
            }
        }

        public virtual void Deserialize(StreamReader message)
        {
            var recordResult = reflectionRecordQuery.Reflect(this.GetType());

            do
            {
                var line = message.ReadLine();

                if (line != null && recordResult != null && recordResult.ReflectableMembers != null
                    && recordResult.ReflectableMembers.Count > 0
                    && recordResult.ReflectableMembers.Any(x => line.StartsWith(x.Value.ReflectAttribute.RecordType)))
                {
                    message.BaseStream.Position -= line.Length;

                    var reflectMember =
                        recordResult.ReflectableMembers.First(x => line.StartsWith(x.Value.ReflectAttribute.RecordType))
                            .Value;
                    var obj = reflectMember.GetValue(this);
                    var type = reflectMember.Type;

                    if (obj is IList || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        if (obj == null)
                        {
                            obj = Activator.CreateInstance(reflectMember.Type);
                            reflectMember.SetValue(this, obj);
                        }

                        var item =
                            (SerializableBase)Activator.CreateInstance(reflectMember.Type.GetGenericArguments()[0]);
                        item.Deserialize(message);
                        ((IList)obj).Add(item);
                    }
                    else
                    {
                        var item = (SerializableBase)Activator.CreateInstance(type);
                        item.Deserialize(message);
                        reflectMember.SetValue(this, item);
                    }
                }
                else
                {
                    this.DeserializeRecord(line);
                }
            }
            while (!message.EndOfStream);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var entry in reflectionRecordQuery.Reflect(this.GetType()).ReflectableMembers)
            {
                info.AddValue(entry.Value.MemberName, entry.Value.GetValue(this), entry.Value.Type);
            }

            foreach (var entry in reflectionFieldQuery.Reflect(this.GetType()).ReflectableMembers)
            {
                info.AddValue(entry.Value.MemberName, entry.Value.GetValue(this), entry.Value.Type);
            }
        }

        private void DeserializeRecord(string record)
        {
            var offset = 0;
        
            foreach (var entry in reflectionFieldQuery.Reflect(this.GetType()).ReflectableMembers)
            {
                if (entry.Value.ReflectAttribute.IsRequired
                    && record.Length - offset < entry.Value.ReflectAttribute.Length)
                {
                    throw new FieldOutOfRangeException(
                        entry.Value.MemberName, record, "Record data length is shorter then the minimum length required");
                }

                var fieldType = entry.Value.Type;
                var isNullable = false;

                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    fieldType = Nullable.GetUnderlyingType(fieldType);
                    isNullable = true;
                }

                var fieldData = record.Substring(offset, entry.Value.ReflectAttribute.Length);
                offset += entry.Value.ReflectAttribute.Length;

                var scaler = entry.Value.ReflectAttribute.Scale == 0 ? 1 : entry.Value.ReflectAttribute.Scale;

                if (fieldType == typeof(string))
                {
                    entry.Value.SetValue(this, fieldData.Trim());
                }
                else if (fieldType == typeof(int))
                {
                    int result;

                    if (!int.TryParse(fieldData.Trim(), out result) && isNullable)
                    {
                        entry.Value.SetValue(this, null);
                    }
                    else if (!isNullable)
                    {
                        throw new FormatException(
                            "Field data could not be parsed because it is in the incorrect format");
                    }
                    else
                    {
                        entry.Value.SetValue(this, result / scaler);
                    }
                }
                else if (fieldType == typeof(long))
                {
                    long result;

                    if (!long.TryParse(fieldData.Trim(), out result) && isNullable)
                    {
                        entry.Value.SetValue(this, null);
                    }
                    else if (!isNullable)
                    {
                        throw new FormatException("Field data could not be parsed because it is in the incorrect format");
                    }
                    else
                    {
                        entry.Value.SetValue(this, result / scaler);
                    }
                }
                else if (fieldType == typeof(double))
                {
                    double result;

                    if (!double.TryParse(fieldData.Trim(), out result) && isNullable)
                    {
                        entry.Value.SetValue(this, null);
                    }
                    else if (!isNullable)
                    {
                        throw new FormatException("Field data could not be parsed because it is in the incorrect format");
                    }
                    else
                    {
                        entry.Value.SetValue(this, result / scaler);
                    }
                }
                else if (fieldType == typeof(float))
                {
                    float result;

                    if (!float.TryParse(fieldData.Trim(), out result) && isNullable)
                    {
                        entry.Value.SetValue(this, null);
                    }
                    else if (!isNullable)
                    {
                        throw new FormatException("Field data could not be parsed because it is in the incorrect format");
                    }
                    else
                    {
                        entry.Value.SetValue(this, result / scaler);
                    }
                }
                else if (fieldType == typeof(decimal))
                {
                    decimal result;

                    if (!decimal.TryParse(fieldData.Trim(), out result) && isNullable)
                    {
                        entry.Value.SetValue(this, null);
                    }
                    else if (!isNullable)
                    {
                        throw new FormatException("Field data could not be parsed because it is in the incorrect format");
                    }
                    else
                    {
                        entry.Value.SetValue(this, result / scaler);
                    }
                }
                else if (fieldType == typeof(DateTime))
                {
                    DateTime result = DateTime.MinValue;

                    bool useFormatString = !string.IsNullOrWhiteSpace(entry.Value.ReflectAttribute.FormattingString);

                    if (useFormatString && !DateTime.TryParseExact(fieldData, entry.Value.ReflectAttribute.FormattingString, null, DateTimeStyles.AllowWhiteSpaces, out result) && isNullable)
                    {
                        entry.Value.SetValue(this, null);
                    }
                    else if (!useFormatString && !DateTime.TryParse(fieldData, null, DateTimeStyles.AllowWhiteSpaces, out result) && isNullable)
                    {
                        entry.Value.SetValue(this, null);
                    }
                    else if (!isNullable)
                    {
                        throw new FormatException("Field data could not be parsed because it is in the incorrect format");
                    }
                    else
                    {
                        entry.Value.SetValue(this, result);
                    }
                }
            }
        }
    }
}
