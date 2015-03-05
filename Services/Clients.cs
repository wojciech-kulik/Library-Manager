using AutoMapper;
using Common;
using Common.Exceptions;
using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Clients : DBTable, IClientsService
    {
        public Clients(string connectionString)
            : base(connectionString)
        {
        }

        public IList<Model.Client> GetAllClients()
        {
            using (var dataContext = GetDataContext())
                return Mapper.Map<List<Model.Client>>(dataContext.Persons.OfType<DB.Client>());
        }

        public Model.Client GetClient(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("clientId");

            using (var dataContext = GetDataContext())
            {
                var client = dataContext.Persons.OfType<DB.Client>().FirstOrDefault(c => c.Id == clientId);
                if (client == null)
                    throw new ClientNotFoundException();

                return Mapper.Map<Model.Client>(client);
            }
        }

        public void AddClient(Model.Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            using (var dataContext = GetDataContext())
            {
                dataContext.Persons.Add(Mapper.Map<DB.Client>(client));
                dataContext.SaveChanges();
            }
        }

        public void EditClient(Model.Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            using (var dataContext = GetDataContext())
            {
                DB.Client dbClient = dataContext.Persons.OfType<DB.Client>().FirstOrDefault(cl => cl.Id == client.Id);
                if (dbClient == null)
                    throw new ClientNotFoundException();

                Mapper.Map<Model.Client, DB.Client>(client, dbClient);
                dataContext.SaveChanges();
            }
        }

        public void DeleteClient(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("clientId");

            using (var dataContext = GetDataContext())
            {
                DB.Client client = dataContext.Persons.OfType<DB.Client>().FirstOrDefault(p => p.Id == clientId);
                if (client == null)
                    throw new ClientNotFoundException();

                foreach (var lending in client.Lendings.ToList())
                {
                    foreach (var lentBook in lending.Books.ToList())
                        dataContext.LentBooks.Remove(lentBook);

                    dataContext.Lendings.Remove(lending);
                }
                dataContext.Persons.Remove(client);
                dataContext.SaveChanges();
            }
        }
    }
}
