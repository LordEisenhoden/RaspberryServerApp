using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryServerApp
{
    class User
    {
        public enum UserType
        {
            User,
            Admin
        }

        public UserType user;

        public void Test()
        {
            user = UserType.Admin;
        }


    }
}
