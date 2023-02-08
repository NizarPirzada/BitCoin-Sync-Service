using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BtcSync.Helper
{
    public static class Extensions
    {

        #region DataTable to List

        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();
                //var properties = classObj.GetType().GetProperties().ToList();
                //properties.ForEach(x => { x.Name = x.Name.ToLower()});



                foreach (var dtField in dtlFieldNames)
                {
                    //PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name.Replace("_", ""));
                    PropertyInfo propertyInfos = classObj.GetType().GetProperties().ToList().Where(x => x.Name.Replace("_", "").ToLower() == dtField.Name.Replace("_", "")).FirstOrDefault();

                    var field = objFieldNames.Find(x => x.Name.Replace("_", "").ToLower() == dtField.Name.Replace("_", ""));
                    if (field != null)
                    {

                        if (propertyInfos.PropertyType == typeof(DateTime) || propertyInfos.PropertyType == typeof(DateTime?))
                        {
                            propertyInfos.SetValue
                            (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToInt(dataRow[dtField.Name]), null);
                        }                        
                        else if (propertyInfos.PropertyType == typeof(long))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(decimal))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(double))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToDouble(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(String))
                        {

                            propertyInfos.SetValue
                            (classObj, ConvertToString(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(List<string>))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToListString(dataRow[dtField.Name]), null);

                        }
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }
        private static List<string> ConvertToListString(object value)
        {
            return ((string)ReturnEmptyIfNull(value)).Split(',').ToList();
        }
        private static string ConvertToString(object value)
        {
            return Convert.ToString(ReturnEmptyIfNull(value));
        }

        private static int ConvertToInt(object value)
        {
            return Convert.ToInt32(ReturnZeroIfNull(value));
        }



        public static object GetNewObject(Type t)
        {
            try
            {
                return t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch
            {
                return null;
            }
        }



        private static long ConvertToLong(object value)
        {
            return Convert.ToInt64(ReturnZeroIfNull(value));
        }

        private static decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(ReturnZeroIfNull(value));
        }
        private static double ConvertToDouble(object value)
        {
            return Convert.ToDouble(ReturnZeroIfNull(value));
        }

        private static DateTime convertToDateTime(object date)
        {
            return Convert.ToDateTime(ReturnDateTimeMinIfNull(date));
        }


        public static object ReturnZeroIfNull(this object value)
        {
            if (value == DBNull.Value)
                return 0;
            if (value == null)
                return 0;
            return value;
        }

        public static object ReturnEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;
            return value;
        }

        public static object ReturnFalseIfNull(this object value)
        {
            if (value == DBNull.Value)
                return false;
            if (value == null)
                return false;
            return value;
        }

        public static object ReturnDateTimeMinIfNull(this object value)
        {
            if (value == DBNull.Value)
                return DateTime.MinValue;
            if (value == null)
                return DateTime.MinValue;
            return value;
        }

        public static object ReturnNullIfDbNull(this object value)
        {
            if (value == DBNull.Value)
                return '\0';
            if (value == null)
                return '\0';
            return value;
        }


        #endregion

    }
}
