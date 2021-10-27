using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class OpportunityCustom : PluginImplementation
    {
        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {
            Entity opportunity = GetOpportunityEntity(this.Context);
            //Entity opportunity = (Entity)this.Context.InputParameters["Target"];
            //opportunity["jrcv_codigodaoportunidade"] = GetNextCode(this.Service);   
            SetCode(this.Context, this.Service, opportunity);

        }

        private static void SetCode(IPluginExecutionContext context, IOrganizationService service, Entity opportunity)
        {
            if (context.MessageName == "Create")
            {
                opportunity["jrcv_codigodaoportunidade"] = GetNextCode(service);
            }
        }

        private static string GetNextCode(IOrganizationService service)
        {
            string code = "OPP-";
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            do
            {
                code += GenetateRandomCode(numbers, 5) + "-";
                code += GenetateRandomCode(letters, 1) + GenetateRandomCode(numbers, 1);
                code += GenetateRandomCode(letters, 1) + GenetateRandomCode(numbers, 1);
            } while (RetrieveOpportunityCode(service, code));            
            return code;            
        }

        private static string GenetateRandomCode(string charSet, int length)
        {
            var random = new Random();
            var array = new char[length];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = charSet[random.Next(charSet.Length)];
            }

            return new String(array);
        }

        private static Boolean RetrieveOpportunityCode(IOrganizationService service, string code)
        {
            QueryExpression query = new QueryExpression("opportunity");
            query.ColumnSet.AddColumns("jrcv_codigodaoportunidade");
            query.Criteria.AddCondition("jrcv_codigodaoportunidade", ConditionOperator.Equal, code);
            return service.RetrieveMultiple(query).TotalRecordCount >0 ? true : false;
        }

        private static Entity GetOpportunityEntity(IPluginExecutionContext context)
        {
            Entity opportunity = new Entity();

            if (context.MessageName == "Create" || context.MessageName == "Update")
            {
                opportunity = (Entity)context.InputParameters["Target"];
            }
            else
            {
                if (context.MessageName == "Delete")
                {
                    opportunity = (Entity)context.PreEntityImages["PreImage"];
                }
            }
            return opportunity;
        }
    }
}