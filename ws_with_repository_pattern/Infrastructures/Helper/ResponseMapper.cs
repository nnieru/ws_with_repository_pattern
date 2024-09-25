using AutoMapper;

namespace ws_with_repository_pattern.Infrastructures.Helper
{
    public static class ResponseMapper<T, V>
    {
        private static readonly IMapper _mapper;

        // Static constructor to initialize the mapper
        static ResponseMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T, V>();
            });

            _mapper = config.CreateMapper();
        }

        // Method to map an object of type T to V
        public static V Map(T source)
        {
            return _mapper.Map<V>(source);
        }

        // Method to map a collection of type T to a collection of V
        public static IEnumerable<V> Map(IEnumerable<T> source)
        {
            return _mapper.Map<IEnumerable<V>>(source);
        }
    }
}