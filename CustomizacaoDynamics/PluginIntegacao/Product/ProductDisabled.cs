using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
            if (Context.MessageName == "Create" || Context.MessageName == "Update" || Context.MessageName == "Delete")
            {
                throw new InvalidPluginExecutionException("Você não tem autorização para realizar ações na tabela de Produtos.");
            }
        }
    }
        
}
