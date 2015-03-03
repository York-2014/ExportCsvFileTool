using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using ExportCsvFiletTool.Entity;
using Newtonsoft.Json;

namespace ExportCsvFiletTool.BLL
{
    public class Common
    {
        //public static readonly string WebSite_USA = "https://06c1060abb7134b5a531e522d0d4f929:4b770aba416883beed7f6da1d6085ea9@drgrab-us.myshopify.com/admin/orders.json";
        //public static readonly string WebSite_Canada = "https://5e7f7d78967fe5546a6c1754db0fd45e:68f7257c6b72bbf0fdb5c3e2280d624e@drgrab-ca.myshopify.com/admin/orders.json";
        //public static readonly string WebSite_Australia = "https://0c2e9f1d55a3f0afaaa6748c5284efba:9650de5aa396d94a91f84ee4b169efa2@stanchen.myshopify.com/admin/orders.json";
        //public static readonly string WebSite_NewZealand = "https://58e63ae0af07ca3f1d177c5dca72e27a:d22ec885315b047d602c0622c3275c25@drgrabnz.myshopify.com/admin/orders.json";

        public static string GetUser(string strUrl)
        {
            string strUser = string.Empty;
            strUser = strUrl.Substring(8, 32);
            return strUser;
        }

        public static string GetPwd(string strUrl)
        {
            string strUser = string.Empty;
            strUser = strUrl.Substring(41, 32);
            return strUser;
        }

