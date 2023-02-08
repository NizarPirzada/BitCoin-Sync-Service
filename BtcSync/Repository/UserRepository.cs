//using Bravo.Core.Common;
//using Bravo.Core.DataInterfaces;
//using Bravo.Core.Entities;
//using Bravo.Core.Entities.Mongo;
//using Bravo.Core.Enum;
//using MySql.Data.MySqlClient;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.IO;
//using System.Linq;

//namespace Bravo.Repository
//{
//    public class UserRepository : Repository, IUserRepository
//    {
//        private DatabaseConnection _DBconnection = new DatabaseConnection();


//        public List<EUser> GetAllUsers()
//        {
//            string sql = @"SELECT * FROM user u WHERE is_active = 1 AND is_approve = 2";
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql);
//            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count < 1) return null;
//            return CollectionFromDataSet<EUser>(ds, UserFromDataRow);
//        }
//        public List<EUser> GetAllUsersWithInactive()
//        {
//            string sql = @"SELECT * FROM user u";
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql);
//            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count < 1) return null;
//            return CollectionFromDataSet<EUser>(ds, UserFromDataRow);
//        }
//        public void InsertInternalUser(long UserId)
//        {

//            string sql = @"Insert into internal_user(internal_user_id, user_id, date_time, user_email)
//                            values(null, @user_id, @date_time, NULL);";
//            MySqlParameter[] parameterArray = new MySqlParameter[]{
//                     new MySqlParameter("@user_id",   UserId),
//                     new MySqlParameter("@date_time",  DateTime.Now), };
//            MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, parameterArray);

//        }
//        public int CheckInternalUser(long UserId)
//        {
//            string sql = @"select user_id from internal_user where user_id=" + UserId + ";";
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql);
//            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count < 1)
//            {
//                return 0;
//            }
//            return 1;
//        }
//        public void RemoveInternalUser(long UserId)
//        {
//            string sql = @"delete from internal_user where user_id=" + UserId + ";";
//            MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql);
//        }
//        public EUser GetUser(System.Int64 userId)
//        {
//            string sql = "select u.*,(SELECT CONCAT(r.mobile_number,'-',r.reminder_registration_id) FROM reminder_registration r WHERE r.user_id=u.user_id AND" +
//                "(CASE WHEN r.is_active is NULL THEN 1 ELSE r.is_active END)=1 AND " +
//                "(CASE WHEN r.registration_status is NULL THEN 1 ELSE r.registration_status END)=1 limit 1 ) AS mobile_number" +
//                " from user u WHERE u.user_id = @UserId";
//            MySqlParameter parameter = new MySqlParameter("@UserId", userId);
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, new MySqlParameter[] { parameter });
//            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return null;
//            return UserFromDataRow(ds.Tables[0].Rows[0]);
//        }

//        public List<EUser> GetAllUser()
//        {
//            string sql = "select * from user where is_active=1 and email<>''";
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, null);
//            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) return null;
//            return CollectionFromDataSet<EUser>(ds, UserFromDataRow);
//        }

//        //fb_user_detail_id


//        public EUser GetUserImage(string UserEmail)
//        {
//            string sql = "select image,first_name,last_name from user where email=@UserEmail";
//            MySqlParameter parameter = new MySqlParameter("@UserEmail", UserEmail);
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, new MySqlParameter[] { parameter });
//            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1) return null;
//            EUser euser = null;
//            euser = new EUser();
//            euser.Image = ds.Tables[0].Rows[0]["image"].ToString();
//            euser.FirstName = ds.Tables[0].Rows[0]["first_name"].ToString();
//            euser.LastName = ds.Tables[0].Rows[0]["last_name"].ToString();
//            return euser;
//        }
//        public EUser GetUser(string emailId)
//        {
//            string sql = "select u.*,(SELECT CONCAT(r.mobile_number,'-',r.reminder_registration_id) FROM reminder_registration r WHERE r.user_id=u.user_id AND" +
//                "(CASE WHEN r.is_active is NULL THEN 1 ELSE r.is_active END)=1 AND " +
//                "(CASE WHEN r.registration_status is NULL THEN 1 ELSE r.registration_status END)=1 limit 1) AS mobile_number" +
//                " from user u WHERE u.email = @EmailId";
//            MySqlParameter parameter = new MySqlParameter("@EmailId", emailId);
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, new MySqlParameter[] { parameter });
//            if (ds == null || ds.Tables[0].Rows.Count < 1) return null;
//            return UserFromDataRow(ds.Tables[0].Rows[0]);
//        }


