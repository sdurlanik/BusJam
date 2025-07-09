using Sdurlanik.BusJam.Models;

namespace Sdurlanik.BusJam.MVC.Controllers
{
    public interface ILevelController
    {
        void LoadLevel(LevelSO levelData);
    }
}