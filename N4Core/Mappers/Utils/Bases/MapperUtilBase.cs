#nullable disable

using AutoMapper;
using N4Core.Records.Bases;

namespace N4Core.Mappers.Utils.Bases
{
    public abstract class MapperUtilBase<TEntity, TQueryModel, TCommandModel> where TEntity : Record, new() where TQueryModel : Record, new() where TCommandModel : Record, new()
    {
        public MapperConfiguration Configuration { get; protected set; }

        protected List<Profile> _profiles;

        protected MapperUtilBase()
        {
            Configuration = new MapperConfiguration(c =>
            {
                c.CreateMap(typeof(TEntity), typeof(TQueryModel));
                c.CreateMap(typeof(TCommandModel), typeof(TEntity));
                c.CreateMap(typeof(TEntity), typeof(TCommandModel));
            });
        }

        public void Set(params Profile[] profiles)
        {
            if (profiles is not null)
            {
                _profiles = profiles.ToList();
                Configuration = new MapperConfiguration(c =>
                {
                    c.CreateMap(typeof(TEntity), typeof(TQueryModel));
                    c.CreateMap(typeof(TCommandModel), typeof(TEntity));
                    c.CreateMap(typeof(TEntity), typeof(TCommandModel));
                    c.AddProfiles(_profiles);
                });
            }
        }

        public virtual TEntity Map(TCommandModel commandModel, TEntity entity = null)
        {
            MapperConfiguration configuration = new MapperConfiguration(c =>
            {
                c.CreateMap(commandModel.GetType(), typeof(TEntity));
                if (_profiles is not null)
                    c.AddProfiles(_profiles);
            });
            Mapper mapper = new Mapper(configuration);
            entity = entity ?? new TEntity();
            mapper.Map(commandModel, entity);
            return entity;
        }

        public virtual TCommandModel Map(TEntity entity)
        {
            MapperConfiguration configuration = new MapperConfiguration(c =>
            {
                c.CreateMap(entity.GetType(), typeof(TCommandModel));
                if (_profiles is not null)
                    c.AddProfiles(_profiles);
            });
            Mapper mapper = new Mapper(configuration);
            return mapper.Map<TCommandModel>(entity);
        }
    }
}