//        public string GetFBIdByUser(Int64 userId)
//        {
//            string sql = "select facebook_id from user where user_id = @user_id";
//            MySqlParameter parameter = new MySqlParameter("@user_id", userId);
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, new MySqlParameter[] { parameter });
//            if (ds?.Tables?[0].Rows.Count > 0) return ds.Tables[0].Rows[0][0].ToString();
//            return null;
//        }

//        public EUser GetUserByFacebookId(string facebookId)
//        {
//            string sql = "select u.*,(SELECT CONCAT(r.mobile_number,'-',r.reminder_registration_id) FROM reminder_registration r WHERE r.user_id=u.user_id AND" +
//                "(CASE WHEN r.is_active is NULL THEN 1 ELSE r.is_active END)=1 AND " +
//                "(CASE WHEN r.registration_status is NULL THEN 1 ELSE r.registration_status END)=1 limit 1) AS mobile_number" +
//                " from user u WHERE facebook_id = @FacebookId and u.is_active=1";
//            MySqlParameter parameter = new MySqlParameter("@FacebookId", facebookId);
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, new MySqlParameter[] { parameter });
//            if (ds == null || ds.Tables[0].Rows.Count < 1) return null;
//            return UserFromDataRow(ds.Tables[0].Rows[0]);
//        }

//        public EUser InsertUser(EUser entity)
//        {
//            if (entity != null)
//            {
//                string sql = @"INSERT INTO `user`
//(`user_id`,
//`first_name`,
//`middle_name`,
//`last_name`,
//`facebook_id`,
//`email`,
//`image`,
//`is_active`,
//`date_time`,
//`modified_date_time`,
//`popup_showed`,
//`user_role_id`,
//`profile_link`,
//`is_approve`,
//`gender`,
//`location`,
//`work`,
//`about`,
//`user_type_id`,
//`invitation_code_id`)
//VALUES
//(@user_id,
//@first_name,
//@middle_name,
//@last_name,
//@facebook_id,
//@email,
//@image,
//@is_active,
//@date_time,
//@modified_date_time,
//@popup_showed,
//@user_role_id,
//@profile_link,
//@is_approve,
//@gender,
//@location,
//@work,
//@about,
//@user_type_id,
//@invitation_code_id);
//                           SELECT LAST_INSERT_ID();";
//                var IsApprove = 2;
//                var IsInvited = 1;
//                if (entity.IsInvitationOn)
//                {
//                    IsApprove = 1;
//                    IsInvited = 0;
//                };

//                MySqlParameter[] parameterArray = new MySqlParameter[]{
//                     new MySqlParameter("@user_id",   entity.UserId),
//                     new MySqlParameter("@first_name",  entity.FirstName ?? (object)DBNull.Value),
//                     new MySqlParameter("@middle_name",  entity.MiddleName ?? (object)DBNull.Value),
//                     new MySqlParameter("@last_name",  entity.LastName ?? (object)DBNull.Value),
//                     new MySqlParameter("@facebook_id",  entity.FacebookID ?? (object)DBNull.Value),
//                     new MySqlParameter("@email",        entity.Email ?? (object)DBNull.Value),
//                     new MySqlParameter("@image",        entity.Image ?? (object)DBNull.Value),
//                     new MySqlParameter("@is_active",   IsInvited),
//                     new MySqlParameter("@date_time",   DateTime.UtcNow),
//                     new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//                     new MySqlParameter("@popup_showed",  entity.PopupShowed),
//                     new MySqlParameter("@user_role_id", entity.UserRoleId),
//                     new MySqlParameter("@profile_link",        entity.ProfileLink ?? (object)DBNull.Value),
//                     new MySqlParameter("@is_approve",   IsApprove),
//                     new MySqlParameter("@gender",        entity.Gender),
//                     new MySqlParameter("@location",   entity.Location),
//                     new MySqlParameter("@about",        entity.About),
//                     new MySqlParameter("@work",   entity.Work ?? (object)DBNull.Value),
//                     new MySqlParameter("@user_type_id",   entity.UserTypeId),
//                     new MySqlParameter("@invitation_code_id",   entity.InvitationCodeId)
//                };

