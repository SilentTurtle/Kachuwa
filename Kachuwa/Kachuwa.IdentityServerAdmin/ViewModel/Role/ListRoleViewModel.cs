using X.PagedList;

namespace Kachuwa.IdentityServerAdmin.ViewModel
{
    public class ListRoleViewModel
    {
        public IPagedList<ListRoleItemViewModel> Roles { get; set; }

        public string Keyword { get; set; }
    }
}