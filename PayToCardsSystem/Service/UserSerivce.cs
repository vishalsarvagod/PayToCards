using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayToCardsSystem.AppCode.CommonCode;

namespace PayToCardsSystem.Service
{
    public class UserService
    {
        vmpaytocardsEntities dbContext = new vmpaytocardsEntities();
        public user_personal_details checkLoginDetails(string userName, string password)
        {
            try
            {
                user_personal_details userPersonalDetails = dbContext.user_personal_details.FirstOrDefault(i => i.UserName == userName && i.Password == password && i.IsActive == true);
                return userPersonalDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsAllowedIPAddress(user_personal_details userPersonalDetails, string ip)
        {

            if (string.IsNullOrEmpty(userPersonalDetails.AllowIPAddress) && string.IsNullOrEmpty(userPersonalDetails.BlockIPAddress))
            {
                return true;
            }
            else if (string.IsNullOrEmpty(userPersonalDetails.AllowIPAddress) && !string.IsNullOrEmpty(userPersonalDetails.BlockIPAddress))
            {
                string[] blockIP = userPersonalDetails.BlockIPAddress.Split(',');
                foreach (string item in blockIP)
                {
                    if (item == ip)
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (!string.IsNullOrEmpty(userPersonalDetails.AllowIPAddress))
            {
                string[] allowIP = userPersonalDetails.AllowIPAddress.Split(',');
                foreach (string item in allowIP)
                {
                    if (item == ip)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public List<SelectListItem> GetAllUserRoleWiseIncludeLogin(user_personal_details userDetails)
        {
            try
            {
                List<SelectListItem> selectUser = new List<SelectListItem>();
                if (userDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where user.C_id > 3 && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                else if (userDetails.Role.Equals(Constant.ROLE_ADMIN))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where (user.AdminId == userDetails.C_id || user.C_id == userDetails.C_id) && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                else if (userDetails.Role.Equals(Constant.ROLE_DISTRIBUTOR))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where (user.DistributorID == userDetails.C_id || user.C_id == userDetails.C_id) && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                else if (userDetails.Role.Equals(Constant.ROLE_MERCHANT))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where (user.C_id == userDetails.C_id || user.C_id == userDetails.C_id) && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                return selectUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SelectListItem> GetAllUserRoleWise(user_personal_details userDetails)
        {
            try
            {
                List<SelectListItem> selectUser = new List<SelectListItem>();
                if (userDetails.Role.Equals(Constant.ROLE_SUPER_ADMIN))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where user.C_id > 3 && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                else if (userDetails.Role.Equals(Constant.ROLE_ADMIN))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where user.AdminId == userDetails.C_id && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                else if (userDetails.Role.Equals(Constant.ROLE_DISTRIBUTOR))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where user.DistributorID == userDetails.C_id && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                else if (userDetails.Role.Equals(Constant.ROLE_MERCHANT))
                {
                    var usersObject = from user in dbContext.user_personal_details
                                      where user.C_id == userDetails.C_id && user.IsActive == true
                                      orderby user.FirstName
                                      select new { user.C_id, user.FirstName, user.LastName };
                    int i = 0;
                    foreach (var item in usersObject)
                    {
                        i++;
                        selectUser.Add(new SelectListItem { Text = item.FirstName + " " + item.LastName, Value = (item.C_id).ToString() });
                    }
                }
                return selectUser;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string generateApiPassword(int passwordLength)
        {
            try
            {
                string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
                char[] chars = new char[passwordLength];
                Random rd = new Random();

                for (int i = 0; i < passwordLength; i++)
                {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                }

                string password = new string(chars);


                user_personal_details transCard = dbContext.user_personal_details.FirstOrDefault(i => i.ApiPassword == password);
                if (transCard != null)
                    password = generateApiPassword(passwordLength);
                return new string(chars);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}