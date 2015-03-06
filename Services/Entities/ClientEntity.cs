using AutoMapper;
using Common;
using Common.Exceptions;
using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Entities
{
    public class ClientEntity : DBEntity<Model.Client, DB.Client>
    {
        public ClientEntity(string connectionString)
            : base(connectionString)
        {
        }

        public override void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("id");

            using (var dataContext = GetDataContext())
            {
                DB.Client client = dataContext.Persons.OfType<DB.Client>().FirstOrDefault(p => p.Id == id);
                if (client == null)
                    throw new RecordNotFoundException();

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
