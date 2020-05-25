using PayToCardsSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayToCardsSystem.Controllers.Api
{
    public class UsersController : ApiController
    {
        private vmpaytocardsEntities _context;
        public UsersController()
        {
            _context = new vmpaytocardsEntities();
        }
        //public IEnumerable<ManageUserModel> GetUsers(string query = null)
        //{
        //    //var moviesQuery = _context.user_personal_details
        //    //    .Include(m => m.Genre)
        //    //    .Where(m => m.NumberAvailable > 0);

        //    //if (!String.IsNullOrWhiteSpace(query))
        //    //    moviesQuery = moviesQuery.Where(m => m.Name.Contains(query));

        //    //return moviesQuery
        //    //    .ToList()
        //    //    .Select();
        //}

    }
}