        public static string ProduceApiUrl(ApiFieldsEntity apiFields)
        {
            string strUrl = string.Empty;
            strUrl = apiFields.WebSite;
            bool existFields = false;
            if (true == apiFields.AnyStatus)
            {
                strUrl = strUrl + "?status=any";
                existFields = true;
            }
            if (apiFields.StartDate != DateTime.MinValue)
            {
                if (true == existFields)
                {
                    strUrl = strUrl + "&created_at_min=" + apiFields.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    strUrl = strUrl + "?created_at_min=" + apiFields.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                existFields = true;
            }
            if (apiFields.EndDate != DateTime.MinValue)
            {
                if (true == existFields)
                {
                    strUrl = strUrl + "&created_at_max=" + apiFields.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    strUrl = strUrl + "?created_at_min=" + apiFields.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                existFields = true;
            }
            if (apiFields.Limit > 0)
            {
                if (true == existFields)
                {
                    strUrl = strUrl + "&limit=" + apiFields.Limit;
                }
                else
                {
                    strUrl = strUrl + "?limit=" + apiFields.Limit;
                }
                existFields = true;
            }
            return strUrl;
        }

        #region 实体与JSON互转
        /// <summary>
        /// 指定实体转JSON
        /// </summary>
        /// <typeparam name="T">类型约束</typeparam>
        /// <param name="t">类型</param>
        /// <returns>返回JSON字符串</returns>
        public static string EntityToJsonString<T>(T t)
        {
            return JsonConvert.SerializeObject(t).ToLower();
        }

        /// <summary>
        /// JSON转指定实体
        /// </summary>
        /// <typeparam name="T">类型约束</typeparam>
        /// <param name="strJson">JSON字符串</param>
        /// <param name="t">类型</param>
        /// <returns>返回指定类型</returns>
        public static T JsonStringToEntity<T>(string strJson, Type t) where T : class
        {
            return JsonConvert.DeserializeObject(strJson, t) as T;
        }
        #endregion

        #region 保存为csv文件
        /// <summary>
        /// Save the List data to CSV file
        /// </summary>
        /// <param name="lstRecordTotal">data source</param>
        /// <param name="filePath">file path</param>
        /// <returns>success flag</returns>
        public static bool SaveDataToCSVFile(List<ExportDataEntity> lstExportData, string filePath)
        {
            bool successFlag = false;

            StringBuilder strColumn = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            StreamWriter sw = null;
            PropertyInfo[] props = GetPropertyInfoArray();

            try
            {
                sw = new StreamWriter(filePath, true, Encoding.Default);
                for (int i = 0; i < props.Length; i++)
                {
                    strColumn.Append(props[i].Name.Replace("_", " "));
                    strColumn.Append(",");
                }
                strColumn.Remove(strColumn.Length - 1, 1);
                sw.WriteLine(strColumn);    //write the column name

                for (int i = 0; i < lstExportData.Count; i++)
                {
                    strValue.Remove(0, strValue.Length); //clear the temp row value
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Name));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Email));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Financial_Status));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Paid_at));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Fulfillment_Status));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Fulfilled_at));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Accepts_Marketing));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Currency));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Subtotal));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Taxes));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Total));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Discount_Code));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Discount_Amount));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Method));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Created_at));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_quantity));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_name));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_price));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_compare_at_price));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_sku));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_requires_shipping));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_taxable));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Lineitem_fulfillment_status));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Name));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Street));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Address1));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Address2));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Company));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_City));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Zip));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Province));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Country));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Billing_Phone));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Name));
                    strValue.Append(",");
                    if (null != lstExportData[i].Shipping_Address2 && string.IsNullOrEmpty(lstExportData[i].Shipping_Address2.ToString()) == false)
                    {
                        int num;
                        string temp = Convert.ToString(lstExportData[i].Shipping_Address2).Substring(0, Convert.ToString(lstExportData[i].Shipping_Address2).Length - 1);
                        if (Int32.TryParse(Convert.ToString(lstExportData[i].Shipping_Address2), out num) || Int32.TryParse(temp, out num))
                        {
                            strValue.Append(string.Format("\"{0},{1}/{2}\"", Convert.ToString(lstExportData[i].Shipping_Company), Convert.ToString(lstExportData[i].Shipping_Address2), Convert.ToString(lstExportData[i].Shipping_Address1)));
                        }
                        else
                        {
                            if (Convert.ToString(lstExportData[i].Shipping_Street).Contains(Convert.ToString(lstExportData[i].Shipping_Company)) == false)
                            {
                                strValue.Append(string.Format("\"{0},{1}\"", lstExportData[i].Shipping_Company, lstExportData[i].Shipping_Street));
                            }
                            else
                            {
                                strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Street));
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToString(lstExportData[i].Shipping_Street).Contains(Convert.ToString(lstExportData[i].Shipping_Company)) == false)
                        {
                            strValue.Append(string.Format("\"{0},{1}\"", Convert.ToString(lstExportData[i].Shipping_Company), Convert.ToString(lstExportData[i].Shipping_Street)));
                        }
                        else
                        {
                            strValue.Append(string.Format("\"{0}\"", Convert.ToString(lstExportData[i].Shipping_Street)));
                        }
                    }
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Address1));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Address2));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Company));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_City));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Zip));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Province));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Country));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Shipping_Phone));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Notes));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Note_Attributes));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Cancelled_at));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Payment_Method));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Payment_Reference));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Refunded_Amount));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Vendor));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Id));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Tags));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Risk_Level));
                    strValue.Append(",");
                    strValue.Append(string.Format("\"{0}\"", lstExportData[i].Source));
                    sw.WriteLine(strValue); //write the row value
                }
                successFlag = true;
            }
            catch (Exception ex)
            {
                successFlag = false;
                Common.WriteLog(string.Format("导出异常：{0}\r\nDetails:{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                }
            }
            return successFlag;
        }

        private static PropertyInfo[] GetPropertyInfoArray()
        {
            PropertyInfo[] props = null;
            try
            {
                Type type = typeof(ExportDataEntity);
                object obj = Activator.CreateInstance(type);
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            catch (Exception ex)
            {
                Common.WriteLog(string.Format("导出异常：{0}\r\nDetails:{1}", ex.Message, ex.StackTrace));
            }
            return props;
        }
        #endregion

        #region  写日志
        /// <summary>
        /// Write text to log file.
        /// </summary>
        /// <param name="strLog"></param>
        public static void WriteLog(string strLogMessage)
        {
            string savePath = string.Format("{0}\\MainAppLog\\{1}.txt", Environment.CurrentDirectory, System.DateTime.Now.ToString("yyyy-MM-dd"));
            string saveFolder = Path.GetDirectoryName(savePath);
            if (!System.IO.Directory.Exists(saveFolder))
            {
                System.IO.Directory.CreateDirectory(saveFolder);
                DirectorySecurity secu = new DirectorySecurity(Path.GetDirectoryName(savePath), AccessControlSections.Access);
                FileSystemAccessRule fsar = new FileSystemAccessRule(Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow);
                secu.SetAccessRule(fsar);
                System.IO.Directory.SetAccessControl(saveFolder, secu);
            }
            using (StreamWriter sw = new StreamWriter(savePath, true))
            {
                sw.WriteLine(string.Format("【{0}】\r\n  {1}\r\n", DateTime.Now.ToString("HH:mm:ss"), strLogMessage));
            }
        }
        #endregion

        #region MD5
        /// <summary>
        /// 取得输入字符串的MD5哈希值
        /// </summary>
        /// <param name="argInput">输入字符串</param>
        /// <returns>MD5哈希值</returns>
        public static string GetMd5Hash(string argInput)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(argInput));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// 验证MD5哈希值
        /// </summary>
        /// <param name="argInput">输入字符串</param>
        /// <param name="argHash">哈希值</param>
        /// <returns>相同返回TRUE,不同返回FALSE</returns>
        public static bool VerifyMd5Hash(string argInput, string argHash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(argInput);

            // Create a StringComparer an comare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, argHash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 获取当前程序版本号
        /// <summary>
        /// 获取当前程序版本号
        /// </summary>
        /// <returns>版本号</returns>
        public static string AssemblyFileVersion()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attributes.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                return ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }
        }
        #endregion
    }
}