//                var identity = MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, parameterArray);

//                if (identity == DBNull.Value)
//                    throw new DataException("Identity column was null as a result of the insert operation.");

//                return GetUser(Convert.ToInt64(identity));
//            }
//            return entity;
//        }


//        public void InsertFBUserDetail(EFbUserDetail entity)
//        {
//            string sql = @"INSERT INTO `fb_user_detail`
//                (
//                `user_id`,
//                `access_token`,
//                `expires_in`,
//                `signed_request`,
//                `creation_date`,
//                `modified_date`
//                )
//                VALUES
//                (
//                @user_id,
//                @access_token,
//                @expires_in,
//                @signed_request,
//                @creation_date,
//                @modified_date);
//                SELECT LAST_INSERT_ID();";

//            MySqlParameter[] parameterArray = new MySqlParameter[]{
//                     new MySqlParameter("@user_id",   entity.UserId),
//                     new MySqlParameter("@access_token",  entity.AccessToken ?? (object)DBNull.Value),
//                     new MySqlParameter("@expires_in",  entity.ExpiresIn ?? (object)DBNull.Value),
//                     new MySqlParameter("@signed_request",  entity.SignedRequest ?? (object)DBNull.Value),
//                     new MySqlParameter("@creation_date",  entity.CreationDate ),
//                     new MySqlParameter("@modified_date",        entity.ModifiedDate ),
//                };
//            MySqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sql, parameterArray);

//        }

//        public bool UpdateInviteFriendOnStartupFlag(System.Int64 userId, bool flag)
//        {
//            string sql = @"Update user
//                           set
//                                `invite_friends_on_startup_flag`  = @InviteFriendsOnStartupFlag,
//                                `modified_date_time` = @modified_date_time
//                           where 
//                                user_id = @UserId";

//            MySqlParameter[] parameters = new MySqlParameter[]{
//                new MySqlParameter("@UserId", userId),
//                new MySqlParameter("@InviteFriendsOnStartupFlag", flag),
//                new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//            };

//            int retVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);
//            return retVal > 0;
//        }

//        public bool UpdateInviteFriendShowed(System.Int64 userId, bool flag)
//        {
//            string sql = @"Update user
//                           set
//                                `invite_friends_showed`  = @invite_friends_showed,
//                                `modified_date_time` = @modified_date_time
//                           where 
//                                user_id = @UserId";

//            MySqlParameter[] parameters = new MySqlParameter[]{
//                new MySqlParameter("@UserId", userId),
//                new MySqlParameter("@invite_friends_showed", flag),
//                new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//            };

//            int retVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);
//            return retVal > 0;
//        }

//        public List<EUser> GetUserFriends(System.Int64 userId)
//        {
//            string sql = @"select t1.*, t2.relationship_status_id
//                           from user t1 inner join user_friend t2 on t1.user_id = t2.friend_id and t2.user_id = @UserId where t1.is_active=1 and t1.is_approve=2";

//            MySqlParameter[] parameters = new MySqlParameter[] {
//                new MySqlParameter("@UserId", userId)
//            };

//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, parameters);
//            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count == 0) return null;
//            return CollectionFromDataSet<EUser>(ds, UserFromDataRow);
//        }

