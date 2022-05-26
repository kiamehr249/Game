using NikGame.Service;

namespace NikGame.Dart.Service
{
    public interface IUserService : IDataService<AppUser>
    {
    }

    public class UserService : DataService<AppUser>, IUserService
    {
        public UserService(IDartUnitOfWork uow) : base(uow)
        {
        }
    }
}