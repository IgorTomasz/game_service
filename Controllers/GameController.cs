using game_service.classes;
using Microsoft.AspNetCore.Mvc;

namespace game_service.Controllers
{
    [ApiController]
    [Route("game/[controller]")]
    public class GameController : ControllerBase
    {


        private readonly AbstractGameFactory _gameFactory;

        public GameController(AbstractGameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }


    }
}
