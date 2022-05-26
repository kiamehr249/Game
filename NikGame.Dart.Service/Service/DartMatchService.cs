using NikGame.Service;

namespace NikGame.Dart.Service
{
    public interface IDartMatchService : IDataService<DartMatch>
    {
    }

    public class DartMatchService : DataService<DartMatch>, IDartMatchService
    {
        public DartMatchService(IDartUnitOfWork uow) : base(uow)
        {
        }
    }
}