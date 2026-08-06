namespace Summary.Shopfa
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public abstract class BaseResponseInfo
    {
        public string Error { get; set; }
        public int? Error_Code { get; set; }
    }

    public class BaseRequestInfo
    {
        public string ApiAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ResponseTokenInfo : BaseResponseInfo
    {
        public string Private_Key { get; set; }
    }

    public class UpdateProductResponseInfo : BaseResponseInfo
    {

    }

    public class ResponseInventoryInfo : BaseResponseInfo
    {
        public IList<InventoryProductInfo> Items { get; set; } = new List<InventoryProductInfo>();
    }

    public class RpeVariantProductInfo : BaseResponseInfo
    {
        public IList<VariantProductInfo> Items { get; set; } = new List<VariantProductInfo>();
    }

    public class VariantProductInfo
    {
        public int Id { get; set; }
        public List<RpeVariantInfo> Variants { get; set; } = new List<RpeVariantInfo>();
        public Dictionary<int, string> Variant_Options { get; set; } = new Dictionary<int, string>();
    }

    public class RpeVariantInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public int Old_Price { get; set; }
        public int Quantity { get; set; }
    }

    public class InventoryProductInfo
    {
        public int Product_Id { get; set; }
        public int Variant_Id { get; set; }
        public string Warehouse_Code { get; set; }
        public int Price { get; set; }
        public int Old_Price { get; set; }
        public short Status { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Extended { get; set; } = new Dictionary<string, object>();
    }

    public class CreateProductRequestInfo
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string text { get; set; }
        public string page_id { get; set; }
        public string status { get; set; }
        public string price { get; set; }
        public string old_price { get; set; }
        public string buy_price { get; set; }
        public string product_quantity { get; set; }
        public string description { get; set; }
        public string brand_id { get; set; }
        public string slug { get; set; }
        public string comment_title { get; set; }
        public string keywords { get; set; }
        public string weight { get; set; }
        public string page_title { get; set; }
        public string allow_comments { get; set; }
        public string allow_ratings { get; set; }
        public string special { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }

    public class GetOrdersResponseInfo : BaseResponseInfo
    {
        public List<Basket> Baskets { get; set; }
    }

    public class OrderSubmitModel
    {
        public string Event_Type { get; set; }
        public OrderSubmitData Data { get; set; }
    }

    public class ShopfaSigninResponse
    {
        public string Private_Key { get; set; }
        public bool Successful { get; set; }
    }

    public class OrderResponseModel
    {
        public IEnumerable<Basket> Baskets { get; set; }
    }

    public class Basket
    {
        public long Id { get; set; }
        public string Session { get; set; }
        public int User_Id { get; set; }
        public int State_Id { get; set; }
        public int City_Id { get; set; }
        public long Date { get; set; }
        public long Update { get; set; }
        public long Payment_Date { get; set; }
        public int Post_Method { get; set; }
        public string Post_Method_Title { get; set; }
        public int Payment_Method { get; set; }
        public string Payment_Method_Title { get; set; }
        public long Address_Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Postalcode { get; set; }
        public string Email { get; set; }
        public string Mellicode { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string Status_Title { get; set; }
        public int Weight { get; set; }
        public int Service_Price { get; set; }
        public int Tax_Price { get; set; }
        public int Post_Price { get; set; }
        public int Item_Price { get; set; }
        public int Sum_Price { get; set; }
        public string Delivery_Time { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public long Id { get; set; }
        public long Product_Id { get; set; }
        public int Price { get; set; }
        public int Old_Price { get; set; }
        public int Buy_Price { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public int Sum_Price { get; set; }
        public long Variant_Id { get; set; }
        public string Thumb { get; set; }
        public string Variant_Title { get; set; }
    }

    public class OrderSubmitData
    {
        public string Object_Type { get; set; }
        public string Action { get; set; }
        public IEnumerable<string> Items { get; set; }
    }

    public class ShopfaWebhookIncomingModel
    {
        public string Object_Type { get; set; }
        public List<string> Items { get; set; }
    }

    public class OrderInfo
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string Postalcode { get; set; }
        public string Email { get; set; }
        public string Mellicode { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public int Payment_Method { get; set; }
        public string Payment_Method_Title { get; set; }
        public int Post_Method { get; set; }
        public string Post_Method_Title { get; set; }
        public int Address_Id { get; set; }
        public string Address { get; set; }
        public int City_Id { get; set; }
        public string City { get; set; }
        public int State_Id { get; set; }
        public string State { get; set; }
        public long Date { get; set; }
        public string DateInFormat
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(Date).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public long Payment_Date { get; set; }
        public string PaymentDateInFormat
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(Payment_Date).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public long Update { get; set; }
        public string UpdateInFormat
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(Update).DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public int Status { get; set; }
        public string Status_Title { get; set; }
        public long Tax_Price { get; set; }
        public long Service_Price { get; set; }
        public int Weight { get; set; }
        public List<ItemInfo> Items { get; set; }
    }

    public class ItemInfo
    {
        public ulong Id { get; set; }
        public ulong Product_Id { get; set; }
        public ulong Price { get; set; }
        public ulong Old_Price { get; set; }
        public ulong Buy_Price { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public ulong Sum_Price { get; set; }
    }
}