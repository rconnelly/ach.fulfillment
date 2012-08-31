using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ach.Fulfillment.Nacha.Attribute;
using Ach.Fulfillment.Nacha.Enumeration;
using Ach.Fulfillment.Nacha.Exception;
using Ach.Fulfillment.Shared.Reflection;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;

namespace Ach.Fulfillment.Nacha.Serialization
{
	[Serializable]
	public abstract class SerializableBase : ISerializable
	{
		private static ReflectionQuery<RecordAttribute> _reflectionRecordQuery = new ReflectionQuery<RecordAttribute>();
		private static ReflectionQuery<FieldAttribute> _reflectionFieldQuery = new ReflectionQuery<FieldAttribute>();

		public virtual string Serialize(bool audit = false)
		{
			StringBuilder message = new StringBuilder();

			foreach (var entry in _reflectionRecordQuery.Reflect(this.GetType()).ReflectableMembers)
			{
				object value = entry.Value.GetValue(this);

				if (value == null)
					continue;
				
				if (value is IList)
				{
					foreach (SerializableBase item in (value as IList))
					{
						if (item == null)
							continue;

						if (!String.IsNullOrEmpty(entry.Value.ReflectAttribute.Prefix))
							message.Append(entry.Value.ReflectAttribute.Prefix);

						message.Append((item as SerializableBase).Serialize());

						if (!String.IsNullOrEmpty(entry.Value.ReflectAttribute.Postfix))
							message.Append(entry.Value.ReflectAttribute.Postfix);
					}
				}
				else
				{
					if (!String.IsNullOrEmpty(entry.Value.ReflectAttribute.Prefix))
							message.Append(entry.Value.ReflectAttribute.Prefix);

					message.Append((value as SerializableBase).Serialize());

					if (!String.IsNullOrEmpty(entry.Value.ReflectAttribute.Postfix))
						message.Append(entry.Value.ReflectAttribute.Postfix);
				}
			}

			foreach (var entry in _reflectionFieldQuery.Reflect(this.GetType()).ReflectableMembers)
			{
				FieldAttribute attribute = entry.Value.ReflectAttribute;
 
				object value = entry.Value.GetValue(this);

				if (value == null && attribute.IsRequired)
					throw new FieldNullException(entry.Value.MemberName, "Field is marked as required.");
			
				Type valueType = entry.Value.Type;

				if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
					valueType = Nullable.GetUnderlyingType(valueType);

				string valueString;

				if (valueType == typeof(string))
				{
					valueString = (string)value ?? "";
				}				
				else if (valueType == typeof(int) || valueType == typeof(long))
				{
					long lValue = value == null ? 0 : (long)value;

					if (attribute.Scale != 0)
						lValue *= attribute.Scale;

					valueString = lValue.ToString();
				}
				else if (valueType == typeof(decimal) || valueType == typeof(double) || valueType == typeof(float))
				{
					decimal dValue = value == null ? 0m : (decimal)value;

					if (attribute.Scale != 0)
						dValue *= (decimal)attribute.Scale;

					dValue = Math.Round(dValue, attribute.Precision);

					valueString = dValue.ToString();
				}
				else if (valueType == typeof(DateTime))
				{
					if (value == null)
						valueString = "";
					if (!String.IsNullOrWhiteSpace(attribute.FormattingString))
						valueString = ((DateTime)value).ToString(attribute.FormattingString);
					else
						valueString = ((DateTime)value).ToString();
				}
				else if (valueType.IsEnum)
				{
					if (value == null)
						valueString = "";
					if (attribute.DataType == DataType.Numeric)
						valueString = ((Enum)value).ToString("D");
					else
						valueString = ((Enum)value).ToString("G");
				}
				else
				{
					throw new System.NotImplementedException(string.Format("Serialiation for type \"{0}\" is not supported at this time", valueType.ToString()));
				}

				if (valueString.Length > attribute.Length)
					throw new FieldOutOfRangeException(entry.Value.MemberName, valueString, String.Format("Data length exceeds defined length of {0}", attribute.Length));

				switch (attribute.PaddingType)
				{
					case PaddingType.Default:
						if (attribute.DataType == DataType.Alphanumeric)
							valueString = valueString.PadRight(attribute.Length, ' ');
						else
							valueString = valueString.PadLeft(attribute.Length, '0');
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
						if (attribute.DataType == DataType.Alphanumeric)
							valueString = valueString.PadRight(attribute.Length, ' ');
						else
							valueString = valueString.PadLeft(attribute.Length, '0');
						break;
				}

				if (audit && attribute.MaskOnAudit)
					valueString = new string('X', valueString.Length);

				message.Append(valueString);
			}

			return message.ToString();
		}

