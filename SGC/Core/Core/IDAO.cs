using Core.Aplicacao;
using Dominio;
using System.Collections.Generic;

namespace Core.Core
{
    public interface IDAO
    {
        void Inserir(EntidadeDominio entidade);
        void Atualizar(EntidadeDominio entidade);
        List<IEntidade> Consultar(IEntidade entidade);
        void Excluir(EntidadeDominio entidade);
    }
}
