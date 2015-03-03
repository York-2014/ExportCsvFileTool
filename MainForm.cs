using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using ExportCsvFiletTool.BLL;
using ExportCsvFiletTool.Entity;
using ExportCsvFiletTool.Helper;
using System.Net;

namespace ExportCsvFiletTool
{
    public partial class MainForm : Form
    {
        public static List<OrderDetailEntity> lstOrders = new List<OrderDetailEntity>();
        private List<string> lstTempWebSites = new List<string>();
        private static string strSaveFolder = string.Empty;
        private DateTime dtStartDate = DateTime.MinValue;
        private DateTime dtEndDate = DateTime.MinValue;
        private Thread thGetOrder;
        private DateTime dtStartTime = DateTime.MinValue;
        private static XmlNodeList xmlNodes = null;
        private static string strPwdHashCode = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        #region 通知UI更新状态
        /// <summary>
        /// 通知状态的委托
        /// </summary>
        /// <param name="obj"></param>
        private delegate void NotifyStatusDelegate(object[] obj);

        /// <summary>
        /// 通知类型
        /// </summary>
        private enum NotifyType
        {
            /// <summary>
            /// 开始
            /// </summary>
            Start = 1,

            /// <summary>
            /// 处理状态
            /// </summary>
            ProcessStatus = 2,

            /// <summary>
            /// 成功
            /// </summary>
            Successfully = 3,

            /// <summary>
            /// 失败
            /// </summary>
            Failed = 4
        }

        #region Invoke通知状态
        /// <summary>
        /// 通知状态
        /// </summary>
        /// <param name="obj"></param>        
        private void NotifyStatus(object[] obj)
        {
            Invoke(new NotifyStatusDelegate(UpdateStatus), new object[] { obj });
        }
        #endregion

        private void DisableControls(bool disable)
        {
            if (true == disable)
            {
                toolStripStatusLabel_gif.Visible = true;
                btn_Export.Enabled = false;
                txtBox_Folder.ReadOnly = true;
                btn_SelectFolder.Enabled = false;
                checkedListBox1.Enabled = false;
                dateTimePicker_StartDate.Enabled = false;
                dateTimePicker_EndDate.Enabled = false;
            }
            else
            {
                toolStripStatusLabel_gif.Visible = false;
                btn_Export.Enabled = true;
                txtBox_Folder.ReadOnly = false;
                btn_SelectFolder.Enabled = true;
                checkedListBox1.Enabled = true;
                dateTimePicker_StartDate.Enabled = true;
                dateTimePicker_EndDate.Enabled = true;
            }
        }

        #region 更新状态
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateStatus(object[] obj)
        {
            #region switch body
            switch ((NotifyType)obj[0])
            {
                case NotifyType.Start:
                    DisableControls(true);
                    break;
                case NotifyType.ProcessStatus:
                    this.toolStripStatusLabel_Status.Text = obj[1].ToString();
                    break;
                case NotifyType.Failed:
                    this.toolStripStatusLabel_Status.Text = obj[1].ToString();
                    string strFailedMsg = string.Format("Location:{0}\nError:{1}\nDatails:{2}", obj[1], ((Exception)obj[2]).Message, ((Exception)obj[2]).StackTrace);
                    Common.WriteLog(strFailedMsg);
                    MessageBox.Show(strFailedMsg, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DisableControls(false);
                    lstTempWebSites.Clear();
                    break;
                case NotifyType.Successfully:
                    TimeSpan span = DateTime.Now - dtStartTime;
                    double totalSeconds = span.TotalSeconds; //耗时
                    this.toolStripStatusLabel_Status.Text = obj[1].ToString();
                    DisableControls(false);
                    MessageBox.Show(string.Format("{0}\nTime-consuming：{1}s\nPath：{2}", obj[1], totalSeconds, obj[2]), "Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(strSaveFolder);
                    lstTempWebSites.Clear();
                    break;
            }
            #endregion
        }
        #endregion

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}", this.Text, Common.AssemblyFileVersion());
            dateTimePicker_StartDate.Value = DateTime.Now.AddDays(-1);
            dateTimePicker_StartDate.Value = Convert.ToDateTime(dateTimePicker_StartDate.Value.ToString("yyyy-MM-dd 00:00"));
            dateTimePicker_EndDate.Value = Convert.ToDateTime(dateTimePicker_StartDate.Value.ToString("yyyy-MM-dd 23:59"));
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(File.ReadAllText(Path.Combine(System.Environment.CurrentDirectory, "settings.xml")));
            xmlNodes = xdoc["settings"]["website"].ChildNodes;
            strPwdHashCode = xdoc["settings"]["pwd"].InnerText;
            foreach (XmlNode website in xmlNodes)
            {
                this.checkedListBox1.Items.Add(website.Attributes[0].Value.Trim(), true);
            }
        }

