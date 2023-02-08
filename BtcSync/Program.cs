using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BitcoinLib.Auxiliary;
using BitcoinLib.ExceptionHandling.Rpc;
using BitcoinLib.Responses;
using BitcoinLib.Services.Coins.Base;
using BitcoinLib.Services.Coins.Bitcoin;
using BitcoinLib.Services;
using System.Globalization;
using System.Configuration;
using BtcSync.Repository;
using BtcSync.Helper;

namespace BtcSync
{
    class Program
    {
        private static readonly ICoinService CoinService = new BitcoinService(useTestnet: false);
        static void Main(string[] args)
        {
            AutomateBitCoin();
            //BitcoinCalls();
        }

        private static void AutomateBitCoin()
        {
            callLog("!!Service Started!!");
            try
            {
                //get all bitcoin transactions
                //add them in db and check if not exists.
                //create another table for bitcoin transactions
                //
                TransactionRepository repo = new TransactionRepository();
                var transactionList = repo.GetAllTransactions();
                callLog("Transaction Received from Db");
                var myTransactions = CoinService.ListTransactions(null, int.MaxValue, 0);
                callLog("Transactions Received from Bitcoin Server");
                List<BitCoinTransaction> bitCoinTransactions = new List<BitCoinTransaction>();
                foreach (var transaction in myTransactions)
                {                    
                    bitCoinTransactions.Add(new BitCoinTransaction()
                    {
                        btc_transaction_id = transaction.TxId,
                        btc_amount = (double)transaction.Amount,
                        btc_server_time = UnixTime.UnixTimeToDateTime(transaction.TimeReceived),
                        source_wallet_key = transaction.Address,
                        created_date = DateTime.UtcNow,
                        description = transaction.Category,
                        transaction_type = transaction.Category,
                        destination_wallet_key = "",
                        account = transaction.Account,
                        label = transaction.Label,
                        mining_fees = Math.Abs((double)transaction.Fee),
                        mining_confirmations = transaction.Confirmations
                    });                    
                }
                callLog("Total Number of items to add " + bitCoinTransactions.Count);
                if (bitCoinTransactions.Count > 0)
                {
                    bitCoinTransactions.ForEach(x =>
                    {

                        bool exists = false;
                        if (transactionList?.Count > 0)
                        {
                            exists = transactionList.Any(t => t.btc_transaction_id == x.btc_transaction_id);
                        }
                        if (!exists)
                        {                            
                            #region Add Work

                            //update amount in user wallet 
                            //Add transaction in user wallet transaction  
                            callLog("Added to BTC Transactions: ");
                            repo.AddTransactions(x);

                            UserWallet user_wallet = null;
                            callLog("Getting Wallet By Key: " + x.source_wallet_key);
                            if (x.transaction_type == "send" && !string.IsNullOrEmpty(x.account.Trim()))
                            {
                                user_wallet = repo.GetUserWalletByUserId(Convert.ToInt32(x.account.Replace("'", "").Replace("'", "")));
                            }
                            else if (x.transaction_type != "send")
                            {
                                user_wallet = repo.GetUserWalletByKey(x.source_wallet_key);
                            }
                            //else
                            //{
                            //    var senderAddrress = CoinService.GetTransactionSenderAddress(x.btc_transaction_id);

                            //}

                            if (user_wallet != null)
                            {
                                callLog("User Wallet Not Null, UID: " + user_wallet.user_id + " UserWalletId: " + user_wallet.user_wallet_id);
                                double totalBalanceAmount = 0;
                                double balanceAmount = Math.Abs(x.btc_amount);
                                double moneyIn = 0;
                                double moneyOut = 0;
                                string sourceWallet = "";
                                UserWalletTransaction uwTr = new UserWalletTransaction();
                                callLog("Transaction Type: " + x.transaction_type);
                                callLog("Transaction Amount: " + balanceAmount);
                                if (x.transaction_type == "send")
                                {
                                    //amount comes negative
                                    moneyOut = (double)balanceAmount;
                                    moneyIn = 0;
                                    totalBalanceAmount = user_wallet.wallet_balance - balanceAmount - x.mining_fees;
                                    uwTr.trans_desc = "Bitcoins_Sent";
                                    sourceWallet = user_wallet.wallet_key;
                                }
                                else
                                {
                                    moneyOut = 0;
                                    moneyIn = balanceAmount;
                                    totalBalanceAmount = user_wallet.wallet_balance + balanceAmount;
                                    //receive amount comes positive
                                    uwTr.trans_desc = "Bitcoins_Received";
                                    sourceWallet = x.source_wallet_key;
                                }

                                uwTr.money_out = moneyOut;
                                uwTr.money_in = moneyIn;
                                uwTr.source_wallet_key = sourceWallet;
                                uwTr.user_id = user_wallet.user_id;
                                uwTr.wallet_Id = user_wallet.wallet_Id;
                                uwTr.transaction_status_id = 3;
                                uwTr.status = "Completed";
                                uwTr.created_at = x.created_date;
                                uwTr.balance = totalBalanceAmount;
                                uwTr.mining_fees = x.mining_fees;
                                uwTr.mining_confirmations = x.mining_confirmations;
                                uwTr.btc_transaction_id = x.btc_transaction_id;
                                try
                                {
                                    repo.AddUserWalletTransaction(uwTr);
                                }
                                catch (Exception ex)
                                {

                                }
                                callLog("Added to User Wallet Transactions");

                                repo.UpdateUserWallet(user_wallet.user_id, totalBalanceAmount);
                                callLog("Updated User Wallet Sum");
                            }
                            #endregion
                        }
                        else
                        {
                            #region Update Work
                            callLog("Update Transaction:" + x.btc_transaction_id + " Amount:"+x.btc_amount);

                            repo.UpdateBitcoinTransaction(x);
                            
                            repo.UpdateUserWalletTransactions(new UserWalletTransaction() {
                                mining_confirmations =x.mining_confirmations,
                                btc_transaction_id =x.btc_transaction_id });
                            #endregion
                        }
                    });
                }
                callLog("!!Service Completed!!");

                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                LogServices.LogText(MethodInfo.GetCurrentMethod().Name, ex.ToString(), "Log.txt", DateTime.UtcNow);
            }
        }

