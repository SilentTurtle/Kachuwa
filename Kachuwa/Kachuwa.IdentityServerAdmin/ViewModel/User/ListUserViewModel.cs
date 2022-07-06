using System.Collections.Generic;
using Kachuwa.IdentityServerAdmin.ViewModel;

namespace Kachuwa.IdentityServerAdmin.ViewModel
{
    public class ListUserViewModel
    {
        public List<ListUserItemViewModel> Users { get; set; }

        public string Keyword { get; set; }
    }
}