//        public List<EUser> GetUserActualFriends(System.Int64 userId)
//        {
//            string sql = @"select t1.*, t2.relationship_status_id
//                           from user t1 inner join user_friend t2 on t1.user_id = t2.friend_id and t2.user_id = @UserId where t1.is_active=1 and t1.is_approve=2 AND t2.relationship_status_id=@RelationStatus";

//            MySqlParameter[] parameters = new MySqlParameter[] {
//                new MySqlParameter("@UserId", userId),
//                new MySqlParameter("@RelationStatus", (int)RelationshipStatuses.Friend)
//            };

//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, parameters);
//            if (ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count == 0) return null;
//            return CollectionFromDataSet<EUser>(ds, UserFromDataRow);
//        }

//        public int InsertUserFriend(Object obj)
//        {
//            EUserFriend userFriend = (EUserFriend)obj;
//            DbParameter[] parameters = new DbParameter[2];
//            parameters[0] = new MySqlParameter("p_user_id", userFriend.UserId);
//            parameters[1] = new MySqlParameter("p_facebook_id", userFriend.FacebookId);
//            return _DBconnection.ExecuteNonQuery("sp_add_user_friend", parameters);
//        }

//        public int InsertUserGenre(Object obj)
//        {
//            EUserGenre userGenre = (EUserGenre)obj;
//            DbParameter[] parameters = new DbParameter[2];
//            parameters[0] = new MySqlParameter("p_user_id", userGenre.UserID);
//            parameters[1] = new MySqlParameter("p_genres", userGenre.Genres);
//            return _DBconnection.ExecuteNonQuery("sp_add_user_genre", parameters);
//        }

//        public bool CheckPopupShowed(long userID)
//        {
//            DbParameter[] parameters = new DbParameter[2];
//            parameters[0] = new MySqlParameter("p_user_id", userID);
//            parameters[1] = new MySqlParameter("@p_popup_showed", SqlDbType.Bit);
//            parameters[1].Direction = ParameterDirection.Output;

//            _DBconnection.ExecuteNonQuery("sp_check_popup_showed", parameters);
//            return Convert.ToBoolean(parameters[1].Value);
//        }

//        public int PopupShowed(long userID)
//        {
//            DbParameter[] parameters = new DbParameter[1];
//            parameters[0] = new MySqlParameter("p_user_id", userID);
//            return _DBconnection.ExecuteNonQuery("sp_popup_showed", parameters);
//        }

