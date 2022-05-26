using NikGame.Service;

namespace NikGame.Dart.Service
{
    public interface IDartShootServicee : IDataService<DartShoot>
    {
    }

    public class DartShootService : DataService<DartShoot>, IDartShootServicee
    {
        public DartShootService(IDartUnitOfWork uow) : base(uow)
        {
        }
    }
}