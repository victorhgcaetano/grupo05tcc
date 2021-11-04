using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginIntegracao
{
    public class ImportOpportunity : PluginImplementation
    {
        public string TableName = "opportunity";
        IOrganizationService service = ConnectionFactory.GetCrmServiceDynamics2();
        Entity opportunity = new Entity();

        public override void ExecutePlugin(IServiceProvider serviceProvider)
        {            
            if (this.Context.MessageName == "Create" || this.Context.MessageName == "Update")
            {
                opportunity = (Entity)this.Context.InputParameters["Target"];
                opportunity["jrcv_criadoviaplugin"] = "SIM"; // identificador de origem do registro                
                //EntityReference account = opportunity.Contains("parentaccountid") ? (EntityReference)opportunity["parentaccountid"] : null;
                //EntityReference contact = opportunity.Contains("parentcontactid") ? (EntityReference)opportunity["parentcontactid"] : null;
                //CreateUpdateReferences((EntityReference)opportunity["parentaccountid"], (EntityReference)opportunity["parentcontactid"]); // Verificar as referencias de Conta e Contato
                if (this.Context.MessageName == "Create")
                {
                    service.Create(opportunity);                    
                }
                else
                {
                    service.Update(opportunity);
                }
                opportunity["jrcv_criadoviaplugin"] = "";
                // retorna valores das referencias do Dynamics 1
                //opportunity["parentaccountid"] = account;
                //opportunity["parentcontactid"] = contact;
            }
            else
            {
                //PEGAR A OPORTUNIDADE PELA PRE-IMAGE
                opportunity = (Entity)this.Context.PreEntityImages["PreImage"];
                service.Delete(TableName, (Guid)RetrieveOpportunity()[0]["opportunityid"]);
            }    
        }

        private EntityCollection RetrieveOpportunity()
        {
            //FUNÇÃO PARA FAZER RETRIEVE DO ID DA OPORTUNIDADE NO DYNAMICS2 POR MEIO DO NOME DA OPORTUNIDADE
            QueryExpression queryRetrieveOpportunity = new QueryExpression(TableName);
            queryRetrieveOpportunity.ColumnSet.AddColumns("opportunityid");
            queryRetrieveOpportunity.Criteria.AddCondition("name", ConditionOperator.Equal, opportunity["name"].ToString());
            return service.RetrieveMultiple(queryRetrieveOpportunity);
        }

        private void CreateUpdateReferences(EntityReference account, EntityReference contact)
        {             
            // PESQUISAR SE NO DESTINO TEM A CONTA
            if(account.Equals(null))
            { 
                string[] accountColumns = { "accountid", "name", "jrcv_cnpj", "jrcv_porte", "jrcv_niveldocliente" };
                EntityCollection accountEntity = RetrieveData("account", (Guid)account.Id, "accountid", service, accountColumns);
                if (accountEntity.Entities.Count() == 0)
                {
                    // SE NÃO TEM, PEGA A CONTA DA ORIGEM E CRIA NO DESTINO
                    accountEntity = RetrieveData("account", (Guid)account.Id, "accountid", this.Service, accountColumns);
                    service.Create(accountEntity[0]);
                }
                //else
                //{
                //    // SE EXISTE NO DESTINO E FOR DIFERENTE DA ORIGEM, ALTERA O ID DA CONTA PARA O DO DESTINO
                //    if((Guid)accountEntity[0].Id != (Guid)account.Id)
                //    {
                //        opportunity["parentaccountid"] = new EntityReference("account", (Guid)accountEntity[0].Id);                    
                //    }                                  
                //}
            }
            // PESQUISAR SE NO DESTINO HÁ O CONTATO
            if (contact.Equals(null))
            {
                string[] contactColumns = { "contactid", "firstname", "lastname", "jrcv_cpf" };
                EntityCollection contactEntity = RetrieveData("contact", (Guid)contact.Id, "contactid", service, contactColumns);
                if (contactEntity.Entities.Count() == 0)
                {
                    contactEntity = RetrieveData("contact", (Guid)contact.Id, "contactid", this.Service, contactColumns);
                    service.Create(contactEntity[0]);
                }
                //else
                //{
                //    if ((Guid)contactEntity[0].Id != (Guid)account.Id) 
                //    {
                //        opportunity["parentcontactid"] = new EntityReference("contact", (Guid)contactEntity[0].Id);                    
                //    }
                //}                
            }
        }

        private EntityCollection RetrieveData(string Entity, Guid Id, string criteria, IOrganizationService Service, string[] columns)
        {
            QueryExpression queryRetrieveName = new QueryExpression(Entity);
            queryRetrieveName.ColumnSet.AddColumns(columns);
            queryRetrieveName.Criteria.AddCondition(criteria, ConditionOperator.Equal, Id);
            return Service.RetrieveMultiple(queryRetrieveName);
        }
    }
}