//        public virtual EUser UserFromDataRow(DataRow dr)
//        {
//            if (dr == null) return null;
//            EUser entity = new EUser();
//            Miscellaneous _misc = new Miscellaneous();
//            if (dr.Table.Columns.Contains("user_id"))
//            {
//                entity.UserId = (System.Int64)dr["user_id"];
//            }
//            if (dr.Table.Columns.Contains("first_name") && dr["first_name"] != DBNull.Value)
//            {
//                entity.FirstName = dr["first_name"].ToString();
//            }
//            if (dr.Table.Columns.Contains("middle_name") && dr["middle_name"] != DBNull.Value)
//            {
//                entity.MiddleName = dr["middle_name"].ToString();
//            }
//            if (dr.Table.Columns.Contains("last_name") && dr["last_name"] != DBNull.Value)
//            {
//                entity.LastName = dr["last_name"].ToString();
//            }
//            if (dr.Table.Columns.Contains("facebook_id") && dr["facebook_id"] != DBNull.Value)
//            {
//                entity.FacebookID = dr["facebook_id"].ToString();
//            }
//            if (dr.Table.Columns.Contains("email") && dr["email"] != DBNull.Value)
//            {
//                entity.Email = dr["email"].ToString();
//            }
//            if (dr.Table.Columns.Contains("image") && dr["image"] != DBNull.Value)
//            {
//                entity.Image = dr["image"].ToString();
//            }
//            if (dr.Table.Columns.Contains("is_active") && dr["is_active"] != DBNull.Value)
//            {
//                entity.IsActive = Convert.ToBoolean(Convert.ToInt32(dr["is_active"]));
//            }
//            if (dr.Table.Columns.Contains("date_time") && dr["date_time"] != DBNull.Value)
//            {
//                entity.DateTime = (System.DateTime)dr["date_time"];
//            }
//            if (dr.Table.Columns.Contains("modified_date_time") && dr["modified_date_time"] != DBNull.Value)
//            {
//                entity.ModifiedDateTime = (System.DateTime)dr["modified_date_time"];
//            }
//            if (dr.Table.Columns.Contains("location") && dr["location"] != DBNull.Value)
//            {
//                entity.Location = dr["location"].ToString();
//            }
//            if (dr.Table.Columns.Contains("gender") && dr["gender"] != DBNull.Value)
//            {
//                entity.Gender = dr["gender"].ToString();
//            }
//            if (dr.Table.Columns.Contains("is_approve") && dr["is_approve"] != DBNull.Value)
//            {
//                entity.IsApprove = Convert.ToInt16(dr["is_approve"].ToString());
//            }
//            if (dr.Table.Columns.Contains("profile_link") && dr["profile_link"] != DBNull.Value)
//            {
//                entity.ProfileLink = dr["profile_link"].ToString();
//            }
//            if (dr.Table.Columns.Contains("work") && dr["work"] != DBNull.Value)
//            {
//                entity.Work = dr["work"].ToString();
//            }
//            if (dr.Table.Columns.Contains("about") && dr["about"] != DBNull.Value)
//            {
//                entity.About = dr["about"].ToString();
//            }
//            if (dr.Table.Columns.Contains("user_role_id") && dr["user_role_id"] != DBNull.Value)
//            {
//                entity.UserRoleId = Convert.ToInt32(dr["user_role_id"].ToString());
//            }
//            if (dr.Table.Columns.Contains("relationship_status_id"))
//            {
//                entity.RelationshipStatus = (RelationshipStatuses)dr["relationship_status_id"];
//            }
//            if (dr.Table.Columns.Contains("invite_friends_on_startup_flag") && dr["invite_friends_on_startup_flag"] != DBNull.Value)
//            {
//                entity.InviteFriendsOnStartupFlag = Convert.ToBoolean(Convert.ToInt32(dr["invite_friends_on_startup_flag"]));
//            }
//            if (dr.Table.Columns.Contains("invite_friends_showed") && dr["invite_friends_showed"] != DBNull.Value)
//            {
//                entity.InviteFriendsShowed = Convert.ToBoolean(Convert.ToInt32(dr["invite_friends_showed"]));
//            }
//            if (dr.Table.Columns.Contains("mobile_number") && dr["mobile_number"] != DBNull.Value)
//            {
//                entity.MobileNumber = dr["mobile_number"].ToString().Substring(0, 11);
//            }
//            //jugar
//            if (dr.Table.Columns.Contains("mobile_number") && dr["mobile_number"] != DBNull.Value)
//            {
//                entity.ReminderRegistartionId = Convert.ToInt32(dr["mobile_number"].ToString().Substring(12));
//            }
//            if (dr.Table.Columns.Contains("is_test_type") && dr["is_test_type"] != DBNull.Value)
//            {
//                entity.IsTestType = Convert.ToBoolean(Convert.ToInt32(dr["is_test_type"]));
//            }

//            return entity;
//        }

//        public EUser UpdateUser(EUser entity)
//        {
//            if (entity != null)
//            {
//                string sql = @"Update `user` set
//                                `first_name`=@first_name,
//                                `middle_name`=@middle_name,
//                                `last_name`=   @last_name,
//                                `facebook_id`=  @facebook_id,
//                                `email` =    @email,
//                                `image`   =  @image,
//                                `modified_date_time`= @modified_date_time,
//                                `popup_showed`  =   @popup_showed  ,
//                                `profile_link` =     @profile_link ,
//                                `gender`    =         @gender,
//                                `location`  =         @location,
//                                `work`      =         @work,
//                                `is_test_type`  =   @is_test_type,
//                                `about`=@about  where `user_id`=@user_id;";

