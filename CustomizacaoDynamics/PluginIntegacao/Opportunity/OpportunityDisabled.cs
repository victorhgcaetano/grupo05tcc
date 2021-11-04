using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class OpportunityDisabled : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity opportunity = (Entity)this.Context.InputParameters["Target"];
            if (Context.MessageName == "Update" || Context.MessageName == "Delete")
            {
                if (RetrieveOpportunityOrigin(this.Service).TotalRecordCount>0 && opportunity["jrcv_criadoviaplugin"].ToString() == "SIM")
                {
                    throw new InvalidPluginExecutionException("Você não tem autorização para realizar esta ação neste registro da tabela de Oportuniadades.");
                }
            }
        }

        public static EntityCollection RetrieveOpportunityOrigin(IOrganizationService service)
        {
            QueryExpression queryRetrieveName = new QueryExpression("opportunity");
            queryRetrieveName.ColumnSet.AddColumns("opportunityid");
            queryRetrieveName.Criteria.AddCondition("jrcv_criadoviaplugin", ConditionOperator.Equal, "SIM");
            return service.RetrieveMultiple(queryRetrieveName);
        }
    }
}
