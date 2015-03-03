using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportCsvFiletTool.Entity
{
    public class OrdersEntity
    {
        public List<OrderDetailEntity> Orders { get; set; }
    }
    public class OrderDetailEntity
    {
        public object buyer_accepts_marketing { get; set; }
        public object cancel_reason { get; set; }
        public object cancelled_at { get; set; }
        public object cart_token { get; set; }
        public object checkout_token { get; set; }
        public object closed_at { get; set; }
        public object confirmed { get; set; }
        public object created_at { get; set; }
        public object currency { get; set; }
        public object email { get; set; }
        public object financial_status { get; set; }
        public object fulfillment_status { get; set; }
        public object gateway { get; set; }
        public object id { get; set; }
        public object landing_site { get; set; }
        public object location_id { get; set; }
        public object name { get; set; }
        public object note { get; set; }
        public object number { get; set; }
        public object reference { get; set; }
        public object referring_site { get; set; }
        public object source { get; set; }
        public object source_identifier { get; set; }
        public object source_name { get; set; }
        public object source_url { get; set; }
        public object subtotal_price { get; set; }
        public object taxes_included { get; set; }
        public object test { get; set; }
        public object token { get; set; }
        public object total_discounts { get; set; }
        public object total_line_items_price { get; set; }
        public object total_price { get; set; }
        public object total_price_usd { get; set; }
        public object total_tax { get; set; }
        public object total_weight { get; set; }
        public object updated_at { get; set; }
        public object user_id { get; set; }
        public object browser_ip { get; set; }
        public object landing_site_ref { get; set; }
        public object order_number { get; set; }
        public object discount_codes { get; set; }
        public object note_attributes { get; set; }
        public object processing_method { get; set; }
        public object checkout_id { get; set; }
        public object tax_lines { get; set; }
        public object tags { get; set; }
        public List<line_itemEntity> line_items { get; set; }
        public List<shipping_lineEntity> shipping_lines { get; set; }
        public payment_detailsEntity payment_details { get; set; }
        public billing_addressEntity billing_address { get; set; }
        public billing_addressEntity shipping_address { get; set; }
        public object fulfillments { get; set; }
        public client_detailsEntity client_details { get; set; }
        public object refunds { get; set; }
        public customerEntity customer { get; set; }
    }

    public class line_itemEntity
    {
        public object fulfillment_service { get; set; }
        public object fulfillment_status { get; set; }
        public object gift_card { get; set; }
        public object grams { get; set; }
        public object id { get; set; }
        public object price { get; set; }
        public object product_id { get; set; }
        public object quantity { get; set; }
        public object requires_shipping { get; set; }
        public object sku { get; set; }
        public object taxable { get; set; }
        public object title { get; set; }
        public object variant_id { get; set; }
        public object variant_title { get; set; }
        public object vendor { get; set; }
        public object name { get; set; }
        public object variant_inventory_management { get; set; }
        public object properties { get; set; }
        public object product_exists { get; set; }
        public object fulfillable_quantity { get; set; }
        public object tax_lines { get; set; }
    }

    public class shipping_lineEntity
    {
        public object code { get; set; }
        public object price { get; set; }
        public object source { get; set; }
        public object title { get; set; }
        public object tax_lines { get; set; }
    }

    public class payment_detailsEntity
    {
        public object avs_result_code { get; set; }
        public object credit_card_bin { get; set; }
        public object cvv_result_code { get; set; }
        public object credit_card_number { get; set; }
        public object credit_card_company { get; set; }
    }

    public class billing_addressEntity
    {
        public object address1 { get; set; }
        public object address2 { get; set; }
        public object city { get; set; }
        public object company { get; set; }
        public object country { get; set; }
        public object first_name { get; set; }
        public object last_name { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public object phone { get; set; }
        public object province { get; set; }
        public object zip { get; set; }
        public object name { get; set; }
        public object country_code { get; set; }
        public object province_code { get; set; }
    }

    public class client_detailsEntity
    {
        public object accept_language { get; set; }
        public object browser_ip { get; set; }
        public object session_hash { get; set; }
        public object user_agent { get; set; }
    }

    public class customerEntity
    {
        public object accepts_marketing { get; set; }
        public object created_at { get; set; }
        public object email { get; set; }
        public object first_name { get; set; }
        public object id { get; set; }
        public object last_name { get; set; }
        public object last_order_id { get; set; }
        public object multipass_identifier { get; set; }
        public object note { get; set; }
        public object orders_count { get; set; }
        public object state { get; set; }
        public object total_spent { get; set; }
        public object updated_at { get; set; }
        public object verified_email { get; set; }
        public object tags { get; set; }
        public object last_order_name { get; set; }
        public default_addressEntity default_address { get; set; }

    }

    public class default_addressEntity
    {
        public object address1 { get; set; }
        public object address2 { get; set; }
        public object city { get; set; }
        public object company { get; set; }
        public object country { get; set; }
        public object first_name { get; set; }
        public object id { get; set; }
        public object last_name { get; set; }
        public object phone { get; set; }
        public object province { get; set; }
        public object zip { get; set; }
        public object name { get; set; }
        public object province_code { get; set; }
        public object country_code { get; set; }
        public object country_name { get; set; }
        public object @default { get; set; }
    }

    public class OrderCountEntity
    {
        public int count { get; set; }
    }
}
