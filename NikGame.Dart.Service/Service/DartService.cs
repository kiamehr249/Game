using Microsoft.Extensions.Configuration;

namespace NikGame.Dart.Service
{
    public interface IDartService
    {
        IUserService iUserServ { get; set; }
        IDartMatchService iDartMatchServ { get; set; }
        IDartShootServicee iDartShootServ { get; set; }
        IDartMatchUserService iDartMatchUserServ { get; set; }
    }


    public class DartService : IDartService
    {
        public IUserService iUserServ { get; set; }
        public IDartMatchService iDartMatchServ { get; set; }
        public IDartShootServicee iDartShootServ { get; set; }
        public IDartMatchUserService iDartMatchUserServ { get; set; }

        public DartService(IConfiguration config)
        {
            IDartUnitOfWork uow = new DartDbContext(config.GetConnectionString("SystemBase"));
            iUserServ = new UserService(uow);
            iDartMatchServ = new DartMatchService(uow);
            iDartShootServ = new DartShootService(uow);
            iDartMatchUserServ = new DartMatchUserService(uow);
        }
    }
}
