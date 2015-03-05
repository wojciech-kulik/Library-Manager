using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IClientsService
    {
        IList<Client> GetAllClients();

        Client GetClient(int clientId);

        void DeleteClient(int clientId);

        void AddClient(Client client);

        void EditClient(Client client);
    }
}