//                MySqlParameter[] parameterArray = new MySqlParameter[]{
//                     new MySqlParameter("@user_id",   entity.UserId),
//                     new MySqlParameter("@first_name",  entity.FirstName ?? (object)DBNull.Value),
//                     new MySqlParameter("@middle_name",  entity.MiddleName ?? (object)DBNull.Value),
//                     new MySqlParameter("@last_name",  entity.LastName ?? (object)DBNull.Value),
//                     new MySqlParameter("@facebook_id",  entity.FacebookID ?? (object)DBNull.Value),
//                     new MySqlParameter("@email",        entity.Email ?? (object)DBNull.Value),
//                     new MySqlParameter("@image",        entity.Image ?? (object)DBNull.Value),
//                     //new MySqlParameter("@is_active",   IsApprove),
//                     //new MySqlParameter("@date_time",   DateTime.UtcNow),
//                     new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//                     new MySqlParameter("@popup_showed",  entity.PopupShowed),
//                     //new MySqlParameter("@user_role_id", 0),
//                     new MySqlParameter("@profile_link",        entity.ProfileLink ?? (object)DBNull.Value),
//                     //new MySqlParameter("@is_approve",   1),
//                      new MySqlParameter("@gender",        entity.Gender),
//                     new MySqlParameter("@location",   entity.Location),
//                     new MySqlParameter("@is_test_type", entity.IsTestType),
//                     new MySqlParameter("@about",        entity.About),
//                     new MySqlParameter("@work",   entity.Work ?? (object)DBNull.Value)
//                };

//                var identity = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameterArray);

//                if (identity < 1)
//                    throw new DataException("Identity column was null as a result of the insert operation.");
//                //return GetUser(entity.UserId);
//            }
//            return entity;
//        }


//        public void UpdateUserLogin(EUser entity)
//        {
//            string sql = @"Update `user` set
//                                `first_name`=@first_name,
//                                `middle_name`=@middle_name,
//                                `last_name`=   @last_name,
//                                `facebook_id`=  @facebook_id,
//                                `image`   =  @image,
//                                `modified_date_time`= @modified_date_time,                            
//                                `gender`    =         @gender,
//                                `location`  =         @location,
//                                `work`      =         @work,
//                                `about`=                @about 
//                                where `user_id`=@user_id;";

//            MySqlParameter[] parameterArray = new MySqlParameter[]{
//                     new MySqlParameter("@user_id",   entity.UserId),
//                     new MySqlParameter("@first_name",  entity.FirstName ?? (object)DBNull.Value),
//                     new MySqlParameter("@middle_name",  entity.MiddleName ?? (object)DBNull.Value),
//                     new MySqlParameter("@last_name",  entity.LastName ?? (object)DBNull.Value),
//                     new MySqlParameter("@facebook_id",  entity.FacebookID ?? (object)DBNull.Value),
//                     new MySqlParameter("@image",        entity.Image ?? (object)DBNull.Value),
//                     new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//                     new MySqlParameter("@gender",        entity.Gender),
//                     new MySqlParameter("@location",   entity.Location),
//                     new MySqlParameter("@about",        entity.About),
//                     new MySqlParameter("@work",   entity.Work ?? (object)DBNull.Value)
//                };
//            var identity = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameterArray);

//            if (identity < 1)
//                throw new DataException("Identity column was null as a result of the insert operation.");
//        }
//        public bool UpdateUser(long userId, string imageURL)
//        {
//            if (imageURL != null)
//            {
//                string sql = @"Update `user` set `image`   =  @image, `modified_date_time` = @modified_date_time  where `user_id`=@user_id;";
//                MySqlParameter[] parameterArray = new MySqlParameter[]{
//                    new MySqlParameter("@user_id",   userId),
//                     new MySqlParameter("@image",imageURL ?? (object)DBNull.Value),
//                     new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//                };

