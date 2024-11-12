using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Application.Services.Payment.ZaloPay
{
    public class CheckStatusResponse
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; } = string.Empty;
        public int sub_return_code { get; set; }

        public string sub_return_message { get; set; } = string.Empty;
        public bool is_processing;
        public int amount { get; set; }
        public int zp_trans_id { get; set; }

    }
}
