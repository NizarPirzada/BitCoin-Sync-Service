using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace BtcSync.Repository
{
    public class Repository
    {
        public Repository()
        {
            if (ConfigurationManager.ConnectionStrings["ai_db"] != null)
                this.ConnectionString = ConfigurationManager.ConnectionStrings["ai_db"].ToString();
        }

        protected delegate T TFromDataRow<T>(DataRow dr);

        protected static List<T> CollectionFromDataSet<T>(DataSet ds, TFromDataRow<T> action)
        {
            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count == 0)
                return null;

            List<T> list = new List<T>(ds.Tables[0].Rows.Count);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(action(dr));
            }

            return list;
        }

        private string[] _numericTypes = { "System.Decimal", "System.Float", "System.Double", "System.Decimal?", "System.Float?", "System.Double?" };
        private string[] _stringTypes = { "System.String", "System.String?" };
        private string[] _intTypes = { "System.Int32", "System.Int16", "System.Int64", "System.Int32?", "System.Int16?", "System.Int64?" };
        private string[] _boolTypes = { "System.Boolean", "System.Boolean?" };
        private string[] _dateTypes = { "System.DateTime", "System.DateTime?" };

        private string conString = string.Empty;

        public virtual string ConnectionString
        {
            get { return conString; }
            set { conString = value; }
        }

        protected KeyValuePair<string, string> ParseWhereClause(string whereClause)
        {
            string keyName = String.Empty;
            string keyValue = String.Empty;

            string[] splitWhereClause = whereClause.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitWhereClause.Length; i++)
            {
                if (i == 0)
                {
                    keyName = splitWhereClause[i];
                }
                else
                {
                    if (String.IsNullOrEmpty(keyValue))
                    {
                        keyValue = splitWhereClause[i];
                    }
                    else
                    {
                        keyValue += ":" + splitWhereClause[i];
                    }
                }
            }
            KeyValuePair<string, string> parsedWhereClause = new KeyValuePair<string, string>(keyName, keyValue);
            return parsedWhereClause;
        }

        protected KeyValuePair<string, string> ParseOrderByClause(string orderByClause)
        {
            string keyName = String.Empty;
            string keyValue = String.Empty;

            string[] splitOrderByClause = orderByClause.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitOrderByClause.Length; i++)
            {
                if (i == 0)
                {
                    keyName = splitOrderByClause[i];
                }
                else
                {
                    if (String.IsNullOrEmpty(keyValue))
                    {
                        keyValue = splitOrderByClause[i];
                    }
                    else
                    {
                        keyValue += ":" + splitOrderByClause[i];
                    }
                }
            }
            KeyValuePair<string, string> parsedOrderByClause = new KeyValuePair<string, string>(keyName, keyValue);
            return parsedOrderByClause;
        }

        public bool IsValueValid(string value)
        {
            Match match = Regex.Match(value, @"^[a-zA-Z0-9\-\ /@,_!$#.&:+';]*$", RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static string ReplaceEscapeChars(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("'", "''");
            }
            return str;
        }

        public string CreateWhereClauseFromIDList(List<int> ListOfIds)
        {
            string whereClause = "(";
            foreach (int id in ListOfIds)
            {
                whereClause += id.ToString() + ",";
            }
            if (whereClause.Length > 1)
                whereClause = whereClause.Remove(whereClause.Length - 1);
            whereClause += ")";
            return whereClause;
        }

        public string GetCommaDelimitedStringFromArray<T>(T[] arr)
        {
            if (arr != null && arr.Length > 0)
            {
                string commaDelmitedValue = string.Empty;
                decimal id;
                if (decimal.TryParse(arr[0].ToString(), out id))
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0) commaDelmitedValue = arr[i].ToString();
                        else commaDelmitedValue += "," + arr[i].ToString();
                    }
                }
                else
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i == 0) commaDelmitedValue = "'" + arr[i].ToString() + "'";
                        else commaDelmitedValue += ",'" + arr[i].ToString() + "'";
                    }
                }
                return commaDelmitedValue;
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetCommaDelimitedStringFromList<T>(List<T> list)
        {
            return GetCommaDelimitedStringFromArray(list.ToArray());
        }
    }
}