		public virtual void Deserialize(string message)
		{
			using (MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(message), false))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream))
					Deserialize(streamReader);
			}
		}

		public virtual void Deserialize(StreamReader message)
		{
			ReflectionQueryResult<RecordAttribute> recordResult = _reflectionRecordQuery.Reflect(this.GetType());

			do
			{
				string line = message.ReadLine();

				if (recordResult != null && recordResult.ReflectableMembers != null && recordResult.ReflectableMembers.Count > 0 && recordResult.ReflectableMembers.Any(x => line.StartsWith(x.Value.ReflectAttribute.RecordType)))
				{
					message.BaseStream.Position -= line.Length;

					ReflectMember<RecordAttribute> reflectMember = recordResult.ReflectableMembers.First(x => line.StartsWith(x.Value.ReflectAttribute.RecordType)).Value;

					object obj = reflectMember.GetValue(this);
					Type type = reflectMember.Type;

					if (obj is IList || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
					{
						if (obj == null)
						{
							obj = Activator.CreateInstance(reflectMember.Type);
							reflectMember.SetValue(this, obj);
						}

						SerializableBase item = (SerializableBase)Activator.CreateInstance(reflectMember.Type.GetGenericArguments()[0]);
						item.Deserialize(message);
						(obj as IList).Add(item);
					}
					else
					{
						SerializableBase item = (SerializableBase)Activator.CreateInstance(type);
						item.Deserialize(message);
						reflectMember.SetValue(this, item);
					}
				}
				else
				{
					DeserializeRecord(line);
				}
			} while (!message.EndOfStream);
		}

		private void DeserializeRecord(string record)
		{
			int offset = 0;
			
			foreach (var entry in _reflectionFieldQuery.Reflect(this.GetType()).ReflectableMembers)
			{
				if (entry.Value.ReflectAttribute.IsRequired && record.Length - offset < entry.Value.ReflectAttribute.Length)
					throw new FieldOutOfRangeException(entry.Value.MemberName, record, "Record data length is shorter then the minimum length required");

				Type fieldType = entry.Value.Type;
				bool isNullable = false;

				if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					fieldType = Nullable.GetUnderlyingType(fieldType);
					isNullable = true;
				}

				string fieldData = record.Substring(offset, entry.Value.ReflectAttribute.Length);
				offset += entry.Value.ReflectAttribute.Length;

				int scaler = entry.Value.ReflectAttribute.Scale == 0 ? 1 : entry.Value.ReflectAttribute.Scale;

				if (fieldType == typeof(string))
					entry.Value.SetValue(this, fieldData.Trim());
				else if (fieldType == typeof(int))
				{
					int result;

					if (!Int32.TryParse(fieldData.Trim(), out result) && isNullable)
						entry.Value.SetValue(this, null);
					else if (!isNullable)
						throw new FormatException("Field data could not be parsed because it is in the incorrect format");
					else
						entry.Value.SetValue(this, result / scaler);
				}
				else if (fieldType == typeof(long))
				{
					long result;

					if (!Int64.TryParse(fieldData.Trim(), out result) && isNullable)
						entry.Value.SetValue(this, null);
					else if (!isNullable)
						throw new FormatException("Field data could not be parsed because it is in the incorrect format");
					else
						entry.Value.SetValue(this, result / scaler);
				}
				else if (fieldType == typeof(double))
				{
					double result;

					if (!Double.TryParse(fieldData.Trim(), out result) && isNullable)
						entry.Value.SetValue(this, null);
					else if (!isNullable)
						throw new FormatException("Field data could not be parsed because it is in the incorrect format");
					else
						entry.Value.SetValue(this, result / scaler);
				}
				else if (fieldType == typeof(float))
				{
					float result;

					if (!float.TryParse(fieldData.Trim(), out result) && isNullable)
						entry.Value.SetValue(this, null);
					else if (!isNullable)
						throw new FormatException("Field data could not be parsed because it is in the incorrect format");
					else
						entry.Value.SetValue(this, result / scaler);
				}
				else if (fieldType == typeof(decimal))
				{
					decimal result;

					if (!Decimal.TryParse(fieldData.Trim(), out result) && isNullable)
						entry.Value.SetValue(this, null);
					else if (!isNullable)
						throw new FormatException("Field data could not be parsed because it is in the incorrect format");
					else
						entry.Value.SetValue(this, result / scaler);
				}
				else if (fieldType == typeof(DateTime))
				{
					DateTime result = DateTime.MinValue;

					bool useFormatString = !String.IsNullOrWhiteSpace(entry.Value.ReflectAttribute.FormattingString);

					if (useFormatString && !DateTime.TryParseExact(fieldData, entry.Value.ReflectAttribute.FormattingString, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result) && isNullable)
						entry.Value.SetValue(this, null);
					else if (!useFormatString && !DateTime.TryParse(fieldData, null, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result) && isNullable)
						entry.Value.SetValue(this, null);
					else if (!isNullable)
						throw new FormatException("Field data could not be parsed because it is in the incorrect format");
					else
						entry.Value.SetValue(this, result);
				}
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (var entry in _reflectionRecordQuery.Reflect(this.GetType()).ReflectableMembers)
				info.AddValue(entry.Value.MemberName, entry.Value.GetValue(this), entry.Value.Type);

			foreach (var entry in _reflectionFieldQuery.Reflect(this.GetType()).ReflectableMembers)
				info.AddValue(entry.Value.MemberName, entry.Value.GetValue(this), entry.Value.Type);
		}
	}
}
