using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao.Opportunity
{
    public class CreateNotification : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity opportunity = (Entity)this.Context.InputParameters["Target"];
            string status = opportunity.Contains("statecode") ? ((OptionSetValue)opportunity["statecode"]).Value.ToString() : string.Empty;
            //RETRIEVE DO STATUS DA OPORTUNIDADE

            if (status == "1")
            {
                Entity preOpportunityImage = Context.PreEntityImages["PreImage"];
                Guid parentAccountId = ((EntityReference)preOpportunityImage["parentaccountid"]).Id;
                //RETRIEVE DO ID DA CONTA DA OPORTUNIDADE PARA USAR NO CAMPO DE ENTITY REFRENCE

                Entity notification = new Entity("jrcv_notificacaodocliente");
                notification["jrcv_conta"] = new EntityReference("account", parentAccountId);
                notification["jrcv_mensagem"] = "OBRIGADO POR CONTRATAR NOSSOS SERVIÇOS!!!";
                notification["jrcv_datadanotificacao"] = DateTime.Now;

                Service.Create(notification);

            }
        }
    }
}