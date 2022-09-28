using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandAPI.Models; //bring in the namespace for our Models.

namespace CommandAPI.Data
{
    public interface ICommandAPIRepo // Specify a public interface and give it name starting with capital 'I'
    {
        bool SaveChanges();
        IEnumerable<Command> GetAllCommands(); // methods of the interface consumers can use to obtain and manipulate data.
        Command GetCommandById(int id);
        void CreateCommand(Command cmd);
        void UpdateCommand(Command cmd);
        void DeleteCommand(Command cmd);
    }
}
