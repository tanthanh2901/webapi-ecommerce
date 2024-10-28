public class ZaloPayCallbackRequest
{
    public string app_id { get; set; }
    public string app_trans_id { get; set; }
    public int status { get; set; }
    public int zp_trans_id { get; set; }
    public long server_time { get; set; }
    public string mac { get; set; }
}
