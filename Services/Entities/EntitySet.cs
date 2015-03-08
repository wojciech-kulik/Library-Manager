using AutoMapper;
using Common;
using Common.Exceptions;
using DB;
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

        public virtual int Add(TModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            using (var dataContext = GetDataContext())
            {
                var toAdd = Mapper.Map<TDBModel>(entity);
                BeforeAdd(dataContext, entity, toAdd);

                var newRecord = dataContext.Set<TDBModel>().Add(toAdd);
                dataContext.SaveChanges();

                return newRecord.Id;
            }
        }

        protected virtual void BeforeAdd(LibraryDataContext dataContext, TModel entity, TDBModel record)
        {
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
                BeforeUpdate(dataContext, entity, record);
                dataContext.SaveChanges();
            }
        }

        protected virtual void BeforeUpdate(LibraryDataContext dataContext, TModel entity, TDBModel record)
        {
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

        protected void UpdateCollection<ModelType, DBModelType>(LibraryDataContext dataContext, ICollection<ModelType> source, ICollection<DBModelType> destination)
            where ModelType : class, Model.IIdRecord
            where DBModelType : class, DB.IIdRecord
        {
            //removed items
            foreach (var oldItem in destination.ToList())
            {
                if (!source.Any(x => x.Id == oldItem.Id))
                {
                    destination.Remove(oldItem);
                }
            }

            //added items
            foreach (var newItem in source)
            {
                if (!destination.Any(x => x.Id == newItem.Id))
                {
                    var toAdd = dataContext.Set<DBModelType>().FirstOrDefault(x => x.Id == newItem.Id);
                    if (toAdd != null)
                    {
                        destination.Add(toAdd);
                    }
                }
            }
        }

        protected void MapCollection<ModelType, DBModelType>(LibraryDataContext dataContext, ICollection<ModelType> source, ICollection<DBModelType> destination)
            where DBModelType : class, DB.IIdRecord
            where ModelType : class, Model.IIdRecord
        {
            destination.Clear();
            foreach (var item in source)
            {
                destination.Add(dataContext.Set<DBModelType>().FirstOrDefault(x => x.Id == item.Id));
            }
        }
    }
}
