using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class ProductDisabled : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            //string usuarioIntegracao = "d5686635-8a2e-ec11-b6e6-00224837b5f6";
            //string usuarioAtual = this.Context.OrganizationId.ToString();

            if (Context.MessageName == "Create" || Context.MessageName == "Update" || Context.MessageName == "Delete")
            {
                throw new InvalidPluginExecutionException("Você não tem autorização para realizar ações na tabela de Produtos.");
            }
        }
    }
}
