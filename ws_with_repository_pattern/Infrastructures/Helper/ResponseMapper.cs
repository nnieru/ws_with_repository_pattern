using System.Net;
using AutoMapper;
using ws_with_repository_pattern.Response;

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

        public static BaseResponse<V> MapToBaseResponse(T source, HttpStatusCode statusCode, string message = "")
        {
            var mappedData = _mapper.Map<V>(source);
            return new BaseResponse<V>
            {
                StatusCode = statusCode,
                data = mappedData,
                message = message
            };
        }

        // Method to map a collection of type T to BaseResponse<IEnumerable<V>>
        public static BaseResponse<IEnumerable<V>> MapToBaseResponse(IEnumerable<T> source, HttpStatusCode statusCode, string message = "")
        {
            var mappedData = _mapper.Map<IEnumerable<V>>(source);
            return new BaseResponse<IEnumerable<V>>
            {
                StatusCode = statusCode,
                data = mappedData,
                message = message
            };
        }
    }
}