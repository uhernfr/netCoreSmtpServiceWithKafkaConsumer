
using AutoMapper;
using Notificacao.Domain.Interface;
using Notificacao.Domain.Model;
using System.Threading.Tasks;

namespace Notificacao.Infrastructure.Service
{
    public class ParametroService
    {
        private readonly IMapper _mapper;
        private readonly IParametroRepository _parametroRepository;

        public ParametroService(IMapper mapper, IParametroRepository parametroRepository)
        {
            _mapper = mapper;
            _parametroRepository = parametroRepository;
        }

        public async Task<Parametro> GetByIdAsync(string tipo)
        {            
            var param = await _parametroRepository.GetParametroAsync(tipo);          
            return param;
        }


    }
}