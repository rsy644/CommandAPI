using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandAPI.Data;
using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandAPIRepo _repository;

        public CommandsController(ICommandAPIRepo repository)
        {
            _repository = repository;
        }

        [HttpGet] // responds to the GET verb
        public ActionResult<IEnumerable<Command>> GetAllCommands() // should return an enumeration of Command objects
        {
            var commandItems = _repository.GetAllCommands(); // we call GetAllCommands on our repository and populate a local variable with the result

            return Ok(commandItems); // return HTTP 200 Result and pass back our result set.
        }

        [HttpGet("{id}")] // route includes additional paramter, in this case Id of the resource we want to achieve.
        public ActionResult<Command> GetCommandById(int id) // requires an id to be passed in as a parameter.
        {
            var commandItem = _repository.GetCommandById(id);

            if(commandItem == null)
            {
                return NotFound();
            }
            return Ok(commandItem);
        }
    }
}
