using Notificacao.Domain.Model;
using System.Threading.Tasks;

namespace Notificacao.Domain.Interface
{
    public interface IParametroRepository
    {
        public Task<Parametro> GetParametroAsync(string tipo);
    }
}
