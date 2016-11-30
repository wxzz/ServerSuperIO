using System;

namespace ServerSuperIO.Common
{
    public class EnumDescriptionAttribute : Attribute
    {
        private string _Description;
        public EnumDescriptionAttribute(string description)
        {
            _Description = description;
        }

        public string Description
        {
            get { return _Description; }
        }

        public static string GetEnumDescription(Enum enumobj)
        {
            System.Reflection.FieldInfo fieldInfo = enumobj.GetType().GetField(enumobj.ToString());
            object[] attribArray = fieldInfo.GetCustomAttributes(false);
            if (attribArray.Length == 0)
            {
                return enumobj.ToString();
            }
            else
            {
                EnumDescriptionAttribute attrib = attribArray[0] as EnumDescriptionAttribute;
                return attrib.Description;
            }
        }
    }
}