//                var identity = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameterArray);

//                if (identity > 0)
//                    return true;
//            }
//            return false;
//        }
//        public bool ValidatePowerUser(long userId)
//        {
//            bool flag = false;
//            string sql = "SELECT user_role_id FROM user where user_id=@userId;";
//            MySqlParameter[] parameter = new MySqlParameter[]{
//                     new MySqlParameter("@userId", userId)
//             };
//            var userRole = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, parameter);
//            if (int.Parse(userRole.Tables[0].Rows[0]["user_role_id"].ToString()) == 2)
//            {
//                flag = true;
//            }
//            return flag;
//        }

//        public bool UpdateInvitationBaseUser(long userId)
//        {
//            string sql = @"Update user
//                           set
//                                `is_approve`  = @InviteUserAproval,is_active=1,
//                                `modified_date_time` = @modified_date_time
//                           where 
//                                user_id = @UserId";

//            MySqlParameter[] parameters = new MySqlParameter[]{
//                new MySqlParameter("@UserId", userId),
//                new MySqlParameter("@InviteUserAproval", 2),
//                new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//            };

//            int retVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);
//            return retVal > 0;
//        }

//        public bool UpdateUserWithInvitationCode(int invitationCodeId, long userId)
//        {
//            string sql = @"Update user
//                           set
//                                `invitation_code_id`  = @invitation_code_id,
//                                `modified_date_time` = @modified_date_time
//                           where 
//                                user_id = @user_id";

//            MySqlParameter[] parameters = new MySqlParameter[]{
//                new MySqlParameter("@invitation_code_id", invitationCodeId),
//                new MySqlParameter("@user_id", userId),
//                new MySqlParameter("@modified_date_time",  DateTime.UtcNow),
//            };

//            int retVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);
//            return retVal > 0;
//        }

//        #region Automation Apis
//        //AQ Implementation
//        public EUser GetUserInfoByEmail(string userEmail)
//        {
//            EUser userEntity = new EUser();
//            string sql = "select * from user where `email`= @email;";
//            MySqlParameter[] parameters = new MySqlParameter[]{
//                new MySqlParameter("@email", userEmail)
//            };
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, parameters);
//            return ds?.Tables?.Count <= 0 ? null : UserFromDataRow(ds.Tables[0].Rows[0]);

//        }
//        public bool RemoveFriendsByIds(long userId, long friendId)
//        {

//            string sql = "Delete from `user_friend` WHERE `user_id`=@user_id and `friend_id`=@friend_id;Delete from `user_friend` WHERE `user_id`=@friend_id and `friend_id`=@user_id;";
//            MySqlParameter[] parameters = new MySqlParameter[]{
//                new MySqlParameter("@user_id", userId),
//                new MySqlParameter("@friend_id", friendId)
//            };

//            int retVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);

//            return retVal > 0;

//        }

//        public bool DeleteUserFriends(string email)
//        {
//            MySqlParameter[] parameters;
//            string query = "Select * from user_friend where user_id = (select user_id from user where email = @email);";
//            parameters = new MySqlParameter[]{
//                        new MySqlParameter("@email", email)
//                    };
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, query, parameters);

//            string sql;
//            int intVal;
//            foreach (DataTable record in ds.Tables)
//            {
//                foreach (DataRow dr in record.Rows)
//                {
//                    sql = "Delete FROM user_friend where user_id = @user_id and friend_id=@friend_id";
//                    parameters = new MySqlParameter[]{
//                        new MySqlParameter("@user_id", dr["friend_id"]),
//                        new MySqlParameter("@friend_id",dr["user_id"])
//                    };
//                    intVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);

//                }

//            }

//            sql = "Delete FROM user_friend where user_id = (select user_id from user where email = @email);";
//            parameters = new MySqlParameter[]{
//                        new MySqlParameter("@email", email)
//                    };
//            intVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);

