using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandAPI.Data;
using CommandAPI.Models;
using CommandAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        //Random change
        private readonly ICommandAPIRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandAPIRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet] // responds to the GET verb
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands() // should return an enumeration of Command objects
        {
            var commandItems = _repository.GetAllCommands(); // we call GetAllCommands on our repository and populate a local variable with the result

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems)); // return HTTP 200 Result and pass back our result set.
            // Maps our collection of Command objects to an IEnumerable of CommandReadDtos that are returned in our OK method.
        }

        [HttpGet("{id}", Name = "GetCommandById")] // route includes additional paramter, in this case Id of the resource we want to achieve.
        public ActionResult<CommandReadDto> GetCommandById(int id) // requires an id to be passed in as a parameter.
        {
            var commandItem = _repository.GetCommandById(id);

            if (commandItem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem)); // returning a single CommandReadDto object in our OK method.
        }

        [HttpPost] // will respond to the class-wide route of 'api/commands' with the POST verb, which 
        // in combination makes this unique to this controller.
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto) // expects 'commandCreateDto' as input. When using postman, we'll place a 'commandCreateDto' in the body of our request.'
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto); // we take our input from commandCreateDto and map it to a newly created Command object.
            _repository.CreateCommand(commandModel); // we take commandModel and pass it to CreateCommand method of our repository.
            _repository.SaveChanges(); // this persists the changes down to our PostgreSQLDB.

            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel); // we create a commandReadDto. We have said we need to pass this back as part of our endpoint specification.

            return CreatedAtRoute(nameof(GetCommandById), // we specify the route name, the id of the resource and the content value of the body returned.
                new { Id = commandReadDto.Id }, commandReadDto);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(commandUpdateDto, commandModelFromRepo);

            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc) // expects a JsonPatchDocument in request body that applies to a CommandUpdate data transfer object.
        {
            var commandModelFromRepo = _repository.GetCommandById(id); // Attempts command resource retrieval.
            if(commandModelFromRepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            // We need to create a placeholder command update dto.

            patchDoc.ApplyTo(commandToPatch, ModelState);
            // we apply the patch document to the newly created command update data transfer object.

            if(!TryValidateModel(commandToPatch)) // picks up any validation problems with our input.
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandModelFromRepo); // we use automapper to map the commandUpdateDto back to our Command object.

            _repository.UpdateCommand(commandModelFromRepo); // does nothing at the moment.

            _repository.SaveChanges();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if(commandModelFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeleteCommand(commandModelFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }
    }
}
