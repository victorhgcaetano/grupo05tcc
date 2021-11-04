using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao.Contact
{
    public class CheckDuplicateCpf : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity contact = (Entity)this.Context.InputParameters["Target"];
            string cpf = contact.Contains("jrcv_cpf") ? contact["jrcv_cpf"].ToString() : string.Empty;

            Entity entidadeContexto = null;

            if (Context.InputParameters.Contains("Target"))
                entidadeContexto = (Entity)Context.InputParameters["Target"];



            if (entidadeContexto.LogicalName == "contact")
            {
                var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='contact'>
                            <attribute name='fullname' />
                            <attribute name='telephone1' />
                            <attribute name='contactid' />
                            <order attribute='fullname' descending='false' />
                            <filter type='and'>
                               <condition attribute='jrcv_cpf' operator='eq' value='{entidadeContexto["jrcv_cpf"]}' />
                              </filter>
                           </entity>
                         </fetch>";

                var Retorno = this.Service.RetrieveMultiple(new FetchExpression(fetch));

                if (Retorno.Entities.Count > 0)
                {
                    throw new InvalidPluginExecutionException("CPF JÁ CADASTRADO!!");
                }
            }
        }
    }
}