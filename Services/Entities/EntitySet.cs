using AutoMapper;
using Common;
using Common.Exceptions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Entities
{
    public class EntitySet<TModel, TDBModel> : DBTable, IEntitySet<TModel>
        where TModel : class, Model.IIdRecord
        where TDBModel : class, DB.IIdRecord
    {
        public EntitySet(string connectionString)
            : base(connectionString)
        {
        }

        public virtual IList<TModel> GetAll()
        {
            using (var dataContext = GetDataContext())
                return Mapper.Map<List<TModel>>(dataContext.Set<TDBModel>().ToList());
        }

        public virtual TModel Get(int id)
        {
            if (id <= 0)
                throw new ArgumentException("id");

            using (var dataContext = GetDataContext())
            {
                var record = dataContext.Set<TDBModel>().FirstOrDefault(c => c.Id == id);
                if (record == null)
                    throw new RecordNotFoundException();

                return Mapper.Map<TModel>(record);
            }
        }

        public virtual void Add(TModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            using (var dataContext = GetDataContext())
            {
                dataContext.Set<TDBModel>().Add(Mapper.Map<TDBModel>(entity));
                dataContext.SaveChanges();
            }
        }

        public virtual void Update(TModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            using (var dataContext = GetDataContext())
            {
                TDBModel record = dataContext.Set<TDBModel>().FirstOrDefault(x => x.Id == entity.Id);
                if (record == null)
                    throw new RecordNotFoundException();

                Mapper.Map<TModel, TDBModel>(entity, record);
                dataContext.SaveChanges();
            }
        }

        public virtual void Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("id");

            using (var dataContext = GetDataContext())
            {
                TDBModel record = dataContext.Set<TDBModel>().FirstOrDefault(x => x.Id == id);
                if (record == null)
                    throw new RecordNotFoundException();

                dataContext.Set<TDBModel>().Remove(record);
                dataContext.SaveChanges();
            }
        }
    }
}
