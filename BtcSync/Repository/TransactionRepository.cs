using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BtcSync.Helper;


namespace BtcSync.Repository
{
    public class TransactionRepository : Repository
    {
        
        public List<BitCoinTransaction> GetUsersById(List<int> userIds)
        {
            string sql = @"select * from user_wallet_transactions";
            //MySqlParameter parameter = new MySqlParameter("@UserId", csv);
            DataSet ds = Helper.MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, null);
            if (ds?.Tables[0]?.Rows.Count == 0) return null;
            return ds.Tables[0].ToList<BitCoinTransaction>();
        }

        public void AddTransactions(BitCoinTransaction transaction)
        {
            string sql = @"insert into bitcoin_transaction values(null,@description,@source_wallet_key,@destination_wallet_key
                            ,@btc_server_time,@btc_transaction_id,@created_date,@btc_amount,@transaction_type,@account,@label,@mining_confirmations,@mining_fees);";
            MySqlParameter[] parameterArray = new MySqlParameter[]{
            new MySqlParameter("@description", transaction.description),
            new MySqlParameter("@source_wallet_key", transaction.source_wallet_key),
            new MySqlParameter("@destination_wallet_key", transaction.destination_wallet_key),
            new MySqlParameter("@btc_server_time", transaction.btc_server_time),
            new MySqlParameter("@btc_transaction_id", transaction.btc_transaction_id),
            new MySqlParameter("@created_date", transaction.created_date),
            new MySqlParameter("@btc_amount", transaction.btc_amount),
            new MySqlParameter("@transaction_type", transaction.transaction_type),
            new MySqlParameter("@account", transaction.account),
            new MySqlParameter("@label", transaction.label),
            new MySqlParameter("@mining_fees", transaction.mining_fees),
            new MySqlParameter("@mining_confirmations", transaction.mining_confirmations)
            };

            Helper.MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, parameterArray);            
        }

        public List<BitCoinTransaction> GetAllTransactions()
        {
            string sql = @"select * from bitcoin_transaction";            
            DataSet ds = Helper.MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, null);
            if (ds?.Tables[0]?.Rows.Count == 0) return null;
            return ds.Tables[0].ToList<BitCoinTransaction>();
        }

        public void UpdateUserWallet(string wallet_key, double amount)
        {
            string sql = @"update user_wallet 
                            set wallet_balance = " + amount
                            + " where wallet_key= '" + wallet_key
                            + "' and wallet_id = 4";

            Helper.MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, null);
        }

        public void UpdateUserWallet(int userId, double amount)
        {
            string sql = @"update user_wallet 
                            set wallet_balance = " + amount
                            + " where user_id= " + userId
                            + " and wallet_id = 4";
            Helper.MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, null);
        }

        public UserWallet GetUserWalletByKey(string wallet_key)
        {
            string sql = @"select * from user_wallet where wallet_key = '" + wallet_key + "'";

            DataSet ds = Helper.MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, null);
            if (ds?.Tables[0]?.Rows.Count == 0) return default(UserWallet);
            return ds.Tables[0].ToList<UserWallet>().FirstOrDefault();
        }

        public UserWallet GetUserWalletByUserId(int user_id)
        {
            string sql = @"select * from user_wallet where user_id = " + user_id + " and wallet_id = 4";

            DataSet ds = Helper.MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, null);
            if (ds?.Tables[0]?.Rows.Count == 0) return default(UserWallet);
            return ds.Tables[0].ToList<UserWallet>().FirstOrDefault();
        }

        public void AddUserWalletTransaction(UserWalletTransaction uwTransaction)
        {
            string sql = @"insert into user_wallet_transactions(
                        trans_desc,
                        money_in,
                        money_out,
                        balance,
                        status,
                        created_at,
                        user_id,
                        transaction_status_id,
                        wallet_Id,
                        source_wallet_key,
                        mining_fees,
                        mining_confirmations,
                        btc_transaction_id
                        ) values (
                        @trans_desc,
                        @money_in,
                        @money_out,
                        @balance,
                        @status,
                        @created_at,
                        @user_id,
                        @transaction_status_id,
                        @wallet_Id,
                        @source_wallet_key,
                        @mining_fees,
                        @mining_confirmations,
                        @btc_transaction_id
                        )";

            MySqlParameter[] parameterArray = new MySqlParameter[]{
            new MySqlParameter("@trans_desc", uwTransaction.trans_desc),
            new MySqlParameter("@money_in", uwTransaction.money_in),
            new MySqlParameter("@money_out", uwTransaction.money_out),
            new MySqlParameter("@balance", uwTransaction.balance),
            new MySqlParameter("@status", uwTransaction.status),
            new MySqlParameter("@created_at", uwTransaction.created_at),
            new MySqlParameter("@user_id", uwTransaction.user_id),
            new MySqlParameter("@transaction_status_id", uwTransaction.transaction_status_id),
            new MySqlParameter("@wallet_Id", uwTransaction.wallet_Id),
            new MySqlParameter("@source_wallet_key", uwTransaction.source_wallet_key),
            new MySqlParameter("@mining_fees", uwTransaction.mining_fees),
            new MySqlParameter("@mining_confirmations", uwTransaction.mining_confirmations),
            new MySqlParameter("@btc_transaction_id", uwTransaction.btc_transaction_id)
            };

            Helper.MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, parameterArray);
        }

        public void UpdateUserWalletTransactions(UserWalletTransaction uwTransaction)
        {
            string sql = @"update user_wallet_transactions 
                            set mining_confirmations = " + uwTransaction.mining_confirmations
                            + " where btc_transaction_id= '" + uwTransaction.btc_transaction_id +"'";                            

            Helper.MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, null);
        }

        public void UpdateBitcoinTransaction(BitCoinTransaction bitcoinTransaction)
        {
            string sql = @"update bitcoin_transaction 
                            set mining_confirmations = " + bitcoinTransaction.mining_confirmations
                            + " where btc_transaction_id= '" + bitcoinTransaction.btc_transaction_id + "'";

            Helper.MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, null);
        }
    }
}
