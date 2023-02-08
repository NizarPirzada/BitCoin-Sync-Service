using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcSync.Repository
{
    public class Entities
    {
    }

    public class BitCoinTransaction
    {
        public int bitcoin_transaction_id { get; set; }
        public string description { get; set; }
        public string source_wallet_key { get; set; }
        public string destination_wallet_key { get; set; }
        public DateTime btc_server_time { get; set; }
        public string btc_transaction_id { get; set; }
        public DateTime created_date { get; set; }
        public double btc_amount { get; set; }
        public string transaction_type { get; set; }
        public string account{ get; set; }
        public string label { get; set; }
        public double mining_fees { get; set; }
        public int mining_confirmations { get; set; }
    }

    public class UserWalletTransaction
    {
        public int id    { get; set; }
        public string trans_desc { get; set; }
        public double money_in { get; set; }
        public double money_out { get; set; }
        public double balance { get; set; }
        public string status { get; set; }
        public int user_id { get; set; }
        public int transaction_status_id { get; set; }
        public string transaction_type { get; set; }
        public int wallet_Id { get; set; }
        public string source_wallet_key { get; set; }
        public string destination_wallet_key { get; set; }
        public DateTime created_at { get; set; }
        public double mining_fees { get; set; }
        public int mining_confirmations { get; set; }
        public string btc_transaction_id { get; set; }

    }

    public class UserWallet
    {        
        public int user_wallet_id { get; set; }        
        public int user_id { get; set; }
        public int wallet_Id { get; set; }
        public string wallet_key { get; set; }        
        public DateTime created_at { get; set; }
        public int wallet_type_id { get; set; }
        public double wallet_balance { get; set; }        
    }
}

