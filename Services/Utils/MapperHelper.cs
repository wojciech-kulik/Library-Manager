using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utils
{
    internal static class MapperHelper
    {
        internal static void InitializeMappings()
        {
            Mapper.CreateMap<Model.Client, DB.Client>().ForMember("Lendings", opt => opt.Ignore());
            Mapper.CreateMap<DB.Client, Model.Client>().ForMember("Lendings", opt => opt.Ignore());
            Mapper.CreateMap<Model.BookCategory, DB.BookCategory>();
            Mapper.CreateMap<DB.BookCategory, Model.BookCategory>();
            Mapper.CreateMap<Model.Book, DB.Book>().ForMember("Authors", opt => opt.Ignore()).ForMember("Publisher", opt => opt.Ignore()).ForMember("BookCategories", opt => opt.Ignore());
            Mapper.CreateMap<DB.Book, Model.Book>();
            Mapper.CreateMap<DB.Employee, Model.Employee>().ForMember("Password", opt => opt.Ignore()).ForMember("Lendings", opt => opt.Ignore()).ForMember("Returns", opt => opt.Ignore());
            Mapper.CreateMap<Model.Employee, DB.Employee>().ForMember("Lendings", opt => opt.Ignore()).ForMember("Returns", opt => opt.Ignore());
            Mapper.CreateMap<Model.Lending, DB.Lending>().ForMember("Books", opt => opt.Ignore()).ForMember("Client", opt => opt.Ignore());
            Mapper.CreateMap<DB.Lending, Model.Lending>().ForMember("Books", opt => opt.Ignore()).ForMember("Client", opt => opt.Ignore());
            Mapper.CreateMap<Model.Address, DB.Address>();
            Mapper.CreateMap<DB.Address, Model.Address>();
            Mapper.CreateMap<Model.LentBook, DB.LentBook>().ForMember("Lending", opt => opt.Ignore());
            Mapper.CreateMap<DB.LentBook, Model.LentBook>().ForMember("Lending", opt => opt.Ignore());
            Mapper.CreateMap<DB.Person, Model.Person>();
            Mapper.CreateMap<Model.Person, DB.Person>();
            Mapper.CreateMap<Model.Author, DB.Author>();
            Mapper.CreateMap<DB.Author, Model.Author>();
            Mapper.CreateMap<Model.Publisher, DB.Publisher>();
            Mapper.CreateMap<DB.Publisher, Model.Publisher>();
        }
    }
}