        public static void callLog(string logText)
        {
            Console.WriteLine("Transaction Got from Bitcoin Server");
            LogServices.LogText(MethodInfo.GetCurrentMethod().Name, logText, "Log.txt", DateTime.UtcNow);
        }

        public void GetTransactions()
        {

        }

        public static void BitcoinCalls()
        {
            try
            {
                Console.Write("\n\nConnecting to {0} {1}Net via RPC at {2}...", CoinService.Parameters.CoinLongName, (CoinService.Parameters.UseTestnet ? "Test" : "Main"), CoinService.Parameters.SelectedDaemonUrl);

                //  Network difficulty
                var networkDifficulty = CoinService.GetDifficulty();
                Console.WriteLine("[OK]\n\n{0} Network Difficulty: {1}", CoinService.Parameters.CoinLongName, networkDifficulty.ToString("#,###", CultureInfo.InvariantCulture));
                // Accounts
                var myAccounts = CoinService.ListAccounts();
                Console.WriteLine("\nMy balance: {0} {1}", myAccounts, CoinService.Parameters.CoinShortName);
                // Lables
                var myLabels = CoinService.ListLabels();
                Console.WriteLine("\nMy balance: {0} {1}", myLabels, CoinService.Parameters.CoinShortName);



                //var myAddresses = CoinService.ListReceivedByAddress();
                //var tet = CoinService.GetAddressInfo("3FWpNZvWy2UVWZcqNjFqZnuVDGADfRJBPC");
                //var tet2 = CoinService.GetWalletInfo();
                //var tet1 = CoinService.GetAddressesByLabel("");
                //var tet3 = CoinService.ListUnspent();
                //var tet3 = CoinService.GetAddressBalance("3CBG2PsaMwXNSYfc479ssiAUNpHbrMiveR");
                //var tet3 = CoinService.get
                //var tet4 = CoinService.Move()

                //var tet4 = CoinService.ListReceivedByAddress();



                //var tet4 = CoinService.SendFrom("3FWpNZvWy2UVWZcqNjFqZnuVDGADfRJBPC", "1JBsFahBmm7ft2ZrLkdK3uiRJtHwb3cTmA", (decimal)0.0012);
                //var test = CoinService.SendFrom("3CBG2PsaMwXNSYfc479ssiAUNpHbrMiveR", "",1);
                //var tet = CoinService.GetAddressBalance("3CBG2PsaMwXNSYfc479ssiAUNpHbrMiveR");
                //var tet = CoinService.GetAddressBalance("3Pf16MKZ3iAH6hLRSUz2M9a9k5aCBijArE");
                //var tet = CoinService.GetAddressBalance("3LDW6yGq22sES54Jn5oNZuXpkjRBy2UQoq");
                var tet = CoinService.GetAddressBalance("3FWpNZvWy2UVWZcqNjFqZnuVDGADfRJBPC");
                var tet2 = CoinService.ListAccounts();



                //  My balance
                var myBalance = CoinService.GetBalance();
                Console.WriteLine("\nMy balance: {0} {1}", myBalance, CoinService.Parameters.CoinShortName);

                //  Current block
                Console.WriteLine("Current block: {0}",
                    CoinService.GetBlockCount().ToString("#,#", CultureInfo.InvariantCulture));

                //  Wallet state
                Console.WriteLine("Wallet state: {0}", CoinService.IsWalletEncrypted() ? "Encrypted" : "Unencrypted");

                //  Keys and addresses
                if (myBalance > 0)
                {
                    //  My non-empty addresses
                    Console.WriteLine("\n\nMy non-empty addresses:");

                    var myNonEmptyAddresses = CoinService.ListReceivedByAddress();

                    foreach (var address in myNonEmptyAddresses)
                    {
                        Console.WriteLine("\n--------------------------------------------------");
                        Console.WriteLine("Account: " + (string.IsNullOrWhiteSpace(address.Account) ? "(no label)" : address.Account));
                        Console.WriteLine("Address: " + address.Address);
                        Console.WriteLine("Amount: " + address.Amount);
                        Console.WriteLine("Confirmations: " + address.Confirmations);
                        Console.WriteLine("--------------------------------------------------");
                    }

                    //  My private keys
                    if (bool.Parse(ConfigurationManager.AppSettings["ExtractMyPrivateKeys"]) && myNonEmptyAddresses.Count > 0 && CoinService.IsWalletEncrypted())
                    {
                        const short secondsToUnlockTheWallet = 30;

                        Console.Write("\nWill now unlock the wallet for " + secondsToUnlockTheWallet + ((secondsToUnlockTheWallet > 1) ? " seconds" : " second") + "...");
                        CoinService.WalletPassphrase(CoinService.Parameters.WalletPassword, secondsToUnlockTheWallet);
                        Console.WriteLine("[OK]\n\nMy private keys for non-empty addresses:\n");

                        foreach (var address in myNonEmptyAddresses)
                        {
                            Console.WriteLine("Private Key for address " + address.Address + ": " + CoinService.DumpPrivKey(address.Address));
                        }

                        Console.Write("\nLocking wallet...");
                        CoinService.WalletLock();
                        Console.WriteLine("[OK]");
                    }

                    //  My transactions 
                    Console.WriteLine("\n\nMy transactions: ");
                    var myTransactions = CoinService.ListTransactions(null, int.MaxValue, 0);

                    foreach (var transaction in myTransactions)
                    {
                        Console.WriteLine("\n---------------------------------------------------------------------------");
                        Console.WriteLine("Account: " + (string.IsNullOrWhiteSpace(transaction.Account) ? "(no label)" : transaction.Account));
                        Console.WriteLine("Address: " + transaction.Address);
                        Console.WriteLine("Category: " + transaction.Category);
                        Console.WriteLine("Amount: " + transaction.Amount);
                        Console.WriteLine("Fee: " + transaction.Fee);
                        Console.WriteLine("Confirmations: " + transaction.Confirmations);
                        Console.WriteLine("BlockHash: " + transaction.BlockHash);
                        Console.WriteLine("BlockIndex: " + transaction.BlockIndex);
                        Console.WriteLine("BlockTime: " + transaction.BlockTime + " - " + UnixTime.UnixTimeToDateTime(transaction.BlockTime));
                        Console.WriteLine("TxId: " + transaction.TxId);
                        Console.WriteLine("Time: " + transaction.Time + " - " + UnixTime.UnixTimeToDateTime(transaction.Time));
                        Console.WriteLine("TimeReceived: " + transaction.TimeReceived + " - " + UnixTime.UnixTimeToDateTime(transaction.TimeReceived));

                        if (!string.IsNullOrWhiteSpace(transaction.Comment))
                        {
                            Console.WriteLine("Comment: " + transaction.Comment);
                        }

                        if (!string.IsNullOrWhiteSpace(transaction.OtherAccount))
                        {
                            Console.WriteLine("Other Account: " + transaction.OtherAccount);
                        }

                        if (transaction.WalletConflicts != null && transaction.WalletConflicts.Any())
                        {
                            Console.Write("Conflicted Transactions: ");

                            foreach (var conflictedTxId in transaction.WalletConflicts)
                            {
                                Console.Write(conflictedTxId + " ");
                            }

                            Console.WriteLine();
                        }

                        Console.WriteLine("---------------------------------------------------------------------------");
                    }

                    //Transaction Details
                    //Console.WriteLine("\n\nMy transactions' details:");
                    //foreach (var transaction in myTransactions)
                    //{
                    //    // Move transactions don't have a txId, which this logic fails for
                    //    if (transaction.Category == "move")
                    //    {
                    //        continue;
                    //    }

                    //    var localWalletTransaction = CoinService.GetTransaction(transaction.TxId);
                    //    IEnumerable<PropertyInfo> localWalletTrasactionProperties = localWalletTransaction.GetType().GetProperties();
                    //    IList<GetTransactionResponseDetails> localWalletTransactionDetailsList = localWalletTransaction.Details.ToList();

                    //    Console.WriteLine("\nTransaction\n-----------");

                    //    foreach (var propertyInfo in localWalletTrasactionProperties)
                    //    {
                    //        var propertyInfoName = propertyInfo.Name;

                    //        if (propertyInfoName != "Details" && propertyInfoName != "WalletConflicts")
                    //        {
                    //            Console.WriteLine(propertyInfoName + ": " + propertyInfo.GetValue(localWalletTransaction, null));
                    //        }
                    //    }

                    //    foreach (var details in localWalletTransactionDetailsList)
                    //    {
                    //        IEnumerable<PropertyInfo> detailsProperties = details.GetType().GetProperties();
                    //        Console.WriteLine("\nTransaction details " + (localWalletTransactionDetailsList.IndexOf(details) + 1) + " of total " + localWalletTransactionDetailsList.Count + "\n--------------------------------");

                    //        foreach (var propertyInfo in detailsProperties)
                    //        {
                    //            Console.WriteLine(propertyInfo.Name + ": " + propertyInfo.GetValue(details, null));
                    //        }
                    //    }
                    //}

                    //  Unspent transactions
                    Console.WriteLine("\nMy unspent transactions:");
                    var unspentList = CoinService.ListUnspent();

                    foreach (var unspentResponse in unspentList)
                    {
                        IEnumerable<PropertyInfo> detailsProperties = unspentResponse.GetType().GetProperties();

                        Console.WriteLine("\nUnspent transaction " + (unspentList.IndexOf(unspentResponse) + 1) + " of " + unspentList.Count + "\n--------------------------------");

                        foreach (var propertyInfo in detailsProperties)
                        {
                            Console.WriteLine(propertyInfo.Name + " : " + propertyInfo.GetValue(unspentResponse, null));
                        }
                    }
                }

                Console.ReadLine();
            }
            catch (RpcInternalServerErrorException exception)
            {
                var errorCode = 0;
                var errorMessage = string.Empty;

                if (exception.RpcErrorCode.GetHashCode() != 0)
                {
                    errorCode = exception.RpcErrorCode.GetHashCode();
                    errorMessage = exception.RpcErrorCode.ToString();
                }

                Console.WriteLine("[Failed] {0} {1} {2}", exception.Message, errorCode != 0 ? "Error code: " + errorCode : string.Empty, !string.IsNullOrWhiteSpace(errorMessage) ? errorMessage : string.Empty);
            }
            catch (Exception exception)
            {
                Console.WriteLine("[Failed]\n\nPlease check your configuration and make sure that the daemon is up and running and that it is synchronized. \n\nException: " + exception);
            }
        }
    }
}
