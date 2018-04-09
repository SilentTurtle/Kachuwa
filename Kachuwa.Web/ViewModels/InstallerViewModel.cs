using System;
using System.Text;

namespace Kachuwa.Web.ViewModels
{
    public class InstallerViewModel
    {
        public string AppName { get; set; }
        public string DbServerType { get; set; }
        public string DbServer { get; set; }
        public string DbUserName { get; set; }
        public string DbPassword { get; set; }
        public string ConnectionString { get; set; }


        public string Email { get; set; }
        public string Password { get; set; }
    }
}
