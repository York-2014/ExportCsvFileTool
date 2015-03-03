using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportCsvFiletTool.Entity
{
    public class ExportDataEntity
    {
        public object Name { get; set; }
        public object Email { get; set; }
        public object Financial_Status { get; set; }
        public object Paid_at { get; set; }
        public object Fulfillment_Status { get; set; }
        public object Fulfilled_at { get; set; }
        public object Accepts_Marketing { get; set; }
        public object Currency { get; set; }
        public object Subtotal { get; set; }
        public object Shipping { get; set; }
        public object Taxes { get; set; }
        public object Total { get; set; }
        public object Discount_Code { get; set; }
        public object Discount_Amount { get; set; }
        public object Shipping_Method { get; set; }
        public object Created_at { get; set; }
        public object Lineitem_quantity { get; set; }
        public object Lineitem_name { get; set; }
        public object Lineitem_price { get; set; }
        public object Lineitem_compare_at_price { get; set; }
        public object Lineitem_sku { get; set; }
        public object Lineitem_requires_shipping { get; set; }
        public object Lineitem_taxable { get; set; }
        public object Lineitem_fulfillment_status { get; set; }
        public object Billing_Name { get; set; }
        public object Billing_Street { get; set; }
        public object Billing_Address1 { get; set; }
        public object Billing_Address2 { get; set; }
        public object Billing_Company { get; set; }
        public object Billing_City { get; set; }
        public object Billing_Zip { get; set; }
        public object Billing_Province { get; set; }
        public object Billing_Country { get; set; }
        public object Billing_Phone { get; set; }
        public object Shipping_Name { get; set; }
        public object Shipping_Street { get; set; }
        public object Shipping_Address1 { get; set; }
        public object Shipping_Address2 { get; set; }
        public object Shipping_Company { get; set; }
        public object Shipping_City { get; set; }
        public object Shipping_Zip { get; set; }
        public object Shipping_Province { get; set; }
        public object Shipping_Country { get; set; }
        public object Shipping_Phone { get; set; }
        public object Notes { get; set; }
        public object Note_Attributes { get; set; }
        public object Cancelled_at { get; set; }
        public object Payment_Method { get; set; }
        public object Payment_Reference { get; set; }
        public object Refunded_Amount { get; set; }
        public object Vendor { get; set; }
        public object Id { get; set; }
        public object Tags { get; set; }
        public object Risk_Level { get; set; }
        public object Source { get; set; }
    }
}