//            return intVal > 0;
//        }
//        public bool DeleteUserFriends(int userId)
//        {
//            MySqlParameter[] parameters;
//            string query = "Select * from user_friend where user_id = @userId;";
//            parameters = new MySqlParameter[]{
//                        new MySqlParameter("@userId", userId)
//                    };
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, query, parameters);

//            string sql;
//            int intVal;
//            foreach (DataTable record in ds.Tables)
//            {
//                foreach (DataRow dr in record.Rows)
//                {
//                    sql = "Delete FROM user_friend where user_id = @user_id and friend_id=@friend_id";
//                    parameters = new MySqlParameter[]{
//                        new MySqlParameter("@user_id", dr["friend_id"]),
//                        new MySqlParameter("@friend_id",dr["user_id"])
//                    };
//                    intVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);

//                }

//            }

//            sql = "Delete FROM user_friend where user_id = @userId;";
//            parameters = new MySqlParameter[]{
//                        new MySqlParameter("@userId", userId)
//                    };
//            intVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);

//            return intVal > 0;
//        }
//        public bool ResetUser(string userEmail, bool flag, string dummyEmail, string dummyFacebookId)
//        {

//            if (flag)
//            {
//                string sql = "Update user set `email`=@updatedEmail, `facebook_id` =@dummyFacebookId where email = @email";
//                MySqlParameter[] parameters = new MySqlParameter[]{
//                    new MySqlParameter("@email", userEmail),
//                    new MySqlParameter("@updatedEmail", dummyEmail),
//                    new MySqlParameter("@dummyFacebookId", dummyFacebookId)
//                };
//                int intVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);
//                return intVal > 0;
//            }
//            else
//            {
//                string sql = "Update user set is_test_type=1 where email = @email";
//                MySqlParameter[] parameters = new MySqlParameter[]{
//                new MySqlParameter("@email", userEmail)
//                };
//                int intVal = MySqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sql, parameters);
//                return intVal > 0;
//            }

//        }
//        #endregion

//        public List<int> GetInternalUsers()
//        {
//            List<int> userIds = new List<int>();
//            string sql = "SELECT user_id FROM internal_user;";
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql);
//            if (ds?.Tables[0]?.Rows.Count > 0)
//            {
//                foreach (DataRow dr in ds.Tables[0].Rows)
//                {
//                    userIds.Add(Convert.ToInt32(dr["user_id"]));
//                }
//            }
//            return userIds;
//        }

//        public List<EUser> GetUsersById(List<int> userIds)
//        {
//            string csv = string.Join<int>(",", userIds);
//            string sql = @"SELECT email , first_name,middle_name,last_name from user where FIND_IN_SET(user_id,@UserId);";
//            MySqlParameter parameter = new MySqlParameter("@UserId", csv);
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, new MySqlParameter[] { parameter });
//            if (ds?.Tables[0]?.Rows.Count == 0) return null;
//            return CollectionFromDataSet<EUser>(ds, UserFromDataRow);
//        }

//        public List<EUser> GetUsersWithDetailById(List<int> userIds)
//        {
//            string csv = string.Join<int>(",", userIds);
//            string sql = @"select u.*,(SELECT CONCAT(r.mobile_number,'-',r.reminder_registration_id) FROM reminder_registration r WHERE r.user_id=u.user_id AND
//                (CASE WHEN r.is_active is NULL THEN 1 ELSE r.is_active END)=1 AND 
//                (CASE WHEN r.registration_status is NULL THEN 1 ELSE r.registration_status END)=1 limit 1 ) AS mobile_number
//                 from user u WHERE FIND_IN_SET(u.user_id,@UserId);";
//            MySqlParameter parameter = new MySqlParameter("@UserId", csv);
//            DataSet ds = MySqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql, new MySqlParameter[] { parameter });
//            if (ds?.Tables[0]?.Rows.Count > 0)
//                return CollectionFromDataSet<EUser>(ds, UserFromDataRow);
//            return null;

//        }
//    }
//}