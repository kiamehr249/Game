using NikGame.Service;

namespace NikGame.Dart.Service
{
    public interface IDartMatchUserService : IDataService<DartMatchUser>
    {
    }

    public class DartMatchUserService : DataService<DartMatchUser>, IDartMatchUserService
    {
        public DartMatchUserService(IDartUnitOfWork uow) : base(uow)
        {
        }
    }
}