        private void btn_Export_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Pwd.Text))
            {
                MessageBox.Show("PLEASE INPUT PASSWORD!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox_Pwd.Focus();
                return;
            }

            if (strPwdHashCode.ToUpper() != Common.GetMd5Hash(textBox_Pwd.Text).ToUpper())
            {
                MessageBox.Show("PASSWORD ERROR!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox_Pwd.Focus();
                return;
            }

            if (dateTimePicker_StartDate.Value > dateTimePicker_EndDate.Value)
            {
                MessageBox.Show("END DATE MUST AFTER START DATE!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker_EndDate.Focus();
                return;
            }

            strSaveFolder = this.txtBox_Folder.Text;
            if (string.IsNullOrEmpty(strSaveFolder))
            {
                MessageBox.Show("PLEASE CHOOSE SAVE PATH!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btn_SelectFolder.Focus();
                return;
            }
            else
            {
                dtStartTime = DateTime.Now;
                if (Directory.Exists(strSaveFolder) == true)
                {
                    foreach (object item in checkedListBox1.CheckedItems)
                    {
                        foreach (XmlNode website in xmlNodes)
                        {
                            if (item.ToString() == website.Attributes[0].Value.Trim())
                            {
                                lstTempWebSites.Add(website.InnerText.Trim());
                                break;
                            }
                        }
                    }

                    dtStartDate = this.dateTimePicker_StartDate.Value;
                    dtEndDate = this.dateTimePicker_EndDate.Value;
                    thGetOrder = new Thread(new ThreadStart(GetOrdersByDateScope));
                    thGetOrder.IsBackground = true;
                    thGetOrder.Start();
                }
                else
                {
                    MessageBox.Show("INVALID DIRECTORY PATH!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void GetOrdersByDateScope()
        {
            List<ExportDataEntity> lstExportData = new List<ExportDataEntity>();
            string strPath = string.Empty;
            //更新进度
            NotifyStatus(new object[] { NotifyType.Start });
            try
            {
                //更新进度
                NotifyStatus(new object[] { NotifyType.ProcessStatus, "Downloading data, please wait..." });
                Thread.Sleep(500);
                lstOrders = GetAllWebSitesOrdersByDateScope(lstTempWebSites, this.dateTimePicker_StartDate.Value, this.dateTimePicker_EndDate.Value);
                //更新进度
                NotifyStatus(new object[] { NotifyType.ProcessStatus, "Download successfully, please wait..." });
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                //更新进度
                NotifyStatus(new object[] { NotifyType.Failed, "Download failed.", ex });
                return;
            }

            try
            {
                //更新进度
                NotifyStatus(new object[] { NotifyType.ProcessStatus, "Transition to export data, please wait..." });
                Thread.Sleep(500);
                lstExportData = TransExportDataFormatByOrders(lstOrders);
                //更新进度
                NotifyStatus(new object[] { NotifyType.ProcessStatus, "Transition successfully, please wait..." });
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                //更新进度
                NotifyStatus(new object[] { NotifyType.Failed, "Transition to export data failed.", ex });
                return;
            }

            try
            {
                //更新进度
                NotifyStatus(new object[] { NotifyType.ProcessStatus, "Export data to csv file, please wait..." });
                Thread.Sleep(500);
                strPath = Path.Combine(strSaveFolder, DateTime.Now.ToString("yyyy-MM-dd(HHmmss)") + ".csv");
                Common.SaveDataToCSVFile(lstExportData, strPath);
                //更新进度
                NotifyStatus(new object[] { NotifyType.Successfully, "Export data successfully!", strPath });
            }
            catch (Exception ex)
            {
                //更新进度
                NotifyStatus(new object[] { NotifyType.Failed, "Export to export data failed.", ex });
                return;
            }
        }

        private void btn_SelectFolder_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = "Please choose a folder to save csv file.";
            this.folderBrowserDialog1.ShowDialog();
            if (string.IsNullOrEmpty(folderBrowserDialog1.SelectedPath) == false)
            {
                this.txtBox_Folder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private List<OrderDetailEntity> GetAllWebSitesOrdersByDateScope(List<string> lstWebSites, DateTime dtStartDate, DateTime dtEndDate)
        {
            lstOrders = new List<OrderDetailEntity>();
            ApiFieldsEntity apiFields = new ApiFieldsEntity();
            OrdersEntity entity = new OrdersEntity();
            string result = string.Empty;
            foreach (string strWebSite in lstWebSites)
            {
                //更新进度
                NotifyStatus(new object[] { NotifyType.ProcessStatus, string.Format("Downloading data, please wait... ({0}/{1})", lstWebSites.IndexOf(strWebSite) + 1, lstWebSites.Count) });
                apiFields = new ApiFieldsEntity();
                apiFields.Limit = 250;
                apiFields.WebSite = strWebSite;
                apiFields.User = Common.GetUser(apiFields.WebSite);
                apiFields.Pwd = Common.GetPwd(apiFields.WebSite);
                apiFields.AnyStatus = true;
                if (dtStartDate != DateTime.MinValue)
                {
                    apiFields.StartDate = dtStartDate;
                }
                if (dtEndDate != DateTime.MinValue)
                {
                    apiFields.EndDate = dtEndDate;
                }
                string strUrl = Common.ProduceApiUrl(apiFields);
                //先判断有多少条符合条件的订单
                int count = Order.GetOrderCountByUrl(strUrl, apiFields.User, apiFields.Pwd);
                int pages = Convert.ToInt32(Math.Ceiling((double)count / (double)250));
                for (int i = 0; i < pages; i++)
                {
                    entity = new OrdersEntity();
                    result = Order.GetOrderByUrl(string.Format("{0}&page={1}", strUrl, i + 1), apiFields.User, apiFields.Pwd);
                    entity = Common.JsonStringToEntity<OrdersEntity>(result, typeof(OrdersEntity));
                    lstOrders.AddRange(entity.Orders);
                }
            }
            return lstOrders;
        }

        private List<ExportDataEntity> TransExportDataFormatByOrders(List<OrderDetailEntity> lstOrders)
        {
            List<ExportDataEntity> lstExportData = new List<ExportDataEntity>();
            ExportDataEntity exportData = new ExportDataEntity();
            foreach (OrderDetailEntity order in lstOrders)
            {
                if (order.name.ToString().Trim() == "#38626")
                {
                }
                foreach (line_itemEntity item in order.line_items)
                {
                    try
                    {
                        exportData = new ExportDataEntity();
                        exportData.Name = order.name;
                        exportData.Email = order.email;
                        if (order.line_items.IndexOf(item) == 0)
                        {
                            exportData.Financial_Status = order.financial_status;
                            exportData.Paid_at = ((DateTime)order.created_at).ToString("yyyy-MM-dd HH*mm*ss zzz").Replace(":", "").Replace("*", ":");
                            exportData.Fulfillment_Status = order.fulfillment_status;
                            exportData.Fulfilled_at = "";//?
                            exportData.Accepts_Marketing = ((bool)order.buyer_accepts_marketing) ? "yes" : "no";//?
                            exportData.Currency = order.currency;
                            exportData.Subtotal = order.subtotal_price;//?
                            exportData.Taxes = order.total_tax;//?
                            exportData.Total = order.total_price;//?
                            exportData.Discount_Code = string.Empty;
                            //exportData.Discount_Code = order.discount_codes;
                            //exportData.Discount_Code = exportData.Discount_Code.ToString() == "[]" ? string.Empty : exportData.Discount_Code;
                            exportData.Discount_Amount = order.total_discounts;//?

                            exportData.Lineitem_compare_at_price = "";//?
                            if (null != order.billing_address)
                            {
                                exportData.Billing_Name = order.billing_address.name;
                                exportData.Billing_Street = string.Format("{0}, {1}", order.billing_address.address1, order.billing_address.address2);//?
                                exportData.Billing_Address1 = order.billing_address.address1;
                                exportData.Billing_Address2 = order.billing_address.address2;
                                exportData.Billing_Company = order.billing_address.company;
                                exportData.Billing_City = order.billing_address.city;
                                exportData.Billing_Zip = order.billing_address.zip;
                                exportData.Billing_Province = order.billing_address.province_code;
                                exportData.Billing_Country = order.billing_address.country_code;
                                exportData.Billing_Phone = order.billing_address.phone;
                                exportData.Shipping_Name = order.shipping_address.name;
                                exportData.Shipping_Street = string.Format("{0}, {1}", order.shipping_address.address1, order.shipping_address.address2);//?
                                exportData.Shipping_Address1 = order.shipping_address.address1;
                                exportData.Shipping_Address2 = order.shipping_address.address2;
                                exportData.Shipping_Company = order.shipping_address.company;
                                exportData.Shipping_City = order.shipping_address.city;
                                exportData.Shipping_Zip = order.shipping_address.zip;
                                exportData.Shipping_Province = order.shipping_address.province_code;
                                exportData.Shipping_Country = order.shipping_address.country_code;
                                exportData.Shipping_Phone = order.shipping_address.phone;
                            }
                            if (order.shipping_lines.Count>0)
                            {
                                if (order.shipping_lines[0].price != null)
                                {
                                    exportData.Shipping = order.shipping_lines[0].price;//?
                                }
                                if (order.shipping_lines[0].code != null)
                                {
                                    exportData.Shipping_Method = order.shipping_lines[0].code;//?
                                }
                            }                            

                            exportData.Notes = order.note;
                            exportData.Note_Attributes = order.note_attributes;
                            exportData.Note_Attributes = exportData.Note_Attributes.ToString() == "[]" ? string.Empty : exportData.Note_Attributes;
                            exportData.Cancelled_at = order.cancelled_at;
                            exportData.Payment_Method = "";//?
                            exportData.Payment_Reference = "";//?
                            exportData.Refunded_Amount = "";//?
                            exportData.Id = order.id;
                            exportData.Tags = order.tags;
                            exportData.Risk_Level = "";//?
                            exportData.Source = order.source_name;
                        }
                        exportData.Lineitem_quantity = order.line_items[order.line_items.IndexOf(item)].quantity;
                        exportData.Lineitem_name = order.line_items[order.line_items.IndexOf(item)].name;
                        exportData.Lineitem_price = order.line_items[order.line_items.IndexOf(item)].price;
                        exportData.Lineitem_sku = order.line_items[order.line_items.IndexOf(item)].sku;
                        exportData.Lineitem_requires_shipping = order.line_items[order.line_items.IndexOf(item)].requires_shipping;
                        exportData.Lineitem_taxable = order.line_items[order.line_items.IndexOf(item)].taxable;
                        exportData.Lineitem_fulfillment_status = order.line_items[order.line_items.IndexOf(item)].fulfillment_status;
                        exportData.Vendor = order.line_items[order.line_items.IndexOf(item)].vendor;
                        exportData.Created_at = ((DateTime)order.created_at).ToString("yyyy-MM-dd HH*mm*ss zzz").Replace(":", "").Replace("*", ":");
                        lstExportData.Add(exportData);
                    }
                    catch (Exception ex)
                    {
                        string str = ex.Message;
                    }
                }
            }
            return lstExportData;
        }
    }
}