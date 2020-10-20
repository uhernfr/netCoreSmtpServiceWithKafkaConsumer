using MongoDB.Bson;

namespace Notificacao.Domain.Model
{
    public class Parametro : Entity
    {
        public string Tipo { get; set; }
        public string Nome { get; set; }
        public string Instituto { get; set; }
        public string Descricao { get; set; }
        public string Valor { get; set; }
        public BsonDocument ValorObj { get; set; }
        public bool Ativo { get; set; }
        public string MotivoInativacao { get; set; }
        //[BsonIgnore]
        //public TipoParametroEnum TipoParametroEnum
        //{
        //    get { return EnumExtension.GetEnumerator<TipoParametroEnum>(TipoParametro?.Trim()); }
        //    set { TipoParametro = value.GetDescription(); }
        //}
    }
}
